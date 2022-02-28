using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LitJson;
using Doozy.Runtime.Signals;
using UnityEngine.SceneManagement;


namespace PIERStory {

    public class StoryLobbyMain : MonoBehaviour
    {
        
        public StoryData currentStoryData;
        JsonData projectCurrentJSON = null;
        public string currentEpisodeID = string.Empty; // 현재 순번의 에피소드 ID 
        public EpisodeData currentEpisodeData; // 현재 순번의 에피소드 데이터 
        public bool hasPremium = false; // 프리미엄 패스 보유 여부 
        public StatePlayButton currentPlayState = StatePlayButton.inactive; // 현재 에피소드 플레이 상태 
        
        public List<FlowElement> ListFlowElements; // Flow 맵 개체들 
        public List<StoryLobbyContentsButton> ListContentsButton; // 컨텐츠 버튼 
        
        
        [Space]
        [Header("Controls")]
        // 스토리 플레이 버튼
        public StoryPlayButton storyPlayButton; // 중앙 플레이 버튼
        
        
        
        public Image imageEpisodeTitle; // 에피소드 타이틀 배경 
        public TextMeshProUGUI textEpisodeTitle; // 에피소드 타이틀 
        public GameObject groupOpenTimer; // 오픈 타이머
        public TextMeshProUGUI textOpenTimer; // 오픈 타이머 
        
        public GameObject mailNotify; // 상단 메일 알림 
        
        public RectTransform rectContentsGroup; // 컨텐츠 그룹 
        public CanvasGroup canvasGroupContents; // 컨텐츠 그룹 canvas group
        public RectTransform arrowGroupContetns; // 컨텐츠 그룹 화살표 
        public GameObject groupStoryContentsBlock; // 접혀있는 상태에서 터치 못하게 하려고..
        
        
        
        [SerializeField] RectTransform flowMap; // flow map 
        bool inTransitionFlow = false;
        
        
        [Space]
        [Header("Sprites")]
        public Sprite spriteActiveEpisodeTitleBG;
        public Sprite spriteInactiveEpisodeTitleBG;
        
        
        public const long addTick = 621355968000000000; // C#과 javascript 타임 Tick 차이 
        public DateTime openDate;
        public long openDateTick;
        public TimeSpan timeDiff; // 오픈시간까지 남은 차이 
        [SerializeField] bool isOpenTimeCountable = false; // 타이머 카운팅이 가능한지 
        
        
        
        // 그룹 컨텐츠 변수들 
        public Vector2 posGroupContentsOrigin; // 그룹 컨텐츠 기본 위치 
        public Vector2 posGroupContentsOpen; // 그룹 컨텐츠 열림 위치 
        bool inTransitionGroupContents; // 그룹 컨텐츠 트랜지션 여부 
        
        bool isGameStarting = false; // 게임 시작했는지 체크, 중복 입력 막기 위해서.
        [SerializeField] bool isEpisodeContinuePlay = false; // 에피소드 이어하기 상태? 
        
        void Update() {
            if(!isOpenTimeCountable) {
                return;
            }
            
            // 타이머 처리 
            if(Time.frameCount % 5 == 0) {
                textOpenTimer.text = GetOpenRemainTime();
            }
        }
        
        /// <summary>
        /// 스토리 로비 
        /// </summary>
        public void InitStoryLobbyControls() {
            
            
            Debug.Log("## InitStoryLobbyControls");
            
            currentStoryData =  StoryManager.main.CurrentProject; // 현재 작품 
            projectCurrentJSON = UserManager.main.GetUserProjectRegularEpisodeCurrent(); // 작품상에서 현재 위치 
            
            isEpisodeContinuePlay = false;
            
            // 에피소드 타이틀 초기화
            SetEpisodeTitleText(string.Empty);
            
            if(projectCurrentJSON == null) {
                SystemManager.ShowSimpleAlert("Invalid Episode State. Please contact to help center");
                return;
            }
            
            currentEpisodeID = SystemManager.GetJsonNodeString(projectCurrentJSON, "episode_id");
            currentEpisodeData = StoryManager.GetRegularEpisodeByID(currentEpisodeID);
            hasPremium = UserManager.main.HasProjectFreepass();
            
            if(LobbyManager.main != null && CheckResumePossible()) {
                isEpisodeContinuePlay = true;
            }
            

            SetPlayState(); // 플레이 및 타이머 설정 
                        
    
            // 컨텐츠 그룹 초기화 
            InitContentsGroup();
            
            
            // Flow 처리 
            InitFlowMap();

        }
        
