using UnityEngine;

using TMPro;
using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    /// <summary>
    /// 엔딩 모음 페이지에서 사용되는 엔딩요소를 이룬다
    /// </summary>
    public class EndingElement : MonoBehaviour
    {
        public ImageRequireDownload endingBanner;
        public TextMeshProUGUI endingType;
        public TextMeshProUGUI endingTitle;

        EpisodeData endingData;

        public void InitEndingInfo(EpisodeData epiData)
        {
            endingData = epiData;
            endingBanner.SetDownloadURL(epiData.popupImageURL, epiData.popupImageKey);

            if (epiData.endingType == LobbyConst.COL_HIDDEN)
                endingType.text = SystemManager.GetLocalizedText("5087");
            else
                endingType.text = SystemManager.GetLocalizedText("5088");

            endingTitle.text = epiData.episodeTitle;

            gameObject.SetActive(true);
        }


        #region OnClick Button Event

        /// <summary>
        /// 엔딩 플레이!
        /// </summary>
        public void OnClickStartEnding()
        {
            UserManager.main.useRecord = false; // 엔딩 플레이는 useRecord를 false 처리한다. 
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, endingData, string.Empty);
        }

        /// <summary>
        /// 엔딩 선택지 보여주기
        /// </summary>
        public void OnClickShowSelection()
        {
            JsonData j = new JsonData();
            j[CommonConst.FUNC] = "getEndingSelectionList";
            j[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            j[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            j["ending_id"] = endingData.episodeID;
            j[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;

            NetworkLoader.main.SendPost(CallbackEndingSelection, j, true);

        }

        void CallbackEndingSelection(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackEndingSelection");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);
            Debug.Log(JsonMapper.ToStringUnicode(result));

            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_ENDINGDATA, endingData, string.Empty);
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SHOW_ENDINGDETAIL, result, string.Empty);
        }

        #endregion
    }
}
