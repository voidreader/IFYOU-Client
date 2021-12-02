using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewEpisodeEnd : CommonView
    {
        public TextMeshProUGUI episodeTitle;

        [Space][Header("Circle progress bar")]
        public GameObject IllustProgress;       // 일러스트 획득률
        public GameObject verticalLine;         // 일러스트, 사건 사이에 들어간 세로선

        [Header("Illust progress bar")]
        public Image prevIllustGauge;
        public Image currIllustGauge;
        public TextMeshProUGUI currIllustValue;
        [Header("Scene progress bar")]
        public Image prevSceneGauge;
        public Image currSceneGauge;
        public TextMeshProUGUI currSceneValue;

        [Space][Header("Buttons")]
        public GameObject nextEpisodeButton;
        public GameObject retryButton;
        public GameObject returnLobbyButton;

        EpisodeData episodeData = null;

        SignalReceiver signalReceiverEpisodeEnd;
        SignalStream signalStreamEpisodeEnd;



        #region Signal 수신 처리

        private void Awake()
        {
            signalStreamEpisodeEnd = SignalStream.Get(LobbyConst.STREAM_GAME, "episodeEnd");
            signalReceiverEpisodeEnd = new SignalReceiver().SetOnSignalCallback(OnSignal);
        }

        private void OnEnable()
        {
            signalStreamEpisodeEnd.ConnectReceiver(signalReceiverEpisodeEnd);
        }

        private void OnDisable()
        {
            signalStreamEpisodeEnd.DisconnectReceiver(signalReceiverEpisodeEnd);
        }

        void OnSignal(Signal signal)
        {
            if(signal.hasValue)
            {
                episodeData = signal.GetValueUnsafe<EpisodeData>();
                SetCurrentEpisodeInfo();
            }
        }

        #endregion


        public override void OnStartView()
        {
            base.OnStartView();
        }

        void SetCurrentEpisodeInfo()
        {
            // 에피소드 넘버링 및 제목 설정
            switch (episodeData.episodeType)
            {
                case EpisodeType.Chapter:
                    episodeTitle.text = string.Format("{0} {1}. ",SystemManager.GetLocalizedText("5027"), episodeData.episodeNO);
                    break;
                case EpisodeType.Ending:

                    if (episodeData.endingType.Equals(LobbyConst.COL_HIDDEN))
                        episodeTitle.text = string.Format("{0}. ", SystemManager.GetLocalizedText("5087"));
                    else
                        episodeTitle.text = string.Format("{0}. ", SystemManager.GetLocalizedText("5088"));

                    break;
                case EpisodeType.Side:
                    episodeTitle.text = string.Format("{0}. ", SystemManager.GetLocalizedText("5028"));
                    break;
            }

            episodeTitle.text += episodeData.episodeTitle;

            // 에피소드에 획득 일러스트가 없는 경우 비활성화 해준다
            if(episodeData.episodeGalleryImageProgressValue < 0)
            {
                IllustProgress.SetActive(false);
                verticalLine.SetActive(false);
            }

            prevIllustGauge.fillAmount = episodeData.episodeGalleryImageProgressValue;
            prevSceneGauge.fillAmount = episodeData.sceneProgressorValue;


        }
    }
}
