﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using LitJson;
using System.Text;
using Toast.Gamebase;

namespace PIERStory
{

    public class NetworkLoader : MonoBehaviour
    {
        public static NetworkLoader main = null;
        
        public static Action<JsonData> OnEpisodeGameData = null;

        #region CONST
        const string FUNC = "func";
        const string HEADER_RETRY = "X-retry";

        public const string FUNC_LOGIN_CLIENT = "loginClient";
        public const string FUNC_SELECTED_STORY_INFO = "getUserSelectedStory";
        public const string FUNC_SELECT_LOBBY_PROJECT_LIST = "selectLobbyProjectList";
        public const string FUNC_UPDATE_EPISODE_PURCHASE = "";
        
        public const string FUNC_UPDATE_EPISODE_SCENE_RECORD = "updateUserEpisodeSceneRecord"; // 씬 경험 기록

        public const string FUNC_CLEAR_EPISODE_SCENE_HISTORY = "clearUserEpisodeSceneHistory";
        public const string FUNC_DELETE_EPISODE_SCENE_HISTORY = "deleteUserEpisodeSceneHistory";
        public const string FUNC_INSERT_DRESS_PROGRESS = "insertUserProjectDressProgress";

        public const string FUNC_UPDATE_EPISODE_COMPLETE_RECORD = "updateUserEpisodePlayRecord"; // 에피소드 플레이 완료 기록 
        public const string FUNC_UPDATE_EPISODE_START_RECORD = "insertUserEpisodeStartRecord"; // 에피소드 플레이 시작 기록 
        public const string FUNC_RESET_EPISODE_PROGRESS = "resetUserEpisodeProgress"; // 에피소드 진행도 리셋 

        public const string FUNC_UPDATE_USER_VOICE_HISTORY = "updateUserVoiceHistory";      // 보이스 해금 갱신
        public const string FUNC_UPDATE_USER_ILLUST_HISTORY = "updateUserIllustHistory";    // 일러스트 해금 갱신
        public const string FUNC_UPDATE_USER_MINICUT_HISTORY = "updateUserMinicutHistory";  // 미니컷 해금 갱신
        public const string FUNC_UPDATE_USER_FAVOR_HISTORY = "updateUserFavorHistory";      // 호감도 갱신
        public const string FUNC_UPDATE_USER_SCRIPT_MISSION = "updateUserScriptMission";    // Drop미션 통신
        public const string FUNC_CHANGE_ACCOUNT_GAMEBASE = "changeAccountByGamebase";

        public const string FUNC_EPISODE_PURCHASE = "purchaseEpisodeType2";
        public const string FUNC_ACCQUIRE_CONSUMABLE_CURRENCY = "accquireUserConsumableCurrency"; // 소모성 재화 획득 
        public const string FUNC_CONSUME_CURRENCY = "consumeUserCurrency"; // 소모성 재화 소모하기 

        // 메일함 관련!
        public const string FUNC_MAIL_LIST = "getUserUnreadMailList"; // 메일함 리스트 조회하기 
        public const string FUNC_MAIL_SINGLE_READ = "requestReceiveSingleMail"; // 메일 하나만 읽기
        public const string FUNC_MAIL_ALL_READ = "requestReceiveAllMail"; // 메일 다 읽기!
        
        public const string FUNC_PROJECT_ALL_EMOTICONS = "getProjectAllEmoticon"; // 프로젝트의 모든 이모티콘 

        // 엔딩 관련
        public const string FUNC_USER_ENDING_LIST = "getUserEndingList";        // 획득한 엔딩 리스트 불러오기

        // 미션 관련
        public const string FUNC_USER_MISSION_REWARD = "getUserMisionReward";   // 미션 개별 보상
        


        // 게임베이스 Launching 정보 조회 
        const string FUNC_GAMEBASE_LAUNCHING = "https://api-lnc.cloud.toast.com/launching/v3.0/appkeys/csYXV5DuW8h22Xxo/configurations";
        #endregion
        
        // * 기본 파라매터 
        const string COL_BUILD = "build";
        const string COL_USERKEY = "userkey";
        const string COL_COUNTRY = "country";


        [SerializeField] string _url = string.Empty; // 서버 URL 
        [SerializeField] string _requestURL = string.Empty; // REQUEST URL

        static JsonData _reqData = null; // request rawData 확인용도

        // 네트워크 통신 목록 전송시에 Add, 응답시에 remove. (sync 처리를 위해  추가)
        public List<string> ListNetwork = new List<string>(); 
        static string _requestRawData = string.Empty;


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


