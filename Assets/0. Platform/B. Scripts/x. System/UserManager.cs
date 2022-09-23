using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;

namespace PIERStory
{

    /// <summary>
    /// 사용자 정보 
    /// </summary>
    public class UserManager : MonoBehaviour
    {
        public static UserManager main = null;

        [SerializeField] bool isAdminUser = false; // 슈퍼유저
        public bool isIntroDone = false; // 인트로 수행 여부 
        
        public int diffRate = 0; //마지막 평가 팝업 뜬 날짜와 현재의 일수 차이 
        public int rateResult = 0; // 0 : 평가하지 않음, 1 : 평가함

        [HideInInspector] public JsonData userJson = null; // 계정정보 (table_account) 
        [HideInInspector] public JsonData bankJson = null; // 유저 소모성 재화 정보 (gem, coin)
        [HideInInspector] public JsonData notReceivedMailJson = null;     // 미수신 메일


        [HideInInspector] public JsonData userIfyouPlayJson = null;         // 이프유플레이 json data
        

        [SerializeField] string debugBankString = string.Empty;

        public List<CoinIndicator> ListCoinIndicators = new List<CoinIndicator>(); // 코인 표시기
        public List<GemIndicator> ListGemIndicators = new List<GemIndicator>(); // 젬 표시기



        #region Actions
        public static Action OnCleanUserEpisodeProgress; // 유저 에피소드 씬 진척도 클리어 콜백 
        public static Action<bool> OnRequestEpisodePurchase = null; // 에피소드 구매 콜백처리

        public static Action OnFreepassPurchase = null; // 프리패스 구매 콜백처리 

        public static Action<int> OnRefreshUnreadMailCount = null; // 미수신 메일 개수 Refresh Action

        #endregion



        /// <summary>
        /// 아래의 정보가 포함된다
        /// episodePurchase : 구매한 에피소드 리스트
        /// episodeScene : 진행 중인 상황 ID 목록
        /// dressCode : 캐릭터 의상 정보 
        /// models 
        /// illusts
        /// challenges
        /// 
        /// 
        /// 
        /// 유저 정보 
        /// drssProgress, favorProgress, illustProgress, challengeProgress 
        /// 진행중인 상황 ID와 클리어 상황 기록은 다르다. 
        /// 
        /// </summary>
        [HideInInspector] public JsonData currentStoryJson = null; // 선택한 프로젝트와 관련된 정보 
        [HideInInspector] public JsonData currentStorySelectionHistoryJson = null;      // 선택한 프로젝트의 선택지 내역(히스토리)

        [HideInInspector] public bool completeReadUserData = false;



        


        /// <summary>
        /// 사건 기록을 하는 통신을 사용할 것인가?
        /// 엔딩 수집 화면에서 사용할 변수
        /// </summary>
        public bool useRecord = true;
        public bool gameComplete = false;

        public string userKey = string.Empty;
        public string gamebaseID = string.Empty;

        // 튜토리얼 관련 정보
        public int tutorialStep = 1;
        public bool tutorialClear = false;

        public bool isSelectionTutorialClear = false; // 선택지 튜토리얼 초기화 여부 
        public bool isHowToPlayClear = false; // How to play 튜토리얼 초기화 여부 

        public int ifyouPassDay = 0;        // 이프유패스 일수


        public string nickname = string.Empty;      // 유저 닉네임
        public string accountLink = string.Empty; // 유저 계정 연동 정보 (table_account)

        public int gem = 0;
        public int coin = 0;
        public int unreadMailCount = 0; // 미수신 메일 카운트

        
        public TimeSpan dailyMissionTimer; // 00시를 기준으로 하는 타이머 
        DateTime dateDailyMission;
        public TimeSpan adCoolDownTimer; // 광고 쿨타임용 타이머 
        DateTime dateAdCoolDown; 
        long tickParam; // 틱 계산용 변수 

        public long allpassExpireTick = 0; // 올패스 만료일시 tick
        public DateTime allpassExpireDate; // 올패스 만료일시
        TimeSpan allpassTimeDiff;


        #region 유저 등급 관련 변수

        public int grade = 1;
        public string gradeName = string.Empty;

        public int nextGrade = 0;           // 예상 다음 시즌 등급

        public int gradeExperience = 0;
        public int keepPoint = 0;
        public int upgradeGoalPoint = 0;
        public int remainDay = 0;
        public int additionalStarDegree = 0;
        public int additionalStarLimitCount = 0;
        public int additionalStarUse = 0;
        public int waitingSaleDegree = 0;
        public bool canPreview = false;

        #endregion

        JsonData resultProjectCurrent = null; // 플레이 위치 

        JsonData resultEpisodeRecord = null; // 에피소드 기록 통신 결과
        JsonData resultEpisodeReset = null; // 에피소드 리셋 통신 결과

        JsonData currentStoryMissionJSON = null; // 현재 선택한 스토리의 미션 JSON
        public Dictionary<int, MissionData> DictStoryMission; // 미션 Dictionary
        public Sprite spriteMissionPopup;       // 미션 팝업에서 사용되는 아이콘

        JsonData currentStoryAbilityJson = null;    // 현재 선택한 작품의 능력치 Json
        public Dictionary<string, List<AbilityData>> DictStoryAbility;      // 받아온 능력치 화자별로 딕셔너리 처리 
        public Dictionary<string, List<AbilityData>> DictOldStoryAbility;   // 코인샵에서 구매전의 능력치 정보 저장용도

        public List<AchievementData> listAchievement;

        #region static const 

        // getUserSelectedStory를 통해 받아온 작품 관련 정보 


        public const string UN_UNREAD_MAIL_COUNT = "unreadMailCount"; // 미수신 메일 개수
        

        const string NODE_TUTORIAL_STEP = "tutorial_step";
        const string NODE_TUTORIAL_CLEAR = "tutorial_clear";
        const string NODE_TUTORIAL_CURRENT = "tutorial_current";


        public const string NODE_BUBBLE_SET = "bubbleSet"; // 말풍선 세트 정보 
        const string NODE_BUBBLE_SPRITE = "bubbleSprite"; // 말풍선 스프라이트 정보 


        const string NODE_PROJECT_MISSIONS = "missions";                    // 프로젝트의 모든 미션


        // 사용자 정보 
        const string NODE_USER_VOICE = "voiceHistory";      // 유저 보이스(더빙) 히스토리 정보
        const string NODE_USER_RAW_VOICE = "rawVoiceHistory"; // 유저 보이스 히스토리 (Raw 타입)

        const string NODE_USER_GALLERY_IMAGES = "galleryImages"; // * 유저 갤러리 이미지 (해금 여부 포함됨) - 일반 & 라이브 페어 시스템 


        public const string NODE_PURCHASE_HIST = "episodePurchase";  //episodePurchase 에피소드 구매 기록
        const string NODE_SCENE_PROGRESS = "sceneProgress"; // 사건ID 진행도. 조건 판정에 사용한다. 
        const string NODE_SCENE_HISTORY = "sceneHistory"; // 한번이라도 오픈한 사건 ID 기록 (삭제되지 않음)
        const string NODE_FIRST_CLEAR_RESULT = "firstClearResult";      // 에피소드 최초 클리어 보상
                

        const string NODE_EPISODE_PROGRESS = "episodeProgress"; // 에피소드 진행도
        const string NODE_EPISODE_HISTORY = "episodeHistory"; // 에피소드 히스토리 


        const string NODE_NEXT_EPISODE = "nextEpisodeID"; // 다음 에피소드 정보 



        const string NODE_PROJECT_CURRENT = "projectCurrent"; // 유저의 작품에서의 플레이 위치 
        const string NODE_SELECTION_PROGRESS = "selectionProgress"; // 선택지 프로그레스 


        const string NODE_SELECTION_PURCHASE = "selectionPurchase";
        const string NODE_SELECTION_HINT_PURCHASE = "selectionHintPurchase";

        public const string NODE_USER_ABILITY = "ability"; // 포장된 유저 능력치
        public const string NODE_RAW_STORY_ABILITY = "rawStoryAbility"; // 스토리 누적 능력치 RAW 데이터 

        

        #endregion


        private void Awake()
        {
            // System(SystemManager)에 귀속이라 Destory 할 필요없다.
            // 다른 씬에서 넘어온 객체가 있을경우. 
            if (main != null)
                return;

            // Singleton
            main = this;

            ListCoinIndicators.Clear();
            ListGemIndicators.Clear();
        }

        

        /// <summary>
        /// 서버에 접속을 시작합니다. 
        /// </summary>
        public void ConnectGameServer(string __gamebaseID)
        {
            
            Debug.Log(string.Format("<color=cyan>ConnectServer [{0}]</color>", __gamebaseID));
            gamebaseID = __gamebaseID;
            
            JsonData sendingData = new JsonData(); // 서버 전송 데이터 
            sendingData[CommonConst.FUNC] = NetworkLoader.FUNC_LOGIN_CLIENT;
            sendingData["deviceid"] = SystemInfo.deviceUniqueIdentifier;
            sendingData["gamebaseid"] = gamebaseID;

            if (NetworkLoader.main == null)
                return;

            NetworkLoader.main.SendPost(CallbackConnectServer, sendingData, true);
        }

        /// <summary>
        /// 로그인 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void CallbackConnectServer(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed Connect Server");
                return;
            }

            Debug.Log(string.Format("CallbackConnectServer: {0}", res.DataAsText));

            userJson = JsonMapper.ToObject(res.DataAsText);

            SystemManager.main.ShowAgreementTermsPopUp();

            // 소모성 재화 정보 update
            SetBankInfo(userJson);

            // 알림 정보 update
            SetNotificationInfo(userJson);

            // 유저 정보
            SetUserInfo(userJson);


