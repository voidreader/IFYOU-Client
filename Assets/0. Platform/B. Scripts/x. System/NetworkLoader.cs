using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using BestHTTP;
using Toast.Gamebase;

namespace PIERStory
{

    public class NetworkLoader : MonoBehaviour
    {
        public static NetworkLoader main = null;
        
        public static Action<JsonData> OnEpisodeGameData = null;

        #region CONST
        const string HEADER_RETRY = "X-retry";

        public const string FUNC_LOGIN_CLIENT = "loginClient";
        public const string FUNC_SELECTED_STORY_INFO = "getUserSelectedStory";
        
        public const string FUNC_SELECT_LOBBY_PROJECT_LIST = "getIfYouProjectList";
        
        public const string FUNC_UPDATE_EPISODE_SCENE_RECORD = "updateUserProjectSceneHist"; // 씬 경험 기록

        public const string FUNC_CLEAR_EPISODE_SCENE_HISTORY = "clearUserEpisodeSceneHistory";
        
        

        public const string FUNC_UPDATE_EPISODE_COMPLETE_RECORD = "requestCompleteEpisodeType2"; // 에피소드 플레이 완료 기록 
        public const string FUNC_EPISODE_COMPLETE = "requestCompleteEpisodeOptimized"; // 에피소드 플레이 완료 기록 


        public const string FUNC_RESET_EPISODE_PROGRESS_TYPE2 = "resetUserEpisodeProgressType2"; // 에피소드 진행도 리셋 신규 15버전 2022.02.28 

        public const string FUNC_UPDATE_USER_VOICE_HISTORY = "updateUserVoiceHistory";      // 보이스 해금 갱신
        public const string FUNC_UPDATE_USER_ILLUST_HISTORY = "updateUserIllustHistory";    // 일러스트 해금 갱신
        public const string FUNC_UPDATE_USER_MINICUT_HISTORY = "updateUserMinicutHistoryVer2";  // 미니컷 해금 갱신
        
        public const string FUNC_UPDATE_USER_SCRIPT_MISSION = "updateUserScriptMission";    // Drop미션 통신
        public const string FUNC_CHANGE_ACCOUNT_GAMEBASE = "changeAccountByGamebase";

        public const string FUNC_EPISODE_PURCHASE = "purchaseEpisodeType2";
        public const string FUNC_ACCQUIRE_CONSUMABLE_CURRENCY = "accquireUserConsumableCurrency"; // 소모성 재화 획득 
        public const string FUNC_CONSUME_CURRENCY = "consumeUserCurrency"; // 소모성 재화 소모하기 

        // 메일함 관련!
        public const string FUNC_MAIL_LIST = "getUserUnreadMailList"; // 메일함 리스트 조회하기 
        public const string FUNC_MAIL_SINGLE_READ = "requestReceiveSingleMail"; // 메일 하나만 읽기
        public const string FUNC_MAIL_ALL_READ = "requestReceiveAllMail"; // 메일 다 읽기!
        


        // 미션 관련
        public const string FUNC_USER_MISSION_REWARD = "getUserMisionReward";   // 미션 개별 보상
        


        // 게임베이스 Launching 정보 조회 
        const string FUNC_GAMEBASE_LAUNCHING = "https://api-lnc.cloud.toast.com/launching/v3.0/appkeys/tcKCFua98jaMTiae/configurations";
        #endregion
        
        // * 기본 파라매터 
        const string COL_BUILD = "build";
        
        const string COL_COUNTRY = "country";
        const string COL_OS = "os";


        [SerializeField] int failCount = 0; // 통신 실패 카운트 
        [SerializeField] bool isFailMessageShow = false; // 통신 실패 메세지 오픈 여부 
        
        [SerializeField] int downloadFailCount = 0; // 다운로드 실패 카운트
        [SerializeField] bool isDownloadFailMessageShow = false; // 다운로드 실패 메세지 오픈 여부 

        [SerializeField] string _url = string.Empty; // 서버 URL 
        [SerializeField] string _requestURL = string.Empty; // REQUEST URL

        static JsonData _reqData = null; // request rawData 확인용도

        // 네트워크 통신 목록 전송시에 Add, 응답시에 remove. (sync 처리를 위해  추가)
        public List<string> ListNetwork = new List<string>(); 
        static string _requestRawData = string.Empty;

        public bool isFirstEpisode = false;
        public int resetTargetEpisodeId = -1;

        public bool seasonCalculating = false;      // true = 시즌 정산중, false = 시즌 진행중

        private void Awake()
        {
            // 다른 씬에서 넘어온 객체가 있을경우. 
            if (main != null)
            {
                Destroy(this);
                return;
            }

            // Singleton
            main = this;
            DontDestroyOnLoad(this);
        }

        
        public void SetURL(string __url) {
            _url = __url;
            Debug.Log("Set URL : " + __url);
        }

        #region 결제 관련 통신
        
