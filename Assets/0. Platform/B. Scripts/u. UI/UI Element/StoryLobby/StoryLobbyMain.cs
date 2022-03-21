using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;
using LitJson;
using DG.Tweening;
using Doozy.Runtime.Signals;


namespace PIERStory {

    public class StoryLobbyMain : MonoBehaviour
    {
        
        public static Action OnCallbackReset = null; // 리셋 콜백
        public static Action CallbackReduceWaitingTimeSuccess = null; // 시간감소 콜백 (성공)
        public static Action CallbackReduceWaitingTimeFail = null; // 시간감소 콜백(실패)
        public static Action<EpisodeData> SuperUserFlowEpisodeStart = null; // 플로우맵 슈퍼유저 에피소드 시작하기 
        public static Action OnInitializeContentGroup = null;
        
        
        public StoryData currentStoryData;
        public JsonData projectCurrentJSON = null;
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
        public TextMeshProUGUI textWaitingCoinPrice; // 남은시간에 따른 오픈 코인 가격 
        public TextMeshProUGUI textReduceWaitingTime; // 광고보고 줄어드는 시간 TextMeshPro
        public GameObject menuReduceWaitingTime; // 대기시간 줄어들기 메뉴 
        
        
        public Image imageEpisodeTitle; // 에피소드 타이틀 배경 
        public TextMeshProUGUI textEpisodeTitle; // 에피소드 타이틀 
        public GameObject groupOpenTimer; // 오픈 타이머
        public TextMeshProUGUI textOpenTimer; // 오픈 타이머 

        public GameObject abilityBriefPrefab;           // 캐릭터 능력치 간소화 prefab
        public Transform characterStatusContent;
        public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap abilityBriefScroll;
        public GameObject scrollNextButton;

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
        [SerializeField] protected bool isOpenTimeCountable = false; // 타이머 카운팅이 가능한지 
        
        public int totalMin = 0; // 기다리면 무료 남은시간 분.
        public int waitingReducePrice = 0;// 기다리면 무료 강제 열기에 필요한 코인 가격 
        
        
        // 그룹 컨텐츠 변수들 
        public Vector2 posGroupContentsOrigin; // 그룹 컨텐츠 기본 위치 
        public Vector2 posGroupContentsOpen; // 그룹 컨텐츠 열림 위치 
        bool inTransitionGroupContents; // 그룹 컨텐츠 트랜지션 여부 
        
        bool isGameStarting = false; // 게임 시작했는지 체크, 중복 입력 막기 위해서.
        [SerializeField] protected bool isEpisodeContinuePlay = false; // 에피소드 이어하기 상태? 
        bool isWaitingResponse = false; //  서버 응답 기다리는 중인지. 
        public bool isFinal = false; // 엔딩 도착 상태
        
        public Color colorEpisodeTitleNormal; // 타이틀 노멀엔딩 색상
        public Color colorEpisodeTitleHidden; // 타이틀 히든엔딩 
        public Color colorEpisodeTitleHappy; // 타이틀 해피 
        public Color colorEpisodeTitleSad; // 타이틀 새드 
        
        private void Start() {
            // Action 연결
            OnCallbackReset = RefreshAfterReset; 
            CallbackReduceWaitingTimeSuccess = RefreshAfterReduceWaitingTime; 
            CallbackReduceWaitingTimeFail = FailReduceWaitingTime;
            OnInitializeContentGroup = InitContentsGroup;

            // 슈퍼유저 관련 처리 
            SuperUserFlowEpisodeStart = SuperUserEpisodeStart;
            
            UserManager.OnFreepassPurchase = PostPurchasePremiumPass;
        }
        
        protected virtual void Update() {
            
            // * 기다무 시스템 관련 타이밍 처리 
            if(!isOpenTimeCountable) {
                return;
            }
            
            // 서버 응답 대기중에는 타이머 체크하지 않음. 
            if(isWaitingResponse)
                return; 
            
            // 타이머 처리 
            if(Time.frameCount % 5 == 0) {
                textOpenTimer.text = GetOpenRemainTime();
            }
        }
        
