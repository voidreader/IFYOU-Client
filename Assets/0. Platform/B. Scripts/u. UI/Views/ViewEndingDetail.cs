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

            endingBanner.SetDownloadURL(endingData.popupImageURL, endingData.popupImageKey);

            if (endingData.endingType == LobbyConst.COL_HIDDEN)
                endingType.text = SystemManager.GetLocalizedText("5087");
            else
                endingType.text = SystemManager.GetLocalizedText("5088");

            endingTitle.text = endingData.episodeTitle;

            int episodeIndex = 0, scriptIndex = 0;

            foreach (string key in selectionData.Keys)
            {
                TextMeshProUGUI[] episodeTexts = episodeElements[episodeIndex].GetComponentsInChildren<TextMeshProUGUI>();
                episodeTexts[0].text = string.Format("{0}", episodeIndex + 1);
                episodeTexts[1].text = key;
                episodeElements[episodeIndex].transform.SetParent(scrollContent);

                for (int i = 0; i < selectionData[key].Count; i++)
                {
                    TextMeshProUGUI[] scriptTexts = scriptElements[scriptIndex].GetComponentsInChildren<TextMeshProUGUI>();
                    scriptTexts[0].text = SystemManager.GetJsonNodeString(selectionData[key][i], GameConst.COL_SCRIPT_DATA);
                    scriptTexts[1].text = SystemManager.GetJsonNodeString(selectionData[key][i], KEY_SELECTION_CONTENT);
                    episodeElements[scriptIndex].transform.SetParent(scrollContent);
                    scriptIndex++;
                }
                emptyElements[episodeIndex].transform.SetParent(scrollContent);
                episodeIndex++;
            }

            TextMeshProUGUI[] endingTexts = episodeEndTitle.GetComponentsInChildren<TextMeshProUGUI>();
            endingTexts[1].text = string.Format("{0}. {1}", endingType.text, endingTitle.text);
            episodeEndTitle.transform.SetParent(scrollContent);
        }

        public override void OnHideView()
        {
            base.OnHideView();

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
