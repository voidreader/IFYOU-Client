using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using LitJson;
using BestHTTP;
using Toast.Gamebase;

namespace PIERStory
{

    /// <summary>
    /// 시스템 매니저는 앱의 시작점이며, 게임베이스를 관리합니다. 
    /// </summary>
    public class SystemManager : MonoBehaviour
    {
        // 단위 : 프로젝트 스토리 > 에피소드

        public static SystemManager main = null;
        public static bool IsGamebaseInit = false; // 게임베이스 초기화 여부 
        public bool isServerValid = false; // 업데이트 필수, 점검 
        
        
        public static float screenRatio = 0;
        public bool isTestServerAccess = false; // 테스트 서버로 연결 여부 
        public string gamebaseAPP_ID = ""; // 6swpd3Jp (피어), qtV3HLW5(코원)
        public string gamebaseLogger_ID = ""; // 6WMxzJjo6i5Z5iXm(피어)
        
        
        // 언어 코드 추가 
        [SerializeField] string currentGamebaseLanguageCode = "en"; // 게임베이스 언어코드
        public string currentAppLanguageCode = "EN"; // 앱, 서버 사용하는 언어코드 (게임베이스와 살짝 다르기 때문에 컨버팅 필요)



        [SerializeField] NetworkLoadingScreen networkLoadingScreen = null; // 네트워크 로딩 스크린



        // 게임베이스 콘솔에 등록된 클라이언트의 상태 정보 
        [Space]
        [SerializeField] int clientStatus = GamebaseLaunchingStatus.IN_SERVICE;
        [SerializeField] string clientStatusString = string.Empty;
        [SerializeField] string serverAccessAddress = string.Empty; // 게임베이스에서 전송받은 서버 접속 주소 
        [SerializeField] string gamebaseID = string.Empty;


        // 스크린 관련 정보
        [SerializeField] Rect safeArea;
        public bool hasSafeArea = false;

        bool isLaunchingCalled = false; //  런칭정보 불러왔는지? 
        public JsonData launchingJSON = null; // 게임베이스 런칭 
        public JsonData givenStoryData = null; // 선택된 스토리 JSON 데이터 
        public JsonData givenEpisodeData = null; // 로비에서 전달받은 에피소드 JSON 데이터 
        public JsonData appComonResourceData = null; // 앱 공용 리소스 데이터 (2021.09.14)
       

        
        #region 기준정보 Variables
        [SerializeField] bool isServerInfoReceived = false; // 서버 기준 정보 전달받았는지 체크 한다. (2021.08.31)
        [SerializeField] bool isAppCommonResourcesReceived = false; // 앱 공용 리소스 통신 완료했는지 체크 (2021.09.14)
        
        [SerializeField] int localVer = 0; // 로컬라이징 텍스트 버전
        public bool isCoinPrizeUse = false; // 코인 응모권 시스템
        public string coinPrizeURL = string.Empty; // 코인 응모권 URL
        public bool allowMissingResource = true; // 에피소드 진입시 없는 리소스 허용 여부 2021.11.08
        public int maxAdCharge = 0;             // 일일 최대 무료 충전 횟수
        
        
        static JsonData localizedTextJSON = null; // 로컬라이징 텍스트 JSON
        
        #endregion


        public JsonData noticeData = null; // 공지사항 데이터 


        string messageRequireUpdate = string.Empty;
        string messageTestVersion = string.Empty;

        private void Awake()
        {
            // 다른 씬에서 넘어온 객체가 있을경우. 
            // 게임씬에서 로비씬으로 다시 넘어왔을 경우, Singleton 유지 
            if (main != null)
            {
                Destroy(this.gameObject);
                return;
            }

            if (Application.isEditor)
                Application.runInBackground = true;

            // 프레임레이트 60으로 설정 (iOS는 기본값이 30이라 애니메이션이 예쁘지 않음)
            Application.targetFrameRate = 60;
            safeArea = Screen.safeArea;
            Debug.Log("safeArea : " + safeArea.ToString());

            // safeArea 있는 경우, 캐릭터 위치 조정을 하려고 한다.
            if (safeArea.y != 0f)
            {
                Debug.Log(">>> SafeArea Device!!!!! <<<");
                hasSafeArea = true;
            }
            
            screenRatio = (float)Screen.width / (float)Screen.height;

            // 핸드폰을 종료해도 데이터 환경 다운로드 허용에 대한 값을 저장한다
            if (!PlayerPrefs.HasKey(SystemConst.KEY_NETWORK_DOWNLOAD))
                PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 0);     // 0 : false, 1 : true


            // 공지사항의 오늘 하루 열지 않음의 설정값에 대해 체크한다
            if(PlayerPrefs.HasKey("noticeOneday"))
            {
                DateTime checkTime = DateTime.Parse(PlayerPrefs.GetString("noticeOneday"));
                DateTime curTime = DateTime.Today;
                TimeSpan timeCal = curTime - checkTime;

                // 하루가 지났으면 키값을 삭제해준다
                if (timeCal.Days > 0)
                    PlayerPrefs.DeleteKey("noticeOneday");
            }

            // * 화면 어두워지지 않도록 설정 
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Singleton
            main = this;
            
            // * 코원, 피어 프로젝트 분기하기. 
            Debug.Log("IDENTIFIER : " + Application.identifier);
            if(Application.identifier.Contains("cowon")) {
                // 코원 프로젝트 
                gamebaseAPP_ID = "qtV3HLW5";
                gamebaseLogger_ID = "7eGOB7zE5dQx4yTC";
                //AppsFlyerSDK.AppsFlyer.useAppsFlyer = true;
            }
            else { 
                // 피어 프로젝트 
                gamebaseAPP_ID = "6swpd3Jp";
                gamebaseLogger_ID = "6WMxzJjo6i5Z5iXm";
                // AppsFlyerSDK.AppsFlyer.useAppsFlyer = false;
            }
            // 시스탬매니저는 앱의 실행부터 끝까지 씬에서 모두 사용됩니다. 
            DontDestroyOnLoad(this);
            

        }
        

