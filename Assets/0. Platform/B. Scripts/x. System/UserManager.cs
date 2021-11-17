using System;
using System.Text;
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
        

        public JsonData userJson = null; // 계정정보 (table_account) 
        public JsonData bankJson = null; // 유저 소모성 재화 정보 (gem, coin)
        public JsonData notReceivedMailJson = null;     // 미수신 메일


        public List<CoinIndicator> ListCoinIndicators = new List<CoinIndicator>(); // 코인 표시기
        public List<GemIndicator> ListGemIndicators = new List<GemIndicator>(); // 사파이어 표시기
        public TicketIndicator ticketIndicators;        // 1회권 표시기
        public Queue<JsonData> OpenSideStories = new Queue<JsonData>();     // 해금된 사이드 에피소드 목록
        public Queue<JsonData> CompleteMissions = new Queue<JsonData>();      // 완료된 미션 목록


        #region Actions
        public static Action OnCleanUserEpisodeProgress; // 유저 에피소드 씬 진척도 클리어 콜백 
        public static Action<bool> OnRequestEpisodePurchase = null; // 에피소드 구매 콜백처리
        public static Action OnRequestEpisodeReset = null; // 에피소드 리셋 콜백처리
        public static Action OnFreepassPurchase = null; // 프리패스 구매 콜백처리 

        public static Action<int> OnRefreshUnreadMailCount = null; // 미수신 메일 개수 Refresh Action

        #endregion


        #region 데이터 검증 체크용 리스트 
        [SerializeField]
        List<string> DebugProjectIllusts = new List<string>();
        [SerializeField]
        List<string> DebugUserIllusts = new List<string>();

        [SerializeField]
        List<string> DebugProjectChallenges = new List<string>();

        [SerializeField]
        List<string> DebugUserChallenges = new List<string>();

        [SerializeField]
        List<string> DebugProjectFavors = new List<string>();
        [SerializeField]
        List<string> DebugUserFavors = new List<string>();

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
        public JsonData currentStoryJson = null; // 선택한 프로젝트와 관련된 정보 

        public bool completeReadUserData = false;
        public bool episodeRecordComplete = false;

        public float prevIllustProgress = -1f;

        // 사건 기록을 하는 통신을 사용할 것인가?
        // 엔딩 수집 화면에서 사용할 변수
        public bool useRecord = true;       

        public string userKey = string.Empty;
        public string gamebaseID = string.Empty;
        public int tutorialStep = 0;
        public int adCharge = 0;

        public int gem = 0;
        public int coin = 0;
        public int unreadMailCount = 0; // 미수신 메일 카운트

        JsonData resultProjectCurrent = null; // 플레이 위치 

        JsonData resultSceneRecord = null; // 사건ID 기록 통신 결과 
        JsonData resultEpisodeRecord = null; // 에피소드 기록 통신 결과
        JsonData resultEpisodeReset = null; // 에피소드 리셋 통신 결과

        #region static const 

        // getUserSelectedStory를 통해 받아온 작품 관련 정보 

        public const string UN_UNREAD_MAIL_COUNT = "unreadMailCount"; // 미수신 메일 개수
        public const string UN_UNREAD_MAIL_LIST = "mailList"; // 미수신 메일 리스트

        const string NODE_DRESS_CODE = "dressCode"; // dressCode (프로젝트 기준정보)
        
        public const string NODE_BUBBLE_SET = "bubbleSet"; // 말풍선 세트 정보 
        const string NODE_BUBBLE_SPRITE = "bubbleSprite"; // 말풍선 스프라이트 정보 
        const string NODE_PROJECT_MODELS = "models"; // 프로젝트의 모든 모델 파일리스트 
        const string NODE_PROJECT_ILLUSTS = "illusts"; // 프로젝트의 모든 일러스트 
        const string NODE_PROJECT_MISSIONS = "missions";                    // 프로젝트의 모든 미션
        const string NODE_PROJECT_MISSION_PROGRESS = "missionProgress";     // 프로젝트 완료한 미션

        // 사용자 정보 
        const string NODE_USER_FAVOR = "favorProgress"; // 유저 호감도 정보 
        const string NODE_USER_MISSIONS = "missionProgress"; // 유저 도전과제 정보 

        const string NODE_USER_VOICE = "voiceHistory";      // 유저 보이스(더빙) 히스토리 정보
        const string NODE_USER_RAW_VOICE = "rawVoiceHistory"; // 유저 보이스 히스토리 (Raw 타입)
        const string NODE_USER_ILLUSTS = "illustHistory";   // 유저 일러스트 히스토리 정보 
        
        const string NODE_DRESS_PROGRESS = "dressProgress"; // 유저 의상 진행정보 (의상 템플릿 관련)

        const string NODE_PURCHASE_HIST = "episodePurchase";  //episodePurchase 에피소드 구매 기록
        const string NODE_SCENE_PROGRESS = "sceneProgress"; // 사건ID 진행도. 조건 판정에 사용한다. 
        const string NODE_SCENE_HISTORY = "sceneHistory"; // 한번이라도 오픈한 사건 ID 기록 (삭제되지 않음)
        const string NODE_FIRST_CLEAR_RESULT = "firstClearResult";      // 에피소드 최초 클리어 보상
        const string NODE_UNLOCK_SIDE = "unlockSide";           // 해금된 사이드 에피소드
        const string NODE_UNLOCK_MISSION = "unlockMission";     // 해금된 미션

        const string NODE_EPISODE_PROGRESS = "episodeProgress"; // 에피소드 진행도
        const string NODE_EPISODE_HISTORY = "episodeHistory"; // 에피소드 히스토리 

        
        const string NODE_NEXT_EPISODE = "nextEpisodeID"; // 다음 에피소드 정보 
        const string NODE_PROJECT_USER_PROPERTY = "userProperty"; // 유저의 프로젝트 관련 재화 정보(대여권과 자유이용권)
        
        const string NODE_COLLECTION_PROGRESS = "progress"; // 유저 작품 수집요소 Progress 
        const string NODE_PROJECT_CURRENT = "projectCurrent"; // 유저의 작품에서의 플레이 위치 
        const string NODE_SELECTION_PROGRESS = "selectionProgress"; // 선택지 프로그레스 
        
        const string NODE_FREEPASS_TIMEDEAL = "userFreepassTimedeal"; // 유저 프리패스 타임딜
        
        
        const string KEY_SOUND_NAME = "sound_name";
        const string KEY_OPEN = "is_open";

        #endregion


        private void Awake()
        {
            // System(SystemManager)에 귀속이라 Destory 할 필요없다.
            // 다른 씬에서 넘어온 객체가 있을경우. 
            if (main != null)
            {
                return;
            }

            // Singleton
            main = this;

            ListCoinIndicators.Clear();
            ListGemIndicators.Clear();
        }

        /// <summary>
        /// 유저 정보 초기화 
        /// </summary>
        public void InitUser(string __gamebaseID)
        {
            Debug.Log("<color=cyan>Init user info </color>");
            gamebaseID = __gamebaseID;

            // 로그인 프로세스를 시작합니다. 
            ConnectServer();
        }

        /// <summary>
        /// 서버에 접속을 시작합니다. 
        /// </summary>
        void ConnectServer()
        {
            JsonData sendingData = new JsonData(); // 서버 전송 데이터 
            sendingData["func"] = NetworkLoader.FUNC_LOGIN_CLIENT;
            sendingData["deviceid"] = SystemInfo.deviceUniqueIdentifier;
            sendingData["gamebaseid"] = gamebaseID;

            if (NetworkLoader.main == null)
                return;

            NetworkLoader.main.SendPost(CallbackConnectServer, sendingData);
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

            // 소모성 재화 정보 update
            SetBankInfo(userJson);

            // 알림 정보 update
            SetNotificationInfo(userJson);


            // account 정보 
            userJson = userJson["account"];
            userKey = userJson["userkey"].ToString();
            tutorialStep = int.Parse(SystemManager.GetJsonNodeString(userJson, "tutorial_step"));
            adCharge = int.Parse(SystemManager.GetJsonNodeString(userJson, "ad_charge"));


            // 유저 정보 불러왔으면, Lobby로 진입을 요청합니다. 
            completeReadUserData = true;
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_CONNECT_SERVER, string.Empty);
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
            if(result["account"][0]["is_done"].ToString() == "0")
            {
                // 여기 오면... 안되는데 ㅠㅠ
                // Force 하기전에 미리 체크해야 되나?
                Debug.LogError("LogError in CallbackChangeAccount");
                SystemManager.ShowSimpleMessagePopUp("새로 로그인한 IDP 로그인 정보가 서버에 없습니다.");
                return;
            }

            SetRefreshUserInfo(result);            
            
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

            userJson = userJson["account"];
            userKey = userJson["userkey"].ToString();
            tutorialStep = int.Parse(SystemManager.GetJsonNodeString(userJson, "tutorial_step"));

            // 사용자 UI 정보를 갱신하는 Event 필요함!

            // 다했다고 안내
            SystemManager.ShowSimpleMessagePopUp("계정 정보를 불러왔습니다.");
        }

        public void SetRefreshInfo(JsonData __j)
        {
            notReceivedMailJson = __j;

            // 소모성 재화 정보 update
            SetBankInfo(__j);

            // 알림 정보 update
            SetNotificationInfo(__j);
        }

        #region 튜토리얼 Update 통신

        /// <summary>
        /// 튜토리얼을 거치면 단계를 업데이트 해준다
        /// </summary>
        public void UpdateTutorialStep()
        {
            JsonData sending = new JsonData();
            sending["func"] = "updateTutorialStep";
            sending["userkey"] = userKey;
            sending["tutorial_step"] = ++tutorialStep;

            NetworkLoader.main.SendPost(CallbackUpdateTutorialStep, sending);
        }

        void CallbackUpdateTutorialStep(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateTutorialStep");
                return;
            }
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
            unreadMailCount = int.Parse(notReceivedMailJson["unreadMailCount"].ToString());
            SetRefreshInfo(notReceivedMailJson);
        }

        #endregion

        #region 미션(보상)
        
        /// <summary>
        /// 개별 미션 보상 받기
        /// </summary>
        public void GetMissionRewared(string missionId, string currency, string quantity, OnRequestFinishedDelegate callback)
        {
            JsonData sending = new JsonData();
            sending["func"] = NetworkLoader.FUNC_USER_MISSION_REWARD;
            sending["project_id"] = StoryManager.main.CurrentProjectID;
            sending["mission_id"] = missionId;
            sending["userkey"] = userKey;
            sending["reward_currency"] = currency;
            sending["reward_quantity"] = quantity;

            NetworkLoader.main.SendPost(callback, sending, true);
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

        public void AddTicketIndicator(TicketIndicator __receiver)
        {

        }

        /// <summary>
        /// 소모성 재화 세팅 
        /// </summary>
        /// <param name="__j"></param>
        public void SetBankInfo(JsonData __j)
        {
            if (!__j.ContainsKey("bank"))
            {
                Debug.LogError("No Bank Node");
                return;
            }
            
            Debug.Log("Refresh BankInfo");

            bankJson = __j["bank"];

            // 재화 값 갱신
            gem = int.Parse(bankJson["gem"].ToString());
            coin = int.Parse(bankJson["coin"].ToString());

            // 붙어있는 표시기들 refresh 처리 
            RefreshCoinIndicators();
            RefreshGemIndicators();

            if(ticketIndicators != null)
                ticketIndicators.RefreshTicket();
        }

        /// <summary>
        /// 확인이 필요한 유저 정보 세팅
        /// </summary>
        /// <param name="__j"></param>
        public void SetNotificationInfo(JsonData __j)
        {
            // 미수신 메일 개수 
            if(__j.ContainsKey(UN_UNREAD_MAIL_COUNT))
            {
                unreadMailCount = int.Parse(__j[UN_UNREAD_MAIL_COUNT].ToString());
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
            for(int i = ListCoinIndicators.Count-1; i>= 0; i--)
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

        #endregion

        #region 유저 노드 제어 



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

        /// <summary>
        /// 어드민 유저 체크 
        /// </summary>
        /// <returns>true : 어드민 유저임, false : 일반 유저임</returns>
        public bool CheckAdminUser()
        {
            if (!userJson.ContainsKey("admin"))
                return false;

            if (userJson["admin"].ToString() == "1")
                return true;

            return false;
        }

        /// <summary>
        /// 작품의 1회 플레이권 갯수를 반환하세요!
        /// </summary>
        /// <param name="projectId">알고 싶은 작품의 proejctId값</param>
        /// <returns>-1인 경우 아예 해당 작품 1회권이 존재하지 않는 것임</returns>
        public int GetOneTimeProjectTicket(string projectId)
        {
            if (bankJson.ContainsKey("onetime_" + projectId))
                return int.Parse(SystemManager.GetJsonNodeString(bankJson, "onetime_" + projectId));
            else
                return -1;
        }

        /// <summary>
        /// 프로젝트 자유이용권 소유자인가요?
        /// </summary>
        /// <returns></returns>
        public bool HasProjectFreepass()
        {

            if (SystemManager.GetJsonNodeBool(bankJson, "free_" + StoryManager.main.CurrentProjectID))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 프로젝트 연관 재화 갱신(자유이용권, 대여권)
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeProjectUserProperty(JsonData __j)
        {
            currentStoryJson[NODE_PROJECT_USER_PROPERTY] = __j;
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
        /// 해금된 사이드 에피소드
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeUnlockSide()
        {
            return resultEpisodeRecord[NODE_UNLOCK_SIDE];
        }


        /// <summary>
        /// 완료된 미션 리스트 큐에 추가
        /// </summary>
        public void ShowCompleteMission(JsonData __j)
        {
            Debug.LogError("Check ShowCompleteMission");
            /*
            if(__j == null && __j.Count == 0)  {
                Debug.Log("No Clear Mission");
                return;
            }
            
            for(int i=0; i<__j.Count;i++) {
                CompleteMissions.Enqueue(__j[i]);
            }
            
            // Queue 바닥날때까지 돈다. 
            while(CompleteMissions.Count > 0) {
                
                // popup 변수를 여러개가 공유할 수 없음. 매번 새로 만들어줘야된다. 
                UIPopup popUp = UIPopupManager.GetPopup("AchieveCollection");
                if (popUp == null)
                {
                    Debug.LogError("No AchieveMission Popup");
                    return;
                }
                
                JsonData currentMissionData = CompleteMissions.Dequeue(); // 하나씩 빼서 팝업을 만든다. 
                Debug.Log("Mission : " + JsonMapper.ToStringUnicode(currentMissionData)); // 체크용도 
             
             
                // 미션 썸네일과 이름 설정 
                Texture2D t = SystemManager.GetLocalTexture2D(SystemManager.GetJsonNodeString(currentMissionData, "image_key"));

                if (t != null)
                {
                    Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
                    popUp.Data.SetImagesSprites(s);
                }

                // AppsFlyerSDK.AppsFlyer.sendEvent("MISSION_CLEAR_"+ currentMissionData["mission_id"].ToString(), null);
                popUp.Data.SetLabelsTexts(SystemManager.GetLocalizedText("5086"), currentMissionData["mission_name"].ToString(), SystemManager.GetLocalizedText("80078"));
                UIPopupManager.ShowPopup(popUp, true, false);
                Debug.Log("Show Mission Popup");   
            }
            */
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
        


        /// <summary>
        /// 유저 프로젝트별 의상 진행정보 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeDressProgress()
        {
            return currentStoryJson[NODE_DRESS_PROGRESS];
        }
        public void SetNodeDressProgress(JsonData __j)
        {
            currentStoryJson[NODE_DRESS_PROGRESS] = __j;
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
        /// 프로젝트 모델 정보 불러오기 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeProjectModels()
        {
            return currentStoryJson[NODE_PROJECT_MODELS];
        }

        /// <summary>
        /// 프로젝트 일러스트 정보 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeProjectIllusts()
        {
            return currentStoryJson[NODE_PROJECT_ILLUSTS];
        }

        public JsonData GetNodeMissionProgress()
        {
            return currentStoryJson[NODE_PROJECT_MISSION_PROGRESS];
        }

        /// <summary>
        /// 프로젝트 도전과제 정보
        /// </summary>
        public JsonData GetNodeProjectChallenges()
        {
            return currentStoryJson[NODE_PROJECT_MISSIONS];
        }


        /// <summary>
        /// 유저 호감도 기록 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeUserFavor()
        {
            return currentStoryJson[NODE_USER_FAVOR];
        }

        public void SetNodeUserFavor(JsonData __newData)
        {
            currentStoryJson[NODE_USER_FAVOR] = __newData;

            RefreshUserFavorHistoryInspector();
        }

        /// <summary>
        /// 유저 일러스트 오픈 기록
        /// 공개처리된, 최초 등장 에피소드가 입력된 미니컷,라이브 오브제, 일러스트, 라이브 일러스트 포함
        /// </summary>
        /// <returns>illustHistory JsonData</returns>
        public JsonData GetNodeUserIllust()
        {
            return currentStoryJson[NODE_USER_ILLUSTS];
        }

        /// <summary>
        /// 일러스트 이름으로 일러스트 기준정보 찾기
        /// </summary>
        /// <param name="__illustName">일러스트 명칭</param>
        /// <returns>일러스트 id와 type을 담은 JsonData</returns>
        public JsonData GetIllustData(string __illustName)
        {
            for(int i=0;i<GetNodeUserIllust().Count;i++)
            {
                // 일러스트 명칭과 동일한 jsonData값을 찾아 id와 type값을 넣어서 return 해준다
                // 4개 종류가 통합되었기 때문에 is_minicut 값도 체크한다.
                if(GetNodeUserIllust()[i]["illust_name"].ToString().Equals(__illustName) 
                    && !SystemManager.GetJsonNodeBool(GetNodeUserIllust()[i], "is_minicut"))
                {
                    JsonData data = new JsonData();
                    data.Add(GetNodeUserIllust()[i]["illust_id"]);
                    data.Add(GetNodeUserIllust()[i]["illust_type"]);
                    data.Add(GetNodeUserIllust()[i]["public_name"]);
                    Debug.Log(JsonMapper.ToStringUnicode(data));
                    return data;
                }
            }
            return null;
        }

        /// <summary>
        /// 유저 일러스트(갤러리 히스토리) json  업데이트
        /// </summary>
        /// <param name="__newData"></param>
        public void SetNodeUserIllust(JsonData __newData)
        {
            currentStoryJson[NODE_USER_ILLUSTS] = __newData;

            RefreshUserIllustHistoryInspector();
        }

        


        /// <summary>
        /// 갤러리 미니컷, 라이브 오브젝트의 ID 찾기 
        /// </summary>
        /// <param name="minicutName">ScriptRow의 scriptData로 들어간 값</param>
        /// <returns>해당 미니컷 id</returns>
        public string GetGalleryMinicutID(string __minicutName, bool isLive2D)
        {
            
            // illust_type
            string illustType = "minicut";
            
            if(isLive2D)
                illustType = "live2d";
            
            for(int i=0;i<GetNodeUserIllust().Count;i++)
            {
                
                // 미니컷이면서, 이름 똑같은 친구
                if(SystemManager.GetJsonNodeBool(GetNodeUserIllust()[i], "is_minicut")
                    && SystemManager.GetJsonNodeString(GetNodeUserIllust()[i], "illust_name") == __minicutName
                    && SystemManager.GetJsonNodeString(GetNodeUserIllust()[i], "illust_type") == illustType) {
                        
                    // id 리턴
                    return SystemManager.GetJsonNodeString(GetNodeUserIllust()[i], "illust_id");
                }

            }

            return string.Empty;
        }
        
        
        /// <summary>
        /// 미니컷 이름 (단순 이미지만!)
        /// </summary>
        /// <param name="__name"></param>
        /// <returns></returns>
        public JsonData GetPublicMinicutJsonByName(string __name) {
            for(int i=0;i<GetNodeUserIllust().Count;i++)
            {
                if(SystemManager.GetJsonNodeString(GetNodeUserIllust()[i], "illust_name") == __name
                    && SystemManager.GetJsonNodeBool(GetNodeUserIllust()[i], "is_minicut")) 
                {
                    return GetNodeUserIllust()[i];
                }
            }

            return null;
        }


        /// <summary>
        /// 유저 도전과제 기록 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeUserChallenges()
        {
            return currentStoryJson[NODE_USER_MISSIONS];
        }

        public void SetNodeUserChallenge(JsonData __newData)
        {
            currentStoryJson[NODE_USER_MISSIONS] = __newData;

            RefreshUserChallengeHistoryInspector();
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
        public JsonData GetNodeUserRawVoiceHistory() {
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
        /// 유저 작품별 컬렉션 진행도 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeUserCollectionProgress() {
            
            
            
            if(currentStoryJson == null || !currentStoryJson.ContainsKey(NODE_COLLECTION_PROGRESS))
                return null;
            
            return currentStoryJson[NODE_COLLECTION_PROGRESS];
        }
        
        
        /// <summary>
        /// 미션 진행율
        /// </summary>
        /// <returns></returns>
        public float GetCollectionMissionProgress() {
            if(GetNodeUserCollectionProgress().ContainsKey("mission"))  {
                return float.Parse(GetNodeUserCollectionProgress()["mission"].ToString());
            }
            
            return 0;
        }
        
        /// <summary>
        /// 엔딩 진행율 
        /// </summary>
        /// <returns></returns>
        public float GetCollectionEndingProgress() {
            if(GetNodeUserCollectionProgress().ContainsKey("ending"))  {
                return float.Parse(GetNodeUserCollectionProgress()["ending"].ToString());
            }
            
            return 0;
        }
        
        /// <summary>
        /// 갤러리 진행율 
        /// </summary>
        /// <returns></returns>
        public float GetCollectionGalleryProgress() {
            if(GetNodeUserCollectionProgress().ContainsKey("gallery"))  {
                return float.Parse(GetNodeUserCollectionProgress()["gallery"].ToString());
            }
            
            return 0;
        }
        
        
        /// <summary>
        /// 플레이 위치 노드 저장 
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeUserProjectCurrent(JsonData __j) {
            currentStoryJson[NODE_PROJECT_CURRENT] = __j;
        }
        
        /// <summary>
        /// 유저, 작품별 플레이 위치 찾기. 
        /// 정규 에피소드 용도 
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserProjectCurrent(string __episodeID) {
            
            if(!currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT))
                return null;
                
            for(int i=0; i<currentStoryJson[NODE_PROJECT_CURRENT].Count; i++) {
                if(currentStoryJson[NODE_PROJECT_CURRENT][i]["episode_id"].ToString() == __episodeID)
                    return currentStoryJson[NODE_PROJECT_CURRENT][i];
            }            
            
            // return currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT) ? currentStoryJson[NODE_PROJECT_CURRENT] : null;
            
            return null;
        }
        
        /// <summary>
        /// 정규 에피소드의 Current를 찾는다.
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserProjectRegularEpisodeCurrent() {
            // 얘는 순서에 영향을 받지 않는다. 리스트에서만 사용한다.
            
            if(!currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT))
                return null;
                
            for(int i=0; i<currentStoryJson[NODE_PROJECT_CURRENT].Count; i++) {
                if(currentStoryJson[NODE_PROJECT_CURRENT][i]["is_special"].ToString() == "0") // is_special 이니?
                    return currentStoryJson[NODE_PROJECT_CURRENT][i];
            }            
           
            return null;
            
        }
        
        /// <summary>
        /// 정규 에피소드 끝났니? 
        /// </summary>
        /// <returns></returns>
        public bool CheckUserProjectRegularEpisodeFinal() {
            
            if(!currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT))
                return false;
                
            for(int i=0; i<currentStoryJson[NODE_PROJECT_CURRENT].Count; i++) {
                if(currentStoryJson[NODE_PROJECT_CURRENT][i]["is_special"].ToString() == "0") { // is_special 이니?
                    return SystemManager.GetJsonNodeBool(currentStoryJson[NODE_PROJECT_CURRENT][i], "is_final");
                }
            }            
           
            return false;
        }
        
        
        /// <summary>
        /// 스페셜 에피소드의 Current를 찾는다.
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserProjectSpecialEpisodeCurrent() {
            // 얘는 순서에 영향을 받지 않는다. 리스트에서만 사용한다.
            
            if(!currentStoryJson.ContainsKey(NODE_PROJECT_CURRENT))
                return null;
                
            for(int i=0; i<currentStoryJson[NODE_PROJECT_CURRENT].Count; i++) {
                if(currentStoryJson[NODE_PROJECT_CURRENT][i]["is_special"].ToString() == "1") // is_special 이니?
                    return currentStoryJson[NODE_PROJECT_CURRENT][i];
            }            
           
            return null;
            
        }
        
        /// <summary>
        /// 작품 선택지 선택 진행도 노드 저장 
        /// </summary>
        /// <param name="__j"></param>
        public void SetNodeUserProjectSelectionProgress(JsonData __j) {
            currentStoryJson[NODE_SELECTION_PROGRESS] = __j;
        }
        
        /// <summary>
        /// 프로젝트 선택지 프로그레스 
        /// Key는 episodeID
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserProjectSelectionProgress(string __episodeID) {
            
            if(!currentStoryJson.ContainsKey(NODE_SELECTION_PROGRESS))
                return null;
                
            if(!currentStoryJson[NODE_SELECTION_PROGRESS].ContainsKey(__episodeID))
                return null;
                
            return currentStoryJson[NODE_SELECTION_PROGRESS][__episodeID];
        }
        
        /// <summary>
        /// 유저의 작품에서 첫번째 selection 터치 체크 
        /// </summary>
        /// <returns>true : 첫번째다.</returns>
        public bool IsUserFirstSelection() {
            if(!currentStoryJson.ContainsKey(NODE_SELECTION_PROGRESS))
                return true;
                
            if(currentStoryJson[NODE_SELECTION_PROGRESS].Keys.Count == 0)
                return true;
                
            return false;
        }
        
        
        /// <summary>
        /// 대상 에피소드에 target_scene_id를 가진 선택지 Progress 체크 
        /// </summary>
        /// <param name="__episodeID"></param>
        /// <param name="__targetSceneID"></param>
        /// <returns></returns>
        public bool CheckProjectSelectionProgressExists(string __episodeID, string __targetSceneID) {
            
            JsonData targetEpisode = GetUserProjectSelectionProgress(__episodeID);
            
            if(targetEpisode == null)
                return false;
            
            // 에피소드별 Progress를 체크해서 ... 비교 
            for(int i=0; i<targetEpisode.Count;i++) {
                if(targetEpisode[i]["target_scene_id"].ToString() == __targetSceneID)
                    return true;
            }
            
            // "target_scene_id": "1021",
            // "selection_data": "남을 공격하는 힘을 얻어야겠어."
            
            return false;
        }
        
        
        


        #endregion

        #region 통신 콜백 
        
        /// <summary>
        /// 프리패스 구매 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackPurchaseFreepass(HTTPRequest req, HTTPResponse res) {
            
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackPurchaseFreepass");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText); // 결과 
            JsonData purchaseResult = result.ContainsKey("purchaseResult")?result["purchaseResult"]:null;
            // 갱신
            
            // * "userProperty" (프로젝트 귀속 Property)
            SetNodeProjectUserProperty(result[NODE_PROJECT_USER_PROPERTY]); 
            
            // * bank 
            SetBankInfo(result);
            
            
            
            // StoryDetail 갱신처리 
            OnFreepassPurchase?.Invoke();
            
            // View 종료를 위해 Event 처리 
            // Doozy.Engine.GameEventMessage.SendEvent("PurchaseFreepass"); 
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_PURCHASE_FREEPASS, string.Empty);
            SystemManager.ShowSimpleMessagePopUp(string.Format(SystemManager.GetLocalizedText("80061"), StoryManager.main.CurrentProjectTitle));
            
            
            // AppsFlyer
            if(purchaseResult == null) {
                return;
            }
            
            string eventName = "USER_PURCHASE_FREEPASS_" + StoryManager.main.CurrentProjectID;
            Dictionary<string, string> appsFlayerParam = new Dictionary<string, string>();
            appsFlayerParam.Add("freepass_no", SystemManager.GetJsonNodeString(purchaseResult, "freepass_no"));
            appsFlayerParam.Add("origin_price", SystemManager.GetJsonNodeString(purchaseResult, "originPrice"));
            appsFlayerParam.Add("sale_price", SystemManager.GetJsonNodeString(purchaseResult, "salePrice"));
            
            // AppsFlyerSDK.AppsFlyer.sendEvent(eventName,appsFlayerParam);
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
        /// 사건ID 히스토리, 진행도 업데이트 후 갱신 처리 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void CallbackUpdateSceneRecord(HTTPRequest req, HTTPResponse res)
        {
            // 통신 실패했을 때 갱신하지 않음. 
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateSceneRecord");
                return;
            }

            resultSceneRecord = JsonMapper.ToObject(res.DataAsText); // 결과 

            // 갱신해서 받아온 데이터 설정
            SetNodeProjectSceneHistory(resultSceneRecord[NODE_SCENE_HISTORY]);
            SetNodeStorySceneProgress(resultSceneRecord[NODE_SCENE_PROGRESS]);

            // 완료된 미션이 있다면 큐에 넣기
            ShowCompleteMission(resultSceneRecord[NODE_UNLOCK_MISSION]);
        }

        /// <summary>
        /// 유저 에피소드 기록 업데이트 콜백
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackUpdateEpisodeRecord(HTTPRequest req, HTTPResponse res)
        {
            // 통신 실패했을 때 갱신하지 않음. 
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateEpisodeRecord");
                episodeRecordComplete = false;
                return;
            }
            
            Debug.Log(">> CallbackUpdateEpisodeRecord : " + res.DataAsText);
            resultEpisodeRecord = JsonMapper.ToObject(res.DataAsText);

            // 노드 저장!
            SetNodeUserEpisodeHistory(resultEpisodeRecord[NODE_EPISODE_HISTORY]); // 히스토리 
            SetNodeUserEpisodeProgress(resultEpisodeRecord[NODE_EPISODE_PROGRESS]); // 진행도 

            ShowCompleteMission(resultEpisodeRecord[NODE_UNLOCK_MISSION]);

            // 다음 에피소드 정보 
            // SetNodeUserNextEpisode(resultEpisodeRecord[NODE_NEXT_EPISODE]);
            
            // played scene count 업데이트 
            if(resultEpisodeRecord.ContainsKey("playedSceneCount")) {
                Debug.Log(JsonMapper.ToStringUnicode(resultEpisodeRecord["playedSceneCount"]));
                Debug.Log("Check this method : CallbackUpdateEpisodeRecord"); 
                // ViewGameEnd.UpdateCurrentEpisodeSceneCount(resultEpisodeRecord["playedSceneCount"][0]);
                Signal.Send(LobbyConst.STREAM_GAME, LobbyConst.SIGNAL_UPDATE_EPISODE_SCENE_COUNT, resultEpisodeRecord["playedSceneCount"][0]);
            }

            episodeRecordComplete = true;
        }

        /// <summary>
        /// 에피소드 시작 기록 업데이트 콜백 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackUpdateEpisodeStartRecord(HTTPRequest req, HTTPResponse res)
        {
            // 통신 실패했을 때 갱신하지 않음. 
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateEpisodeRecord");
                return;
            }

            resultEpisodeRecord = JsonMapper.ToObject(res.DataAsText);

            // Progress, 현재 에피소드 정보 저장 
            SetNodeUserEpisodeProgress(resultEpisodeRecord[NODE_EPISODE_PROGRESS]);
            
        }

        /// <summary>
        /// 에피소드 리셋 프로그레스 
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
            Debug.Log(JsonMapper.ToStringUnicode(resultEpisodeReset));

            // ! 삭제 대상 
            // SetNodeUserNextEpisode(resultEpisodeReset[NODE_NEXT_EPISODE]); // 다음 에피소드 

            // ! 삭제 대상 아님 
            SetNodeUserEpisodeProgress(resultEpisodeReset[NODE_EPISODE_PROGRESS]); // 에피소드 progress 
            SetNodeStorySceneProgress(resultEpisodeReset[NODE_SCENE_PROGRESS]); // 씬 progress
            
            SetNodeUserProjectCurrent(resultEpisodeReset[NODE_PROJECT_CURRENT]);  // projectCurrent
            SetNodeUserProjectSelectionProgress(resultEpisodeReset[NODE_SELECTION_PROGRESS]); // 선택지 기록 

            // 다했으면 후속  처리 시작!
            OnRequestEpisodeReset?.Invoke();
            
            
            // 알림 팝업 후 목록화면 갱신처리 
            SystemManager.ShowSimpleMessagePopUp("선택한 시점으로 이야기가 초기화 되었습니다.");

            // Doozy Nody StoryDetail로 돌아가기 위한 이벤트 생성 
            // Doozy.Engine.GameEventMessage.SendEvent("EventCloseEpisodeReset");
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_CLOSE_RESET, string.Empty);
            

        }


        /// <summary>
        /// 에피소드 구매&대여 결과 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackPurchaseEpisode(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
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
                
            // 유저 프로젝트 연결 재화 
            if(responseData.ContainsKey(NODE_PROJECT_USER_PROPERTY)) {
                SetNodeProjectUserProperty(responseData[NODE_PROJECT_USER_PROPERTY]);
            }

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
            
            
            SetNodeUserIllust(responseData[NODE_USER_ILLUSTS]);
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
            SetNodeUserIllust(responseData[NODE_USER_ILLUSTS]);
        }


        #endregion

        #region 사용자 에피소드 관련 메소드



        /// <summary>
        /// 선택한 스토리에서 유저 데이터 분리해서 저장 
        /// </summary>
        /// <param name="__j"></param>
        public void SetStoryUserData(JsonData __j)
        {
            currentStoryJson = __j;
            // Debug.Log("SetUserEpisodeSceneHistory " + JsonMapper.ToJson(currentStoryJson));


            /// 데이터 확인용도 
            if (!Application.isEditor)
                return;

            DebugProjectIllusts.Clear();
            DebugProjectChallenges.Clear();
            DebugProjectFavors.Clear();
            

            for (int i=0; i<GetNodeProjectIllusts().Count; i++) {
                DebugProjectIllusts.Add(JsonMapper.ToStringUnicode(GetNodeProjectIllusts()[i]));
            }

            for (int i = 0; i < GetNodeProjectChallenges().Count; i++)
            {
                DebugProjectChallenges.Add(JsonMapper.ToStringUnicode(GetNodeProjectChallenges()[i]));
            }

            
           

            RefreshUserIllustHistoryInspector();
            RefreshUserChallengeHistoryInspector();
            RefreshUserFavorHistoryInspector();
        }

        /// <summary>
        /// 완료된 미션이었는지 체크한다
        /// </summary>
        /// <returns>true를 리턴하면 완료한 미션, false를 return하면 아직 미완료한 미션</returns>
        public bool CheckCompleteMission(string missionName)
        {
            for(int i=0;i<GetNodeMissionProgress().Count;i++)
            {
                if (GetNodeMissionProgress()[i]["mission_name"].ToString().Equals(missionName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 미션 이름을 비교하여 mission Id를 반환한다
        /// </summary>
        public string GetMissionId(string missionName)
        {
            for(int i=0;i<GetNodeProjectChallenges().Count;i++)
            {
                if (GetNodeProjectChallenges()[i]["mission_name"].ToString().Equals(missionName))
                    return GetNodeProjectChallenges()[i]["mission_id"].ToString();
            }

            return string.Empty;
        }

        #region 인스펙터 체크용!

        /// <summary>
        /// 유저 호감도 인스펙터 확인용!
        /// </summary>
        void RefreshUserFavorHistoryInspector()
        {
            if (!Application.isEditor)
                return;

            DebugUserFavors.Clear();
            for(int i=0; i<GetNodeUserFavor().Count;i++)
            {
                DebugUserFavors.Add(JsonMapper.ToStringUnicode(GetNodeUserFavor()[i]));
            }
        }


        // 인스펙터 확인용도!
        void RefreshUserIllustHistoryInspector()
        {

            if (!Application.isEditor)
                return;
            
            DebugUserIllusts.Clear();
            for (int i = 0; i < GetNodeUserIllust().Count; i++)
            {
                DebugUserIllusts.Add(JsonMapper.ToStringUnicode(GetNodeUserIllust()[i]));
            }
        }

        void RefreshUserChallengeHistoryInspector()
        {
            if (!Application.isEditor)
                return;

            DebugUserChallenges.Clear();
            for (int i = 0; i < GetNodeUserChallenges().Count; i++)
            {
                DebugUserChallenges.Add(JsonMapper.ToStringUnicode(GetNodeUserChallenges()[i]));
            }
        }

        #endregion

        #region 유저 의상 변경점 업데이트 
        public void UpdateDressProgress(string __speaker, string __dress_id)
        {
            JsonData j = new JsonData();

            j["project_id"] = StoryManager.main.CurrentProjectID;
            j["speaker"] = __speaker;
            j["dress_id"] = __dress_id;
            j["func"] = NetworkLoader.FUNC_INSERT_DRESS_PROGRESS;


            NetworkLoader.main.SendPost(OnUpdateDressProgress, j);
        }

        void OnUpdateDressProgress(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("OnUpdateDressProgress");
                return;
            }

            // 갱신해서 받아온 데이터를 설정 
            SetNodeDressProgress(JsonMapper.ToObject(res.DataAsText));
        }
        #endregion




        /// <summary>
        /// 현재의 사건 ID를 기록에 추가한다. 
        /// 사건 History, 사건 Progress 같이 추가된다. 
        /// </summary>
        /// <param name="__project_id"></param>
        /// <param name="__episode_id"></param>
        /// <param name="__scene_id"></param>
        public void UpdateSceneIDRecord(string __project_id, string __episode_id, string __scene_id)
        {
            
            // * 여기도, 이어하기를 통해 진입한 경우 통신하지 않음
            // * 마지막 지점에 도착하면 isResumePlay는 false로 변경한다. 
            // 수집 엔딩 보는 중이어도 통신하지 않음
            /*
            if(GameManager.isResumePlay || !useRecord)
                return;
            */
            
            JsonData j = new JsonData();

            // j["func"] = NetworkLoader.func;
            // UserManager.main.UpdateCurrentSceneID(StoryManager.main.CurrentProjectID, StoryManager.main.CurrentEpisodeID, scene_id);

            j["func"] = NetworkLoader.FUNC_UPDATE_EPISODE_SCENE_RECORD;
            j["userkey"] = userKey;
            j["project_id"] = __project_id;
            j["episode_id"] = __episode_id;
            j["scene_id"] = __scene_id;

            NetworkLoader.main.SendPost(CallbackUpdateSceneRecord, j);
        }

        public void UpdateSceneIDRecord(string __scene_id)
        {
            UpdateSceneIDRecord(StoryManager.main.CurrentProjectID, StoryManager.main.CurrentEpisodeID, __scene_id);
        }

        






        /// <summary>
        /// 지정한 사건ID를 Progress에서 제거합니다.
        /// </summary>
        /// <param name="__scene_id"></param>
        public void DeleteSceneID(string __scene_id, string __project_id)
        {
            JsonData j = new JsonData();

            j["func"] = NetworkLoader.FUNC_DELETE_EPISODE_SCENE_HISTORY;
            j["scene_id"] = __scene_id;
            j["project_id"] = __project_id;

            NetworkLoader.main.SendPost(CallbackUpdateCurrentSceneID, j);
        }

        /// <summary>
        /// 유저별 프로젝트 기록에 대상 사건ID 추가 
        /// </summary>
        /// <param name="__sceneID"></param>
        public void AddSceneToUserProjectSceneHistory(string __sceneID)
        {
            Debug.Log(">> AddSceneToUserProjectSceneHistory : " + __sceneID);

            if (!CheckSceneHistory(__sceneID))
            {
                GetNodeProjectSceneHistory().Add(__sceneID);

            }

            Debug.Log(JsonMapper.ToStringUnicode(GetNodeProjectSceneHistory()));
        }




        /// <summary>
        /// 지정한 사건ID의 업데이트 후 갱신합니다. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void CallbackUpdateCurrentSceneID(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req,res))
            {
                Debug.LogError("CallbackClearSelectedEpisodeSceneHistory");
                return;
            }

            string requestData = Encoding.UTF8.GetString(req.RawData);
            Debug.Log(">> CallbackUpdateCurrentSceneID : " + requestData);
            JsonData reqJson = JsonMapper.ToObject(requestData);
             

            // 갱신해서 받아온 데이터를 설정 
            SetNodeStorySceneProgress(JsonMapper.ToObject(res.DataAsText));
            
            // 히스토리에 추가한다. 
            AddSceneToUserProjectSceneHistory(reqJson["scene_id"].ToString());
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
            if(!NetworkLoader.CheckResponseValidation(req, res))
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

            for(int i=0; i<GetNodeUserEpisodeProgress().Count;i++)
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

            Debug.Log(string.Format("CheckSceneProgress : [{0}]", __scene_id));

            for(int i=0; i<GetNodeStorySceneProgress().Count;i++)
            {
                if (GetNodeStorySceneProgress()[i].ToString() == __scene_id)
                    return true;
            }

            Debug.Log(string.Format("{0} 히스토리 없음", __scene_id));
            return false;
        }

        /// <summary>
        /// 유저의 사건ID 히스토리 유무 체크
        /// </summary>
        /// <param name="__sceneID">사건 ID</param>
        public bool CheckSceneHistory(string __sceneID)
        {
            // 어드민 유저 무조건 true 
            if(CheckAdminUser())
                return true; 
            
            
            if (currentStoryJson == null || GetNodeProjectSceneHistory() == null)
            {
                Debug.Log("<color=orange>GetNodeProjectSceneHistory is empty</color>");
                return false;
            }

            for(int i=0; i<GetNodeProjectSceneHistory().Count;i++)
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

        #endregion

        /// <summary>
        /// 현재 일러스트가 신규 일러스트인지 체크한다
        /// </summary>
        /// <param name="__illustID">일러스트 ID</param>
        /// <param name="__illustType">일러스트 타입(illust/live2d)</param>
        /// <returns></returns>
        public bool CheckNewIllustUnlock(string __illustID, string __illustType)
        {
            // 노드 루프돌면서 오픈된 기록이 있는지 체크한다.
            for(int i=0; i< GetNodeUserIllust().Count;i++)
            {
                if (GetNodeUserIllust()[i]["illust_id"].ToString() == __illustID
                    && GetNodeUserIllust()[i]["illust_type"].ToString() == __illustType
                    && GetNodeUserIllust()[i]["illust_open"].ToString() == "1" 
                    && GetNodeUserIllust()[i]["is_minicut"].ToString() == "0")
                    return true;
            }

            return false;
        }

        
        /// <summary>
        /// Gallery Image의 progressor 값 계산
        /// </summary>
        /// <param name="__j">episode, side의 galleryImage key 값이 입력되는 jsonData를 받아온다</param>
        /// <returns>gallery Image의 퍼센테이지 계산 결과값</returns>
        public float CalcGalleryImage(JsonData __j)
        {
            float galleryValue = -1f;

            if(__j.Count > 0)
            {
                float getGallery = 0f;

                for(int i=0;i < __j.Count;i++)
                {
                    if (CheckGalleryImage(SystemManager.GetJsonNodeString(__j[i], "illust_id")))
                        getGallery += 1f;
                }

                galleryValue = getGallery / __j.Count;
            }

            return galleryValue;
        }

        /// <summary>
        /// 갤러리 이미지를 획득 했는지 체크
        /// </summary>
        /// <param name="__illustId">illust, live illust, minicut, live object id값</param>
        bool CheckGalleryImage(string __galleryId)
        {

            for (int i = 0; i < GetNodeUserIllust().Count; i++)
            {
                if (GetNodeUserIllust()[i]["illust_id"].ToString().Equals(__galleryId) && GetNodeUserIllust()[i]["illust_open"].ToString().Equals("1"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 필요한 가격만큼 보석을 보유하고 있는지 체크 
        /// </summary>
        /// <param name="__requirePrice"></param>
        /// <returns></returns>
        public bool CheckGemProperty(int __requirePrice)
        {
            if(CheckAdminUser())
                return true;
            
            return __requirePrice <= gem;
        }

        /// <summary>
        /// 필요한 가격만큼 코인을 보유하고 있는지 체크 
        /// </summary>
        /// <param name="__requirePrice"></param>
        /// <returns></returns>
        public bool CheckCoinProperty(int __requirePrice)
        {
            
            if(CheckAdminUser())
                return true;
            
            return __requirePrice <= coin;
        }

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
            for(int i=0; i < GetNodeUserRawVoiceHistory().Count;i++) {
                if(SystemManager.GetJsonNodeString(GetNodeUserRawVoiceHistory()[i], KEY_SOUND_NAME) == soundName 
                    && SystemManager.GetJsonNodeBool(GetNodeUserRawVoiceHistory()[i], KEY_OPEN)) {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 대상 보이스를 해금 처리 
        /// </summary>
        /// <param name="soundName"></param>
        public void SetVoiceOpen(string soundName) {
            for(int i=0; i < GetNodeUserRawVoiceHistory().Count;i++) {
                if(SystemManager.GetJsonNodeString(GetNodeUserRawVoiceHistory()[i], KEY_SOUND_NAME) == soundName) {
                    GetNodeUserRawVoiceHistory()[i][KEY_OPEN] = 1; // 오픈처리
                    // AppsFlyerSDK.AppsFlyer.sendEvent("VOICE_ACQUIRE_[" + SystemManager.GetJsonNodeString(GetNodeUserRawVoiceHistory()[i], "speaker") + " ]_[" + SystemManager.GetJsonNodeString(GetNodeUserRawVoiceHistory()[i], "sound_id") + "]", null);
                    return;
                }
            }
        }

        /// <summary>
        /// 해금된 이미지(라이브 오브제)인지 check
        /// </summary>
        /// <param name="imageName">이미지(라이브오브제) 이름</param>
        public bool CheckMinicutUnlockable(string imageName, bool isLive2D)
        {
            // illust_type
            string illustType = "minicut";
            
            if(isLive2D)
                illustType = "live2d";
            
            // 노드 루프돌면서 오픈된 기록이 있는지 체크한다.
            for(int i=0;i< GetNodeUserIllust().Count;i++)
            {
                    if(SystemManager.GetJsonNodeString(GetNodeUserIllust()[i], "illust_name") == imageName
                        && SystemManager.GetJsonNodeString(GetNodeUserIllust()[i], "illust_type") == illustType
                        && SystemManager.GetJsonNodeBool(GetNodeUserIllust()[i], "is_minicut")
                        && !SystemManager.GetJsonNodeBool(GetNodeUserIllust()[i], "illust_open")) {
                        
                        return true; // 해금 가능함
                    }
            }

            return false; // 해금 못해! 
        }

        /// <summary>
        /// 에피소드 타입과 에피소드 id로 해당 에피소드 데이터를 전달해주는 함수
        /// </summary>
        public JsonData FindEpisodeData(string episodeType, string episodeId)
        {
            // 에피소드 타입이 스페셜 에피소드인 경우
            if(episodeType.Equals("side"))
            {
                for(int i=0;i<currentStoryJson["sides"].Count;i++)
                {
                    if (currentStoryJson["sides"][i]["episode_id"].ToString().Equals(episodeId))
                        return currentStoryJson["sides"][i];
                }
            }
            else
            {
                // 정규, 엔딩인 경우
                for (int i = 0; i < currentStoryJson["episodes"].Count; i++)
                {
                    if (currentStoryJson["episodes"][i]["episode_id"].ToString().Equals(episodeId))
                        return currentStoryJson["episodes"][i];
                }

            }

            return null;
        }
        
        /// <summary>
        /// 유저 프리패스 타임딜 목록 가져오기
        /// </summary>
        /// <returns></returns>
        public JsonData GetUserFreepassTimedeal() {
            if(!currentStoryJson.ContainsKey(NODE_FREEPASS_TIMEDEAL)) {
                Debug.Log("GetUserFreepassTimedeal, No Node");
                return null;
            }
            
            return currentStoryJson[NODE_FREEPASS_TIMEDEAL];
        }
        
        /// <summary>
        /// 유저 프리패스 타임딜 입력
        /// </summary>
        /// <param name="__data"></param>
        public void SetUserFreepassTimedeal(JsonData __data) {
            currentStoryJson[NODE_FREEPASS_TIMEDEAL] = __data;
        }
    }
}