        #region 컨텐츠 그룹 제어 (앨범, 스페셜 에피소드, 엔딩, 미션)
        
        /// <summary>
        /// 컨텐츠 그룹 초기화 
        /// </summary>
        void InitContentsGroup() {
            rectContentsGroup.DOKill();
            rectContentsGroup.anchoredPosition = posGroupContentsOrigin; // 기본 위치로 지정 
            canvasGroupContents.alpha = 0.8f; 
            
            for(int i=0; i< ListContentsButton.Count;i++) {
                ListContentsButton[i].InitContentsButton();
            }
            
            groupStoryContentsBlock.SetActive(true);
            
        }
        
        /// <summary>
        /// 컨텐츠 그룹 열고 닫기 
        /// </summary>
        public void OnClickContentsArrow() {
            
            if(inTransitionFlow)
                return;
                
            if(rectContentsGroup.anchoredPosition.x < -300) { // 열림 
                rectContentsGroup.DOAnchorPos(posGroupContentsOpen, 0.2f).OnStart(()=>{inTransitionGroupContents = true;}).OnComplete(()=> {inTransitionGroupContents = false;});
                canvasGroupContents.DOFade(1, 0.2f);
                arrowGroupContetns.localScale = new Vector3(-1, 1, 1);
                
                groupStoryContentsBlock.SetActive(false);
                
            }
            else { // 닫힘 
                rectContentsGroup.DOAnchorPos(posGroupContentsOrigin, 0.2f).OnStart(()=>{inTransitionGroupContents = true;}).OnComplete(()=> {inTransitionGroupContents = false;});
                canvasGroupContents.DOFade(0.8f, 0.2f);
                arrowGroupContetns.localScale = Vector3.one;
                
                groupStoryContentsBlock.SetActive(true);
            }
        }
        
        #endregion
        
        
        #region 플로우 맵 제어 
        
        /// <summary>
        /// FlowMap 초기화 
        /// </summary>
        void InitFlowMap() {
            
            flowMap.anchoredPosition = new Vector2(-820, 0); // flowmap 위치
            bool isMatchEpisode = false; // 아래 for문에서 사용 

            
            // 비활성화 시키고 
            for(int i=0; i<ListFlowElements.Count;i++) {
                ListFlowElements[i].gameObject.SetActive(false);
            }
            
            
            // EpisodeData 할당 처리
            for(int i=0; i<StoryManager.main.ListCurrentProjectEpisodes.Count;i++) {
                
                if(i >= ListFlowElements.Count)
                    break;
                
                ListFlowElements[i].InitFlowElement(StoryManager.main.ListCurrentProjectEpisodes[i]);
            }
            
            // 다시 돌리면서 타이머 설정 추가 
            for(int i=0; i<StoryManager.main.ListCurrentProjectEpisodes.Count;i++) {
                if(i >= ListFlowElements.Count)
                    break;
                    
                if(ListFlowElements[i].currentEpisode.episodeID == currentEpisodeID) {
                    isMatchEpisode = true;   
                    ListFlowElements[i].SetOpenTime(openDateTick); // tick 전달한다. 
                    continue; 
                }
                
                // 연달아 오픈을 위한 추가 처리
                if(isMatchEpisode 
                    && ListFlowElements[i].currentEpisode.episodeType == EpisodeType.Chapter
                    && ListFlowElements[i].currentEpisode.nextOpenMin == 0) {
                        
                        // for문 매칭 이후 next_open_min이 0인 경우에는 동일한 대기시간으로 처리한다.
                        ListFlowElements[i].SetOpenTime(openDateTick);
                        
                }
                
                if(isMatchEpisode && ListFlowElements[i].currentEpisode.episodeType == EpisodeType.Chapter
                    && ListFlowElements[i].currentEpisode.nextOpenMin > 0) {
                    
                    // 연달아 오픈이 종료되었다고 판단하고 break
                    break;        
                }
            } // ? end of initFlow
            
        }
        
