using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Mediation;
using Unity.Services.Core;
using UnityEngine.Analytics;

using LitJson;

#if UNITY_IOS
using UnityEngine.iOS;
using Unity.Advertisement.IosSupport;
#endif




namespace PIERStory {

    public class AdManager : MonoBehaviour
    {
        public static AdManager main = null;
        
        public static Action OnShowAdvertisement;
        
        public static Action<bool> OnCompleteRewardAD = null; // 동영상 광고 보고 콜백
        
        // * 광고 도중 터치 막기 위한 변수.
        public bool isAdShowing = false; // 현재 광고가 보여지고 있다.
        
        public string rewardedAdUnitId = "Rewarded_Android";
        public string interstitialAdUnitId = "Interstitial_Android";
        
        [SerializeField] int gamePlayRowCount = 0;
        [SerializeField] bool isRewarded = false; // 영상광고 끝까지 재생되었는지. 
        
        [Space]
        [Header("IronSource")] 
        [SerializeField] string ironSource_android = string.Empty;
        [SerializeField] string ironSource_ios = string.Empty;
        [SerializeField] string ironSourceKey = string.Empty;
        public bool isIronSourceBannerLoad = false;
        
        JsonData serverData = null;
        
        IRewardedAd rewardedAd;
        IInterstitialAd interstitialAd;
        
        #region 서버 광고 기준정보
        
        public bool useLoadingAD = false; // 에피소드 로딩 광고 사용여부
        int shareLoadingInterstitial = 0; // 에피소드 로딩 전면광고 점유율
        int shareLoadingRewarded = 0; // 에피소드 로딩 동영상 광고 점유율
        
        public bool useBannerAD = false; // 띠배너 광고 사용여부
        public bool usePlayAD = false; // 플레이 도중 광고 사용여부
        [SerializeField] int sharePlayInterstitial = 0; // 게임플레이 전면광고 점유율
        [SerializeField] int sharePlayRewarded = 0; // 게임플레이 동영상 광고 점유율
        [SerializeField] int ratePlayAD = 0; // 플레이 광고 재생 확률
        [SerializeField] int lineOfPlayAD = 0; // 플레이 광고 실행에 필요한 스크립트 라인 수
        
        public bool useSelectionAD = false; // 선택지 광고 사용여부
        int shareSelectionInterstitial = 0; // 선택지 전면광고 점유율
        int shareSelectionRewarded = 0; // 선택지 동영상 광고 점유율 
        public bool useDoubleRewardAD = false; // 광고보고 2배 받기 사용여부 
        int shareDoubleInterstitial = 0; // 2배 보상 전면광고 점유율
        int shareDoubleRewarded = 0; // 2배 보상 동영상 광고 점유율 
        
        #endregion
        
        private void Awake() {
            if(main != null) {
                Destroy(this.gameObject);
                return;
            }
            
            main = this;
            DontDestroyOnLoad(this);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            // iOS 추적 권한 요청 
            RequestAuthorizationTracking(); 
            

            InitUnityMediation();
            
            InitIronSource();
        }
        
        /// <summary>
        /// 추적 권한 요청 
        /// </summary>
        void RequestAuthorizationTracking() {
            if(Application.isEditor)
                return;
            
#if UNITY_IOS

            // check with iOS to see if the user has accepted or declined tracking
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            Version currentVersion = new Version(Device.systemVersion); 
            Version ios14 = new Version("14.5"); 
            
            Debug.Log(string.Format("### ATTrackingStatusBinding {0}/{1}", status.ToString(), ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED.ToString()));
            Debug.Log(currentVersion.ToString());
           
            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED && currentVersion >= ios14)
            {
                // 호출 
                ATTrackingStatusBinding.RequestAuthorizationTracking(AuthorizationTrackingReceived);
            }
#endif        
    
        }
        
        private void AuthorizationTrackingReceived(int status) {
             Debug.LogFormat("Tracking status received: {0}", status);

        }
        
        
        
        /// <summary>
        /// 전면에 등장하는 영상 광고, 전면광고 On/Off 에 대한 상태 처리 
        /// </summary>
        /// <param name="__flag"></param>
        public void SetFrontAdStatus(bool __flag) {
            isAdShowing = __flag;
            
            
            // 추가 로직
            // 광고의 실행, 종료에 따라 영향 받는 부분 로직
            if(GameManager.main != null)
            {
                
                if(__flag)
                {
                    // 자동 재생 중이라면
                    if (GameManager.main.isAutoPlay)
                        GameManager.main.StopAutoPlay();

                    foreach (GameSoundCtrl sc in GameManager.main.SoundGroup)
                        sc.PlayAudioClip(false);
                }
                else
                {
                    if (GameManager.main.isAutoPlay)
                        GameManager.main.StartAutoPlay();

                    if (!GameManager.main.isPlaying)
                        return;

                    foreach (GameSoundCtrl sc in GameManager.main.SoundGroup)
                        sc.PlayAudioClip(true);
                }


                if (GameManager.main.isAutoPlay)
                {
                    if(__flag)
                        GameManager.main.StopAutoPlay();
                    else
                        GameManager.main.StartAutoPlay();
                }



            }
        }
        
        
        #region ironSource
        /// <summary>
        /// 아이언 소스 
        /// </summary>
        void InitIronSource() {
            
            
            
            #if UNITY_ANDROID
            ironSourceKey = ironSource_android;
            #else
            ironSourceKey = ironSource_ios;
            #endif
            
            IronSource.Agent.init(ironSourceKey);
            IronSource.Agent.validateIntegration();
            
            InitBanner();
        }
        
