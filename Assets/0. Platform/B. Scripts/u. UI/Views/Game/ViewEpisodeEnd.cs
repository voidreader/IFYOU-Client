using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewEpisodeEnd : CommonView
    {
        public ImageRequireDownload episodeImage;
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

        EpisodeData updateData = null;
        EpisodeData nextData = null;
        EpisodeData episodeData = null;

        
        SignalReceiver signalReceiverNextData;
        SignalReceiver signalReceiverEpisodeEnd;

        
        SignalStream signalStreamNextData;
        SignalStream signalStreamEpisodeEnd;


        #region Signal 수신 처리

        private void Awake()
        {

            // * 다음 플레이 에피소드 데이터 수신 
            signalStreamNextData = SignalStream.Get(LobbyConst.STREAM_GAME, GameConst.SIGNAL_NEXT_DATA);
            signalReceiverNextData = new SignalReceiver().SetOnSignalCallback(OnNextSignal);
            
            // * 방금 플레이했던 에피소드의 데이터 수신 
            signalStreamEpisodeEnd = SignalStream.Get(LobbyConst.STREAM_GAME, GameConst.SIGNAL_EPISODE_END);
            signalReceiverEpisodeEnd = new SignalReceiver().SetOnSignalCallback(OnSignal);
        }

        private void OnEnable()
        {
            signalStreamNextData.ConnectReceiver(signalReceiverNextData);
            signalStreamEpisodeEnd.ConnectReceiver(signalReceiverEpisodeEnd);
        }

        private void OnDisable()
        {
            signalStreamNextData.DisconnectReceiver(signalReceiverNextData);
            signalStreamEpisodeEnd.DisconnectReceiver(signalReceiverEpisodeEnd);
        }



        void OnNextSignal(Signal s)
        {
            // 버튼 세팅(다시하기(엔딩, 사이드), 다음 에피소드 결정)
            if (s.hasValue)
            {
                nextData = s.GetValueUnsafe<EpisodeData>();

                // 다음화가 존재하는 경우
                if(nextData != null)
                {
                    nextEpisodeButton.SetActive(true);
                    retryButton.SetActive(false);
                }
                else
                {
                    nextEpisodeButton.SetActive(false);
                    retryButton.SetActive(true);
                }
            }
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
            episodeImage.InitImage();
            episodeImage.SetDownloadURL(episodeData.popupImageURL, episodeData.popupImageKey);

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
            
            // 갤러리 이미지 값 리프레시 시키고, curr에 할당한다. 
            episodeData.RefreshGalleryProgressValue();
            currIllustGauge.fillAmount = episodeData.episodeGalleryImageProgressValue;
            
            // 게임매니저에서 updatedEpisodeSceneProgressValue 값 받아와서 재계산. 
            currSceneGauge.fillAmount = GameManager.main.updatedEpisodeSceneProgressValue / episodeData.totalSceneCount; 
            // played scene count 표기하고 갱신
            episodeData.SetNewPlayedSceneCount(GameManager.main.updatedEpisodeSceneProgressValue);

        }

        #region OnClick event

        public void OnClickNextEpisode()
        {
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, nextData, string.Empty);
            Signal.Send(LobbyConst.STREAM_GAME, GameConst.SIGNAL_NEXT_EPISODE, string.Empty);
        }

        public void OnClickRetryEpisode()
        {
            GameManager.main.RetryPlay();
        }

        public void OnClickReturnLobby()
        {
            GameManager.main.EndGame();
        }

        #endregion
    }
}