        /// <summary>
        /// SystemManager의 Start는 앱의 최초 시작지점입니다. 
        /// </summary>
        void Start()
        {

            /* TOAST 게임베이스 초기화. 아래의 순서로 진행됩니다. 
             * 1. Gamebase 초기화 (TOAST 서버와 통신합니다.)
             * 2. Gamebase 로그인 (Gamebase에서 유저별 고유 ID를 받습니다.)
             * 3. Gamebase ID로 PIER 서버 로그인 
             */
             
             // 게임베이스 초기화
            GameBaseInitialize();
          
            // 디바이스 스펙에 따른 그래픽 퀄리티 설정             
            ChangeQuality();
            
            
        }
        
        /// <summary>
        /// iOS 퀄리티 런타임으로 변경하기
        /// </summary>
        void ChangeQuality() {
            
            
            Debug.Log(string.Format("Current QualityLevel is [{0}]", QualitySettings.GetQualityLevel()));
            
            // iOS만 실행하도록 전처리 
            // iOS는 가장 낮은 퀄리티가 기본으로 세팅 되어있다. 
            // 21.11.09 세상에는 아직도 사양이 낮은 폰을 사용하는 사람이 있기 떄문에 AOS, IOS 할 것 없이 사양 다운을 해준다...
            Debug.Log(">>> System RAM Check :: " + SystemInfo.systemMemorySize);
            
            // 
            if(SystemInfo.systemMemorySize >= 4000) {
                Debug.Log(">> Quality Up");
                
                // 퀄리티 설정 0이 가장 낮은 퀄리티 half res
                // 1은 full res. 
                QualitySettings.SetQualityLevel(1);
            }
            else {
                Debug.Log(">> Quality Down");
                QualitySettings.SetQualityLevel(0);
            }
        }
        

        #region Gamebase Launching 