        // Start is called before the first frame update
        void Start()
        {
            if (SystemManager.main.isTestServerAccess)
                _url = CommonConst.TEST_SERVER_URL;
            else
                _url = CommonConst.LIVE_SERVER_URL;

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
            sending["func"] = "getUserPurchaseList";
            
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
        
        #endregion

        #region 기타 통신 RequestGamebaseLaunching, UpdateEpisodeStartRecord, UpdateEpisodeCompleteRecord
        
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
            sending["func"] = "updateWithdrawDate";
            
            SendPost(OnResponseEmptyPostProcess, sending);
        }
        
        
        /// <summary>
        /// 프리패스 구매 처리 
        /// </summary>
        /// <param name="__freepassNo">timedeal ID</param>
        /// <param name="__originPrice">원 가격</param>
        /// <param name="__salePrice">할인 가격</param>
        public void PurchaseProjectFreepass(string __freepassNo, int __originPrice, int __salePrice) {

        }
        
        /// <summary>
        /// 선택지 프로그레스 업데이트
        /// </summary>
        /// <param name="__targetSceneID">이동하게 되는 사건ID</param>
        /// <param name="__selectionData">버튼 텍스트</param>
        public void UpdateUserSelectionProgress(string __targetSceneID, string __selectionData) {

        }

        /// <summary>
        /// 작품의 플레이 지점을 업데이트한다. (이어하기에 사용)
        /// </summary>
        /// <param name="__sceneID"></param>
        /// <param name="__scriptNO"></param>
        public void UpdateUserProjectCurrent(string __episodeID, string __sceneID, long __scriptNO) 
        {

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
            request.ConnectTimeout = System.TimeSpan.FromSeconds(10);
            request.Timeout = System.TimeSpan.FromSeconds(30);
            request.Send();
        }

        /// <summary>
        /// 유저 호감도 업데이트 
        /// </summary>
        /// <param name="__favorName">호감도 이름</param>
        /// <param name="__score">점수</param>
        /// <param name="__cb">콜백</param>
        public void UpdateUserFavor(string __favorName, int __score, OnRequestFinishedDelegate __cb)
        {
            JsonData sending = new JsonData();
            sending["project_id"] = StoryManager.main.CurrentProjectID; // 현재 프로젝트 
            sending["favor_name"] = __favorName; // 호감도 이름
            sending["score"] = __score; // 스코어 
            sending["func"] = FUNC_UPDATE_USER_FAVOR_HISTORY; // func 지정 

            SendPost(__cb, sending);
        }



        


        /// <summary>
        /// 유저의 미니컷 해금기록 업데이트
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="minicutType"></param>
        /// <returns></returns>
        public bool UpdateUserImage(string imageName, string minicutType)
        {

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
            sending[COL_USERKEY] = UserManager.main.userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            
            sending[CommonConst.SOUND_NAME] = soundName;
            sending["sound_id"] = soundID; // 사운드 ID 추가 
            
            sending[FUNC] = FUNC_UPDATE_USER_VOICE_HISTORY;

            SendPost(OnUpdateUserVoice, sending);
        }
        
        void OnUpdateUserVoice(HTTPRequest request, HTTPResponse response) {
            if(CheckResponseValidation(request, response)) {
                return;
            }
            
            // 성공시 응답으로 sound_name을 그대로 받는다. 
            // UserManager에 갱신해준다. 
            UserManager.main.SetVoiceOpen(response.DataAsText);
        }




        
        /// <summary>
        /// 에피소드 구매 (2021.08.04 수정)
        /// </summary>
        /// <param name="__episodeID">대상 에피소드ID</param>
        /// <param name="__purchaseType">구매 타입</param>
        /// <param name="__currency">재화코드</param>
        /// <param name="__currencyQuantity">재화개수</param>


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
            sending[FUNC] = FUNC_EPISODE_PURCHASE;
            sending["project_id"] = StoryManager.main.CurrentProjectID; // 현재 프로젝트 ID 
            sending["episodeID"] = __episodeID; // 타겟 에피소드 
            sending["purchaseType"] = __purchaseType.ToString();
            sending["currency"] = __currency;
            sending["currencyQuantity"] = __currencyQuantity;
            

            SendPost(UserManager.main.CallbackPurchaseEpisode, sending);
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

        }

