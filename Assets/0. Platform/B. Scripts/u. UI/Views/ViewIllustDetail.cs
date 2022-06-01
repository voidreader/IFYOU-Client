using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using BestHTTP;

namespace PIERStory
{
    public class ViewIllustDetail : CommonView
    {
        static JsonData illustData;     // 일러스트 상세 데이터
        static JsonData userGalleryData = null; // 유저 갤러리 데이터 

        static string title = string.Empty;
        static string summary = string.Empty;
        static bool isLive = false;
        static bool isMinicut = false;

        public RectTransform viewRect;
        
        public ImageRequireDownload illustImage;
        public Image image;
        
        public RawImage liveRenderTexture;

        public GameObject illustContents;
        public TextMeshProUGUI illustTitle;
        public TextMeshProUGUI illustSummary;
        
        public GameObject shareBonus; // 대상 일러스트에 공유 보너스 있음!
        public GameObject shareBox; // 공유 확인 팝업 
        public bool isShareBonusGet = false; // 공유 보너스 맞았었는지 
        public GameObject buttonShare;
        
        [Space]        
        public ScriptImageMount imageMount;

        public static void SetData(JsonData __j, bool __live, bool __minicut, string __title, string __summary, JsonData __userGalleryData)
        {
            illustData = __j;
            Debug.Log(JsonMapper.ToStringUnicode(illustData));
            
            userGalleryData = __userGalleryData;

            isLive = __live;
            isMinicut = __minicut;

            title = __title;
            summary = __summary;
        }

        public override void OnView()
        {
            base.OnView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);

            ViewGallery.OnDelayIllustOpen?.Invoke(true);
            SystemManager.HideNetworkLoading();
            
            
            RequestLobbyOpen();
        }
        
        public override void OnStartView()
        {
            base.OnStartView();
            illustImage.gameObject.SetActive(false);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
            
            // live2D 아닌 경우
            if (!isLive)
            {
                // 일반 이미지, 에셋번들부터 확인할 수 있게 Mount로 변경한다. 
                
                string imageType = GameConst.TEMPLATE_ILLUST;
                
                if(isMinicut) {
                    imageType = GameConst.TEMPLATE_IMAGE;
                }

                // OnCompleteImageLoad 에서 이미지 활성화 
                imageMount = new ScriptImageMount(imageType, illustData, OnCompleteImageLoad);

            }
            else
            {
                illustImage.gameObject.SetActive(false);

                if (isMinicut)
                {
                    liveRenderTexture.rectTransform.sizeDelta = new Vector2(viewRect.rect.width, viewRect.rect.width);
                    LobbyManager.main.currentLiveObject.PlayCubismAnimation();
                }
                else
                {
                    liveRenderTexture.rectTransform.sizeDelta = new Vector2(viewRect.rect.height, viewRect.rect.height);
                    LobbyManager.main.currentLiveIllust.PlayCubismAnimation();
                }
            }
            
            isShareBonusGet = SystemManager.GetJsonNodeBool(userGalleryData, "share_bonus");

            illustTitle.text = title;
            illustSummary.text = summary;
            // buttonIcon.sprite = spriteEyeOpen;
            illustContents.SetActive(true);
            
            shareBonus.SetActive(!isShareBonusGet);
            HideShareBox();

            LobbyManager.main.touchEffect.gameObject.SetActive(false);
        }

        public override void OnHideView()
        {
            base.OnHideView();
            
            // 시그널 보내고 
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);
            
            
            // 터치 이펙트 활성화, 
            LobbyManager.main.touchEffect.gameObject.SetActive(true);
            
            // 이미지 비활성화 
            illustImage.gameObject.SetActive(false);
            
            // 로딩 하이드.
            SystemManager.HideNetworkLoading(); 
            
            try {
                // 일반 이미지의 경우 어드레서블 체크해서 Release. 
                if(!isLive && imageMount != null && imageMount.isAddressable && imageMount.isMounted) {
                    imageMount.DestroyAddressable();
                }
            }
            catch {
                
            }
            

            if (LobbyManager.main.transform.childCount < 1)
                return;


            if (LobbyManager.main.currentLiveIllust != null && LobbyManager.main.currentLiveIllust.liveImageController != null) {
                
                LobbyManager.main.currentLiveIllust.DestroyAddressableModel();
                LobbyManager.main.currentLiveIllust = null;
            }

