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

        [Header("Selection scroll snap")]
        public Transform scrollContent;
        public GameObject selectionEpisodePrefab;
        

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

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);


            // 에피소드가 완료되었으니 모든 사운드를 멈춘다
            foreach (GameSoundCtrl sc in GameManager.main.SoundGroup)
            {
                if (sc.GetIsPlaying)
                    sc.PauseAudioClip();
            }


            // 사용자가 현재 화에서 선택한 선택지 셋팅
            JsonData selectionData = SystemManager.GetJsonNode(UserManager.main.currentStorySelectionHistoryJson, GameConst.TEMPLATE_SELECTION);

            // 선택지는 최신순으로 들어오지 않는다
            int reverse = 0;

            // 플레이가 2회차 이하인 경우
            if(selectionData.Count < 3)
            {
                reverse = 1;

                // 첫 플레이인 경우
                if (selectionData.Count < 2)
                    reverse = 2;
            }

            foreach(string roundKey in selectionData.Keys)
            {
                // 현재 회차 도달할 때까지 건너뛰기
                if (reverse != 2)
                {
                    reverse++;
                    continue;
                }

                foreach(string titleKey in selectionData[roundKey].Keys)
                {
                    // 현재 플레이하고 있는 에피소드와 제목이 같은 행을 찾는다
                    if(episodeData.episodeTitle.Equals(titleKey))
                    {
                        foreach(string prevScriptKey in selectionData[roundKey][titleKey].Keys)
                        {
                            // 선택지 직전 대사 대사
                            JsonData selectionGroup = selectionData[roundKey][titleKey][prevScriptKey];

                            for(int i=0;i<selectionGroup.Count;i++)
                            {
                                // 선택한 선택지만 보여준다
                                if(SystemManager.GetJsonNodeBool(selectionGroup[i], "selected"))
                                {
                                    SelectionEpisodeElement selectionEpisode = Instantiate(selectionEpisodePrefab, scrollContent).GetComponent<SelectionEpisodeElement>();
                                    selectionEpisode.SetCurrentEpisodeSelection(prevScriptKey, SystemManager.GetJsonNodeString(selectionGroup[i], "selection_content"));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 현재 에피소드 정보 기입
        /// </summary>
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