        /// <summary>
        /// 프리미엄 패스 구매 후 호출 
        /// </summary>
        void PostPurchasePremiumPass() {
            Debug.Log(">>> PostPurchasePremiumPass <<<");
            
            if(!this.gameObject.activeSelf)
                return;
            
            InitStoryLobbyControls();
        }
        
        /// <summary>
        /// 스토리 로비 
        /// </summary>
        public virtual void InitStoryLobbyControls() {
            
            
            Debug.Log("## InitStoryLobbyControls");
            
            // 기본정보 
            InitBaseInfo();

            SetPlayState(); // 플레이 및 타이머 설정 
                        
    
            // 컨텐츠 그룹 초기화 
            InitContentsGroup();
            
            
            // Flow 처리 
            InitFlowMap();

            InitAbilityBreif();

            // 슈퍼유저 
            StoryLobbyTop.OnRefreshSuperUser?.Invoke();
            
            // 게임형로비 상단 초기화
            StoryLobbyTop.OnInitializeStoryLobbyTop?.Invoke();
        }


        public void MainContainerHide()
        {
            for (int i = 0; i < characterStatusContent.childCount; i++)
                Destroy(characterStatusContent.GetChild(i).gameObject);
        }

        
        /// <summary>
        /// 리셋 후 리프레시
        /// </summary>
        void RefreshAfterReset() {
            Debug.Log(" >> RefreshAfterReset");
            InitBaseInfo();
            SetPlayState();
            InitFlowMap();

        }
        
        /// <summary>
        /// 오픈시간 감소 후 리프레시 
        /// </summary>
        protected void RefreshAfterReduceWaitingTime() {
            
            Debug.Log(" >> RefreshAfterReduceWaitingTime");
            
            InitBaseInfo();
            SetPlayState();
            
            isWaitingResponse = false;
            
            menuReduceWaitingTime.SetActive(false); // 메뉴 닫기 
        }
        
        protected void FailReduceWaitingTime() {
            
            isWaitingResponse = false;
        }
        
        
        /// <summary>
        /// 기본정보 처리
        /// </summary>
        void InitBaseInfo() {
            textReduceWaitingTime.text = SystemManager.main.waitingReduceTimeAD.ToString() +" min"; // 광고보고 차감되는 시간 SysteManager..
           
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
            currentEpisodeData.SetPurchaseState(); // 구매기록 refresh.
            
            
            hasPremium = UserManager.main.HasProjectFreepass();
            
            if(LobbyManager.main != null && CheckResumePossible()) {
                isEpisodeContinuePlay = true;
            }
            
            
        }
        
        
        #region 컨텐츠 그룹 제어 (앨범, 스페셜 에피소드, 엔딩, 미션)
        
        /// <summary>
        /// 컨텐츠 그룹 초기화 
        /// </summary>
        void InitContentsGroup() {

            if (rectContentsGroup == null)
                return;

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
            
            int flowIndex = 0;
            
            // EpisodeData 할당 처리
            for(int i=0; i<StoryManager.main.ListCurrentProjectEpisodes.Count;i++) {
                
                if(flowIndex >= ListFlowElements.Count)
                    break;
                
                // 스페셜 에피소드는 제외. 
                if(StoryManager.main.ListCurrentProjectEpisodes[i].episodeType == EpisodeType.Side)
                    continue;
                
                ListFlowElements[flowIndex++].InitFlowElement(StoryManager.main.ListCurrentProjectEpisodes[i]);
            }
            
            // 프리미엄 패스 유저는 타이머를 돌리지 않음 
            if(hasPremium)
                return; 
            
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
                
            flowMap.DOAnchorPos(new Vector2(-85, 0), 0.3f).OnStart(()=> {inTransitionFlow = true;}).OnComplete(()=>{inTransitionFlow = false;});
        }
        
        public void OnClickFlowClose() {
            if(inTransitionFlow)           
                    return;   
                    
            flowMap.DOAnchorPos(new Vector2(-820, 0), 0.3f).OnStart(()=> {inTransitionFlow = true;}).OnComplete(()=>{inTransitionFlow = false;});
        }  
        
