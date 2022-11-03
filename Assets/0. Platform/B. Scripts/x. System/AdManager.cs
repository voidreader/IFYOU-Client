using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Mediation;
using Unity.Services.Core;


using LitJson;

// using Firebase;
// using Facebook.Unity;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;


#if UNITY_IOS
using UnityEngine.iOS;
using Unity.Advertisement.IosSupport;
#endif





namespace PIERStory {
    
    /// <summary>
    /// ! 광고 모듈이 관리되는 곳!
    /// </summary>
    public class AdManager : MonoBehaviour
    {
        public static AdManager main = null;
        
        public static Action OnShowAdvertisement;
        
        public static Action<bool> OnCompleteRewardAD = null; // 동영상 광고 보고 콜백
        public bool isAdManagerInit = false;
        public bool isPaidSelection = false;
        
        

        
        
        // * 광고 도중 터치 막기 위한 변수.
        public bool isAdShowing = false; // 현재 광고가 보여지고 있다.
        
        
        [SerializeField] int gamePlayRowCount = 0;
        [SerializeField] bool isRewarded = false; // 영상광고 끝까지 재생되었는지. 
        
        JsonData serverData = null;


        #region Unity Mediation
        [Space]
        [Header("Unity Mediation")]
        public string unityRewardedAdUnitId = "Rewarded_Android";
        public string unityInterstitialAdUnitId = "Interstitial_Android";
        public string unityBannerUnitID = string.Empty;
        
        IRewardedAd unityRewardedAd;
        IInterstitialAd unityInterstitialAd;
        
        IBannerAd unityBannerAd = null;
        public bool isUnityBannerLoaded = false; // 유니티 배너 로드 되었는지?
        #endregion
        
        
        #region Google Admob
        
        [Space]
        [Header("Google Admob")]
        GoogleMobileAds.Api.RewardedAd admobRewardedAd; // 보상형광고 
        GoogleMobileAds.Api.InterstitialAd interstitial; // 전면광고 
        GoogleMobileAds.Api.RewardedInterstitialAd rewardInterstitial; // 보상형 전면광고 

        
        [SerializeField] string admobRewardID = string.Empty;  // 애드몹 보상형 광고 ID 
        [SerializeField] string admobBannerID = string.Empty; // 애드몹 배너 광고 ID 
        [SerializeField] string admobInterstitialID = string.Empty; // 애드몹 전면 광고 ID
        
        [SerializeField] string admobRewardInterstitialID = string.Empty; // 애드몹 보상형 전면 광고 ID
        
        // 아래 플랫폼별 ID 인스펙터에서 초기화 
        [SerializeField] string admobRewardID_Anroid = string.Empty;
        [SerializeField] string admobRewardID_iOS = string.Empty;
        
        [SerializeField] string aadmobBannerID_Anroid = string.Empty;
        [SerializeField] string aadmobBannerID_iOS = string.Empty;
        
        [SerializeField] string admobInterstitialID_Anroid = string.Empty;
        [SerializeField] string admobInterstitialID_iOS = string.Empty;
        [SerializeField] string admobRewardInterstitialID_Anroid = string.Empty;
        [SerializeField] string admobRewardInterstitialID_iOS = string.Empty;
        
        #endregion
        
        #region IronSource
        [Space]
        [Header("IronSource")]
        
        [SerializeField] string ironSourceAppKey = string.Empty;
        [SerializeField] string ironSourceAppKey_Android = string.Empty;
        [SerializeField] string ironSourceAppKey_iOS = string.Empty;
        
        public bool isIronSourceInited = false;
        public bool isIronSourceBannerLoaded = false; // 아이언소스 배너 로드 되었는지? 
        
        
        
        #endregion
        
        #region 서버 광고 기준정보
        [Space]
        [Header("Standard Info")]
        public bool useLoadingAD = false; // 에피소드 로딩 광고 사용여부
        int shareLoadingInterstitial = 0; // 에피소드 로딩 전면광고 점유율
        int shareLoadingRewarded = 0; // 에피소드 로딩 동영상 광고 점유율
        