        public void OnClickFlowOpen() {
            if(inTransitionFlow)           
                return;
                
            flowMap.DOAnchorPos(new Vector2(-85, 0), 0.5f).OnStart(()=> {inTransitionFlow = true;}).OnComplete(()=>{inTransitionFlow = false;});
        }
        
        public void OnClickFlowClose() {
            if(inTransitionFlow)           
                    return;   
                    
            flowMap.DOAnchorPos(new Vector2(-820, 0), 0.5f).OnStart(()=> {inTransitionFlow = true;}).OnComplete(()=>{inTransitionFlow = false;});
        }  
        
        #endregion
        

        
        /// <summary>
        /// 상태 및 오픈 타이머 설정 
        /// </summary>
        void SetPlayState() {
            // 에피소드 오픈 시간 처리
            openDateTick = ConvertServerTimeTick(long.Parse(projectCurrentJSON["next_open_tick"].ToString()));
            openDate = new DateTime(openDateTick); // 틱으로 오픈 시간 생성 
            
            Debug.Log("## open date : " + openDate.ToString());
            Debug.Log("## utc date : " + DateTime.UtcNow.ToString());
            
            timeDiff = openDate - DateTime.UtcNow; // 시간차 체크 
            Debug.Log("## timeDiff : " + timeDiff.Ticks);
            
            isOpenTimeCountable = false;
            if(timeDiff.Ticks > 0) {
                isOpenTimeCountable = true; // 시간 돌아간다. 
                currentPlayState = StatePlayButton.inactive;
            }
            else {
                // 프리미엄 유저인지 아닌지에 따라서 상태 처리 
                currentPlayState = hasPremium ? StatePlayButton.premium : StatePlayButton.active;
            }
            
            groupOpenTimer.SetActive(isOpenTimeCountable);
            textEpisodeTitle.gameObject.SetActive(!isOpenTimeCountable);
         
                 
            // 스토리 플레이버튼 초기화 
            storyPlayButton.SetPlayButton(currentPlayState, currentEpisodeData.sceneProgressorValue, CheckResumePossible());
            storyPlayButton.SetTimeOpenPrice(GetEpisodeTimeOpenPrice());
            
            InitEpisodeTitleColor();
            
        }
        
        /// <summary>
        /// 에피소드 기다리면 무료 오픈 가격 구하기 
        /// </summary>
        /// <returns></returns>
        int GetEpisodeTimeOpenPrice() {
            if(currentPlayState != StatePlayButton.inactive)
                return 0;
                
            return timeDiff.Minutes / 10 * SystemManager.main.episodeOpenPricePer;    
        }
        
        
        /// <summary>
        /// 에피소드 타이틀 텍스트 설정 
        /// </summary>
        /// <param name="__text"></param>
        void SetEpisodeTitleText(string __text) {
            textEpisodeTitle.text = __text;
        }
        
        /// <summary>
        /// 타이틀 정보 처리 
        /// </summary>
        void InitEpisodeTitleColor() {
            
            // 타이틀 설정 
            SetEpisodeTitleText(currentEpisodeData.storyLobbyTitle);
            
            if(currentPlayState == StatePlayButton.inactive) {
                imageEpisodeTitle.sprite = spriteInactiveEpisodeTitleBG;    
                // textEpisodeTitle.color = Color.white;
            }
            else {
                imageEpisodeTitle.sprite = spriteActiveEpisodeTitleBG;    
                // textEpisodeTitle.color = Color.black;
            }

        }
        
        
        /// <summary>
        /// 오픈시간까지 남은시간 구하기 (UTC 기준, 서버에서도 UTC로 준다. )
        /// </summary>
        /// <returns></returns>        
        string GetOpenRemainTime() {
            timeDiff = openDate - DateTime.UtcNow;
            
            if(timeDiff.Ticks < 0) {
                SetPlayState(); // 오픈시간이된 경우 리프레시
                return string.Empty;
            }
            
            storyPlayButton.SetTimeOpenPrice(GetEpisodeTimeOpenPrice()); // 가격이 10분마다 변한다. 
            
            return string.Format ("{0:D2}:{1:D2}:{2:D2}",timeDiff.Hours ,timeDiff.Minutes, timeDiff.Seconds);
        }
        
