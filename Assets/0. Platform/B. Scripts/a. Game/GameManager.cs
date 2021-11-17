using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;

namespace PIERStory
{
    public class GameManager : MonoBehaviour
    {
        /*
         * 현재 아직 미생성한 클래스들에 대해서 변수들을 주석해놨음
         */

        public static GameManager main = null;      // singleton
        ///public static List<GameSelectionCtrl> ListAppearSelection = new List<GameSelectionCtrl>();

        // 이어하기 변수
        public static bool isResumePlay = false;        // 이어하기를 통한 진입인가?
        public static string lastPlaySceneId = string.Empty;
        public static long lastPlayScriptNo = 0;
        public static bool hasLastPlayScriptNo = false;     // 이어하기 타겟 script_no가 대본상에 존재하는지 체크

        [HideInInspector] public JsonData currentEpisodeJson = null;    // 선택한 에피소드 정보

        JsonData episodeElement = null;     // 에피소드 구성 및 리소스(script, background, image, illust, emoticon)
        JsonData scriptJson = null;         // 스크립트 JsonData
        [HideInInspector] public JsonData loadingJson = null;
        [HideInInspector] public JsonData loadingDetailJson = null;

        public bool isPlaying = false;
        public bool isScriptFetch = false;      // 스크립트 정보를 가져왔는지 확인

        public GameObject prefabScriptPage;     // prefab

        [Space]
        //public ScriptPage currentPage;  // 생성된 현재 페이지

        public bool isThreadHold = false;           // 플레이 스레드 대기 여부. 보통 연출 대기에 쓰임
        public bool isWaitingScreenTouch = false;   // 게임 플레이 도중 스크린 터치 기다림
        public bool useSkip = false;                // 스킵을 사용했는가?
        public bool skipable = false;               // 스킵 기능 사용이 가능한가?
        //public ViewInGameMenu inGameMenu;
        public string currentSceneId = string.Empty;    // 현재 sceneId(사건ID)
        public List<GameModelCtrl> ListMovingModels = new List<GameModelCtrl>();

        public int episodeDownloadableResourceCount = 0;


        [Space][Header("인게임 배경")]
        public GameSpriteCtrl currentBG; // 현재 보여주고 있는 배경 
        public ScriptImageMount currentBackgroundMount = null;
        public int indexPoolBG = 0;
        public List<GameSpriteCtrl> PoolSpriteBG;
        public GameSpriteCtrl defaultBG; // 디폴트 BG 

        [Header("일러스트, 미니컷(라이브오브제)")]
        public GameSpriteCtrl currentIllust;                  // 현재 보여주고 있는 일반 일러스트 
        public ScriptLiveMount currentLiveObj;                // 현재 보여주고 있는 live2D 오브제
        public ScriptLiveMount currentLiveIllust;             // 현재 보여주고 있는 live2D 일러스트
        public ScriptImageMount currentMinicutMount = null;   // 현재 보여주고있는 미니컷 마운트

        public Image currentMinicut = null; // 현재 이미지.
        //public GameSpriteCtrl defaultImage; // 디폴트 이미지
        public Transform illustPillar;        // live일러스트, liveObject 부모 transform
        public RawImage liveObjectTexture;


        [Space][Header("스탠딩 모델")]
        public ScriptModelMount standingSpeaker = null;
        public GameModelCtrl[] characterModels = new GameModelCtrl[3];  // 0 : 좌, 1:중앙, 2:우
        public Sprite defaultCharacterSprite;
        public GameObject infoPrefab;           // 더미 캐릭터 정보 담은 prefab
        public Transform modelPillar;           // 캐릭터 모델의 부모 transform.

        [Header("말풍선")]
        public List<GameBubbleCtrl> ListBubbles;  // 말풍선들!
        public GameBubbleCtrl partnerBubble;      // 발신자 말풍선(전화용)
        public GameBubbleCtrl selfBubble;         // 수신자 말풍선(전화용)
        int bubbleIndex = 0; // 말풍선 pooling 인덱스