        /// <summary>
        /// 유저의 유료상품 구매내역 
        /// </summary>
        public void RequestUserPurchaseHistory() {
            JsonData sending = new JsonData();
            sending["func"] = "getUserPurchaseListVer2";
            
            SendPost(BillingManager.main.OnRequestUserPurchaseHistory, sending);
        }
        
        
        /// <summary>
        /// 게임서버의 상품 리스트 받아오기 
        /// </summary>
        public void RequestGameProductList() {
            JsonData sending = new JsonData();
            sending["func"] = "getAllProductList";
            
            SendPost(BillingManager.main.OnRequestGameProductList, sending);
            
        }
        
        
        /// <summary>
        /// 환전 상품 받아오기 
        /// </summary>
        public void RequestCoinExchangeProductList() {
            JsonData sending = new JsonData();
            sending["func"] = "getCoinExchangeProductList";
            
            SendPost(BillingManager.main.OnRequestCoinExchangeProductList, sending);
        }
        
        
        #endregion

        #region 기타 통신 RequestGamebaseLaunching, UpdateEpisodeStartRecord, UpdateEpisodeCompleteRecord
        
        /// <summary>
        /// 추천 팝업 요청 
        /// </summary>
        public void RequestRecommedStory() {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestRecommendProject";
            SendPost(OnCompleteRequestRecommedStory, sending, true);
        }
        
        void OnCompleteRequestRecommedStory(HTTPRequest request, HTTPResponse response) {
            if(!CheckResponseValidation(request, response)) {
                return;
            }
            
            Debug.Log(string.Format("OnCompleteRequestRecommedStory [{0}]", response.DataAsText));
            
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            
            SystemManager.ShowRecommendStoryPopup(result["project_id"]);
            
        }
        
        
        /// <summary>
        /// 에피소드 클리어 보상 요청 
        /// </summary>
        /// <param name="isDouble"></param>
        public void RequestEpisodeFirstClearReward(string __currency, int __quantity, bool isDouble) {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestEpisodeFirstClear";
            sending["is_double"] = isDouble;
            sending["episode_id"] = StoryManager.main.CurrentEpisodeID;
            sending["currency"] = __currency;
            sending["quantity"] = __quantity;
            
            // 통신 
            SendPost(UserManager.main.CallbackEpisodeFirstClearReward, sending, true);
        }
        
        
        /// <summary>
        /// 스페셜 에피소드 해금 
        /// </summary>
        /// <param name="__special"></param>
        public void RequestUnlockSpecialEpisode(EpisodeData __special) {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestUnlockSpecialEpisode";
            sending["episode_id"] = __special.episodeID;
            sending["project_id"] = StoryManager.main.CurrentProjectID;
            
            SendPost(null, sending, false, false);
        }
        
        /// <summary>
        /// 미션 해금 
        /// </summary>
        /// <param name="__mission"></param>
        public void RequestUnlockMission(MissionData __mission) {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestUnlockMission";
            sending["mission_id"] = __mission.missionID;
            sending["project_id"] = StoryManager.main.CurrentProjectID;
            
            SendPost(null, sending, false, false);
        }
        
        /// <summary>
        /// 광고 기록 
        /// </summary>
        /// <param name="__projectID"></param>
        /// <param name="__adType"></param>
        public void LogAdvertisement(string __adType) {
            JsonData sending = new JsonData();
            sending["func"] = "insertUserAdHistory";
            
            // 현재 프로젝트ID
            if(!string.IsNullOrEmpty(StoryManager.main.CurrentProjectID))
                sending["project_id"] = StoryManager.main.CurrentProjectID;
            else 
                sending["project_id"] = -1;
            
            // 현재 에피소드ID
            if(!string.IsNullOrEmpty(StoryManager.main.CurrentEpisodeID)) 
                sending["episode_id"] = StoryManager.main.CurrentEpisodeID;
            else 
                sending["episode_id"] = -1;            
                
                
            // 광고타입     
            sending["ad_type"] = __adType;
            
            SendPost(NetworkLoader.main.OnResponseEmptyPostProcess, sending, false);
            
        }
        
        
        /// <summary>
        /// 유저에게 재화지급
        /// </summary>
        /// <param name="__currency"></param>
        /// <param name="__quantity"></param>
        /// <param name="__path"></param>
        /// <param name="__cb"></param>        
        public void AddUserProperty(string __currency, int __quantity, string __path, OnRequestFinishedDelegate __cb) {
            
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "insertUserProperty";
            sending["currency"] = __currency;
            sending["quantity"] = __quantity;
            sending["pathCode"] = __path;
            
            SendPost(__cb, sending);
        }
        

        
        /// <summary>
        /// 기본 재화 정보 요청 
        /// </summary>
        public void RequestUserBaseProperty() {
            JsonData sending = new JsonData();
            sending["func"] = "getUserBankInfoWithResponse";
            
            SendPost(OnRequestUserBaseProperty, sending);
        }
        
        void OnRequestUserBaseProperty(HTTPRequest request, HTTPResponse response) {
            if(!CheckResponseValidation(request, response))
                return;
                
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            
            UserManager.main.SetBankInfo(result);
        }
        
