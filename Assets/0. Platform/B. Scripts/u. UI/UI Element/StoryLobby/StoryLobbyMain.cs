using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

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
        public static Action OnEpisodePlay = null; 
        
        public static Action OnPassPurchase = null;  // 프리미엄패스 구매 콜백 
        
        
        public StoryData currentStoryData;
        public JsonData projectCurrentJSON = null;
        public string currentEpisodeID = string.Empty; // 현재 순번의 에피소드 ID 
        public EpisodeData currentEpisodeData; // 현재 순번의 에피소드 데이터 
        public bool hasPass = false; // 패스 보유 여부 
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
        
        
        
        
        // 연재 정보 관련 추가 
         public GameObject serialGroup;
         public TextMeshProUGUI textSerialDayInfo ; // 연재 주기 정보
         public TextMeshProUGUI textCommingSoon; // 공개 예정일 정보
         public TextMeshProUGUI textSerialTimer; // 연재일 타이머 (당일 오픈인 경우 ) 
         
        

        public GameObject abilityBriefPrefab;           // 캐릭터 능력치 간소화 prefab
        public Transform characterStatusContent;
        public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap abilityBriefScroll;
        public GameObject scrollNextButton;
        
        // 생성된 좌측 하단 캐릭터 세트 
        public List<CharacterAbilityBriefElement> ListBriefElements = new List<CharacterAbilityBriefElement>();
        

        public RectTransform rectContentsGroup; // 컨텐츠 그룹 
        public CanvasGroup canvasGroupContents; // 컨텐츠 그룹 canvas group
        public RectTransform arrowGroupContetns; // 컨텐츠 그룹 화살표 
        public GameObject groupStoryContentsBlock; // 접혀있는 상태에서 터치 못하게 하려고..
        
        bool inTransitionFlow = false;
        
        
        [Space]
        [Header("Sprites")]
        public Sprite spriteActiveEpisodeTitleBG;
        public Sprite spriteInactiveEpisodeTitleBG;
        
        
        
        public DateTime openDate;
        public long openDateTick;
        public TimeSpan timeDiff; // 오픈시간까지 남은 차이 
        [SerializeField] protected bool isOpenTimeCountable = false; // 타이머 카운팅이 가능한지 
        
        public int totalMin = 0; // 기다리면 무료 남은시간 분.
        public int waitingReducePrice = 0;// 기다리면 무료 강제 열기에 필요한 코인 가격 
        
        
        // 그룹 컨텐츠 변수들 
        public Vector2 posGroupContentsOrigin; // 그룹 컨텐츠 기본 위치 
        public Vector2 posGroupContentsOpen; // 그룹 컨텐츠 열림 위치 
        
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
            OnEpisodePlay = OnClickPlay;

            // 슈퍼유저 관련 처리 
            SuperUserFlowEpisodeStart = SuperUserEpisodeStart;
            
            OnPassPurchase = PostPurchasePremiumPass;
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
                textSerialTimer.text = textOpenTimer.text;
            }
        }
        
        /// <summary>
        /// 프리미엄 패스 구매 후 호출 
        /// </summary>
        void PostPurchasePremiumPass() {
            Debug.Log(">>> PostPurchasePremiumPass <<<");
            
            if(!gameObject.activeSelf)
                return;
            
            InitStoryLobbyControls();
        }
        
        /// <summary>
        /// 스토리 로비 
        /// </summary>
        public virtual void InitStoryLobbyControls() {
            
            if(UserManager.main == null || !UserManager.main.completeReadUserData)
                return;
            
            
            // 기본정보 
            InitBaseInfo();

            SetPlayState(); // 플레이 및 타이머 설정 
            
            ResetContentsGroupPos();                    
    
            // 컨텐츠 그룹 초기화 
            InitContentsGroup();
            
            
            // Flow 처리 
            InitFlowMap();

            InitAbilityBreif();

            // 게임형로비 상단 초기화
            StoryLobbyTop.OnInitializeStoryLobbyTop?.Invoke();

            // 꾸미기 모드에서 돌아왔을 때 interactable false 만들기
            ViewStoryLobby.OnInActiveInteractable?.Invoke(false);
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

            int briefIndex = 0;
            
            foreach (string key in UserManager.main.DictStoryAbility.Keys)
            {
                for (int i = 0; i < UserManager.main.DictStoryAbility[key].Count; i++)
                {
                    // 메인 능력치만 뽑아서 넣어준다
                    if (UserManager.main.DictStoryAbility[key][i].isMain)
                    {
                        ListBriefElements[briefIndex++].mainAbilityGauge.fillAmount = UserManager.main.DictStoryAbility[key][i].abilityPercent;
                        break;
                    }
                }
            }            
            

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

            Debug.Log("<color=white>" + JsonMapper.ToStringUnicode(projectCurrentJSON) + "</color>");
            isEpisodeContinuePlay = false;
            
            storyPlayButton.gameObject.SetActive(true);
            
            // 에피소드 타이틀 초기화
            SetEpisodeTitleText(string.Empty);
            
            if(projectCurrentJSON == null) {
                SystemManager.ShowSimpleAlert("Invalid Episode State. Please contact to help center");
                return;
            }
            
            currentEpisodeID = SystemManager.GetJsonNodeString(projectCurrentJSON, "episode_id");
            currentEpisodeData = StoryManager.GetRegularEpisodeByID(currentEpisodeID);

            if(currentEpisodeData == null || !currentEpisodeData.isValidData) {
                SystemManager.ShowMessageAlert("개발팀에 문의해주세요");
                return;
            }


            currentEpisodeData.SetPurchaseState(); // 구매기록 refresh.
            
            hasPass = UserManager.main.HasProjectFreepass();

            if (StoryLobbyManager.main != null && CheckResumePossible())
                isEpisodeContinuePlay = true;
            
        }
        
        
        #region 컨텐츠 그룹 제어 (앨범, 스페셜 에피소드, 엔딩, 미션)
        
        void ResetContentsGroupPos() {
            if (rectContentsGroup == null)
                return;

            rectContentsGroup.DOKill();
            rectContentsGroup.anchoredPosition = posGroupContentsOrigin; // 기본 위치로 지정 
            canvasGroupContents.alpha = 0.8f; 
            
            arrowGroupContetns.localScale = Vector3.one;
            
            groupStoryContentsBlock.SetActive(true);
        }
        
        /// <summary>
        /// 컨텐츠 그룹 초기화 
        /// </summary>
        void InitContentsGroup() {

            if(UserManager.main == null || !UserManager.main.completeReadUserData)
                return;

            
            for(int i=0; i< ListContentsButton.Count;i++) {
                ListContentsButton[i].InitContentsButton();
            }
        }
        
        /// <summary>
        /// 컨텐츠 그룹 열고 닫기 
        /// </summary>
        public void OnClickContentsArrow() {
            
            if(inTransitionFlow)
                return;
                
            if(rectContentsGroup.anchoredPosition.x < -300) { // 열림 
                rectContentsGroup.DOAnchorPos(posGroupContentsOpen, 0.2f);
                canvasGroupContents.DOFade(1, 0.2f);
                arrowGroupContetns.localScale = new Vector3(-1, 1, 1);
                
                groupStoryContentsBlock.SetActive(false);
                
            }
            else { // 닫힘 
                rectContentsGroup.DOAnchorPos(posGroupContentsOrigin, 0.2f);
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
            
            int flowIndex = 0;
            
            // 비활성화 시키고 
            for(int i=0; i<ListFlowElements.Count;i++) {
                ListFlowElements[i].gameObject.SetActive(false);
            }
            
            // EpisodeData 할당 처리
            // 최초에 InitFlowElement를 진행하고 대기시간이나 연재일 정보를 설정한다.
            for(int i=0; i<StoryManager.main.ListCurrentProjectEpisodes.Count;i++) {
                
                if(flowIndex >= ListFlowElements.Count)
                    break;
                
                // 스페셜 에피소드는 제외. 
                if(StoryManager.main.ListCurrentProjectEpisodes[i].episodeType == EpisodeType.Side)
                    continue;
                
                ListFlowElements[flowIndex++].InitFlowElement(StoryManager.main.ListCurrentProjectEpisodes[i]);
            }
            
            
            TimeSpan diffPublish; // 연재일 시간차 timediff는 대기시간에 대한 시간차 
            EpisodeData targetEpisode;
            
            // 이미 출시된 상태인지 체크 필요하다. 
            bool isPublished = true; 
            flowIndex = 0; 
            
            // 다시 loop 돌림. 
            for(int i=0; i<StoryManager.main.ListCurrentProjectEpisodes.Count;i++) {
                
                targetEpisode = StoryManager.main.ListCurrentProjectEpisodes[i];
                diffPublish = targetEpisode.publishDate - DateTime.UtcNow.AddHours(9); // UTC 기준 연재일 구한다. 
                
                // 스페셜 에피소드 해당없음 
                if(targetEpisode.episodeType == EpisodeType.Side)
                    continue;
                    
                if(targetEpisode.episodeState == EpisodeState.Prev) {
                    flowIndex++;
                    continue;
                }
                    
                
                // diffPublish의 토탈분이 0보다 크면 출시일이 미래다. 
                if(diffPublish.TotalMinutes > 0) {
                    isPublished = false; // 출시되지 않은 상태. 
                }
                
                // 현재 상태 
                if(targetEpisode.episodeState == EpisodeState.Current) {
                    
                    if(isPublished) 
                        ListFlowElements[flowIndex++].SetOpenTime(openDateTick); // tick 전달한다.(대기시간 남은정도에 상관없이)
                    else 
                        ListFlowElements[flowIndex++].SetPublishDate(targetEpisode.publishDate); // 출시되지 않은 상태면, 출시일로 전달.
                }
                else {
                    
                    // 미래 상태에 대한 처리 
                    // 출시되지 않은 상태일때만 comming soon 처리해준다.
                    if(!isPublished)
                        ListFlowElements[flowIndex++].SetPublishDate(targetEpisode.publishDate);
                    else 
                        flowIndex++;
                    
                }
                
                
            }
        } // ? InitFlowMap 끝. 
        
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

            // 능력치가 한명뿐이면 무한 스크롤을 false처리 한다
            if(UserManager.main.DictStoryAbility.Count > 1) {
                abilityBriefScroll.infinitelyScroll = true;
                abilityBriefScroll.gameObject.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Unrestricted;
            }
            else {
                abilityBriefScroll.infinitelyScroll = false;
                abilityBriefScroll.gameObject.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
                
            }
            
            ListBriefElements.Clear();


            foreach (string key in UserManager.main.DictStoryAbility.Keys)
            {
                for (int i = 0; i < UserManager.main.DictStoryAbility[key].Count; i++)
                {
                    // 메인 능력치만 뽑아서 넣어준다
                    if (UserManager.main.DictStoryAbility[key][i].isMain)
                    {
                        abilityBrief = Instantiate(abilityBriefPrefab, characterStatusContent).GetComponent<CharacterAbilityBriefElement>();
                        abilityBrief.InitAbilityBrief(UserManager.main.DictStoryAbility[key][i]);
                        
                        ListBriefElements.Add(abilityBrief);
                        
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
            
            // * 연재와 비연재로 나누어야 한다. 
            // * 연재는 프리미엄 패스에 영향을 받지 않는다. 무조건 대기해야한다. 
            // * 비연재는 프리미엄 패스에 영향을 받는다. 
            
            
            // 에피소드 오픈 시간 처리
            openDateTick = SystemConst.ConvertServerTimeTick(SystemManager.GetJsonNodeLong(projectCurrentJSON, "next_open_tick"));
            openDate = new DateTime(openDateTick); // 틱으로 오픈 시간 생성 
            
            Debug.Log("## open date : " + openDate.ToString());
            Debug.Log("## utc date : " + DateTime.UtcNow.ToString());
            
            timeDiff = openDate - DateTime.UtcNow; // 시간차 체크 
            Debug.Log("## timeDiff : " + timeDiff.Ticks);
            
            isOpenTimeCountable = false; // false로 초기화해놓고 시작한다. 
            
            
            if(timeDiff.Ticks > 0) {
                
                // 연재의 경우. 
                if(currentStoryData.isSerial && currentEpisodeData.isSerial) {
                    
                    Debug.Log("<color=cyan>Serial Episode, Future </color>");
                    
                    isOpenTimeCountable = true; 
                    currentPlayState = StatePlayButton.Serial; // ! 연재 대기 중 상태다. 
                    
                }
                else {
                    
                    Debug.Log("<color=cyan>NOT Serial Episode, Future </color>");
                    
                    if(!hasPass) { // 비연재, 프리미엄 패스 아님 
                         
                        isOpenTimeCountable = true; 
                        currentPlayState = currentStoryData.IsValidOnedayPass() || UserManager.main.ifyouPassDay > 0 ? StatePlayButton.active : StatePlayButton.inactive;
                    }
                    else { // 비연재, 프리미엄 패스 
                        
                        // 상태 처리 
                        currentPlayState = hasPass ? StatePlayButton.premium : StatePlayButton.active;
                    }
                }
            } 
            else { // 대기시간 없음 

                // 상태 처리 
                currentPlayState = hasPass ? StatePlayButton.premium : StatePlayButton.active;
                
            }
            
            
            isFinal = false;
            
            // 현재 순번 에피소드가 마지막, 엔딩이면 isFinal 변수 처리 
            if(SystemManager.GetJsonNodeBool(projectCurrentJSON, "is_final") 
                && SystemManager.GetJsonNodeBool(projectCurrentJSON, "is_ending")) {
                isFinal = true;        
            }
            
            
            // 파이널인지 아닌지에 따라 다르게 처리한다.
            // * 엔딩에는 연재물 적용이 되지 않아서 걱정할 필요 없다.
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
            SystemManager.SetText(textEpisodeTitle, __text);
        }
        
        /// <summary>
        /// 타이틀 정보 처리 
        /// </summary>
        protected void InitEpisodeTitleColor() {
            
            
            Debug.Log("<color=cyan>InitEpisodeTitleColor :: " + currentPlayState.ToString() + "</color>");
            
            imageEpisodeTitle.gameObject.SetActive(false);
            serialGroup.SetActive(false);
            
            // * 연재중인 작품이고, 정규 에피소드인 경우에 연재 정보 표시로 변환한다.
            if(currentPlayState == StatePlayButton.Serial ) {
                serialGroup.SetActive(true);
                SystemManager.SetText(textSerialDayInfo, string.Format(SystemManager.GetLocalizedText("5184"), currentStoryData.GetSeiralDay()));
                
                int remainDay = GetOpenRemainTimeDay();
                
                if(remainDay <= 0) {
                    textSerialTimer.gameObject.SetActive(true); // 당일 오픈은 타이머 등장시킨다. 
                    textCommingSoon.text = string.Empty;
                }
                else {
                    textSerialTimer.gameObject.SetActive(false); 
                    SystemManager.SetText(textCommingSoon, string.Format(SystemManager.GetLocalizedText("5188"), (int)timeDiff.Days));
                }
                
                
                if(GameManager.main != null) { // 에피소드 종료 화면에서 띄워진 경우. 
                    storyPlayButton.SetPlayButton(StatePlayButton.End, 0, false); // 로비 버튼을 띄운다.
                }
                else 
                    storyPlayButton.gameObject.SetActive(false); // 버튼을 비활성화한다.
                
                return; // 끝..!
            }
            
            
            
            imageEpisodeTitle.gameObject.SetActive(true); // 비연재 
            
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
            
            if(timeDiff.Ticks <= 0) {
                SetPlayState(); // 오픈시간이된 경우 리프레시
                return string.Empty;
            }
            
            // storyPlayButton.SetTimeOpenPrice(GetEpisodeTimeOpenPrice()); 
            textWaitingCoinPrice.text = GetEpisodeTimeOpenPrice().ToString(); // 가격이 10분마다 변한다. 
            
            return string.Format ("{0:D2}:{1:D2}:{2:D2}",timeDiff.Hours ,timeDiff.Minutes, timeDiff.Seconds);
        }
        
        
        /// <summary>
        /// 남은 시간의 일자 구하기 
        /// </summary>
        /// <returns></returns>
        protected int GetOpenRemainTimeDay() {
            timeDiff = openDate - DateTime.UtcNow;
            if(timeDiff.Ticks < 0)
                return -1;
                
            return (int)timeDiff.Days;
        }
        
        
        /// <summary>
        /// 이어하기 가능한지 체크 여부 
        /// </summary>
        /// <returns></returns>
        bool CheckResumePossible() {
            if(projectCurrentJSON == null || string.IsNullOrEmpty(SystemManager.GetJsonNodeString(projectCurrentJSON, GameConst.COL_SCENE_ID)) || SystemManager.GetJsonNodeBool(projectCurrentJSON, "is_final"))
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
                SystemManager.ShowLackOfCurrencyPopup(false, "6324", GetEpisodeTimeOpenPrice());
                return;
            }
            
            
            isWaitingResponse = true;  // 서버 응답 
            
            JsonData j = new JsonData();
            
            j[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            j[LobbyConst.EPISODE_PRICE] = GetEpisodeTimeOpenPrice();
            j[CommonConst.FUNC] = "requestWaitingEpisodeWithCoin";

            // ! 코인으로 여는거랑, 광고로 여는거랑 콜백이 달라요!
            // * 코인으로 열면, 해당 에피소드는 Permanent로 구매처리가 같이 진행된다. 
            NetworkLoader.main.SendPost(UserManager.main.CallbackReduceWaitingTimeWithCoin, j, true);
        }
        
        
        public void OnClickPlay() {
            
            // * 연재 대기 상태에서는 로비로 보낸다. 
            // 
            if(currentPlayState == StatePlayButton.Serial) {
                if(GameManager.main != null) {
                    UserManager.main.gameComplete = true;
                    GameManager.main.EndGame();
                    return;
                }
            }
            
            // 카운팅 돌아가는 도중에는 플레이가 아니고, 코인으로 감소하기 
            if(isOpenTimeCountable) {
                
                // 오픈 메뉴 띄운다. 
                menuReduceWaitingTime.SetActive(true);
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
                    SystemManager.ShowMessageAlert("Episode data is not valid");
                    return;
                }
                
                SystemManager.ShowStoryResetPopup(firstEpisode);
                return;
            } // ? 엔딩 도달한 경우 처리 끝
            
            
            // 에피소드 진입 처리 
            SystemManager.main.givenEpisodeData = currentEpisodeData;
            SystemManager.ShowNetworkLoading(true); 
            
            PurchaseState episodePurchaseState = currentEpisodeData.purchaseState;
            
            
            // * 프리미엄 패스 구매 여부 체크 필요. 
            // 22.07.19 원데이 패스, 이프유패스 작품 광고 제거
            if(hasPass || currentStoryData.IsValidOnedayPass() || UserManager.main.ifyouPassDay > 0) {
                
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
            
            SystemManager.isQuitGame = false; // 변수 초기화
            UserManager.main.useRecord = true;
            
            SystemManager.ShowNetworkLoading(); // 게임시작할때 어색하지 않게, 네트워크 로딩 추가 
            
            // 다음 에피소드 진행 
            // * 2021.09.23 iOS 메모리 이슈를 해결하기 위해 중간 Scene을 거쳐서 실행하도록 처리 
            // * GameScene에서 게임이 시작되는 경우만!
            //if(GameManager.main != null)
            //    SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
            //else
            //    SceneManager.LoadSceneAsync(CommonConst.SCENE_GAME, LoadSceneMode.Single).allowSceneActivation = true;
            
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
            
                lastPlaySceneID = SystemManager.GetJsonNodeString(projectCurrentJSON, GameConst.COL_SCENE_ID);
                lastPlayScriptNO = SystemManager.GetJsonNodeLong(projectCurrentJSON, "script_no");

                GameManager.SetResumeGame(lastPlaySceneID, lastPlayScriptNO);
            }
            else
            {
                GameManager.SetNewGame(); // 새로운 게임
            }

            // 통신 
            NetworkLoader.main.UpdateUserProjectCurrent(currentEpisodeData.episodeID, lastPlaySceneID, lastPlayScriptNO, false, "StartGame");
            
            Dictionary<string, string> eventValues = new Dictionary<string, string>();
            eventValues.Add("project_id", StoryManager.main.CurrentProjectID);
            eventValues.Add("episode_id", StoryManager.main.CurrentEpisodeID);
            AdManager.main.SendAppsFlyerEvent("af_episode_start", eventValues);
            
        }               
        
        
        #endregion
        
        
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