        [Space]
        public GameNarrationCtrl gameNarrationSingle; // 나레이션 

        [Space][Space][Header("**선택지**")]
        public List<ScriptRow> ListSelectionRows = new List<ScriptRow>();                 // 수집된 행들 
        public List<GameSelectionCtrl> ListGameSelection = new List<GameSelectionCtrl>(); // 선택지 UI
        public Image SelectionMain = null;                      // 선택지 메인 오브젝트
        public bool isSelectionInputWait = false;               // 선택지 입력 기다리기!
        public string targetSelectionSceneID = string.Empty;    // 선택지 선택 후 이동할 사건ID
        public GameObject screenInputBlocker = null;            // 연출중 입력막고싶다..!

        [Space][Space][Header("리소스 접근을 위한 Dictionary")]
        public Dictionary<string, GameSpriteCtrl> DictIllusts = new Dictionary<string, GameSpriteCtrl>();                     // 일러스트 Dictionary
        public Dictionary<string, ScriptLiveMount> DictLiveIllusts = new Dictionary<string, ScriptLiveMount>();               // Live illust Dictionary
        public Dictionary<string, ScriptLiveMount> DictLiveObjs = new Dictionary<string, ScriptLiveMount>();                  // Live Object Dictionary

        // 미니컷 이미지는 해상도 대응 때문에 UI Image로 변경한다.
        public Dictionary<string, ScriptImageMount> DictMinicutImages = new Dictionary<string, ScriptImageMount>();     // 이미지(미니컷) 딕셔너리
        public Dictionary<string, ScriptImageMount> DictBackgroundMounts = new Dictionary<string, ScriptImageMount>();  // 배경 딕셔너리

        public Sprite defaultMinicutSprite;         // 미니컷 리소스 없는 경우를 위한 defaultSprite

        [Space]
        public Dictionary<string, string> DictModelByDress = new Dictionary<string, string>();
        public Dictionary<string, ScriptModelMount> DictModels = new Dictionary<string, ScriptModelMount>();      // 캐릭터 모델 Dictionary
        public Dictionary<string, ScriptImageMount> DictEmoticon = new Dictionary<string, ScriptImageMount>();    // 이모티콘 Dictonary


        public GameSoundCtrl[] SoundGroup;  // 0 : BGM, 1 : Voice, 2 : Sound effect



        public List<GameSpriteCtrl> PoolIllust;

        public List<string> ListLoadingImageKey = new List<string>(); // 하나의 에피소드에 로딩되는 이미지 리스트. (여러번 통신을 막기위해 사용)
        public List<string> ListLoadingModel = new List<string>(); // 하나의 에피소드에 로딩되는 캐릭터 리스트 

        int targetRow = -1;
        // 인덱스들 
        public int indexPoolIllust = 0;

        [Space]
        public NetworkLoadingScreen gameNetworkLoadingScreen;

        [Space][Header("Sprite resources")]
        public Sprite spriteSelectionNormalBase = null;     // 일반 선택지 
        public Sprite spriteSelectionLockedBase = null;     // 선택지 잠금 상태
        public Sprite spriteSelectionUnlockedBase = null;   // 선택지 활성 상태 스프라이트 
        public Sprite spriteSelectionLockIcon = null;
        public Sprite spriteSelectionUnlockIcon = null;
        public Sprite spriteIllustPopup = null;             // 일러스트 획득 팝업 아이콘

        #region Awake, Start, static Initialize

        /// <summary>
        /// 신규 게임 처리
        /// </summary>
        public static void SetNewGame()
        {
            Debug.Log("New Game!");

            isResumePlay = false;
            lastPlaySceneId = string.Empty;
            lastPlayScriptNo = 0;
        }

