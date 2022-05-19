using System;
using UnityEngine;
using TMPro;
using LitJson;

namespace PIERStory {

    /// <summary>
    /// 게임 종료 화면에서 사용하는 클래스
    /// StoryLobbyMain 상속받아서 override.
    /// </summary>

    public class EpisodeEndControls : StoryLobbyMain
    {
        public static Action OnRefreshUpdateTimeDeal = null;
        
        [Space]
        public TextMeshProUGUI textSummary;
        public EndingNotification endingNotification;
        
        JsonData timedeal = null;
        
        [Space]
        public PassButton passButton; // 패스 버튼
        public ImageRequireDownload passBadge; // 패스 뱃지 
        
        private void Start() {
            OnRefreshUpdateTimeDeal = SetPremiumPassObject; // Action 설정 
            OnPassPurchase = PostPurchasePremiumPass; 
        }

        protected override void Update() {
            base.Update();
        }
        
        public override void InitStoryLobbyControls() {
            
            Debug.Log("## EpisodeEndControls.InitStoryLobbyControls ");
            CallbackReduceWaitingTimeSuccess = RefreshAfterReduceWaitingTime; 
            CallbackReduceWaitingTimeFail = FailReduceWaitingTime;
            OnEpisodePlay = OnClickPlay;
            
            this.InitBaseInfo(); // 기본정보
            
            NotifyPassTimeDeal(); // 타임딜 체크 및 생성 
            
            // 일반 설정 시작 
            SetPlayState(); // 플레이 및 타이머 설정 
           

            // 엔딩에 도달한 경우 추가 로직 (엔딩을 플레이 하지는 않았음)
            if (currentEpisodeData.episodeType == EpisodeType.Ending && !UserManager.main.CheckReachFinal()) {
                SetEndingNotification();

                // 다음으로 이어질 화가 히든엔딩이고, 이번에 해금되는 것이라면? 업적 통신!
                if (currentEpisodeData.endingType == LobbyConst.COL_HIDDEN && !currentEpisodeData.endingOpen)
                    NetworkLoader.main.RequestIFYOUAchievement(9);

                // 다음화의 EpisodeData의 엔딩 해금을 true로 만들어준다
                for(int i=0;i<StoryManager.main.ListCurrentProjectEpisodes.Count;i++)
                {
                    if (StoryManager.main.ListCurrentProjectEpisodes[i].episodeType != EpisodeType.Ending)
                        continue;

                    if (StoryManager.main.ListCurrentProjectEpisodes[i].episodeID == currentEpisodeData.episodeID)
                    {
                        StoryManager.main.ListCurrentProjectEpisodes[i].endingOpen = true;

                        // 엔딩 해금을 true로 변경해준 뒤에 allClear 체크를 해서 통신한다
                        if (UserManager.main.ProjectAllClear())
                            NetworkLoader.main.RequestIFYOUAchievement(8, int.Parse(StoryManager.main.CurrentProjectID));

                        break;
                    }
                }



                return;
            }
            
            // * 튜토리얼 3번 호출 
            // 다음 에피소드가 무조건 대기 상태여야한다. 
            if(UserManager.main.CheckReachFinal())
                return;
                
            // 이번에 볼 작품이 대기 중이니..? 
            // 연재작은 매우 뒤에 오픈될 수도 있어서. 
            if(!isOpenTimeCountable || currentEpisodeData.isSerial) {
                return;
            }
            
            // 2분 후 오픈이면 하지 띄우지 않음. 
            if(timeDiff.Minutes < 2) 
                return;
            
            
            // 튜토리얼 3 호출
            /*
            if ((UserManager.main.tutorialStep == 2 && UserManager.main.tutorialClear) || (UserManager.main.tutorialStep == 3 && !UserManager.main.tutorialClear))
                UserManager.main.UpdateTutorialStep(3, 0, CallbackStartTutorial);            
            */
            
            
            
        }
        
        
        