        #endregion
        
        /// <summary>
        /// 캐릭터 능력치 간소화 표기
        /// </summary>
        void InitAbilityBreif()
        {
            // 캐릭터 능력치가 없으면 생성하지 말자
            if (UserManager.main.DictStoryAbility.Count == 0)
            {
                abilityBriefScroll.gameObject.SetActive(false);
                scrollNextButton.SetActive(false);
                return;
            }

            CharacterAbilityBriefElement abilityBrief = null;
            abilityBriefScroll.gameObject.SetActive(true);
            scrollNextButton.SetActive(true);

            // 능력치 있는 캐릭터가 한 명뿐이라면 next버튼이 필요가 없지
            if (UserManager.main.DictStoryAbility.Count == 1)
                scrollNextButton.SetActive(false);

            foreach (string key in UserManager.main.DictStoryAbility.Keys)
            {
                for (int i = 0; i < UserManager.main.DictStoryAbility[key].Count; i++)
                {
                    // 메인 능력치만 뽑아서 넣어준다
                    if (UserManager.main.DictStoryAbility[key][i].isMain)
                    {
                        abilityBrief = Instantiate(abilityBriefPrefab, characterStatusContent).GetComponent<CharacterAbilityBriefElement>();
                        abilityBrief.InitAbilityBrief(UserManager.main.DictStoryAbility[key][i]);
                        break;
                    }
                }
            }


            // 생성 후 panel 수 갱신
            abilityBriefScroll.Setup();
        }


        
        /// <summary>
        /// 상태 및 오픈 타이머 설정 
        /// </summary>
        public void SetPlayState() {
            
            // * 스페셜 에피소드를 플레이 한 경우에 대한 예외처리 추가 
            if(currentEpisodeData.episodeType == EpisodeType.Side) {
                
                Debug.Log("### ViewEpisodeEnd Special Episode");
                
                // 카운트 하지 않고, Lobby로 보내도록 해야한다. 
                isOpenTimeCountable = false;
                storyPlayButton.SetPlayButton(StatePlayButton.End, 0, false);
                groupOpenTimer.SetActive(false);
                textEpisodeTitle.gameObject.SetActive(true);
                
                InitEpisodeTitleColor(); 
                
                return;
            }
            
            
            
            // 에피소드 오픈 시간 처리
            openDateTick = ConvertServerTimeTick(long.Parse(projectCurrentJSON["next_open_tick"].ToString()));
            openDate = new DateTime(openDateTick); // 틱으로 오픈 시간 생성 
            
            Debug.Log("## open date : " + openDate.ToString());
            Debug.Log("## utc date : " + DateTime.UtcNow.ToString());
            
            timeDiff = openDate - DateTime.UtcNow; // 시간차 체크 
            Debug.Log("## timeDiff : " + timeDiff.Ticks);
            
            isOpenTimeCountable = false;
            
            
            
            // 프리미엄 패스 유저는 tick 이 0보다 커도 기다무를 하지 않도록 처리 
            if(timeDiff.Ticks > 0 && !hasPremium) {
                isOpenTimeCountable = true; // 시간 돌아간다. 
                currentPlayState = StatePlayButton.inactive;
            }
            else {
                // 프리미엄 유저인지 아닌지에 따라서 상태 처리 
                currentPlayState = hasPremium ? StatePlayButton.premium : StatePlayButton.active;
            }
            
            
            isFinal = false;
            
            // 현재 순번 에피소드가 마지막, 엔딩이면 isFinal 변수 처리 
            if(SystemManager.GetJsonNodeBool(projectCurrentJSON, "is_final") 
                && SystemManager.GetJsonNodeBool(projectCurrentJSON, "is_ending")) {
                isFinal = true;        
            }
            
            
            // 파이널인지 아닌지에 따라 다르게 처리한다.
            if(isFinal) {
                // 플레이 버튼을 리셋플레이로 변경한다.    
                storyPlayButton.SetPlayButton(StatePlayButton.End, 0, false);
                
                groupOpenTimer.SetActive(false);
                textEpisodeTitle.gameObject.SetActive(true);
            }
            else {
                groupOpenTimer.SetActive(isOpenTimeCountable);
                textEpisodeTitle.gameObject.SetActive(!isOpenTimeCountable);
                
                    
                // 스토리 플레이버튼 초기화 
                storyPlayButton.SetPlayButton(currentPlayState, currentEpisodeData.sceneProgressorValue, CheckResumePossible());
                textWaitingCoinPrice.text = GetEpisodeTimeOpenPrice().ToString();                
            }

            InitEpisodeTitleColor();
            
        }
        