        /// <summary>
        /// 단일 메일 수신
        /// </summary>
        public void RequestSingleMail(OnRequestFinishedDelegate __cb, string mailNo)
        {
            JsonData sendingData = new JsonData();
            sendingData[FUNC] = FUNC_MAIL_SINGLE_READ;
            sendingData["mail_no"] = mailNo;

            SendPost(__cb, sendingData, true);
        }

        /// <summary>
        /// 메일 한 번에 수신(모두 받기)
        /// </summary>
        public void RequestAllMail(OnRequestFinishedDelegate __cb)
        {
            JsonData sendingData = new JsonData();
            sendingData[FUNC] = FUNC_MAIL_ALL_READ;

            SendPost(__cb, sendingData, true);
        }

        #endregion


        /// <summary>
        /// 통신 기본 파라매터 
        /// </summary>
        /// <param name="__j"></param>
        void SetBaseParams(JsonData __j) {
            __j[COL_USERKEY] = UserManager.main.userKey;
            __j[COL_BUILD] = Application.identifier;
            __j[COL_COUNTRY] = Gamebase.GetCountryCodeOfDevice();
            __j[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;
        }

        /// <summary>
        /// 독립적인 콜백을 갖는 Post 전송용 메소드 
        /// </summary>
        /// <param name="__cb"></param>
        /// <param name="__sendingData"></param>
        public void SendPost(OnRequestFinishedDelegate __cb, JsonData __sendingData, bool __isSync = false)
        {
            _requestURL = _url + CommonConst.CLIENT_URL; // 두개 더합니다. 

            if (__sendingData == null)
                __sendingData = new JsonData();
                
            SetBaseParams(__sendingData);
           

            // func 동작에 대한 List ADD
            if (__sendingData.ContainsKey(FUNC))
                ListNetwork.Add(__sendingData[FUNC].ToString());

            // 동기 통신에 대한 처리
            if(__isSync)
                SystemManager.ShowNetworkLoading(); // 동기 통신의 경우는 꼭 얘를 띄워주자.


            // 콜백 전달해준다. 
            HTTPRequest request = new HTTPRequest(new System.Uri(_requestURL), HTTPMethods.Post, __cb);
            request.SetHeader("Content-Type", "application/json; charset=UTF-8");
            request.RawData = Encoding.UTF8.GetBytes(JsonMapper.ToJson(__sendingData));

            request.ConnectTimeout = System.TimeSpan.FromSeconds(15);
            request.Timeout = System.TimeSpan.FromSeconds(30);
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
            // CheckResponseValidation(request, response);
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

            #region ListNetwork에 대한 추가 처리 

            try
            {
                if(request.RawData != null) {
                
                    _requestRawData = Encoding.UTF8.GetString(request.RawData);
                    _requestRawData = _requestRawData.Trim();
                    
                    // 유효한 json string 일때만, json으로 변환하도록 한다.
                    if((_requestRawData.StartsWith("{") && _requestRawData.EndsWith("}")) || (_requestRawData.StartsWith("[") && _requestRawData.EndsWith("]"))) {
                        _reqData = JsonMapper.ToObject(_requestRawData);
                        
                        if (_reqData != null && _reqData.ContainsKey(FUNC))
                        {
                            main.ListNetwork.Remove(_reqData[FUNC].ToString());
                        }        
                    } //  end of deal with json
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e.StackTrace);
            }

            // 일부 화면에서는 수동으로 네트워크 로딩 화면을 제거하고 싶다.
            if (CheckServerWork() && !dontHideNetworkLoading)
                SystemManager.HideNetworkLoading();

            #endregion
            
            // 통신 Error 핸들링 
            switch(request.State) {
                case HTTPRequestStates.Finished:
                    if(response.IsSuccess) { // 굿! (status 200~299 , 304)
                        return true;
                    }
                    else { // 통신에는 성공했지만 status가 false 
                        // 실패로 날아오는 경우는 message와 code가 날아온다. (서버에서)
                        message = GetServerErrorMessage(response);
                        SystemManager.HideNetworkLoading(); // 실패했을때 네트워크 로딩은 제거해주자.
                        
                        // status가 error로 날아오는 경우에 대한 메세지 처리 (2021.11.02)
                        if(!string.IsNullOrEmpty(message)) {
                            SystemManager.ShowSimpleMessagePopUp(message); // 메세지 띄워주고.
                        }
                        
                        return false;
                    }
                    
                case HTTPRequestStates.Error: // 예상하지 못한 에러가 발생했다.
                // 서버로 리포트.(ERROR만)
                // main.ReportRequestError(Encoding.UTF8.GetString(request.RawData), request.Exception != null ? request.Exception.Message : "No Exception");
                message = "서버 통신 과정에서 오류가 발생했습니다.";
                break;
                
                case HTTPRequestStates.Aborted: // 네트워크 연결이 끊어졌다. 
                message = "서버와의 연결이 끊겼습니다.\n통신 환경이 좋은 곳에서 다시 연결을 시도해보십시오.";
                break;
                
                case HTTPRequestStates.ConnectionTimedOut: // 서버에 연결되지 못함 
                message = "서버에 접속하지 못했습니다.";
                break;
                
                case HTTPRequestStates.TimedOut: // 서버에서 주어진 시간내에 응답을 하지 못함
                message = "서버로부터의 응답이 지연되고 있습니다.";
                break;
            }
            
            // 팝업창으로 알려준다.
            // 메세지가 있는 경우 (통신 완료되지 못함)
            if(!string.IsNullOrEmpty(message)) {
                
                Debug.LogError("Request Fail : " + message);
                int retryCount = 0;  // 재시도 카운트.
                
                try {
                    
                    // 헤더를 받아와서 HEADER_RETRY값으로 리트라이 횟수를 파악한다. 
                    List<string> headers = request.GetHeaderValues(HEADER_RETRY);
                    
                    // 재시도를 한번도 하지 않았음.
                    if(headers == null || headers.Count == 0) { 
                        Debug.LogWarning("First Try");
                        retryCount = 2;
                        request.SetHeader(HEADER_RETRY, retryCount.ToString());  // retry 카운트를 2로 저장한다.    
                    }
                    else {
                        // 재시도한 이력이 있어서 헤더에 값을 가지고 있다. 
                        // retry 카운트 차감
                        retryCount = int.Parse(headers[0]);
                        Debug.LogWarning("Retry : " + retryCount);
                        
                        retryCount--; // 1회 차감.
                        request.SetHeader(HEADER_RETRY, retryCount.ToString());
                    }
                    
                    if(retryCount > 0) {
                        request.Send(); // 다시 전송. 
                        return false; // false 리턴한다. 다시 통신완료를 기다린다.
                    }
                    else { // 3번하는 동안 모두 실패했다. 
                        SystemManager.HideNetworkLoading(); // 실패했을때 네트워크 로딩은 제거해주자.
                        SystemManager.ShowSimpleMessagePopUp(message, SystemManager.LoadLobbyScene);
                    }
                    
                } catch (System.Exception e) {
                    message = "서버 통신 과정에서 알 수 없는 오류가 발생했습니다.";
                    Debug.Log(e.StackTrace);
                    SystemManager.HideNetworkLoading(); // 실패했을때 네트워크 로딩은 제거해주자.
                    SystemManager.ShowSimpleMessagePopUp(message, SystemManager.LoadLobbyScene);
                }                
            } // ? end of retry
            
            // 여기까지 내려왔으면 다 실패
            return false;
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
            switch(request.State)             {
                
                
                case HTTPRequestStates.Finished:
                    if(response.IsSuccess)
                        return true;
                    else {
                        
                        // 없는 리소스 허용하면, true 리턴 
                        if(SystemManager.main.allowMissingResource)
                            return true;
                        
                        // * AWS S3의 경우, 리소스가 없는 경우에 대해서는 request는 완료,
                        // * response에서 fail을 준다. 
                        // * 이 경우는 서버 설정에 따라 진입을 막을지 허용할지 처리한다. allow_missing_resource
                        exceptionMessage = string.Format("{0}-{1} Message: {2}", response.StatusCode, response.Message, response.DataAsText);
                        Debug.Log(string.Format("!!! Download response fail [{0}]", exceptionMessage));
                    }
                
                break;
                
                default:
                exceptionMessage = request.State.ToString();
                Debug.Log(string.Format("!!! Download request fail [{0}]", exceptionMessage));
                break;
            }

            main.ReportRequestError(request.Uri.ToString(), exceptionMessage);
            
            if(GetRequestRetryCount(request) > 0) {
                request.Send();
            }
            else {
                // 3번 모두 실패하면 로비로 돌려보낸다.
                SystemManager.HideNetworkLoading();
                SystemManager.ShowSimpleMessagePopUp(SystemManager.GetLocalizedText("80084"), SystemManager.LoadLobbyScene);
            }
            
            return false;
            
            
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