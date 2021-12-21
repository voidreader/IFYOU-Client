using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Mediation;
using Unity.Services.Core;



namespace PIERStory {

    public class AdManager : MonoBehaviour
    {
        public static AdManager main = null;
        
        public string rewardedAdUnitId = "Rewarded_Android";
        public string interstitialAdUnitId = "Interstitial_Android";
        
        [SerializeField] int gamePlayRowCount = 0;
        
        IRewardedAd rewardedAd;
        IInterstitialAd interstitialAd;
        
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