        /// <summary>
        /// 런칭 정보 가져와서 가공하기.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void CallbackGamebaseLaunching(HTTPRequest req, HTTPResponse res)
        {
            isLaunchingCalled = true;

            if (req.State != HTTPRequestStates.Finished)
            {
                launchingJSON = null;
                return;
            }

            Debug.Log(res.DataAsText);

            launchingJSON = JsonMapper.ToObject(res.DataAsText);

            try
            {
                if (launchingJSON["header"]["isSuccessful"].IsBoolean && bool.Parse(launchingJSON["header"]["isSuccessful"].ToString()) == true)
                {
                    messageRequireUpdate = launchingJSON["launching"]["maintenance"]["message"]["requestUpdate"].ToString();
                    messageTestVersion = launchingJSON["launching"]["maintenance"]["message"]["tester"].ToString();
                }
                else
                {
                    launchingJSON = null;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                launchingJSON = true;
            }
        } // ? CallbackGamebaseLaunching

        #endregion

        #region Gamebase 초기화 

        /// <summary>
        /// 게임베이스 초기화 
        /// </summary>
        void GameBaseInitialize()
        {
            Debug.Log("<color=white>GameBaseInitialize</color>");

            // 두번 초기화를 막는다. 
            if (IsGamebaseInit)
            {
                return;
            }
            
            //AppsFlyerSDK.AppsFlyer.sendEvent("APP_OPEN", null);
            
            // 언어정보 설정!
            SetCurrentLanguageCode(); 
            
            // 디버그 메세지 체크용도 
            Gamebase.SetDebugMode(false);

            Debug.Log("<<< Current Version : " + Application.version);
            isServerValid = true;


            // configuration 설정 (초기화)
            var configuration = new GamebaseRequest.GamebaseConfiguration();
            configuration.appID = gamebaseAPP_ID;
            configuration.appVersion = Application.version; // 어플리케이션 버전(게임베이스 콘솔 등록 필수)
            configuration.displayLanguageCode = currentGamebaseLanguageCode; // Display 언어 코드 
            configuration.enablePopup = true;                                   // Gamebase 제공 팝업 사용 true
            configuration.enableLaunchingStatusPopup = false;                   // Gamebase 제공 점검 팝업 사용 false


#if UNITY_ANDROID
            configuration.storeCode = GamebaseStoreCode.GOOGLE;
#elif UNITY_IOS
            configuration.storeCode = GamebaseStoreCode.APPSTORE;
#endif

            // ! 게임베이스 초기화 호출 
            Gamebase.Initialize(configuration, OnGamebaseInitialize);
        } // ? end of GameBaseInitialize
        
        
        /// <summary>
        /// 게임베이스 Initialize Callback
        /// </summary>
        /// <param name="launchingInfo"></param>
        /// <param name="error"></param>
        void OnGamebaseInitialize(GamebaseResponse.Launching.LaunchingInfo launchingInfo, GamebaseError error) {
            // 초기화 실패했을 경우에 대한 처리. 
            if (!Gamebase.IsSuccess(error))
            {
                // 이 부분에 초기화가 실패해서 접속을 할 수 없다는 안내 멘트 필요
                Debug.Log(string.Format("Gamebase Initialization failed. error is {0}", error));
                ShowSimpleMessagePopUp("Game initialization failed : " + error.message, Application.Quit);

                if (error.code == GamebaseErrorCode.LAUNCHING_UNREGISTERED_CLIENT)
                {
                }

                return; // 끝. 
            } // ? Gamebase Initialize 에러 체크 끝. 
            
            // * 여기서 부터 초기화 이후 로직.
            Debug.Log("Gamebase Initialization succeeded.");
            IsGamebaseInit = true; // 초기화 완료 
            Gamebase.AddEventHandler(GamebaseObserverHandler);
            
            
            // * 게임베이스에서 전달받은 서버 접속 주소 (우리가 등록한 주소)
            // * 게임베이스는 테스트, 베타서비스, 심사중, 서비스 서버 총 4종의 주소를 등록할 수 있도록 하고 있다.
            serverAccessAddress = launchingInfo.launching.app.accessInfo.serverAddress;
            Debug.Log(">> serverAccessAdress : " + serverAccessAddress); // 주소체크 
            NetworkLoader.main.SetURL(serverAccessAddress);
            
            // * 게임 상태 정보 
            var status = launchingInfo.launching.status;

            // 클라이언트 상태 체크
            clientStatus = status.code;
            clientStatusString = status.message;
            // 앱 버전의 게임 상태 정보
            // 상태에 따라서 접속 서버, 업데이트 요청, 점검 등 처리가 필요하다. 
            Debug.Log(string.Format(">> Gamebase Status CODE {0} <<", clientStatus.ToString()));
            
            StartCoroutine(ConnectingGamebase(launchingInfo));
        }
        
        /// <summary>
        /// 게임베이스 초기화 이후, 접속 처리 
        /// </summary>
        /// <returns></returns>
        IEnumerator ConnectingGamebase(GamebaseResponse.Launching.LaunchingInfo launchingInfo) {
            
            // * 로컬라이징, 서버 기준 정보를 먼저 받아온다. 
            // * 폰트 에셋 번들도 이 시점에서 받아야 한다. 
            RequestGameServerInfo();
            RequestAppCommonResources();
        
            // 통신 완료까지 대기 
            while(!isServerInfoReceived || !isAppCommonResourcesReceived)
                yield return null;      
                
            #region Gamebase Launching 
            isLaunchingCalled = false; 
            NetworkLoader.main.RequestGamebaseLaunching(CallbackGamebaseLaunching);
            while (!isLaunchingCalled) // 응답 받을때까지 기다리기. 
                yield return null;
            #endregion
            
            
            // * 상태에 따른 추가 처리 용도 
            switch (clientStatus)
            {
                // 여기는 정상 서비스
                case GamebaseLaunchingStatus.IN_SERVICE: // 정상 서비스 중
                case GamebaseLaunchingStatus.RECOMMEND_UPDATE: // 업데이트 권장
                case GamebaseLaunchingStatus.IN_SERVICE_BY_QA_WHITE_LIST: // 점검 중 QA 단말기의 접속
                    break;

                case GamebaseLaunchingStatus.IN_TEST: // 테스트 버전
                    // 테스트 버전이라는 팝업 알림 처리  
                    // 21.09.02 밴을 걸었을 때, 런칭 상태를 먼저 확인하기 떄문에 밴에 관한 팝업이 뜨지를 않는다
                    //ShowSimpleMessagePopUp(messageTestVersion);
                    break;

                case GamebaseLaunchingStatus.IN_REVIEW: // iOS & Android 심사
                    // 심사할때는 라이브 서버인척 하기. 
                    break;

                case GamebaseLaunchingStatus.REQUIRE_UPDATE: // 업그레이드 필수
                    // 각 스토어 페이지를 열어주고 앱은 종료 처리 
                    ShowSimpleMessagePopUp(messageRequireUpdate, Application.Quit);
                    isServerValid = false;
                    break;

                case GamebaseLaunchingStatus.INSPECTING_ALL_SERVICES: // 점검에 대한 처리 
                case GamebaseLaunchingStatus.INSPECTING_SERVICE:
                    // 점검에 대한 처리 
                    ShowSimpleMessagePopUp(string.Format("{0}\n~\n{1}\n{2}", launchingInfo.launching.maintenance.beginDate, launchingInfo.launching.maintenance.endDate, Uri.UnescapeDataString(launchingInfo.launching.maintenance.message)), Application.Quit);
                    isServerValid = false; 
                    break;


                case GamebaseLaunchingStatus.INTERNAL_SERVER_ERROR: // Error in internal server.
                    ShowSimpleMessagePopUp("Internal Server Error", Application.Quit);
                    isServerValid = false;
                    break;
            } // ? end of switch
            
            // 게임베이스 이용약관 Query.
            QueryGamebaseTerms(); 
            

            // TOAST Analytics 연동하기. 
            InitToastLogger();
            
            if(!isServerValid)
                yield break;
                
            // 게임베이스 초기화 및 접속 종료 후 로그인 처리 
            LoginPlatform();
        } // ? ConnectingGamebase
        
        #endregion
        

        #region 게임서버 기준정보 
        // 데이터를 매번 받아오지 않고, 각 데이터 종류마다 버전을 갖고있다. 
        // 디바이스에 저장된 버전과 비교해서 버전이 다른 경우에만 데이터를 요청할 것.
        // 로컬라이징 텍스트 추가 (2021.08.31)
        
        
        /// <summary>
        /// 앱 공용 리소스 다운받기 
        /// </summary>
        void RequestAppCommonResources() {
            
            isAppCommonResourcesReceived = false;
            
            JsonData reqData  = new JsonData();
            reqData["func"] = "getAppCommonResources";
            
            NetworkLoader.main.SendPost(OnRequestAppCommonResources, reqData, false);
        }
        
        void OnRequestAppCommonResources(HTTPRequest request, HTTPResponse response ) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            
            appComonResourceData = JsonMapper.ToObject(response.DataAsText);
            
            // * appComonResourceData.models 앱의 공용 모델들. 2021.09.14
            
            isAppCommonResourcesReceived = true;
        }
        
    
        /// <summary>
        /// 게임서버 기준정보 받아오기 
        /// 2021.08.31 : 로컬라이징 텍스트 리스트 
        /// </summary>
        /// <returns></returns>
        void RequestGameServerInfo() {
            
            isServerInfoReceived = false; 
            
            JsonData reqData  = new JsonData();
            reqData["func"] = "getServerInfo";
            
            NetworkLoader.main.SendPost(OnRequestGameServerInfo, reqData, false);
        }
        
        
        /// <summary>
        /// 게임서버 기준정보 통신 완료 
        /// </summary>
        void OnRequestGameServerInfo(HTTPRequest request, HTTPResponse response ) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            // 버전 비교를 시작한다.
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            int serverlocalVer = int.Parse(result["local_ver"].ToString()); // 서버 로컬라이징 텍스트 버전
            
            
            // 코인 응모권 시스템 관련 (서버에서 받아온다.)
            isCoinPrizeUse = GetJsonNodeBool(result, "coin_url_use");
            coinPrizeURL = GetJsonNodeString(result, "coin_url"); // URL
            
            // 없는 리소스 허용 여부
            allowMissingResource = GetJsonNodeBool(result, "allow_missing_resource");

            // 일일 최대 무료충전 횟수 
            maxAdCharge = int.Parse(GetJsonNodeString(result, "max_ad_charge"));

            // 디바이스 정보 불러다놓고, 
            localVer = GetDeviceLocalVer();
            localizedTextJSON = GetDeviceLocalData();
            
