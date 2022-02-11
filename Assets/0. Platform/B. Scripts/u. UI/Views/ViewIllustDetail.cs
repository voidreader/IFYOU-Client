using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewIllustDetail : CommonView
    {
        static JsonData illustData;     // 일러스트 상세 데이터

        static string title = string.Empty;
        static string summary = string.Empty;
        static bool isLive = false;
        static bool isMinicut = false;

        public RectTransform viewRect;
        public Image buttonIcon;        // 일러스트 상세 내용 보이기/숨기기 버튼 아이콘
        public Sprite spriteEyeOpen;
        public Sprite spriteEyeClose;
        
        public ImageRequireDownload illustImage;
        public RawImage liveRenderTexture;

        public GameObject illustContents;
        public TextMeshProUGUI illustTitle;
        public TextMeshProUGUI illustSummary;

        public static void SetData(JsonData __j, bool __live, bool __minicut, string __title, string __summary)
        {
            illustData = __j;

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
        }
        
        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
            
            // live2D 아닌 경우
            if (!isLive)
            {
                illustImage.gameObject.SetActive(true);

                if (isMinicut)
                    illustImage.OnDownloadImage = MinicutResize;
                else
                    illustImage.OnDownloadImage = IllustSetNativeSize;

                illustImage.SetDownloadURL(SystemManager.GetJsonNodeString(illustData, CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(illustData, CommonConst.COL_IMAGE_KEY));
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

            illustTitle.text = title;
            illustSummary.text = summary;
            buttonIcon.sprite = spriteEyeOpen;
            illustContents.SetActive(true);
        }

        public override void OnHideView()
        {
            base.OnHideView();
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);

            if (LobbyManager.main.transform.childCount < 1)
                return;


            if (LobbyManager.main.currentLiveIllust != null && LobbyManager.main.currentLiveIllust.liveImageController != null)
                LobbyManager.main.currentLiveIllust.liveImageController.DestroySelf();

            if (LobbyManager.main.currentLiveObject != null && LobbyManager.main.currentLiveObject.liveImageController != null)
                LobbyManager.main.currentLiveObject.liveImageController.DestroySelf();


            // 연타 클릭되어 생성된 것이 있다면 파괴(혹시 모를 안전장치)
            if (LobbyManager.main.transform.childCount > 0)
            {
                GameLiveImageCtrl[] gameLive = LobbyManager.main.GetComponentsInChildren<GameLiveImageCtrl>();

                foreach (GameLiveImageCtrl gl in gameLive)
                    gl.DestroySelf();
            }
        }

        void IllustSetNativeSize()
        {
            illustImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1755);
        }

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
        }
    }
}
