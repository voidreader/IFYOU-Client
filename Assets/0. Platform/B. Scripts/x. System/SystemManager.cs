using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using TMPro;
using LitJson;
using BestHTTP;
using Toast.Gamebase;
using Doozy.Runtime.Signals;

// Live2D
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.MotionFade;

using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

using RTLTMPro;

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
        public static bool noticePopupExcuted = false; // 한번이라도 공지 팝업이 실행되면 true로 변경됨
        public static bool isQuitGame = false; // 이전 게임 중단 여부 
        public bool isServerValid = false; // 업데이트 필수, 점검 
        public bool isAddressableCatalogUpdated = false; // 어드레서블 카탈로그 업데이트 여부 

        
        
        public WebView webView = null; // 웹뷰. 
        public bool isWebViewOpened = false; // 웹뷰가 현재 보여지고 있는지 체크 
        
        public static float screenRatio = 0;
        public bool isTestServerAccess = false; // 테스트 서버로 연결 여부 
        public string gamebaseAPP_ID = ""; // 6swpd3Jp (피어), qtV3HLW5(코원)
        public string gamebaseLogger_ID = ""; // 6WMxzJjo6i5Z5iXm(피어)
        
        
        // 언어 코드 추가 
        public string currentGamebaseLanguageCode = "en"; // 게임베이스 언어코드
        public string currentAppLanguageCode = "EN"; // 앱, 서버 사용하는 언어코드 (게임베이스와 살짝 다르기 때문에 컨버팅 필요)



        [SerializeField] NetworkLoadingScreen networkLoadingScreen = null; // 네트워크 로딩 스크린



        // 게임베이스 콘솔에 등록된 클라이언트의 상태 정보 
        [Space]
        [SerializeField] int clientStatus = GamebaseLaunchingStatus.IN_SERVICE;
        [SerializeField] string clientStatusString = string.Empty;
        [SerializeField] string serverAccessAddress = string.Empty; // 게임베이스에서 전송받은 서버 접속 주소 
        [SerializeField] string gamebaseID = string.Empty;
        
        public GamebaseResponse.Push.TokenInfo pushTokenInfo = null; // 게임베이스 푸시 토큰 정보 


        // 스크린 관련 정보
        [SerializeField] Rect safeArea;
        public bool hasSafeArea = false;

        bool isLaunchingCalled = false; //  런칭정보 불러왔는지? 
        public JsonData launchingJSON = null; // 게임베이스 런칭 
        public StoryData givenStoryData = null; // 선택된 스토리 JSON 데이터 (프로젝트)
        public EpisodeData givenEpisodeData = null; // 로비에서 전달받은 에피소드 데이터 (에피소드)
        
        JsonData baseCurrencyData = null; // 기본 재화 데이터 

        
        #region 기준정보 Variables
        
    
        [SerializeField] bool isServerInfoReceived = false; // 서버 기준 정보 전달받았는지 체크 한다. (2021.08.31)
        
        
        JsonData timedealStandard = null; // 타임딜 기준정보 
        [SerializeField] int localVer = 0; // 로컬라이징 텍스트 버전
        
        public bool useProjectNotify = false; // 프로젝트 알림설정 사용여부 
        
        public bool allowMissingResource = true; // 에피소드 진입시 없는 리소스 허용 여부 2021.11.08
        
        
        public string coinShopURL = string.Empty; // 코인샵 URL
        public string surveyUrl = string.Empty;
        public int firsetResetPrice = 0; // 최초 리셋 가격
        
        public int episodeOpenPricePer = 0; // 에피소드 시간단축오픈 10분당 코인 가격 
        public int waitingReduceTimeAD = 0; // 광고보고 차감되는 에피소드 열림시간. (분)
        public int removeAdPrice = 100; // 에피소드 광고 제거 비용 (코인)
        
        
        // 개인정보 보호 정책 및 이용약관 URL
        string privacyURL = string.Empty;
        string termsOfUseURL = string.Empty;
        public string bundleURL = string.Empty; // 에셋번들 URL
       
        public string contentsURL = string.Empty; // 리소스 다운로드 URL 
        string copyrightURL = string.Empty; // 저작권 URL
        
        
        // * 어드레서블 정보 
        public string addressable_version = "0";
        public string addressable_url = string.Empty;
        
        
        static JsonData localizedTextJSON = null; // 로컬라이징 텍스트 JSON
        
        #endregion

        
        #region getPlatformEvents 에서 가져오는 데이터들         
        public JsonData storyGenreData = null; // 공개된 작품 장르
        public JsonData promotionData = null;       // 프로모션 데이터
        public JsonData noticeData = null;          // 공지사항 데이터 
        public JsonData introData = null; // 인트로 정보 
        #endregion

        
        
        const string KEY_ENCRYPTION = "imageEncrypt_2"; // 암호화 여부 
        
        #region 내장폰트, 에셋번들 폰트
        public bool isApplicationFontAvailable = false;
        [SerializeField] TMP_FontAsset innerFontEN = null;
        
        // * 에셋번들로 받은 폰트
        // * 한글 영어는 같이 씀.         
        public TMP_FontAsset mainAssetFont = null; // 한글
        public TMP_FontAsset jaFont = null; // 일본어 폰트
        public TMP_FontAsset koFont = null; // 한글, 영어 폰트 
        public TMP_FontAsset arFont = null; // 아랍 폰트
        public TMP_FontAsset arNormalBubbleFont = null; // 아랍어 일반 말풍선 폰트 
        public AsyncOperationHandle<TMP_FontAsset> mountedAssetFontJA; 
        public AsyncOperationHandle<TMP_FontAsset> mountedAssetFontKO; 
        Shader assetFontShader;

        #endregion
        
        #region 공용 스프라이트
        [Space][Header("공용 스프라이트")]
        public Sprite spriteCoin;       // 60 사이즈 코인
        public Sprite spriteStar;       // 60 사이 사이즈 스타
        public Sprite spriteInappOriginIcon; // 인앱 구매확정 메일 아이콘        
        public Sprite spriteAllPassIcon; // 올 패스 아이콘

        
        
        #endregion
        

        // * 비암호화 저장 세팅 (디폴트는 암호화)
        public static ES3Settings noEncryptionSetting;
        static FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);
        static string originArabicText = string.Empty;

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

            // 프레임레이트 설정 (iOS는 기본값이 30이라 애니메이션이 예쁘지 않음)
            Application.targetFrameRate = 50;
            safeArea = Screen.safeArea;
            Debug.Log("safeArea : " + safeArea.ToString());

            // safeArea 있는 경우, 캐릭터 위치 조정을 하려고 한다.
            if (safeArea.y != 0f)
            {
                Debug.Log(">>> SafeArea Device!!!!! <<<");
                hasSafeArea = true;
            }
            
            screenRatio = (float)Screen.width / Screen.height;

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

            if (!PlayerPrefs.HasKey(GameConst.AUTO_PLAY))
                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.normalDelay);

            if (!PlayerPrefs.HasKey(GameConst.MISSION_POPUP))
                PlayerPrefs.SetInt(GameConst.MISSION_POPUP, 1);

            if (!PlayerPrefs.HasKey(GameConst.ILLUST_POPUP))
                PlayerPrefs.SetInt(GameConst.ILLUST_POPUP, 1);

            // * 화면 어두워지지 않도록 설정 
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Singleton
            main = this;
            
            // * 코원, 피어 프로젝트 분기하기. 
            // Debug.Log("IDENTIFIER : " + Application.identifier);
            if(Application.identifier.Contains("cowon")) {
                // 코원 프로젝트 
                gamebaseAPP_ID = "qtV3HLW5";
                gamebaseLogger_ID = "7eGOB7zE5dQx4yTC";
                
            }
            else { 
                // 피어 프로젝트 
                gamebaseAPP_ID = "6swpd3Jp";
                gamebaseLogger_ID = "6WMxzJjo6i5Z5iXm";
                
            }
            
            
            // * 2021.11.25 보안 처리 추가 
            // * 보안처리되지 않았던 빌드가 첫 진입시에는 데이터 폴더를 삭제한다. 
            if(!PlayerPrefs.HasKey(KEY_ENCRYPTION)) {
                
                Debug.Log("!!!!!!!! It's not secure. clean data path.");
                
                // 1.0.27 버전부터는 모든 이미지 파일에는 암호화가 적용된다.
                ClearPersistentDataPath();
                PlayerPrefs.SetInt(KEY_ENCRYPTION, 1);
                PlayerPrefs.Save();
            }            
            
            // 시스탬매니저는 앱의 실행부터 끝까지 씬에서 모두 사용됩니다. 
            DontDestroyOnLoad(this);
        }
        

        /// <summary>
        /// SystemManager의 Start는 앱의 최초 시작지점입니다. 
        /// </summary>
        void Start()
        {
            // * 게임베이스 초기화는 ViewTitle에서 시작한다. 
          
            // 디바이스 스펙에 따른 그래픽 퀄리티 설정             
            ChangeQuality();

            // 암호화하지 않는 EasySave 세팅             
            noEncryptionSetting = new ES3Settings(ES3.EncryptionType.None, "password");

        }
        
        void Update() {
            
            // 테스트 로직 
            if(Application.isEditor && Input.GetKeyDown(KeyCode.S)) {
                UserManager.main.SetAdminUser();
            }
        }
        
        /// <summary>
        /// iOS 퀄리티 런타임으로 변경하기
        /// </summary>
        void ChangeQuality() {
            
            
            // Debug.Log(string.Format("Current QualityLevel is [{0}]", QualitySettings.GetQualityLevel()));
            
            // iOS만 실행하도록 전처리 
            // iOS는 가장 낮은 퀄리티가 기본으로 세팅 되어있다. 
            // 21.11.09 세상에는 아직도 사양이 낮은 폰을 사용하는 사람이 있기 떄문에 AOS, IOS 할 것 없이 사양 다운을 해준다...
            
            int minRam = 4000;
            
            
            #if UNITY_ANDROID
            
                minRam = 4200; // Android 에서는 4200으로 기준선 높인다 . 
            
            #endif
            
            Debug.Log(">>> System RAM Check :: " + SystemInfo.systemMemorySize);
            Debug.Log(">>> Limit RAM Check :: " + minRam);
            
            // 
            if(SystemInfo.systemMemorySize >= minRam) {
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
                SetDefaultLaunchingInfo();
                launchingJSON = null;
                return;
            }

            // Debug.Log(res.DataAsText);

            launchingJSON = JsonMapper.ToObject(res.DataAsText);

            try
            {
                if (launchingJSON["header"]["isSuccessful"].IsBoolean && bool.Parse(launchingJSON["header"]["isSuccessful"].ToString()) == true)
                {
                    // 테스트 서버의 경우 URL 변경 
                    if(isTestServerAccess) {
                        coinShopURL = launchingJSON["launching"]["server"]["test_coinshop_url"].ToString();    
                        privacyURL = launchingJSON["launching"]["server"]["test_privacy_url"].ToString();
                        termsOfUseURL = launchingJSON["launching"]["server"]["test_terms_url"].ToString();
                        bundleURL = launchingJSON["launching"]["server"]["test_bundle_url"].ToString();
                        
                        // 어드레서블 카탈로그 URL 및 버전 
                        addressable_url = launchingJSON["launching"]["server"]["test_addressable"].ToString();
                        addressable_version = launchingJSON["launching"]["server"]["test_addressable_version"].ToString();
                    }
                    else {
                        addressable_url = launchingJSON["launching"]["server"]["addressable"].ToString();
                        addressable_version = launchingJSON["launching"]["server"]["addressable_version"].ToString();
                    }
                    
                    // 어드레서블 URL 완성하기 (OS별로 구분된다)
                    #if UNITY_IOS
                    addressable_url += "iOS/catalog_1.json";
                    #else
                    addressable_url += "Android/catalog_1.json";
                    #endif                       
                }
                else
                {
                    launchingJSON = null;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                launchingJSON = null;
                
                SetDefaultLaunchingInfo();
                
            }
        } // ? CallbackGamebaseLaunching
        
        
        /// <summary>
        /// 런칭이 제대로 동작하지 않았을때를 대비한 처리 
        /// </summary>
        void SetDefaultLaunchingInfo() {
            addressable_url = "https://d2dvrqwa14jiay.cloudfront.net/bundle/";
            #if UNITY_IOS
            addressable_url += "iOS/catalog_1.json";
            #else
            addressable_url += "Android/catalog_1.json";
            #endif      
        }

        #endregion

        #region Gamebase 초기화 

        /// <summary>
        /// 게임베이스 초기화 
        /// </summary>
        public void GameBaseInitialize()
        {
            Debug.Log("<color=white>GameBaseInitialize</color>");

            // 두번 초기화를 막는다. 
            if (IsGamebaseInit)
            {
                return;
            }
            
            
            
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

            Debug.Log("currentGamebaseLanguageCode :: " + currentGamebaseLanguageCode);

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

                ShowSystemPopup("Game initialization failed : " + error.message, Application.Quit, Application.Quit, false, false);

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
            
            isTestServerAccess = false; 
        
            // 통신 완료까지 대기 
            while(!isServerInfoReceived)
                yield return null;      
                
            
            // * 상태에 따른 추가 처리 용도 
            switch (clientStatus)
            {
                // 여기는 정상 서비스
                case GamebaseLaunchingStatus.IN_SERVICE: // 정상 서비스 중
                case GamebaseLaunchingStatus.RECOMMEND_UPDATE: // 업데이트 권장
                case GamebaseLaunchingStatus.IN_SERVICE_BY_QA_WHITE_LIST: // 점검 중 QA 단말기의 접속
                    break;

                case GamebaseLaunchingStatus.IN_TEST: // 테스트 버전
                    isTestServerAccess = true; // 테스트 버전.
                    break;

                case GamebaseLaunchingStatus.IN_REVIEW: // iOS & Android 심사
                    isTestServerAccess = true; // 테스트 버전.
                    // 심사할때는 라이브 서버인척 하기. 
                    break;

                case GamebaseLaunchingStatus.REQUIRE_UPDATE: // 업그레이드 필수
                    // 각 스토어 페이지를 열어주고 앱은 종료 처리 
                    ShowSystemPopupLocalize("3", ForwardToStore, ForwardToStore, true, false);
                    isServerValid = false;
                    break;

                case GamebaseLaunchingStatus.INSPECTING_ALL_SERVICES: // 점검에 대한 처리 
                case GamebaseLaunchingStatus.INSPECTING_SERVICE:
                    // 점검에 대한 처리 
                    ShowSystemPopup(string.Format("{0}\n~\n{1}\n{2}", launchingInfo.launching.maintenance.beginDate, launchingInfo.launching.maintenance.endDate, Uri.UnescapeDataString(launchingInfo.launching.maintenance.message)), Application.Quit, Application.Quit, true, false);
                    isServerValid = false; 
                    break;


                case GamebaseLaunchingStatus.INTERNAL_SERVER_ERROR: // Error in internal server.
                    ShowSystemPopup("Internal Server Error", Application.Quit, Application.Quit, false, false);
                    isServerValid = false;
                    break;
            } // ? end of switch
            
            
            #region Gamebase Launching 
            isLaunchingCalled = false; 
            NetworkLoader.main.RequestGamebaseLaunching(CallbackGamebaseLaunching);
            while (!isLaunchingCalled) // 응답 받을때까지 기다리기. 
                yield return null;
            #endregion
            
            
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
        /// 게임서버 기준정보 받아오기 
        /// 2021.08.31 : 로컬라이징 텍스트 리스트 
        /// </summary>
        /// <returns></returns>
        void RequestGameServerInfo() {
            
            isServerInfoReceived = false; 
            
            JsonData reqData  = new JsonData();
            reqData[CommonConst.FUNC] = "getServerMasterInfo";
            
            NetworkLoader.main.SendPost(OnRequestGameServerInfo, reqData, false);
        }
        
        
        /// <summary>
        /// 게임서버 기준정보 통신 완료 
        /// </summary>
        void OnRequestGameServerInfo(HTTPRequest request, HTTPResponse response ) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            Debug.Log(response.DataAsText);
    
            
            // 버전 비교를 시작한다.
            JsonData result = JsonMapper.ToObject(response.DataAsText);
                    
            // Debug.Log(JsonMapper.ToStringUnicode(result["master"]));
            JsonData masterInfo = result["master"]; // 마스터 정보 
            
            // * 타임딜 기준정보 추가 
            if(result.ContainsKey("timedeal"))
                timedealStandard = result["timedeal"];
            
            // Debug.Log(JsonMapper.ToStringUnicode(result["ad"]));
            JsonData adInfo = result["ad"]; // 광고 기준정보 
            int serverlocalVer = int.Parse(masterInfo["local_ver"].ToString()); // 서버 로컬라이징 텍스트 버전
            
            AdManager.main.SetServerAdInfo(adInfo); // 광고 기준정보 세팅 
            
            
            // 기본 재화 정보 
            // 코인, 젬 미리 다운로드 시켜놓기. 
            baseCurrencyData = result["baseCurrency"]; 
            
            if(baseCurrencyData != null && baseCurrencyData.ContainsKey(LobbyConst.COIN)) {
                RequestDownloadImage(SystemManager.GetJsonNodeString(baseCurrencyData[LobbyConst.COIN], CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(baseCurrencyData[LobbyConst.COIN], CommonConst.COL_IMAGE_KEY), null);
            }
            if(baseCurrencyData != null && baseCurrencyData.ContainsKey(LobbyConst.GEM)) {
                RequestDownloadImage(SystemManager.GetJsonNodeString(baseCurrencyData[LobbyConst.GEM], CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(baseCurrencyData[LobbyConst.GEM], CommonConst.COL_IMAGE_KEY), null);
            }
            
            
            // 없는 리소스 허용 여부
            allowMissingResource = GetJsonNodeBool(masterInfo, "allow_missing_resource");

           
            coinShopURL = GetJsonNodeString(masterInfo, "coinshop_url"); // 코인샵 URL
            surveyUrl = GetJsonNodeString(masterInfo, "survey_url");
            firsetResetPrice = GetJsonNodeInt(masterInfo, "first_reset_price"); // 최초 리셋 비용 
            
            removeAdPrice = GetJsonNodeInt(masterInfo, "remove_ad_price"); // 에피소드 광고 제거 비용
            
            episodeOpenPricePer = GetJsonNodeInt(masterInfo, "open_price_per"); // 에피소드 시간단축 오픈 10분당 코인 가격
            waitingReduceTimeAD = GetJsonNodeInt(masterInfo, "reduce_waiting_time_ad"); // 광고보고 단축되는 시간 .
            
            
            privacyURL = GetJsonNodeString(masterInfo, "privacy_url");
            termsOfUseURL = GetJsonNodeString(masterInfo, "terms_url"); 
            contentsURL = GetJsonNodeString(masterInfo, "contents_url"); 
            copyrightURL = GetJsonNodeString(masterInfo, "copyright_url"); 
            
            
            useProjectNotify = GetJsonNodeBool(masterInfo, "project_notify"); // 프로젝트 알림설정 사용여부 
           

            // 디바이스 정보 불러다놓고, 
            localVer = GetDeviceLocalVer();
            localizedTextJSON = GetDeviceLocalData();
            
            // Debug.Log(string.Format("device local ver : [{0}] / server local ver : [{1}]", localVer, serverlocalVer));
            
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
        public void SetCurrentLanguageCode() {
            
            // 저장된 언어정보가 없을때. 
            if(!ES3.KeyExists(SystemConst.KEY_LANG)) {
                
                switch(Application.systemLanguage) {
                    case SystemLanguage.Korean:
                    currentAppLanguageCode = "KO";
                    break;
                    
                    case SystemLanguage.English:
                    currentAppLanguageCode = "EN";
                    break;
                    
                    case SystemLanguage.Japanese:
                    currentAppLanguageCode = "JA";
                    break;
                    
                    case SystemLanguage.Arabic:
                    currentAppLanguageCode = "AR";
                    break;
                    
                    default:
                    currentAppLanguageCode = "EN";
                    break;
                } // ? end of switch
                
                
                ES3.Save<string>(SystemConst.KEY_LANG, currentAppLanguageCode); // 저장한다. 
            } // 저장된 언어 정보 없는 경우 
            
            currentAppLanguageCode = ES3.Load<string>(SystemConst.KEY_LANG);
            
            // 게임베이스와 서버에서 사용하는 언어코드가 서로 다르기 때문에 컨버팅 해준다. 
            switch(currentAppLanguageCode) {
                case "EN":
                currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.English;
                break;
                case "KO":
                currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.Korean;
                break;
                case "JA":
                currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.Japanese;
                break;
                default: // 아랍어도 게임베이스 언어는 영어. 
                currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.English;
                break;
            }

        }
        
        /// <summary>
        /// 어드레서블 카탈로그 처리 
        /// </summary>
        public void InitAddressableCatalog() {
            
            
            Debug.Log("#### InitAddressableCatalog START ###");
            
            // 로컬에 저장된 어드레서블 키를 불러온다. 
            string currentAddressableVersion = ES3.LoadString(CommonConst.KEY_ADDRESSABLE_VERSION, "0");
            
            // 비교해서 버전이 안맞으면, clear cache를 한다. 
            // 어드레서블에서 큰 변경이 있는 경우만 버전 업.
            if(addressable_version != currentAddressableVersion) {
                
                Debug.Log("#### InitAddressableCatalog diff version, clear cache ###");
                Caching.ClearCache();
                ES3.Save<string>(CommonConst.KEY_ADDRESSABLE_VERSION, addressable_version); // 저장 
                
            }
            
         
            
            Debug.Log(string.Format("Addressable URL : [{0}]", addressable_url));
            Addressables.LoadContentCatalogAsync(addressable_url).Completed += (op) => {
            
                if(op.Status == AsyncOperationStatus.Succeeded) {
                    Debug.Log("### InitAddressableCatalog " +  op.Status.ToString());
                    isAddressableCatalogUpdated = true; // 카탈로그 로딩 완료 처리 
                    return;
                }
                else {
                    
                    // 한번더 시도한다. 
                    Addressables.LoadContentCatalogAsync(addressable_url).Completed += (op) => {
            
                        if(op.Status == AsyncOperationStatus.Succeeded) {
                            Debug.Log("### InitAddressableCatalog #2" +  op.Status.ToString());
                            isAddressableCatalogUpdated = true; // 카탈로그 로딩 완료 처리 
                            
                            return;
                        }
                        else {
                            
                           
                            NetworkLoader.main.ReportRequestError(op.OperationException.ToString(), "LoadContentCatalogAsync");
                            isAddressableCatalogUpdated = false;
                            
                            
                            //  카탈로그 실패시 접속 할 수 없음. 
                            SystemManager.ShowSystemPopup(SystemManager.GetDefaultServerErrorMessage(), NetworkLoader.OnFailedServer, NetworkLoader.OnFailedServer, false, false);
                            return;
                            
                        }
                        
                    }; // end of second try
                    
                    
                } // end of else
                
            }; // END!!!            
            
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
                return;
            }

            // LoginForLastLoggedInProvider 호출 , OnCallbackLogin으로 가기!
            Gamebase.LoginForLastLoggedInProvider(OnCallbackLogin);
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__providerName"></param>
        public void LoginByExpireToken(string __providerName) {
            
            Debug.Log("### LoginByExpireToken : " + __providerName);
            Gamebase.Login(__providerName, OnCallbackLogin);
            
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
                    ShowSystemPopup("로그인 서버가 응답하지 않습니다. 다시 로그인을 시도합니다.", null, null, false, true);
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
                    
                    // Token 관련 메세지는 토큰만료 로그인 팝업을 띄워준다.
                    // 2주 미접속시 토큰이 만료되어 이전 로그인 방식으로 로그인 할 수 없음 
                    if(error.code == GamebaseErrorCode.AUTH_TOKEN_LOGIN_FAILED 
                        || error.code == GamebaseErrorCode.AUTH_TOKEN_LOGIN_INVALID_LAST_LOGGED_IN_IDP 
                        || error.code == GamebaseErrorCode.AUTH_TOKEN_LOGIN_INVALID_TOKEN_INFO
                        || error.code == GamebaseErrorCode.AUTH_IDP_LOGIN_FAILED) {
                            
                            SystemManager.ShowNoDataPopup(CommonConst.POPUP_EXPIRE_TOKEN);
                            return;
                        }
                        
                    
                    
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
            UserManager.main.ConnectGameServer(gamebaseID);
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



        #region 약관 및 푸쉬

        /// <summary>
        /// 푸시 토큰 정보 불러오기 
        /// </summary>
        public void QueryPushTokenInfo(Action __callback)
        {

            if (Application.isEditor)
                return;
                
            if(pushTokenInfo != null)
                return;

            Gamebase.Push.QueryTokenInfo((data, error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    pushTokenInfo = data; // 데이터 설정 
                    
                    __callback?.Invoke();
                    
                    
                    if(!pushTokenInfo.agreement.pushEnabled) {
                        Debug.Log("Push is disable");
                        return;
                    }
                    
                    // 토큰 만료때문에 호출때마다 pushRegister 등록해준다. 
                    PushRegister(data.agreement.adAgreement, data.agreement.adAgreementNight);
                    Debug.Log(string.Format("### Push TokenInfo = pushAlert : {0}, nightPush : {1}", data.agreement.adAgreement, data.agreement.adAgreementNight));
                    MainMore.OnRefreshMore?.Invoke();
                }
                else
                {
                    Debug.LogError(string.Format("### QueryToken response failed. Error : {0}", error));
                    
                }


                
            });

        }

        /// <summary>
        /// 로그인 후 약관 동의 팝업 등장
        /// </summary>
        public void ShowAgreementTermsPopUp()
        {
            Debug.Log("ShowAgreementTermsPopUp");

            // 에디터에서는 지원하지 않음.
            if (Application.isEditor)
                return;


            // * 2021.10.13 게임베이스 호출로 변경.
            Gamebase.Terms.ShowTermsView((data, error) =>
            {
                if (Gamebase.IsSuccess(error) == true)
                {
                    Debug.Log("ShowTermsView succeeded : " + data.ToString());

                    // If the 'PushConfiguration' is not null,
                    // save the 'PushConfiguration' and use it for Gamebase.Push.RegisterPush() after Gamebase.Login().
                    GamebaseResponse.Push.PushConfiguration pushConfiguration = GamebaseResponse.Push.PushConfiguration.From(data);

                    if(pushConfiguration != null)
                    {
                        Debug.Log("이용약관 success 후 pushConfiguration = " + pushConfiguration);
                        if(pushConfiguration.pushEnabled) // Register 처리 .
                            PushRegister(pushConfiguration.adAgreement, pushConfiguration.adAgreementNight);
                    }
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
        public void QueryGamebaseTerms()
        {

            if (Application.isEditor)
                return;


            Gamebase.Terms.QueryTerms((data, error) =>
            {
                if (Gamebase.IsSuccess(error) == true)
                {
                    gamebaseTermsSeq = data.termsSeq;
                    gamebaseTermsVersion = data.termsVersion;

                    Debug.Log(string.Format("QueryTerms succeeded. [{0}]/[{1}]", gamebaseTermsSeq, gamebaseTermsVersion));

                    listGamebaseTerms = data.contents;

                    for (int i = 0; i < listGamebaseTerms.Count; i++)
                        Debug.Log(string.Format("[{0}] : [{1}]", listGamebaseTerms[i].termsContentSeq, listGamebaseTerms[i].name));
                }
                else
                {
                    Debug.LogError(string.Format("QueryTerms failed. error:{0}", error));
                    listGamebaseTerms = null;
                    gamebaseTermsSeq = -1;
                    gamebaseTermsVersion = string.Empty;
                }
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
                    QueryPushTokenInfo(null);
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

        #endregion


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
                    ShowSystemPopup(string.Format("[{0}] {1}", error.code, error.message), null, null, true, false);
                    return;
                }
                
                // 지정한 idp로 로그인 시도. 
                Gamebase.Login(__idp, (authToken, error) => {
                
                    
                    if(!Gamebase.IsSuccess(error)) {
                        Debug.Log(string.Format("#### Login failed. error is {0}", error));
                        ShowSystemPopup(string.Format("[{0}] {1}", error.code, error.message), null, null, true, false);
                        
                        // 로그인 복원
                        Gamebase.Login(GamebaseAuthProvider.GUEST, OnCallbackLogin);
                        return;
                    }
               
                    // 로그인 성공 
                    string newGamebaseID = Gamebase.GetUserID();
                    Debug.Log(string.Format("#### New Gamebase ID = [{0}]", newGamebaseID));
                    
                    // 로그인에 성공했으면 현재 GamebaseID로 userkey를 조회한다. 
                    JsonData reqData = new JsonData();
                    reqData[CommonConst.FUNC] = "checkAccountExistsByGamebase";
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
            Debug.Log("OnRequest__checkAccountExistsByGamebase");
            
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                Debug.Log(string.Format("#### OnRequest__checkAccountExistsByGamebase failed. error is {0}", request.State));
                return;
            }
            
            // 여기서는 연동한 구글, 애플과 연결된 계정정보를 불러오고 처리를 진행한다. 
            JsonData result = JsonMapper.ToObject(response.DataAsText);

            if (result == null || result.Count == 0) { 
                // ! 계정이 없다. => call updateAccountWithGamebaseID => OnRequest__updateAccountWithGamebaseID
                 Debug.Log("### No connected Gamebase account");
                 
                 // 현재 유저의 table_account에 현재의 gamebaseID를 덮어씌운다. 
                 // userkey는 변동없이 유지된다. 
                 JsonData reqData = new JsonData();
                 reqData[CommonConst.FUNC] = "updateAccountWithGamebaseID";
                 reqData["gamebaseID"] = Gamebase.GetUserID();
                 
                 //  ! 이 통신에서 유저에게 연동보상을 지급한다. 
                 NetworkLoader.main.SendPost(OnRequest__updateAccountWithGamebaseID, reqData, true);
            }
            else {
                // ! 이전에 연결된 계정이 있다. 
                string connectedUserkey = result[0]["userkey"].ToString();
                Debug.Log(string.Format("### connected account exists [{0}]", connectedUserkey));
                
                // * 여기서부터 복잡해지는건데... 이미 연동한 계정이 있는 경우는..? 
                // * 유저에게 연동된 계정이 있는데, 연동하겠느냐고 묻는다. 
                // TODO 예, 아니오 팝업을 띄운다.
                if(connectedUserkey != UserManager.main.userKey) {
                    ShowSystemPopupLocalize("6111", ConfirmPreviousAccountLoad, CancleAccountConnect);
                }
                else { // 같은 계정으로 연결되었다. => 토큰 만료? 
                    Debug.Log("### Connected same account #####");
                    
                    // 연동이 완료되었음을 안내. 
                    ShowSystemPopupLocalize("6112", null, null, true, false);
                    UserManager.main.accountLink = "link"; // 링크 처리 
                    RefreshAccountLinkRelated();
                }
 
            }
        }
        
        
        /// <summary>
        /// 게임베이스 ID => table_account에 업데이트 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void OnRequest__updateAccountWithGamebaseID(HTTPRequest request, HTTPResponse response) {
            if (!NetworkLoader.CheckResponseValidation(request, response))
            {
                Debug.Log(string.Format("#### OnRequest__updateAccountWithGamebaseID failed. error is {0}", request.State));
                ShowSystemPopup("게임베이스 연동 과정에서 오류가 발생했습니다.", null, null, false, false);
                return;
            }
            
            
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            
            // 2022.01 미수신 메일 카운트 날아온다.
            UserManager.main.SetNotificationInfo(result);

            // 연동이 완료되었습니다. 메세지 처리 
            ShowSystemPopupLocalize("6110", null, null, true, false);
            UserManager.main.accountLink = "link";

            // 22.04.06 첫 계정 연동을 했으므로 초심자 업적 업데이트 호출도 해준다
            NetworkLoader.main.RequestIFYOUAchievement(1);

            // Refresh 처리 
            PopupAccount.OnRefresh?.Invoke();
            MainToggleNavigation.OnToggleAccountBonus?.Invoke();
            MainMore.OnRefreshMore?.Invoke();

            UserManager.main.RequestUserGradeInfo(UserManager.main.CallbackUserGreadeInfo);
            NetworkLoader.main.RequestIfyouplayList();
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
            StartCoroutine(RoutineLoadingConnectedAccount());
       }
       
       
        IEnumerator RoutineLoadingConnectedAccount() {
            
            Debug.Log("#### RoutineLoadingConnectedAccount :: " + Gamebase.GetUserID());
            
             // ! 연결된 계정 다시 로드 
            UserManager.main.ConnectGameServer(Gamebase.GetUserID());
            
            
            // 여기 여러 통신이 체인처럼 연쇄적으로 하기때문에 여러개 넣었다.
            yield return  null; 
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            
            yield return new WaitForSeconds(0.1f); 
            
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            
            yield return new WaitForSeconds(0.1f);
            
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            
            Debug.Log(string.Format("#### RoutineLoadingConnectedAccount Load user done [{0}]", UserManager.main.userKey));
            NetworkLoader.main.RequestIFYOUAchievement(1);

            // 연동이 완료되었습니다.
            ShowSystemPopupLocalize("6112", null, null, true, false);
        
            
            // Refresh 처리 
            RefreshAccountLinkRelated();
        }
        
        /// <summary>
        /// 계정 연동 관련 컨트롤 리프레시 
        /// </summary>
        void RefreshAccountLinkRelated() {
            
            Debug.Log("#### RefreshAccountLinkRelated");
            
            // MainToggleNavigation.OnToggleAccountBonus?.Invoke();
            PopupAccount.OnRefresh?.Invoke(); // 계정연동 팝업 
            MainMore.OnRefreshMore?.Invoke(); // 세팅화면 
            
            ViewMain.OnRefreshViewMain?.Invoke(); // 메인화면 
            ViewCommonTop.OnRefreshAccountLink?.Invoke(); // 상단 
            
            
            // 유저의 구매 내역 받아오기
            NetworkLoader.main.RequestUserPurchaseHistory();
            
            // 코인 환전 상품 정보 가져오기
            NetworkLoader.main.RequestCoinExchangeProductList();

            // 업적 리스트 갱신
            UserManager.main.RequestUserGradeInfo(UserManager.main.CallbackUserGreadeInfo);

            NetworkLoader.main.RequestIfyouplayList();


            StartCoroutine(RefreshScreenView());
        }      
        

        /// <summary>
        /// 화면 갱신
        /// </summary>
        IEnumerator RefreshScreenView()
        {
            ShowNetworkLoading();

            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());

            MainProfile.OnRefreshIFYOUAchievement?.Invoke();
            MainIfyouplay.OnRefreshIfyouplay?.Invoke();

            HideNetworkLoading();
        }
        
        public void Logout()
        {
            Gamebase.Logout((error) =>
            {
                if (Gamebase.IsSuccess(error))
                    Debug.Log("Logout succeeded.");
                else
                    Debug.Log(string.Format("Logout failed. error is {0}", error));
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
                    ShowSystemPopup("연동 계정 변경이 완료되었습니다.", null, null, true, false);
                }
                else
                {
                    Debug.Log(string.Format("AddMapping failed. error is {0}", error));
                    ShowSystemPopup(error.message, null, null, false, false);
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

        #region JsonData 추출

        /// <summary>
        /// JSON 특정 노드의 노드 값을 알려주쇼
        /// </summary>
        /// <param name="__node"></param>
        /// <param name="__col"></param>
        /// <returns></returns>
        public static JsonData GetJsonNode(JsonData __node, string __col) {

            if (__node == null)
                return null;

            if (!__node.ContainsKey(__col))
                return null;
                
            if (__node[__col] == null)
                return null;
                
            return __node[__col];
        }
        
        /// <summary>
        /// 특정 노드의 int 값을 알려주세요. 
        /// </summary>
        /// <param name="__node"></param>
        /// <param name="__col"></param>
        /// <returns></returns>
        public static int GetJsonNodeInt(JsonData __node, string __col) {
            if (__node == null || !__node.ContainsKey(__col))
                return 0;


            if (__node[__col] == null)
                return 0;
            
            try {    
                return int.Parse(GetJsonNodeString(__node, __col));
            }
            catch (Exception e) {
                NetworkLoader.main.ReportRequestError(e.StackTrace, "GetJsonNodeInt : " + __col);
                Debug.LogError(e.StackTrace);
                return 0;
            }

            
        }
        
        public static long GetJsonNodeLong(JsonData __node, string __col) {
            if (__node == null || !__node.ContainsKey(__col))
                return 0;


            if (__node[__col] == null)
                return 0;
            
            try {    
                return long.Parse(GetJsonNodeString(__node, __col));
            }
            catch (Exception e) {
                NetworkLoader.main.ReportRequestError(e.StackTrace, "GetJsonNodeLong : " + __col);
                return 0;
            }

            
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__node"></param>
        /// <param name="__col"></param>
        /// <returns></returns>
        public static float GetJsonNodeFloat(JsonData __node, string __col) {
            if (__node == null || !__node.ContainsKey(__col))
                return 0;


            if (__node[__col] == null)
                return 0;
            
            try {    
                return float.Parse(GetJsonNodeString(__node, __col));
            }
            catch {
                return 0;
            }
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
                // Debug.Log(string.Format("Error : [{0}] is not a node", __col));
                return false;
            }
                
            if (__node[__col] == null) {
                Debug.Log(string.Format("Error : [{0}] is null", __col));
                return false;
            }
            
            if(string.IsNullOrEmpty(__node[__col].ToString()) ||  __node[__col].ToString() == "0")
                return false;
            
            return true;
        }

        #endregion

        #region 로딩 팝업

        /// <summary>
        /// 네트워크 로딩 팝업 오픈 
        /// </summary>
        public static void ShowNetworkLoading(bool __isInstant = false)
        {
            // 각 씬마다 보유하고 있음.
            if (LobbyManager.main != null)
                main.networkLoadingScreen = LobbyManager.main.GetLobbyNetworkLoadingScreen();

            if (StoryLobbyManager.main != null)
                main.networkLoadingScreen = StoryLobbyManager.main.GetLobbyNetworkLoadingScreen();

            if (GameManager.main != null)
                main.networkLoadingScreen = GameManager.main.GetGameNetworkLoadingScreen();               
                
            if (main.networkLoadingScreen)
                main.networkLoadingScreen.ShowNetworkLoading(__isInstant);
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

        #endregion


        #region 시스템(앱) 팝업

        
        
        
        /// <summary>
        /// 리셋 팝업 호출 
        /// </summary>
        /// <param name="__targetEpisode"></param>
        public static void ShowFlowResetPopup(EpisodeData __targetEpisode) {
            
            Debug.Log("## ShowFlowResetPopup : " + __targetEpisode.episodeTitle);
            
            // Signal 보내고 팝업 호출 
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_EPISODE_RESET, __targetEpisode, string.Empty);
            
            PopupBase p = PopupManager.main.GetPopup("Reset"); 
            if(p == null) {
                Debug.LogError(">> No Reset Popup");
                return; 
            }
            
            // * StoryReset과 FlowReset은 같은 스크립트 파일을 공유해서, 이렇게 구분한다.
            p.Data.isPositive = false;
            
            // 팝업 호출!
            PopupManager.main.ShowPopup(p, false);
        }
        
        
        /// <summary>
        /// 스토리 리셋 팝업 호출 
        /// </summary>
        /// <param name="__targetEpisode"></param>
        public static void ShowStoryResetPopup(EpisodeData __targetEpisode) {
            Debug.Log("## ShowStoryResetPopup : " + __targetEpisode.episodeTitle);
            
            // Signal 보내고 팝업 호출 
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_EPISODE_RESET, __targetEpisode, string.Empty);
            
            PopupBase p = PopupManager.main.GetPopup("StoryReset"); 
            if(p == null) {
                Debug.LogError(">> No Reset Popup");
                return; 
            }
            
            // * StoryReset과 FlowReset은 같은 스크립트 파일을 공유해서, 이렇게 구분한다.
            p.Data.isPositive = true; 
            
            // 팝업 호출!
            PopupManager.main.ShowPopup(p, false);
        }
        
        
        /// <summary>
        /// 리소스 획득 팝업 (OK), 다운로드 리소스 아이콘 
        /// </summary>
        /// <param name="__message"></param>
        /// <param name="__url"></param>
        /// <param name="__key"></param>
        public static void ShowResourcePopup(string __message, int __quantity, string __url, string __key) {
            PopupBase p = PopupManager.main.GetPopup("Resource");
            
            if (p == null)
            {
                Debug.LogError(">> No Resource Popup");
                return;
            }
            
            p.Data.isConfirm = false;
            p.Data.imageURL = __url;
            p.Data.imageKey = __key;
            p.Data.SetLabelsTexts(__message, __quantity.ToString()); // 메세지, 개수 
            
            PopupManager.main.ShowPopup(p, false);
        }
        
        /// <summary>
        /// 기본 재화, 스타코인에 대한 OK 팝업 
        /// </summary>
        /// <param name="__message"></param>
        /// <param name="__currency"></param>
        /// <param name="__quantity"></param>
        public static void ShowResourcePopup(string __message, string __currency, int __quantity) {
            PopupBase p = PopupManager.main.GetPopup("Resource");
            
            if (p == null)
            {
                Debug.LogError(">> No Resource Popup");
                return;
            }
            
            p.Data.isConfirm = false;
            p.Data.imageURL = string.Empty;
            p.Data.imageKey = string.Empty;
            
            if(__currency == LobbyConst.GEM) {
                p.Data.SetImagesSprites(SystemManager.main.spriteStar);
                p.Data.contentValue = 100;
            }
            else if(__currency == LobbyConst.COIN) {
                p.Data.SetImagesSprites(SystemManager.main.spriteCoin);
                p.Data.contentValue = 100;
            }
            
            p.Data.SetLabelsTexts(__message, __quantity.ToString()); // 메세지, 개수 
            
            PopupManager.main.ShowPopup(p, false);
        }
        
        
        /// <summary>
        /// 리소스 사용 컨펌 팝업 
        /// </summary>
        /// <param name="__message"></param>
        /// <param name="__url"></param>
        /// <param name="__key"></param>
        /// <param name="__positive"></param>
        public static void ShowResourceConfirm (string __message, int __quantity, string __url, string __key, Action __positive, string __textPositive = "", string __textNegative = "") {
            PopupBase p = PopupManager.main.GetPopup("Resource");
            
            if (p == null)
            {
                Debug.LogError(">> No Resource Popup");
                return;
            }
            
            if(string.IsNullOrEmpty(__textPositive)) {
                __textPositive = SystemManager.GetLocalizedText("5041");
            }
            
            if(string.IsNullOrEmpty(__textNegative)) {
                __textNegative = SystemManager.GetLocalizedText("5040");
            }
            
            
            p.Data.isConfirm = true;
            p.Data.imageURL = __url;
            p.Data.imageKey = __key;
            p.Data.SetLabelsTexts(__message, __quantity.ToString());
            p.Data.SetButtonsText(__textPositive, __textNegative);
            
            p.Data.positiveButtonCallback = __positive;
            
            PopupManager.main.ShowPopup(p, false);
        }
        
        /// <summary>
        /// Confirm, Submit 버튼을 가지고 있는 앱용 시스템 팝업
        /// </summary>
        /// <param name="__message">팝업에 들어갈 문구</param>
        /// <param name="__positive">확인 및 예를 눌렀을 때 동작할 함수</param>
        /// <param name="__negative">아니오, 닫기 눌렀을 때 동작할 함수</param>
        /// <param name="isPositive">긍정형인가?</param>
        /// <param name="isConfirm">예/아니오 팝업인가, 확인만 뜨는 팝업인가(false = Submit)</param>
        public static void ShowSystemPopup(string __message, Action __positive, Action __negative, bool isPositive = true, bool isConfirm = true)
        {
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_CONFIRM);

            if(p == null)
            {
                Debug.LogError(">> No System Popup");
                return;
            }

            p.Data.isConfirm = isConfirm;
            p.Data.SetLabelsTexts(__message);

            if (__positive != null)
                p.Data.SetPositiveButtonCallback(__positive);

            if (__negative != null)
                p.Data.SetNegativeButtonCallback(__negative);

            PopupManager.main.ShowPopup(p, false);
        }

        /// <summary>
        /// 로컬라이징 적용 시스템 팝업
        /// </summary>
        public static void ShowSystemPopupLocalize(string __textId, Action __positive, Action __negative, bool isPositive = true, bool isConfirm = true)
        {
            ShowSystemPopup(GetLocalizedText(__textId), __positive, __negative, isPositive, isConfirm);
        }

        
        /// <summary>
        /// 하단에 생기는 까만 정말 간단한 팝업
        /// </summary>
        public static void ShowSimpleAlert(string __message, bool addQueue = true)
        {
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_SIMPLE_ALERT);

            if(p == null)
            {
                Debug.LogError(">> No Simple alert popup");
                return;
            }

            p.Data.SetLabelsTexts(__message);

            PopupManager.main.ShowPopup(p, addQueue);
        }

        
        public static void ShowSimpleAlertLocalize(string __texId, bool addQueue = true)
        {
            ShowSimpleAlert(GetLocalizedText(__texId), addQueue);
        }


        /// <summary>
        /// 화면 중앙에 뜨는 3초뒤 사라지는 팝업
        /// </summary>
        public static void ShowMessageAlert(string __message)
        {
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_MESSAGE_ALERT);

            if (p == null)
            {
                Debug.LogError(">> No Message alert popup");
                return;
            }

            p.Data.SetLabelsTexts(__message);
            PopupManager.main.ShowPopup(p, true);
        }


        public static void ShowMessageWithLocalize(string __textId)
        {
            ShowMessageAlert(GetLocalizedText(__textId));
        }



        public void ShowMissingFunction(string __message)
        {
            ShowMessageAlert(__message);
        }

        #endregion



        #region 프로모션, 장르, 카테고리 관련
        
        /// <summary>
        /// NetworkLoader.RequestPlatformServiceEvents
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void CallbackPlatformServiceEvent(HTTPRequest req, HTTPResponse res)
        {
            if(!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackPlatformServiceEvent");
                return;
            }
            
            Debug.Log("CallbackPlatformServiceEvent :: " + res.DataAsText);
            JsonData result = JsonMapper.ToObject(res.DataAsText);
            
            promotionData = result["promotion"];
            noticeData = result["notice"];
            storyGenreData = result["genre"];
            introData = result["intro"]; 
        }




        /// <summary>
        /// 장르 관련 정보 세팅 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void SetCategoryGenre(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkLoader.CheckResponseValidation(request, response))
                return;

            // genre_name
            storyGenreData = JsonMapper.ToObject(response.DataAsText);
        }
        
        
        #endregion

        #region 파일 체크, 파일 다운로드 공용
        
        IEnumerator LoadingAddressableFont() {
            
            // 둘다 불러놓는다. 
            string KO_addressableKey = "Font/" + "KoPubWorld Dotum Medium SDF.asset"; 
            string JA_addressableKey = "Font/" + "NotoSansJP-Medium SDF.asset"; 
            
            assetFontShader = Shader.Find("TextMeshPro/Mobile/Distance Field"); // 타 프로젝트에서 가져와서 쉐이더 필요.          
            
            if(jaFont == null && koFont == null) {
                // LoadAssetAsync. 
                // 한글 & 영어 폰트
                Addressables.LoadAssetAsync<TMP_FontAsset>(KO_addressableKey).Completed += (handle) => {
                    if(handle.Status == AsyncOperationStatus.Succeeded) { 
                        // * 성공
                        mountedAssetFontKO = handle;
                        koFont = handle.Result;
                        
                        // 다른 프로젝트에서 가져오는거라서 Shader처리 해준다.
                        koFont.material.shader = assetFontShader;
                        // Debug.Log("<color=cyan>Font loaded OK!!!!!</color>");
                    }
                    else {
                        Debug.Log("<color=cyan>Font loaded FAIL....</color>");
                        NetworkLoader.main.ReportRequestError(handle.OperationException.ToString(), "Fail KO Font LoadAssetAsync");
                        SystemManager.main.isAddressableCatalogUpdated = false;
                    }
                };
                
                // 일어폰트
                Addressables.LoadAssetAsync<TMP_FontAsset>(JA_addressableKey).Completed += (handle) => {
                    if(handle.Status == AsyncOperationStatus.Succeeded) { 
                        // * 성공
                        mountedAssetFontJA = handle;
                        jaFont = handle.Result;
                        
                        // 다른 프로젝트에서 가져오는거라서 Shader처리 해준다.
                        jaFont.material.shader = assetFontShader;
                        // Debug.Log("<color=cyan>Font loaded OK!!!!!</color>");
                    }
                    else {
                        Debug.Log("<color=cyan>Font loaded FAIL....</color>");
                        NetworkLoader.main.ReportRequestError(handle.OperationException.ToString(), "Fail JA Font LoadAssetAsync");
                        SystemManager.main.isAddressableCatalogUpdated = false;
                    }
                
                };                
            }
            
            
            // 폰트 정보 둘다 불러올때까지 대기 
            while(jaFont == null || koFont == null) {
                yield return null;
                
                // 둘 중 하나가 fail이 나온 경우 
                if(!isAddressableCatalogUpdated) {
                    Debug.Log(string.Format("Initialization failed. error is {0}", "No Font info"));
                    ShowSystemPopup("Game initialization failed : " + "No Font info", Application.Quit, Application.Quit, false, false);
                    yield break;
                }
            }
            
            // 로딩 다 하고 메인폰트 설정
            SetMainFont();
        }
        
        /// <summary>
        /// 언어에 따른 메인 폰트 설정
        /// </summary>
        public void SetMainFont() {

            switch(currentAppLanguageCode) {
                case "KO":    // 한글 영어는 같이 씀 
                case "EN":
                mainAssetFont = koFont;
                break;
                
                case "JA":
                
                mainAssetFont = jaFont;
                break;
                
                
                case "AR":
                mainAssetFont = arFont; // 아랍 폰트는 내장됨. 
                break;
                
                default:
                break;
            }
            
            // 폰트 로드 완료 처리 
            isApplicationFontAvailable = true; 
        }
        
        /// <summary>
        /// 에셋번들로 폰트 불러오기 
        /// </summary>
        public void LoadAddressableFont()  {
            
            Debug.Log(string.Format("<color=yellow> {0} </color>", "LoadAddressableFont"));
            
            // * 기본폰트는 AppleGothic. 
            // * 모든 UI의 폰트는 AppleGothic으로 설정한다. 
            // * 언어에 따라서 변경. 
            StartCoroutine(LoadingAddressableFont());
            
        }
        
        
        /// <summary>
        /// 언어에 해당하는 에셋 폰트 가져오기 
        /// </summary>
        /// <param name="__isException"></param>
        /// <returns></returns>
        public TMP_FontAsset getCurrentLangFont(bool __isException) {
            
            switch (currentAppLanguageCode) {
                case "JA":  // 일본어 
                    return jaFont; 
                case "AR": // 아랍어
                
                    // 아랍어 폰트 2개로 분리. (말풍선용과 시스템 UI)
                    if(__isException)
                        return arFont; // 시스템
                    else  
                        return arNormalBubbleFont; // 말풍선
                
                default:
                if(__isException)  // appleGothic 유지 
                    return innerFontEN; 
                else 
                    return mainAssetFont; // 유지하지 않음 
            }
        }

        
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
            
            
            if(localizedTextJSON == null) {
                // Debug.LogError("localizedTextJSON is null");
                return string.Empty;
            }
            
            if(string.IsNullOrEmpty(__id)) {
                Debug.LogError("id is null or empty");
                return string.Empty;
            }
                
            if(!localizedTextJSON.ContainsKey(__id)) {
                Debug.LogError(string.Format("TEXT ID : [{0}] is missing", __id));
                return string.Empty;
            }
            
            // 여기서 가끔 null이 있어서 exception 생기는 경우가 있다. 
            try {
                return localizedTextJSON[__id][main.currentAppLanguageCode.ToUpper()].ToString();
            }
            catch {
                NetworkLoader.main.ReportRequestError(string.Format("[{0}]/[{1}]", __id, main.currentAppLanguageCode), "com_localize is null");
                return string.Empty;
            }
            
        }
        
        /// <summary>
        /// TextMeshPro에 로컬라이즈 텍스트 설정 
        /// </summary>
        /// <param name="__textUI"></param>
        /// <param name="__id"></param>
        public static void SetLocalizedText(TextMeshProUGUI __textUI, string __id) {
            if(__textUI == null)
                return;
                
            if(string.IsNullOrEmpty(__id))
                return;
                
            __textUI.text = GetLocalizedText(__id);
            
            // 텍스트 설정 후 처리 
            if(main.currentAppLanguageCode == CommonConst.COL_AR) {
                SetArabicTextUI(__textUI);
            }
            else {
                if(__textUI.GetComponent<TextLangFontChanger>() != null) {
                    __textUI.GetComponent<TextLangFontChanger>().SetNonArabic();
                }
            }
        }
        
        /// <summary>
        /// TextMeshPro에 텍스트 저장 
        /// 텍스트 저장 후 처리까지!
        /// </summary>
        /// <param name="__textUI"></param>
        /// <param name="__text"></param>
        public static void SetText(TextMeshProUGUI __textUI, string __text) {
            if(__textUI == null)
                return;
                
            __textUI.text = __text; 
            
            // 텍스트 설정 후 처리 
            if(main.currentAppLanguageCode == CommonConst.COL_AR) {
                SetArabicTextUI(__textUI);
            }
            else {
                if(__textUI.GetComponent<TextLangFontChanger>() != null) {
                    __textUI.GetComponent<TextLangFontChanger>().SetNonArabic();
                }
            } 
        }
        
        
        public static void LoadLobbyScene() {
            Signal.Send(LobbyConst.STREAM_COMMON, "LobbyBegin");
            SceneManager.LoadSceneAsync(CommonConst.SCENE_MAIN_LOBBY, LoadSceneMode.Single).allowSceneActivation = true;
        }


        /// <summary>
        /// OnApplicationPause
        /// </summary>
        /// <param name="pauseStatus"></param>
        private void OnApplicationPause(bool pauseStatus) {
            if(pauseStatus) {
                Debug.Log("#### OnApplicationPause pause ####");
            }
            else {
                Debug.Log("#### OnApplicationPause resume ####");
            }
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
                    ShowSystemPopupLocalize("6008", Application.Quit, Application.Quit, true, false);           // 탈퇴, 감사합니다. 앱 종료 
                }
                else
                {
                    Debug.Log(string.Format("Withdraw failed. error is {0}", error));
                    ShowSystemPopup(error.message, null, null, false, false);
                }
            });
        }
        



        /// <summary>
        /// persistentDataPath 모두 삭제하기. (보안)
        /// </summary>
        void ClearPersistentDataPath()
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);

            foreach (FileInfo file in di.GetFiles())
                file.Delete();


            foreach (DirectoryInfo dir in di.GetDirectories())
                dir.Delete(true);
        }
        
        /// <summary>
        /// Uniwebview 연동하기 
        /// </summary>
        /// <param name="__url"></param>
        public void ShowDefaultWebview(string __url, string __title) {
            
            if(SystemManager.main.isWebViewOpened)
                return;
            
                /*            
            GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration = new GamebaseRequest.Webview.GamebaseWebViewConfiguration();
            configuration.title = __title;
            configuration.orientation = GamebaseScreenOrientation.PORTRAIT;
            configuration.colorR = 0;
            configuration.colorG = 0;
            configuration.colorB = 0;
            configuration.colorA = 255;
            configuration.barHeight = 30;
            configuration.isBackButtonVisible = false;
            // configuration.contentMode = GamebaseWebViewContentMode.MOBILE;

            
            Gamebase.Webview.ShowWebView(__url, configuration, (error) =>{ 
                Debug.Log("Webview Closed");
                NetworkLoader.main.RequestUserBaseProperty();
            }, null, null);            
            */
            
            // 언어 설정 
            string langParam = string.Format("?lang={0}", SystemManager.main.currentAppLanguageCode);
            
            webView = WebView.CreateInstance();
            WebView.OnHide += OnHideWebview;
            
            Debug.Log(">> OnHideWebview LoadURL");
            webView.ClearCache();
            webView.SetFullScreen(); // 풀스크린 
            webView.ScalesPageToFit = true;
            // webView.Style = WebViewStyle.Popup; // 팝업 스타일 테스트
            webView.LoadURL(URLString.URLWithPath(__url + langParam));
            webView.Show();            
            
            SystemManager.main.isWebViewOpened = true; // 오픈할때 true로 변경 
            SetBlockBackButton(true);
        }
        
        public void OpenPrivacyURL() {
            ShowDefaultWebview(privacyURL, GetLocalizedText("5050"));
        }
        
        public void OpenTermsURL() {
            ShowDefaultWebview(termsOfUseURL, GetLocalizedText("5049"));
        }
        
        /// <summary>
        /// 코인샵 웹뷰 오픈 
        /// </summary>
        public void OpenCoinShopWebview() {
            
            if(main.isWebViewOpened)
                return;
            
            if(string.IsNullOrEmpty(SystemManager.main.coinShopURL)) {
                Debug.LogError("No Coinshop url");
                return;
            }
            
            
            string uidParam = string.Format("?uid={0}", UserManager.main.GetUserPinCode());
            string langParam = string.Format("&lang={0}", SystemManager.main.currentAppLanguageCode);
            
            string finalURL = SystemManager.main.coinShopURL + uidParam + langParam;
            Debug.Log("Coinshop : " + finalURL);
            
            /*
            GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration = new GamebaseRequest.Webview.GamebaseWebViewConfiguration();
            configuration.title = SystemManager.GetLocalizedText("6186");
            configuration.orientation = GamebaseScreenOrientation.PORTRAIT;
            configuration.colorR = 0;
            configuration.colorG = 0;
            configuration.colorB = 0;
            configuration.colorA = 255;
            configuration.barHeight = 30;
            configuration.isBackButtonVisible = false;
            configuration.contentMode = GamebaseWebViewContentMode.MOBILE;

            
            Gamebase.Webview.ShowWebView(finalURL, configuration, (error) =>{ 
                Debug.Log("Webview Closed");
                NetworkLoader.main.RequestUserBaseProperty();
            }, null, null);            
            */

            webView = WebView.CreateInstance();
            WebView.OnHide += OnHideCoinShopWebview;
            
            
            Debug.Log(">> OpenCoinShopWebview OPEN");
            webView.ClearCache();
            webView.SetFullScreen(); // 풀스크린 
            webView.ScalesPageToFit = true;
            // webView.Style = WebViewStyle.Browser; // 브라우저 스타일 
            webView.LoadURL(URLString.URLWithPath(finalURL));
            webView.Show();
            
            SystemManager.main.isWebViewOpened = true; // 오픈할때 true로 변경 
            SetBlockBackButton(true);
        }
        
        /// <summary>
        /// 작품 로비 외에 
        /// </summary>
        /// <param name="__view"></param>
        void OnHideCoinShopWebview(WebView __view) {
            Debug.Log(">> OnHideCoinShopWebview");
            
            
            SystemManager.main.isWebViewOpened = false;  // 닫힐때 false로 변경 
            SetBlockBackButton(false);
            
            WebView.OnHide -= OnHideCoinShopWebview;
            __view.gameObject.SetActive(false);
            
            Destroy(__view);
            
            if (LobbyManager.main != null)
            {
                UserManager.main.RequestUserGradeInfo(UserManager.main.CallbackUserGreadeInfo);
                NetworkLoader.main.RequestIfyouplayList();
            }            
            
            // 추가로 뱅크 갱신 필요 
            NetworkLoader.main.RequestUserBaseProperty();
        }
        
        
        void OnHideWebview(WebView __view) {
            
            Debug.Log(">> OnHideWebview");
            
            
            SystemManager.main.isWebViewOpened = false;  // 닫힐때 false로 변경 
            SetBlockBackButton(false);
            
            WebView.OnHide -= OnHideWebview;
            __view.gameObject.SetActive(false);
            
            Destroy(__view);

            if (LobbyManager.main != null)
            {
                UserManager.main.RequestUserGradeInfo(UserManager.main.CallbackUserGreadeInfo);
                NetworkLoader.main.RequestIfyouplayList();
            }
        }
        
        void ForwardToStore() {
            Application.OpenURL("http://onelink.to/g9ja38");
        }

        // 웹뷰 강제로 닫기.         
        public void HideWebviewForce() {
            Debug.Log(">> HideWebviewForce");
            
            if(webView == null) {
                return;
            }

            webView.Hide();            
            
        }
        

        
        /// <summary>
        /// 기본 재화의 아이콘 URL 
        /// </summary>
        /// <param name="__currency"></param>
        /// <returns></returns>        
        public string GetCurrencyImageURL (string __currency) {
            if(baseCurrencyData == null || !baseCurrencyData.ContainsKey(__currency))
                return string.Empty;
                
            return GetJsonNodeString(baseCurrencyData[__currency], CommonConst.COL_IMAGE_URL);
        }
        
        
        /// <summary>
        /// 기본 재화의 아이콘 Key
        /// </summary>
        /// <param name="__currency"></param>
        /// <returns></returns>
        public string GetCurrencyImageKey (string __currency) {
            if(baseCurrencyData == null || !baseCurrencyData.ContainsKey(__currency))
                return string.Empty;
                
            return GetJsonNodeString(baseCurrencyData[__currency], CommonConst.COL_IMAGE_KEY);
        }
        
        
        /// <summary>
        /// 조건에 걸리는 프리미엄패스 타임딜 정보 가져오기 
        /// </summary>
        /// <param name="__episodeNumber"></param>
        /// <param name="__type"></param>
        /// <returns></returns>
        public JsonData GetNewTimeDeal(EpisodeData __currentEpisode) {
            if(timedealStandard == null)
                return null;
                
            if(!__currentEpisode.isValidData)
                return null;
                
            if(__currentEpisode.episodeType == EpisodeType.Side)
                return null;
            
            if(__currentEpisode.episodeType == EpisodeType.Chapter) { // * 정규 에피소드에 대한 체크 
                for(int i=0; i<timedealStandard.Count;i++) {
                    
                    if(GetJsonNodeBool(timedealStandard[i], "conditions"))
                        continue;
                        
                    // 넘어온 number에서 -1 값이랑 동일한 progress가 있는지 체크한다. 
                    if(GetJsonNodeInt(timedealStandard[i], "episode_progress") == __currentEpisode.episodeNumber - 1) {
                        return timedealStandard[i]; // * 찾았다..!
                    }
                    
                }
            }
            else if (__currentEpisode.episodeType == EpisodeType.Ending) {  // 엔딩에 대한 체크 
            
            
                // 히든엔딩만 해당한다. 
                if(__currentEpisode.endingType != "hidden")
                    return null;
                
                for(int i=0; i<timedealStandard.Count;i++) {
                    
                    // 컨디션 값 1을 걸 찾는거다. 
                    // 공용 타임딜 정보에는 히든엔딩 관련 row는 하나 밖에 없다. 
                    if(GetJsonNodeBool(timedealStandard[i], "conditions"))
                        return timedealStandard[i];
                }                
            }
            
            return null;
            
            
        } // ? GetNewTimeDeal
        
        
        
        // ! 여기서부터 static 친구들 
        
        /// <summary>
        /// 작품 추천 팝업 띄우기 
        /// </summary>
        /// <param name="__data"></param>
        public static void ShowRecommendStoryPopup(JsonData __data) {
            
            if(__data == null || __data.Count == 0) {
                Debug.Log("No Recommend popup");
                return;
            }
            
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_RECOMMEND);
            
            p.Data.contentJson = __data; // 데이터 전달 
            
            // 팝업 큐 
            PopupManager.main.ShowPopup(p, true);
        }
        
        /// <summary>
        /// 인트로 팝업 띄우기 
        /// </summary>
        public static void ShowIntroPopup() {
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_INTRO);
            
            p.Data.contentJson = main.introData; // 인트로 데이터 전달 
            
            PopupManager.main.ShowPopup(p, true);
        }
        
        /// <summary>
        /// 재화 부족할 때 띄우는 팝업 호출
        /// </summary>
        /// <param name="isGem">true = 스타 부족, false = 코인 부족</param>
        /// <param name="commentLocalizingId">타이틀 밑에 들어가는 부가 설명 텍스트가 들어갈 로컬라이징 id값</param>
        /// <param name="price">필요한 가격</param>
        public static void ShowLackOfCurrencyPopup(bool isGem, string commentLocalizingId, int price)
        {
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_LACK_OF_CURRENCY);

            string title = isGem ? GetLocalizedText("6319") : GetLocalizedText("6320");

            p.Data.isPositive = isGem;
            p.Data.contentValue = price;
            p.Data.SetLabelsTexts(title, GetLocalizedText(commentLocalizingId));
            PopupManager.main.ShowPopup(p, false);
        }

        
        
        /// <summary>
        /// 생성시 데이터가 없는 팝업 
        /// </summary>
        /// <param name="__popupName"></param>
        public static void ShowNoDataPopup(string __popupName, bool __Queue = false) {
            PopupBase p = PopupManager.main.GetPopup(__popupName);
            
            if(p == null) {
                Debug.LogError("There is No Popup : " + __popupName);
                NetworkLoader.main.ReportRequestError(__popupName, "No PopupBase");
                return;
            }
            
            PopupManager.main.ShowPopup(p, __Queue);
        }
        
        /// <summary>
        /// 능력치 팝업 띄우기.. 
        /// </summary>
        /// <param name="__speaker"></param>
        /// <param name="__abilityName"></param>
        /// <param name="__addValue"></param>
        public static void ShowAbilityPopup(string __speaker, string __abilityName, int __addValue) {
            // 팝업 띄워주기 
            PopupBase p = PopupManager.main.GetPopup(GameConst.POPUP_GAME_ABILITY);
            p.Data.contentJson = UserManager.main.GetSpeakerAbilityJSON(__speaker, __abilityName);
            p.Data.contentValue = __addValue;
            
            if(p.Data.contentJson == null) {
                SystemManager.main.ShowMissingFunction(string.Format("[{0}]/[{1}] 능력 정보 없음", __speaker, __abilityName));
                return;
            }
            
            PopupManager.main.ShowPopup(p, true, false);
        }
        
        
        /// <summary>
        /// 어드레서블 Live2D 모델의 유효성 체크 
        /// </summary>
        public static bool CheckAddressableCubismValidation(CubismModel __model) {
            ModelClips clips = __model.gameObject.GetComponent<ModelClips>();  // 에셋번들에서 받아온 AnimationClips. 
            
            // * 페이드 모션 체크. 리스트에 저장된 페이드오브젝트와 클립 개수가 안맞으면 사용하지 않는다
            CubismFadeController cubismFadeController = __model.gameObject.GetComponent<CubismFadeController>();
            CubismMotionController cubismMotionController = __model.gameObject.GetComponent<CubismMotionController>();
            
           if(cubismFadeController == null ||
                cubismMotionController == null ||
                cubismFadeController.CubismFadeMotionList == null || 
                cubismFadeController.CubismFadeMotionList.CubismFadeMotionObjects.Length != clips.ListClips.Count) {
                    
                return false; // 유효하지 않음
                    
            }
            
            return true; // 유효하다.!
        }
        
        /// <summary>
        /// 어드레서블 사용시 Live2D Shader 처리 
        /// </summary>
        public static void SetAddressableCubismShader(CubismModel __model) {
            // * 어드레서블 에셋을 통한 생성인 경우는 Shader 처리 추가 필요. 
            Shader cubismShader = Shader.Find("Live2D Cubism/Unlit");
            CubismRenderer render;
            for(int i=0;i <__model.Drawables.Length;i++) {
                render = __model.Drawables[i].gameObject.GetComponent<CubismRenderer>();
                render.Material.shader = cubismShader;
            }   
        }
        
        /// <summary>
        /// 기본 서버  접속 실패 메세지 
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultServerErrorMessage() {
            switch(main.currentAppLanguageCode) {
                case "KO":
                return "서버에 접속하지 못했습니다.";
                
                case "JA":
                return "サーバーとの接続に失敗しました。";
                
                default:
                return "Failed to connect to server.";
            }
        }
        
        
        /// <summary>
        /// 로비에서 게임 종료 팝업 호출 
        /// </summary>
        /// <param name="__currentView"></param>
        public static void CheckExitPopupShowInLobby(CommonView __currentView) {
            
            Debug.Log("^^^ CheckExitPopupShowInLobby ");
            
            CommonView.DeleteDumpViews();
            
            // 웹뷰 활성화중에는 웹뷰를 닫는다. 
            if(main != null && main.isWebViewOpened) {
                Debug.Log("^^^ CheckExitPopupShowInLobby #1");
                main.HideWebviewForce();
                return;
            }
            
            if(PopupManager.main.GetFrontActivePopup() == null 
            && ((CommonView.ListActiveViews.Count == 1 && CommonView.ListActiveViews.Contains(__currentView)) || CommonView.ListActiveViews.Count < 1)) {
                SystemManager.ShowSystemPopup(SystemManager.GetLocalizedText("6064"), Application.Quit, null, true);    
            }
        }
        
        
        public static void ShowPopupPass(string __projectID, bool __isIndependant = false) {
            
            PopupBase p = PopupManager.main.GetPopup("PremiumPass");
            
            if(p == null) {
                Debug.LogError("No Premium Pass popup");
                return;
            }
            
            ShowNetworkLoading(true);
            
            p.Data.targetData = __projectID; // 대상 스토리의 프로젝트ID를 넘겨줘서 오픈시킨다. 
            
            // 프리미엄 패스 팝업이 무거워서. 여러번 누르는 경우가 있다. 
            // 그래서 동일 팝업이 뜨지 않게 처리.. 
            if(__isIndependant)
                PopupManager.main.ShowIndependentPopup(p); 
            else 
                PopupManager.main.ShowPopup(p, true, false);
            
        }
        
        
        /// <summary>
        /// 아랍 TextMeshPro 설정 
        /// </summary>
        /// <param name="__textUI"></param>
        public static void SetArabicTextUI(TextMeshProUGUI __textUI) {
            
            originArabicText = __textUI.text;
               __textUI.isRightToLeftText = true;
            finalText.Clear();
            RTLSupport.FixRTL(originArabicText, finalText, false, true, true);
            finalText.Reverse();
            
            __textUI.text = finalText.ToString();
            
            // 정렬에 대한 추가 처리 
            if(__textUI.GetComponent<TextLangFontChanger>() != null) {
                __textUI.GetComponent<TextLangFontChanger>().SetArabicAlignment();
            }            
        }
        
        
        /// <summary>
        /// Doozy BackButton 제어 
        /// </summary>
        /// <param name="__flag"></param>
        public static void SetBlockBackButton(bool __flag) {
            Doozy.Runtime.UIManager.Input.BackButton.blockBackInput = __flag;
        }
        
        /// <summary>
        /// 저작권 웹페이지 오픈 
        /// </summary>
        public void OpenCopyrightURL() {
            Application.OpenURL(main.copyrightURL);
        }
 
    }
}