        public bool useBannerAD = false; // 띠배너 광고 사용여부
        public bool usePlayAD = false; // 플레이 도중 광고 사용여부
        [SerializeField] int sharePlayInterstitial = 0; // 게임플레이 전면광고 점유율
        [SerializeField] int sharePlayRewarded = 0; // 게임플레이 동영상 광고 점유율
        [SerializeField] int ratePlayAD = 0; // 플레이 광고 재생 확률
        [SerializeField] int lineOfPlayAD = 0; // 플레이 광고 실행에 필요한 스크립트 라인 수
        
        public bool isFirstSelectionAdPlayed = false; // 첫번째 선택지 광고가 플레이 되었는지? 
        
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
            if(isAdManagerInit)
                return;
            
            // iOS 추적 권한 요청 
            RequestAuthorizationTracking(); 
            
            InitAdmob();

            InitUnityMediation();
            
            InitIronSource();
            
            // InitFirebase();
            
            // InitFacebook();
            
                        
            isAdManagerInit = true;
        }
        
        /*
        void Update() {
            // throwExceptionEvery60Updates();
        }
        */
        
        private void OnApplicationPause(bool pauseStatus) {
            IronSource.Agent.onApplicationPause (pauseStatus);
        }
        
        
        /// <summary>
        /// 하단 배너 표시
        /// </summary>
        public void PlayBottomBanner() {
            
            // 게임플레이 도중에만 표시되고, 각종패스 구매자는 사용하지 않는다.

            if(UserManager.main.ifyouPassDay > 0)
                return; // 이프유 패스 보유자
                
            
            // 게임씬에서만 동작한다.  원데이패스나 프리미엄패스 사용자는 광고 뜨지 않음 
            if(GameManager.main != null && StoryManager.main != null && (UserManager.main.HasProjectFreepass() || StoryManager.main.CurrentProject.IsValidOnedayPass() ))
                return;
                       
            // 로드 되었으면 보여주는것으로 처리 
            if(MediationService.InitializationState != InitializationState.Initialized)
                return;
                
            CreateUnityBanner();
            
            
            
            /*
            if(isIronSourceBannerLoaded) {
                IronSource.Agent.displayBanner(); 
            }
            else { // 첫 호출시에는 로드 
                LoadIronSourceBanner();
            }
            */
        }
        
        
        #region 아이언소스
        
        void InitIronSource() {
            #if UNITY_ANDROID
            ironSourceAppKey = ironSourceAppKey_Android;
            ironSourceAppKey = ironSourceAppKey_iOS;
            #elif UNITY_IOS
            ironSourceAppKey = ironSourceAppKey_iOS;
            #else
            ironSourceAppKey = "unexpected_platform";
            #endif
            
            Debug.Log ("unity-script: InitIronSource Start called");

            //Dynamic config example
            IronSourceConfig.Instance.setClientSideCallbacks (true); // 클라리언스 타이트 콜백. 

            string id = IronSource.Agent.getAdvertiserId ();
            Debug.Log ("unity-script: IronSource.Agent.getAdvertiserId : " + id);
            Debug.Log ("unity-script: IronSource.Agent.validateIntegration");
            IronSource.Agent.validateIntegration ();

            Debug.Log ("unity-script: unity version" + IronSource.unityVersion ());

 
            // SDK init
            Debug.Log ("unity-script: IronSource.Agent.init");
            IronSourceEvents.onSdkInitializationCompletedEvent += IronSourceInitCompletedEvent;
            IronSource.Agent.init (ironSourceAppKey);
   
        }
        
