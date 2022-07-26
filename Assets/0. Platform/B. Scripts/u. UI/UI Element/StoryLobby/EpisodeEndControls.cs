using System.Collections;
using UnityEngine;

using TMPro;
using DG.Tweening;

namespace PIERStory {

    /// <summary>
    /// 게임 종료 화면에서 사용하는 클래스
    /// StoryLobbyMain 상속받아서 override.
    /// </summary>

    public class EpisodeEndControls : StoryLobbyMain
    {
        
        [Space]
        public TextMeshProUGUI textSummary;
        public EndingNotification endingNotification;
        public CanvasGroup canvasGroup;
        
        [Space]
        public PassButton passButton; // 패스 버튼
        public ImageRequireDownload passBadge; // 패스 뱃지 
        public AllPassTimer allpassTimer; // 올패스 타이머 
        
        private void Start() {
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
            
            // 일반 설정 시작 
            SetPlayState(); // 플레이 및 타이머 설정 

            // 플레이 카운트
            int rateCount = PlayerPrefs.HasKey(SystemConst.RATE_PLAY_COUNT) ? PlayerPrefs.GetInt(SystemConst.RATE_PLAY_COUNT) : 0;
            PlayerPrefs.SetInt(SystemConst.RATE_PLAY_COUNT, rateCount + 1);

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
            
            StartCoroutine(RoutinePostEpisodeEnd());
            SystemManager.HideNetworkLoading();
            canvasGroup.DOFade(1f, 0.5f);
        }
        
        IEnumerator RoutinePostEpisodeEnd() {
            yield return new WaitForSeconds(0.2f);
            
            // 다음 에피소드가 없으면 더이상 아래 로직을 실행하지 않음 
            if(UserManager.main.CheckReachFinal())
                yield break;
            
            // 활성화된 창이 있으면 대기한다. 
            while(PopupManager.main.GetFrontActivePopup() != null)
                yield return new WaitForSeconds(0.1f);
            
            // 통신 완료되길 기다린다. 
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            yield return new WaitForSeconds(0.1f);
                
            // 다음 오픈되는 에피소드가 연재작이라 대기해야되는 경우. 
            if(isOpenTimeCountable && currentEpisodeData.isSerial) {
                NetworkLoader.main.RequestRecommedStory();
                yield break;
            }
                
                
            // 대기 중이거나, 연재 작품은 튜토리얼 띄우지 않음. 
            if(!isOpenTimeCountable || currentEpisodeData.isSerial) {
                yield break;
            }
            
            // 2분 후 오픈이면 하지 띄우지 않음. 
            if(timeDiff.Minutes < 2) 
                yield break;
            
            // * 튜토리얼 3번 호출 
            if ((UserManager.main.tutorialStep <= 2 && UserManager.main.tutorialClear) || (UserManager.main.tutorialStep == 3 && !UserManager.main.tutorialClear))
                UserManager.main.UpdateTutorialStep(3, 0, CallbackStartTutorial);                 
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
            
            currentEpisodeID = SystemManager.GetJsonNodeString(projectCurrentJSON, LobbyConst.STORY_EPISODE_ID);
            currentEpisodeData = StoryManager.GetRegularEpisodeByID(currentEpisodeID); // 다음번 플레이될 차례의 에피소드 데이터 
            currentEpisodeData.SetPurchaseState(); // 구매기록 refresh.
            

            isEpisodeContinuePlay = false;
            
            SystemManager.SetText(textSummary, currentEpisodeData.episodeSummary); // 요약정보 추가 
        }
        
        /// <summary>
        /// 프리미엄 패스 오브젝트 설정 
        /// </summary>
        void SetPremiumPassObject() {
             
            // 프리미엄 패스 및 올 패스 오브젝트 추가 
            passButton.gameObject.SetActive(false);
            passBadge.gameObject.SetActive(false);
            allpassTimer.gameObject.SetActive(false);
            
            hasPass = UserManager.main.HasProjectFreepass(); // 패스 보유 여부 
            
            // 패스 보유 여부에 따른 오브젝트 설정 
            if(hasPass) {
                
                if(UserManager.main.HasProjectPremiumPassOnly(StoryManager.main.CurrentProjectID)) {
                    passBadge.gameObject.SetActive(true);
                    passBadge.SetDownloadURL(StoryManager.main.freepassBadgeURL, StoryManager.main.freepassBadgeKey, true);    
                }
                else {
                    // 올패스 처리 
                    allpassTimer.InitAllPassTimer();
                }
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
        /// 에피소드 종료 화면에서 팝업패스 클릭
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