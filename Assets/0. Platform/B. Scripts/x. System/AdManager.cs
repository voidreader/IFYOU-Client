using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Mediation;
using Unity.Services.Core;
using LitJson;



namespace PIERStory {

    public class AdManager : MonoBehaviour
    {
        public static AdManager main = null;
        
        public string rewardedAdUnitId = "Rewarded_Android";
        public string interstitialAdUnitId = "Interstitial_Android";
        
        [SerializeField] int gamePlayRowCount = 0;
        
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

            InitUnityMediation();
            
        }
        
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
        
        #region 선택지 선택 후 광고 처리
        
        /// <summary>
        /// 선택지 선택 후 광고
        /// </summary>
        public void PlaySelectionAD() {
            if(!useSelectionAD) 
                return;
                
            // 무료 유저가 아니면 광고 재생 되지 않음
            if(GameManager.main != null && GameManager.main.currentEpisodeData.purchaseState == PurchaseState.AD)
                return;
                
                
            ShowSelectionAD();
        }
        
        
        /// <summary>
        /// 선택지 선택 후 광고 보여주기 
        /// </summary>
        public void ShowSelectionAD() {
            if(UnityEngine.Random.Range(0, 100) < shareSelectionInterstitial)
                ShowInterstitial();
            else 
                ShowRewardAd();
                
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
                
            // 무료 유저가 아니면 광고 재생 되지 않음
            if(GameManager.main != null && GameManager.main.currentEpisodeData.purchaseState == PurchaseState.AD)
                return;
                
            ShowLoadingAD();
        }
        
        /// <summary>
        /// 플레이 AD 재생. 
        /// </summary>
        public void ShowLoadingAD() {
            
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
            if(GameManager.main != null && GameManager.main.currentEpisodeData.purchaseState == PurchaseState.AD)
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
        public void ShowPlayAD() {
            
            // 여기서도 점유율에 따라서, 처리 
            if(UnityEngine.Random.Range(0, 100) < sharePlayInterstitial)
                ShowInterstitial();
            else 
                ShowRewardAd();
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
        }

        void OnInterstitialFailedToShow(object sender, ShowErrorEventArgs args) {
            Debug.Log("InterstitialAd failed to show.");
            // Execute logic for the ad failing to show.
        }

        private void OnInterstitialClosed(object sender, EventArgs e) {
            Debug.Log("InterstitialAd has closed");
            // Execute logic after an ad has been closed.
            CreateInterstitial();
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
            
            rewardedAd.Load();
        }
        
        public void ShowRewardAd() {
            if(rewardedAd.AdState == AdState.Loaded)
                rewardedAd.Show();
        }
        
        void OnRewardedLoaded(object sender, System.EventArgs args) {
            Debug.Log("OnRewardedLoaded");
        }
        
        void OnRewardedFailedLoad(object sender, LoadErrorEventArgs args) {
            Debug.Log("OnRewardedFailedLoad : " + args.Message);
        }
        void OnRewardedClosed(object sender, EventArgs e) {
            Debug.Log("Rewarded is closed.");
            // Execute logic for the user closing the ad.
            
            CreateRewardAd();
        }
        void OnUserRewarded(object sender, RewardEventArgs args) {
            Debug.Log("Ad has rewarded user.");
            // Execute logic for rewarding the user.
        }
        
        void OnRewardedFailedToShow(object sender, ShowErrorEventArgs args) {
            Debug.Log("Ad failed to show.");
            // Execute logic for the ad failing to show.
        }
        
        #endregion
        


    }
}