using System.Collections.Generic;
using UnityEngine;
using LitJson;
using BestHTTP;
using Sirenix.OdinInspector;
using System.Collections;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public enum EpisodeType { Chapter, Ending, Side }; // 에피소드 타입 
    // 에피소드 상태 enum
    public enum EpisodeState { None, Prev, Current, Future, Block };    // 에피소드 진행중에는 prev, current, future, block 에피소드가 엔딩에 들어온 경우 해당 엔딩 제외 전부 block처리
    public enum PurchaseState { None, Permanent, OneTime, Rent, Free, AD } // 에피소드 구매 상태  (구매이력 없음/영구구매/1회권/대여/광고)
    
    public enum CurrencyType {
        Gem,
        Coin,
        Freepass,
        OneTime, // 1회권
        
        Rent // 대여권
    }
    

    /// <summary>
    /// 선택한 스토리의 기본 정보, 리스트, 말풍선 세트에 관여하는 클래스 
    /// </summary>
    
    public class StoryManager : MonoBehaviour
    {
        public static StoryManager main = null;

        
        public StoryData CurrentProject = null; // 현재 선택한 작품 Master 정보 
        JsonData ProjectDetailJson = null; // 조회로 가져온 작품에 대한 기준정보와 유저정보.
        [SerializeField] EpisodeData CurrentEpisodeData = null; // 선택한 에피소드 정보(JSON => Serializable Class)

        [HideInInspector] public JsonData EpisodeListJson = null; // 에피소드 리스트 JSON 
        [HideInInspector] public JsonData SideEpisodeListJson = null; // 사이드 에피소드 리스트 JSON 
        
        public List<EpisodeData> RegularEpisodeList = new List<EpisodeData>(); // chapter(정규)만 따로 빼놓는다.
        public List<EpisodeData> ReverseRegularEpisodeList = new List<EpisodeData>(); // 정규 에피소드의 역순 
        public List<EpisodeData> SideEpisodeList = new List<EpisodeData>(); // 사이드 에피소드
        public List<EpisodeData> ListCurrentProjectEpisodes = new List<EpisodeData>(); // 현재 선택된 혹은 플레이중인 작품의 EpisodeData의 List.
        
        [Space]
        [Header("== 에피소드 카운팅 ==")]
        public int regularEpisodeCount = 0; // 엔딩을 제외한 정규 에피소드 개수
        public int totalEndingCount = 0;    // 엔딩 총 개수
        public int unlockEndingCount = 0; // 해금된 엔딩 개수
        public int sideEpisodeCount = 0; // 사이드 에피소드 개수
        public int unlockSideEpisodeCount = 0; // 해금된 사이드 에피소드 개수 
        [Space]
        

        JsonData totalStoryListJson = null; // 조회로 가져온 모든 작품 리스트(all)
        JsonData recommendStoryIdJSON = null; // 추천 작품 ID JSON
        
        
        public List<StoryData> listTotalStory = new List<StoryData>(); // 작품 리스트 
        public List<StoryData> listRecommendStory = new List<StoryData>(); // 추천 작품 리스트 

        #region 말풍선 세트와 관련된 JSON , 변수

        JsonData bubbleSetJson = null; // 말풍선 세트 (마스터)
        JsonData bubbleSpriteJson = null; // 말풍선 스프라이트 정보 

        [HideInInspector]
        public JsonData talkBubbleJson = null; // 대화 말풍선 기준정보
        [HideInInspector]
        public JsonData whisperBubbleJson = null; // 속삭임 말풍선 기준정보
        [HideInInspector]
        public JsonData feelingBubbleJson = null; // 속마음 말풍선 기준정보
        [HideInInspector]
        public JsonData yellBubbleJson = null; // 외침 말풍선 기준정보

        [HideInInspector]
        public JsonData monologueBubbleJson = null; // 독백 말풍선 기준정보

        [HideInInspector]
        public JsonData speechBubbleJson = null; // 중요대사 말풍선 기준정보
 
        public Dictionary<string, string> bubbleID_Dictionary; // 말풍선 id-url 집합 
        public Dictionary<string, string> bubbleURL_Dictionary; // 말풍선 url-key 집합 

        //네임태그 dict!
        Dictionary<string, JsonData> DictNametag = new Dictionary<string, JsonData>();

        #endregion

        #region 선택한 작품의 캐릭터 모델 데이터 
        JsonData modelJson = null; // 프로젝트 모델 JSON 
        Dictionary<string, JsonData> DictProjectModel; // 프로젝트 모델의 Dictionary (캐릭터이름 - Json 조합)
        [ShowInInspector] Dictionary<string, string> DictProjectModelDebug; // Debug 

        JsonData liveIllustJson = null; // 프로젝트 라이브 일러스트 JSON
        Dictionary<string, JsonData> DictProjectLiveIllust; // 라이브 일러스트 Dictionary 일러스트이름 - JSON 조합
        [ShowInInspector] Dictionary<string, string> DictProjectLiveIllustDebug; // Debug 

        JsonData liveObjectJson = null; // 프로젝트 라이브 오브젝트 JSON 
        Dictionary<string, JsonData> DictProjectLiveObject; // 라이브 오브젝트 Dictionary 오브젝트이름 - JSON 조합
        [ShowInInspector] Dictionary<string, string> DictProjectLiveObjectDebug; // Debug 

        #endregion

        #region 선택한 작품의 기준정보 데이터 
        
        JsonData storyDetailJson = null; // 작품 상세정보(타이틀,요약,작가, 썸네일, 완결 여부 등)

        
        JsonData illustJson = null; // 이미지 일러스트 정보
        JsonData minicutJSON = null; // 이미지 미니컷 정보 
        JsonData dressCodeJson = null; // 의상 정보
        JsonData emoticonJson = null; // 이모티콘 정보(백그라운드 다운로드)
        JsonData loadingJson = null;    // 로딩 스크린 정보
        [SerializeField] Dictionary<string, byte[]> DictDownloadedEmoticons = new Dictionary<string, byte[]>(); // 다운받아놓은 이모티콘 bytes. 
        [SerializeField] int CountDownloadingEmoticon = 0; // 이모티콘 다운로드 카운팅용! 
        [SerializeField] bool isDownloadEmoticonsSaved = false; // 이모티콘 다운로드 1회 실행했는지 체크용 변수

        Dictionary<string, byte[]> DictDownloadedLoadings = new Dictionary<string, byte[]>();       // 다운 받아놓은 로딩화면 bytes
        int countDownloadingLoading = 0;        // 로딩화면 다운로드 카운팅용
        bool isDownloadLoadingsSaved = false;   // 로딩화면 다운로드 1회 실행했는지 체크용 변수


        
        // BGM 배너 URL & KEY
        [SerializeField] string bgmBannerURL = string.Empty; 
        [SerializeField] string bgmBannerKey = string.Empty;
        public string freepassBannerURL = string.Empty;
        public string freepassBannerKey = string.Empty;
        public string freepassTitleURL = string.Empty;
        public string freepassTitleKey = string.Empty;

        #endregion

        #region 네임태그 정보
        
        [HideInInspector]
        public JsonData storyNametagJSON = null;

        #endregion
        
        JsonData storyCurrencyJSON = null; // 작품 화폐 정보 


        
        public static bool enterGameScene = false;          // Game scene으로 진입했는지?
        public static bool playSideEpisode = false;         // speical episode를 플레이했는지


        
        public string CurrentProjectID = string.Empty;      // 선택한 프로젝트 ID 
        public string CurrentProjectTitle = string.Empty;   // 선택한 프로젝트 Title 
        public string CurrentEpisodeID = string.Empty;      // 선택한 에피소드 ID 
        public string CurrentEpisodeTitle = string.Empty;   // 선택한 에피소드 제목

        // EpisodeElement prefab에서 사용될 리소스들
        
        [SerializeField] string currentBubbleSetID = string.Empty; // 현재 말풍선 세트 ID 
        [SerializeField] int currentBubbleSetVersion = 0; // 현재 말풍선세트 버전 정보 
        JsonData currentBubbleSetJson = null;

        #region static, const

        // ..
        public const string KEY_BUBBLE_DETAIL_PREFIX = "BubbleDetail";
        public const string KEY_BUBBLE_VER_PREFIX = "BubbleVer";
        public const string KEY_PROJECT_BUBBLE_SET_ID = "ProjectBubbleSetID"; // 프로젝트 Bubble SET ID 


        public const string NODE_EPISODE = "episodes";
        public const string NODE_SIDE = "sides";
        public const string NODE_DRESS_CODE = "dressCode";
        const string NODE_PROJECT_LIVE_ILLUSTS = "liveIllusts"; // 프로젝트 라이브 일러스트 
        const string NODE_PROJECT_LIVE_OBJECTS = "liveObjects"; // 프로젝트 라이브 오브젝트

        const string NODE_BUBBLE_MASTER = "bubbleMaster"; // 말풍선 마스터 노드 
        const string NODE_NAMETAG = "nametag";  // 네임태그 노드 
        
        const string NODE_CURRENCY = "currency"; // 프로젝트 화폐코드 
        const string NODE_FREEPASS_PRICE = "freepassPrice"; // 프리패스 Price
        
        // * 프로젝트에 소속된 배너 및 관련 이미지 친구들 
        const string NODE_GALLERY_BANNER = "galleryBanner";
        const string NODE_BGM_BANNER = "bgmBanner";
        const string NODE_FREEPASS_BANNER = "freepassBanner";
        const string NODE_FREEPASS_TITLE = "freepassTitle";
        const string NODE_FREEPASS_ORIGIN_PRICE = "origin_freepass_price";
        const string NODE_FREEPASS_SALE_PRICE = "sale_freepass_price";
        const string NODE_FREEPASS_PRODUCT = "freepasProduct"; // 작품별 프리패스 타임딜 상품리스트
        
        const string NODE_PROJECT_ILLUSTS = "illusts"; // 프로젝트의 모든 일러스트 
        const string NODE_PROJECT_MINICUTS = "minicuts"; // 프로젝트의 모든 미니컷
        const string NODE_PROJECT_MODELS = "models"; // 프로젝트의 모든 모델 파일리스트 
        

        public const string EVENT_STORY_INFO_LOADING = "EventStoryInfoLoding"; // 스토리 정보 로딩 화면 
        public const string EVENT_STORY_INFO_LOADING_CLOSE = "EventCloseStoryInfoLoding"; // 스토리 정보 로딩 화면 
        public const string EVENT_STORY_INFO_OPEN = "EventStoryInfoOpen"; // 스토리 정보 오픈 요청 

        // 말풍선 관련 컬럼들 추가 
        public const string BUBBLE_TALK = "talk";
        public const string BUBBLE_WHISPER = "whisper";
        public const string BUBBLE_FEELING = "feeling";
        public const string BUBBLE_YELL = "yell";
        public const string BUBBLE_MONOLOGUE = "monologue"; // 독백 
        public const string BUBBLE_SPEECH = "speech"; // 중요대사



        public const string BUBBLE_VARIATION_NORMAL = "normal";
        public const string BUBBLE_VARIATION_EMOTICON = "emoticon";
        public const string BUBBLE_VARIATION_REVERSE_EMOTICON = "reverse_emoticon";
        public const string BUBBLE_VARIATION_DOUBLE = "double"; // 2인스탠딩 배리에이션 

        #endregion


        


        private void Awake()
        {
            // 다른 씬에서 넘어온 객체가 있을경우. 
            if (main != null)
            {
                Destroy(this.gameObject);
                return;
            }

            // Singleton
            main = this;
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 스토리 정보 초기화 하기 
        /// </summary>
        public void InitStoryInfo()
        {
            illustJson = null;
            minicutJSON = null;
            EpisodeListJson = null;
            storyNametagJSON = null;
            storyCurrencyJSON = null;

            EpisodeListJson = null;
            SideEpisodeListJson = null;

            ProjectDetailJson = null;
            CurrentProjectID = string.Empty;
            CurrentProjectTitle = string.Empty;
        }



        /// <summary>
        /// 어드민에 있는 작품 리스트를 가져온다.
        /// </summary>
        public void RequestStoryList(OnRequestFinishedDelegate __cb)
        {
            JsonData sendingData = new JsonData();
            sendingData["func"] = NetworkLoader.FUNC_SELECT_LOBBY_PROJECT_LIST;
            sendingData["package"] = Application.identifier;

            NetworkLoader.main.SendPost(__cb, sendingData);
        }
        
        /// <summary>
        /// 작품들 리스트화 하기.
        /// </summary>
        /// <param name="__listJSON"></param>
        public void SetStoryList(JsonData __listJSON) {
            totalStoryListJson = __listJSON["all"];
            recommendStoryIdJSON = __listJSON["recommend"];
            
            listTotalStory.Clear();
            for(int i=0; i<totalStoryListJson.Count;i++) {
                StoryData storyData = new StoryData(totalStoryListJson[i]);
                listTotalStory.Add(storyData);
            }
            
            
            // 추천 작품 처리 
            listRecommendStory.Clear();
            for(int i=0;i<listTotalStory.Count;i++) {
                // if(listTotalStory[i].projectID)
                for(int j=0;j<recommendStoryIdJSON.Count;j++) {
                    if(listTotalStory[i].projectID == recommendStoryIdJSON[j].ToString()) {
                        listRecommendStory.Add(listTotalStory[i]);
                    }
                }
            }
               
        }

        /// <summary>
        /// 작품 Id로 해당 작품 찾기
        /// </summary>
        public StoryData FindProject(string __projectId)
        {
            if (listTotalStory.Count < 1)
                return null;

            foreach(StoryData sd in listTotalStory)
            {
                if (sd.projectID == __projectId)
                    return sd;
            }

            return null;
        }

       
        /// <summary>
        /// 스토리 정보 요청 
        /// </summary>
        /// <param name="__projectID">프로젝트 ID </param>
        /// <param name="__J">작품 기본 정보</param>
        public void RequestStoryInfo(StoryData __storyData)
        {
            Debug.Log(string.Format("<color=yellow>RequestStoryInfo [{0}]</color>", __storyData.projectID));


            // 네트워크 로딩 표시 
            SystemManager.ShowNetworkLoading();

            // 초기화
            InitStoryInfo();

            // Game 씬에서 단독으로 실행 안된다. (아직 기능 추가 되지 않음)
            // 선택한 프로젝트에 대한 종합정보가 들어온다.(유저정보 포함)
            SetCurrentProject(__storyData);
            SystemManager.main.givenStoryData = __storyData;

            
            // 통신 준비 
            JsonData sendingData = new JsonData();
            sendingData["func"] = NetworkLoader.FUNC_SELECTED_STORY_INFO;
            sendingData["project_id"] = __storyData.projectID;
            
            // 로컬에 저장되어 있었던, 프로젝트와 연결된 말풍선 세트 정보를 함께 전송한다. 
            // 달라진 부분이 있는 경우에만 말풍선 세트 정보를 새로 받는다 (텍스트가 많다.)
            sendingData["clientBubbleSetID"] = currentBubbleSetID; 
            sendingData["userBubbleVersion"] = currentBubbleSetVersion; // 유저가 저장하고 있던 말풍선 세트의 버전 정보를 전달 

            // 선택지 히스토리 통신도 함께 진행합시다(여기서 받으면 선택지 페이지 안들어갔다고 해도, 에피소드 종료 페이지에서 사용해야함)
            UserManager.main.SetCurrentStorySelectionList(__storyData.projectID);


            // 콜백에서 이어지도록 처리 
            NetworkLoader.main.SendPost(CallbackStoryInfo, sendingData);

        }

        /// <summary>
        /// 선택한 작품의 기본 정보 설정 
        /// </summary>
        /// <param name="__j"></param>
        public void SetCurrentProject(StoryData __storyData)
        {
            CurrentProject = __storyData;
            CurrentProjectID = CurrentProject.projectID;
            CurrentProjectTitle = CurrentProject.title;


            // 말풍선 세트 ID 
            currentBubbleSetID = CurrentProject.bubbleSetID;

            // 로컬에 저장된 정보 불러온다. 
            LoadProjectBubbleSetID(CurrentProjectID); // 프로젝트에 연결된 말풍선 세트 ID
            LoadBubbleSetLocalInfo(currentBubbleSetID);  // 말풍선 세트 버전 정보
        }

        /// <summary>
        ///  콜백, 스토리에 관련된 정보를 모두 불러왔습니다.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void CallbackStoryInfo(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Network Error in CallbackStoryInfo"); 
                
                // 실패하면 메세지가 뜨고, 화면 진입하지 않음. 
                return;
            }
            
            

            // 작품의 기준정보, 작품과 관련된 유저 정보.. 엄청나게 많이 온다. 
            // 모든 정보는 받아와서 UserManager에게 넘겨주고 (사용자 정보가 포함되어있다.) 
            // 기준 정보만 StoryManager에서 보관하는 방식 
            ProjectDetailJson = JsonMapper.ToObject(res.DataAsText);
            // 유저 정보 할당 
            UserManager.main.SetStoryUserData(ProjectDetailJson);

            #region 말풍선 밑작업

            // 말풍선의 경우 데이터가 많아서, 버전 관리를 통해서 신규 버전이 있을때만 서버에서 내려받도록 합니다. 
            currentBubbleSetID = ProjectDetailJson[NODE_BUBBLE_MASTER]["bubbleID"].ToString(); // 연결된 말풍선 세트 ID 
            currentBubbleSetVersion = int.Parse(ProjectDetailJson[NODE_BUBBLE_MASTER]["bubble_ver"].ToString()); // 말풍선 버전 

            // 말풍선 기초정보가 없는 경우, Local의 정보를 불러와서 설정 
            if (ProjectDetailJson.ContainsKey(UserManager.NODE_BUBBLE_SET))
            {
                // 말풍선 정보 있으면, 로컬에 저장하기
                currentBubbleSetJson = ProjectDetailJson[UserManager.NODE_BUBBLE_SET];

                // 저장요청 
                SaveBubbleSetLocalInfo(currentBubbleSetID);
            }
            else
            {
                // 말풍선 정보 없는 경우는 로컬에서 로드하여 노드에 할당 
                ProjectDetailJson[UserManager.NODE_BUBBLE_SET] = currentBubbleSetJson;
            }
            #endregion


            // 말풍선 정보 세팅
            SetBubbles();

            // 프로젝트 네임태그 
            SetProjectNametag();

            // 프로젝트 모델, 라이브 일러스트, 오브젝트 설정
            SetProjectModels();
            SetProjectLiveIllusts();
            SetProjectLiveObjects();
            

            // 프로젝트 기준정보 설정 
            SetProjectStandard();
            
            // 현재 작품의 EpisodeData 클리어 . 
            ListCurrentProjectEpisodes.Clear();
            

            // 에피소드 정보 할당
            EpisodeListJson = ProjectDetailJson[NODE_EPISODE]; // 프로젝트의 정규 에피소드
            SideEpisodeListJson = ProjectDetailJson[NODE_SIDE]; // 프로젝트의 사이드 에피소드
            sideEpisodeCount = SideEpisodeListJson.Count; // 사이드 에피소드 전체 
            unlockSideEpisodeCount = 0; 
            
            EpisodeData newEpisodeData = null;
            
            SideEpisodeList.Clear();
            
            // * 해금된 사이드 에피소드 개수 구하기 
            for(int i=0; i<SideEpisodeListJson.Count;i++) {
                
                newEpisodeData = new EpisodeData(SideEpisodeListJson[i]);
                
                ListCurrentProjectEpisodes.Add(newEpisodeData);  // 모든 에피소드 
                SideEpisodeList.Add(newEpisodeData); // 사이드만 모아주기 
                
                if(SystemManager.GetJsonNodeBool(SideEpisodeListJson[i], CommonConst.IS_OPEN))
                    unlockSideEpisodeCount++;
            }
            
            
            // 정규 에피소드 수집 
            RegularEpisodeList.Clear();
            regularEpisodeCount = 0;
            unlockEndingCount = 0;
            
           
            for(int i=0; i<EpisodeListJson.Count;i++) {
                
                newEpisodeData = new EpisodeData(EpisodeListJson[i]);
                
                ListCurrentProjectEpisodes.Add(newEpisodeData); // 정규+엔딩 모아주기 
                
                if (SystemManager.GetJsonNodeBool(EpisodeListJson[i], "ending_open"))
                        unlockEndingCount++;
                
                
                if (EpisodeListJson[i][LobbyConst.STORY_EPISODE_TYPE].ToString() != CommonConst.COL_CHAPTER)
                {
                    totalEndingCount++;
                    continue;
                }
                
                RegularEpisodeList.Add(newEpisodeData);
            }
            
            regularEpisodeCount = RegularEpisodeList.Count; // 카운팅 
            
            
            
            #region 에피소드 역순 배열을 위한 작업 
            
            ReverseRegularEpisodeList.Clear();
            
              
            // * IF YOU 로직 
            for(int i=RegularEpisodeList.Count-1; i>=0; i--) {
                ReverseRegularEpisodeList.Add(RegularEpisodeList[i]);
            }
            
            #endregion

            Debug.Log(string.Format("<color=yellow>[{0}] Episodes are loaded </color>", EpisodeListJson.Count));


            // 기초작업 완료 후 View 오픈 요청 
            OpenViewStoryDetail(); 

        }
        
        
        /// <summary>
        /// 선택된 프로젝트의 정규 에피소드 데이터 리프레시 
        /// </summary>
        public void UpdateRegularEpisodeData() {
            for(int i=0; i<ListCurrentProjectEpisodes.Count;i++) {
                ListCurrentProjectEpisodes[i].SetEpisodePlayState();
            }    
        }


        /// <summary>
        /// 스토리 상세화면으로 진입하도록 이벤트 봬기. 
        /// 로비에서 진입시 이 부분에서 View 이동
        /// </summary>
        void OpenViewStoryDetail()
        {
            Debug.Log("OpenViewStoryDetail");
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_STORY_DETAIL, "open!");
            // ViewStoryDetail
            /*
            if (!enterGameScene)
            {
                Debug.Log("OpenViewStoryDetail");
                Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_STORY_DETAIL, "open!");
            }
            */
        }

        #region 네임태그 관련 메소드 

        /// <summary>
        /// 프로젝트 네임태그 설정 
        /// </summary>
        void SetProjectNametag()
        {
            if (!ProjectDetailJson.ContainsKey(NODE_NAMETAG))
            {
                Debug.Log("Nametag Info is null");
                storyNametagJSON = null;
            }

            storyNametagJSON = ProjectDetailJson[NODE_NAMETAG];

            // 편한접근을 위해서 Dictionary 구성 
            DictNametag.Clear();

            // speaker 컬럼별로 분류!
            string speaker = string.Empty;
            for(int i =0; i<storyNametagJSON.Count;i++)
            {
                speaker = storyNametagJSON[i]["speaker"].ToString();

                if(!DictNametag.ContainsKey(speaker))
                {
                    DictNametag.Add(speaker, storyNametagJSON[i]);
                }
            }
        }

        /// <summary>
        /// 네임태그의 언어별 캐릭터 이름 
        /// </summary>
        /// <param name="__speaker"></param>
        /// <returns></returns>
        public string GetNametagName(string __speaker)
        {
            if (!DictNametag.ContainsKey(__speaker))
                return string.Empty;

            // 임시로 한글명만. 
            return DictNametag[__speaker][CommonConst.COL_KO].ToString();
        }

        /// <summary>
        /// 네임태그의 색상정보 알려주세요
        /// </summary>
        /// <param name="__speaker"></param>
        /// <param name="__isMainColor"></param>
        /// <returns></returns>
        public string GetNametagColor(string __speaker, bool __isMainColor = true)
        {
            if (!DictNametag.ContainsKey(__speaker))
                return GameConst.COLOR_BLACK_RGB;

            if(__isMainColor)
                return DictNametag[__speaker][GameConst.COL_MAIN_COLOR].ToString();
            else
                return DictNametag[__speaker][GameConst.COL_SUB_COLOR].ToString();
        }

        #endregion


        /// <summary>
        /// 프로젝트 기준정보 설정
        /// </summary>
        void SetProjectStandard()
        {
            // 프로젝트 귀속 이미지들 초기화 
            bgmBannerURL = string.Empty;
            bgmBannerURL = string.Empty;
            freepassBannerURL = string.Empty;
            freepassBannerKey = string.Empty;
            freepassTitleURL = string.Empty;
            freepassTitleKey = string.Empty;
            
            illustJson = GetNodeProjectIllusts(); // 일러스트 기본 정보 
            minicutJSON = GetNodeProjectMinicuts(); // 미니컷 기본정보 
            
            dressCodeJson = ProjectDetailJson[NODE_DRESS_CODE]; // 의상 
            
            storyCurrencyJSON = ProjectDetailJson[NODE_CURRENCY]; // 화폐
            
            // 상세정보 
            storyDetailJson = ProjectDetailJson[LobbyConst.NODE_DETAIL][0];

            // * 바보! GetJsonNode와 RequestDownloadImage 메소드에 유효성 검사를 추가하고 여기서는 신경쓰지 않게 한다. 
            
           
            // 갤러리 
            bgmBannerURL = SystemManager.GetJsonNodeString(SystemManager.GetJsonNode(ProjectDetailJson,NODE_BGM_BANNER), SystemConst.IMAGE_URL);
            bgmBannerKey = SystemManager.GetJsonNodeString(SystemManager.GetJsonNode(ProjectDetailJson,NODE_BGM_BANNER), SystemConst.IMAGE_KEY);
            SystemManager.RequestDownloadImage(bgmBannerURL, bgmBannerKey, null);

            freepassBannerURL = SystemManager.GetJsonNodeString(SystemManager.GetJsonNode(ProjectDetailJson, NODE_FREEPASS_BANNER), SystemConst.IMAGE_URL);
            freepassBannerKey = SystemManager.GetJsonNodeString(SystemManager.GetJsonNode(ProjectDetailJson, NODE_FREEPASS_BANNER), SystemConst.IMAGE_KEY);
            SystemManager.RequestDownloadImage(freepassBannerURL, freepassBannerKey, null);
            
            freepassTitleURL = SystemManager.GetJsonNodeString(SystemManager.GetJsonNode(ProjectDetailJson, NODE_FREEPASS_TITLE), SystemConst.IMAGE_URL);
            freepassTitleKey = SystemManager.GetJsonNodeString(SystemManager.GetJsonNode(ProjectDetailJson, NODE_FREEPASS_TITLE), SystemConst.IMAGE_KEY);
            SystemManager.RequestDownloadImage(freepassBannerURL, freepassBannerKey, null);
            
        }

        /// <summary>
        /// 프로젝트 라이브 오브젝트 설정
        /// </summary>
        void SetProjectLiveObjects()
        {
            liveObjectJson = ProjectDetailJson[NODE_PROJECT_LIVE_OBJECTS];
            SetProjectLiveObjectDictionary();
        }

        /// <summary>
        /// 프로젝트 라이브 일러스트 설정 
        /// </summary>
        void SetProjectLiveObjectDictionary()
        {
            DictProjectLiveObject = new Dictionary<string, JsonData>();
            DictProjectLiveObjectDebug = new Dictionary<string, string>();

            Debug.Log("liveObjectJson count : " + liveObjectJson.Count); // 카운트 체크 

            // Dictionary로 분류 
            foreach (string key in liveObjectJson.Keys)
            {
                DictProjectLiveObject.Add(key, liveObjectJson[key]);
                DictProjectLiveObjectDebug.Add(key, JsonMapper.ToStringUnicode(liveObjectJson[key]));
            }
        }


        /// <summary>
        /// 프로젝트 라이브 일러스트 설정
        /// </summary>
        void SetProjectLiveIllusts()
        {
            liveIllustJson = ProjectDetailJson[NODE_PROJECT_LIVE_ILLUSTS];
            SetProjectLiveIllustDictionary();
        }

        /// <summary>
        /// 프로젝트 라이브 일러스트 딕셔너리 설정 
        /// </summary>
        void SetProjectLiveIllustDictionary()
        {
            DictProjectLiveIllust = new Dictionary<string, JsonData>();
            DictProjectLiveIllustDebug = new Dictionary<string, string>();

            Debug.Log("liveIllustJson count : " + liveIllustJson.Count); // 카운트 체크 

            // Dictionary로 분류 
            foreach (string key in liveIllustJson.Keys)
            {
                DictProjectLiveIllust.Add(key, liveIllustJson[key]);
                DictProjectLiveIllustDebug.Add(key, JsonMapper.ToStringUnicode(liveIllustJson[key]));
            }
        }

        /// <summary>
        /// 프로젝트 모델 정보 설정 
        /// </summary>
        void SetProjectModels()
        {
            // 프로젝트 모델 JSON 
            modelJson = GetNodeProjectModels();
            SetProjectModelDictionary(); // 모델별로 Dictionary로 정리 
        }

        /// <summary>
        /// 프로젝트 모델파일 Dictionary로 만들기 
        /// </summary>
        void SetProjectModelDictionary()
        {
            DictProjectModel = new Dictionary<string, JsonData>();
            DictProjectModelDebug = new Dictionary<string, string>();

            Debug.Log("modelJson count : " + modelJson.Count); // 카운트 체크 

            // 사이좋게 넣어줍니다
            foreach (string key in modelJson.Keys)
            {
                DictProjectModel.Add(key, modelJson[key]);
                DictProjectModelDebug.Add(key, JsonMapper.ToStringUnicode(modelJson[key]));
            }
        }



        /// <summary>
        /// 말풍선 세트 정보 설정 
        /// </summary>
        void SetBubbles()
        {

            bubbleSetJson = UserManager.main.GetNodeBubbleSet(); // 말풍선 세트만 따로 받아온다. 
            bubbleSpriteJson = UserManager.main.GetNodeBubbleSprite(); // 말풍선 스프라이트 정보 

            // 템플릿 6종!
            talkBubbleJson = GetJsonData(bubbleSetJson, BUBBLE_TALK); // 대화 
            whisperBubbleJson = GetJsonData(bubbleSetJson, BUBBLE_WHISPER); // 속삭임
            feelingBubbleJson = GetJsonData(bubbleSetJson, BUBBLE_FEELING); // 속마음 
            yellBubbleJson = GetJsonData(bubbleSetJson, BUBBLE_YELL); // 외침
            monologueBubbleJson = GetJsonData(bubbleSetJson, BUBBLE_MONOLOGUE); // 독백
            speechBubbleJson = GetJsonData(bubbleSetJson, BUBBLE_SPEECH); // 중요대사


            CollectBubbleImages();
        }
        
        
        #region 작품 리소스 관리 일러스트, 미니컷, 라이브 일러스트, 라이브 오브제, 캐릭터 모델
        
        
        
        /// <summary>
        /// 갤러리 미니컷, 라이브 오브젝트의 ID 찾기 
        /// </summary>
        /// <param name="minicutName">ScriptRow의 scriptData로 들어간 값</param>
        /// <returns>해당 미니컷 id</returns>
        public string GetGalleryMinicutID(string __minicutName, string __minicutType)
        {
            Debug.Log(string.Format("GetGalleryMinicutID [{0}]/[{1}]", __minicutName, __minicutType));
            
            // 라이브 2D인지, 일반 이미지인지 분리해서 처리한다. 
            if(__minicutType == "live_object") {
                // dictionary 조회해서 처리 
                return DictProjectLiveObject.ContainsKey(__minicutName) ? DictProjectLiveObject[__minicutName][0]["live_object_id"].ToString() : string.Empty;
            }
            else {
                // 이미지 미니컷 데이터에서 검색.
                for(int i=0; i<minicutJSON.Count;i++) {
                    
                    if(SystemManager.GetJsonNodeString(minicutJSON[i], "image_name") == __minicutName)
                        return SystemManager.GetJsonNodeString(minicutJSON[i], "minicut_id");
                }
                
                return string.Empty;
            }

        }
        
        
        /// <summary>
        /// 미니컷 이름으로 Json 찾기 
        /// </summary>
        /// <param name="__name"></param>
        /// <returns></returns>
        public JsonData GetPublicMinicutJsonByName(string __name) {
            
            for(int i=0;i<minicutJSON.Count;i++)
            {
                if(SystemManager.GetJsonNodeString(minicutJSON[i], "image_name") == __name)
                {
                    return minicutJSON[i];
                }
            }

            return null;
        }        
        
        /// <summary>
        /// 일러스트 이름으로 일러스트 기준정보 찾기
        /// </summary>
        /// <param name="__illustName">일러스트 명칭</param>
        /// <returns>일러스트 id와 type을 담은 JsonData</returns>
        public JsonData GetImageIllustData(string __illustName)
        {
            // * 2021.12.23 이 메소드는 일반 일러스트만을 대상으로 합니다. 
            // illust_id, image_name, image_url,key ,is_public, appear_episode, public_name, live_pair_id..
            for(int i=0; i<illustJson.Count;i++) {
                if(SystemManager.GetJsonNodeString(illustJson[i], "image_name") == __illustName) {
                    return illustJson[i];
                }
            }
            
            return null;
            
            /*
            for(int i=0;i<GetNodeUserIllust().Count;i++)
            {
                // 일러스트 명칭과 동일한 jsonData값을 찾아 id와 type값을 넣어서 return 해준다
                // 4개 종류가 통합되었기 때문에 is_minicut 값도 체크한다.
                if(GetNodeUserIllust()[i][LobbyConst.ILLUST_NAME].ToString().Equals(__illustName) 
                    && !SystemManager.GetJsonNodeBool(GetNodeUserIllust()[i], CommonConst.COL_IS_MINICUT))
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
            */
        }        
        
        /// <summary>
        /// 프로젝트 모델 정보 불러오기 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeProjectModels()
        {
            return ProjectDetailJson[NODE_PROJECT_MODELS];
        }
        
        /// <summary>
        /// 프로젝트 이미지 일러스트 정보 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeProjectIllusts()
        {
            return ProjectDetailJson[NODE_PROJECT_ILLUSTS];
        }
        
        /// <summary>
        /// 프로젝트 이미지 미니컷 정보 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeProjectMinicuts() {
            return ProjectDetailJson[NODE_PROJECT_MINICUTS];
        }


        /// <summary>
        /// 모델 이름으로 모델 리소스 정보 가져오기 
        /// </summary>
        /// <param name="__name"></param>
        /// <returns></returns>
        public JsonData GetModelJsonByModelName(string __name)
        {
            if (!DictProjectModel.ContainsKey(__name))
                return null;

            return DictProjectModel[__name];
        }

        /// <summary>
        /// 이름으로 라이브 일러스트 정보 가져오기 
        /// </summary>
        /// <param name="__name"></param>
        /// <returns></returns>
        public JsonData GetLiveIllustJsonByName(string __name)
        {
            if (!DictProjectLiveIllust.ContainsKey(__name))
                return null;

            return DictProjectLiveIllust[__name];
        }


        /// <summary>
        /// 이름으로 라이브 오브젝트 정보 가져오기 
        /// </summary>
        /// <param name="__name"></param>
        /// <returns></returns>
        public JsonData GetLiveObjectJsonByName(string __name)
        {
            if (!DictProjectLiveObject.ContainsKey(__name))
                return null;

            return DictProjectLiveObject[__name];
        }        
        
        #endregion
        
        
        



        /// <summary>
        /// 버블 이미지 수집하기 
        /// </summary>
        void CollectBubbleImages()
        {
            bubbleID_Dictionary = new Dictionary<string, string>();
            bubbleURL_Dictionary = new Dictionary<string, string>();

            // 각 말풍선 그룹별로 처리 한다.
            CollectGroupImageInfo(GetJsonData(talkBubbleJson, BUBBLE_VARIATION_NORMAL));
            CollectGroupImageInfo(GetJsonData(talkBubbleJson, BUBBLE_VARIATION_EMOTICON));
            CollectGroupImageInfo(GetJsonData(talkBubbleJson, BUBBLE_VARIATION_REVERSE_EMOTICON));
            CollectGroupImageInfo(GetJsonData(talkBubbleJson, BUBBLE_VARIATION_DOUBLE));

            CollectGroupImageInfo(GetJsonData(whisperBubbleJson, BUBBLE_VARIATION_NORMAL));
            CollectGroupImageInfo(GetJsonData(whisperBubbleJson, BUBBLE_VARIATION_EMOTICON));
            CollectGroupImageInfo(GetJsonData(whisperBubbleJson, BUBBLE_VARIATION_REVERSE_EMOTICON));
            CollectGroupImageInfo(GetJsonData(whisperBubbleJson, BUBBLE_VARIATION_DOUBLE));

            CollectGroupImageInfo(GetJsonData(feelingBubbleJson, BUBBLE_VARIATION_NORMAL));
            CollectGroupImageInfo(GetJsonData(feelingBubbleJson, BUBBLE_VARIATION_EMOTICON));
            CollectGroupImageInfo(GetJsonData(feelingBubbleJson, BUBBLE_VARIATION_REVERSE_EMOTICON));
            CollectGroupImageInfo(GetJsonData(feelingBubbleJson, BUBBLE_VARIATION_DOUBLE));

            CollectGroupImageInfo(GetJsonData(yellBubbleJson, BUBBLE_VARIATION_NORMAL));
            CollectGroupImageInfo(GetJsonData(yellBubbleJson, BUBBLE_VARIATION_EMOTICON));
            CollectGroupImageInfo(GetJsonData(yellBubbleJson, BUBBLE_VARIATION_REVERSE_EMOTICON));
            CollectGroupImageInfo(GetJsonData(yellBubbleJson, BUBBLE_VARIATION_DOUBLE));


            // 추가된 2종의 템플릿(독백과 중요대사)
            CollectGroupImageInfo(GetJsonData(monologueBubbleJson, BUBBLE_VARIATION_NORMAL));
            CollectGroupImageInfo(GetJsonData(monologueBubbleJson, BUBBLE_VARIATION_EMOTICON));
            CollectGroupImageInfo(GetJsonData(monologueBubbleJson, BUBBLE_VARIATION_REVERSE_EMOTICON));
            CollectGroupImageInfo(GetJsonData(monologueBubbleJson, BUBBLE_VARIATION_DOUBLE));

            CollectGroupImageInfo(GetJsonData(speechBubbleJson, BUBBLE_VARIATION_NORMAL));
            CollectGroupImageInfo(GetJsonData(speechBubbleJson, BUBBLE_VARIATION_EMOTICON));
            CollectGroupImageInfo(GetJsonData(speechBubbleJson, BUBBLE_VARIATION_REVERSE_EMOTICON));
            CollectGroupImageInfo(GetJsonData(speechBubbleJson, BUBBLE_VARIATION_DOUBLE));

        }

        void CollectGroupImageInfo(JsonData __j) {

            // 없는 경우에 return 처리 추가 
            if (__j == null)
                return;


            string currentID = string.Empty;
            string currentURL = string.Empty;
            string currentKEY = string.Empty;
            

            // 루프 돌면서 중복이 되지않게 url 과 key를 수집한다. 
            // bubbleDictionary에 url - key 조합으로 저장한다. 
            // bubbleSetJSON 이 아니라 .. 
            for(int i=0; i<__j.Count;i++)
            {

                // 말풍선 스프라이트 
                currentID = GetNodeValue(__j[i], GameConst.COL_BUBBLE_SPRITE_ID);
                if(currentID != "-1") {
                    currentURL = GetNodeValue(__j[i], GameConst.COL_BUBBLE_SPRITE_URL);
                    currentKEY = GetNodeValue(__j[i], GameConst.COL_BUBBLE_SPRITE_KEY);
                    AddBubbleDictionary(currentID, currentURL, currentKEY);
                }

                // 말풍선 외곽선 스프라이트 
                currentID = GetNodeValue(__j[i], GameConst.COL_BUBBLE_OUTLINE_ID);
                if(currentID != "-1") {
                    currentURL = GetNodeValue(__j[i], GameConst.COL_BUBBLE_OUTLINE_URL);
                    currentKEY = GetNodeValue(__j[i], GameConst.COL_BUBBLE_OUTLINE_KEY);
                    AddBubbleDictionary(currentID, currentURL, currentKEY);
                }

                // 말꼬리 스프라이트
                currentID = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAIL_ID);
                if(currentID != "-1") {
                    currentURL = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAIL_URL);
                    currentKEY = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAIL_KEY);
                    AddBubbleDictionary(currentID, currentURL, currentKEY);
                }

                // 말꼬리 외곽선 
                currentID = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAIL_OUTLINE_ID);
                if (currentID != "-1")
                {
                    currentURL = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAIL_OUTLINE_URL);
                    currentKEY = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAIL_OUTLINE_KEY);
                    AddBubbleDictionary(currentID, currentURL, currentKEY);
                }



                // 반전 말꼬리 스프라이트
                currentID = GetNodeValue(__j[i], GameConst.COL_BUBBLE_R_TAIL_ID);
                if(currentID != "-1") {
                    currentURL = GetNodeValue(__j[i], GameConst.COL_BUBBLE_R_TAIL_URL);
                    currentKEY = GetNodeValue(__j[i], GameConst.COL_BUBBLE_R_TAIL_KEY);
                    AddBubbleDictionary(currentID, currentURL, currentKEY);
                }

                // 반전 말꼬리 외곽선 
                currentID = GetNodeValue(__j[i], GameConst.COL_BUBBLE_R_TAIL_OUTLINE_ID);
                if (currentID != "-1")
                {
                    currentURL = GetNodeValue(__j[i], GameConst.COL_BUBBLE_R_TAIL_OUTLINE_URL);
                    currentKEY = GetNodeValue(__j[i], GameConst.COL_BUBBLE_R_TAIL_OUTLINE_KEY);
                    AddBubbleDictionary(currentID, currentURL, currentKEY);
                }

                // 네임태그
                currentID = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAG_ID);
                if (currentID != "-1")
                {
                    currentURL = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAG_URL);
                    currentKEY = GetNodeValue(__j[i], GameConst.COL_BUBBLE_TAG_KEY);
                    AddBubbleDictionary(currentID, currentURL, currentKEY);
                }
            }
        }


        /// <summary>
        /// 버블 딕셔너리에 추가 
        /// </summary>
        /// <param name="__id"></param>
        /// <param name="__url"></param>
        /// <param name="__key"></param>
        void AddBubbleDictionary(string __id, string __url, string __key)
        {
            // _id가 음수이거나 url이나 key 값이 없으면 뺀다.
            try
            {
                if (int.Parse(__id) < 0 || string.IsNullOrEmpty(__url) || string.IsNullOrEmpty(__key))
                    return;
            }
            catch
            {
                Debug.Log("AddBubbleDictionary int parse exception");
                return;
            }

            // 필요한 컬럼이 3개라서 Dictionary를 두개 가져간다.
            if (bubbleID_Dictionary.ContainsKey(__id))
                return;

            bubbleID_Dictionary[__id] = __url;
            bubbleURL_Dictionary[__url] = __key;
        }


        /// <summary>
        /// 말풍선 그룹 정보 가져오기 
        /// </summary>
        /// <param name="__template">템플릿</param>
        /// <param name="__variation">배리에이션</param>
        /// <returns></returns>
        public JsonData GetBubbleGroupJSON(string __template, string __variation)
        {
            switch (__template)
            {
                case BUBBLE_TALK:
                case GameConst.TEMPLATE_PHONE_SELF:
                    return GetJsonData(talkBubbleJson, __variation);
                case BUBBLE_FEELING:
                case GameConst.TEMPLATE_PHONE_PARTNER:
                    return GetJsonData(feelingBubbleJson, __variation);
                case BUBBLE_WHISPER:
                    return GetJsonData(whisperBubbleJson, __variation);
                case BUBBLE_YELL:
                    return GetJsonData(yellBubbleJson, __variation);
                case BUBBLE_MONOLOGUE:
                    return GetJsonData(monologueBubbleJson, __variation);

                case BUBBLE_SPEECH:
                    return GetJsonData(speechBubbleJson, __variation);

                default:
                    return null;
            }
        }

        /// <summary>
        /// 노드가져오기 
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="__key"></param>
        /// <returns></returns>
        public JsonData GetJsonData(JsonData __j, string __key)
        {

            if (__j == null)
                return null;

            if (!__j.ContainsKey(__key))
                return null;

            return __j[__key];
        }


        /// <summary>
        /// Node 안의 value 값을 string으로 받기! 없으면 empty.
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="__col"></param>
        /// <returns></returns>
        public static string GetNodeValue(JsonData __j, string __col)
        {
            if (__j == null)
                return string.Empty;

            if (!__j.ContainsKey(__col))
                return string.Empty;
               

            if (__j[__col] == null)
                return string.Empty;

            return __j[__col].ToString();
        }


        /// <summary>
        /// 선택한 에피소드 정보 저장하기.
        /// </summary>
        /// <param name="__j"></param>
        public void SetCurrentEpisodeJson(EpisodeData __data)
        {
            CurrentEpisodeData = __data;
            CurrentEpisodeID = CurrentEpisodeData.episodeID;
            CurrentEpisodeTitle = CurrentEpisodeData.episodeTitle;

            // 진입한 에피소드가 사이드인 경우
            if(CurrentEpisodeData.episodeType == EpisodeType.Side)
                playSideEpisode = true;
            else 
                playSideEpisode = false;
            
        }



        public string GetStoryTitle()
        {
            return CurrentProject.title;
        }

        public string GetAuthor()
        {
            return CurrentProject.writer;
        }

        #region 의장 정보 컨트롤 

        /// <summary>
        /// 작품 의상 기준정보 데이터 
        /// </summary>
        /// <returns></returns>
        public JsonData GetNodeDressCode()
        {
            return dressCodeJson;
        }


        /// <summary>
        /// 화자+의상ID로 일치하는 모델 이름 찾기
        /// </summary>
        /// <param name="__speaker"></param>
        /// <param name="__dressID"></param>
        /// <returns></returns>
        public string GetTargetDressModelNameByDressID(string __speaker, string __dressID) 
        {
            if (dressCodeJson == null)
                return null;

            for(int i=0; i<dressCodeJson.Count;i++)
            {
                // 일치하는 모델 찾음!
                if (dressCodeJson[i][GameConst.COL_DRESSMODEL_NAME].ToString() == __speaker
                        && dressCodeJson[i][GameConst.COL_DRESS_ID].ToString() == __dressID)
                {
                    return dressCodeJson[i][GameConst.COL_MODEL_NAME].ToString();
                }
            }

            return null;
        }


        /// <summary>
        /// 화자+의상이름으로 일치하는 모델 이름 찾기
        /// </summary>
        /// <param name="__speaker"></param>
        /// <param name="__dressName"></param>
        /// <returns></returns>
        public string GetTargetDressModelNameByDressName(string __speaker, string __dressName)
        {
            if (dressCodeJson == null)
                return null;

            for (int i = 0; i < dressCodeJson.Count; i++)
            {
                // 일치하는 모델 찾음!
                if (dressCodeJson[i][GameConst.COL_DRESSMODEL_NAME].ToString() == __speaker
                        && dressCodeJson[i][GameConst.COL_DRESS_NAME].ToString() == __dressName)
                {
                    return dressCodeJson[i][GameConst.COL_MODEL_NAME].ToString();
                }
            }

            return null;
        }
        /// <summary>
        /// 해당 대상코드 노드를 찾습니다. (의상정보)
        /// </summary>
        /// <param name="__speaker"></param>
        /// <param name="__dressName"></param>
        /// <returns></returns>
        public JsonData GetTargetDressCodeNodeByDressName(string __speaker, string __dressName)
        {
            if (dressCodeJson == null)
                return null;

            for (int i = 0; i < dressCodeJson.Count; i++)
            {
                // 일치하는 모델 찾음!
                if (dressCodeJson[i][GameConst.COL_DRESSMODEL_NAME].ToString() == __speaker
                        && dressCodeJson[i][GameConst.COL_DRESS_NAME].ToString() == __dressName)
                {
                    return dressCodeJson[i];
                }
            }

            return null;
        }

        #endregion

        #region 말풍선 로컬 정보 

        /// <summary>
        /// 로컬에 저장된 프로젝트의 말풍선 세트 ID를 불러온다. 
        /// </summary>
        /// <param name="__projectID"></param>
        void LoadProjectBubbleSetID(string __projectID)
        {
            // 서버에서는 전송받은 말풍선 세트 ID와 말풍선 세트 버전을 통해서
            // 신규로 말풍선 세트 정보를 내려줄지 말지를 결정한다. 

            string key = KEY_PROJECT_BUBBLE_SET_ID + __projectID;

            if (ES3.KeyExists(key))
                currentBubbleSetID = ES3.Load<string>(key);
            else
                currentBubbleSetID = "0";
        }
        
        /// <summary>
        /// 말풍선 세트 로컬 정보 조회 
        /// </summary>
        /// <param name="__bubbleID">말풍선 세트 ID</param>
        void LoadBubbleSetLocalInfo(string __bubbleID)
        {
            // ID를 연결해서 키를 완성한다. 
            string bubbleDetail = string.Empty;
            currentBubbleSetVersion = 0;

            string detail_key = KEY_BUBBLE_DETAIL_PREFIX + __bubbleID;
            string version_key = KEY_BUBBLE_VER_PREFIX + __bubbleID;

            // 체크.. 
            Debug.Log(string.Format("LoadBubbleSetLocalInfo, [{0}]/[{1}]", detail_key, version_key));


            if (ES3.KeyExists(detail_key))
                bubbleDetail = ES3.Load<string>(detail_key);
            else
                bubbleDetail = string.Empty;

            if (ES3.KeyExists(version_key))
                currentBubbleSetVersion = ES3.Load<int>(version_key); // 이 정보는 작품 상세정보를 요청할때 함께 전달합니다. 
            else
                currentBubbleSetVersion = 0;

            // 데이터 없으면 version 기본값 0으로처리 
            if (string.IsNullOrEmpty(bubbleDetail))
                currentBubbleSetVersion = 0;

            try
            {
                currentBubbleSetJson = JsonMapper.ToObject(bubbleDetail);
            }
            catch(System.Exception e)
            {
                Debug.Log(e.StackTrace);
                currentBubbleSetVersion = 0; // 읽다가 오류나도 0. 
            }

        }

        /// <summary>
        /// 말풍선 세트 정보 로컬에 저장하기!
        /// </summary>
        /// <param name="__bubbleID"></param>
        void SaveBubbleSetLocalInfo(string __bubbleID)
        {

            Debug.Log("SaveBubbleSetLocalInfo : " + __bubbleID);

            // 말풍선 세트별 상세 JSON 저장 
            ES3.Save<int>(KEY_BUBBLE_VER_PREFIX + __bubbleID, currentBubbleSetVersion);
            ES3.Save<string>(KEY_BUBBLE_DETAIL_PREFIX + __bubbleID, JsonMapper.ToStringUnicode(currentBubbleSetJson));

            // 프로젝트와 연결된 말풍선 세트 ID도 함께 저장해둔다. 
            ES3.Save<string>(KEY_PROJECT_BUBBLE_SET_ID + CurrentProjectID, __bubbleID);
        }

        #endregion

        /// <summary>
        /// 라이브 일러스트 있는지 체크하기
        /// </summary>
        /// <param name="__name"></param>
        /// <returns></returns>
        public bool CheckExistsLiveIllust(string __name)
        {
            return DictProjectLiveIllust.ContainsKey(__name);
        }

        /// <summary>
        /// 작품 title 반환하기
        /// </summary>
        /// <param name="projectId">작품 ID</param>
        /// <returns>파라미터로 받은 작품ID의 작품 제목 반환</returns>
        public string GetStoryTitle(string projectId)
        {
            
            if(listTotalStory.Count == 0)
                return null;
            

            for(int i=0;i<listTotalStory.Count;i++)
            {
                if (listTotalStory[i].projectID == projectId)
                    return listTotalStory[i].title;
            }

            return null;
        }
        
        #region 작품 상세정보 
        
        
        /// <summary>
        /// 작품 상세정보 컬럼 value 얻기 
        /// </summary>
        /// <param name="__col">title, summary, writer 등</param>
        /// <returns></returns>
        public string GetStoryDetailInfo(string __col) {
            
            if(storyDetailJson == null)
                return string.Empty;
            
            return SystemManager.GetJsonNodeString(storyDetailJson, __col);
        }
        
 
        

        /// <summary>
        /// 일러스트 이름으로 JSON Node 찾기 
        /// </summary>
        /// <param name="__illustName"></param>
        /// <returns></returns>
        public JsonData GetIllustJsonByIllustName(string __illustName) {
           if(illustJson == null) 
                return null;
                
            for(int i=0; i<illustJson.Count;i++) {
                if(illustJson[i]["image_name"].ToString() == __illustName) {
                    return illustJson[i];
                }
            }
            
            return null;
        }
        
        
        
        
        /// <summary>
        /// 이동 컬럼(#)을 통해 다음 에피소드를 찾아가는 경우 사용 
        /// </summary>
        /// <param name="targetID"></param>
        /// <returns></returns>
        public static EpisodeData GetNextFollowingEpisodeData(string targetID) {
            
            for(int i=0; i<main.ListCurrentProjectEpisodes.Count; i++) {
                if(main.ListCurrentProjectEpisodes[i].episodeID == targetID)
                    return main.ListCurrentProjectEpisodes[i];
            }
            
            return null;
        }
        
        /// <summary>
        /// 정규 에피소드의 다음 순서 에피소드 찾기 
        /// </summary>
        /// <param name="currentEpisodeData"></param>
        /// <returns></returns>
        public static EpisodeData GetNextRegularEpisodeData(EpisodeData currentEpisodeData) {
            
            bool isFound = false;
            
            if(currentEpisodeData.episodeType == EpisodeType.Ending || currentEpisodeData.episodeType == EpisodeType.Side)
                return null;
                
            for(int i=0; i<main.RegularEpisodeList.Count;i++) {
                
                // 찾았으면 다음행이 다음 순번이다.
                if(isFound)
                    return main.RegularEpisodeList[i];
                
                if(main.RegularEpisodeList[i] == currentEpisodeData) {
                    isFound = true;
                }
            } // 없으면 null;
            
            return null;
        }
        
        
        #endregion
        
        #region 작품 리소스 몰래 내려받기 
        
        
        /// <summary>
        /// 프로젝트의 모든 이모티콘 조회 
        /// </summary>
        public void RequestProjectAllEmoticon() {
            emoticonJson = null;
            DictDownloadedEmoticons.Clear();
            CountDownloadingEmoticon = 0;
            isDownloadEmoticonsSaved = false;
            
            JsonData sending = new JsonData();
            sending["func"] = NetworkLoader.FUNC_PROJECT_ALL_EMOTICONS;
            sending["project_id"] = CurrentProjectID;
            
            NetworkLoader.main.SendPost(OnReceivedProjectAllEmoticon, sending);
        }
        
        void OnReceivedProjectAllEmoticon(HTTPRequest request, HTTPResponse response) {
            
            if (!NetworkLoader.CheckResponseValidation(request, response))
            {
                Debug.LogError("Network Error in OnReceivedProjectAllEmoticon");
                return;
            }
            
            Debug.Log(response.DataAsText);

           // 조회하고 와서 json으로 변환 
            emoticonJson = JsonMapper.ToObject(response.DataAsText);
            // Debug.Log(string.Format("TOTAL EMOTICON COUNT [{0}] ", emoticonJson.Count));
            
            DownloadProjectEmoticonBackground();
        }
        
        /// <summary>
        /// 다운로드 프로젝트 이모티콘! 몰래몰래!
        /// 
        /// </summary>
        public void DownloadProjectEmoticonBackground() {
            
            string image_url = string.Empty;
            string image_key = string.Empty;
            
            if(emoticonJson == null)
                return;
                
            Debug.Log(string.Format("DownloadProjectEmoticonBackground [{0}] images", emoticonJson.Count));                
                
            // 곧바로 다운로드 시작합니다. 
            for(int i=0; i<emoticonJson.Count; i++) {
                
                image_url = emoticonJson[i]["image_url"].ToString();
                image_key = emoticonJson[i]["image_key"].ToString();
                
                // 있으면 continue
                if(ES3.FileExists(image_key))
                    continue;
                    
                
                // 없으면 개별 다운로드 시작 
                var req = new HTTPRequest(new System.Uri(image_url), OnEmoticonDownloaded);
                req.Tag = image_key;
                req.Send();
                
                CountDownloadingEmoticon++; // 카운트 증가
            }
        }
        
        void OnEmoticonDownloaded(HTTPRequest request, HTTPResponse response) {
            
            // 다운로드에 실패해도 카운트는 차감한다.
            // 어차피 에피소드 실행할때 또 다운로드를 시도한다.
            
            CountDownloadingEmoticon--;
            
            if (request.State != HTTPRequestStates.Finished)
            {
                return;
            }
            
            // 다운로드 성공했으면, 로컬에 저장은 하지말고 모아놓는다. 
            if(DictDownloadedEmoticons.ContainsKey(request.Tag.ToString()))
                return;
            
            // 추가 
            DictDownloadedEmoticons.Add(request.Tag.ToString(), response.Data); 
            
            if(CountDownloadingEmoticon <= 0)
                SaveDownloadedEmoticons();
            
        }
        
        /// <summary>
        /// 다운받은 이모티콘 byte 저장. 
        /// </summary>
        public void SaveDownloadedEmoticons() {
            
            if(isDownloadEmoticonsSaved)
                return;
            
            
            Debug.Log(">> SaveDownloadedEmoticons Starts!");
            
            foreach(string key in DictDownloadedEmoticons.Keys) {
                ES3.SaveRaw(DictDownloadedEmoticons[key], key); // SaveRaw로 저장한다.
                
                // 어차피 ES3에서 LoadImage는 Raw를 불러와서 texture로 변환하기 때문에 
                // Raw로 저장해!
            }
            
            DictDownloadedEmoticons.Clear(); // 다했으면 클리어!
            isDownloadEmoticonsSaved = true;
        }
        
        /// <summary>
        /// 해당 작품 모든 로딩 화면 다운로드
        /// </summary>
        public void DownloadProjectAllEpisodeLoading()
        {
            loadingJson = SystemManager.GetJsonNode(ProjectDetailJson, "episodeLoadingList");
            DictDownloadedLoadings.Clear();
            countDownloadingLoading = 0;
            isDownloadLoadingsSaved = false;

            string imageUrl = string.Empty;
            string imageKey = string.Empty;

            for (int i = 0; i < loadingJson.Count; i++)
            {
                imageUrl = SystemManager.GetJsonNodeString(loadingJson[i], "image_url");
                imageKey = SystemManager.GetJsonNodeString(loadingJson[i], "image_key");

                // 있으면 continue
                if (ES3.FileExists(imageKey))
                    continue;

                // 없으면 개별 다운로드 시작 
                var req = new HTTPRequest(new System.Uri(imageUrl), OnLoadingDownloaded);
                req.Tag = imageKey;
                req.Send();

                countDownloadingLoading++; // 카운트 증가
            }
        }

        void OnLoadingDownloaded(HTTPRequest request, HTTPResponse response)
        {
            // 다운로드에 실패해도 카운트는 차감한다.
            countDownloadingLoading--;

            if (request.State != HTTPRequestStates.Finished)
                return;

            // 다운로드 성공했으면, 로컬에 저장은 하지말고 모아놓는다. 
            if (DictDownloadedLoadings.ContainsKey(request.Tag.ToString()))
                return;

            // 추가 
            DictDownloadedLoadings.Add(request.Tag.ToString(), response.Data);

            if (countDownloadingLoading <= 0)
                SaveDownloadedLoadings();
        }

        void SaveDownloadedLoadings()
        {
            if (isDownloadLoadingsSaved)
                return;

            Debug.Log(">> SaveDownloadedLoadings Starts!");

            foreach (string key in DictDownloadedLoadings.Keys)
                ES3.SaveRaw(DictDownloadedLoadings[key], key);

            DictDownloadedLoadings.Clear();
            isDownloadLoadingsSaved = true;
        }
        
        
        #endregion
    
        #region 작품 재화(화폐) 기준정보 관련 
        
        
        /// <summary>
        /// 작품에서 사용하는 화폐코드를 가져온다.
        /// </summary>
        /// <param name="__type"></param>
        /// <returns></returns>
        public string GetProjectCurrencyCode(CurrencyType __type) {
            
            string dataType = string.Empty;
            
            switch(__type) {
                case CurrencyType.Freepass:
                dataType = "nonconsumable";
                break;
                
                case CurrencyType.OneTime:
                dataType = "consumable";
                break;
                
                case CurrencyType.Rent:
                dataType = "ticket";
                break;
                
                default:
                break;
                
            }
            
            if(string.IsNullOrEmpty(dataType))
                return null;
                
            // loop.
            for(int i=0;i<storyCurrencyJSON.Count;i++) {
                if(storyCurrencyJSON[i]["currency_type"].ToString() == dataType)
                    return storyCurrencyJSON[i]["currency"].ToString();
            }
                
            return null;
            
        }
        
        #endregion
        
        #region 갤러리 리소스 
        

        
        /// <summary>
        /// 갤러리의 작품별 BGM 배너 주세요 
        /// </summary>
        /// <returns></returns>
        public Texture2D GetGalleryBgmBanner()  {
            if(string.IsNullOrEmpty(bgmBannerKey))
                return null;
            
            if(!ES3.FileExists(bgmBannerKey))
                return null;
                
            try {
                
                return SystemManager.GetLocalTexture2D(bgmBannerKey);
                
            }
            catch(System.Exception e) {
                Debug.Log(e.StackTrace);
                return null;
            }
        }
        
        
        #endregion
        
        #region 프리패스 친구들
        
        /// <summary>
        /// 프리패스 가격 노드 가져오기
        /// </summary>
        /// <returns></returns>
        public JsonData GetProjectFreepassNode() {
            if(!ProjectDetailJson.ContainsKey(NODE_FREEPASS_PRICE))
                return null;
            
            return ProjectDetailJson[NODE_FREEPASS_PRICE];
            
        }
    
        /// <summary>
        /// 프리패스 가격 가져오기 
        /// </summary>
        /// <param name="__node">NODE_FREEPASS_ORIGIN_PRICE, NODE_FREEPASS_SALE_PRICE </param>
        /// <returns></returns>
        public int GetProjectFreepassPrice(bool __isOrigin) {
            
            string value = string.Empty;
            
            if(__isOrigin)
                value = SystemManager.GetJsonNodeString(ProjectDetailJson[NODE_FREEPASS_PRICE], NODE_FREEPASS_ORIGIN_PRICE);
            else 
                value = SystemManager.GetJsonNodeString(ProjectDetailJson[NODE_FREEPASS_PRICE], NODE_FREEPASS_SALE_PRICE);
            
                
            int price = 0;
            
            if(int.TryParse(value, out price))
                return price;
            else 
                return -1;
            
        }
        
        /// <summary>
        /// 작품별 프리패스 타임딜 상품리스트 가져오기
        /// </summary>
        /// <returns></returns>
        public JsonData GetProjectFreepassProduct() {
            if(!ProjectDetailJson.ContainsKey(NODE_FREEPASS_PRODUCT))
                return null;
            
            return ProjectDetailJson[NODE_FREEPASS_PRODUCT];
        }
        
        /// <summary>
        /// 특정 프리패스 상품 정보 가져오기 
        /// </summary>
        /// <param name="__no"></param>
        /// <returns></returns>
        public JsonData GetFreepassProductByNO(string __no) {
            if(GetProjectFreepassProduct() == null)            
                return null;
                
            for(int i=0; i<GetProjectFreepassProduct().Count;i++) {
                
                if(GetProjectFreepassProduct()[i]["freepass_no"].ToString() == __no) 
                    return GetProjectFreepassProduct()[i]; // 찾았다!
                
            }
            
            return null;
        }
        
        
        
        #endregion
   
        
    }
    
}