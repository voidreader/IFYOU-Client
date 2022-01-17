using UnityEngine;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using DanielLochner.Assets.SimpleScrollSnap;

namespace PIERStory
{
    public class ViewEpisodeEnd : CommonView
    {
        public ImageRequireDownload episodeImage;
        public TextMeshProUGUI episodeTitle;

        [Space][Header("Circle progress bar")]
        [SerializeField] float illustProgress = 0;
        [SerializeField] float sceneProgress = 0;
        [SerializeField] EpisodeContentProgress illustProgressBar;
        [SerializeField] EpisodeContentProgress sceneProgressBar;
        [SerializeField] GameObject contentsMiddleVerticalLine; // 일러스트, 경험한 사건 사이에 선 


        [Header("Selection scroll snap")]
        public SimpleScrollSnap selectionSnap;
        public Transform scrollContent;
        public GameObject selectionEpisodePrefab;
        public Transform pagenation;
        public GameObject pageTogglePrefab;
        

        [Space][Header("Buttons")]
        public GameObject nextEpisodeButton;
        public GameObject retryButton;
        public GameObject returnLobbyButton;

        EpisodeData nextData = null;
        EpisodeData episodeData = null;


        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            
            episodeData = SystemListener.main.episodeEndCurrentData;
            nextData = SystemListener.main.episodeEndNextData;
            
            // 방금 플레이한 에피소드 정보 설정 
            SetCurrentEpisodeInfo();
            
            // 다음 에피소드 정보 설정  
            if(nextData != null && nextData.isValidData) { // 다음 에피소드 있음
                nextEpisodeButton.SetActive(true);
                retryButton.SetActive(false);
            }
            else {
                nextEpisodeButton.SetActive(false);
                retryButton.SetActive(true);
            }



            // 에피소드가 완료되었으니 모든 사운드를 멈춘다
            foreach (GameSoundCtrl sc in GameManager.main.SoundGroup)
            {
                if (sc.GetIsPlaying)
                    sc.StopAudioClip();
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
                                    Instantiate(pageTogglePrefab, pagenation);
                                    break;
                                }
                            }
                        }
                    }
                }

                // Instantiate로 만드므로 Panel 설정이 필요함
                selectionSnap.Setup();
            }
        }

        public override void OnHideView()
        {
            base.OnHideView();

            for (int i = 0; i < scrollContent.childCount; i++)
            {
                Destroy(scrollContent.GetChild(i).gameObject);
                Destroy(pagenation.GetChild(i).gameObject);
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

            // 진행도 처리 
            illustProgress = episodeData.episodeGalleryImageProgressValue;
            sceneProgress = episodeData.sceneProgressorValue;

            // 갤러리 프로그레스. -1이면 없다. 
            if(illustProgress > -1) {
                contentsMiddleVerticalLine.SetActive(true);
                illustProgressBar.gameObject.SetActive(true);
                illustProgressBar.SetProgress(illustProgress);
                
                // 갤러리 Progress 리프레시 
                episodeData.RefreshGalleryProgressValue();
                illustProgressBar.SetAdditionalProgress(episodeData.episodeGalleryImageProgressValue);
                
            }
            else {
                illustProgressBar.gameObject.SetActive(false);
                contentsMiddleVerticalLine.SetActive(false);
            }
            
            sceneProgressBar.SetProgress(sceneProgress);
            // * refresh된 값을 추가로 할당한다. 
            episodeData.SetNewPlayedSceneCount(GameManager.main.updatedEpisodeSceneProgressValue);
            sceneProgressBar.SetAdditionalProgress(episodeData.sceneProgressorValue);

        }

        #region OnClick event

        public void OnClickNextEpisode()
        {
            
            Debug.Log(">> OnClickNextEpisode << ");
            
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
