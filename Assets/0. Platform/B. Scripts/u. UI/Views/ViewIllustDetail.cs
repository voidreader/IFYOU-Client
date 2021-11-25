using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

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
        public Transform livePillar;        // live2D가 생성될 곳

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

            if(!isLive)
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
                    //LobbyManager.main.currentLiveObject.PlayCubismAnimation();
                }
                else
                {
                    liveRenderTexture.rectTransform.sizeDelta = new Vector2(viewRect.rect.height, viewRect.rect.height);
                    //LobbyManager.main.currentLiveIllust.PlayCubismAnimation();
                }
            }

            illustTitle.text = title;
            illustSummary.text = summary;
        }

        void IllustSetNativeSize()
        {
            illustImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1755);
        }

        public void HideView()
        {
            if (livePillar.childCount == 0)
                return;

            /*
             * 21.11.24 임시적으로 주석
            if (LobbyManager.main.currentLiveIllust != null && LobbyManager.main.currentLiveIllust.liveImageController != null)
                LobbyManager.main.currentLiveIllust.liveImageController.DestroySelf();

            if (LobbyManager.main.currentLiveObject != null && LobbyManager.main.currentLiveObject.liveImageController != null)
                LobbyManager.main.currentLiveObject.liveImageController.DestroySelf();
            */

            // 연타 클릭되어 생성된 것이 있다면 파괴(혹시 모를 안전장치)
            if (livePillar.childCount > 0)
            {
                GameLiveImageCtrl[] gameLive = livePillar.GetComponentsInChildren<GameLiveImageCtrl>();

                foreach (GameLiveImageCtrl gl in gameLive)
                    gl.DestroySelf();
            }
        }
    }
}