        void IronSourceInitCompletedEvent() {
            
            Debug.Log ("unity-script: IronSource.Agent.init Completed");
            isIronSourceInited = true;
        }
        
        
        
        
        public void LoadIronSourceBanner() {
            
            // IronSource.Agent.int
            
            // Add Banner Events
            IronSourceEvents.onBannerAdLoadedEvent += IronSourceBannerAdLoadedEvent;
            IronSourceEvents.onBannerAdLoadFailedEvent += IronSourceBannerAdLoadFailedEvent;		
            IronSourceEvents.onBannerAdClickedEvent += IronSourceBannerAdClickedEvent; 
            IronSourceEvents.onBannerAdScreenPresentedEvent += IronSourceBannerAdScreenPresentedEvent; 
            IronSourceEvents.onBannerAdScreenDismissedEvent += IronSourceBannerAdScreenDismissedEvent;
            IronSourceEvents.onBannerAdLeftApplicationEvent += IronSourceBannerAdLeftApplicationEvent;
            
            IronSource.Agent.loadBanner (IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        }
        
        /// <summary>
        /// 하단 배너 감추기
        /// </summary>
        public void HideIronSourceBanner() {
            
            if(!isIronSourceInited || !isIronSourceBannerLoaded)
                return;
            
            IronSource.Agent.hideBanner();
        }
        
        void IronSourceBannerAdLoadedEvent ()
        {
            Debug.Log ("unity-script: I got BannerAdLoadedEvent");
            isIronSourceBannerLoaded = true;
        }
        
        void IronSourceBannerAdLoadFailedEvent (IronSourceError error)
        {
            Debug.Log ("unity-script: I got BannerAdLoadFailedEvent, code: " + error.getCode () + ", description : " + error.getDescription ());
        }
        
        void IronSourceBannerAdClickedEvent ()
        {
            Debug.Log ("unity-script: I got BannerAdClickedEvent");
        }
        
        void IronSourceBannerAdScreenPresentedEvent ()
        {
            Debug.Log ("unity-script: I got BannerAdScreenPresentedEvent");
        }
        
        void IronSourceBannerAdScreenDismissedEvent ()
        {
            Debug.Log ("unity-script: I got BannerAdScreenDismissedEvent");
        }
        
        void IronSourceBannerAdLeftApplicationEvent ()
        {
            Debug.Log ("unity-script: I got BannerAdLeftApplicationEvent");
        }        
        
        #endregion
        
        
        
        #region 구글 애드몹 
        /// <summary>
        /// 애드몹 초기화 
        /// </summary>
        void InitAdmob() {
            // 플랫폼에 따른 ID 값 설정
            #if UNITY_IOS
            admobRewardID = admobRewardID_iOS;
            admobInterstitialID = admobInterstitialID_iOS;
            admobBannerID = aadmobBannerID_iOS;
            admobRewardInterstitialID = admobRewardInterstitialID_iOS;
                        
            #else
            admobRewardID = admobRewardID_Anroid;
            admobInterstitialID = admobInterstitialID_Anroid;
            admobBannerID = aadmobBannerID_Anroid;
            admobRewardInterstitialID = admobRewardInterstitialID_Anroid;
            #endif
            
            
            // 초기화 시작 
            MobileAds.Initialize(HandleInitCompleteAction);
            
        }
        
        /// <summary>
        /// 애드몹 초기호 콜백
        /// </summary>
        /// <param name="initstatus"></param>
        private void HandleInitCompleteAction(InitializationStatus initstatus) {
            Debug.Log("Google Admob Initialization complete.");
            
            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // the main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                InitAdmobRewardedAd();
                RequestInterstitial();

            });
        }        
        
        /// <summary>
        /// 애드몹 AdRequest 생성 헬퍼 
        /// </summary>
        /// <returns></returns>
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }        
        
        
        
        /// <summary>
        /// 애드몹 보상형 전면광고 초기화 
        /// </summary>
        void InitAdmobRewardInterstitial() {
            AdRequest request = new AdRequest.Builder().Build();
            RewardedInterstitialAd.LoadAd(admobRewardInterstitialID, request, rewardInterstitialLoadCallback);

        }
        private void rewardInterstitialLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error) {
            if (error == null)
            {
                rewardInterstitial = ad;
                rewardInterstitial.OnAdDidDismissFullScreenContent += HandleAdDidDismiss;
                
            }
        }
        
        private void HandleAdDidDismiss(object sender, EventArgs args) {
            InitAdmobRewardInterstitial();
        }
        // 애드몹 보상형 전면광고 종료 

        public void ShowRewardInterstitial() {
            if(rewardInterstitial != null) {
                rewardInterstitial.Show(earnRewardInterstitial);
            }
            else {
                InitAdmobRewardInterstitial();
            }
        }
        
        void earnRewardInterstitial(Reward reward) {
            // 보상 받음 
            OnCompleteRewardAD?.Invoke(true);
            isRewarded = false;
            
        }
        
        
        
        
        /// <summary>
        /// 애드몹 보상형 광고 초기화 
        /// </summary>
        void InitAdmobRewardedAd() {
            
            if(this.admobRewardedAd != null)
                this.admobRewardedAd.Destroy();
            
            this.admobRewardedAd = new GoogleMobileAds.Api.RewardedAd(admobRewardID);
                    // Called when an ad request has successfully loaded.
            this.admobRewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            this.admobRewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            this.admobRewardedAd.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            this.admobRewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            this.admobRewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            
            // Called when the ad is closed.
            this.admobRewardedAd.OnAdClosed += HandleRewardedAdClosed;

            
            // Load the rewarded ad with the request.
            this.admobRewardedAd.LoadAd(CreateAdRequest());

        }
        
        public void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("HandleRewardedAdLoaded event received");
        }

        public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("HandleRewardedAdFailedToLoad event received : " + args.LoadAdError.GetMessage());
        }

        public void HandleRewardedAdOpening(object sender, EventArgs args)
        {
            Debug.Log("HandleRewardedAdOpening event received");
            SetFrontAdStatus(true);
        }

        public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            Debug.Log("HandleRewardedAdFailedToShow event received with message: " + args.AdError.GetMessage());
            SetFrontAdStatus(false);                                
        }

        public void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            Debug.Log("HandleRewardedAdClosed event received");
            
            MobileAdsEventExecutor.ExecuteInUpdate(()=>{
                
                InitAdmobRewardedAd(); // 광고 닫히고, 생성 
                SetFrontAdStatus(false);    
                
                OnCompleteRewardAD?.Invoke(isRewarded); // 콜백 호출
                
                if(isRewarded)
                    NetworkLoader.main.IncreaseDailyMissionCount(3);
            });
            
            
        }

        public void HandleUserEarnedReward(object sender, GoogleMobileAds.Api.Reward __reward)
        {
            Debug.Log("HandleUserEarnedReward event received");
            isRewarded = true; // true로 변경! 다 봤다!
        }
        
        
        
        /// <summary>
        /// 전면광고 생성 및 로드
        /// </summary>
        private void RequestInterstitial()
        {
            
            // Clean up interstitial before using it
            if(this.interstitial != null) {
                this.interstitial.Destroy();
            }
            
            // Initialize an InterstitialAd.
            this.interstitial = new GoogleMobileAds.Api.InterstitialAd(admobInterstitialID);
            this.interstitial.OnAdClosed += HandleOnAdClosed;
            this.interstitial.OnAdLoaded += HandleOnAdLoaded;
            

            // Load the interstitial with the request.
            this.interstitial.LoadAd(CreateAdRequest());
            

        }
        
        /// <summary>
        /// 애드몹 전면광고. 화면 전환시에만 사용한다. 
        /// </summary>
        public void ShowAdmobInterstitial() {
            
           
            if(UserManager.main.ifyouPassDay > 0)
                return; // 이프유 패스 보유자
                
            
            // 게임씬에서만 동작한다.  원데이패스나 프리미엄패스 사용자는 광고 뜨지 않음 
            if(GameManager.main != null && StoryManager.main != null && (UserManager.main.HasProjectFreepass() || StoryManager.main.CurrentProject.IsValidOnedayPass() ))
                return;
           
            if(this.interstitial != null && this.interstitial.IsLoaded()) {
                this.interstitial.Show();
            }
            else {
                RequestInterstitial();
            }
        }
        
        /// <summary>
        /// 인게임 인터스티셜
        /// </summary>
        public void ShowInGameInterstitial() {
            
            
            
            if(this.interstitial != null && this.interstitial.IsLoaded()) {
                this.interstitial.Show();
            }
            else {
                RequestInterstitial();
            }
        }
        
        
        public void HandleOnAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("Interstitial HandleOnAdClosed");
        }

        
        void HandleOnAdClosed(object sender, EventArgs args) {
            Debug.Log("Interstitial HandleOnAdClosed");
            
            MobileAdsEventExecutor.ExecuteInUpdate(() => {
                RequestInterstitial();    
            });
  
        }

        
        

        
        
        #endregion
        // ? 애드몹 종료 
        
        
        
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
                    foreach (GameSoundCtrl sc in GameManager.main.SoundGroup)
                        sc.PlayAudioClip(false);
                }
                else
                {
                    if (!GameManager.main.isPlaying)
                        return;

                    foreach (GameSoundCtrl sc in GameManager.main.SoundGroup)
                        sc.PlayAudioClip(true);
                }
            }
        }
        
        
       
        
        
        /// <summary>
        /// 유니티 메디에이션 초기화 
        /// </summary>
        /// <returns></returns>
        async void InitUnityMediation() {

            #if UNITY_ANDROID
            unityRewardedAdUnitId = "Rewarded_Android";
            unityInterstitialAdUnitId = "Interstitial_Android";
            unityBannerUnitID = "Banner_Android";
            #else
            unityRewardedAdUnitId = "Rewarded_iOS";
            unityInterstitialAdUnitId = "Interstitial_iOS";            
            unityBannerUnitID = "Banner_iOS";
            #endif               
            
            // Initialize package to access API
            await UnityServices.InitializeAsync();
            
            try
            {
				Debug.Log("Unity Mediation Initializing...");
                await UnityServices.InitializeAsync(); 
				Debug.Log("Unity Mediation Initialized!"); 
                UnityMediationInitCompleted();
			}
            catch (Exception e)
            {
                UnityMediationInitFailed(e);
			}            
            
            // 스테이트 체크 
            // Debug.Log(UnityServices.State);
         
            

            
        }
        
        void UnityMediationInitCompleted() {
            
            CreateRewardAd();
            CreateInterstitial();
        }
        
        
        void UnityMediationInitFailed(Exception error)
        {
            var initializationError = SdkInitializationError.Unknown;
            if (error is InitializeFailedException initializeFailedException)
            {
				  initializationError = initializeFailedException.initializationError;
			}
			Debug.Log($"Unity Mediation Initialization Failed: {initializationError}:{error.Message}");
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
            if(__isRewarded) {
                
                // 광고 준비되었는지 체크 추가
                if(unityRewardedAd == null || unityRewardedAd.AdState != AdState.Loaded) {
                    CreateRewardAd();
                    return;
                }
                    
                OnShowAdvertisement = ShowRewardAd;
            }
            else {
                
                // 광고 준비되었는지 체크 추가
                if(unityInterstitialAd == null || unityInterstitialAd.AdState != AdState.Loaded) {
                    CreateInterstitial();
                    return;
                }
                
                OnShowAdvertisement = ShowUnityAdsInterstitial;
            }
                
            
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
            
            if(this.interstitial == null || !this.interstitial.IsLoaded()) {
                RequestInterstitial();
                return; 
            }
            
            // 유료 선택지에서는 뜨지 않음 
            if(isPaidSelection) {
                isPaidSelection = false; // 한번 걸렸으면 다시 false로 돌린다. 
                return; 
            }
            
            if(!useSelectionAD) 
                return;
                
            // 패스 유저인 경우 광고 등장하지 않음.
            try {
                // 1화에서는 등장하지 않음
                // 정규에피소드에서만 등장
                if(GameManager.main.currentEpisodeData.episodeType != EpisodeType.Chapter)
                    return;
                    
                if(GameManager.main.currentEpisodeData.episodeNumber < 2)
                    return;
                
                
                // 한번이라도 광고재생 되었으면, gamePlayRowCount 100 미만이면 재생하지 않음(너무 많은 재생을 막기 위함)
                if(isFirstSelectionAdPlayed && gamePlayRowCount < lineOfPlayAD)
                    return;
                
                
                if(UserManager.main.HasProjectFreepass() || StoryManager.main.CurrentProject.IsValidOnedayPass() || UserManager.main.ifyouPassDay > 0)
                    return;
            }
            catch {
                return;
            }

                
            ShowSelectionAD();
        }
        
        
        /// <summary>
        /// 선택지 선택 후 광고 보여주기 
        /// </summary>
        public void ShowSelectionAD() {
            
            isPaidSelection = false;
            
            PopupBase p = PopupManager.main.GetPopup("AdvertisementShow");
            if(p == null) {
                Debug.LogError("AdvertisementShow ");
            }
            
            PopupManager.main.ShowPopup(p, false, false);
            
            isFirstSelectionAdPlayed = true; // 재생되었음. 
            InitGamePlayRowCount(); // 선택지 선택하면 플레이 카운트 초기화
        }
        
        /// <summary>
        /// 보상형 전면광고 동의 팝업. 
        /// </summary>
        void ShowAdvertisementAgreement() {
            
        }
        
        
        
        #endregion
        
        #region 에피소드 로딩 광고 처리 
        
        /// <summary>
        /// 로딩 광고 
        /// </summary>
        public void PlayLoadingAD() {
            if(!useLoadingAD)
                return;
                
            if(GameManager.main == null)
                return;
                
            if(!GameManager.main.currentEpisodeData.isValidData)
                return;
                
            if(GameManager.main.currentEpisodeData.purchaseState != PurchaseState.AD)
                return;                
            
            if(UserManager.main.HasProjectFreepass()) 
                return;
            
            if(UnityEngine.Random.Range(0, 100) < shareSelectionInterstitial)
                ShowAdvertisementReady(false);
            else 
                ShowAdvertisementReady(true);
            
            // Debug.Log(">> PlayLoadingAD : " + SystemManager.main.givenEpisodeData.purchaseState.ToString());
            // ShowLoadingAD();
        }
        
        /// <summary>
        /// 플레이 AD 재생. 
        /// </summary>
        void ShowLoadingAD() {
            
            ShowRewardAd();
            
            // 여기서도 점유율에 따라서, 처리 
            /*
            if(UnityEngine.Random.Range(0, 100) < shareLoadingInterstitial)
                ShowInterstitial();
            else 
            */
                
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
            gamePlayRowCount++;
            
            // Play AD 사용하지 않으면 끝!
            if(!usePlayAD)
                return;
                
            // 무료 유저가 아니면 광고 재생 되지 않음
            if(GameManager.main != null && GameManager.main.currentEpisodeData.purchaseState != PurchaseState.AD)
                return;
            
            
            
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
        public void ShowUnityAdsInterstitial() {
            // Ensure the ad has loaded, then show it.
            if (unityInterstitialAd.AdState == AdState.Loaded) {
                unityInterstitialAd.ShowAsync();
            }
            else {
                
            }
        }     
        
        void CreateInterstitial() {
            unityInterstitialAd = MediationService.Instance.CreateInterstitialAd(unityInterstitialAdUnitId);
            // Subscribe callback methods to load events:
            unityInterstitialAd.OnLoaded += OnInterstitialLoaded;
            unityInterstitialAd.OnFailedLoad += OnInterstitialFailedToLoad;

            // Subscribe callback methods to show events:
            unityInterstitialAd.OnShowed += OnInterstitialShown;
            unityInterstitialAd.OnFailedShow += OnInterstitialFailedToShow;
            unityInterstitialAd.OnClosed += OnInterstitialClosed;
            unityInterstitialAd.LoadAsync();
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
            NetworkLoader.main.IncreaseDailyMissionCount(3);
        }
   
                
        
        #endregion
        
        
        #region 유니티 미디에이션
        
        void CreateUnityBanner() {
            
            isUnityBannerLoaded = false;
            
            BannerAdSize bannerSize = new BannerAdSize(BannerAdPredefinedSize.Banner);
            unityBannerAd = MediationService.Instance.CreateBannerAd(unityBannerUnitID, bannerSize, BannerAdAnchor.BottomCenter, Vector2.zero);
                   
            LoadUnityBanner();
        }
        
        async void LoadUnityBanner() {
           
            try
            {
				  await unityBannerAd.LoadAsync();
				  AdLoaded();
            }
            catch (LoadFailedException e)
            {
				  AdFailedLoad(e);
			}            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void HideUnityBanner() {
            if(!isUnityBannerLoaded)   
                return;
                
            unityBannerAd.Dispose();
        }
        
        void AdLoaded()
        {
			Debug.Log("Ad loaded");
            isUnityBannerLoaded = true;
            
  		}

        void AdFailedLoad(LoadFailedException e)
        {
			Debug.Log("Failed to load ad");
			Debug.Log(e.Message);
            isUnityBannerLoaded = false;
  		}        
        
        
        void CreateRewardAd() {
            unityRewardedAd = MediationService.Instance.CreateRewardedAd(unityRewardedAdUnitId);
            
            unityRewardedAd.OnLoaded += OnRewardedLoaded;
            unityRewardedAd.OnFailedLoad += OnRewardedFailedLoad;
            unityRewardedAd.OnClosed += OnRewardedClosed;
            unityRewardedAd.OnUserRewarded += OnUserRewarded;
            unityRewardedAd.OnFailedShow += OnRewardedFailedToShow;
            unityRewardedAd.OnShowed += OnRewardedShow;
            unityRewardedAd.LoadAsync();
        }
        
        /// <summary>
        /// 콜백이 없는 동영상 광고 재생 
        /// </summary>
        public void ShowRewardAd() {
            
            OnCompleteRewardAD = null;
            isRewarded = false;
            
            if(unityRewardedAd.AdState == AdState.Loaded)
                unityRewardedAd.ShowAsync();
            else {
                SystemManager.ShowSimpleAlertLocalize("6093");
            }
        }
        
        /// <summary>
        /// 영상 광고 가능한지. 
        /// </summary>
        /// <returns></returns>
        public bool CheckRewardedAdPossible() {
            
            // 애드몹과 유니티 미디에이션 체크한다.
            if((admobRewardedAd == null || !admobRewardedAd.IsLoaded()) || (unityRewardedAd == null || unityRewardedAd.AdState != AdState.Loaded)) {
              
                // 없으면 생성해주자. 
                // 애드몹  
                if(admobRewardedAd == null || !admobRewardedAd.IsLoaded())
                    InitAdmobRewardedAd();
                    
                // 유니티 미디에이션 
                if(unityRewardedAd == null || unityRewardedAd.AdState != AdState.Loaded)
                    CreateRewardAd();
                    
                
                return false;
            }
            
                
            if(admobRewardedAd.IsLoaded() || unityRewardedAd.AdState == AdState.Loaded)
                return true;
                
            return false;
        }
        
        /// <summary>
        /// 콜백이 있는 동영상 광고 재생 
        /// </summary>
        /// <param name="callback"></param>
        public void ShowRewardAdWithCallback(Action<bool> callback) {
            
            // * 2022.04.22 콜백있는 동영상 광고는 애드몹 우선으로 변경한다. 
            
            OnCompleteRewardAD = callback;
            isRewarded = false;
            
            // * 애드몹 우선으로 실행 
            if(admobRewardedAd != null && admobRewardedAd.IsLoaded()) {
               admobRewardedAd.Show();  
               return;
            }
            else {
                // 애드몹 로딩이 완료되지 않았으면 로딩처리 지시하고, 유니티 애즈로 넘어간다.
                InitAdmobRewardedAd();
            }
            
            
            // * 유니티 애즈 
            if(unityRewardedAd != null && unityRewardedAd.AdState == AdState.Loaded)
                unityRewardedAd.ShowAsync();
            else {
                // 유니티 애즈
                CreateRewardAd();
                
                // 유니티 애즈도 없으면.. ㅠ
                SystemManager.ShowSimpleAlertLocalize("6093");
            }
        }
        
        void OnRewardedShow(object sender, EventArgs args) {
            Debug.Log("OnRewardedShow");
            SetFrontAdStatus(true);
        }
        
        
        void OnRewardedLoaded(object sender, EventArgs args) {
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
            
            
            
            isRewarded = true; // true로 변경! 다 봤다!
            OnCompleteRewardAD?.Invoke(isRewarded); // 콜백 호출
            NetworkLoader.main.IncreaseDailyMissionCount(3);
        }
        
        void OnRewardedFailedToShow(object sender, ShowErrorEventArgs args) {
            Debug.Log("Ad failed to show.");
            // Execute logic for the ad failing to show.
            
            SetFrontAdStatus(false);
        }
        
        #endregion
        
        
        


        #region 페이스북
        
        /*
        void InitFacebook() {
            if (!FB.IsInitialized) {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            } else {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }
        
        private void InitCallback ()
        {
            if (FB.IsInitialized) {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            } else {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity (bool isGameShown)
        {
            
        }
        */
        
        #endregion

    }
}