        /// <summary>
        /// 이어하기 가능한지 체크 여부 
        /// </summary>
        /// <returns></returns>
        bool CheckResumePossible() {
            if(projectCurrentJSON == null || string.IsNullOrEmpty(projectCurrentJSON["scene_id"].ToString()) || projectCurrentJSON["is_final"].ToString().Equals("1"))
                return false;
                
            return true;
        }
        
         void RefreshMailNotification(int __cnt) {
            mailNotify.SetActive(__cnt > 0);
        }
        
        
        
        
        
        #region 플레이 처리 
        
        public void OnClickPlay() {
            
            // * 임시 로직
            SystemManager.main.givenEpisodeData = currentEpisodeData;
            SystemManager.ShowNetworkLoading(); 
            
            // * 구매기록이 없으면, 구매처리를 한다. (0원)
            if(!currentEpisodeData.CheckExistsPurchaseData()) {
                UserManager.OnRequestEpisodePurchase = PurchasePostProcess;
                NetworkLoader.main.PurchaseEpisode(currentEpisodeData.episodeID, PurchaseState.Permanent, currentEpisodeData.currencyStarPlay, "0");
            }
            else {
                PurchasePostProcess(true);
            }
        }

        
        /// <summary>
        /// 구매 후 처리. 
        /// </summary>
        /// <param name="__isPurchaseSuccess"></param>
        void PurchasePostProcess(bool __isPurchaseSuccess)
        {
            Debug.Log(">> PurchasePostProcess : + " + __isPurchaseSuccess);
            
            if (!__isPurchaseSuccess)
            {
                Debug.LogError("Error in purchase");
                SystemManager.HideNetworkLoading();
                return;
            }
            
            if(isGameStarting)
                return;
            
            // * 구매 성공시,  현재 에피소드의 구매 정보를 갱신한다. 
            // ! 구매 정보는 갱신하고, 플레이 버튼들은 변경하지 않는다. 
            currentEpisodeData.SetPurchaseState();
            
            
            StartGame();
            
        } // ? End of purchasePostProcess        
        
        
        /// <summary>
        /// 찐 게임 start
        /// </summary>
        void StartGame()
        {
            Debug.Log("Game Start!!!!!");
            
            Signal.Send(LobbyConst.STREAM_COMMON, "GameBegin", string.Empty);
            IntermissionManager.isMovingLobby = false; // 게임으로 진입하도록 요청
            
            SystemManager.ShowNetworkLoading(); // 게임시작할때 어색하지 않게, 네트워크 로딩 추가 
            
            // 다음 에피소드 진행 
            // * 2021.09.23 iOS 메모리 이슈를 해결하기 위해 중간 Scene을 거쳐서 실행하도록 처리 
            // * GameScene에서 게임이 시작되는 경우만!
            if(GameManager.main != null) {
                SceneManager.LoadSceneAsync("Intermission", LoadSceneMode.Single).allowSceneActivation = true;
            }
            else {
                SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single).allowSceneActivation = true;
            }
            
            // true로 변경해놓는다.
            isGameStarting = true;
            
            // ! 이어하기 체크 
            string lastPlaySceneID = null;
            long lastPlayScriptNO = 0;

            // 플레이 지점 저장 정보를 가져오자. 
            if (isEpisodeContinuePlay)
            { // 이어하기 가능한 상태.
                Debug.Log("<color=yellow>CONTINUE PLAY</color>");
            
                lastPlaySceneID = projectCurrentJSON["scene_id"].ToString();
                lastPlayScriptNO = long.Parse(projectCurrentJSON["script_no"].ToString());

                GameManager.SetResumeGame(lastPlaySceneID, lastPlayScriptNO);
            }
            else
            {
                GameManager.SetNewGame(); // 새로운 게임
            }

            // 통신 
            NetworkLoader.main.UpdateUserProjectCurrent(currentEpisodeData.episodeID, lastPlaySceneID, lastPlayScriptNO);
            
            
            AppsFlyerSDK.AppsFlyer.sendEvent("episode_start", new Dictionary<string, string>() {
                { "project_id", StoryManager.main.CurrentProjectID },
                { "episode_id", StoryManager.main.CurrentEpisodeID }
            });
            
        }               
        
        
        #endregion
        
        
        
        
        public static long ConvertServerTimeTick(long __serverTick) {
            return (__serverTick * 10000) + addTick;
        }     
        
    }
}