            if (LobbyManager.main.currentLiveObject != null && LobbyManager.main.currentLiveObject.liveImageController != null) {
                LobbyManager.main.currentLiveObject.DestroyAddressableModel();
                LobbyManager.main.currentLiveObject = null;
            }


            // 연타 클릭되어 생성된 것이 있다면 파괴(혹시 모를 안전장치)
            if (LobbyManager.main.transform.childCount > 0)
            {
                GameLiveImageCtrl[] gameLive = LobbyManager.main.GetComponentsInChildren<GameLiveImageCtrl>();

                foreach (GameLiveImageCtrl gl in gameLive)
                    gl.DestroySelf();
            }

            
            
            // System.GC.Collect();
        }
        
        
        /// <summary>
        /// 이미지 로드 완료됨 (미니컷 이미지, 일반 일러스트)
        /// </summary>
        void OnCompleteImageLoad() {
            
            illustImage.gameObject.SetActive(true);
            
            if(imageMount.isAddressable && imageMount.isMounted) {
                image.sprite = imageMount.sprite;
                image.SetNativeSize();
                
                if(isMinicut) {
                    MinicutResize();
                }
                else {
                    IllustResize();
                }
                
            }
            else {
                illustImage.SetDownloadURL(SystemManager.GetJsonNodeString(illustData, CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(illustData, CommonConst.COL_IMAGE_KEY), true);
                
                if(isMinicut) {
                    illustImage.OnDownloadImage = MinicutResize;
                }
                else {
                    illustImage.OnDownloadImage = IllustResize;
                }
            }
        }
        
        
        void Update() {
            if(Input.GetKeyDown(KeyCode.C)) {
                HideUpperUI();
            }
            else if(Input.GetKeyDown(KeyCode.X)) {
                ShowUpperUI();
            }
        }


        /// <summary>
        /// 일러스트 사이즈 조정
        /// </summary>
        void IllustResize() {
            image.rectTransform.localScale = Vector3.one * 0.76f;
        }
        
        /// <summary>
        /// 미니컷 사이즈 조정 
        /// </summary>
        void MinicutResize()
        {
            illustImage.GetComponent<Image>().SetNativeSize();

            float ratioScale = 1f;

            // 화면 비율에 따라서 비율 조절
            if (SystemManager.screenRatio >= 0.4f)
                ratioScale = 0.65f;

            if (SystemManager.screenRatio >= 0.75f)
                ratioScale = 0.9f;

            illustImage.GetComponent<Image>().rectTransform.localScale = Vector3.one * ratioScale;
        }

        public void OnClickAcitveIllustContents()
        {
            /*
            if(illustContents.activeSelf)
            {
                illustContents.SetActive(false);
                buttonIcon.sprite = spriteEyeClose;
            }
            else
            {
                illustContents.SetActive(true);
                buttonIcon.sprite = spriteEyeOpen;
            }
            */
        }
        
        
        /// <summary>
        /// 공유 팝업 보여주고 닫기 
        /// </summary>
        public void ShowShareBox() {
            shareBox.SetActive(true);
        }
        
        public void HideShareBox() {
            shareBox.SetActive(false);
        }
        
        
        /// <summary>
        /// 공유할때 위 UI 보였다가 안보였다 하기 
        /// </summary>
        public void HideUpperUI() {
            buttonShare.SetActive(false);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_PARENT, false, string.Empty);
        }
        
        public void ShowUpperUI() {
            buttonShare.SetActive(true);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_PARENT, true, string.Empty);
        }
        
        
        /// <summary>
        /// 페이스북 공유
        /// </summary>
        public void OnClickFacebookShare() {
            StartCoroutine(RoutinePost("Facebook"));      
            
        }
        
        /// <summary>
        /// 트위터 공유
        /// </summary>
        public void OnClickTwitterShare() {
            StartCoroutine(RoutinePost("Twitter"));
            
        }
        
        IEnumerator RoutinePost(string __type) {
            
            if(Application.isEditor) {
                RequestShareBonus();
                yield break;
            }
            
            bool isAvailable = false;
            SocialShareComposerType shareType = SocialShareComposerType.WhatsApp;
            
            if(__type == "Facebook") {
                shareType = SocialShareComposerType.Facebook;
                isAvailable = SocialShareComposer.IsComposerAvailable(shareType);
            }
            else if(__type == "Twitter") {
                shareType = SocialShareComposerType.Twitter;
                isAvailable = SocialShareComposer.IsComposerAvailable(shareType);
            }
            else {
                isAvailable = true;
            }
                
                
            if(!isAvailable) { // 트위터 인스톨 되어있지 않음 
                SystemManager.ShowMessageAlert(string.Format(SystemManager.GetLocalizedText("6253"), __type));
                yield break;
            }
            
            HideUpperUI();
            HideShareBox();
            
            // 위에서 시그널 처리가 있어서 프레임 3회 대기 
            yield return null;
            yield return null;
            yield return null;
            
            // * 페이스북, 트위터 
            if(shareType != SocialShareComposerType.WhatsApp) {
                SocialShareComposer composer = SocialShareComposer.CreateInstance(shareType);
                
                // 텍스트랑 스크린샷 같이 가능한건 트위터만 가능 
                if(shareType == SocialShareComposerType.Twitter)                
                    composer.SetText("#IFyou #Episode #StoryGame\nDownload right now! : http://onelink.to/g9ja38");
                
                composer.AddScreenshot();
                composer.SetCompletionCallback((result, error) => {
                    Debug.Log("Social Share Composer was closed. Result code: " + result.ResultCode);
                    
                    ShowUpperUI(); // 상단 다시 복귀 
                    
                    // 통신 시작 
                    // 보상을 받은적이 없는 경우만 처리 
                    if(!isShareBonusGet) {
                        RequestShareBonus();   
                    }
                    
                    
                });
                
                // Show 
                composer.Show();                    
            }
            else {
                // * 여기는 기타 공유    
            }
         
        } // ? end of routine
        
        
        /// <summary>
        /// 공유하고 보상 요청하기 
        /// </summary>
        public void RequestShareBonus() {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestGalleryShareBonus";
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            sending["illust_type"] = SystemManager.GetJsonNodeString(userGalleryData, "illust_type");
            sending["illust_id"] = SystemManager.GetJsonNodeInt(userGalleryData, "illust_id");
            
            NetworkLoader.main.SendPost(CallbackRequestShareBonus, sending, true);
        }
        
        void CallbackRequestShareBonus(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRequestShareBonus");
                return;
            }

            // Debug.Log(string.Format("CallbackConnectServer: {0}", res.DataAsText));
            JsonData result = JsonMapper.ToObject(res.DataAsText);
            
            Debug.Log("### CallbackRequestShareBonus : " + res.DataAsText);
            
            // bank
            // galleryImages
            // currency
            // quantity 
            
            UserManager.main.SetBankInfo(result); // 뱅크 
            UserManager.main.SetNodeUserGalleryImages(result["galleryImages"]); // 갤러리 이미지 
            
            int quantity = SystemManager.GetJsonNodeInt(result, "quantity");
            
            // 획득팝업 띄우기             
            SystemManager.ShowResourcePopup(string.Format(SystemManager.GetLocalizedText("6256"), quantity), result["currency"].ToString(), quantity);
            
            isShareBonusGet = true;
            shareBonus.SetActive(false);
            
            // 갤러리 리프레시..!
            ViewGallery.ActionRefreshGallery?.Invoke();
        }        
        
        
        /// <summary>
        /// 갤러리에서 오픈시 오픈했다고 기록을 남긴다. 
        /// </summary>
        void RequestLobbyOpen() {
            
            if(SystemManager.GetJsonNodeBool(userGalleryData, "gallery_open"))
                return;
            
            JsonData sending = new JsonData();
            sending["func"] = "requestGalleryLobbyOpen";
            sending["project_id"] = StoryManager.main.CurrentProjectID;
            sending["illust_type"] = SystemManager.GetJsonNodeString(userGalleryData, "illust_type");
            sending["illust_id"] = SystemManager.GetJsonNodeInt(userGalleryData, "illust_id");            
            
            NetworkLoader.main.SendPost(CallbackRequestLobbyOpen, sending, true);
        }
        
        void CallbackRequestLobbyOpen(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRequestLobbyOpen");
                return;
            }
            
            JsonData result = JsonMapper.ToObject(res.DataAsText);
            UserManager.main.SetNodeUserGalleryImages(result["galleryImages"]); // 갤러리 정보 갱신
            
            ViewGallery.ActionRefreshGallery?.Invoke(); // 갤러리 뷰 리프레시 
            StoryLobbyMain.OnInitializeContentGroup?.Invoke(); // 스토리 로비 메인 컨텐츠 그룹 갱신 
        }
        
    }
}