        /// <summary>
        /// 이어하기 게임 설정
        /// </summary>
        /// <param name="__sceneId">마지막 플레이 sceneID(=사건ID)</param>
        /// <param name="__scriptNo">마지막 플레이 스크립트 번호(DB에 등록된 script_no)</param>
        public static void SetResumeGame(string __sceneId, long __scriptNo)
        {
            Debug.Log(string.Format("Resume Game! {0} / {1}", __sceneId, __scriptNo));

            isResumePlay = true;
            lastPlaySceneId = __sceneId;
            lastPlayScriptNo = __scriptNo;

            hasLastPlayScriptNo = false;
        }

        private void Awake()
        {
            main = this;
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            Debug.Log(">>>>> GameManager Start <<<<<");

            GarbageCollect();

            isPlaying = false;
            isScriptFetch = false;

            // 유저 정보 로그인이 되었는지 체크
            yield return new WaitUntil(() => UserManager.main != null && UserManager.main.GetUserAccountJSON() != null);
            Debug.Log("GameManager account check done");

            // 로비에서 에피소드 선택해서 게임씬에 진입한 경우
            if(SystemManager.main.givenEpisodeData != null)
            {

            }
            else
            {
                // 단독 실행, 없을 떄는 지정받은 에피소드 ID를 통해 진행
                Debug.Log("GameManager RequestStoryInfoIndependently");

                // 프로젝트 정보를 가져오고, 설정한 에피소드 데이터를 사용한다.
                StoryManager.main.RequestStoryInfo(StoryManager.main.CurrentProjectID, null);
            }

            SoundSetting(GameConst.BGM_MUTE, 0);
            SoundSetting(GameConst.VOICE_MUTE, 1);
            SoundSetting(GameConst.SOUNDEFFECT_MUTE, 2);
        }

