using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

namespace PIERStory
{
    public class ViewLoadingGame :CommonView
    {
        public ImageRequireDownload loadingImage;

        public TextMeshProUGUI textTitle;
        public TextMeshProUGUI textInfo;

        int loadingTextIndex = 0;

        public override void OnStartView()
        {
            base.OnStartView();

            textTitle.text = GameManager.main.currentEpisodeData.episodeTitle;
            StartCoroutine(RoutineGameLoading());
        }

        void CallbackDownloadLoadingImage()
        {

        }

         IEnumerator RoutineGameLoading()
        {
            // 게임 매니저에서 에피소드 스크립트 정보 가져올 때까지 대기
            yield return new WaitUntil(() => GameManager.main.isScriptFetch);

            if (GameManager.main.loadingJson.Count > 0)
                loadingImage.SetDownloadURL(SystemManager.GetJsonNodeString(GameManager.main.loadingJson[0], CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(GameManager.main.loadingJson[0], CommonConst.COL_IMAGE_KEY));
            else
                loadingImage.SetDownloadURL(string.Empty, string.Empty);

            if(GameManager.main.loadingDetailJson.Count >0)
            {
                loadingTextIndex = 0;
                textInfo.text = SystemManager.GetJsonNodeString(GameManager.main.loadingDetailJson[loadingTextIndex], "loading_text");
            }

            yield return new WaitUntil(() => GameManager.main.GetCurrentPageInitialized());

            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_GAME, "gameLoadingComplete", string.Empty);
        }
    }
}
