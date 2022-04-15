using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewEpisodeEnd : CommonView
    {
        public StoryData currentStoryData; 
        
        public ImageRequireDownload episodeImage;
        public EpisodeEndControls episodeEndControls;


        void Update() {
            
            // 로비로 나가는거 확인창 띄우기 백스페이스 
            if(Input.GetKeyDown(KeyCode.Escape)) {
                CommonView.DeleteDumpViews(); 
                
                if(SystemManager.main.isWebViewOpened) {
                    SystemManager.main.HideWebviewForce();
                    return;
                }
                
                
                if(PopupManager.main.GetFrontActivePopup() != null) {
                    return;
                }
                
                if(CommonView.ListActiveViews.Count == 1 && CommonView.ListActiveViews.Contains(this)  // 1개 활성화, 본인
                    || CommonView.ListActiveViews.Count == 2 && CommonView.ListActiveViews.Contains(this)) {
                    
                    SystemManager.ShowSystemPopupLocalize("6302", OnClickReturnLobby, null, true);
                }
                
            }
        }

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
            
            // 리뷰 팝업 위치
            // RateGame.Instance.IncreaseCustomEvents();
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