            // 유저 정보 불러왔으면, Lobby로 진입을 요청합니다. 
            completeReadUserData = true;
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_CONNECT_SERVER, string.Empty);



            // 서비스 중인 스토리 리스트 조회
            RequestServiceStoryList();

            // 푸시 토큰
            SystemManager.main.QueryPushTokenInfo(null);
        }

        /// <summary>
        /// 서비스 중인 작품 리스트 조회하기 
        /// </summary>
        public void RequestServiceStoryList()
        {
            StoryManager.main.RequestStoryList(OnRequestServiceStoryList);
        }

        /// <summary>
        /// callback 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void OnRequestServiceStoryList(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkLoader.CheckResponseValidation(request, response))
            {
                return;
            }

            Debug.Log(">> OnRequestServiceStoryList : " + response.DataAsText);

            // 작품 리스트 받아와서 스토리 매니저에게 전달. 
            StoryManager.main.SetStoryList(JsonMapper.ToObject(response.DataAsText));

            // 프로모션, 공지사항, 출석 체크 조회 
            RequestServiceEvents();
        }

        /// <summary>
        /// 진행중인 이벤트 정보 조회 
        /// </summary>
        public void RequestServiceEvents()
        {
            NetworkLoader.main.RequestPlatformServiceEvents(); // 공지사항, 프로모션, 장르 조회 
            NetworkLoader.main.RequestIfyouplayList();
            // StartCoroutine(RoutineDailyMissionTimer());
            // StartCoroutine(RoutineAdCooldown());
        }
        
        
        
        /// <summary>
        /// 게임플레이 종료 후 씬 전환전 리프레시 처리 
        /// </summary>
        public void RefreshAfterGamePlay() {
            Debug.Log("### RefreshAfterGamePlay ###");
            
            StoryManager.main.RequestStoryList(OnRequestStoryList);
            StoryManager.main.RequestStoryInfo(SystemManager.main.givenStoryData);
        }
        
        void OnRequestStoryList(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            Debug.Log(">> OnRequestStoryList : " + response.DataAsText);
            
            // 작품 리스트 받아와서 스토리 매니저에게 전달. 
            StoryManager.main.SetStoryList(JsonMapper.ToObject(response.DataAsText));
        }




        /// <summary>
        /// 이전 연동된 계정 교체 처리 
        /// </summary>
        /// <param name="__previousGamebaseID"></param>
        public void ChangeAccountByGamebase(string __previousGamebaseID)
        {

            Debug.Log(string.Format("ChangeAccountByGamebase : [{0}]", __previousGamebaseID));

            JsonData sendingData = new JsonData(); // 서버 전송 데이터 
            sendingData["func"] = NetworkLoader.FUNC_CHANGE_ACCOUNT_GAMEBASE;
            sendingData["preGamebaseID"] = __previousGamebaseID; // 이전에 연동했던 게임베이스 ID 

            if (NetworkLoader.main == null)
                return;

            // 통신!
            NetworkLoader.main.SendPost(CallbackChangeAccount, sendingData);
        }


        /// <summary>
        /// 계정 연동을 통한 교체 콜백 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void CallbackChangeAccount(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackChangeAccount");
                return;
            }

            Debug.Log(string.Format("CallbackChangeAccount: {0}", res.DataAsText));
            JsonData result = JsonMapper.ToObject(res.DataAsText);
            if (result["account"][0]["is_done"].ToString() == "0")
            {
                // 여기 오면... 안되는데 ㅠㅠ
                // Force 하기전에 미리 체크해야 되나?
                Debug.LogError("LogError in CallbackChangeAccount");
                SystemManager.ShowSystemPopup("새로 로그인한 IDP 로그인 정보가 서버에 없습니다.", null, null, false, false);
                return;
            }

            SetRefreshUserInfo(result);

        }


        /// <summary>
        /// userkey및 기타 닉네임, 레벨 정도 설정 
        /// </summary>
        /// <param name="__accountInfo"></param>
        void SetUserInfo(JsonData __accountInfo)
        {
            userJson = SystemManager.GetJsonNode(__accountInfo, "account"); // 유저 json 

            userKey = SystemManager.GetJsonNodeString(userJson, CommonConst.COL_USERKEY);


            if (__accountInfo["tutorial"].Count > 0)
            {
                tutorialStep = SystemManager.GetJsonNodeInt(__accountInfo["tutorial"][0], NODE_TUTORIAL_STEP);
                tutorialClear = SystemManager.GetJsonNodeBool(__accountInfo["tutorial"][0], NODE_TUTORIAL_CLEAR);
            }

            isSelectionTutorialClear = SystemManager.GetJsonNodeBool(userJson, "tutorial_selection"); // 선택지 튜토리얼 
            isHowToPlayClear = SystemManager.GetJsonNodeBool(userJson, "how_to_play"); // 하우 투 플레이 튜토리얼 

            accountLink = SystemManager.GetJsonNodeString(userJson, "account_link");
            ViewCommonTop.OnRefreshAccountLink?.Invoke(); // 상단 갱신 (계정연동 보상때문에)

            nickname = SystemManager.GetJsonNodeString(userJson, "nickname");

            // 슈퍼유저 처리 
            isAdminUser = SystemManager.GetJsonNodeBool(userJson, "admin");

            // 인트로 완료 여부
            isIntroDone = SystemManager.GetJsonNodeBool(userJson, "intro_done");

            // 올패스 만료 일시에 대한  처리 2022.05.23
            SetAllpassExpire(SystemManager.GetJsonNodeLong(userJson, "allpass_expire_tick"));
            
            // Rate 평가 팝업 관련 변수 추가 2022.06
            diffRate = SystemManager.GetJsonNodeInt(userJson, "diff_rate");
            rateResult = SystemManager.GetJsonNodeInt(userJson, "rate_result");

            // 이프유 패스 관련 변수 추가 2022.07.18
            ifyouPassDay = SystemManager.GetJsonNodeInt(userJson, "ifyou_pass_day");
        }

        /// <summary>
        /// 올 패스 만료시간 세팅 
        /// </summary>
        /// <param name="__expireTick"></param>
        public void SetAllpassExpire(long __expireTick)
        {
            allpassExpireTick = SystemConst.ConvertServerTimeTick(__expireTick);
            allpassExpireDate = new DateTime(allpassExpireTick);
        }


        /// <summary>
        /// 갱신된 유저 정보 설정. 
        /// </summary>
        /// <param name="__j"></param>
        public void SetRefreshUserInfo(JsonData __j)
        {
            // 소모성 재화 정보 update
            SetBankInfo(__j);

            // 알림 정보 update
            SetNotificationInfo(__j);

            SetUserInfo(__j);

            // 사용자 UI 정보를 갱신하는 Event 필요함!

            // 다했다고 안내
            SystemManager.ShowSystemPopupLocalize("6112", null, null, true, false);
        }

        public void SetRefreshInfo(JsonData __j)
        {
            notReceivedMailJson = __j;

            // 소모성 재화 정보 update
            SetBankInfo(__j);

            // 알림 정보 update
            SetNotificationInfo(__j);
        }


        /// <summary>
        /// 선택한 스토리에서 유저 데이터 분리해서 저장 
        /// </summary>
        /// <param name="__j"></param>
        public void SetStoryUserData(JsonData __j)
        {
            currentStoryJson = __j;

            #region 미션 
            currentStoryMissionJSON = GetNodeProjectMissions();
            DictStoryMission = new Dictionary<int, MissionData>();

            // 딕셔너리에 정리하기. JSON 쓰기 시러요..!
            for (int i = 0; i < currentStoryMissionJSON.Count; i++)
            {
                MissionData missionData = new MissionData(currentStoryMissionJSON[i]);
                DictStoryMission.Add(missionData.missionID, missionData);
            }

            #endregion

            // 능력치            
            SetStoryAbilityDictionary(currentStoryJson[NODE_USER_ABILITY]);

            // 선택지 히스토리 
            currentStorySelectionHistoryJson = currentStoryJson["selectionHistory"];

            /// 데이터 확인용도 
            if (!Application.isEditor)
                return;



        }

        #region 작품 능력치 

        /// <summary>
        /// 스토리 능력치 Dictionary
        /// </summary>
        /// <param name="__abilityData"></param>
        public void SetStoryAbilityDictionary(JsonData __abilityData)
        {
            currentStoryAbilityJson = __abilityData;
            DictStoryAbility = new Dictionary<string, List<AbilityData>>();

            foreach (string key in currentStoryAbilityJson.Keys)
            {
                List<AbilityData> abilityDatas = new List<AbilityData>();

                for (int i = 0; i < currentStoryAbilityJson[key].Count; i++)
                {
                    AbilityData abilityData = new AbilityData(currentStoryAbilityJson[key][i]);
                    abilityDatas.Add(abilityData);
                }

                DictStoryAbility.Add(key, abilityDatas);
            }
        }

        /// <summary>
        /// 코인샵 진입전에 호출해주기 
        /// </summary>
        public void SaveCurrentAbilityDictionary()
        {
            // 똑같은 딕셔너리 생성 
            DictOldStoryAbility = new Dictionary<string, List<AbilityData>>();
            foreach (string key in currentStoryAbilityJson.Keys)
            {
                List<AbilityData> abilityDatas = new List<AbilityData>();

                for (int i = 0; i < currentStoryAbilityJson[key].Count; i++)
                {
                    AbilityData abilityData = new AbilityData(currentStoryAbilityJson[key][i]);
                    abilityDatas.Add(abilityData);
                }

                DictOldStoryAbility.Add(key, abilityDatas);
            }
        }

        /// <summary>
        /// 이전과 비교해서 달라진 능력치 메세지 띄워주기 
        /// </summary>
        public void NotifyUpdatedAbility()
        {

            float gap = 0;

            // old와 비교해야한다. 
            // key : 화자
            foreach (string key in DictStoryAbility.Keys)
            {

                Debug.Log(string.Format("[{0}] : old[{1}] / current[{2}]", key, DictOldStoryAbility[key].Count, DictStoryAbility[key].Count));

                for (int i = 0; i < DictStoryAbility[key].Count; i++)
                { // 화자별로 능력치 훑는다. 

                    // 값이 똑같으면, 일없다. 
                    if (DictOldStoryAbility[key][i].currentValue == DictStoryAbility[key][i].currentValue)
                        continue;

                    // 갭은 현재 능력치 - 이전 능력치다. 
                    gap = DictStoryAbility[key][i].currentValue - DictOldStoryAbility[key][i].currentValue;

                    Debug.Log(string.Format("[{0}]/[{1}]/[{2}]", key, DictStoryAbility[key][i].originAbilityName, gap));

                    // 능력치 팝업 호출하자. 
                    SystemManager.ShowAbilityPopup(key, DictStoryAbility[key][i].originAbilityName, Mathf.RoundToInt(gap));
                }
            }
        }

        /// <summary>
        /// 능력치를 찾아줘
        /// </summary>
        /// <param name="__name">캐릭터 이름(=speaker)</param>
        /// <param name="__ability">능력치 이름(한글 오리지널 이름)</param>
        /// <returns></returns>
        public AbilityData GetAbilityData(string __name, string __ability)
        {
            if (!DictStoryAbility.ContainsKey(__name))
            {
                Debug.LogError(__name + ", 이 캐릭터는 능력치 데이터에 없는걸");
                return null;
            }

            // 해당 캐릭터의 능력치 정보를 찾아서 return
            for (int i = 0; i < DictStoryAbility[__name].Count; i++)
            {
                if (DictStoryAbility[__name][i].originAbilityName == __ability)
                    return DictStoryAbility[__name][i];
            }

            Debug.LogError(string.Format("{0}, 이 캐릭터는 {1} 능력치가 존재하지 않습니다", __name, __ability));
            return null;
        }




        #endregion



        #region 선택지 관련

        /// <summary>
        /// 선택지 구매
        /// </summary>
        public void PurchaseSelection(int __selectionGroup, int __selectionNo, int __price, OnRequestFinishedDelegate __cb)
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "purchaseSelection";
            sending[CommonConst.COL_USERKEY] = userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            sending[CommonConst.COL_EPISODE_ID] = StoryManager.main.CurrentEpisodeID;
            sending[GameConst.COL_SELECTION_GROUP] = __selectionGroup;
            sending[GameConst.COL_SELECTION_NO] = __selectionNo;
            sending["price"] = __price;


            NetworkLoader.main.SendPost(__cb, sending, true);
        }

        /// <summary>
        /// 선택지 구매 목록 갱신
        /// </summary>
        /// <param name="__data"></param>
        public void SetPurchaseSelection(JsonData __data)
        {
            // 여기 Exception 날 수 있음
            currentStoryJson[NODE_SELECTION_PURCHASE][StoryManager.main.CurrentEpisodeID] = __data["list"];
        }


        /// <summary>
        /// 해당 에피소드의 선택한 선택지를 구매한적 있는지 체크체크
        /// </summary>
        /// <param name="__episodeID"></param>
        /// <returns></returns>
        public bool IsPurchaseSelection(string __episodeID, int __selectionGroup, int __selectionNo)
        {
            // key값이 없으면 구매한 적이 없는 에피소드
            if (!currentStoryJson[NODE_SELECTION_PURCHASE].ContainsKey(__episodeID))
                return false;

            JsonData selectionPurchaseData = currentStoryJson[NODE_SELECTION_PURCHASE][__episodeID];

            for (int i = 0; i < selectionPurchaseData.Count; i++)
            {
                // 선택지 그룹이 같은게 아니면 넘겨넘겨
                if (SystemManager.GetJsonNodeInt(selectionPurchaseData[i], GameConst.COL_SELECTION_GROUP) != __selectionGroup)
                    continue;

                // 같은 선택지 그룹 내에서 같은 번호면 구매한적이 있으니 true 반환
                if (SystemManager.GetJsonNodeInt(selectionPurchaseData[i], GameConst.COL_SELECTION_NO) == __selectionNo)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 선택지 힌트 요청 및 구매
        /// </summary>
        public void RequestSelectionHint(int __selectionGroup, int __selectionNo, OnRequestFinishedDelegate __cb)
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestSelectionHint";
            sending[CommonConst.COL_USERKEY] = userKey;
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            sending[CommonConst.COL_EPISODE_ID] = StoryManager.main.CurrentEpisodeID;
            sending[GameConst.COL_SELECTION_GROUP] = __selectionGroup;
            sending[GameConst.COL_SELECTION_NO] = __selectionNo;

            NetworkLoader.main.SendPost(__cb, sending, true);
        }

        /// <summary>
        /// 선택지 힌트 구매 목록 갱신
        /// </summary>
        public void SetSelectionHint(JsonData __j)
        {
            if (!__j.ContainsKey(NODE_SELECTION_HINT_PURCHASE))
            {
                Debug.LogError("선택지 힌트 노드가 존재하지 않음!");
                return;
            }

            currentStoryJson[NODE_SELECTION_HINT_PURCHASE] = __j[NODE_SELECTION_HINT_PURCHASE];
        }

        /// <summary>
        /// 구매한적 있는 선택지 힌트인가요?
        /// </summary>
        /// <returns></returns>
        public bool IsPurchaseSelectionHint(string __episodeId, int __selectionGroup, int __selectionNo)
        {
            // 해당 에피소드 key가 존재하지 않으면 구매한적 없음
            if (!currentStoryJson[NODE_SELECTION_HINT_PURCHASE].ContainsKey(__episodeId))
                return false;

            JsonData selectionHintData = currentStoryJson[NODE_SELECTION_HINT_PURCHASE][__episodeId];

            for (int i = 0; i < selectionHintData.Count; i++)
            {
                if (SystemManager.GetJsonNodeInt(selectionHintData[i], GameConst.COL_SELECTION_GROUP) != __selectionGroup)
                    continue;

                if (SystemManager.GetJsonNodeInt(selectionHintData[i], GameConst.COL_SELECTION_NO) == __selectionNo)
                    return true;
            }

            return false;
        }


        #endregion



        #region 작품 미션



        /// <summary>
        /// 미션 이름을 비교하여 mission Id를 반환한다
        /// </summary>
        public MissionData GetMissionData(string missionName)
        {
            // origin name을 체크하도록 변경
            foreach (MissionData missionData in DictStoryMission.Values)
            {
                if (missionData.originName == missionName)
                    return missionData;
            }

            return null;
        }

        public void SetMissionData(int __missionID, MissionData __data)
        {
            if (DictStoryMission.ContainsKey(__missionID))
            {
                DictStoryMission[__missionID] = __data;

            }
        }

        #endregion

        /// <summary>
        /// 플레이 미완된 엔딩 갯수
        /// </summary>
        /// <returns></returns>
        public int GetInCompleteEndingCount()
        {
            int incompleteCount = 0;

            foreach (EpisodeData endingData in StoryManager.main.ListCurrentProjectEpisodes)
            {
                // 엔딩이고, 열렸는데 플레이 안한경우
                if (endingData.episodeType == EpisodeType.Ending && endingData.endingOpen && !IsCompleteEpisode(endingData.episodeID))
                    incompleteCount++;
            }

            return incompleteCount;
        }

        /// <summary>
        /// 해당 화 플레이 완료 기록이 있나요?
        /// </summary>
        /// <param name="__episodeId"></param>
        /// <returns></returns>
        public bool IsCompleteEpisode(string __episodeId)
        {
            for (int i = 0; i < GetNodeUserEpisodeHistory().Count; i++)
            {
                if (GetNodeUserEpisodeHistory()[i].ToString() == __episodeId)
                    return true;
            }

            return false;
        }



        #region 튜토리얼 Update 통신

        /// <summary>
        /// 튜토리얼 단계 업데이트 함수
        /// </summary>
        /// <param name="__cb">튜토리얼 업데이트 이후 추가로 callback할 함수</param>
        public void UpdateTutorialStep(int __step, int __isClear, OnRequestFinishedDelegate __cb)
        {
            JsonData sendingData = new JsonData();
            sendingData[CommonConst.FUNC] = "requestUserTutorialProgress";
            sendingData[CommonConst.COL_USERKEY] = userKey;
            sendingData["step"] = __step;
            sendingData["is_clear"] = __isClear;

            OnRequestFinishedDelegate callback = CallbackTutorialUpdate + __cb;

            NetworkLoader.main.SendPost(callback, sendingData, true);
        }


        void CallbackTutorialUpdate(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackTutorialUpdate");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);
            tutorialStep = SystemManager.GetJsonNodeInt(result[NODE_TUTORIAL_CURRENT][0], NODE_TUTORIAL_STEP);
            tutorialClear = SystemManager.GetJsonNodeBool(result[NODE_TUTORIAL_CURRENT][0], NODE_TUTORIAL_CLEAR);

            Debug.Log(JsonMapper.ToStringUnicode(result));

            // 튜토리얼 완료라면 보상 갱신
            if (SystemManager.GetJsonNodeBool(result[NODE_TUTORIAL_CURRENT][0], "tutorial_clear"))
            {
                if (tutorialStep != 2)
                    SetBankInfo(result);
            }
        }


        /// <summary>
        /// 첫번째 선택지 선택시 호출한다. 선택지 튜토리얼 관련 처리 
        /// </summary>
        public void RequestSelectionTutorialClear()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "updateTutorialSelection";
            sending[CommonConst.COL_USERKEY] = userKey;

            NetworkLoader.main.SendPost(CallbackTutorialSelection, sending);
        }

        void CallbackTutorialSelection(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackTutorialSelection");
                return;
            }

            isSelectionTutorialClear = true; // 따로 데이터 받는거 없이 true 처리 
        }


        /// <summary>
        /// How to play 튜토리얼 클리어 
        /// </summary>
        public void RequestHowToPlayTutorialClear()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "updateTutorialHowToPlay";
            sending[CommonConst.COL_USERKEY] = userKey;

            NetworkLoader.main.SendPost(CallbackTutorialHowToPlay, sending, true);
        }

        void CallbackTutorialHowToPlay(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackTutorialSelection");
                return;
            }

            isHowToPlayClear = true; // 따로 데이터 받는거 없이 true 처리

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            // resource 팝업창 
            string currency = SystemManager.GetJsonNodeString(result["got"], "currency");
            int quantity = SystemManager.GetJsonNodeInt(result["got"], "quantity");

            Debug.Log(string.Format("CallbackTutorialHowToPlay [{0}]/[{1}]", currency, quantity));

            // 재화 획득 팝업
            SystemManager.ShowResourcePopup(SystemManager.GetLocalizedText("6202"), quantity, SystemManager.main.GetCurrencyImageURL(currency), SystemManager.main.GetCurrencyImageKey(currency));

            SetBankInfo(result);

            HowToPlayFloating.RefreshHowToPlayState?.Invoke(); // 플로팅 버튼 리프레시 
        }



        #endregion

        #region 메일(우편함)

        /// <summary>
        /// 미수신 우편 리스트 호출
        /// </summary>
        public void CallbackNotRecievedMail(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackNotRecievedMail");
                return;
            }

            // 미수신 mail json
            notReceivedMailJson = JsonMapper.ToObject(res.DataAsText);
            unreadMailCount = SystemManager.GetJsonNodeInt(notReceivedMailJson, UN_UNREAD_MAIL_COUNT);
            SetRefreshInfo(notReceivedMailJson);
        }

        #endregion

        #region 미션(보상)

        /// <summary>
        /// 개별 미션 보상 받기
        /// </summary>
        public void GetMissionRewared(MissionData missionData, OnRequestFinishedDelegate callback)
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = NetworkLoader.FUNC_USER_MISSION_REWARD;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            sending["mission_id"] = missionData.missionID;
            sending[CommonConst.COL_USERKEY] = userKey;
            //sending["reward_currency"] = missionData.rewardCurrency;
            //sending["reward_quantity"] = missionData.rewardQuantity;

            NetworkLoader.main.SendPost(callback, sending, true);
        }

        /// <summary>
        /// 해금되고 보상은 받지않은 미션 개수 가져오기 
        /// </summary>
        /// <returns></returns>        
        public int GetUnlockStateMissionCount()
        {

            if (DictStoryMission == null)
                return 0;

            return DictStoryMission.Values.Count(mission => mission.missionState == MissionState.unlocked);
        }

        #endregion

        #region 유저 소모성 재화 제어, 알림 정보 제어 

        /// <summary>
        /// 코인 표시기 추가 
        /// </summary>
        /// <param name="__receiver"></param>
        public void AddCoinIndicator(CoinIndicator __receiver)
        {
            if (ListCoinIndicators.Contains(__receiver))
                return;

            ListCoinIndicators.Add(__receiver);
            RefreshCoinIndicators(); // 방금 들어온 친구도 갱신되도록 처리
        }

        /// <summary>
        /// 사파이어 표시기 추가 
        /// </summary>
        /// <param name="__receiver"></param>
        public void AddGemIndicator(GemIndicator __receiver)
        {
            if (ListGemIndicators.Contains(__receiver))
                return;

            ListGemIndicators.Add(__receiver);
            RefreshGemIndicators();
        }


        /// <summary>
        /// 소모성 재화 세팅 
        /// </summary>
        /// <param name="__j"></param>
        public void SetBankInfo(JsonData __j, bool __refresh = true)
        {
            if (!__j.ContainsKey("bank"))
            {
                Debug.LogError("No Bank Node");
                return;
            }

            Debug.Log("Refresh BankInfo");

            bankJson = __j["bank"];
            if (bankJson == null)
            {
                Debug.LogError("bank is null...!!");
                return;
            }

            debugBankString = JsonMapper.ToStringUnicode(bankJson);

            // 재화 값 갱신
            gem = int.Parse(bankJson[LobbyConst.GEM].ToString());
            coin = int.Parse(bankJson[LobbyConst.COIN].ToString());

            // 상단 리프레시
            if (!__refresh)
                return;

            // 붙어있는 표시기들 refresh 처리 
            RefreshCoinIndicators();
            RefreshGemIndicators();
        }


        /// <summary>
        /// 상단 리프레시만. 
        /// </summary>
        public void RefreshIndicators()
        {
            RefreshCoinIndicators();
            RefreshGemIndicators();
        }


        /// <summary>
        /// 확인이 필요한 유저 정보 세팅
        /// </summary>
        /// <param name="__j"></param>
        public void SetNotificationInfo(JsonData __j)
        {
            // 미수신 메일 개수 
            if (__j.ContainsKey(UN_UNREAD_MAIL_COUNT))
            {
                unreadMailCount = SystemManager.GetJsonNodeInt(__j, UN_UNREAD_MAIL_COUNT);
                // 등록된 Action을 통해서 갱신하도록 해줍니다!
                OnRefreshUnreadMailCount?.Invoke(unreadMailCount);
            }
        }


        /// <summary>
        /// 코인 표시기 갱신!
        /// </summary>
        void RefreshCoinIndicators()
        {
            // 씬이 전환되면서 null이 쌓이기 때문에 한번 정리하고 해야한다.
            for (int i = ListCoinIndicators.Count - 1; i >= 0; i--)
            {
                if (!ListCoinIndicators[i])
                    ListCoinIndicators.RemoveAt(i);
            }

            // 코인 
            for (int i = 0; i < ListCoinIndicators.Count; i++)
            {
                ListCoinIndicators[i].RefreshCoin(coin);
            }
        }

        /// <summary>
        /// 사파이어 표시기 갱신!
        /// </summary>
        void RefreshGemIndicators()
        {

            // 씬이 전환되면서 null이 쌓이기 때문에 한번 정리하고 해야한다.
            for (int i = ListGemIndicators.Count - 1; i >= 0; i--)
            {
                if (!ListGemIndicators[i])
                    ListGemIndicators.RemoveAt(i);
            }


            // 사파이어!
            for (int i = 0; i < ListGemIndicators.Count; i++)
            {
                ListGemIndicators[i].RefreshGem(gem);
            }
        }


        /// <summary>
        /// 필요한 가격만큼 보석을 보유하고 있는지 체크 
        /// </summary>
        /// <param name="__requirePrice"></param>
        /// <returns></returns>
        public bool CheckGemProperty(int __requirePrice)
        {
            /*
            if(CheckAdminUser())
                return true;
            */

            return __requirePrice <= gem;
        }

        /// <summary>
        /// 필요한 가격만큼 코인을 보유하고 있는지 체크 
        /// </summary>
        /// <param name="__requirePrice"></param>
        /// <returns></returns>
        public bool CheckCoinProperty(int __requirePrice)
        {
            /*
            if(CheckAdminUser())
                return true;
            */

            return __requirePrice <= coin;
        }


        #endregion

        #region 유저 노드 제어 

        /// <summary>
        /// 계정 연동되어있는지 체크 
        /// </summary>
        /// <returns>true : 연동됨</returns>
        public bool CheckAccountLink()
        {
            if (UserManager.main == null)
                return false;

            if (UserManager.main.accountLink == "-")
                return false;

            return true;
        }

        /// <summary>
        /// 유저 계정 노드 
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserAccountJSON()
        {
            return userJson;
        }

        /// <summary>
        /// 닉네임 
        /// </summary>
        /// <returns></returns>
        public string GetUserNickname()
        {
            return userJson["nickname"].ToString();
        }

        /// <summary>
        /// 유저 핀코드 
        /// </summary>
        /// <returns></returns>
        public string GetUserPinCode()
        {
            if (!userJson.ContainsKey("pincode"))
                return string.Empty;

            return userJson["pincode"].ToString();
        }

        public void SetAdminUser()
        {
            SystemManager.ShowSimpleAlert("슈퍼유저로 변경되었습니다.");
            isAdminUser = true;
            ViewCommonTop.OnRefreshSuperUser?.Invoke();
        }

        /// <summary>
        /// 어드민 유저 체크 
        /// </summary>
        /// <returns>true : 어드민 유저임, false : 일반 유저임</returns>
        public bool CheckAdminUser()
        {
            return isAdminUser;
        }


        /// <summary>
        /// 프로젝트 자유이용권 소유자인가요?
        /// </summary>
        /// <returns></returns>
        public bool HasProjectFreepass()
        {
            // * 현재 진입한 작품기준으로 검색한다. 

            // 올패스 사용여부를 먼저 체크한다 (2022.05.23)
            allpassTimeDiff = allpassExpireDate - System.DateTime.UtcNow;
            if (allpassTimeDiff.Ticks > 0)
            {
                return true;
            }
            
            return StoryManager.main.CurrentProject.hasPremiumPass;

        }

        public bool HasProjectFreepass(string __targetProjectID)
        {
            if(string.IsNullOrEmpty(__targetProjectID)) {
                Debug.LogError("Null parameter inHasProjectFreepass ");
                return false;
            }
            
            // 올패스 사용여부를 먼저 체크한다 (2022.05.23)
            allpassTimeDiff = allpassExpireDate - System.DateTime.UtcNow;
            if (allpassTimeDiff.Ticks > 0)
            {
                return true;
            }
            
            try {
                return StoryManager.main.FindProject(__targetProjectID).hasPremiumPass;
            }
            catch {
                Debug.LogError("Can't find project in HasProjectFreePass : " + __targetProjectID );
                return false;
            }
        }

        /// <summary>
        /// 타겟 프로젝트의 프리미엄 패스 보유 여부 
        /// </summary>
        /// <param name="__targetProjectID"></param>
        /// <returns></returns>        
        public bool HasProjectPremiumPassOnly(string __targetProjectID)
        {
            if(string.IsNullOrEmpty(__targetProjectID)) {
                Debug.LogError("Null parameter HasProjectPremiumPassOnly ");
                return false;
            }
            
            try {
                return StoryManager.main.FindProject(__targetProjectID).hasPremiumPass;
            }
            catch {
                Debug.LogError("Can't find project in HasProjectFreePass : " + __targetProjectID );
                return false;
            }
        }


        /// <summary>
        /// 올패스 유효시간 차 주세요 
        /// </summary>
        /// <returns></returns>
        public string GetAllPassTimeDiff()
        {
            allpassTimeDiff = allpassExpireDate - System.DateTime.UtcNow;

            if (allpassTimeDiff.Ticks <= 0)
            {
                return string.Empty;
            }

            if (allpassTimeDiff.TotalHours < 24)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", allpassTimeDiff.Hours, allpassTimeDiff.Minutes, allpassTimeDiff.Seconds);
            }
            else
            {
                return string.Format("{0}d {1:D2}:{2:D2}:{3:D2}", allpassTimeDiff.Days, allpassTimeDiff.Hours, allpassTimeDiff.Minutes, allpassTimeDiff.Seconds);
            }


        }



        /// <summary>
        /// 에피소드 구매 기록 노드 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeEpisodePurchaseHistory()
        {
            return currentStoryJson[NODE_PURCHASE_HIST];
        }

        public void SetNodeEpisodePurchaseHistory(JsonData __j)
        {
            currentStoryJson[NODE_PURCHASE_HIST] = __j;
        }

        /// <summary>
        /// 진행중인 사건 정보 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeStorySceneProgress()
        {
            return currentStoryJson[NODE_SCENE_PROGRESS];
        }

        public void SetNodeStorySceneProgress(JsonData __j)
        {
            currentStoryJson[NODE_SCENE_PROGRESS] = __j;
        }
        
        /// <summary>
        /// SceneProgress 입력하기 
        /// </summary>
        /// <param name="__sceneID"></param>
        public void AddSceneProgress(string __sceneID) {
            
            if(currentStoryJson == null || !currentStoryJson.ContainsKey(NODE_SCENE_PROGRESS))
                return;
            
            
            if(CheckSceneProgress(__sceneID))
                return;
                
            currentStoryJson[NODE_SCENE_PROGRESS].Add(__sceneID);
            Debug.Log(JsonMapper.ToJson(currentStoryJson[NODE_SCENE_PROGRESS]));
        }
        

        /// <summary>
        /// 에피소드 플레이 저장 기록 jsonData
        /// </summary>
        public JsonData GetResultEpisodeRecord()
        {
            return resultEpisodeRecord;
        }

        /// <summary>
        /// 에피소드 최초 클리어 보상
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeFirstClearResult()
        {
            return resultEpisodeRecord[NODE_FIRST_CLEAR_RESULT];
        }
        
        
        
        
        /// <summary>
        /// Drop Mission에 대한 처리 
        /// </summary>
        /// <param name="__missionName"></param>
        public void CompleteMissionByDrop(string __missionName) {
            MissionData missionData = GetMissionData(__missionName);
            
            
            // 데이터 없음 
            if(missionData == null || missionData.missionID <= 0) {
                return;
            }
            
            // 이미 해금된 상태
            if(missionData.missionState != MissionState.locked) {
                return;
            }
            
            ShowMissionUnlockPopup(missionData); // 미션 팝업 호출 
        }



        /// <summary>
        /// 에피소드 클리어 후 미션 해금 체크 
        /// </summary>
        /// <param name="__useQueue"></param>
        public void CompleteMissionByEpisode(string __episodeID)
        {
            foreach(int key in DictStoryMission.Keys) {
                
                // 해금되지 않은 미션 정보 조회 
                if(DictStoryMission[key].missionState != MissionState.locked)
                    continue;
                    
                // scene type만 
                if(DictStoryMission[key].missionType != MissionType.episode)
                    continue;
                
                // 조건 체크 
                // 대상씬이 조건에 있는 경우만 진행한다.
                if(!DictStoryMission[key].CheckExistsCondition(__episodeID))
                    continue;
                    

                // 유저 기록과 대조해서 체크 시작 
                if(DictStoryMission[key].CheckUserHist()) {
                    // 해금조건이 맞다! 
                    // 팝업을 오픈하자!
                    ShowMissionUnlockPopup(DictStoryMission[key]);
                }
                
                    
            } //  ? end of foreach
        }
        
        /// <summary>
        /// 씬 정보 업데이트 후 미션 해금 체크 
        /// </summary>
        /// <param name="__useQueue"></param>
        public void CompleteMissionByScene(string __sceneID) {
            foreach(int key in DictStoryMission.Keys) {
                
                // 해금되지 않은 미션 정보 조회 
                if(DictStoryMission[key].missionState != MissionState.locked)
                    continue;
                    
                // scene type만 
                if(DictStoryMission[key].missionType != MissionType.scene)
                    continue;
                
                // 조건 체크 
                // 대상씬이 조건에 있는 경우만 진행한다.
                if(!DictStoryMission[key].CheckExistsCondition(__sceneID))
                    continue;
                    

                // 유저 기록과 대조해서 체크 시작 
                if(DictStoryMission[key].CheckUserHist()) {
                    // 해금조건이 맞다! 
                    // 팝업을 오픈하자!
                    ShowMissionUnlockPopup(DictStoryMission[key]);
                }
                
                    
            } //  ? end of foreach
        }
        
        
        /// <summary>
        /// 작품 진입시에 미해금된 미션 체크 
        /// </summary>
        public void CheckUnlockedMission() {
            
            bool hasNewMission = false;
            
            // 에피소드 해금과 씬 해금을 모두 체크해야한다. 
            if(DictStoryMission == null)
                return;
                
            
            foreach(int key in DictStoryMission.Keys) {
                if(DictStoryMission[key].missionState != MissionState.locked)
                    continue;
                    
                // 에피소드와 씬 타입만 체크한다. 
                if(DictStoryMission[key].missionType == MissionType.episode || DictStoryMission[key].missionType == MissionType.scene) {
                    
                    // 조건에 충족하는 경우 
                    if(DictStoryMission[key].CheckUserHist()) {
                        // 대상 미션 오픈 처리 
                        ShowMissionUnlockPopup(DictStoryMission[key], false);
                        hasNewMission = true;
                    }
                    
                }
                    
            }
            
            if(hasNewMission) {
                StoryLobbyMain.OnInitializeContentGroup?.Invoke();
            }
            
        }
        
        
        /// <summary>
        /// 미션 해금 팝업 오픈 
        /// </summary>
        /// <param name="__missionData"></param>
        public void ShowMissionUnlockPopup(MissionData __missionData, bool __needPopup = true) {
            
            if(!DictStoryMission.ContainsKey(__missionData.missionID)) {
                Debug.LogError("ShowMissionUnlockPopup No mission data : " + __missionData.missionName);
                return;
            }            
            
            
            // 갱신 
            DictStoryMission[__missionData.missionID].missionState = MissionState.unlocked;
            
            
            // 프로젝트 클리어 했는지 체크
            if (ProjectAllClear())
                NetworkLoader.main.RequestIFYOUAchievement(8, int.Parse(StoryManager.main.CurrentProjectID));            
            
            NetworkLoader.main.RequestUnlockMission(__missionData);
            
            
            Debug.Log("Mission Clear :: "+ __missionData.missionName);
            
            if(!__needPopup) {
                return;
            }
            
            
            // 게임 화면에서 미션팝업을 꺼뒀으면 여길 타지말자
            if (GameManager.main != null && PlayerPrefs.GetInt(GameConst.MISSION_POPUP) != 1) {
                Debug.Log("OFF mission popup");
                return;
            }
           
            PopupBase popUp = PopupManager.main.GetPopup(GameConst.POPUP_ACHIEVEMENT_ILLUST);
            if (popUp == null)
            {
                Debug.LogError("No AchieveMission Popup");
                return;
            }
            
            
            popUp.Data.SetImagesSprites(spriteMissionPopup);
            popUp.Data.SetLabelsTexts(string.Format(SystemManager.GetLocalizedText("5086"), __missionData.missionName));
            PopupManager.main.ShowPopup(popUp, true, false);
            

        }
        
        
        
        
        
        
        /// <summary>
        /// 사건을 통해 열리는 스페셜 에피소드 처리 
        /// </summary>
        /// <param name="__sceneID"></param>
        public void ShowCompleteSpecialEpisodeByScene(string __sceneID) {
            for(int i=0; i<StoryManager.main.SideEpisodeList.Count;i++) {
                if(StoryManager.main.SideEpisodeList[i].isUnlock)
                    continue;
                    
                if(StoryManager.main.SideEpisodeList[i].unlockStyle != "event")
                    continue;
                    
                if(!StoryManager.main.SideEpisodeList[i].CheckExistsUnlockCondition(__sceneID))
                    continue;
                    
                if(StoryManager.main.SideEpisodeList[i].CheckUserHist()) {
                    ShowSpecialEpisodeUnlockPopup(StoryManager.main.SideEpisodeList[i]);
                }
            }
        }
        
        
        /// <summary>
        /// 에피소드를 통해 열리는 스페셜 에피소드 처리
        /// </summary>
        /// <param name="__episodeID"></param>
        public void ShowCompleteSpecialEpisodeByEpisode(string __episodeID) {
            for(int i=0; i<StoryManager.main.SideEpisodeList.Count;i++) {
                if(StoryManager.main.SideEpisodeList[i].isUnlock)
                    continue;
                    
                if(StoryManager.main.SideEpisodeList[i].unlockStyle != "episode")
                    continue;
                    
                if(!StoryManager.main.SideEpisodeList[i].CheckExistsUnlockCondition(__episodeID))
                    continue;
                    
                if(StoryManager.main.SideEpisodeList[i].CheckUserHist()) {
                    ShowSpecialEpisodeUnlockPopup(StoryManager.main.SideEpisodeList[i]);
                }
            }
        }
        
        
        /// <summary>
        /// 스페셜 에피소드 해금 팝업 
        /// </summary>
        /// <param name="__special"></param>
        public void ShowSpecialEpisodeUnlockPopup(EpisodeData __special) {
            
            // 통신 처리
            NetworkLoader.main.RequestUnlockSpecialEpisode(__special);
            
            
            PopupBase unlockPopup = PopupManager.main.GetPopup(GameConst.POPUP_SIDE_ALERT);
            
            unlockPopup.Data.SetLabelsTexts(SystemManager.GetLocalizedText("6234"), __special.episodeTitle);
            unlockPopup.Data.imageURL = __special.popupImageURL;
            unlockPopup.Data.imageKey = __special.popupImageKey;
            __special.isUnlock = true;
            PopupManager.main.ShowPopup(unlockPopup, true, false);            
        }
        




        /// <summary>
        /// 유저 프로젝트별 사건 경험 히스토리
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeProjectSceneHistory()
        {
            if (currentStoryJson.ContainsKey(NODE_SCENE_HISTORY))
                return currentStoryJson[NODE_SCENE_HISTORY];

            else return null;
        }

        /// <summary>
        /// 유저 프로젝트별 사건 경험 히스토리
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeProjectSceneHistory(JsonData __j)
        {
            currentStoryJson[NODE_SCENE_HISTORY] = __j;
        }
        
        // sceneID를 작품 scene hist 에 입력하기 
        public void AddUserProjectSceneHist(string __sceneID) {
            if(currentStoryJson == null || !currentStoryJson.ContainsKey(NODE_SCENE_HISTORY))
                return;
                
            // 없으면 입력하기 
            if(!CheckSceneHistory(__sceneID))
                currentStoryJson[NODE_SCENE_HISTORY].Add(__sceneID);
        }



        public void SetProjectMissionAllClear(int __v)
        {
            currentStoryJson["missionAllClear"] = __v;
        }

        /// <summary>
        /// 미션 올클리어 체크
        /// </summary>
        /// <returns>true = 미션 올클리어 보상 받음, false = 미션 올클리어 보상 아직 받지 못함</returns>
        public bool GetProjectMissionAllClear()
        {
            return SystemManager.GetJsonNodeBool(currentStoryJson, "missionAllClear");
        }



        /// <summary>
        /// 프로젝트 말풍선 세트 정보
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeBubbleSet()
        {
            return currentStoryJson[NODE_BUBBLE_SET];
        }

        /// <summary>
        /// 프로젝트 말풍선 스프라이트 정보
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeBubbleSprite()
        {
            return currentStoryJson[NODE_BUBBLE_SPRITE];
        }



        /// <summary>
        /// 프로젝트 도전과제 정보
        /// </summary>
        JsonData GetNodeProjectMissions()
        {
            return currentStoryJson[NODE_PROJECT_MISSIONS];
        }


        /// <summary>
        /// 유저 갤러리 이미지 리스트 및 오픈 기록  
        /// 일반, 라이브 페어 시스템 적용
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserGalleryImage()
        {
            if (currentStoryJson == null || !currentStoryJson.ContainsKey(NODE_USER_GALLERY_IMAGES))
                return null;

            return currentStoryJson[NODE_USER_GALLERY_IMAGES];
        }


        /// <summary>
        /// 유저 갤러리 이미지 목록 및 해금 정보 업데이트 
        /// </summary>
        /// <param name="__newData"></param>
        public void SetNodeUserGalleryImages(JsonData __newData)
        {
            currentStoryJson[NODE_USER_GALLERY_IMAGES] = __newData;
        }



        /// <summary>
        /// 유저 에피소드 진행도 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeUserEpisodeProgress()
        {
            return currentStoryJson[NODE_EPISODE_PROGRESS];
        }

        /// <summary>
        /// 에피소드 진행도에 단일 에피소드 추가 
        /// </summary>
        /// <param name="__playEpisodeID"></param>
        public void AddUserEpisodeProgress(int __playEpisodeID)
        {
            GetNodeUserEpisodeProgress().Add(__playEpisodeID);
        }

        /// <summary>
        /// 유저 에피소드 진행도
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeUserEpisodeProgress(JsonData __j)
        {
            currentStoryJson[NODE_EPISODE_PROGRESS] = __j;

            string.Format("UserEpisodeProgress [{0}]", JsonMapper.ToStringUnicode(__j));
        }

        /// <summary>
        /// 해당 작품의 배경음악 list
        /// </summary>
        public JsonData GetNodeUserBgm()
        {
            return currentStoryJson["bgms"];
        }

        /// <summary>
        /// 공개 보이스(더빙) list
        /// </summary>
        public JsonData GetNodeUserVoiceHistory()
        {
            return currentStoryJson[NODE_USER_VOICE];
        }

        /// <summary>
        /// VoiceHistory, 화자별로 포장되지 않은 상태
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeUserRawVoiceHistory()
        {
            return currentStoryJson[NODE_USER_RAW_VOICE];
        }


        /// <summary>
        /// 유저 에피소드 히스토리
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeUserEpisodeHistory()
        {
            return currentStoryJson[NODE_EPISODE_HISTORY];
        }

        /// <summary>
        /// 유저 에피소드 히스토리
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeUserEpisodeHistory(JsonData __j)
        {
            currentStoryJson[NODE_EPISODE_HISTORY] = __j;
        }

        /// <summary>
        /// 유저 에피소드 히스토리에 에피소드 단일 개체 추가 
        /// </summary>
        /// <param name="__playEpisodeID"></param>
        public void AddUserEpisodeHistory(int __playEpisodeID)
        {
            GetNodeUserEpisodeHistory().Add(__playEpisodeID);

            Debug.Log("AddUserEpisodeHistory : " + JsonMapper.ToStringUnicode(GetNodeUserEpisodeHistory()));
        }

        public JsonData GetNodeUserEpisodePurchase()
        {
            return currentStoryJson["episodePurchase"];
        }

        /// <summary>
        /// 다음 에피소드 정보 갱신
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeUserNextEpisode(JsonData __j)
        {
            currentStoryJson[NODE_NEXT_EPISODE] = __j;
            string.Format("SetNodeUserNextEpisode [{0}]", JsonMapper.ToStringUnicode(__j));
        }




        /// <summary>
        /// 플레이 위치 노드 저장 
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeUserProjectCurrent(JsonData __j)
        {
            currentStoryJson[NODE_PROJECT_CURRENT] = __j;
        }

        /// <summary>
        /// 유저, 작품별 플레이 위치 찾기. 
        /// 정규 에피소드 용도 
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserProjectCurrent(string __episodeID)
        {

            if (!currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT))
                return null;

            for (int i = 0; i < currentStoryJson[NODE_PROJECT_CURRENT].Count; i++)
            {
                if (currentStoryJson[NODE_PROJECT_CURRENT][i]["episode_id"].ToString() == __episodeID)
                    return currentStoryJson[NODE_PROJECT_CURRENT][i];
            }

            // return currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT) ? currentStoryJson[NODE_PROJECT_CURRENT] : null;

            return null;
        }

        /// <summary>
        /// 정규 에피소드의 Current를 찾는다.
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserProjectRegularEpisodeCurrent()
        {
            // 얘는 순서에 영향을 받지 않는다. 리스트에서만 사용한다.

            if (!currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT))
            {
                Debug.LogError("No project current node here");
                return null;
            }

            for (int i = 0; i < currentStoryJson[NODE_PROJECT_CURRENT].Count; i++)
            {
                if (!SystemManager.GetJsonNodeBool(currentStoryJson[NODE_PROJECT_CURRENT][i], "is_special")) // is_special 이니?
                    return currentStoryJson[NODE_PROJECT_CURRENT][i];
            }

            return null;

        }

        /// <summary>
        /// 현재 열람중인 작품이 마지막에 도달했는지 체크 (엔딩까지 플레이 완료)
        /// </summary>
        /// <returns></returns>
        public bool CheckReachFinal()
        {
            JsonData current = GetUserProjectRegularEpisodeCurrent();

            if (current == null)
                return false;

            if (SystemManager.GetJsonNodeBool(current, "is_final")
                && SystemManager.GetJsonNodeBool(current, "is_ending"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 현재 열람중인 작품이 엔딩에 도달했는지 체크 (엔딩 플레이 완료 여부는 관계없음)
        /// </summary>
        /// <returns></returns>
        public bool CheckReachEnding()
        {
            JsonData current = GetUserProjectRegularEpisodeCurrent();

            if (current == null)
                return false;

            if (SystemManager.GetJsonNodeBool(current, "is_ending"))
                return true;

            return false;
        }

        /// <summary>
        /// 정규 에피소드 끝났니?  (다음 에피소드가 엔딩인 경우)
        /// </summary>
        /// <returns></returns>
        public bool CheckUserProjectRegularEpisodeFinal()
        {
            if (!currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT))
                return false;

            // 
            for (int i = 0; i < currentStoryJson[NODE_PROJECT_CURRENT].Count; i++)
            {

                // 정규 에피소드, 다음에피소드가 엔딩인지.  
                if (!SystemManager.GetJsonNodeBool(currentStoryJson[NODE_PROJECT_CURRENT][i], "is_special")
                    && SystemManager.GetJsonNodeBool(currentStoryJson[NODE_PROJECT_CURRENT][i], "is_ending"))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 스페셜 에피소드의 Current를 찾는다.
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserProjectSpecialEpisodeCurrent()
        {
            // 얘는 순서에 영향을 받지 않는다. 리스트에서만 사용한다.

            if (!currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT))
                return null;

            for (int i = 0; i < currentStoryJson[NODE_PROJECT_CURRENT].Count; i++)
            {
                if (currentStoryJson[NODE_PROJECT_CURRENT][i]["is_special"].ToString() == "1") // is_special 이니?
                    return currentStoryJson[NODE_PROJECT_CURRENT][i];
            }

            return null;

        }


        /// <summary>
        /// 작품 선택지 선택 진행도 노드 저장 
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeUserProjectSelectionProgress(JsonData __j)
        {
            currentStoryJson[NODE_SELECTION_PROGRESS] = __j;
        }

        /// <summary>
        /// 프로젝트 선택지 프로그레스 
        /// Key는 episodeID
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserProjectSelectionProgress(string __episodeID)
        {
            if (!currentStoryJson.ContainsKey(NODE_SELECTION_PROGRESS))
                return null;

            if (!currentStoryJson[NODE_SELECTION_PROGRESS].ContainsKey(__episodeID))
                return null;

            return currentStoryJson[NODE_SELECTION_PROGRESS][__episodeID];
        }

        /// <summary>
        /// 유저의 작품에서 첫번째 selection 터치 체크 
        /// </summary>
        /// <returns>true : 첫번째다.</returns>
        public bool IsUserFirstSelection()
        {
            if (!currentStoryJson.ContainsKey(NODE_SELECTION_PROGRESS))
                return true;

            if (currentStoryJson[NODE_SELECTION_PROGRESS].Keys.Count == 0)
                return true;

            return false;
        }


        /// <summary>
        /// 대상 에피소드에 target_scene_id를 가진 선택지 Progress 체크 
        /// </summary>
        /// <param name="__episodeID"></param>
        /// <param name="__targetSceneID"></param>
        /// <returns></returns>
        public bool CheckProjectSelectionProgressExists(string __episodeID, string __targetSceneID)
        {
            JsonData targetEpisode = GetUserProjectSelectionProgress(__episodeID);

            if (targetEpisode == null)
                return false;

            // 에피소드별 Progress를 체크해서 ... 비교 
            // * 지나갔던 길은 다시 체크하지 않게 수정.
            for (int i = 0; i < targetEpisode.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(targetEpisode[i], "target_scene_id") == __targetSceneID
                    && !GameManager.main.CheckResumeSelectionPassed(targetEpisode[i]))
                {

                    Debug.Log("## Move to __targetSceneID : " + __targetSceneID);

                    // 루트 정보 저장하고 return true
                    GameManager.main.AddResumeSelectionRoute(targetEpisode[i]);
                    return true;
                }
            }

            // "target_scene_id": "1021",
            // "selection_data": "남을 공격하는 힘을 얻어야겠어."

            return false;
        }



        #endregion

        #region 통신 콜백 



        /// <summary>
        /// 에피소드 첫 보상 콜백 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void CallbackEpisodeFirstClearReward(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkLoader.CheckResponseValidation(request, response))
            {
                Debug.LogError("CallbackEpisodeFirstClearReward");
                return;
            }

            // bank, reward
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            Debug.Log(">> CallbackEpisodeFirstClearReward :: " + response.DataAsText);

            // bank 
            SetBankInfo(result);

            // resource 팝업창 
            string currency = SystemManager.GetJsonNodeString(result["reward"], "currency");
            int quantity = SystemManager.GetJsonNodeInt(result["reward"], "quantity");

            // 재화 획득 팝업 6201
            SystemManager.ShowResourcePopup(SystemManager.GetLocalizedText("6201"), quantity, SystemManager.main.GetCurrencyImageURL(currency), SystemManager.main.GetCurrencyImageKey(currency));

        }




        /// <summary>
        /// 프리패스 구매 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackPurchaseFreepass(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackPurchaseFreepass");
                return;
            }
            
            Debug.Log(string.Format("CallbackPurchaseFreepass [{0}]", res.DataAsText));
            JsonData result = JsonMapper.ToObject(res.DataAsText); // 결과 
            // Debug.Log(JsonMapper.ToStringUnicode(result));


            // * bank 
            SetBankInfo(result);
            
            // 데이터 처리 선행             
            if(GameManager.main != null || StoryLobbyManager.main != null) {
                StoryManager.main.CurrentProject.hasPremiumPass = true;
            }
            else { // 메인에서 구매 
                SystemListener.main.introduceStory.hasPremiumPass = true;
            }            


            // 모든 팝업 비활성화 
            PopupManager.main.HideActivePopup();

            // 이프유업적 프리패스 구매
            /*
            NetworkLoader.main.RequestIFYOUAchievement(15, SystemManager.GetJsonNodeInt(result, CommonConst.COL_PROJECT_ID));

            // 6306
            if (string.IsNullOrEmpty(StoryManager.main.CurrentProjectID))
                SystemManager.ShowMessageAlert(SystemManager.GetLocalizedText("6306"));
            else
                SystemManager.ShowMessageAlert(string.Format(SystemManager.GetLocalizedText("80061"), StoryManager.main.CurrentProjectTitle));
            */
            
            StartCoroutine(DelayRefreshPremiumPassStarPurchase());
        }
        
        /// <summary>
        /// 딜레이 갱신. 
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayRefreshPremiumPassStarPurchase() {
            
            Debug.Log("### DelayRefreshPremiumPassStarPurchase");
            
            yield return null;
            yield return null;
            yield return null;
            
            yield return new WaitForSeconds(0.1f);
            
            // 메세지 처리 
            if(GameManager.main != null || StoryLobbyManager.main != null) {
                Debug.Log("### DelayRefreshPremiumPassStarPurchase Current Story Target");
                SystemManager.ShowSystemPopup(string.Format(SystemManager.GetLocalizedText("6445"), StoryManager.main.CurrentProject.title), null, null, true, false);    
                StoryManager.main.CurrentProject.hasPremiumPass = true;
            }
            else { // 메인에서 구매 
            
            Debug.Log("### DelayRefreshPremiumPassStarPurchase Introduce Story Target");
                SystemManager.ShowSystemPopup(string.Format(SystemManager.GetLocalizedText("6445"), SystemListener.main.introduceStory.title), null, null, true, false);    
                SystemListener.main.introduceStory.hasPremiumPass = true;
            }
            
            
            
            BillingManager.main.CallPassButtonsRefresh();            
        }

        /// <summary>
        /// 선택지 프로그레스 업데이트 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackUpdateSelectionProgress(HTTPRequest req, HTTPResponse res)
        {
            // 통신 실패했을 때 갱신하지 않음. 
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateSelectionProgress");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText); // 결과 

            // 갱신
            currentStoryJson[NODE_SELECTION_PROGRESS] = result;

            // 업적용 선택지 고른 횟수 체크
            NetworkLoader.main.RequestIFYOUAchievement(13);
        }

        public void CallbackUpdateSelectionCurrent(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateSelectionCurrent");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            // 21.12.27 currentStoryJson에는 아직 뭔가 겹치는게 없어서 일단...이렇게 끝
        }


        /// <summary>
        /// 프로젝트 플레이 위치 저장 콜백!
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackUpdateProjectCurrent(HTTPRequest req, HTTPResponse res)
        {
            // 통신 실패했을 때 갱신하지 않음. 
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateProjectCurrent");
                return;
            }

            resultProjectCurrent = JsonMapper.ToObject(res.DataAsText); // 결과 

            // 갱신
            currentStoryJson[NODE_PROJECT_CURRENT] = resultProjectCurrent;
        }

        /// <summary>
        /// 프로젝트 플레이 위치 저장(시작 시점에!) 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackUpdateProjectCurrentWhenStart(HTTPRequest req, HTTPResponse res)
        {
            // 통신 실패했을 때 갱신하지 않음. 
            if (!NetworkLoader.CheckResponseValidation(req, res, true))
            {
                Debug.LogError("CallbackUpdateProjectCurrent");
                return;
            }

            resultProjectCurrent = JsonMapper.ToObject(res.DataAsText); // 결과 

            // 갱신
            currentStoryJson[NODE_PROJECT_CURRENT] = resultProjectCurrent;
        }








        /// <summary>
        /// 에피소드 리셋 프로그레스 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackResetEpisodeProgress(HTTPRequest req, HTTPResponse res)
        {
            // TODO 통신 실패했을 때 처리 필요해..
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateEpisodeRecord");
                return;
            }

            resultEpisodeReset = JsonMapper.ToObject(res.DataAsText);
            Debug.Log("CallbackUpdateEpisodeRecord : " + JsonMapper.ToStringUnicode(resultEpisodeReset));


            // ! 삭제 대상 아님 
            SetNodeUserEpisodeProgress(resultEpisodeReset[NODE_EPISODE_PROGRESS]); // 에피소드 progress 
            SetNodeStorySceneProgress(resultEpisodeReset[NODE_SCENE_PROGRESS]); // 씬 progress

            SetNodeUserProjectCurrent(resultEpisodeReset[NODE_PROJECT_CURRENT]);  // projectCurrent
            SetNodeUserProjectSelectionProgress(resultEpisodeReset[NODE_SELECTION_PROGRESS]); // 선택지 기록 

            SetBankInfo(resultEpisodeReset); // 뱅크 정보 업데이트 

            UpdateUserAbility(resultEpisodeReset[NODE_USER_ABILITY]); // 능력치 
            UpdateRawStoryAbility(resultEpisodeReset[NODE_RAW_STORY_ABILITY]);
            UserManager.main.SetStoryAbilityDictionary(resultEpisodeReset[NODE_USER_ABILITY]);

            // 첫화로 에피소드 리셋하는 경우 호출하는 이프유 업적
            if (NetworkLoader.main.isFirstEpisode)
                NetworkLoader.main.RequestIFYOUAchievement(16, -1, NetworkLoader.main.resetTargetEpisodeId);

            NetworkLoader.main.RequestIFYOUAchievement(17, -1, NetworkLoader.main.resetTargetEpisodeId);

            // 알림 팝업 후 목록화면 갱신처리 
            SystemManager.ShowSystemPopupLocalize("6167", null, null, true, false);

            // * Doozy Nody StoryDetail로 돌아가기 위한 이벤트 생성 
            // * ViewStoryDetail 에서 이 시그널을 Listener를 통해서 받는다. (Inspector)

            // refresh 플레이 상태 
            StoryManager.main.RefreshRegularEpisodePlayState();

            // 리셋 콜백
            StoryLobbyMain.OnCallbackReset?.Invoke();

            PopupFlowReset.OnHidePopup?.Invoke();
        }

        /// <summary>
        /// 처음으로 돌아가기 콜백 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackStartOverEpisode(HTTPRequest req, HTTPResponse res)
        {
            // TODO 통신 실패했을 때 처리 필요해..
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackStartOverEpisode");
                return;
            }

            resultEpisodeReset = JsonMapper.ToObject(res.DataAsText);
            Debug.Log(JsonMapper.ToStringUnicode(resultEpisodeReset));


            SetNodeStorySceneProgress(resultEpisodeReset[NODE_SCENE_PROGRESS]); // 씬 progress
            SetNodeUserProjectCurrent(resultEpisodeReset[NODE_PROJECT_CURRENT]);  // projectCurrent
            SetNodeUserProjectSelectionProgress(resultEpisodeReset[NODE_SELECTION_PROGRESS]); // 선택지 기록 

            UpdateUserAbility(resultEpisodeReset[NODE_USER_ABILITY]); // 능력치 
            UpdateRawStoryAbility(resultEpisodeReset[NODE_RAW_STORY_ABILITY]);


            // 통신 완료 후 게임매니저 메소드 호출 
            GameManager.main.RetryPlay();
        }


        /// <summary>
        /// 에피소드 기다리는 시간 감소 콜백 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void CallbackReduceWaitingTime(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkLoader.CheckResponseValidation(request, response))
            {
                StoryLobbyMain.CallbackReduceWaitingTimeFail?.Invoke();
                return;
            }

            JsonData result = JsonMapper.ToObject(response.DataAsText);
            Debug.Log(JsonMapper.ToStringUnicode(result));

            // 메세지 띄우고,  projectCurrent, bank 업데이트 
            // SystemManager.ShowMessageWithLocalize("6220");
            SystemManager.ShowSystemPopupLocalize("6220", null, null, true, false);
            
            SetNodeUserProjectCurrent(result[NODE_PROJECT_CURRENT]);  // projectCurrent
            // SetBankInfo(result); // 뱅크 정보 업데이트             


            // StoryLobbyMain 리프레시 요청 
            // 게임씬과 로비씬에서 담당 스크립트가 다르다 .
            if (GameManager.main != null)
            {
                EpisodeEndControls.CallbackReduceWaitingTimeSuccess?.Invoke();
            }
            else
            {
                StoryLobbyMain.CallbackReduceWaitingTimeSuccess?.Invoke();
            }


        }


        /// <summary>
        /// 코인으로 기다무 해금처리에 대한 콜백 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void CallbackReduceWaitingTimeWithCoin(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkLoader.CheckResponseValidation(request, response))
            {
                StoryLobbyMain.CallbackReduceWaitingTimeFail?.Invoke();
                return;
            }


            // * 코인으로 구매한 경우 서버에서 Permanent로 구매처리를 진행하기 때문에
            // * 아래에서 PurchaseHistory를 다시 받아온다. 
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            Debug.Log(JsonMapper.ToStringUnicode(result));

            // 메세지 띄우고,  projectCurrent, bank 업데이트 
            SystemManager.ShowMessageWithLocalize("6220");
            SetNodeUserProjectCurrent(result[NODE_PROJECT_CURRENT]);  // projectCurrent
            SetBankInfo(result); // 뱅크 정보 업데이트             

            //에피소드 구매 기록 
            if (result.ContainsKey(NODE_PURCHASE_HIST))
                SetNodeEpisodePurchaseHistory(result[NODE_PURCHASE_HIST]);


            // 22.04.06 기다무 시간 단축 초심자 클리어 했는지 체크도 해야함
            NetworkLoader.main.RequestIFYOUAchievement(6);

            NetworkLoader.main.RequestIFYOUAchievement(14);

            // StoryLobbyMain 리프레시 요청 
            // 게임씬과 로비씬에서 담당 스크립트가 다르다 .
            if (GameManager.main != null)
            {
                EpisodeEndControls.CallbackReduceWaitingTimeSuccess?.Invoke();
            }
            else
            {
                StoryLobbyMain.CallbackReduceWaitingTimeSuccess?.Invoke();
            }
        }



        /// <summary>
        /// 에피소드 구매&대여 결과 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackPurchaseEpisode(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res, true))
            {
                Debug.LogError("CallbackPurchaseEpisode");
                OnRequestEpisodePurchase?.Invoke(false);

                return;
            }

            // 응답결과 가져오기 
            Debug.Log("CallbackPurchaseEpisode : " + res.DataAsText);
            JsonData responseData = JsonMapper.ToObject(res.DataAsText);

            // 소모성 재화(코인, 젬)
            if (responseData.ContainsKey("bank"))
                SetBankInfo(responseData);

            // 에피소드 구매 기록 갱신 
            if (responseData.ContainsKey(NODE_PURCHASE_HIST))
                SetNodeEpisodePurchaseHistory(responseData[NODE_PURCHASE_HIST]);



            OnRequestEpisodePurchase?.Invoke(true);

        }


        /// <summary>
        /// 일러스트 해금 콜백
        /// </summary>
        public void CallbackUpdateIllustHistory(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateIllustHistory");

                // 실패에 대한 추가 처리 필요!
                return;
            }

            // 응답결과 가져오기 
            Debug.Log("CallbackUpdateIllustHistory : " + res.DataAsText);
            JsonData responseData = JsonMapper.ToObject(res.DataAsText);

            // 노드 갱신하자.
            SetNodeUserGalleryImages(responseData[NODE_USER_GALLERY_IMAGES]);

            // 이프유 업적(8.완벽주의자) 통신하기
            if (ProjectAllClear())
                NetworkLoader.main.RequestIFYOUAchievement(8, int.Parse(StoryManager.main.CurrentProjectID));
        }

        /// <summary>
        /// 미니컷 해금 콜백
        /// </summary>
        public void CallbackUpdateMinicutHistory(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateMinicutHistory");
                return;
            }

            // Debug.Log("CallbackUpdateMinicutHistory : " + res.DataAsText);
            JsonData responseData = JsonMapper.ToObject(res.DataAsText);

            // Node 갱신
            SetNodeUserGalleryImages(responseData[NODE_USER_GALLERY_IMAGES]);

            if (ProjectAllClear())
                NetworkLoader.main.RequestIFYOUAchievement(8, int.Parse(StoryManager.main.CurrentProjectID));
        }




        #endregion

        #region 사용자 에피소드 관련 메소드
            

        /// <summary>
        /// 현재의 사건 ID를 기록에 추가한다. 
        /// 사건 History, 사건 Progress 같이 추가된다. 
        /// </summary>
        /// <param name="__project_id"></param>
        /// <param name="__episode_id"></param>
        /// <param name="__scene_id"></param>
        public void UpdateSceneIDRecord(string __project_id, string __episode_id, string __scene_id)
        {
            
            Debug.Log(string.Format("UpdateSceneIDRecord : ", __scene_id));

            // * 여기도, 이어하기를 통해 진입한 경우 통신하지 않음
            // * 마지막 지점에 도착하면 isResumePlay는 false로 변경한다. 
            // 수집 엔딩 보는 중이어도 통신하지 않음
            /*
            if(GameManager.isResumePlay || !useRecord)
                return;
            */

            JsonData j = new JsonData();

            j[CommonConst.FUNC] = NetworkLoader.FUNC_UPDATE_EPISODE_SCENE_RECORD;
            j[CommonConst.COL_USERKEY] = userKey;
            j[CommonConst.COL_PROJECT_ID] = __project_id;
            j[CommonConst.COL_EPISODE_ID] = __episode_id;
            j[GameConst.COL_SCENE_ID] = __scene_id;

            NetworkLoader.main.SendPost(null, j, false, false);
            
            
            // 통신 응답을 기다리지 않고 바로 노드에 입력해준다. 
            AddUserProjectSceneHist(__scene_id);
            AddSceneProgress(__scene_id);
            
            CompleteMissionByScene(__scene_id); // 미션 해금 처리 
            ShowCompleteSpecialEpisodeByScene(__scene_id); // 스페셜 에피소드 해금 처리
        }

        public void UpdateSceneIDRecord(string __scene_id)
        {
            // 이미 프로그레스에 있으면 통신할 필요없다. 
            // 스킵에서 또 호출하기 싫으니까!
            if (UserManager.main.CheckSceneProgress(__scene_id))
                return;

            UpdateSceneIDRecord(StoryManager.main.CurrentProjectID, StoryManager.main.CurrentEpisodeID, __scene_id);
        }
        
        
        /// <summary>
        /// 2022.09.14 
        /// 에피소드 클리어 처리 최적화 버전
        /// </summary>
        /// <param name="nextEpisode">다음 회차</param>
        /// <param name="lastSceneID">마지막 씬 ID</param>
        public void RequestCompleteEpisodeOptimized(EpisodeData nextEpisode, string lastSceneID) {
            JsonData sending = new JsonData();
            sending["project_id"] = StoryManager.main.CurrentProjectID; // 현재 프로젝트 ID 
            sending["episodeID"] = StoryManager.main.CurrentEpisodeID; // 현재 프로젝트 ID 
            sending["episode_id"] = StoryManager.main.CurrentEpisodeID; 

            
            // 진행할 다음화가 있다면 다음화의 episode id 와 엔딩 여부를 추가해준다.
            if(nextEpisode != null && nextEpisode.isValidData && !string.IsNullOrEmpty(nextEpisode.episodeID)) {
                sending["nextEpisodeID"] = nextEpisode.episodeID; // 다음 에피소드 ID 있을때만
                
                // 다음화가 엔딩인 경우에 대한 처리
                if(nextEpisode.episodeType == EpisodeType.Ending) 
                    sending["is_next_ending"] = 1;
                else
                    sending["is_next_ending"] = 0;
                
            }
            
            // 마지막 플레이 사건ID 처리
            if(!string.IsNullOrEmpty(lastSceneID)) {
                // 마지막 플레이 scene_id 추가
                sending["scene_id"] = lastSceneID;    
                // 통신 응답을 기다리지 않고 바로 노드에 입력해준다. 
                AddUserProjectSceneHist(lastSceneID);
                AddSceneProgress(lastSceneID);
                
                CompleteMissionByScene(lastSceneID); // 미션 해금 처리 
                ShowCompleteSpecialEpisodeByScene(lastSceneID); // 스페셜 에피소드 해금 처리
            }

            sending["func"] = NetworkLoader.FUNC_EPISODE_COMPLETE; // 
            sending["ver"] = 42; // 버전 2022.01.24
            sending["useRecord"] = UserManager.main.useRecord;


            NetworkLoader.main.SendPost(UserManager.main.CallbackRequestCompleteEpisode, sending);
        }
        
        /// <summary>
        /// 유저 에피소드 클리어 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackRequestCompleteEpisode(HTTPRequest req, HTTPResponse res)
        {
            // 통신 실패했을 때 갱신하지 않음. 
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackRequestCompleteEpisode");
                return;
            }

            Debug.Log(">> CallbackRequestCompleteEpisode : " + res.DataAsText);

            // ! 여기에 JSON 따로 저장합니다. 
            resultEpisodeRecord = JsonMapper.ToObject(res.DataAsText);

            int playedEpisodeID = SystemManager.GetJsonNodeInt(resultEpisodeRecord, CommonConst.COL_EPISODE_ID);


            // 노드 저장!
            if (playedEpisodeID > 0)
            {
                // 첫 클리어 보상 관련 변수
                GameManager.main.hasFirstClearReward = !IsCompleteEpisode(playedEpisodeID.ToString());

                AddUserEpisodeHistory(playedEpisodeID); // 히스토리 
                AddUserEpisodeProgress(playedEpisodeID);  // 진행도 
                
                GameManager.main.currentEpisodeData.isClear = true;
            }

            SetNodeUserProjectCurrent(resultEpisodeRecord[NODE_PROJECT_CURRENT]);
            
            // * 에피소드 관련 미션과 첫클리어 보상에 대한 처리는 GameManager에서 진행한다. 2022.07.27

            // 현재 플레이한 에피소드가 정규 에피소드인 경우 이프유 업적 통신하기
            if (GameManager.main != null && GameManager.main.currentEpisodeData.episodeType == EpisodeType.Chapter)
                NetworkLoader.main.RequestIFYOUAchievement(12, -1, int.Parse(StoryManager.main.CurrentEpisodeID));

            NetworkLoader.main.IncreaseDailyMissionCount(2);
        }

        



        /// <summary>
        /// 에피소드 진입시 사건 Progress 클리어 
        /// </summary>
        /// <param name="__project_id"></param>
        /// <param name="__episode_id"></param>
        /// <param name="__scene_id"></param>
        public void ClearSelectedEpisodeSceneProgress(string __project_id, string __episode_id, System.Action __cb)
        {
            Debug.Log("<color=white>ClearSelectedEpisodeSceneProgress</color>");

            JsonData j = new JsonData();
            j["func"] = NetworkLoader.FUNC_CLEAR_EPISODE_SCENE_HISTORY;
            j["project_id"] = __project_id;
            j["episode_id"] = __episode_id;

            OnCleanUserEpisodeProgress = __cb;
            NetworkLoader.main.SendPost(CallbackClearSelectedEpisodeSceneHistory, j);


        }

        /// <summary>
        /// ClearSelectedEpisodeSceneProgress 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void CallbackClearSelectedEpisodeSceneHistory(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackClearSelectedEpisodeSceneHistory");
                return;
            }

            // 대상 에피소드에 속한 scene 정보만 삭제하기 때문에.. 
            // 리스트를 갱신해서 받아와야겠다..!!!
            Debug.Log(res.DataAsText);

            // 새로운 리스트로 갱신한다. 
            SetNodeStorySceneProgress(JsonMapper.ToObject(res.DataAsText));

            // 다 하고, 콜백 메소드 호출한다(GameManager)
            OnCleanUserEpisodeProgress?.Invoke();
        }


        /// <summary>
        /// 에피소드 ID 진행도에 있는지 체크. 
        /// </summary>
        /// <param name="__episodeID"></param>
        /// <returns></returns>
        public bool CheckEpisodeProgress(string __episodeID)
        {
            // Debug.Log(string.Format("CheckEpisodeProgress : [{0}]", __episodeID));

            for (int i = 0; i < GetNodeUserEpisodeProgress().Count; i++)
            {
                // 있어!
                if (GetNodeUserEpisodeProgress()[i].ToString() == __episodeID)
                    return true;
            }


            return false;
        }


        /// <summary>
        /// 현재 진행도에 대상 씬을 클리어했는지 체크 
        /// </summary>
        /// <param name="__scene_id"></param>
        /// <returns></returns>
        public bool CheckSceneProgress(string __scene_id)
        {
            if (currentStoryJson == null)
                return false;
                
            if(!currentStoryJson.ContainsKey(NODE_SCENE_PROGRESS))
                return false;

            Debug.Log(string.Format("CheckSceneProgress : [{0}]", __scene_id));

            for (int i = 0; i < GetNodeStorySceneProgress().Count; i++)
            {
                if (GetNodeStorySceneProgress()[i].ToString() == __scene_id)
                    return true;
            }

            Debug.Log(string.Format("{0} No progress scene", __scene_id));
            return false;
        }

        /// <summary>
        /// 유저의 사건ID 히스토리 유무 체크
        /// </summary>
        /// <param name="__sceneID">사건 ID</param>
        public bool CheckSceneHistory(string __sceneID)
        {
            // 어드민 유저 무조건 true 
            /*
            if(CheckAdminUser())
                return true; 
            */


            if (currentStoryJson == null || GetNodeProjectSceneHistory() == null)
            {
                Debug.Log("<color=orange>GetNodeProjectSceneHistory is empty</color>");
                return false;
            }

            for (int i = 0; i < GetNodeProjectSceneHistory().Count; i++)
            {
                if (GetNodeProjectSceneHistory()[i].ToString() == __sceneID)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 유저가 해당 작품의 에피소드를 구매 혹은 대여한 적이 있는지 유무 체크
        /// </summary>
        /// <param name="__episodeId">식별자</param>
        /// <returns>true는 존재, false는 미구매(대여)</returns>
        public bool CheckPurchaseEpisode(string __episodeId, ref JsonData __j)
        {
            for (int i = 0; i < GetNodeUserEpisodePurchase().Count; i++)
            {
                if (GetNodeUserEpisodePurchase()[i]["episode_id"].ToString().Equals(__episodeId))
                {
                    __j = GetNodeUserEpisodePurchase()[i];
                    return true;
                }
            }

            __j = null;
            return false;
        }

        /// <summary>
        /// 엔딩 에피소드 Id에 해당하는 엔딩 힌트 정보를 넘겨준다
        /// </summary>
        /// <param name="__episodeId"></param>
        /// <returns>엔딩 힌트 요소 데이터</returns>
        public JsonData EndingHintData(string __episodeId)
        {
            JsonData hintData = SystemManager.GetJsonNode(currentStoryJson, "endingHint");

            for (int i = 0; i < hintData.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(hintData[i], "ending_id") == __episodeId)
                    return hintData[i];
            }

            return null;
        }




        #endregion


        #region 일러스트 관련

        /// <summary>
        /// 현재 일러스트가 해금가능한지 체크한다.
        /// </summary>
        /// <param name="__illustID">일러스트 ID</param>
        /// <param name="__illustType">일러스트 타입(illust/live_illust)</param>
        /// <returns></returns>
        public bool CheckIllustUnlockable(string __illustID, string __illustType)
        {
            // 노드 루프돌면서 오픈된 기록이 있는지 체크한다.
            for (int i = 0; i < GetUserGalleryImage().Count; i++)
            {
                if (GetUserGalleryImage()[i]["illust_id"].ToString() == __illustID
                    && GetUserGalleryImage()[i]["illust_type"].ToString() == __illustType
                    && GetUserGalleryImage()[i]["illust_open"].ToString() == "0")

                    // 갤러리 이미지에 목록이 있고, 해금되지 않았다. => 해금 가능하다.
                    return true;
            }

            // 목록에 겂거나, 이미 해금되었다. 
            return false;
        }

        /// <summary>
        ///  갤러리에서 사용되는 이미지들의 퍼블릭 네임 가져오기 
        /// </summary>
        /// <param name="__id"></param>
        /// <param name="__type"></param>
        /// <returns></returns>
        public string GetGalleryImagePublicName(string __id, string __type)
        {
            for (int i = 0; i < GetUserGalleryImage().Count; i++)
            {

                if (SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], "illust_id") == __id
                    && SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], "illust_type") == __type)
                {

                    return SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], "public_name");
                }
            }

            return string.Empty;
        }


        public float CalcEpisodeGalleryProgress(string __episodeID)
        {

            int totalEpisodeImage = 0;
            int openEpisodeImage = 0;

            for (int i = 0; i < GetUserGalleryImage().Count; i++)
            {
                if (SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], "appear_episode") == __episodeID
                    && SystemManager.GetJsonNodeBool(GetUserGalleryImage()[i], "valid"))
                {
                    totalEpisodeImage++;

                    // 오픈된 경우. 
                    if (SystemManager.GetJsonNodeBool(GetUserGalleryImage()[i], "illust_open"))
                        openEpisodeImage++;
                }
            }

            if (totalEpisodeImage == 0)
                return -1;

            if (openEpisodeImage == 0)
                return 0;

            Debug.Log(string.Format("### CalcEpisodeGalleryProgress {0}/{1}", openEpisodeImage, totalEpisodeImage));
            return (float)openEpisodeImage / (float)totalEpisodeImage;
        }

        /// <summary>
        /// 갤러리 이미지를 획득 했는지 체크
        /// </summary>
        /// <param name="__illustId">illust, live_illust, minicut, live_object id값</param>
        bool ObtainGalleryImage(string __galleryId, string __galleryType)
        {

            for (int i = 0; i < GetUserGalleryImage().Count; i++)
            {
                // 값 있을때.  타입이랑 미니컷 여부까지 맞아야 한다. 
                if (SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], "illust_id") == __galleryId
                    && SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], "illust_type") == __galleryType
                    && SystemManager.GetJsonNodeBool(GetUserGalleryImage()[i], "illust_open"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 갤러리에 있는 공개된 일러스트(이미지, 일러스트)인지 확인
        /// </summary>
        /// <param name="__illustName"></param>
        /// <returns>true = 존재, false = 비공개용</returns>
        public bool RevealedGalleryImage(string __illustName)
        {
            for (int i = 0; i < GetUserGalleryImage().Count; i++)
            {
                if (SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], "illust_name") == __illustName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 해금된 이미지(라이브 오브제)인지 check
        /// </summary>
        /// <param name="imageName">이미지(라이브오브제) 이름</param>
        public bool CheckMinicutUnlockable(string imageName, string minicutType)
        {
            // 노드 루프돌면서 오픈된 기록이 있는지 체크한다.
            for (int i = 0; i < GetUserGalleryImage().Count; i++)
            {
                if (SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], LobbyConst.ILLUST_NAME) == imageName
                    && SystemManager.GetJsonNodeString(GetUserGalleryImage()[i], "illust_type") == minicutType
                    && !SystemManager.GetJsonNodeBool(GetUserGalleryImage()[i], "illust_open"))
                {

                    return true; // 해금 가능
                }
            }

            return false; // 데이터가 없다. 불가능
        }

        #endregion


        #region 갤러리 - 보이스 관련

        /// <summary>
        /// 해금된 voice인지 check
        /// </summary>
        /// <param name="soundName">사운드 명</param>
        /// <param name="episodeTitle">현재 에피소드 타이틀</param>
        /// <returns>true면 해금한적 있음, false면 해금한적 없음</returns>
        public bool CheckNewVoice(string soundName)
        {

            // 이전의 VoiceHistory는 화자 > 에피소드 까지 3중 키로 포장되어 와서 
            // 있는지 없는지 체크가 너무 어렵다. 
            // 그래서 RawVoiceHistory를 사용하도록 한다! 
            for (int i = 0; i < GetNodeUserRawVoiceHistory().Count; i++)
            {
                if (SystemManager.GetJsonNodeString(GetNodeUserRawVoiceHistory()[i], CommonConst.SOUND_NAME) == soundName
                    && SystemManager.GetJsonNodeBool(GetNodeUserRawVoiceHistory()[i], CommonConst.IS_OPEN))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 대상 보이스를 해금 처리 
        /// </summary>
        /// <param name="soundName"></param>
        public void SetVoiceOpen(string soundName)
        {
            for (int i = 0; i < GetNodeUserRawVoiceHistory().Count; i++)
            {
                if (SystemManager.GetJsonNodeString(GetNodeUserRawVoiceHistory()[i], CommonConst.SOUND_NAME) == soundName)
                {
                    GetNodeUserRawVoiceHistory()[i][CommonConst.IS_OPEN] = 1; // 오픈처리
                    return;
                }
            }
        }

        #endregion


        #region 유저 능력치 관련 메소드 

        public JsonData GetUserAbility()
        {
            if (!currentStoryJson.ContainsKey(NODE_USER_ABILITY))
            {
                return null;
            }

            return currentStoryJson[NODE_USER_ABILITY];
        }

        public JsonData GetRawStoryAbility()
        {
            if (!currentStoryJson.ContainsKey(NODE_RAW_STORY_ABILITY))
            {
                return null;
            }

            return currentStoryJson[NODE_RAW_STORY_ABILITY];
        }

        /// <summary>
        /// 유저 능력치 json 갱신 
        /// </summary>
        /// <param name="__newData"></param>
        public void UpdateUserAbility(JsonData __newData)
        {
            currentStoryJson[NODE_USER_ABILITY] = __newData;

            Debug.Log("###  UpdateUserAbility : " + JsonMapper.ToStringUnicode(currentStoryJson[NODE_USER_ABILITY]));
        }

        /// <summary>
        /// 스토리 누적 능력치 갱신 
        /// </summary>
        /// <param name="__newData"></param>
        public void UpdateRawStoryAbility(JsonData __newData)
        {
            currentStoryJson[NODE_RAW_STORY_ABILITY] = __newData;
        }


        /// <summary>
        /// 현재 작품의 화자, 능력치에 해당하는 수치 가져오기 
        /// </summary>
        /// <param name="__speaker"></param>
        /// <param name="__ability"></param>
        /// <returns></returns>
        public int GetSpeakerAbilityValue(string __speaker, string __ability)
        {

            // key 체크 
            if (!GetUserAbility().ContainsKey(__speaker))
            {
                Debug.LogError(string.Format("No match speaker in [{0}/{1}]", __speaker, __ability));
                return 0;
            }

            for (int i = 0; i < GetUserAbility()[__speaker].Count; i++)
            {
                if (GetUserAbility()[__speaker][i]["ability_name"].ToString() == __ability)
                {
                    return SystemManager.GetJsonNodeInt(GetUserAbility()[__speaker][i], "current_value");
                }
            }

            Debug.LogError(string.Format("No match ability in [{0}/{1}]", __speaker, __ability));
            return 0;

        }

        /// <summary>
        /// 현재 작품의 화자, 능력치에 해당하는 Percentage 가져오기 
        /// </summary>
        /// <param name="__speaker"></param>
        /// <param name="__ability"></param>
        /// <returns></returns>
        public int GetSpeakerAbilityPercentage(string __speaker, string __ability)
        {

            int currentValue = 0;
            int maxValue = 0;

            // key 체크 
            if (!GetUserAbility().ContainsKey(__speaker))
            {
                Debug.LogError(string.Format("No match speaker in [{0}/{1}]", __speaker, __ability));
                return 0;
            }

            for (int i = 0; i < GetUserAbility()[__speaker].Count; i++)
            {
                if (GetUserAbility()[__speaker][i]["ability_name"].ToString() == __ability)
                {
                    currentValue = SystemManager.GetJsonNodeInt(GetUserAbility()[__speaker][i], "current_value");
                    maxValue = SystemManager.GetJsonNodeInt(GetUserAbility()[__speaker][i], "max_value");
                    break;
                }
            }

            // 백분율 
            return Mathf.RoundToInt((float)currentValue / (float)maxValue * 100f);


        }

        /// <summary>
        /// 화자, 능력치 json 
        /// </summary>
        /// <param name="__speaker"></param>
        /// <param name="__ability"></param>
        /// <returns></returns>
        public JsonData GetSpeakerAbilityJSON(string __speaker, string __ability)
        {
            // key 체크 
            if (!GetUserAbility().ContainsKey(__speaker))
            {
                Debug.LogError(string.Format("No match speaker in [{0}/{1}]", __speaker, __ability));
                return null;
            }

            for (int i = 0; i < GetUserAbility()[__speaker].Count; i++)
            {
                if (GetUserAbility()[__speaker][i]["ability_name"].ToString() == __ability)
                {
                    return GetUserAbility()[__speaker][i];
                }
            }

            Debug.LogError(string.Format("No match ability in [{0}/{1}]", __speaker, __ability));
            return null;
        }

        /// <summary>
        /// 에피소드, 씬에서 이미 획득한 능력치 정보가 있는지 체크 
        /// </summary>
        /// <param name="__episode_id"></param>
        /// <param name="__scene_id"></param>
        /// <param name="__abilityName"></param>
        /// <param name="__value"></param>
        /// <returns></returns>
        public bool CheckSceneAbilityHistory(string __episode_id, string __scene_id, string __speaker, string __abilityName, int __value)
        {

            if (GetRawStoryAbility() == null)
                return false;

            for (int i = 0; i < GetRawStoryAbility().Count; i++)
            {
                if (SystemManager.GetJsonNodeString(GetRawStoryAbility()[i], CommonConst.COL_EPISODE_ID) == __episode_id
                    && SystemManager.GetJsonNodeString(GetRawStoryAbility()[i], GameConst.COL_SCENE_ID) == __scene_id
                    && SystemManager.GetJsonNodeString(GetRawStoryAbility()[i], GameConst.COL_SPEAKER) == __speaker
                    && SystemManager.GetJsonNodeString(GetRawStoryAbility()[i], "ability_name") == __abilityName
                    && SystemManager.GetJsonNodeInt(GetRawStoryAbility()[i], "add_value") == __value)
                    return true; // 데이터 있음 

            }


            return false;
        }


        /// <summary>
        /// 유저 인트로 수행 여부 처리 
        /// </summary>        
        public void UpdateIntroComplete()
        {
            JsonData sending = new JsonData();
            sending["func"] = "updateUserIntroDone";
            NetworkLoader.main.SendPost(OnUpdateIntroComplete, sending, false);
        }

        void OnUpdateIntroComplete(HTTPRequest request, HTTPResponse response)
        {

            NetworkLoader.CheckResponseValidation(request, response);

            isIntroDone = true; // 그냥 true 처리 
        }



        #endregion

        #region 이프유플레이 관련
        
        /// <summary>
        /// 일일 미션 카운트 추가 콜백 2022.09
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackIncreaseDailyMissionCount(HTTPRequest req, HTTPResponse res) {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackIfyouplayList");
                return;
            }
            
            JsonData result = JsonMapper.ToObject(res.DataAsText);
            
            // userIfyouPlayJson 노드 갱신. 
            if(userIfyouPlayJson != null && result.ContainsKey(LobbyConst.NODE_DAILY_MISSION)) {
                userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION] = result[LobbyConst.NODE_DAILY_MISSION];
            }
            
        }
        
        /// <summary>
        /// 이프유 페이지 전체 데이터 받아오기 콜백 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackIfyouplayList(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackIfyouplayList");
                return;
            }

            userIfyouPlayJson = JsonMapper.ToObject(res.DataAsText);
            
            SetDailyMissionTimer();
            SetIFyouPlayAdCoolTime();
        }

        
        public void CallbackIfyouplayRefresh(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackIfyouplayList");
                return;
            }

            userIfyouPlayJson = JsonMapper.ToObject(res.DataAsText);
            
            SetDailyMissionTimer();
            SetIFyouPlayAdCoolTime();
            
            MainIfyouplay.OnRefreshIfyouplay?.Invoke();

        }
        
        
        /// <summary>
        /// 이프유 플레이 광고 보상 요청 콜백 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackIfyouPlayAdReward(HTTPRequest req, HTTPResponse res) {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackIfyouPlayAdReward");
                return;
            }
            
            JsonData result = JsonMapper.ToObject(res.DataAsText);
            bool isSuccess = SystemManager.GetJsonNodeBool(result, "result"); // 성공여부 
            
            // 실패시 
            if(!isSuccess) {
                string failMessage = SystemManager.GetJsonNodeString(result, "message");
                Debug.LogError("CallbackGetMissionReward Error : " + failMessage);
                
                if(failMessage == "received") { // 이미 받은 상태. 
                    SystemManager.ShowMessageWithLocalize("80025");
                }
                else if(failMessage == "no data") {
                    SystemManager.ShowMessageWithLocalize("80019");
                }
                
                
                return;
                
            }
            
            // 재화 획득 메세지 
            SystemManager.ShowResourcePopup(SystemManager.GetLocalizedText("6488"), SystemManager.GetJsonNodeString(result, "currency"),SystemManager.GetJsonNodeInt(result, "quantity"));
            
            // 재화 갱신 
            SetBankInfo(result);
            
            
            // 노드 갱신 
            if(result.ContainsKey(LobbyConst.NODE_TIMER_AD_REWARD) && UserManager.main.userIfyouPlayJson != null) {
                UserManager.main.userIfyouPlayJson[LobbyConst.NODE_TIMER_AD_REWARD] = result[LobbyConst.NODE_TIMER_AD_REWARD];
                SetIFyouPlayAdCoolTime(); // 쿨타임 설정 
                
                MainIfyouplay.OnRefreshTimerAdvertisementPart?.Invoke();
            }
            else if(result.ContainsKey(LobbyConst.NODE_MISSION_AD_REWARD) && UserManager.main.userIfyouPlayJson != null) {
                UserManager.main.userIfyouPlayJson[LobbyConst.NODE_MISSION_AD_REWARD] = result[LobbyConst.NODE_MISSION_AD_REWARD];
                MainIfyouplay.OnRefreshMissionAdvertisementPart?.Invoke();
            }
            
            
            if(result.ContainsKey(LobbyConst.NODE_DAILY_MISSION)) {
                UserManager.main.userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION] = result[LobbyConst.NODE_DAILY_MISSION];
                MainIfyouplay.OnRefreshDailyMissionPart?.Invoke();
            }
            
            
            ViewMain.OnRefreshIfyouplayNewSign?.Invoke();

            
            
            
        }
        
        
        
        /// <summary>
        /// 수령할 수 있는 데일리 미션 체크 
        /// </summary>
        /// <returns></returns>
        bool CheckReceivableDailyMission() {
            if(userIfyouPlayJson == null || !userIfyouPlayJson.ContainsKey(LobbyConst.NODE_DAILY_MISSION)) {
                return false;
            }
            
            if(userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION].Count == 0)
                return false;
                
            for(int i=0; i<userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION].Count;i++) {
                
                // 리미트 카운트 이상 달성했으면, 받을 수 있다. 
                if(SystemManager.GetJsonNodeInt(userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION][i], "current_result") >=
                 SystemManager.GetJsonNodeInt(userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION][i], "limit_count")) {
                    return true;
                }
                
            }
            
            return false;
        }


        /// <summary>
        /// 수령가능 보상이 있거나, 출석 보충을 해야하거나 하는 경우 사용되는 함수
        /// </summary>
        /// <returns></returns>
        public bool CheckIfyouplayAction()
        {
            if (userIfyouPlayJson == null)
                return false;

            JsonData attendanceMission = SystemManager.GetJsonNode(userIfyouPlayJson, LobbyConst.NODE_ATTENDANCE_MISSION);

            if (attendanceMission == null)
                return false;


            // 오늘 아직 출석하지 않았을 때 true
            if (!TodayAttendanceCheck())
                return true;

            // 데일리 미션 체크 
            if(CheckReceivableDailyMission())
                return true;
               

            JsonData missionAdReward = SystemManager.GetJsonNode(userIfyouPlayJson, "missionAdReward");

            if (missionAdReward == null)
                return false;

            // clear_date는 3단계 보상까지 다 받으면 null값이 아니며, 보상을 받을 수 있을 때는 총 광고를 본 횟수가 총 봐야하는 광고수 이상이 되면 true를 리턴
            if (string.IsNullOrEmpty(SystemManager.GetJsonNodeString(missionAdReward[0], "clear_date")) &&
                SystemManager.GetJsonNodeInt(missionAdReward[0], "current_result") >= SystemManager.GetJsonNodeInt(missionAdReward[0], "total_count"))
                return true;

            JsonData timerAdReward = SystemManager.GetJsonNode(userIfyouPlayJson, "timerAdReward");

            if (timerAdReward == null)
                return false;

            long endDateTick = SystemConst.ConvertServerTimeTick(SystemManager.GetJsonNodeLong(timerAdReward[0], "remain_date_tick"));
            DateTime endDate = new DateTime(endDateTick);

            adCoolDownTimer = endDate - DateTime.UtcNow;

            // 볼 수 있는 쿨타임 광고가 존재함
            if (adCoolDownTimer.Ticks <= 0)
                return true;


            return false;
        }
        
        /// <summary>
        /// 데일리 미션 타이머 세팅 
        /// </summary>
        void SetDailyMissionTimer() {
            if(userIfyouPlayJson == null || !userIfyouPlayJson.ContainsKey(LobbyConst.NODE_DAILY_MISSION))
                return;
                
            JsonData dailyMissionData = userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION];
            
            if(dailyMissionData.Count == 0)
                return;
                
            long endDateTick = SystemConst.ConvertServerTimeTick(SystemManager.GetJsonNodeLong(dailyMissionData[0], "end_date_tick"));
            dateDailyMission = new DateTime(endDateTick);

            dailyMissionTimer = dateDailyMission - DateTime.UtcNow;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDailyMissionRefreshTimeText() {
            
            try {
                if(dateDailyMission == null)
                    return string.Empty;
                
                dailyMissionTimer = dateDailyMission - System.DateTime.UtcNow;
            }
            catch {
                return string.Empty;
            }

            if (dailyMissionTimer.Ticks <= 0)
            {
                return string.Empty;
            }

            if (dailyMissionTimer.TotalHours < 24)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", dailyMissionTimer.Hours, dailyMissionTimer.Minutes, dailyMissionTimer.Seconds);
            }
            else
            {
                return string.Format("{0}d {1:D2}:{2:D2}:{3:D2}", dailyMissionTimer.Days, dailyMissionTimer.Hours, dailyMissionTimer.Minutes, dailyMissionTimer.Seconds);
            }
        }



        /// <summary>
        ///  이프유 플레이 광고 쿨타임 설정 
        /// </summary>
        void SetIFyouPlayAdCoolTime() {
            if(userIfyouPlayJson == null || !userIfyouPlayJson.ContainsKey(LobbyConst.NODE_TIMER_AD_REWARD))
                return;
                
            JsonData timerAd = SystemManager.GetJsonNode(userIfyouPlayJson, LobbyConst.NODE_TIMER_AD_REWARD);
                
            long endDateTick = SystemConst.ConvertServerTimeTick(SystemManager.GetJsonNodeLong(timerAd[0], "remain_date_tick"));
            dateAdCoolDown = new DateTime(endDateTick);

            adCoolDownTimer = dateAdCoolDown - DateTime.UtcNow;
            
        }
        
        /// <summary>
        /// 타이머 광고 남은 쿨타임 텍스트 
        /// </summary>
        /// <returns></returns>
        public string GetIFyouPlayAdRefreshTimeText() {
            
            try {
                if(dateAdCoolDown == null)
                    return string.Empty;
                
            
                adCoolDownTimer = dateAdCoolDown - System.DateTime.UtcNow;
            }
            catch {
                return string.Empty;

            }
            

            if (adCoolDownTimer.Ticks <= 0)
            {
                return string.Empty;
            }
            
            return string.Format("{0:D2}:{1:D2}", adCoolDownTimer.Minutes, adCoolDownTimer.Seconds);

            
        }
        
        

        #region 출석


        /// <summary>
        /// 이프유플레이 출석 jsonData만 갱신
        /// </summary>
        /// <param name="__j"></param>
        public void UpdateIFyouPlayAttendance(JsonData __j)
        {
            if(__j == null || !__j.ContainsKey(LobbyConst.NODE_ATTENDANCE))
                return;
            
            if(userIfyouPlayJson == null || !userIfyouPlayJson.ContainsKey(LobbyConst.NODE_ATTENDANCE_MISSION))
                return;
            
            
            userIfyouPlayJson[LobbyConst.NODE_ATTENDANCE_MISSION][LobbyConst.NODE_ATTENDANCE] = __j[LobbyConst.NODE_ATTENDANCE];

            ViewMain.OnRefreshIfyouplayNewSign?.Invoke();
        }
        
        

        /// <summary>
        /// 오늘의 출석체크 보상을 받았는지 체크합니다
        /// </summary>
        /// <returns></returns>
        public bool TodayAttendanceCheck()
        {
            string attendanceId = userIfyouPlayJson[LobbyConst.NODE_ATTENDANCE_MISSION][LobbyConst.NODE_ATTENDANCE][LobbyConst.NODE_ATTENDANCE][0].ToString();
            JsonData attendanceList = SystemManager.GetJsonNode(userIfyouPlayJson[LobbyConst.NODE_ATTENDANCE_MISSION][LobbyConst.NODE_ATTENDANCE], attendanceId);

            for (int i = 0; i < attendanceList.Count; i++)
            {
                if (!SystemManager.GetJsonNodeBool(attendanceList[i], "current"))
                    continue;

                // 클릭할 수 있는게 있다면 아직 안받은거야
                if (SystemManager.GetJsonNodeBool(attendanceList[i], "click_check"))
                    return false;
            }

            return true;
        }


        #endregion

        #region Daily Mission

        /// <summary>
        /// 데일리 미션 보상 요청
        /// </summary>
        /// <param name="missionNo">미션 번호</param>
        /// <param name="__callback"></param>
        public void RequestDailyMissionReward(int missionNo)
        {
            JsonData sending = new JsonData();
            //sending[CommonConst.FUNC] = "requestDailyMissionReward";
            sending[CommonConst.FUNC] = "requestDailyMissionRewardOptimized";
            sending[CommonConst.COL_USERKEY] = userKey;
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;
            sending["mission_no"] = missionNo;

            NetworkLoader.main.SendPost(CallbackGetMissionReward, sending, true);
        }
        
        public void CallbackGetMissionReward(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackGetMissionReward");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);
            bool isSuccess = SystemManager.GetJsonNodeBool(result, "result"); // 성공여부 
            
            
            // 실패시 
            if(!isSuccess) {
                string failMessage = SystemManager.GetJsonNodeString(result, "message");
                Debug.LogError("CallbackGetMissionReward Error : " + failMessage);
                
                if(failMessage == "received") { // 이미 받은 상태. 
                    SystemManager.ShowMessageWithLocalize("80025");
                }
                else if(failMessage == "no data") {
                    SystemManager.ShowMessageWithLocalize("80019");
                }
                
                
                return;
            }
            
            
            // 리소스 팝업을 띄워줍시다.
            // 미션 보상을 받았습니다 메세지. 
            SystemManager.ShowResourcePopup(SystemManager.GetLocalizedText("6123"), SystemManager.GetJsonNodeString(result, "currency"),SystemManager.GetJsonNodeInt(result, "quantity"));
            

            SetBankInfo(result);
            
            if(result.ContainsKey(LobbyConst.NODE_DAILY_MISSION) && UserManager.main.userIfyouPlayJson != null) {
                UserManager.main.userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION] = result[LobbyConst.NODE_DAILY_MISSION];
                SetDailyMissionTimer();
            }

            MainIfyouplay.OnRefreshDailyMissionPart?.Invoke();
            ViewMain.OnRefreshIfyouplayNewSign?.Invoke();
        }        

        #endregion


        #endregion

        #region IFYOU 업적 관련


        /// <summary>
        /// 이프유 업적 누적 콜백
        /// </summary>
        public void CallbackRequestAchievement(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRequestAchievement");
                return;
            }

            Debug.Log(JsonMapper.ToStringUnicode(JsonMapper.ToObject(res.DataAsText)));
        }

        /// <summary>
        /// 유저 업적 리스트 요청
        /// </summary>
        public void RequestUserGradeInfo(OnRequestFinishedDelegate __cb, bool isSync = false)
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestUserGradeInfo";
            sending[CommonConst.COL_USERKEY] = userKey;
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;

            NetworkLoader.main.SendPost(__cb, sending, isSync);
        }


        /// <summary>
        /// 통상적 업적 리스트 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackUserGreadeInfo(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackUserGreadeInfo");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);
            //Debug.Log(string.Format("유저 업적 리스트 요청 결과 = {0}", JsonMapper.ToStringUnicode(result)));

            // 시즌 정산중인지 체크
            SetSeasonCheck(result);

            // grade key값에 대한 정보 세팅
            SetUserGradeInfo(result);

            // 업적 리스트 세팅
            SetAchievementList(result);
        }

        public void CallbackNewCompleteAchievement(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackNewCompleteAchievement");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            Debug.Log(JsonMapper.ToStringUnicode(result));

            SetSeasonCheck(result);
            SetAchievementList(result);

            ViewMain.OnRefreshProfileNewSign?.Invoke();
        }

        /// <summary>
        /// 업적 보상을 받을 수 있는 총 수 count
        /// </summary>
        /// <returns></returns>
        public int CountClearAchievement()
        {
            if (NetworkLoader.main.seasonCalculating)
                return 0;

            int count = 0;

            for (int i = 0; i < listAchievement.Count; i++)
            {
                if (listAchievement[i].achievementDegree >= 1f)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// 시즌 정산중인지 체크 세팅
        /// </summary>
        public void SetSeasonCheck(JsonData __j)
        {
            NetworkLoader.main.seasonCalculating = SystemManager.GetJsonNodeBool(__j["season_check"][0], "calculate_check");
        }

        /// <summary>
        /// 사용자의 등급 정보 세팅
        /// </summary>
        /// <param name="__j"></param>
        public void SetUserGradeInfo(JsonData __j)
        {
            grade = SystemManager.GetJsonNodeInt(__j["grade"][0], "grade");
            gradeName = SystemManager.GetJsonNodeString(__j["grade"][0], "name");

            nextGrade = SystemManager.GetJsonNodeInt(__j["grade"][0], "next_grade");

            gradeExperience = SystemManager.GetJsonNodeInt(__j["grade"][0], "grade_experience");
            keepPoint = SystemManager.GetJsonNodeInt(__j["grade"][0], "keep_point");
            upgradeGoalPoint = SystemManager.GetJsonNodeInt(__j["grade"][0], "upgrade_point");
            remainDay = SystemManager.GetJsonNodeInt(__j["grade"][0], "remain_day");
            additionalStarDegree = SystemManager.GetJsonNodeInt(__j["grade"][0], "add_star");
            additionalStarLimitCount = SystemManager.GetJsonNodeInt(__j["grade"][0], "add_star_limit");
            additionalStarUse = SystemManager.GetJsonNodeInt(__j["grade"][0], "add_star_use");
            waitingSaleDegree = SystemManager.GetJsonNodeInt(__j["grade"][0], "waiting_sale");
            canPreview = SystemManager.GetJsonNodeBool(__j["grade"][0], "preview");
        }

        public void SetAchievementList(JsonData __j)
        {
            AchievementData achievement;
            listAchievement = new List<AchievementData>();

            foreach (string key in __j.Keys)
            {
                if (key == "season_check" || key == "grade")
                    continue;

                for (int i = 0; i < __j[key].Count; i++)
                {
                    achievement = new AchievementData(__j[key][i]);
                    listAchievement.Add(achievement);
                }
            }

            MainProfile.OnRefreshIFYOUAchievement?.Invoke();
        }


        /// <summary>
        /// 작품의 갤러리, 미션, 스페셜, 엔딩 모두 오픈했는지 체크
        /// </summary>
        /// <returns></returns>
        public bool ProjectAllClear()
        {
            // 갤러리 모두 획득했는지 체크
            for (int i = 0; i < currentStoryJson[NODE_USER_GALLERY_IMAGES].Count; i++)
            {
                // 공개용 갤러리만 체크한다
                if (!SystemManager.GetJsonNodeBool(currentStoryJson[NODE_USER_GALLERY_IMAGES][i], "is_public"))
                    continue;

                // 아직 오픈하지 못한 것이 있다면 false 처리
                if (!SystemManager.GetJsonNodeBool(currentStoryJson[NODE_USER_GALLERY_IMAGES][i], CommonConst.ILLUST_OPEN))
                    return false;
            }

            // 사운드(보이스) 해금 체크!
            for (int i = 0; i < GetNodeUserRawVoiceHistory().Count; i++)
            {
                // 보이스 열리지 않은게 있으면 완벽주의자가 아니야~~~
                if (!SystemManager.GetJsonNodeBool(GetNodeUserRawVoiceHistory()[i], CommonConst.IS_OPEN))
                    return false;
            }


            // 미션 모두 클리어 한건지 체크
            foreach (int key in DictStoryMission.Keys)
            {
                // 미션이 잠긴게 있다면 false 처리
                if (DictStoryMission[key].missionState == MissionState.locked)
                    return false;
            }


            // 엔딩을 모두 해금했는지 체크
            for (int i = 0; i < StoryManager.main.RegularEpisodeList.Count; i++)
            {
                // 엔딩만 체크
                if (StoryManager.main.RegularEpisodeList[i].episodeType != EpisodeType.Ending)
                    continue;

                // 엔딩이 해금되지 않은게 있다면 false
                if (!StoryManager.main.RegularEpisodeList[i].endingOpen)
                    return false;
            }


            // 사이드 해금 체크
            for (int i = 0; i < StoryManager.main.SideEpisodeList.Count; i++)
            {
                // 사이드 해금되지 않은게 있다면 false
                if (!StoryManager.main.SideEpisodeList[i].isUnlock)
                    return false;
            }

            return true;
        }

        #endregion
    
        #region Rate 평가 팝업
        
        /// <summary>
        /// 평가 팝업 기록 저장하기 
        /// </summary>
        public void UpdateRateHistory(int __result)
        {
            JsonData sendData = new JsonData();
            sendData["func"] = "updateRateHistory";
            sendData["rate_result"] = __result;
            

            NetworkLoader.main.SendPost(CallbackUpdateRateHistory, sendData, false);

        }
        
        /// <summary>
        /// 평가팝업 콜백 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void CallbackUpdateRateHistory(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkLoader.CheckResponseValidation(request, response))
                return;

            diffRate = 0; // 0으로 처리
            
        }
        
        #endregion
        
        /// <summary>
        /// 이프유 패스 사용중인지 체크 
        /// </summary>
        /// <returns></returns>    
        public bool CheckIFyouPassUsing() {
            return ifyouPassDay > 0;
        }
        
        /// <summary>
        /// 이프유 패스 만료 메세지 
        /// </summary>
        /// <returns></returns>
        public string GetIFyouPassExpireMessage() {
            if (ifyouPassDay <= 0)
                return string.Empty;
                
            if(ifyouPassDay >= 30) {
                // 오늘 만료! 메세지 표시 
                return SystemManager.GetLocalizedText("6458");
            }
            else {
                // 몇일 후 만료
                return string.Format(SystemManager.GetLocalizedText("6457"), (30 - ifyouPassDay).ToString());
            }
        }
    }
}