        /// <summary>
        /// 에피소드 기다리면 무료 오픈 가격 구하기 
        /// </summary>
        /// <returns></returns>
        protected int GetEpisodeTimeOpenPrice() {
            if(currentPlayState != StatePlayButton.inactive)
                return 0;
                
            totalMin = (int)(timeDiff.TotalMinutes); // 남은시간 분단위로 가져오기 
            waitingReducePrice = totalMin / 10 * SystemManager.main.episodeOpenPricePer + SystemManager.main.episodeOpenPricePer;
            
            // 최소가격 설정 
            if(waitingReducePrice < SystemManager.main.episodeOpenPricePer)
                waitingReducePrice = SystemManager.main.episodeOpenPricePer;
                
            return waitingReducePrice;
        }
        
        
        /// <summary>
        /// 에피소드 타이틀 텍스트 설정 
        /// </summary>
        /// <param name="__text"></param>
        protected void SetEpisodeTitleText(string __text) {
            textEpisodeTitle.text = __text;
        }
        
        /// <summary>
        /// 타이틀 정보 처리 
        /// </summary>
        protected void InitEpisodeTitleColor() {
            
            if(currentEpisodeData.episodeType == EpisodeType.Side) {
                imageEpisodeTitle.sprite = spriteActiveEpisodeTitleBG;    
                textEpisodeTitle.color = Color.black;
                SetEpisodeTitleText(currentEpisodeData.storyLobbyTitle);
            }
            
            
            // * 여기도 파이널 여부 추가 체크 
            if(isFinal) {
                imageEpisodeTitle.sprite = spriteActiveEpisodeTitleBG;    
                textEpisodeTitle.color = Color.black;
                string endingText = string.Empty;
                               
                if(currentEpisodeData.endingType == "hidden") { // 히든
                    imageEpisodeTitle.color = colorEpisodeTitleHidden;
                    textEpisodeTitle.color = Color.white;
                    // endingText = "Hidden Ending";
                }
                else if(currentEpisodeData.endingType == "final") { // 해피
                    imageEpisodeTitle.color = colorEpisodeTitleHappy;
                    // endingText = "Final Ending";
                }
                else if(currentEpisodeData.endingType == "normal") { // 노멀 
                    imageEpisodeTitle.color = colorEpisodeTitleNormal;
                    // endingText = "Normal Ending";
                }
                else if(currentEpisodeData.endingType == "sad") { // 새드 
                    imageEpisodeTitle.color = colorEpisodeTitleSad;
                    // endingText = "Sad Ending";
                }
                
                // textEpisodeTitle.text = endingText;
                SetEpisodeTitleText(currentEpisodeData.storyLobbyTitle);
                
            }
            else { // 엔딩 도달하지 않았을 경우 일반 처리 
                // 타이틀 설정 
                SetEpisodeTitleText(currentEpisodeData.storyLobbyTitle);
                imageEpisodeTitle.color = Color.white;
                
                if(currentPlayState == StatePlayButton.inactive) {
                    imageEpisodeTitle.sprite = spriteInactiveEpisodeTitleBG;    
                    imageEpisodeTitle.color = Color.white; // 색상 초기화 
                }
                else {
                    imageEpisodeTitle.sprite = spriteActiveEpisodeTitleBG;    
                    textEpisodeTitle.color = Color.black;
                }                
            }
        }
        
        
        /// <summary>
        /// 오픈시간까지 남은시간 구하기 (UTC 기준, 서버에서도 UTC로 준다. )
        /// </summary>
        /// <returns></returns>        
        protected string GetOpenRemainTime() {
            timeDiff = openDate - DateTime.UtcNow;
            
            if(timeDiff.Ticks < 0) {
                SetPlayState(); // 오픈시간이된 경우 리프레시
                return string.Empty;
            }
            
            // storyPlayButton.SetTimeOpenPrice(GetEpisodeTimeOpenPrice()); 
            textWaitingCoinPrice.text = GetEpisodeTimeOpenPrice().ToString(); // 가격이 10분마다 변한다. 
            
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
        
 
        
        
        
        
        
        #region 플레이 버튼 관련 처리 
        
        /// <summary>
        /// 광고 보고 오픈 시간 차감처리 
        /// </summary>
        public void OnClickReduceWaitingTimeAD() {
            AdManager.main.ShowRewardAdWithCallback(RequestReduceWaitingTimeAD);
        }
        
        void RequestReduceWaitingTimeAD(bool isRewarded) {
            
            if(!isRewarded)
                return;
            
            JsonData j = new JsonData();

            j["project_id"] = StoryManager.main.CurrentProjectID;
            j["func"] = "requestWaitingEpisodeWithAD";

            NetworkLoader.main.SendPost(UserManager.main.CallbackReduceWaitingTime, j);
        }
        
        
        
        /// <summary>
        /// 코인 써서 오픈 시간 차감 처리 
        /// </summary>
        public void OnClickReduceWaitingTimeCoin() {
            
            if(isWaitingResponse)
                return;

            // 필요한 수만큼 가지고 있는지 체크해서 없으면 상점으로 보내는 팝업 띄워주기
            if(!UserManager.main.CheckCoinProperty(GetEpisodeTimeOpenPrice()))
            {
                SystemManager.ShowConnectingShopPopup(SystemManager.main.spriteCoin, GetEpisodeTimeOpenPrice() - UserManager.main.coin);
                return;
            }
            
            
            isWaitingResponse = true;  // 서버 응답 
            
            JsonData j = new JsonData();
            
            j["project_id"] = StoryManager.main.CurrentProjectID;
            j["price"] = GetEpisodeTimeOpenPrice();
            j["func"] = "requestWaitingEpisodeWithCoin";

            // ! 코인으로 여는거랑, 광고로 여는거랑 콜백이 달라요!
            // * 코인으로 열면, 해당 에피소드는 Permanent로 구매처리가 같이 진행된다. 
            NetworkLoader.main.SendPost(UserManager.main.CallbackReduceWaitingTimeWithCoin, j, true);
        }
        
        
        public void OnClickPlay() {
            
            // 카운팅 돌아가는 도중에는 플레이가 아니고, 코인으로 감소하기 
            if(isOpenTimeCountable) {
                
                // 오픈 메뉴 띄운다. 
                menuReduceWaitingTime.SetActive(true);
                
                // int openPrice = GetEpisodeTimeOpenPrice();
                
                // SystemManager.ShowResourceConfirm(SystemManager.GetLocalizedText("6221"), GetEpisodeTimeOpenPrice(), )
                /*
                SystemManager.ShowResourceConfirm(string.Format(SystemManager.GetLocalizedText("6221"), openPrice)
                                , openPrice
                                , SystemManager.main.GetCurrencyImageURL("coin")
                                , SystemManager.main.GetCurrencyImageKey("coin")
                                , OnClickReduceWaitingTimeCoin
                                , SystemManager.GetLocalizedText("5041")
                                , SystemManager.GetLocalizedText("5040"));
                */
                // OnClickReduceWaitingTimeCoin();
                return;
            }
            
            // * 엔딩까지 모두 보고 마지막에 도달한 경우는 Reset을 띄운다.
            if(storyPlayButton.stateButton == StatePlayButton.End) {
                
                // 게임씬에서 마지막에 도달한 경우 로비로 돌려보낸다.
                if(GameManager.main != null) {
                    UserManager.main.gameComplete = true;
                    GameManager.main.EndGame();
                    return;
                }
                
                EpisodeData firstEpisode = StoryManager.GetFirstRegularEpisodeData(); // 첫 에피소드 
                
                // 데이터 이상할때..
                if(firstEpisode == null || !firstEpisode.isValidData) {
                    SystemManager.ShowMessageAlert("Episode data is not valid", false);
                    return;
                }
                
                SystemManager.ShowStoryResetPopup(firstEpisode);
                return;
            } // ? 엔딩 도달한 경우 처리 끝
            
            
            
            
            // 에피소드 진입 처리 
            SystemManager.main.givenEpisodeData = currentEpisodeData;
            SystemManager.ShowNetworkLoading(); 
            
            PurchaseState episodePurchaseState = currentEpisodeData.purchaseState;
            
            
            // * 프리미엄 패스 구매 여부 체크 필요. 
            if(hasPremium) {
                
                if(episodePurchaseState != PurchaseState.Permanent) {
                    
                    // 구매상태가 영구적인 상태가 아니라면 재구매 처리한다. 
                    UserManager.OnRequestEpisodePurchase = PurchasePostProcess;
                    NetworkLoader.main.PurchaseEpisode(currentEpisodeData.episodeID, PurchaseState.Permanent, currentEpisodeData.currencyStarPlay, "0");
                }
                else {
                    PurchasePostProcess(true); // 영구적인 상태라면 그냥 바로 진행 
                }
                
            }
            else {
                // * 프리미엄 패스 유저 아님 
                
                // 영구적인 상태가 아니라면 AD로 진행 
                // nextOpenMin이 0보다 커야한다 (기다무 대상 에피소드)
                if(episodePurchaseState != PurchaseState.Permanent && currentEpisodeData.nextOpenMin > 0) {
                    
                    UserManager.OnRequestEpisodePurchase = PurchasePostProcess;
                    NetworkLoader.main.PurchaseEpisode(currentEpisodeID, PurchaseState.AD, "none", "0");
                }
                else { // 영구적인 상태라면 그냥 바로 진행
                    PurchasePostProcess(true); // 그냥 진행 
                }
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
        void StartGame(bool __isNewGameForce = false)
        {
            Debug.Log("Game Start!!!!!");
            
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_GAME_BEGIN, string.Empty);
            IntermissionManager.isMovingLobby = false; // 게임으로 진입하도록 요청
            UserManager.main.useRecord = true;
            
            SystemManager.ShowNetworkLoading(); // 게임시작할때 어색하지 않게, 네트워크 로딩 추가 
            
            // 다음 에피소드 진행 
            // * 2021.09.23 iOS 메모리 이슈를 해결하기 위해 중간 Scene을 거쳐서 실행하도록 처리 
            // * GameScene에서 게임이 시작되는 경우만!
            if(GameManager.main != null)
                SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
            else
                SceneManager.LoadSceneAsync(CommonConst.SCENE_GAME, LoadSceneMode.Single).allowSceneActivation = true;
            
            // true로 변경해놓는다.
            isGameStarting = true;
            
            if(__isNewGameForce) {
                GameManager.SetNewGame(); // 새로운 게임
                return;
            }
            
            
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
            
            
            Firebase.Analytics.FirebaseAnalytics.LogEvent("EpisodeStart", new Firebase.Analytics.Parameter("project_id", StoryManager.main.CurrentProjectID)
            , new Firebase.Analytics.Parameter("episode_id", StoryManager.main.CurrentEpisodeID));
            
        }               
        
        
        #endregion
        
        
        
        
        public static long ConvertServerTimeTick(long __serverTick) {
            return (__serverTick * 10000) + addTick;
        } 
        
        
        /// <summary>
        /// 슈퍼유저 
        /// </summary>
        /// <param name="__episodeData"></param>
        void SuperUserEpisodeStart(EpisodeData __episodeData) {
            SystemManager.main.givenEpisodeData = __episodeData;
            SystemManager.ShowNetworkLoading();
            StartGame(true);
        }
        

        
    }
}