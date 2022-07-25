using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewEndingDetail : CommonView
    {
        public ImageRequireDownload endingBanner;
        public TextMeshProUGUI endingType;
        public TextMeshProUGUI endingTitle;
        public GameObject newSign;

        public Transform scrollContent;

        [Space][Header("엔딩 선택지 관련")]
        public GameObject episodeTitlePrefab;
        public GameObject selectionScriptPrefab;
        public GameObject emptyPrefab;
        public GameObject endingTitlePrefab;

        List<GameObject> createObject = new List<GameObject>();

        EpisodeData endingData = null;
        JsonData selectionData = null;

        SignalReceiver signalReceiverEndingData;
        SignalReceiver signalReceiverSelectionData;

        SignalStream signalStreamEndigData;
        SignalStream signalStreamSelectionData;

        const string KEY_SELECTION_CONTENT = "selection_content";

        #region Signal 수신 처리

        private void Awake()
        {
            signalStreamEndigData = SignalStream.Get(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_ENDINGDATA);
            signalReceiverEndingData = new SignalReceiver().SetOnSignalCallback(OnEndingSignal);

            signalStreamSelectionData = SignalStream.Get(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SHOW_ENDINGDETAIL);
            signalReceiverSelectionData = new SignalReceiver().SetOnSignalCallback(OnSelctionSignal);
        }

        private void OnEnable()
        {
            signalStreamEndigData.ConnectReceiver(signalReceiverEndingData);
            signalStreamSelectionData.ConnectReceiver(signalReceiverSelectionData);
        }

        private void OnDisable()
        {
            signalStreamEndigData.DisconnectReceiver(signalReceiverEndingData);
            signalStreamSelectionData.DisconnectReceiver(signalReceiverSelectionData);
        }

        void OnEndingSignal(Signal s)
        {
            if(s.hasValue)
                endingData = s.GetValueUnsafe<EpisodeData>();
        }

        void OnSelctionSignal(Signal s)
        {
            if(s.hasValue)
                selectionData = s.GetValueUnsafe<JsonData>();
        }

        #endregion


        public override void OnStartView()
        {
            base.OnStartView();

            // 엔딩 배너, 타입, 제목 세팅
            endingBanner.SetDownloadURL(endingData.popupImageURL, endingData.popupImageKey);

            if (endingData.endingType == LobbyConst.COL_HIDDEN)
                SystemManager.SetLocalizedText(endingType,"5087");
            else
                SystemManager.SetLocalizedText(endingType, "5088");

            SystemManager.SetText(endingTitle, string.Format("{0}\n<size=22>{1}</size>", endingData.episodeTitle, endingData.episodeSummary));

            endingType.gameObject.SetActive(false);
            endingTitle.gameObject.SetActive(false);
            //  newSign.SetActive(!UserManager.main.IsCompleteEpisode(endingData.episodeID));

            int episodeIndex = 0;

            // 선택지 세팅
            foreach (string key in selectionData.Keys)
            {
                // 정규 에피소드 넘버링, 제목
                SelectionEpisodeTitleElement titleElement = Instantiate(episodeTitlePrefab, scrollContent).GetComponent<SelectionEpisodeTitleElement>();
                titleElement.SetEpisodeTitle(episodeIndex + 1, key);
                createObject.Add(titleElement.gameObject);
                episodeIndex++;

                for (int i = 0; i < selectionData[key].Count; i++)
                {
                    // 어떤 선택지를 선택했는지
                    SelectionEndingScriptElement scriptElement = Instantiate(selectionScriptPrefab, scrollContent).GetComponent<SelectionEndingScriptElement>();
                    scriptElement.SetSelectionScript(SystemManager.GetJsonNodeString(selectionData[key][i], GameConst.COL_SCRIPT_DATA), SystemManager.GetJsonNodeString(selectionData[key][i], KEY_SELECTION_CONTENT));
                    createObject.Add(scriptElement.gameObject);
                }

                // 큰 의미는 없고, 한 챕터가 끝났을 때에 여백을 주고 있음
                GameObject emptyObject = Instantiate(emptyPrefab, scrollContent);
                createObject.Add(emptyObject);
            }

            // for문이 전부 돌고, 제일 마지막에 엔딩 무엇에 도달했는지 표기
            SelectionEndingTitleElement endingTitleElement = Instantiate(endingTitlePrefab, scrollContent).GetComponent<SelectionEndingTitleElement>();
            endingTitleElement.SetEndingTitle(string.Format("{0}. {1}", endingType.text, endingData.episodeTitle));
            createObject.Add(endingTitleElement.gameObject);

            endingType.gameObject.SetActive(true);
            endingTitle.gameObject.SetActive(true);
        }

        public override void OnHideView()
        {
            base.OnHideView();

            // element들을 모두 Instantiate 하여 생성해줬으므로 모두 파괴한다
            foreach (GameObject g in createObject)
                Destroy(g);

            createObject.Clear();
        }

        public void OnClickStartEnding()
        {
            UserManager.main.useRecord = false;         // 엔딩 플레이는 useRecord를 false 처리한다. 
            //IntermissionManager.isMovingLobby = false;  // 게임으로 진입하도록 요청

            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_GAME_BEGIN, string.Empty);

            //if (GameManager.main != null)
            //    SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
            //else
            //    SceneManager.LoadSceneAsync(CommonConst.SCENE_GAME, LoadSceneMode.Single).allowSceneActivation = true;

            GameManager.SetNewGame();
            NetworkLoader.main.UpdateUserProjectCurrent(endingData.episodeID, null, 0, false, "ViewEndingDetail.OnClickStartEnding");
        }
    }
}