        static void GarbageCollect()
        {
            Debug.Log("GarbageCollect Called");

            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// sound 관련 세팅
        /// </summary>
        void SoundSetting(string key, int soundIndex)
        {
            if (!PlayerPrefs.HasKey(key))
                PlayerPrefs.SetInt(key, 0);
            else
            {
                //if (PlayerPrefs.GetInt(key) < 1)
                //    SoundGroup[soundIndex].UnmuteAudioClip();
                //else
                //    SoundGroup[soundIndex].MuteAudioClip();
            }
        }

        public void RequestEpisodeScript()
        {
            Debug.Log("<color=cyan>RequestEpisodeScript</color>");

            // 통신 
            JsonData j = new JsonData();
            j[CommonConst.COL_EPISODE_ID] = StoryManager.main.CurrentEpisodeID;
            j[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            j[CommonConst.FUNC] = GameConst.FUNC_GET_EPISODE_SCRIPT;

            //NetworkLoader.main.SendPost(OnFetchEpisodeScript, j);
            NetworkLoader.main.RequestEpisodeGameData(j, OnFetchEpisodeScript);


            // 로딩창을 띄우고, 통신을 시작한다.
            //Doozy.Engine.GameEventMessage.SendEvent("EventGameLoading");
        }

        /// <summary>
        /// 에피소드의 스크립트 정보 통신 콜백
        /// </summary>
        void OnFetchEpisodeScript(JsonData __j)
        {
            // 초기화는 여기서 일임하지만, 다음 View를 띄우는 일은 GameLoading 에서 이벤트를 띄운다.
            loadingJson = SystemManager.GetJsonNode(__j, GameConst.NODE_LOADING);
            loadingDetailJson = SystemManager.GetJsonNode(__j, GameConst.NODE_LOADING_DETAIL);
            InitGamePage(__j);
        }

        /// <summary>
        /// 에피소드 선택(리스트에서..)
        /// </summary>
        public void SelectEpisode(JsonData __j)
        {
            currentEpisodeJson = __j;

            // 스토리 매니저에게 현재 실행하는 에피소드 ID를 전달한다. 
            StoryManager.main.SetCurrentEpisodeJson(currentEpisodeJson);

            Debug.Log(string.Format("SelectEpisode [{0}]", StoryManager.main.CurrentEpisodeID));


            // 새로 진입할때, 대상 에피소드의 상황ID 클리어 진척도를 초기화한다.
            // 초기화 하고 callback 으로 RequestEpisodeScript 실행
            if (isResumePlay)
                RequestEpisodeScript();
            else
                UserManager.main.ClearSelectedEpisodeSceneProgress(StoryManager.main.CurrentProjectID, StoryManager.main.CurrentEpisodeID, RequestEpisodeScript);
        }

        /// <summary>
        /// 게임 페이지 초기화를 시작합니다. 
        /// Loading을 통해 전달 받는다. 
        /// </summary>
        /// <param name="__j"></param>
        public void InitGamePage(JsonData __j)
        {
            // 페이지 초기화전에 리소스 초기화 진행합니다. 
            InitGameResourceObjects();

            isScriptFetch = true; // 데이터 가져왔어요..!

            episodeElement = __j;
            scriptJson = __j[GameConst.NODE_SCRIPT];

            // 페이지를 생성하고 넘겨준다. 
            /*
            currentPage = Instantiate(prefabScriptPage, this.transform).GetComponent<ScriptPage>();
            currentPage.InitPage(episodeElement);

            episodeDownloadableResourceCount = currentPage.GetPrepCount();

            for (int i = 0; i < currentPage.ListRows.Count; i++)
            {
                // 최근 스크립트No가 0보다 크고, 해당 script_no를 찾았을 때
                if (lastPlayScriptNo > 0 && currentPage.ListRows[i].script_no == lastPlayScriptNo)
                {
                    // 그 해당 script_no의 template이 선택지인 경우
                    if (currentPage.ListRows[i].template.Equals(ScriptConst.TEMPLATE_SELECTION))
                    {
                        for (int j = i; j > 0; j--)
                        {
                            // 선택지가 아닌행을 찾아서 첫 선택지 도입의 위치를 찾아서 lastPlayScriptNO를 갱신해준다
                            if (!currentPage.ListRows[j].template.Equals(ScriptConst.TEMPLATE_SELECTION))
                            {
                                lastPlayScriptNo = currentPage.ListRows[j + 1].script_no;
                                break;
                            }
                        }

                        break;
                    }
                    else
                        break;      // 선택지가 아니면 lastPlayScriptNO를 갱신할 필요가 없다. 아마도

                }
            }

            // 코루틴 실행합니다!
            StartCoroutine(RoutineEpisodePlay());
            */
        }

        /// <summary>
        /// 게임 오브젝트 리소스 초기화 
        /// </summary>
        void InitGameResourceObjects()
        {
            /*
            for (int i = 0; i < PoolSpriteBG.Count; i++)
                PoolSpriteBG[i].gameObject.SetActive(false);

            for (int i = 0; i < PoolIllust.Count; i++)
                PoolIllust[i].gameObject.SetActive(false);
            */
            indexPoolIllust = 0;
            indexPoolBG = 0;


            //DictBackgroundMounts.Clear();
            //DictIllusts.Clear();
            //DictMinicutImages.Clear();
            //DictLiveIllusts.Clear();
            //DictLiveObjs.Clear();
            DictModelByDress.Clear();
            //DictModels.Clear();
            //DictEmoticon.Clear();

            //currentMinicutMount = null;
            currentMinicut.sprite = null;


            // 모든 스크린 이펙트 제거 
            //ScreenEffectManager.main.RemoveAllScreenEffect();

            //HideSelection(); // 선택지 제거 

            ListLoadingImageKey.Clear();
            ListLoadingModel.Clear();

            //BubbleManager.main.ClearBubbleSpriteDictionary();

            //ListMovingModels.Clear();

            // 모델 엄마 transform 위치 조정 추가 
            // 노치 폰에서는 조금더 위치를 내려준다. 
            if (SystemManager.main.hasSafeArea)
                modelPillar.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_SAFEAREA_POS_Y, 0);
            else
                modelPillar.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_ORIGIN_POS_Y, 0);

        }

        #endregion
    }

}



