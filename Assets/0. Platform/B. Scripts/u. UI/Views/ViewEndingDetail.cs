using UnityEngine;

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

        public Transform scrollContent;
        public Transform storage;

        public GameObject[] episodeElements;
        public GameObject[] scriptElements;
        public GameObject[] emptyElements;
        public GameObject episodeEndTitle;

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
                endingType.text = SystemManager.GetLocalizedText("5087");
            else
                endingType.text = SystemManager.GetLocalizedText("5088");

            endingTitle.text = endingData.episodeTitle;

            int episodeIndex = 0, scriptIndex = 0;

            // 해당 파트는 GetComponentsInChildren<TextMeshProUGUI>()가 
            // 선택지 세팅
            foreach (string key in selectionData.Keys)
            {
                // 엔딩까지 도달하기 위한 0 = 몇화인지, 1 = 제목이 무언지
                TextMeshProUGUI[] episodeTexts = episodeElements[episodeIndex].GetComponentsInChildren<TextMeshProUGUI>();
                episodeTexts[0].text = string.Format("{0}", episodeIndex + 1);
                episodeTexts[1].text = key;
                episodeElements[episodeIndex].transform.SetParent(scrollContent);

                for (int i = 0; i < selectionData[key].Count; i++)
                {
                    // 0 = 도달하기 전 대사, 1 = 선택했던 선택지 대사
                    TextMeshProUGUI[] scriptTexts = scriptElements[scriptIndex].GetComponentsInChildren<TextMeshProUGUI>();
                    scriptTexts[0].text = SystemManager.GetJsonNodeString(selectionData[key][i], GameConst.COL_SCRIPT_DATA);
                    scriptTexts[1].text = SystemManager.GetJsonNodeString(selectionData[key][i], KEY_SELECTION_CONTENT);
                    episodeElements[scriptIndex].transform.SetParent(scrollContent);
                    scriptIndex++;
                }
                emptyElements[episodeIndex].transform.SetParent(scrollContent);
                episodeIndex++;
            }

            // for문이 전무 돌고, 제일 마지막에 엔딩 무엇에 도달했는지 표기
            TextMeshProUGUI[] endingTexts = episodeEndTitle.GetComponentsInChildren<TextMeshProUGUI>();
            endingTexts[1].text = string.Format("{0}. {1}", endingType.text, endingTitle.text);
            episodeEndTitle.transform.SetParent(scrollContent);
        }

        public override void OnHideView()
        {
            base.OnHideView();

            // storage로 element들을 다 옮겨준다
            episodeEndTitle.transform.SetParent(storage);

            foreach (GameObject g in episodeElements)
                g.transform.SetParent(storage);

            foreach (GameObject g in scriptElements)
                g.transform.SetParent(storage);

            foreach (GameObject g in emptyElements)
                g.transform.SetParent(storage);
        }
    }
}