        /// <summary>
        /// 배너 초기화
        /// </summary>
        void InitBanner() {
            IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;        
            IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent; 
        }
        
        /// <summary>
        /// 띠배너 불러오기
        /// </summary>
        public void LoadBanner() {
            
            if(!useBannerAD) {
                Debug.Log("Off Banner AD");
                return;
            }


            // 무료 유저가 아니면 광고 재생 되지 않음
            if(GameManager.main != null && GameManager.main.currentEpisodeData.purchaseState != PurchaseState.AD)
                return;            
            
            isIronSourceBannerLoad = false;
            
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
            
            
            
        }
        
        /// <summary>
        /// 띠배너 감추기
        /// </summary>
        public void HideBanner() {
            
            if(!isIronSourceBannerLoad)
                return;
                
            if(!useBannerAD)
                return;
                
            IronSource.Agent.hideBanner();
        }
        
        void BannerAdLoadedEvent() {
            Debug.Log("#### ironSourceBanner Loaded");
            isIronSourceBannerLoad = true;
        }
        
        void BannerAdLoadFailedEvent(IronSourceError error) {
            Debug.Log("#### ironSource bannder fail : " + error.getDescription());
        }
        
        void BannerAdClickedEvent() {
            
        }
        
        #endregion
        
        
        /// <summary>
        /// 유니티 메디에이션 초기화 
        /// </summary>
        /// <returns></returns>
        async void InitUnityMediation() {
            // Initialize package to access API
            await UnityServices.InitializeAsync();
            
            
            
            // 스테이트 체크 
            Debug.Log(UnityServices.State);

            #if UNITY_ANDROID
            rewardedAdUnitId = "Rewarded_Android";
            interstitialAdUnitId = "Interstitial_Android";
            #else
            rewardedAdUnitId = "Rewarded_iOS";
            interstitialAdUnitId = "Interstitial_iOS";            
            #endif            
            

            CreateRewardAd();
            CreateInterstitial();
        }
        
        /// <summary>
        /// 서버 기준정보 세팅하기
        /// </summary>
        /// <param name="__j"></param>
        public void SetServerAdInfo(JsonData __j) {
            serverData = __j;   
            
            useLoadingAD = SystemManager.GetJsonNodeBool(serverData, "loading_is_active");
            useBannerAD = SystemManager.GetJsonNodeBool(serverData, "banner_is_active");
            usePlayAD = SystemManager.GetJsonNodeBool(serverData, "play_is_active");
            useSelectionAD = SystemManager.GetJsonNodeBool(serverData, "selection_is_active");
            useDoubleRewardAD = SystemManager.GetJsonNodeBool(serverData, "reward_is_active");
            
            shareLoadingInterstitial = SystemManager.GetJsonNodeInt(serverData, "loading_interstitial");
            shareLoadingRewarded = SystemManager.GetJsonNodeInt(serverData, "loading_rewarded");
            
            sharePlayInterstitial = SystemManager.GetJsonNodeInt(serverData, "play_interstitial");
            sharePlayRewarded = SystemManager.GetJsonNodeInt(serverData, "play_rewarded");
            lineOfPlayAD = SystemManager.GetJsonNodeInt(serverData, "play_line");
            ratePlayAD = SystemManager.GetJsonNodeInt(serverData, "play_percent");
            
            
            shareSelectionInterstitial = SystemManager.GetJsonNodeInt(serverData, "selection_interstitial");
            shareSelectionRewarded = SystemManager.GetJsonNodeInt(serverData, "selection_rewarded");
            
            shareDoubleInterstitial = SystemManager.GetJsonNodeInt(serverData, "reward_interstitial");
            shareDoubleRewarded = SystemManager.GetJsonNodeInt(serverData, "reward_rewarded");
            
        }
        
        
        /// <summary>
        /// 광고 보여주기 전, 3초 마음의 준비하기 
        /// </summary>
        /// <param name="__isRewarded">동영상 광고</param>
        void ShowAdvertisementReady(bool __isRewarded) {
            if(__isRewarded)
                OnShowAdvertisement = ShowRewardAd;
            else 
                OnShowAdvertisement = ShowInterstitial;
                
            
            PopupBase p = PopupManager.main.GetPopup("AdvertisementShow");
            if(p == null) {
                Debug.LogError("AdvertisementShow ");
            }
            
            PopupManager.main.ShowPopup(p, false, false);
            
        }
        
        
        #region 선택지 선택 후 광고 처리
        
