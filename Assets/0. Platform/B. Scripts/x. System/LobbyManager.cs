using UnityEngine;
using BestHTTP;


using LitJson;
using Toast.Gamebase;
using Doozy.Runtime.Signals;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace PIERStory {

    /// <summary>
    /// Lobby 씬에서만 존재합니다. 
    /// 각 씬에만 존재하는 singleton 개체가 필요하다(LobbyManager, GameManager)
    /// </summary>
    public class LobbyManager : MonoBehaviour {

        public static LobbyManager main = null;

        public bool isLobbyManagerInit = false;        

        public ScriptLiveMount currentLiveIllust = null; // Live Illust for Gallery 
        public ScriptLiveMount currentLiveObject = null; // Live Object for Gallery
        public TouchEffect touchEffect;                  // 일러스트 상세에서 터치 이펙트 제어용
        
        int scaleOffset = 0;
        string illustName = string.Empty;

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


        [Header("프로필")]
        public Texture2D textureNoneFrame;

        public Sprite spriteBronzeBadge;
        public Sprite spriteSilverBadge;
        public Sprite spriteGoldBadge;
        public Sprite spritePlatinumBadge;
        public Sprite spriteIFYOUBadge;

        [Space]
        public Sprite spriteLevelTag1;
        public Sprite spriteLevelTag2;
        public Sprite spriteLevelTag3;
        public Sprite spriteLevelTag4;

        [Header("작품 로비")]
        public SpriteRenderer lobbyBackground;


        
        [Header("갤러리 - 사운드 BGMSprite")]
        public Sprite spriteOpenVoice;
        public Sprite spriteLockVoice;
        public Sprite toggleSelected;
        public Sprite toggleUnselected;

        [Header("미션View Sprite")]
        public Sprite spriteGetReward;          // 얻을 수 있는 보상 버튼
        public Sprite spriteGotReward;          // 얻은 보상 버튼
        
        [Header("에피소드 관련 Sprite")]
        public Sprite spriteEpisodeOpen;    // 열린 스페셜, 엔딩 에피소드
        public Sprite spriteEpisodeLock;    // 잠긴 스페셜, 엔딩 에피소드
        
       

        
        [Header("== 컬러 ==")]
        
        public Color colorEndingFutureCover; // 엔딩 미래 커버
        public Color colorEndingPastCover;  // 엔딩 과거 커버 
        


        [Space]
        [SerializeField] NetworkLoadingScreen lobbyNetworkLoadingScreen; 


        private void Awake()
        {
            main = this;
        }

        
        System.Collections.IEnumerator Start() {



            // * 팝업매니저 초기화(로비씬)
            PopupManager.main.InitPopupManager();
            
            yield return null;
            yield return new WaitForSeconds(0.1f);
            
            if(BubbleManager.main != null) {
                BubbleManager.main.ShowFakeBubbles(false);
            }
            
            ViewCommonTop.OnBackAction = null;
            
            // 어드레서블 카탈로그 로드 
            InitAddressableCatalog();
            
            // * 로비씬 시작을 알린다. 
            Signal.Send(LobbyConst.STREAM_COMMON, "LobbyPlay"); 
            
            isLobbyManagerInit = true;
        }
        
        
        
        /// <summary>
        /// 
        /// </summary>
        void InitAddressableCatalog() {
            
            Debug.Log("#### InitAddressableCatalog ###");
            string catalogURL = string.Empty;
            
            // 테스트용
            // Addressables.ClearDependencyCacheAsync("Font");
            
            
            #if UNITY_IOS
            catalogURL = "https://d2dvrqwa14jiay.cloudfront.net/bundle2021/iOS/catalog_1.json";
            
            #else
            catalogURL = "https://d2dvrqwa14jiay.cloudfront.net/bundle2021/Android/catalog_1.json";
            // catalogURL = "https://d2dvrqwa14jiay.cloudfront.net/test_bundle/Android/catalog_1.json";
            
            #endif
            
            Debug.Log("### InitAddressableCatalog URL ::  " +  catalogURL);
            
            Addressables.LoadContentCatalogAsync(catalogURL).Completed += (op) => {
            
                if(op.Status == AsyncOperationStatus.Succeeded) {
                    Debug.Log("### InitAddressableCatalog " +  op.Status.ToString());
                    SystemManager.main.isAddressableCatalogUpdated = true;
                    return;
                }
                else {
                    
                    // 한번더 시도한다. 
                    Addressables.LoadContentCatalogAsync(catalogURL).Completed += (op) => {
            
                        if(op.Status == AsyncOperationStatus.Succeeded) {
                            Debug.Log("### InitAddressableCatalog #2" +  op.Status.ToString());
                            SystemManager.main.isAddressableCatalogUpdated = true;
                            
                            return;
                        }
                        else {
                            
                            NetworkLoader.main.ReportRequestError(op.OperationException.ToString(), "LoadContentCatalogAsync");
                            SystemManager.main.isAddressableCatalogUpdated = false;
                            
                            
                            //  카탈로그 실패시 접속 할 수 없음. 
                            SystemManager.ShowSystemPopup(SystemManager.GetDefaultServerErrorMessage(), NetworkLoader.OnFailedServer, NetworkLoader.OnFailedServer, false, false);
                            return;
                            
                        }
                        
                    }; // end of second try
                    
                    
                } // end of else
                
            }; // END!!!
        }
        
        
        
        private void Update() {
            /*
            if(Input.GetKeyDown(KeyCode.O)) {
                SystemManager.ShowIntroPopup();
            }
            */

            /*
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
            */
            
            
            if(Input.GetKeyDown(KeyCode.R)) { // 경험치 획득 
                // // NetworkLoader.main.UpdateUserExp(50, "event", -1);
                // PopupBase sidePopup = PopupManager.main.GetPopup(GameConst.POPUP_SIDE_ALERT);
                // PopupManager.main.ShowPopup(sidePopup, true);
                
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, "showIntro", string.Empty);
                
            }
        }



        public void SetLiveParent(Transform __model)
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
                currentLiveIllust.SetModelDataFromStoryManager(true);

            }
            else { // 라이브 오브제 추가 
                currentLiveObject = new ScriptLiveMount(illustName, OnGalleryLiveObjectMount, this, true);
                currentLiveObject.SetModelDataFromStoryManager(true);
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