        /// <summary>
        /// 유저 탈퇴하고, 게임서버와 통신. 탈퇴일시를 누른시간 업데이트 
        /// </summary>
        public void UpdateWithdrawDate() {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "updateWithdrawDate";
            
            SendPost(OnResponseEmptyPostProcess, sending);
        }
        
        
        /// <summary>
        /// 프리패스 구매 처리 (legacy 사용하지 않음)
        /// </summary>
        /// <param name="__freepassNo">timedeal ID</param>
        /// <param name="__originPrice">원 가격</param>
        /// <param name="__salePrice">할인 가격</param>
        public void PurchaseProjectPass(int __timeDealID, string __projectID, int __originPrice, int __salePrice) {
            JsonData sending = new JsonData();
            sending[CommonConst.COL_PROJECT_ID] = __projectID;
            sending[LobbyConst.NODE_CURRENCY] = "Free" + __projectID;
            sending["originPrice"] = __originPrice;
            
            // 최소값 설정 
            if(__salePrice < 3)
                sending["salePrice"] = 3;
            else
                sending["salePrice"] = __salePrice;
            
            sending["timedeal_id"] = __timeDealID;
            sending[CommonConst.FUNC] = "purchasePremiumPass"; //  purchaseFreepass > purchasePremiumPass 변경 
            
            SendPost(UserManager.main.CallbackPurchaseFreepass, sending);
        }
        
        /// <summary>
        /// 프리미엄 패스 스타로 구매하기
        /// </summary>
        /// <param name="__projectID"></param>
        /// <param name="__currentPrice"></param>
        public void PurchasePremiumPassByStar(string __projectID, int __currentPrice) {
            
            Debug.Log(string.Format("## PurchasePremiumPassByStar [{0}]/[{1}] ", __projectID, __currentPrice));
            
            JsonData sending = new JsonData();
            
            sending[CommonConst.COL_PROJECT_ID] = __projectID;
            sending["price"] = __currentPrice;
            sending[CommonConst.FUNC] = "purchasePremiumPass"; //  purchaseFreepass > purchasePremiumPass 변경 
            
            SendPost(UserManager.main.CallbackPurchaseFreepass, sending);
        }
        
        /// <summary>
        /// 선택지 프로그레스 업데이트
        /// </summary>
        /// <param name="__targetSceneID">이동하게 되는 사건ID</param>
        /// <param name="__selectionData">버튼 텍스트</param>
        public void UpdateUserSelectionProgress(string __targetSceneID, string __selectionData) {
            JsonData sending = new JsonData();
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID; // 현재 프로젝트 
            sending["episodeID"] = StoryManager.main.CurrentEpisodeID; 
            sending["target_scene_id"] = __targetSceneID;
            sending["selection_data"] = __selectionData;
            sending[CommonConst.FUNC] = "updateSelectionProgress"; // func 지정 

            SendPost(UserManager.main.CallbackUpdateSelectionProgress, sending);
        }

        
        /// <summary>
        /// 선택지 기록 쌓기
        /// </summary>
        /// <param name="__targetSceneID">이동하게 되는 사건ID</param>
        /// <param name="__selectionGroup">선택지 그룹 번호</param>
        /// <param name="__selectionNo">선택지 내 번호</param>
        public void UpdateUserSelectionCurrent(string __targetSceneID, int __selectionGroup, int __selectionNo)
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "updateUserSelectionCurrent";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            sending["episodeID"] = StoryManager.main.CurrentEpisodeID;
            sending["target_scene_id"] = __targetSceneID;
            sending[GameConst.COL_SELECTION_GROUP] = __selectionGroup;
            sending[GameConst.COL_SELECTION_NO] = __selectionNo;

            SendPost(UserManager.main.CallbackUpdateSelectionCurrent, sending);
        }


        /// <summary>
        /// 작품의 플레이 지점을 저장한다 (작품 순번 체크와 이어하기에서 사용)
        /// </summary>
        /// <param name="__episodeID">에피소드 ID</param>
        /// <param name="__sceneID">사건 ID</param>
        /// <param name="__scriptNO">Script NO</param>
        /// <param name="__isStarting">시작시점에서의 호출인지 여부 </param>
        public void UpdateUserProjectCurrent(string __episodeID, string __sceneID, long __scriptNO, bool __isStarting = false, string __callby = "default") 
        {
            if (!UserManager.main.useRecord)
                return;

            JsonData sending = new JsonData();
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID; // 현재 프로젝트 
            sending["episodeID"] = __episodeID;
            sending["scene_id"] = __sceneID;
            sending["script_no"] = __scriptNO;
            sending[CommonConst.FUNC] = "updateUserProjectCurrent"; // func 지정 
            sending["callby"]  = __callby; // 호출지점
            
            // 에피소드 시작시점과, 플레이 도중일때 콜백을 다르게 분리했다. 
            if(__isStarting) 
                SendPost(UserManager.main.CallbackUpdateProjectCurrentWhenStart, sending);
            else
                SendPost(UserManager.main.CallbackUpdateProjectCurrent, sending);
        }

        /// <summary>
        /// 게임베이스 런칭 호출 
        /// </summary>
        /// <param name="__cb"></param>
        public void RequestGamebaseLaunching(OnRequestFinishedDelegate __cb)
        {
           
            // GET 방식으로 호출
            HTTPRequest request = new HTTPRequest(new System.Uri(FUNC_GAMEBASE_LAUNCHING), HTTPMethods.Get, __cb);
            request.SetHeader("Content-Type", "application/json; charset=UTF-8");
            request.ConnectTimeout = TimeSpan.FromSeconds(10);
            request.Timeout = TimeSpan.FromSeconds(30);
            request.Send();
        }

        