        /// <summary>
        /// 선택지 선택 후 광고
        /// </summary>
        public void PlaySelectionAD() {
            if(!useSelectionAD) 
                return;
                
            // 무료 유저가 아니면 광고 재생 되지 않음
            if(GameManager.main != null && GameManager.main.currentEpisodeData.purchaseState != PurchaseState.AD)
                return;
                
                
            ShowSelectionAD();
        }
        
        
        /// <summary>
        /// 선택지 선택 후 광고 보여주기 
        /// </summary>
        void ShowSelectionAD() {
            if(UnityEngine.Random.Range(0, 100) < shareSelectionInterstitial)
                ShowAdvertisementReady(false);
            else 
                ShowAdvertisementReady(true);
                
            InitGamePlayRowCount(); // 선택지 선택하면 플레이 카운트 초기화
        }
        
        #endregion
        
        #region 에피소드 로딩 광고 처리 
        
        /// <summary>
        /// 로딩 광고 
        /// </summary>
        public void PlayLoadingAD() {
            if(!useLoadingAD)
                return;
                
            Debug.Log(">> PlayLoadingAD : " + SystemManager.main.givenEpisodeData.purchaseState.ToString());
                
            // 무료 유저가 아니면 광고 재생 되지 않음
            if(GameManager.main != null && SystemManager.main.givenEpisodeData.purchaseState != PurchaseState.AD)
                return;
                
            ShowLoadingAD();
        }
        
        /// <summary>
        /// 플레이 AD 재생. 
        /// </summary>
        void ShowLoadingAD() {
            
            // 여기서도 점유율에 따라서, 처리 
            if(UnityEngine.Random.Range(0, 100) < shareLoadingInterstitial)
                ShowInterstitial();
            else 
                ShowRewardAd();
        }        
        #endregion
        
                
        /// <summary>
        /// 플레이 Row 카운트 초기화
        /// </summary>
        public void InitGamePlayRowCount() {
            gamePlayRowCount = 0;
        }
        
        /// <summary>
        /// 스크립트 Row 마다 추가
        /// </summary>
        public void AddGameRowCount() {
            
            // Play AD 사용하지 않으면 끝!
            if(!usePlayAD)
                return;
                
            // 무료 유저가 아니면 광고 재생 되지 않음
            if(GameManager.main != null && GameManager.main.currentEpisodeData.purchaseState != PurchaseState.AD)
                return;
            
            gamePlayRowCount++;
            
            // 정해진 라인까지 그냥 증가.
            if(gamePlayRowCount >= lineOfPlayAD) {
                gamePlayRowCount = 0;
                
                // 확률처리. 
                if(UnityEngine.Random.Range(0,100) < ratePlayAD) 
                    ShowPlayAD();
            }
        }
        
        /// <summary>
        /// 플레이 AD 재생. 
        /// </summary>
        void ShowPlayAD() {
            
            // 여기서도 점유율에 따라서, 처리 
            if(UnityEngine.Random.Range(0, 100) < sharePlayInterstitial)
                ShowAdvertisementReady(false);
            else 
                ShowAdvertisementReady(true);
        }
        

        #region 전면광고


        /// <summary>
        /// 전면광고 보기
        /// </summary>
        public void ShowInterstitial() {
            // Ensure the ad has loaded, then show it.
            if (interstitialAd.AdState == AdState.Loaded) {
                interstitialAd.Show();
            }
            else {
                
            }
        }     
        
        void CreateInterstitial() {
            interstitialAd = MediationService.Instance.CreateInterstitialAd(interstitialAdUnitId);
            // Subscribe callback methods to load events:
            interstitialAd.OnLoaded += OnInterstitialLoaded;
            interstitialAd.OnFailedLoad += OnInterstitialFailedToLoad;

            // Subscribe callback methods to show events:
            interstitialAd.OnShowed += OnInterstitialShown;
            interstitialAd.OnFailedShow += OnInterstitialFailedToShow;
            interstitialAd.OnClosed += OnInterstitialClosed;
            interstitialAd.Load();
        }
        

        void OnInterstitialLoaded(object sender, EventArgs args) {
            Debug.Log("InterstitialAd loaded.");
            // Execute logic for when the ad has loaded
        }

        void OnInterstitialFailedToLoad(object sender, LoadErrorEventArgs args) {
            Debug.Log("InterstitialAd failed to load.");
            // Execute logic for the ad failing to load.
        }