        /// <summary>
        /// 새로운 프리미엄패스 타임딜 알림 
        /// </summary>
        void NotifyPassTimeDeal() {
            
            // 프리패스 보유중이면 필요없다.
            if(UserManager.main.HasProjectFreepass(StoryManager.main.CurrentProjectID)) {
                return;
            }
            
            timedeal = SystemManager.main.GetNewTimeDeal(currentEpisodeData);
            
            if(timedeal == null)
                return;
                
            // 타임딜 정보 있으면 유저 타임딜로 입력 처리. 
            int timedealID = SystemManager.GetJsonNodeInt(timedeal, "timedeal_id");
            int deadline = SystemManager.GetJsonNodeInt(timedeal, "deadline");
            int discount = SystemManager.GetJsonNodeInt(timedeal, "discount");
            
            
            JsonData sendingData = new JsonData();
            sendingData["func"] = "updatePassTimeDeal";
            sendingData["timedeal_id"] = timedealID;
            sendingData["deadline"] = deadline;
            sendingData["discount"] = discount;
            sendingData["project_id"] = StoryManager.main.CurrentProjectID;
            
            NetworkLoader.main.SendPost(UserManager.main.CallbackUpdateTimeDeal, sendingData, true);

        }
        
        
        
        
        /// <summary>
        /// 거의 똑같은데 마지막만 다름 
        /// </summary>
        void InitBaseInfo() {
            
            Debug.Log("## EpisodeEndControls.InitBaseInfo");
            

            // 엔딩 알림창 
            endingNotification.gameObject.SetActive(false);
            
            // 오픈시간 관련 처리 
            textReduceWaitingTime.text = SystemManager.main.waitingReduceTimeAD.ToString() +" min"; // 광고보고 차감되는 시간 SysteManager..
           
            currentStoryData =  StoryManager.main.CurrentProject; // 현재 작품 
            projectCurrentJSON = UserManager.main.GetUserProjectRegularEpisodeCurrent(); // 작품상에서 현재 위치 
            
            storyPlayButton.gameObject.SetActive(true);
            
            // 에피소드 타이틀 초기화
            SetEpisodeTitleText(string.Empty);
            
            // 프리미엄 관련 처리 
            SetPremiumPassObject();
            
            if(projectCurrentJSON == null) {
                SystemManager.ShowSimpleAlert("Invalid Episode State. Please contact to help center");
                return;
            }
            
            currentEpisodeID = SystemManager.GetJsonNodeString(projectCurrentJSON, "episode_id");
            currentEpisodeData = StoryManager.GetRegularEpisodeByID(currentEpisodeID); // 다음번 플레이될 차례의 에피소드 데이터 
            currentEpisodeData.SetPurchaseState(); // 구매기록 refresh.
            
            
            
            isEpisodeContinuePlay = false;
            textSummary.text = currentEpisodeData.episodeSummary; // 요약정보 추가 
            

            
        }
        
        /// <summary>
        /// 프리미엄 패스 오브젝트 설정 
        /// </summary>
        void SetPremiumPassObject() {
            
            if(!this.gameObject.activeSelf)
                return;
                        
            // 프리미엄 패스 오브젝트 추가 
            passButton.gameObject.SetActive(false);
            passBadge.gameObject.SetActive(false);
            
            hasPremium = UserManager.main.HasProjectFreepass(); // 프리미엄 패스 보유 여부 
            
            // 프리미엄 패스 보유 여부에 따른 오브젝트 설정 
            if(hasPremium) {
                passBadge.gameObject.SetActive(true);
                passBadge.SetDownloadURL(StoryManager.main.freepassBadgeURL, StoryManager.main.freepassBadgeKey, true);
            }
            else {
                passButton.gameObject.SetActive(true);
                passButton.SetPremiumPass();
            }
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
        
        /// <summary>
        /// 에피소드 종료 화면에서 팝업패스 클리규 
        /// </summary>
        public void OnClickPassButton() {
            SystemManager.ShowPopupPass(StoryManager.main.CurrentProjectID, false);
            
            // 패스 구매 콜백. 
            UserManager.OnFreepassPurchase = this.InitStoryLobbyControls;
        }
        
        
        /// <summary>
        /// 프리미엄 패스 구매 후 호출 
        /// </summary>
        void PostPurchasePremiumPass() {
            Debug.Log(">>> PostPurchasePremiumPass <<<");
            
            if(!this.gameObject.activeSelf)
                return;
                
            this.InitStoryLobbyControls();
        }        
        
    }
}