        /// <summary>
        /// Drop미션 완료 요청
        /// </summary>
        /// <param name="missionName">scriptData에 입력된 미션 이름</param>
        public void UpdateScriptMission(string missionName)
        {
            
            MissionData missionData = UserManager.main.GetMissionData(missionName);
            
            // 데이터 없음 
            if(missionData == null || missionData.missionID <= 0) {
                GameManager.main.isThreadHold = false;
                return;
            }
            
            // 이미 해금된 상태
            if(missionData.missionState != MissionState.locked) {
                GameManager.main.isThreadHold = false;
                return;
            }

            

            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = FUNC_UPDATE_USER_SCRIPT_MISSION;
            sending["userkey"] = UserManager.main.userKey;
            sending["project_id"] = StoryManager.main.CurrentProjectID;
            sending["mission_id"] = missionData.missionID;

            SendPost(CallbackUpdateScriptMission, sending, false);
        }

        void CallbackUpdateScriptMission(HTTPRequest req, HTTPResponse res)
        {
            GameManager.main.isThreadHold = false;      // 통신이 완료된 후에 행을 진행해준다

            if (!CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateScriptMission");
                return;
            }
            
            Debug.Log("Wait Off [CallbackUpdateScriptMission] : " + res.DataAsText);

            JsonData data = JsonMapper.ToObject(res.DataAsText);
            // UserManager.main.ShowCompleteMission(data);
        }

        
        /// <summary>
        /// 유저 일러스트 해금 요청 
        /// </summary>
        /// <param name="__illustID">일러스트 ID</param>
        /// <param name="__illustType">일러스트 타입(illust,live2d)</param>
        public bool UpdateUserIllust(string __illustID, string __illustType)
        {

            // 리턴 값에 따라서 메세지 처리 추가할것. (신규 일러스트가 해금..어쩌고저쩌고)

            // 새로운 일러스트 아니면 통신할 필요 없음 
            if (!UserManager.main.CheckIllustUnlockable(__illustID, __illustType))
                return false; 

            JsonData sending = new JsonData();
            sending["project_id"] = StoryManager.main.CurrentProjectID; // 현재 프로젝트 ID 
            sending["illust_id"] = __illustID; // 일러스트 ID
            
            // 일러스트 타입(illust / live2d) 2 종류로 변형해서 전송 
            sending["illust_type"] = __illustType.Contains("live")?"live2d":__illustType; 
            sending[CommonConst.FUNC] = FUNC_UPDATE_USER_ILLUST_HISTORY;

            SendPost(UserManager.main.CallbackUpdateIllustHistory, sending);
            return true; 
            // 신규면 true 리턴, 이거 응답받고 메세지 띄우면 안되고, CallbackUpdateIllustHistory 에서 
            // 정상 응답받으면 메세지 처리 해줘야함!
        }
        


        /// <summary>
        /// 유저의 미니컷 해금기록 업데이트
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="minicutType">live_object or minicut</param>
        /// <returns></returns>
        public bool UpdateUserImage(string imageName, string minicutType)
        {
            Debug.Log(string.Format("UpdateUserImage [{0}]/[{1}]", imageName, minicutType));
            
            // 새로운 미니컷(이미지,라이브 오브제)가 아니면 통신 안함
            if (!UserManager.main.CheckMinicutUnlockable(imageName, minicutType))
                return false;

            string imageId = StoryManager.main.GetGalleryMinicutID(imageName, minicutType);

            // 목록에 없는 거여도 통신 안하기
            if (string.IsNullOrEmpty(imageId))
                return false;

            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = FUNC_UPDATE_USER_MINICUT_HISTORY;
            sending["userkey"] = UserManager.main.userKey;
            sending["project_id"] = StoryManager.main.CurrentProjectID;
            sending["minicut_id"] = imageId;            // 미니컷 Id. 이미지, 라이브오브제 상관없이 다 들어옴
            sending["minicut_type"] = minicutType.Contains("live")?"live2d":minicutType;      // (minicut, live2d) 2종류가 들어온다

            SendPost(UserManager.main.CallbackUpdateMinicutHistory, sending);
            return true;
        }


        /// <summary>
        /// 유저 보이스(더빙) 해금 요청. 전달만 함
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="soundName"></param>
        public void UpdateUserVoice(string soundName, string soundID)
        {
            // return값이 true면 오픈한 적이 있기 때문에 통신할 필요X
            if (UserManager.main.CheckNewVoice(soundName))
                return;

            JsonData sending = new JsonData();
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            
            sending[CommonConst.SOUND_NAME] = soundName;
            sending["sound_id"] = soundID; // 사운드 ID 추가 
            
            sending[CommonConst.FUNC] = FUNC_UPDATE_USER_VOICE_HISTORY;

            SendPost(OnUpdateUserVoice, sending);
        }
        
        void OnUpdateUserVoice(HTTPRequest request, HTTPResponse response) {
            if(CheckResponseValidation(request, response)) {
                return;
            }
            
            // 성공시 응답으로 sound_name을 그대로 받는다. 
            // UserManager에 갱신해준다. 
            UserManager.main.SetVoiceOpen(response.DataAsText);

            if (UserManager.main.ProjectAllClear())
                RequestIFYOUAchievement(8, int.Parse(StoryManager.main.CurrentProjectID));
        }

       
        