            Debug.Log(string.Format("device local ver : [{0}] / server local ver : [{1}]", localVer, serverlocalVer));
            
            // 비교해서 다르면 서버한테 텍스트 정보를 요청한다.
            // 버전 똑같고, 데이터도 잘 있으면 아무것도 안함.
            if(localVer != serverlocalVer || localizedTextJSON == null) {
                
                RequestLocalizedText(serverlocalVer);
                return;    
            }
            
            
            // 다음 과정을 진행해도 된다고 알림.
            isServerInfoReceived = true;
        }
        
        /// <summary>
        /// 로컬라이징 텍스트 데이터를 주세요!
        /// </summary>
        void RequestLocalizedText(int __serverLocalVer) {
            JsonData reqData = new JsonData();
            reqData["func"] = "getClientLocallizingList";
            reqData["serverLocalVer"] = __serverLocalVer;
            
            NetworkLoader.main.SendPost(OnRequestLocalizedText, reqData, false);
        }
        
        /// <summary>
        /// 로컬라이징 텍스트를 받았어요!
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>        
        void OnRequestLocalizedText(HTTPRequest request, HTTPResponse response ) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            // 데이터 받는다. 
            localizedTextJSON = JsonMapper.ToObject(response.DataAsText);
            
            // requestData에서 버전 다시 받아놓는다. 
            JsonData reqData = JsonMapper.ToObject(Encoding.UTF8.GetString(request.RawData));
            localVer = int.Parse(reqData["serverLocalVer"].ToString());
            
            // 디바이스에 저장한다.
            SaveServerInfoToDevice();
            
            // 로그인 과정을 더 진행해도 된다고 알려준다.
            isServerInfoReceived = true;
        }
        
        
        /// <summary>
        /// 서버 기준정보를 디바이스에 저장한다. (말풍선 세트와 유사하다)
        /// </summary>
        void SaveServerInfoToDevice() {
            
            // 로컬라이징 텍스트 버전 저장
            ES3.Save<int>(SystemConst.KEY_LOCAL_VER, localVer);
            ES3.Save<string>(SystemConst.KEY_LOCAL_DATA, JsonMapper.ToJson(localizedTextJSON));
        }
        
        /// <summary>
        /// 디바이스의 로컬라이징 텍스트 버전 정보를 알려주세요 제발 
        /// </summary>
        /// <returns></returns>
        int GetDeviceLocalVer() {
            
            return ES3.Load<int>(SystemConst.KEY_LOCAL_VER, 0);

        }
        
        /// <summary>
        /// 디바이스에 저장된 텍스트 데이터 가져오기 
        /// </summary>
        /// <returns></returns>
        JsonData GetDeviceLocalData() {
            
            if(!ES3.KeyExists(SystemConst.KEY_LOCAL_DATA))
                return null;
            
            JsonData result = null;
            
            try {
                 result = JsonMapper.ToObject(ES3.Load<string>(SystemConst.KEY_LOCAL_DATA));
            }
            catch {
                return null;
            }
            
            return result;
        }
        
        /// <summary>
        /// 언어정보 세팅하기.
        /// </summary>
        void SetCurrentLanguageCode() {
            
            // 저장된 언어정보가 없을때. 
            if(!ES3.KeyExists(SystemConst.KEY_LANG)) {
                
                switch(Application.systemLanguage) {
                    case SystemLanguage.Korean:
                    currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.Korean; // ko
                    break;
                    
                    case SystemLanguage.English:
                    currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.English; // en
                    break;
                    
                    case SystemLanguage.Japanese:
                    currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.Japanese; // ja
                    break;
                    
                    case SystemLanguage.ChineseSimplified:
                    currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.Chinese_Simplified; // zh-CN
                    break;
                    
                    case SystemLanguage.ChineseTraditional:
                    currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.Chinese_Traditional; // zh-TW
                    break;
                    
                    default:
                    currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.English; // 기본값 영어
                    break;
                } // ? end of switch
                
                ES3.Save<string>(SystemConst.KEY_LANG, currentGamebaseLanguageCode); // 저장한다. 
            }
            
            currentGamebaseLanguageCode = ES3.Load<string>(SystemConst.KEY_LANG);
            
            // 게임베이스와 서버에서 사용하는 언어코드가 서로 다르기 때문에 컨버팅 해준다. 
            
            // 중국어는 바꿔줘야한다. 특별처리.
            if(currentGamebaseLanguageCode == GamebaseDisplayLanguageCode.Chinese_Simplified)  // 간체
                currentAppLanguageCode = "ZH";
            else if(currentGamebaseLanguageCode == GamebaseDisplayLanguageCode.Chinese_Traditional)  // 번체
                currentAppLanguageCode = "TC";
            else 
                currentAppLanguageCode = currentGamebaseLanguageCode.ToUpper(); // 나머지는 대문자로 바꿔주면 끝!
        }
        
        
        #endregion


        /// <summary>
        /// 플랫폼 로그인 처리, 게임베이스 로그인 → 서버 로그인 과정으로 진행 
        /// </summary>
        public void LoginPlatform(bool isForceGuest = false)
        {
            // 마지막에 진행한 로그인 방법을 가져와서 실행합니다.(GUEST, Google, Apple)
            string lastLoggedInProvider = Gamebase.GetLastLoggedInProvider();
            Debug.Log(string.Format("Gamebase Last Logged Provider [{0}]", lastLoggedInProvider));

            // 에디터에서 실행되거나, 지난 로그인 기록이 없는 경우 게스트로 로그인 처리 
            if (string.IsNullOrEmpty(lastLoggedInProvider) || Application.isEditor || lastLoggedInProvider == "guest" || isForceGuest)
            {
                Debug.Log("Gamebase Guest Login <<<<<");

                Gamebase.Login(GamebaseAuthProvider.GUEST, OnCallbackLogin);
                // AppsFlyerSDK.AppsFlyer.sendEvent("USER_UID", null);
                return;
            }

            // LoginForLastLoggedInProvider 호출 , OnCallbackLogin으로 가기!
            Gamebase.LoginForLastLoggedInProvider(OnCallbackLogin);
        }

        /// <summary>
        /// 로그인 콜백!
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="error"></param>
        void OnCallbackLogin(GamebaseResponse.Auth.AuthToken authToken, GamebaseError error)
        {
            // error 처리 
            if (!Gamebase.IsSuccess(error))
            {
                // Check the error code and handle the error appropriately.
                Debug.Log(string.Format("Login failed. error is {0}", error));
                //ShowSimpleMessagePopUp(string.Format("Login failed. error is {0}", error));

                if (error.code == (int)GamebaseErrorCode.SOCKET_ERROR || error.code == (int)GamebaseErrorCode.SOCKET_RESPONSE_TIMEOUT)
                {
                    Debug.Log(string.Format("Retry Login or notify an error message to the user. : {0}", error.message));
                    ShowSimpleMessagePopUp("로그인 서버가 응답하지 않습니다. 다시 로그인을 시도합니다.");
                    LoginPlatform();
                }
                else if (error.code == GamebaseErrorCode.BANNED_MEMBER)
                {
                    GamebaseResponse.Auth.BanInfo banInfo = GamebaseResponse.Auth.BanInfo.From(error);
                    if (banInfo != null)
                    {
                        //ShowSimpleMessagePopUp("정지된 계정입니다.");
                        Application.Quit();
                    }
                }
                else {
                    // ShowSimpleMessagePopUp("로그인 서버가 응답하지 않습니다. 다시 로그인을 시도합니다.");
                    LoginPlatform(true); // 강제로 게스트 로그인 시도.
                }

                return;
            } // error 처리 끝

            
            // 게임베이스 ID 받아옵니다. 
            gamebaseID = authToken.member.userId;
            Debug.Log(string.Format("<color=yellow>Login succeeded. Gamebase userId is [{0}] </color>", gamebaseID));


            // 서버 로그인을 진행합니다. (게임베이스 ID를 기준으로 로그인처리)
            // 이후 UserManager에서 PIER 서버 접속을 처리하게 되며, 로그인 완료시에 Lobby 진입 이벤트 호출(UserManager.CallbackConnectServer)
            UserManager.main.InitUser(gamebaseID);
        }

        void GamebaseObserverHandler(GamebaseResponse.Event.GamebaseEventMessage message)
        {
            switch (message.category)
            {
                case GamebaseEventCategory.SERVER_PUSH_APP_KICKOUT:
                case GamebaseEventCategory.SERVER_PUSH_TRANSFER_KICKOUT:
                    {
                        GamebaseResponse.Event.GamebaseEventServerPushData serverPushData = GamebaseResponse.Event.GamebaseEventServerPushData.From(message.data);
                        if (serverPushData != null)
                        {
                            // 팝업이 뜬 이후 닫기를 눌렀을 때, 호출이 된다
                            Debug.Log("extras : " + serverPushData.extras);
                            Application.Quit();
                            
                        }
                        break;
                    }

                case GamebaseEventCategory.OBSERVER_HEARTBEAT:
                    GamebaseResponse.Event.GamebaseEventObserverData observerData = GamebaseResponse.Event.GamebaseEventObserverData.From(message.data);
                    if(observerData != null)
                    {
                        // 탈퇴한 회원? 존재하지 않는 회원?일 경우
                        if (observerData.code.Equals(GamebaseErrorCode.INVALID_MEMBER))
                            Debug.Log("존재하지 않는 회원입니다");
                        else if(observerData.code.Equals(GamebaseErrorCode.BANNED_MEMBER))
                        {
                            Gamebase.Util.ShowAlert("BAN_MEMBER", observerData.message);
                            Application.Quit();
                        }
                    }
                    break;
            }
        }


        /// <summary>
        /// 로그인 후 약관 동의 팝업 등장
        /// </summary>
        public void ShowAgreementTermsPopUp()
        {
            Debug.Log("ShowAgreementTermsPopUp");
            
            // 에디터에서는 지원하지 않음.
            if(Application.isEditor)
                return;
        
            
            // 이용약관, 개인정보처리방침을 수락했다면 key값이 존재하고, getInt 값도 1이다.
            /*
            if (PlayerPrefs.HasKey("useTerms") && PlayerPrefs.HasKey("privacy") && PlayerPrefs.GetInt("useTerms") > 0 || PlayerPrefs.GetInt("privacy") > 0)
                return;

            LobbyManager.main.termView = true;
            */
            
            // 사실 뷰.  
            // Doozy.Engine.GameEventMessage.SendEvent("EventAgreementTerms");
            
            // * 2021.10.13 게임베이스 호출로 변경.
            Gamebase.Terms.ShowTermsView((data, error) => 
            {
                if (Gamebase.IsSuccess(error) == true)
                {
                    Debug.Log("ShowTermsView succeeded : " + data.ToString());
                    // AppsFlyerSDK.AppsFlyer.sendEvent("APP_TERMSOFUSE", null);
                    // AppsFlyerSDK.AppsFlyer.sendEvent("APP_PERSONALINFORMATION", null);

                    // If the 'PushConfiguration' is not null,
                    // save the 'PushConfiguration' and use it for Gamebase.Push.RegisterPush() after Gamebase.Login().
                    GamebaseResponse.Push.PushConfiguration pushConfiguration = GamebaseResponse.Push.PushConfiguration.From(data);
                    
                    // Register 처리 .
                    PushRegister(pushConfiguration.adAgreement, pushConfiguration.adAgreementNight);
                }
                else
                {
                    Debug.Log(string.Format("ShowTermsView failed. error:{0}", error));
                }
            });
            
        }
        
        int gamebaseTermsSeq = -1;
        string gamebaseTermsVersion = string.Empty;
        List<GamebaseResponse.Terms.ContentDetail> listGamebaseTerms;
        
        /// <summary>
        /// 게임베이스에서 이용약관 조회하기.
        /// </summary>
        public void QueryGamebaseTerms() {
            
            if(Application.isEditor)
                return;
            
            
            Gamebase.Terms.QueryTerms((data, error) => 
            {
                if (Gamebase.IsSuccess(error) == true)
                {
                    
                    gamebaseTermsSeq = data.termsSeq; 
                    gamebaseTermsVersion = data.termsVersion;
                    
                    Debug.Log(string.Format("QueryTerms succeeded. [{0}]/[{1}]", gamebaseTermsSeq, gamebaseTermsVersion));
                    
                    listGamebaseTerms = data.contents;
                    
                    for(int i=0 ;i<listGamebaseTerms.Count;i++) {
                        Debug.Log(string.Format("[{0}] : [{1}]", listGamebaseTerms[i].termsContentSeq, listGamebaseTerms[i].name));
                    }
                }
                else
                {
                    Debug.Log(string.Format("QueryTerms failed. error:{0}", error));
                    listGamebaseTerms = null;
                    gamebaseTermsSeq = -1;
                    gamebaseTermsVersion = string.Empty;
                }
            });
        }


        /// <summary>
        /// 약관 및 푸시 내용 update
        /// </summary>
        /// <param name="adPush">(광고) 푸시 알림</param>
        /// <param name="nightAdPush">야간 푸시 알림</param>
        /// <param name="allEssential">모든 필수 약관</param>
        /// <param name="userTerms">이용약관</param>
        /// <param name="privacyTerms">개인정보처리 방침</param>
        public void SubmitQuery(bool adPush, bool nightAdPush, bool allEssential = true, bool userTerms = true, bool privacyTerms = true)
        {

            // 에디터에서는 푸시나 약관 정보에 대해서 update하지 않는다.
            if (Application.isEditor)
                return;
                
            if(gamebaseTermsSeq < 0) {
                Debug.Log("Failed to get gamebase terms info");
                return;
            }
            
            if(listGamebaseTerms.Count == 5) {
                
            }
            
            for(int i=0; i<listGamebaseTerms.Count;i++) {
                
            }
            
            
            List<GamebaseRequest.Terms.Content> contentList = new List<GamebaseRequest.Terms.Content>();

            contentList.Add(new GamebaseRequest.Terms.Content()
            {
                termsContentSeq = 255,
                agreed = allEssential
            });

            contentList.Add(new GamebaseRequest.Terms.Content()
            {
                termsContentSeq = 256,
                agreed = userTerms
            });

            contentList.Add(new GamebaseRequest.Terms.Content()
            {
                termsContentSeq = 257,
                agreed = privacyTerms
            });

            contentList.Add(new GamebaseRequest.Terms.Content()
            {
                termsContentSeq = 258,
                agreed = adPush
            });

            contentList.Add(new GamebaseRequest.Terms.Content()
            {
                termsContentSeq = 259,
                agreed = nightAdPush
            });
            
            

            Gamebase.Terms.UpdateTerms(new GamebaseRequest.Terms.UpdateTermsConfiguration()
            {
                termsSeq = gamebaseTermsSeq,
                termsVersion = gamebaseTermsVersion,
                contents = contentList
            }, (error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    Debug.Log("Success");
                    PlayerPrefs.SetInt("useTerms", 1);
                    PlayerPrefs.SetInt("privacy", 1);

                    // AppsFlyerSDK.AppsFlyer.sendEvent("APP_TERMSOFUSE", null);
                    // AppsFlyerSDK.AppsFlyer.sendEvent("APP_PERSONALINFORMATION", null);

                    // 푸쉬 알림에 대한 설정
                    PushRegister(adPush, nightAdPush);
                }
                else
                    Debug.LogError(string.Format("UpdateTerms fail. error : {0}", error));
            });
        }

        /// <summary>
        /// 푸시 설정
        /// </summary>
        /// <param name="adPush">주간 광고성 푸시</param>
        /// <param name="nightAdPush">야간 광고성 푸시</param>
        public void PushRegister(bool adPush, bool nightAdPush)
        {
            Debug.Log(string.Format("PushRegister : ad[{0}], night[{1}]", adPush, nightAdPush));
            
            GamebaseRequest.Push.PushConfiguration pushConfiguration = new GamebaseRequest.Push.PushConfiguration();
            pushConfiguration.pushEnabled = true;
            pushConfiguration.adAgreement = adPush;
            pushConfiguration.adAgreementNight = nightAdPush;

            var notificationOptions = new GamebaseRequest.Push.NotificationOptions
            {
                foregroundEnabled = true, // 앱 실행중에도 푸시 메세지 노출
                priority = GamebaseNotificationPriority.HIGH
            };

            // 토큰 등록 진행 
            Gamebase.Push.RegisterPush(pushConfiguration, notificationOptions, (error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    Debug.Log("RegisterPush succeeded.");

                    /*
                    if(adPush)
                        AppsFlyerSDK.AppsFlyer.sendEvent("APP_ADINFORM", null);

                    if(nightAdPush)
                        AppsFlyerSDK.AppsFlyer.sendEvent("APP_NIGHTADINFORM", null);
                    */

                }
                else
                {
                    // 오류 처리, firebase의 google-service.json 을 xml로 컨버팅 했는지 체크할것.
                    Debug.Log(string.Format("code:{0}, message:{1}", error.code, error.message));

                    GamebaseError moduleError = error.error; // GamebaseError.error object from external module
                    if (null != moduleError)
                    {
                        int moduleErrorCode = moduleError.code;
                        string moduleErrorMessage = moduleError.message;

                        Debug.Log(string.Format("moduleErrorCode:{0}, moduleErrorMessage:{1}", moduleErrorCode, moduleErrorMessage));
                    }
                }
            });
        }
        
        
        /// <summary>
        /// 슬라이드 메뉴에서 게임베이스 로그인 
        /// </summary>
        /// <param name="__idp"></param>
        public void LoginGamebaseBySlide(string __idp) {
            
            Debug.Log("## LoginGamebaseBySlide : " + __idp);
            
            // 게임베이스 로그인 상태에서 또 로그인이 불가능하기 때문에 기존을 로그아웃 한다. 
            Gamebase.Logout((error) => {
                
                // 설마 로그아웃을 실패하진 않겠지!
                if(!Gamebase.IsSuccess(error)) {
                    Debug.Log(string.Format("# Logout failed. error is {0}", error));
                    ShowSimpleMessagePopUp(string.Format("[{0}] {1}", error.code, error.message));
                    return;
                }
                
                // 지정한 idp로 로그인 시도. 
                Gamebase.Login(__idp, (authToken, error) => {
                
                    
                    if(!Gamebase.IsSuccess(error)) {
                        Debug.Log(string.Format("#### Login failed. error is {0}", error));
                        ShowSimpleMessagePopUp(string.Format("[{0}] {1}", error.code, error.message));
                        
                        
                        // 로그인 복원
                        Gamebase.Login(GamebaseAuthProvider.GUEST, OnCallbackLogin);
                        
                        return;
                    }
               
                    // 로그인 성공 
                    string newGamebaseID = Gamebase.GetUserID();
                    Debug.Log(string.Format("#### New Gamebase ID = [{0}]", newGamebaseID));
                    
                    // 로그인에 성공했으면 현재 GamebaseID로 userkey를 조회한다. 
                    JsonData reqData = new JsonData();
                    reqData["func"] = "checkAccountExistsByGamebase";
                    reqData["gamebaseID"] = newGamebaseID;
                    
                    NetworkLoader.main.SendPost(OnRequest__checkAccountExistsByGamebase, reqData, true);
                
                });
                
            });
            
            
        }
        
        /// <summary>
        /// checkAccountExistsByGamebase callback
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void OnRequest__checkAccountExistsByGamebase(HTTPRequest request, HTTPResponse response) {

        }
        
        /// <summary>
        /// 계정 연동 취소. 
        /// </summary>        
        void CancleAccountConnect() {
            Debug.Log("##### CancleAccountConnect");
            
            // ! 여기서 이제 어떻게 하지? 이미 login 되어있는데..? 
            Gamebase.Logout((error) => {
                if (Gamebase.IsSuccess(error))
                {
                    Debug.Log("##### Logout succeeded.");
                    
                    // 다시 게스트로 로그인.. 
                    // 이전 계정으로 복귀되는지 확인 
                    Gamebase.Login(GamebaseAuthProvider.GUEST, (authToken, error)=>{
                        if (Gamebase.IsSuccess(error)) {
                            Debug.Log(string.Format("### Login succeeded. Gamebase ID is {0}", Gamebase.GetUserID()));
                        }
                        else {
                            Debug.Log(string.Format("##### Login failed. error is {0}", error));
                        }
                    });
                }
                else
                {
                    Debug.Log(string.Format("Logout failed. error is {0}", error));
                }
               
            });
            
        }
        
        /// <summary>
        /// 연결된 이전 계정으로 재 로그인
        /// </summary>
        void ConfirmPreviousAccountLoad() {
            Debug.Log("##### ConfirmPreviousAccountLoad");
            
            // 코루틴에서 처리 
            // StartCoroutine(RoutineLoadingConnectedAccount());
       }
        


        
        public void Logout()
        {
            Gamebase.Logout((error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    Debug.Log("Logout succeeded.");
                }
                else
                {
                    Debug.Log(string.Format("Logout failed. error is {0}", error));
                }
            });
        }
        
        
        #region Gamebase IDP Mapping 
        
        public void AddMapping(string providerName)
        {
            Debug.Log(">>> Start AddMapping");
            
            Gamebase.AddMapping(providerName, (authToken, error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    Debug.Log("AddMapping succeeded.");
                    ShowSimpleMessagePopUp("연동 계정 변경이 완료되었습니다.");
                }
                else
                {
                    Debug.Log(string.Format("AddMapping failed. error is {0}", error));
                    ShowSimpleMessagePopUp(error.message);
                }
            });
        }        
        
        #endregion
        



        /// <summary>
        /// TOAST 로그 시스템
        /// </summary>
        void InitToastLogger()
        {
            Debug.Log("TOAST Logger Call");
            var config = new GamebaseRequest.Logger.Configuration(gamebaseLogger_ID);
            Gamebase.Logger.Initialize(config);
        }




