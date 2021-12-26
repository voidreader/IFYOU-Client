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
        
        public ImageRequireDownload illustImage;
        public RawImage liveRenderTexture;

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


        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);

            ViewGallery.OnDelayIllustOpen?.Invoke(true);

            // live2D 아닌 경우
            if (!isLive)
            {
                illustImage.gameObject.SetActive(true);
                illustImage.SetDownloadURL(SystemManager.GetJsonNodeString(illustData, CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(illustData, CommonConst.COL_IMAGE_KEY));

                if (isMinicut)
                    illustImage.OnDownloadImage = illustImage.GetComponent<Image>().SetNativeSize;
                else
                    illustImage.OnDownloadImage = IllustSetNativeSize;
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
        }

        public override void OnHideView()
        {
            base.OnHideView();

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
    }
}
