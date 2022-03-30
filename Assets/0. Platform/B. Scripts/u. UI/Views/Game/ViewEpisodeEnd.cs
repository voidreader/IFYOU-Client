using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewEpisodeEnd : CommonView
    {
        public StoryData currentStoryData; 
        
        public ImageRequireDownload episodeImage;
        public EpisodeEndControls episodeEndControls;


        public override void OnStartView()
        {
            base.OnStartView();
            

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
            
            
            // 현재 작품 정보에서 종료 이미지 가져오기 
            currentStoryData = StoryManager.main.CurrentProject;
            
            RateGame.Instance.IncreaseCustomEvents();
            episodeImage.SetDownloadURL(currentStoryData.episodeFinishImageURL, currentStoryData.episodeFinishImageKey); // 종료 이미지 처리 
            
            
            // 관련 컨트롤 초기화
            episodeEndControls.InitStoryLobbyControls();
            
            Debug.Log("## ViewEpisodeEnd BakcAction Setting");
            ViewCommonTop.OnBackAction = OnClickReturnLobby;
        }


        public override void OnHideView()
        {
            base.OnHideView();
            ViewCommonTop.OnBackAction = null; // 액션 초기화 
            
        }
        
        
        void OnDestroy() {
            ViewCommonTop.OnBackAction = null; // 액션 초기화 
        }


        #region OnClick event

        public void OnClickRetryEpisode()
        {
            GameManager.main.RetryPlay();
        }

        
        /// <summary>
        /// 로비로 돌아가기 
        /// </summary>
        public void OnClickReturnLobby()
        {
            Debug.Log("OnClickReturnLobby !!!!!!");
            
            try {
            
                // ViewEpisodeEnd 활성화 되어있지 않으면 동작하지 않음 .
                if(!this.gameObject.activeSelf)
                    return;
                
                UserManager.main.gameComplete = true;
                GameManager.main.EndGame();
            }
            catch(System.Exception e) {
                ViewCommonTop.OnBackAction = null; // 액션 초기화 
            }
        }

        #endregion
    }
}
