using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using LitJson;
using Toast.Gamebase;

namespace PIERStory {

    /// <summary>
    /// Lobby 씬에서만 존재합니다. 
    /// 각 씬에만 존재하는 singleton 개체가 필요하다(LobbyManager, GameManager)
    /// </summary>
    public class LobbyManager : MonoBehaviour {

        public static LobbyManager main = null;

        JsonData platformImageJson = null; // 플랫폼 이미지 JSON

        public ScriptLiveMount currentLiveIllust = null; // Live Illust for Gallery 
        public ScriptLiveMount currentLiveObject = null; // Live Object for Gallery
        
        int scaleOffset = 0;
        string illustName = string.Empty;

       
        [Space]
        [Space]
        [Header("로비 씬 공통 스프라이트")]
        public Sprite spriteGemIcon; // 보석
        public Sprite spriteCoinIcon; // 코인(응모권)
        public Sprite spriteOneTimeIcon; // 1회권
        public Sprite spriteFreepassIcon; // 프리패스 아이콘
        public Sprite spriteInappOriginIcon; // 인앱 구매확정 메일 아이콘
        
        [Header("갤러리 - 사운드 BGMSprite")]
        public Sprite spriteOpenVoice;
        public Sprite spriteLockVoice;

        [Header("미션View Sprite")]
        public Sprite spriteLockOverlay;          // 클리어하지 못한 미션 overlay
        public Sprite spriteClearOverlay;         // 보상 수령이 끝난 overlay

        public Sprite spriteGetReward;      // 단일 미션 보상
        public Sprite spriteLockReward;     // 해금하지 않은 미션
        public Sprite spriteGotReward;      // 이미 받은 미션 보상

        public Sprite spriteOpenMissionStroke;      // 해금, 수령 미션 테두리
        public Sprite spriteLockMissionStroke;      // 해금되지 않은 테두리

        public Sprite spriteHiddenMissionBadge;     // 히든미션
        
        [Header("에피소드 관련 Sprite")]
        public Sprite spriteEpisodePrevCover; // 과거 커버
        public Sprite spriteEpisodeNextCover; // 미래 커버
        public Sprite spriteEpisodeCurrentCover; // 현재 커버
        
        public Sprite spriteEpisodePrevIcon; // 과거 커버
        public Sprite spriteEpisodeNextIcon; // 미래 커버 
        
        [Header("네비게이션 Sprite")]
        public Sprite spriteNavLobbyOn;
        public Sprite spriteNavLobbyOff;
        public Sprite spriteNavCategoryOn;
        public Sprite spriteNavCategoryOff;
        public Sprite spriteNavShopOn;
        public Sprite spriteNavShopOff;
        public Sprite spriteNavIfYouOn;
        public Sprite spriteNavIfYouOff;
        public Sprite spriteNavProfileOn;
        public Sprite spriteNavProfileOff;
        public Sprite spriteNavMoreOn;
        public Sprite spriteNavMoreOff;
        
        
        
        [Header("== 컬러 ==")]
        public Color colorPremiumBox = Color.white;
        public Color colorFreeBox = Color.white;
        public Color colorOneTimeBox = Color.white;
        public Color colorNavOn = Color.white;
        public Color colorNavOff = Color.white;
        

        public bool termView = false;
       

        [Space]
        [SerializeField] NetworkLoadingScreen lobbyNetworkLoadingScreen; 


        private void Awake()
        {
            main = this;
        }

        
        void Start() {

            // * 팝업매니저 초기화(로비씬)
            PopupManager.main.InitPopupManager(); 
        
        }
        
        private void Update() {
            if(Input.GetKeyDown(KeyCode.O)) {
                PopupBase popup = PopupManager.main.GetPopup("AchivementIllust");
                popup.Data.SetLabelsTexts("가짜이름");
                PopupManager.main.ShowPopup(popup, true, false);
            }
            
            if(Input.GetKeyDown(KeyCode.C)) {
                //SystemManager.ShowConfirmPopUp("TEST", null, null);
                PopupBase p = PopupManager.main.GetPopup("PremiumPass");
                
                PopupManager.main.ShowPopup(p, false, false);
            }
            if(Input.GetKeyDown(KeyCode.D)) {
                SystemManager.ShowAlert("경고 메세지 테스트");
            }
            
            if(Input.GetKeyDown(KeyCode.P)) {
                // AdManager.main.ShowRewardAd();
                SystemManager.ShowNetworkLoading();
            }
            else if(Input.GetKeyDown(KeyCode.L)) {
                SystemManager.HideNetworkLoading();
            }
        }

        public void InitLobbyScene()
        {
            Debug.Log("<color=cyan>LobbyManager Start</color>");

            // 게임씬에서 넘어왔다면 곧바로 StoryDetail로 넘겨줘야 한다. 
            if (SystemManager.main.givenStoryData != null)
            {
                //StoryManager.main.RequestStoryInfo(StoryManager.main.CurrentProjectID, SystemManager.main.givenStoryData);
            }
            else
            {
                // 첫 시작시, 타이틀로 이동을 요청합니다. 
                //GameEventMessage.SendEvent(SystemManager.EVENT_GO_TITLE);
            }
        }



        public void SetIllustParent(Transform __model)
        {
            __model.SetParent(transform);
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
        
        
        
        /// <summary>
        /// 갤러리의 라이브 일러스트 처리!
        /// </summary>
        /// <param name="__name"></param>
        /// <param name="__scale"></param>
        public void SetGalleryLiveIllust(string __name, int __scale, bool liveObj)
        {
            scaleOffset = __scale;
            illustName = __name;
            
            
            if(!liveObj) {
                currentLiveIllust = new ScriptLiveMount(illustName, OnGalleryLiveIllustMount, this, false);
                currentLiveIllust.SetModelDataFromStoryManager();

            }
            else { // 라이브 오브제 추가 
                currentLiveObject = new ScriptLiveMount(illustName, OnGalleryLiveObjectMount, this, true);
                currentLiveObject.SetModelDataFromStoryManager();
            }
        }
        
        /// <summary>
        /// 갤러리 Live Object 마운트 완료 
        /// </summary>
        void OnGalleryLiveObjectMount() {
            
            if(currentLiveObject == null || currentLiveObject.liveImage == null) {
                Debug.LogError("Something wrong in OnGalleryLiveObjectMount");
                return;
            }
            
            currentLiveObject.liveImage.transform.localScale = new Vector3(currentLiveObject.gameScale , currentLiveObject.gameScale, 1);

            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SHOW_ILLUSTDETAIL, string.Empty);
            //GameEventMessage.SendEvent("EventIllustDetail");
        }

        void OnGalleryLiveIllustMount()
        {
            Debug.Log(string.Format("OnGalleryLiveIllustMount gameScale({0})/scaleOffset({1})", currentLiveIllust.gameScale, scaleOffset));
            float scale = currentLiveIllust.gameScale + scaleOffset;

            currentLiveIllust.liveImage.transform.localScale = new Vector3(scale, scale, 1);

            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SHOW_ILLUSTDETAIL, string.Empty);
            //GameEventMessage.SendEvent("EventIllustDetail");
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
            
            if(data == null || data.Count == 0)
                return null;
            
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
            JsonData data = JsonMapper.ToObject(res.DataAsText);
            string imageKey = string.Empty;
            string imageURL = string.Empty;

            for(int i=0;i<data.Count;i++)
            {
                if(data[i]["image_key"] == null)
                    continue;
                
                imageKey = data[i]["image_key"].ToString();
                imageURL = data[i]["image_url"].ToString();
                
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