using UnityEngine;

using TMPro;

namespace PIERStory {

    public class EpisodeEndControls : StoryLobbyMain
    {
        
        [Space]
        public TextMeshProUGUI textSummary;
        public EndingNotification endingNotification;
        
        

        protected override void Update() {
            base.Update();
        }
        
        public override void InitStoryLobbyControls() {
            
            Debug.Log("## EpisodeEndControls.InitStoryLobbyControls ");
            CallbackReduceWaitingTimeSuccess = RefreshAfterReduceWaitingTime; 
            CallbackReduceWaitingTimeFail = FailReduceWaitingTime;
            OnEpisodePlay = OnClickPlay;
                        
            this.InitBaseInfo(); // 기본정보
            
            // 일반 설정 시작 
            SetPlayState(); // 플레이 및 타이머 설정 

            if ((UserManager.main.tutorialStep == 2 && UserManager.main.tutorialClear) || (UserManager.main.tutorialStep == 3 && !UserManager.main.tutorialClear))
                UserManager.main.UpdateTutorialStep(3, 0, CallbackStartTutorial);


            // 엔딩에 도달한 경우 추가 로직 (엔딩을 플레이 하지는 않았음)
            if (currentEpisodeData.episodeType == EpisodeType.Ending && !UserManager.main.CheckReachFinal()) {
                SetEndingNotification();
                return;
            }
        }
        
        
        
        /// <summary>
        /// 거의 똑같은데 마지막만 다름 
        /// </summary>
        void InitBaseInfo() {
            
            Debug.Log("## EpisodeEndControls.InitBaseInfo");
            
            endingNotification.gameObject.SetActive(false);
            
            textReduceWaitingTime.text = SystemManager.main.waitingReduceTimeAD.ToString() +" min"; // 광고보고 차감되는 시간 SysteManager..
           
            currentStoryData =  StoryManager.main.CurrentProject; // 현재 작품 
            projectCurrentJSON = UserManager.main.GetUserProjectRegularEpisodeCurrent(); // 작품상에서 현재 위치 
            
            
            // 에피소드 타이틀 초기화
            SetEpisodeTitleText(string.Empty);
            
            if(projectCurrentJSON == null) {
                SystemManager.ShowSimpleAlert("Invalid Episode State. Please contact to help center");
                return;
            }
            
            currentEpisodeID = SystemManager.GetJsonNodeString(projectCurrentJSON, "episode_id");
            currentEpisodeData = StoryManager.GetRegularEpisodeByID(currentEpisodeID);
            currentEpisodeData.SetPurchaseState(); // 구매기록 refresh.
            
            
            hasPremium = UserManager.main.HasProjectFreepass();
            isEpisodeContinuePlay = false;
            
            textSummary.text = currentEpisodeData.episodeSummary; // 요약정보 추가 
            
        }
        
        /// <summary>
        /// 엔딩 도달한 경우에 대한 처리 
        /// </summary>
        void SetEndingNotification() {
            endingNotification.SetEndingNotification(currentEpisodeData, OnClickPlay);
            
            // 일반 컨트롤은 감춘다.            
            imageEpisodeTitle.gameObject.SetActive(false); 
            storyPlayButton.gameObject.SetActive(false);
            
        }


        void CallbackStartTutorial(BestHTTP.HTTPRequest req, BestHTTP.HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackStartTutorial, Tutorial Mission3");
                return;
            }

            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_TUTORIAL_MISSION_3);
            p.Data.contentValue = GetEpisodeTimeOpenPrice();
            PopupManager.main.ShowPopup(p, true);
        }
    }
}