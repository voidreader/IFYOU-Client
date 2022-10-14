﻿using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


using LitJson;
using BestHTTP;
using Toast.Gamebase;
using Doozy.Runtime.Signals;


namespace PIERStory {

    /// <summary>
    /// Lobby 씬에서만 존재합니다. 
    /// 각 씬에만 존재하는 singleton 개체가 필요하다(LobbyManager, GameManager)
    /// </summary>
    public class LobbyManager : MonoBehaviour {

        public static LobbyManager main = null;


        [Header("이프유플레이")]
        public Sprite spriteCircleBase;
        public Sprite spriteCircleOpen;
        public Sprite spriteCircleLimit;                // 출석 보충해야 받을 수 있는 상태
        public Sprite spriteCircleLimitWhite;           // 출석 보충하면 바로 받을 수 있는 상태
        public Sprite spriteSquareBase;
        public Sprite spriteSquareOpen;

        [Space]
        public Sprite spriteDailyMissionOngoing;
        public Sprite spriteDailyMissionClaim;
        public Sprite spriteDailyMissionFinish;


        // [Header("프로필")]
        // public Sprite spriteBronzeBadge;
        // public Sprite spriteSilverBadge;
        // public Sprite spriteGoldBadge;
        // public Sprite spritePlatinumBadge;
        // public Sprite spriteIFYOUBadge;

        // [Space]
        // public Sprite spriteLevelTag1;
        // public Sprite spriteLevelTag2;
        // public Sprite spriteLevelTag3;
        // public Sprite spriteLevelTag4;


        [Space]
        [SerializeField] NetworkLoadingScreen lobbyNetworkLoadingScreen; 


        private void Awake()
        {
            main = this;

            Debug.Log("Initialize LobbyManager");
        }

        
        System.Collections.IEnumerator Start() {

            // * 팝업매니저 초기화(로비씬)
            PopupManager.main.InitPopupManager();
            
            yield return null;
            yield return new WaitForSeconds(0.1f);

            if(UserManager.main != null && !string.IsNullOrEmpty(UserManager.main.userKey))
            {
                
                NetworkLoader.main.RequestIfyouplayList();
                yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            }
            
            ViewCommonTop.OnBackAction = null;
            
            
            // * 로비씬 시작을 알린다. 
            Signal.Send(LobbyConst.STREAM_COMMON, "LobbyPlay"); 
        }
        
        
        

        
        
        private void Update() {
            
            if(Input.GetKeyDown(KeyCode.O)) {
                NetworkLoader.main.RequestRecommedStory();
            }
            

            /*
            if(Input.GetKeyDown(KeyCode.P)) {
                // AdManager.main.ShowRewardAd();
                SystemManager.ShowNetworkLoading();
            }
            */
            
            
            if(Input.GetKeyDown(KeyCode.R)) {
                //Signal.Send(LobbyConst.STREAM_IFYOU, "showIntro", string.Empty);
                SystemManager.ShowIntroPopup();
            }
        }



        public void OnClickContactTemp()
        {
            Debug.Log("Open Contact");

            Gamebase.Contact.OpenContact((error) =>
            {
                if(Gamebase.IsSuccess(error))
                {

                }
                else
                {
                    Debug.Log("GameBase Contact Error : "+ error.code);
                }
            });
        }



        /// <summary>
        /// 로비 씬에서 사용하는 네트워크 로딩 스크린 받기
        /// </summary>
        /// <returns></returns>
        public NetworkLoadingScreen GetLobbyNetworkLoadingScreen()
        {
            return lobbyNetworkLoadingScreen;
        }
        
        
        #region 플랫폼 로딩 화면 처리 
        
        
        /// <summary>
        /// 플랫폼 로딩 이미지 하나 주세요!
        /// 이 메소드는 기존 목록을 모두 다운로드 받았다는 전제하게 동작한다. 
        /// </summary>
        /// <returns></returns>
        public Texture2D GetRandomPlatformLoadingTexture() {
            
            JsonData data = null; // 로컬에 저장된 로딩 이미지 목록 
            int imageIndex = 0; // 랜덤 index 
            string imageKey = string.Empty; // 불러올 이미지 key 
            string imageURL = string.Empty; // 불러올 이미지 url
            Texture2D selectedTexture = null;
            
            
            if(!ES3.KeyExists(SystemConst.KEY_PLATFORM_LOADING))
                return null;
                
            
            data = JsonMapper.ToObject(ES3.Load<string>(SystemConst.KEY_PLATFORM_LOADING));
            Debug.Log("GetRandomPlatformLoadingTexture : " + JsonMapper.ToStringUnicode(data));
            
            if(data == null || data.Count == 0) {
                Debug.Log("<color=orange>No PlatformLoading Texture </color>");
                return null;
            }
            
            imageIndex = Random.Range(0, data.Count);
            imageKey = data[imageIndex][SystemConst.IMAGE_KEY].ToString();
            imageURL = data[imageIndex][SystemConst.IMAGE_URL].ToString();
            
            if(string.IsNullOrEmpty(imageKey) || string.IsNullOrEmpty(imageURL)) 
                return null;
                
            selectedTexture = SystemManager.GetLocalTexture2D(imageKey);
            
           
            return selectedTexture;
            
               
        }
        
        /// <summary>
        /// 서버에 플랫폼 로딩 이미지 리스트 요청 
        /// </summary>        
        public void RequestPlatformLoadingImages() {
            
            JsonData imageData = new JsonData();
            imageData[CommonConst.FUNC] = "mainLoadingImageRandom";
            imageData[LobbyConst.COL_LANG] = "KO";


            NetworkLoader.main.SendPost(OnRequestPlatformLoadingImages, imageData, false);
        }
        
        /// <summary>
        /// 플랫폼 로딩화면 처리 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>        
        void OnRequestPlatformLoadingImages(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
                return;


            Debug.Log("OnRequestPlatformLoadingImages : " + res.DataAsText);
            if(string.IsNullOrEmpty(res.DataAsText))
                return;
            
            JsonData data = JsonMapper.ToObject(res.DataAsText);
            string imageKey = string.Empty;
            string imageURL = string.Empty;
            
            if(data == null) {
                return;
            }
            

            for(int i=0;i<data.Count;i++)
            {
                if(data[i][CommonConst.COL_IMAGE_KEY] == null)
                    continue;
                
                imageKey = SystemManager.GetJsonNodeString(data[i] ,CommonConst.COL_IMAGE_KEY);
                imageURL = SystemManager.GetJsonNodeString(data[i], CommonConst.COL_IMAGE_URL);
                
                if(string.IsNullOrEmpty(imageKey))
                    continue;
                
                // imageKey 파일에 있는지 체크한다.     
                // 파일 없으면 다운로드 요청 시~작!
                if(!SystemManager.CheckFileExists(imageKey)) {
                    SystemManager.RequestDownloadImage(imageURL, imageKey, null);
                }
            }
            
            // Save 로컬에 세이브한다. 
            ES3.Save<string>(SystemConst.KEY_PLATFORM_LOADING, JsonMapper.ToJson(data));
        }


        #endregion

    }
}