        /// <summary>
        /// 에피소드 진행도 리셋 
        /// </summary>
        /// <param name="resetEpisodeID">타겟 에피소드</param>
        public void ResetEpisodeProgress(string __resetEpisodeID, int __price, bool __isFree)
        {
            // 에피소드 리셋은 반드시 통신이 완료되고 다음이 진행되어야 합니다. 
            JsonData sending = new JsonData();
            sending["project_id"] = StoryManager.main.CurrentProjectID; // 현재 프로젝트 ID 
            sending["episodeID"] = __resetEpisodeID; // 타겟 에피소드 
            sending["price"] = __price; // 리셋 가격
            sending["isFree"] = __isFree; // 무료 유료 처리 
            sending[CommonConst.FUNC] = FUNC_RESET_EPISODE_PROGRESS_TYPE2;

            resetTargetEpisodeId = int.Parse(__resetEpisodeID);

            // 통신 시작!
            SendPost(UserManager.main.CallbackResetEpisodeProgress, sending, true);
        }        
        


        #endregion

        #region 동기 통신
        
        /// <summary>
        /// 에피소드 구매 (2021.08.04 수정)
        /// </summary>
        /// <param name="__episodeID">대상 에피소드ID</param>
        /// <param name="__purchaseType">구매 타입</param>
        /// <param name="__currency">재화코드</param>
        /// <param name="__currencyQuantity">재화개수</param>
        public void PurchaseEpisode(string __episodeID, PurchaseState __purchaseType, string __currency, string __currencyQuantity)
        {
            // 에피소드 구매는 반드시 통신이 완료되고 다음이 진행되어야 합니다. 
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = FUNC_EPISODE_PURCHASE;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID; // 현재 프로젝트 ID 
            sending["episodeID"] = __episodeID; // 타겟 에피소드 
            sending["purchaseType"] = __purchaseType.ToString();
            sending["currency"] = __currency;
            sending["currencyQuantity"] = __currencyQuantity;
            

            SendPost(UserManager.main.CallbackPurchaseEpisode, sending, true);
        }
        
        
        /// <summary>
        /// 에피소드 리소스와 스크립트 조회
        /// </summary>
        /// <param name="__sending">전송 데이터</param>
        /// <param name="__cb">콜백</param>        
        public void RequestEpisodeGameData(JsonData __sending, Action<JsonData> __cb) {
            OnEpisodeGameData = __cb;
            SendPost(OnRequestEpisodeGameData, __sending, true);
        }
        
        void OnRequestEpisodeGameData(HTTPRequest request, HTTPResponse response) {
            if(!CheckResponseValidation(request, response)) {
                return;
            }
            
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            OnEpisodeGameData?.Invoke(result);
        }
        

        /// <summary>
        /// 메일 리스트 조회 
        /// </summary>
        public void RequestUnreadMailList(OnRequestFinishedDelegate __cb = null)
        {
            OnRequestFinishedDelegate callback = UserManager.main.CallbackNotRecievedMail;
            if(__cb != null)
                callback += __cb;
            
            JsonData sendingData = new JsonData();
            sendingData[CommonConst.FUNC] = FUNC_MAIL_LIST;

            SendPost(callback, sendingData, true);
        }

        /// <summary>
        /// 단일 메일 수신
        /// </summary>
        public void RequestSingleMail(OnRequestFinishedDelegate __cb, string mailNo)
        {
            JsonData sendingData = new JsonData();
            sendingData[CommonConst.FUNC] = FUNC_MAIL_SINGLE_READ;
            sendingData["mail_no"] = mailNo;

            SendPost(__cb, sendingData, true);
        }

        /// <summary>
        /// 메일 한 번에 수신(모두 받기)
        /// </summary>
        public void RequestAllMail(OnRequestFinishedDelegate __cb)
        {
            JsonData sendingData = new JsonData();
            sendingData[CommonConst.FUNC] = FUNC_MAIL_ALL_READ;

            SendPost(__cb, sendingData, true);
        }

        #endregion





        /// <summary>
        /// 커밍순 리스트 요청
        /// </summary>
        public void RequestComingSoonList()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "getCommingList";
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;