        // Implement show event callback methods:
        void OnInterstitialShown(object sender, EventArgs args) {
            Debug.Log("InterstitialAd shown successfully.");
            // Execute logic for the ad showing successfully.
            
            SetFrontAdStatus(true);
        }

        void OnInterstitialFailedToShow(object sender, ShowErrorEventArgs args) {
            Debug.Log("InterstitialAd failed to show.");
            // Execute logic for the ad failing to show.
            
            SetFrontAdStatus(false);
        }

        private void OnInterstitialClosed(object sender, EventArgs e) {
            Debug.Log("InterstitialAd has closed");
            
            SetFrontAdStatus(false);
            // Execute logic after an ad has been closed.
            CreateInterstitial();
            
            // 광고 기록 
            NetworkLoader.main.LogAdvertisement("interstitial");
            
        }
   
                
        
        #endregion
        
        
        #region 동영상 광고 
        
        void CreateRewardAd() {
            rewardedAd = MediationService.Instance.CreateRewardedAd(rewardedAdUnitId);
            
            rewardedAd.OnLoaded += OnRewardedLoaded;
            rewardedAd.OnFailedLoad += OnRewardedFailedLoad;
            rewardedAd.OnClosed += OnRewardedClosed;
            rewardedAd.OnUserRewarded += OnUserRewarded;
            rewardedAd.OnFailedShow += OnRewardedFailedToShow;
            rewardedAd.OnShowed += OnRewardedShow;
            rewardedAd.Load();
        }
        
        /// <summary>
        /// 콜백이 없는 동영상 광고 재생 
        /// </summary>
        public void ShowRewardAd() {
            
            OnCompleteRewardAD = null;
            isRewarded = false;
            
            if(rewardedAd.AdState == AdState.Loaded)
                rewardedAd.Show();
            else {
                SystemManager.ShowSimpleAlertLocalize("6093");
            }
        }
        
        /// <summary>
        /// 콜백이 있는 동영상 광고 재생 
        /// </summary>
        /// <param name="callback"></param>
        public void ShowRewardAdWithCallback(Action<bool> callback) {
            OnCompleteRewardAD = callback;
            isRewarded = false;
            
            if(rewardedAd.AdState == AdState.Loaded)
                rewardedAd.Show();
            else {
                SystemManager.ShowSimpleAlertLocalize("6093");
            }
        }
        
        void OnRewardedShow(object sender, System.EventArgs args) {
            Debug.Log("OnRewardedShow");
            SetFrontAdStatus(true);
        }
        
        
        void OnRewardedLoaded(object sender, System.EventArgs args) {
            Debug.Log("OnRewardedLoaded");
        }
        
        void OnRewardedFailedLoad(object sender, LoadErrorEventArgs args) {
            Debug.Log("OnRewardedFailedLoad : " + args.Message);
        }
        
        // 영상광고 닫힘 (보상여부와 상관없음 )
        void OnRewardedClosed(object sender, EventArgs e) {
            Debug.Log("Rewarded is closed.");
            // Execute logic for the user closing the ad.
            CreateRewardAd();
            
            
            SetFrontAdStatus(false);
            
        }
        
        // 영상 광고를 끝까지 보았음
        void OnUserRewarded(object sender, RewardEventArgs args) {
            Debug.Log("Ad has rewarded user.");
            // Execute logic for rewarding the user.
            
            
            // 광고 기록
            NetworkLoader.main.LogAdvertisement("rewarded");
            
            isRewarded = true; // true로 변경! 다 봤다!
            OnCompleteRewardAD?.Invoke(isRewarded); // 콜백 호출
        }
        
        void OnRewardedFailedToShow(object sender, ShowErrorEventArgs args) {
            Debug.Log("Ad failed to show.");
            // Execute logic for the ad failing to show.
            
            SetFrontAdStatus(false);
        }
        
        #endregion
        
        
        

        #region 유니티 애널리틱스

        /// <summary>
        /// 패키지 버튼 클릭
        /// </summary>
        /// <param name="_packageName"></param>
        public void AnalyticsPackageButtonClick(string _packageName) {
            AnalyticsEvent.Custom("packageButton", new Dictionary<string, object> {
                {"product_id", _packageName}
            });
        }
        
        /// <summary>
        /// 코인샵 오픈 
        /// </summary>
        /// <param name="__openPosition"></param>
        public void AnalyticsCoinShopOpen(string __openPosition) {
            AnalyticsEvent.Custom("openCoinShop", new Dictionary<string, object> {
                {"openPosition", __openPosition}
            });
        }
        

        
        public void AnalyticsEnter(string position) {
            AnalyticsEvent.Custom(position, null);
        }
        
        
        #endregion

    }
}