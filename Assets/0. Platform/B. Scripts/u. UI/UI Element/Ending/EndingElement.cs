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

        public UnityEngine.UI.Image replayIcon;
        public GameObject showChoiceButton;

        readonly Vector2 openEndingSize = new Vector2(660, 500);
        readonly Vector2 lockEndingSize = new Vector2(660, 435);

        EpisodeData endingData;

        public void InitEndingInfo(EpisodeData epiData)
        {
            endingData = epiData;
            endingBanner.SetDownloadURL(epiData.popupImageURL, epiData.popupImageKey);

            if (epiData.endingType == LobbyConst.COL_HIDDEN)
                endingType.text = SystemManager.GetLocalizedText("5087");
            else
                endingType.text = SystemManager.GetLocalizedText("5088");


            if (endingData.endingOpen)
            {
                endingTitle.text = epiData.episodeTitle;
                GetComponent<RectTransform>().sizeDelta = openEndingSize;
                showChoiceButton.SetActive(true);
            }
            else
            {
                EpisodeData dependEpisodeData = StoryManager.GetRegularEpisodeByID(epiData.dependEpisode);

                endingTitle.text = string.Format("{0}\n EP{1}", "귀속 에피소드", dependEpisodeData.episodeNO);
                GetComponent<RectTransform>().sizeDelta = lockEndingSize;
                showChoiceButton.SetActive(false);
            }

            gameObject.SetActive(true);
        }


        #region OnClick Button Event

        /// <summary>
        /// 엔딩 플레이!
        /// </summary>
        public void OnClickStartEnding()
        {
            // 아직 열지 못했다면 플레이 놉!
            if (!endingData.endingOpen)
                return;


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