#region 시스테 로컬 설정(BGM,효과음 등) 

        const string BGM_SETTING = "bgm_setting";
        const string SE_SETTING = "se_setting";

        /// <summary>
        /// 배경음 설정
        /// </summary>
        /// <returns></returns>
        public bool GetBgmSetting()
        {
            return PlayerPrefs.GetInt(BGM_SETTING, 1) > 0 ? true : false;
        }

        /// <summary>
        /// 효과음 설정 
        /// </summary>
        /// <returns></returns>
        public bool GetSoundSetting()
        {
            return PlayerPrefs.GetInt(SE_SETTING, 1) > 0 ? true : false;
        }

        /// <summary>
        /// 설정 저장 
        /// </summary>
        /// <param name="__flag"></param>
        public void SetBgmSetting(bool __flag)
        {
            PlayerPrefs.SetInt(BGM_SETTING, __flag ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetSoundSetting(bool __flag)
        {
            PlayerPrefs.SetInt(SE_SETTING, __flag ? 1 : 0);
            PlayerPrefs.Save();
        }

#endregion
        
        /// <summary>
        /// JSON 특정 노드의 노드 값을 알려주쇼
        /// </summary>
        /// <param name="__node"></param>
        /// <param name="__col"></param>
        /// <returns></returns>
        public static JsonData GetJsonNode(JsonData __node, string __col) {
            if (!__node.ContainsKey(__col))
                return null;
                
            if (__node[__col] == null)
                return null;
                
            return __node[__col];
        }
        

        /// <summary>
        /// JSON 특정 노드의 string 값을 알려주세요
        /// </summary>
        /// <param name="__node"></param>
        /// <param name="__col"></param>
        /// <returns></returns>
        public static string GetJsonNodeString(JsonData __node, string __col)
        {
           
            if (__node == null || !__node.ContainsKey(__col))
                return string.Empty;


            if (__node[__col] == null)
                return string.Empty;

            return __node[__col].ToString();
            
        }
        
        /// <summary>
        /// 특정 노드의 bool 값을 알려주세요.
        /// </summary>
        /// <param name="__node"></param>
        /// <param name="__col"></param>
        /// <returns></returns>
        public static bool GetJsonNodeBool(JsonData __node, string __col) {
            
            if (__node == null || !__node.ContainsKey(__col)) {
                Debug.Log(string.Format("Error : {0} is not a node", __col));
                return false;
            }
                
            if (__node[__col] == null) {
                Debug.Log(string.Format("Error : {0} is null", __col));
                return false;
            }
            
            return __node[__col].ToString() == "1" ? true : false;
        }
        
        
        /// <summary>
        /// 네트워크 로딩 팝업 오픈 
        /// </summary>
        public static void ShowNetworkLoading()
        {

        }

        /// <summary>
        /// 네트워크 로딩 팝업 해제 
        /// </summary>
        public static void HideNetworkLoading()
        {
            if (main.networkLoadingScreen)
                main.networkLoadingScreen.OffNetworkLoading();
        }

        /// <summary>
        /// 네트워크 로딩스크린이 떠있는지 체크 
        /// </summary>
        /// <returns></returns>
        public static bool CheckNetworkLoading()
        {
            if (main.networkLoadingScreen == null)
                return false;

            return main.networkLoadingScreen.gameObject.activeSelf;
        }
        
        
        /// <summary>
        /// 중앙 경고창( 자동 사라짐)
        /// </summary>
        /// <param name="__message"></param>
        public static void ShowAlert(string __message) {

        }
        
        public static void ShowAlertWithLocalize(string __textID) {

        }


        /// <summary>
        /// 유저의 응답(YES/NO)를 받는 메세지 창을 생성합니다. 
        /// </summary>
        /// <param name="__message"></param>
        /// <param name="__positive"></param>
        public static void ShowConfirmPopUp(string __message, UnityEngine.Events.UnityAction __positive, UnityEngine.Events.UnityAction __negative, bool hideOnClickOverlay = true)
        {

            // 중복 방지 

        }


        public static void ShowSimpleMessagePopUp(string __message)
        {
            ShowSimpleMessagePopUp(__message, null);
        }
        
        /// <summary>
        /// 로컬라이즈 메세지를 이용한 단순 메세지 팝업 호출
        /// </summary>
        /// <param name="__textID"></param>
        public static void ShowSimpleMessagePopUpWithLocalize(string __textID, UnityEngine.Events.UnityAction __callback = null) {
            ShowSimpleMessagePopUp(GetLocalizedText(__textID), __callback);
        }

        /// <summary>
        /// 원버튼의 단순 메세지 팝업 호출 
        /// </summary>
        /// <param name="__message"></param>
        public static void ShowSimpleMessagePopUp(string __message, UnityEngine.Events.UnityAction __callback)
        {


        }


        public void ShowMissingFunction(string __message)
        {

        }

        #region 파일 체크, 파일 다운로드 공용
        
        /// <summary>
        /// 파일 로컬 저장 되어있는지 체크 
        /// </summary>
        /// <param name="__key"></param>
        /// <returns></returns>
        public static bool CheckFileExists(string __key) {
            if(!ES3.FileExists(__key))
                return false;
                
            return true;
        }
        
        /// <summary>
        /// 로컬 텍스쳐 불러오기 
        /// </summary>
        /// <param name="__key"></param>
        /// <returns></returns>
        public static Texture2D GetLocalTexture2D(string __key) {
            if(!ES3.FileExists(__key) || string.IsNullOrEmpty(__key))
                return null;
            
            return ES3.LoadImage(__key);
        }
        
        /// <summary>
        /// 이미지 파일 다운로드 요청 
        /// </summary>
        /// <param name="__url">image url</param>
        /// <param name="__key">image key</param>
        /// <param name="__cb">callback</param>
        public static void RequestDownloadImage(string __url, string __key, OnRequestFinishedDelegate __cb)  {
            
            // url, key 체크 
            if(string.IsNullOrEmpty(__url) || string.IsNullOrEmpty(__key))
                return;
                
            // 파일 이미 있으면 다운받지 않음.
            if(ES3.FileExists(__key)) 
                return;
            
            OnRequestFinishedDelegate cb = OnCompleteDownloadImage;
            
            if(__cb != null)
                cb += __cb; // chain으로 묶는다. 
            
            // 파일이 없으면 network로 불러온다.
            var req = new HTTPRequest(new System.Uri(__url), cb);
            req.Tag = __key; // key tag로 연결 
            req.Send();
        }
        
        static void OnCompleteDownloadImage(HTTPRequest request, HTTPResponse response) {
            
            if(!NetworkLoader.CheckResponseValidation(request, response))
                return;
            
            // 이미지 로컬 저장은 여기서 수행 
                
            // 성공시, 로컬 저장 
            if(request.State == HTTPRequestStates.Finished && response.IsSuccess) {
                ES3.SaveRaw(response.Data, request.Tag.ToString());
            }
        }
        
        #endregion
        
        /// <summary>
        /// 로컬라이징 텍스트 얻어오기 (아직 미완성)
        /// </summary>
        /// <param name="__id">텍스트ID</param>
        /// <returns></returns>
        public static string GetLocalizedText(string __id) {
            
            // 데이터가 없거나, id가 없을때. value 값이 null이 왔을 떄
            if(localizedTextJSON == null || !localizedTextJSON.ContainsKey(__id) || string.IsNullOrEmpty(__id))
                return string.Empty;
            
            return localizedTextJSON[__id][main.currentAppLanguageCode].ToString();
        }
        
        
        public static void LoadLobbyScene() {
            SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single).allowSceneActivation = true;
        }

        private void OnApplicationQuit()
        {

        }
        
        
        /// <summary>
        /// 스페셜 에피소드 해금 메세지 팝업 처리 
        /// </summary>
        /// <param name="__j">해금된 에피소드 array</param>
        public void ShowUnlockSidePopUp(JsonData __j)
        {

        }
         
         /// <summary>
        /// 확인받고, 게임베이스 탈퇴처리 진행.
        /// </summary>
        public void WithdrawGamebase()
        {
            Gamebase.Withdraw((error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    Debug.Log("Withdraw succeeded.");

                    NetworkLoader.main.UpdateWithdrawDate(); // 게임서버 통신 호출 
                    // 바이바이 팝업창 
                    SystemManager.ShowSimpleMessagePopUpWithLocalize("6008", Application.Quit); // 탈퇴, 감사합니다. 앱 종료 
                }
                else
                {
                    Debug.Log(string.Format("Withdraw failed. error is {0}", error));

                    SystemManager.ShowSimpleMessagePopUp(error.message);
                }
            });
        }
         
    }
}