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
        int sharePlayInterstitial = 0; // 게임플레이 전면광고 점유율
        int sharePlayRewarded = 0; // 게임플레이 동영상 광고 점유율
        int ratePlayAD = 0; // 플레이 광고 재생 확률
        int lineOfPlayAD = 0; // 플레이 광고 실행에 필요한 스크립트 라인 수
        
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
        
        
        public void InitGameRowCount() {
            gamePlayRowCount = 0;
        }
        
        /// <summary>
        /// 스크립트 Row 마다 추가
        /// </summary>
        public void AddGameRowCount() {
            gamePlayRowCount++;
            
            if(gamePlayRowCount >= 60) {
                gamePlayRowCount = 0;
                ShowMiddleAd();
            }
        }
        
        public void ShowMiddleAd() {
            int dice = UnityEngine.Random.Range(0, 100);
            if(dice < 20)
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