            SendPost(StoryManager.main.CallbackComingSoonList, sending);
        }


        #region 이프유플레이

        /// <summary>
        /// 이프유 플레이 페이지에 들어가는 모든 정보 리스트
        /// 2022.09.15 변경 
        /// </summary>
        public void RequestIfyouplayList(bool __needSync = false)
        {
            JsonData sending = new JsonData();
            
            // 수정된 메소드 호출한다. 
            sending[CommonConst.FUNC] = "requestIfyouPlayListOptimized";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;

            
            // 
            if(__needSync) {
                SendPost(UserManager.main.CallbackIfyouplayRefresh, sending, __needSync);    
            }
            else {
                SendPost(UserManager.main.CallbackIfyouplayList, sending, __needSync);
            }
            
        }


        #region 출석

        /// <summary>
        /// Daily 출석보상 요청
        /// </summary>
        /// <param name="attendanceId">출석보상 id</param>
        /// <param name="daySeq">몇일차</param>
        public void SendAttendanceReward(int attendanceId, int daySeq, OnRequestFinishedDelegate callback)
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "sendAttendanceReward";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending["attendance_id"] = attendanceId;
            sending[LobbyConst.NODE_DAY_SEQ] = daySeq;

            SendPost(callback, sending, true);
        }


        #endregion


        /// <summary>
        /// 일일미션 누적 요청 (1씩 카운트 쌓기)
        /// 전체 일일미션 클리어하기, 광고, 에피소드 클리어 까지 3개의 미션에서 사용 (mission_no : 1,2,3)
        /// </summary>
        /// <param name="missionNO"></param>
        public void IncreaseDailyMissionCount(int missionNO) {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "increaseDailyMissionCount";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;
            sending["mission_no"] = missionNO;

            SendPost(UserManager.main.CallbackIncreaseDailyMissionCount, sending);
        }


        /// <summary>
        /// 광고 보상 요청
        /// </summary>
        /// <param name="adNo"></param>
        public void RequestAdReward(int adNo)
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestAdRewardOptimized";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;
            sending["ad_no"] = adNo;

            SendPost(UserManager.main.CallbackIfyouPlayAdReward, sending, true);
        }


        #endregion


        /// <summary>
        /// 서비스 중인 프로모션, 공지사항, 장르 통합 조회
        /// </summary>
        public void RequestPlatformServiceEvents() {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "getPlatformEvents";

            SendPost(SystemManager.main.CallbackPlatformServiceEvent, sending);
        }


        /// <summary>
        /// 이프유 업적 통신
        /// </summary>
        /// <param name="__achievementType"></param>
        public void RequestIFYOUAchievement(int __achievementType, int __projectId = -1, int __episodeId = -1)
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestAchievementMain";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending["achievement_id"] = __achievementType;
            sending[CommonConst.COL_PROJECT_ID] = __projectId;
            sending[CommonConst.COL_EPISODE_ID] = __episodeId;

            SendPost(UserManager.main.CallbackRequestAchievement, sending);
        }




        #region 카테고리

        /// <summary>
        /// 카테고리에서 사용하는 장르 요청
        /// </summary>
        /// <param name="__cb"></param>
        public void RequestGenre () {
            JsonData sendingData = new JsonData();
            sendingData[CommonConst.FUNC] = "getDistinctProjectGenre";
            
            SendPost(SystemManager.main.SetCategoryGenre, sendingData, false);
        }
        #endregion


        /// <summary>
        /// 통신 기본 파라매터 
        /// </summary>
        /// <param name="__j"></param>
        void SetBaseParams(JsonData __j) {
            __j[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            __j[COL_BUILD] = Application.identifier;
            __j[COL_COUNTRY] = Gamebase.GetCountryCodeOfDevice();
            __j[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;
            __j[COL_OS] = 0;
            
            #if UNITY_IOS
            __j[COL_OS] = 1; // 아이폰
            #endif
        }

        /// <summary>
        /// 독립적인 콜백을 갖는 Post 전송용 메소드 
        /// </summary>
        /// <param name="__cb"></param>
        /// <param name="__sendingData"></param>
        public void SendPost(OnRequestFinishedDelegate __cb, JsonData __sendingData, bool __isSync = false, bool __addList = true)
        {
            _requestURL = _url + CommonConst.CLIENT_URL; // 두개 더합니다. 

            if (__sendingData == null)
                __sendingData = new JsonData();
                
            SetBaseParams(__sendingData);
           

            // func 동작에 대한 List ADD
            if (__sendingData.ContainsKey(CommonConst.FUNC) && __addList)
                ListNetwork.Add(__sendingData[CommonConst.FUNC].ToString());

            // 동기 통신에 대한 처리
            if(__isSync)
                SystemManager.ShowNetworkLoading(); // 동기 통신의 경우는 꼭 얘를 띄워주자.
            else
                SystemManager.HideNetworkLoading();


            // 콜백 전달해준다. 
            HTTPRequest request = new HTTPRequest(new System.Uri(_requestURL), HTTPMethods.Post, __cb);
            // HTTPRequest req = new HTTPRequest(new System.Uri(_requestURL), HTTPMethods.Post);
            
            request.SetHeader("Content-Type", "application/json; charset=UTF-8");
            request.RawData = Encoding.UTF8.GetBytes(JsonMapper.ToJson(__sendingData));

            request.ConnectTimeout = System.TimeSpan.FromSeconds(10);
            request.Timeout = System.TimeSpan.FromSeconds(15);
            request.Tag = JsonMapper.ToJson(__sendingData);

            Debug.Log(">> POST : " + JsonMapper.ToJson(__sendingData));

            request.Send();
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void OnResponseEmptyPostProcess(HTTPRequest request, HTTPResponse response)
        {
            CheckResponseValidation(request, response);
        }



        /// <summary>
        /// 네트워크 통신 체크 공통
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool CheckResponseValidation(HTTPRequest request, HTTPResponse response, bool dontHideNetworkLoading = false)
        {
            
            string message = string.Empty;
            _requestRawData = string.Empty;
            string currentFunc = string.Empty;

            #region ListNetwork에 대한 추가 처리 

            try
            {
                // 통신 완료 후, func 값 추출 
                if(request.RawData != null) {
                
                    _requestRawData = Encoding.UTF8.GetString(request.RawData);
                    _requestRawData = _requestRawData.Trim();
                    
                    // 유효한 json string 일때만, json으로 변환하도록 한다.
                    if((_requestRawData.StartsWith("{") && _requestRawData.EndsWith("}")) || (_requestRawData.StartsWith("[") && _requestRawData.EndsWith("]"))) {
                        _reqData = JsonMapper.ToObject(_requestRawData);
                        
                        if (_reqData != null && _reqData.ContainsKey(CommonConst.FUNC))
                        {
                            currentFunc = _reqData[CommonConst.FUNC].ToString();
                        }        
                    } //  end of deal with json
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e.StackTrace);
            } // ? func 추출 완료



            #endregion
            
            // 통신 Error 핸들링 
            switch(request.State) {
                case HTTPRequestStates.Finished:

                    //통신이 실질적으로 완료됨.
                    main.ListNetwork.Remove(_reqData[CommonConst.FUNC].ToString());
                    
                    // 일반적으로 네트워크 로딩 제거 
                    if(CheckServerWork() && !dontHideNetworkLoading)
                        SystemManager.HideNetworkLoading();                     
                
                    // 실패 메세지가 보여지는 중이 아니라면 count 0으로 초기화
                    if(!main.isFailMessageShow)
                        main.failCount = 0;                            
                
                    if(response.IsSuccess) { // 굿! (status 200~299 , 304)
                        return true;
                    }
                    else { // 통신에는 성공했지만 status가 false 
                        // 실패로 날아오는 경우는 message와 code가 날아온다. (서버에서)
                        // 서버에서 status 400 으로 전달받는다. 
                        message = GetServerErrorMessage(response);


                        // status가 error로 날아오는 경우에 대한 메세지 처리 (2021.11.02)
                        if (!string.IsNullOrEmpty(message))
                            SystemManager.ShowSystemPopup(message, null, null, false, false);        // 메세지 띄워주고.
                        
                        SystemManager.HideNetworkLoading(); // 여기서는 무조건 제거 
                        return false; 
                    }
                    
                // ! 여기서부터는 일반적인 응답 오류가 아닌 request 에러에 대한 처리 
                case HTTPRequestStates.Error: // 예상하지 못한 에러가 발생했다.
                // 서버로 리포트.(ERROR만)
                // main.ReportRequestError(Encoding.UTF8.GetString(request.RawData), request.Exception != null ? request.Exception.Message : "No Exception");
                message = SystemManager.GetLocalizedText("6173"); // 서버 오류가 발생함
                break;
                
                case HTTPRequestStates.Aborted: // 네트워크 연결이 끊어졌다. 
                message = SystemManager.GetLocalizedText("6174"); 
                break;
                
                case HTTPRequestStates.ConnectionTimedOut: // 서버에 연결되지 못함 
                message = SystemManager.GetLocalizedText("6175");
                break;
                
                case HTTPRequestStates.TimedOut: // 서버에서 주어진 시간내에 응답을 하지 못함
                message = SystemManager.GetLocalizedText("6176");
                break;
                
                default: 
                message = "Unknown error occurred.";
                break;
            }
            
            // 팝업창으로 알려준다.
            // 메세지가 있는 경우 (통신 완료되지 못함)
            if(!string.IsNullOrEmpty(message)) {
                
                Debug.LogError("Request Fail : " + message);
                
                // 실패 카운트 추가 
                main.failCount++;
                
                if(!main.isFailMessageShow && main.failCount >= 5) {
                    SystemManager.HideNetworkLoading();
                    // ! 오류 메세지. 
                    // 게임 밖으로 내보낸다. 
                    SystemManager.ShowSystemPopup(message, OnFailedServer, OnFailedServer, false, false);
                    return false; // 이제 그만 보내. 
                }
                
                // 다시 보낸다.
                request.Send();
          
            } // ? end of retry
            
            // 여기까지 내려왔으면 다 실패
            return false;
        }
        
        /// <summary>
        /// 서버 연결 오류 
        /// </summary>
        public static void OnFailedServer() {
            Application.Quit();
        }
        
        static void OnResourceDownloadFail() {
            SystemManager.LoadLobbyScene();
            main.isDownloadFailMessageShow = false;
        }
        
        /// <summary>
        /// 리소스 다운로드에 대한 유효성 체크 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool CheckInGameDownloadValidation(HTTPRequest request, HTTPResponse response) {
            
            string exceptionMessage = string.Empty;
            
            // * 리소스 다운로드는 에러 핸들링이 일반 서버 통신과는 달라서 여기서 처리한다. 
            switch(request.State) {
                
                case HTTPRequestStates.Finished:
                    if(response.IsSuccess) {
                        
                        // 실패 메세지 보여지는 중이 아니라면 카운트 0으로 초기화
                        if(!main.isDownloadFailMessageShow)
                            main.downloadFailCount = 0;
                        
                        
                        return true; // 종료
                    }
                    else {
                        
                        
                        // * AWS S3의 경우, 리소스가 없는 경우에 대해서는 request는 완료,
                        // * response에서 fail을 준다. 
                        // * 이 경우는 서버 설정에 따라 진입을 막을지 허용할지 처리한다. allow_missing_resource
                        try { 
                            exceptionMessage = string.Format("{0}-{1} Response Message", response.StatusCode, response.Message);
                        }
                        catch{
                            exceptionMessage = "finished but response is fail";
                        }
                        
                        Debug.LogError(string.Format("!!! Download response fail [{0}]", exceptionMessage));
                        
                        // 서버로 리포트 
                        main.ReportRequestError(request.Uri.ToString(), string.Format("Request is finished, but response failed [{0}]", response.StatusCode));
                        
                    }
                    break; // ? end of Finished 
                
                
                default:
                    exceptionMessage = request.State.ToString();
                    Debug.LogError(string.Format("!!! Download request fail [{0}]", exceptionMessage));
                    
                    // 서버로 리포트 
                    main.ReportRequestError(request.Uri.ToString(), string.Format("Request failed. [{0}]", request.State.ToString()));
                break;
            }

            
            // * 요청이 올바르게 처리되지 못한경우 exceptionMessage가 할당된다. 
            // * 한번에 날아간 여러개의 다운로드 요청을 합해서 실패 카운트가 10 이상이면 로비로 되돌려보낸다. 
            // * 실패 요청은 1초 후에 재시도 처리한다.
            if(!string.IsNullOrEmpty(exceptionMessage)) {
                // 시간초과이거나, 다운받지 못한 경우... 
                Debug.LogError("Download Fail : " + exceptionMessage);
                main.downloadFailCount++;
                
                if(!main.isDownloadFailMessageShow && main.downloadFailCount >= 10) {
                    SystemManager.HideNetworkLoading();

                    // ! 오류 메세지. 
                    // 게임 밖으로 내보낸다. 
                    SystemManager.ShowSystemPopupLocalize("80084", OnResourceDownloadFail, OnResourceDownloadFail, false, false);
                    main.isDownloadFailMessageShow = true; // 메세지 중복 호출 막는다. 
                    return false; // 이제 그만 보내. 
                }
                
                // 요청 다시 시도한다.
                main.ResendRequest(request);

            }
            
            return false;

        }
        
        /// <summary>
        /// 실패 요청에 대해서 다시 요청하기 (코루틴 연계)
        /// </summary>
        /// <param name="__request"></param>
        public void ResendRequest(HTTPRequest __request) {
            StartCoroutine(RoutineResend(__request));
        }
        
        IEnumerator RoutineResend(HTTPRequest __request)  {
            yield return new WaitForSeconds(1); // 1초 있다가 재시도 
            
            Debug.Log(string.Format("******* Request again [{0}]", __request.Uri.ToString()));
            
            __request.Send();
        }
        
        
        /// <summary>
        /// request의 헤더에 있는 retryCount 가져오기 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        static int GetRequestRetryCount(HTTPRequest request) {
            
            int retryCount = 0;
            
            List<string> headers = request.GetHeaderValues(HEADER_RETRY);
            if(headers == null || headers.Count == 0) {
                // 재시도를 한번도 하지 않은 경우, 2로 설정 
                SetRequestRetryCount(request, 2);
                return 2;
            }
            
            // 재시도 이력이 있음
            int.TryParse(headers[0], out retryCount);
            Debug.Log("Retry Download : "  + retryCount);
            
            retryCount--;
            SetRequestRetryCount(request, retryCount);
            return retryCount;
        }
        
        /// <summary>
        /// request에 재시도 헤더값 설정
        /// </summary>
        /// <param name="request"></param>
        /// <param name="newRetryCount"></param>
        static void SetRequestRetryCount(HTTPRequest request, int newRetryCount) {
            request.SetHeader(HEADER_RETRY, newRetryCount.ToString());
        }
        
        
        /// <summary>
        /// Request Error 리포트하기. 
        /// </summary>
        /// <param name="__rawData"></param>
        /// <param name="__message"></param>
        public void ReportRequestError(string __rawData, string __message) {
            // HTTPRequest Error 발생시에 메세지 처리하지 않고 서버로 리포트하기.
            
            JsonData sendingData = new JsonData();
            sendingData["rawData"] = __rawData;
            sendingData["message"] = __message;
            sendingData["func"] = "reportRequestError";
            
            SendPost(OnResponseEmptyPostProcess, sendingData, false);
        }
        
        /// <summary>
        /// 서버 에러 메세지 가져오기 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        static string GetServerErrorMessage(HTTPResponse response) {
            if(string.IsNullOrEmpty(response.DataAsText))
                return "상세 에러 메세지를 전송받지 못했습니다.";
        
            JsonData errorData;
            string errorMessage = null;
            string errorCode = null;
            string serverMessage = string.Empty;
                            
            try {
                errorData = JsonMapper.ToObject(response.DataAsText);
                
                if(errorData.ContainsKey("code")) {
                    errorCode = errorData["code"].ToString();
                    errorMessage = SystemManager.GetLocalizedText(errorCode); 
                }
                else {
                    errorCode = string.Empty;
                    errorMessage = "Unknwon message";
                }
                
                if(errorData.ContainsKey("koMessage")) {
                    serverMessage = errorData["koMessage"].ToString(); // 서버에서 전달 받은 메세지 
                }
                
                // errorMessage = string.Format("{0} [{1}]", SystemManager.GetLocalizedText(errorCode), errorMessage);
            } 
            catch(Exception e) {
                Debug.Log(e.StackTrace);
                errorMessage = "Unknown Exception";
            }
            
            return errorMessage;
        }

        /// <summary>
        /// 서버 통신 완료되었는지 체크 
        /// </summary>
        /// <returns>List 비어있으면 true</returns>
        public static bool CheckServerWork()
        {
            return main.ListNetwork.Count <= 0;
        }
    }
}