﻿using UnityEngine;

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

        public UnityEngine.UI.Image buttonBox;
        public GameObject newSign;
        public GameObject showChoiceButton;
        public GameObject endingHintButton;

        /// <summary>
        /// 엔딩 힌트 데이터 존재 여부
        /// </summary>
        bool hasEndingHintData
        {
            get {

                foreach (EndingHintData hintData in StoryManager.main.endingHintList)
                {
                    if (hintData.endingId == endingData.episodeID)
                        return true;
                }

                return false;
            }
        }

        readonly Vector2 openEndingSize = new Vector2(660, 553);
        readonly Vector2 lockEndingSize = new Vector2(660, 488);

        EpisodeData endingData;

        public void InitEndingInfo(EpisodeData epiData)
        {
            endingData = epiData;
            endingBanner.SetDownloadURL(epiData.popupImageURL, epiData.popupImageKey);

            if (epiData.endingType == LobbyConst.COL_HIDDEN)
                SystemManager.SetText(endingType, SystemManager.GetLocalizedText("5087"));
            else
                SystemManager.SetText(endingType, SystemManager.GetLocalizedText("5088"));


            if (endingData.endingOpen)
            {
                GetComponent<RectTransform>().sizeDelta = openEndingSize;
                buttonBox.sprite = StoryLobbyManager.main.spriteEpisodeOpen;
                buttonBox.rectTransform.anchoredPosition = new Vector2(0, 62);

                newSign.SetActive(!UserManager.main.IsCompleteEpisode(epiData.episodeID));
            }
            else
            {
                GetComponent<RectTransform>().sizeDelta = lockEndingSize;
                buttonBox.sprite = StoryLobbyManager.main.spriteEpisodeLock;
                buttonBox.rectTransform.anchoredPosition = Vector2.zero;

                newSign.SetActive(false);
            }

            SystemManager.SetText(endingTitle, string.Format("{0}\n<size=22>{1}</size>", epiData.episodeTitle, epiData.episodeSummary));

            showChoiceButton.SetActive(endingData.endingOpen);
            endingHintButton.SetActive(!endingData.endingOpen && hasEndingHintData);

            gameObject.SetActive(true);
        }


        #region OnClick Button Event

        public void OnClickEndingHintButton()
        {
            PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_ENDING_HINT);

            if(p == null)
            {
                Debug.LogError("엔딩 힌트 팝업 없음!");
                return;
            }

            p.Data.contentEpisode = endingData;
            //p.Data.SetLabelsTexts(string.Format("{0} {1}", SystemManager.GetLocalizedText("5025"), SystemManager.GetLocalizedText("5201")));
            PopupManager.main.ShowPopup(p, false);
        }


        /// <summary>
        /// 엔딩 플레이!
        /// </summary>
        public void OnClickStartEnding()
        {
            // 아직 열지 못했다면 플레이 놉!
            if (!endingData.endingOpen)
                return;

            UserManager.main.useRecord = false;         // 엔딩 플레이는 useRecord를 false 처리한다. 

            SystemManager.main.givenEpisodeData = endingData;
            SystemManager.ShowNetworkLoading();

            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_GAME_BEGIN, string.Empty);
            //IntermissionManager.isMovingLobby = false;  // 게임으로 진입하도록 요청
            //
            //if (GameManager.main != null)
            //    SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
            //else
            //    SceneManager.LoadSceneAsync(CommonConst.SCENE_GAME, LoadSceneMode.Single).allowSceneActivation = true;

            GameManager.SetNewGame();
            // 통신 
            NetworkLoader.main.UpdateUserProjectCurrent(endingData.episodeID, null, 0, false, "OnClickStartEnding");
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
