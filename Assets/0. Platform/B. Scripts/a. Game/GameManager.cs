using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager main = null;      // singleton


        // 이어하기 변수
        public static bool isResumePlay = false;        // 이어하기를 통한 진입인가?
        public static string lastPlaySceneId = string.Empty;
        public static long lastPlayScriptNo = 0;
        public static bool hasLastPlayScriptNo = false;     // 이어하기 타겟 script_no가 대본상에 존재하는지 체크
        JsonData resumePlaySelectionJSON = null; // 이어하기 선택지 지나친 경로 정보 

        public EpisodeData currentEpisodeData = null;    // 선택한 에피소드 정보(JSON => Serializable Class)
        public float updatedEpisodeSceneProgressValue = 0; // 플레이 후에 업데이트된 사건 달성률 2021.12.14

        JsonData episodeElement = null;     // 에피소드 구성 및 리소스(script, background, image, illust, emoticon)
        JsonData scriptJson = null;         // 스크립트 JsonData
        [HideInInspector] public JsonData loadingJson = null;
        [HideInInspector] public JsonData loadingDetailJson = null;

        public bool isPlaying = false;
        public bool isScriptFetch = false;      // 스크립트 정보를 가져왔는지 확인

        public GameObject prefabScriptPage;     // prefab

        [Space]
        public ScriptPage currentPage;  // 생성된 현재 페이지

        public bool isThreadHold = false;           // 플레이 스레드 대기 여부. 보통 연출 대기에 쓰임
        public bool isWaitingScreenTouch = false;   // 게임 플레이 도중 스크린 터치 기다림
        public bool useSkip = false;                // 스킵을 사용했는가?
        public bool skipable = false;               // 스킵 기능 사용이 가능한가?
        public bool isAutoPlay = false;

        public bool isJustSkipStop = false; // * 막 스킵이 끝났는지 체크 변수 

        public ViewGameMenu inGameMenu;
        public string currentSceneId = string.Empty;    // 현재 sceneId(사건ID)

        public int episodeDownloadableResourceCount = 0;


        [Space]
        [Header("인게임 배경")]
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
        public GameSpriteCtrl defaultImage; // 디폴트 이미지
        public Transform illustPillar;        // live일러스트, liveObject 부모 transform
        public RawImage liveObjectTexture;


        [Space]
        [Header("스탠딩 모델")]
        public ScriptModelMount standingSpeaker = null;
        public GameModelCtrl[] characterModels = new GameModelCtrl[3];  // 0 : 좌, 1:중앙, 2:우
        public Sprite defaultCharacterSprite;
        public GameObject infoPrefab;           // 더미 캐릭터 정보 담은 prefab
        public Transform modelPillar;           // 캐릭터 모델의 부모 transform.


        [Space]
        [Space]
        public string targetSelectionSceneID = string.Empty;    // 선택지 선택 후 이동할 사건ID
        public bool isSelectionInputWait = false;               // 선택지 입력 기다리기!



        [Space]
        [Space]
        [Header("리소스 접근을 위한 Dictionary")]
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

        [Space(20)]
        public NetworkLoadingScreen gameNetworkLoadingScreen;

        [Space][Header("Sprite resources")]
        public Sprite spriteSelectionNormalBase = null;     // 일반 선택지 
        public Sprite spriteSelectionLockedBase = null;     // 선택지 잠금 상태
        public Sprite spriteSelectionUnlockedBase = null;   // 선택지 활성 상태 스프라이트
        public Sprite spriteSelectionLockIcon = null;       // 선택지 잠김 아이콘
        public Sprite spriteSelectionUnlockIcon = null;     // 선택지 잠김 풀림 아이콘

        [Space]
        public Sprite spriteInappOriginIcon = null;         // 메일함에서 사용하는 아이콘

        [Space]
        public Sprite spriteGameMessageNormal = null;       // 게임 메시지 일반
        public Sprite spriteGameMessagePositive = null;     // 게임 메시지 긍정
        public Sprite spriteGameMessageNegative = null;     // 게임 메시지 부정

        [Space]
        public Sprite spriteIllustPopup = null;             // 일러스트 획득 팝업 아이콘


        // 자동 재생
        public float flowTime = 0f;
        float delayTime = 0f;

        #region Properties

        public ScriptRow prevRow
        {
            get { return currentPage.GetPrevRow(); }
        }

        public ScriptRow currentRow
        {
            get { return currentPage.GetCurrentRow(); }
        }

        public ScriptRow nextRow
        {
            get {

                // 만약 현재에 사건 이동이 걸려있다면 다음행을 검색한다
                if(!string.IsNullOrEmpty(currentRow.target_scene_id))
                {
                    for (int i = 0; i < currentPage.ListRows.Count; i++)
                    {
                        if (currentPage.ListRows[i].scene_id == currentRow.target_scene_id)
                            return currentPage.ListRows[i];
                    }
                }

                return currentPage.GetNextRowWithoutIncrement(); 
            }
        }


        #endregion

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

            // PopupManager.main.InitPopupManager();
            
            yield return null;
            
            Signal.Send(LobbyConst.STREAM_COMMON, GameConst.SIGNAL_GAME_PLAY); // 게임씬 시작을 알린다. 

            GarbageCollect();

            isPlaying = false;
            isScriptFetch = false;
            
            

            // 유저 정보 로그인이 되었는지 체크
            yield return new WaitUntil(() => UserManager.main != null && UserManager.main.GetUserAccountJSON() != null);
            Debug.Log("GameManager account check done");

            // 로비에서 에피소드 선택해서 게임씬에 진입한 경우
            if (SystemManager.main.givenEpisodeData != null)
                SelectEpisode(SystemManager.main.givenEpisodeData); // 바로 선택한 에피소드 할당 처리
            else
            {
                // 단독 실행, 없을 떄는 지정받은 에피소드 ID를 통해 진행
                Debug.Log("GameManager RequestStoryInfoIndependently");

                // 프로젝트 정보를 가져오고, 설정한 에피소드 데이터를 사용한다.
                StoryManager.main.RequestStoryInfo(StoryManager.main.CurrentProject);
            }

            SoundSetting(GameConst.BGM_VOLUME, 0);
            SoundSetting(GameConst.VOICE_VOLUME, 1);
            SoundSetting(GameConst.SOUNDEFFECT_VOLUME, 2);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                ScriptRow lastRow = currentPage.FindLastSceneRow();
                if (lastRow != null)
                {
                    HandleScriptJump(lastRow);
                }
            }

        }


        /// <summary>
        /// 게임 씬에서 사용하는 네트워크 로딩 스크린 받기
        /// </summary>
        /// <returns></returns>
        public NetworkLoadingScreen GetGameNetworkLoadingScreen()
        {
            return gameNetworkLoadingScreen;
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
                PlayerPrefs.SetFloat(key, 1f);

            SoundGroup[soundIndex].ChangeSoundVolume(PlayerPrefs.GetFloat(key));
        }

        public void RequestEpisodeScript()
        {
            Debug.Log("<color=cyan>RequestEpisodeScript</color>");

            // 통신 
            JsonData j = new JsonData();
            j[CommonConst.COL_EPISODE_ID] = StoryManager.main.CurrentEpisodeID;
            j[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            j[CommonConst.FUNC] = GameConst.FUNC_GET_EPISODE_SCRIPT;

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
        public void SelectEpisode(EpisodeData __currentEpisodeData)
        {
            currentEpisodeData = __currentEpisodeData;

            // 스토리 매니저에게 현재 실행하는 에피소드 ID를 전달한다. 
            StoryManager.main.SetCurrentEpisodeJson(currentEpisodeData);

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
            currentPage = Instantiate(prefabScriptPage, this.transform).GetComponent<ScriptPage>();
            currentPage.InitPage(episodeElement);

            episodeDownloadableResourceCount = currentPage.GetPrepCount();

            for (int i = 0; i < currentPage.ListRows.Count; i++)
            {
                // 최근 스크립트No가 0보다 크고, 해당 script_no를 찾았을 때
                if (lastPlayScriptNo > 0 && currentPage.ListRows[i].script_no == lastPlayScriptNo)
                {
                    // 그 해당 script_no의 template이 선택지인 경우
                    if (currentPage.ListRows[i].template.Equals(GameConst.TEMPLATE_SELECTION))
                    {
                        for (int j = i; j > 0; j--)
                        {
                            // 선택지에 관련되지 않은 행을 찾아서 첫 선택지 도입의 위치를 찾아서 lastPlayScriptNO를 갱신해준다
                            if (!currentPage.ListRows[j].template.Contains(GameConst.TEMPLATE_SELECTION))
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
        }

        /// <summary>
        /// 게임 오브젝트 리소스 초기화 
        /// </summary>
        void InitGameResourceObjects()
        {
            for (int i = 0; i < PoolSpriteBG.Count; i++) {
                if(PoolSpriteBG[i] == null)
                    continue;
                    
                PoolSpriteBG[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < PoolIllust.Count; i++) {
                if(PoolIllust[i] == null)
                    continue;
                
                PoolIllust[i].gameObject.SetActive(false);
            }

            indexPoolIllust = 0;
            indexPoolBG = 0;


            DictBackgroundMounts.Clear();
            DictIllusts.Clear();
            DictMinicutImages.Clear();
            DictLiveIllusts.Clear();
            DictLiveObjs.Clear();
            DictModelByDress.Clear();
            DictModels.Clear();
            DictEmoticon.Clear();

            currentMinicutMount = null;
            currentMinicut.sprite = null;


            // 모든 스크린 이펙트 제거 
            ScreenEffectManager.main.RemoveAllScreenEffect();

            //HideSelection(); // 선택지 제거 

            ListLoadingImageKey.Clear();
            ListLoadingModel.Clear();

            BubbleManager.main.ClearBubbleSpriteDictionary();

            // 모델 엄마 transform 위치 조정 추가 
            // 노치 폰에서는 조금더 위치를 내려준다. 
            if (SystemManager.main.hasSafeArea)
                modelPillar.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_SAFEAREA_POS_Y, 0);
            else
                modelPillar.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_ORIGIN_POS_Y, 0);

        }

        /// <summary>
        /// 프로그레서 값 주세요!(로딩 뷰에게 준다!)
        /// </summary>
        public float GetPagePrepValue()
        {
            // 로딩이 진행될수록 카운트가 감소한다. 

            if (episodeDownloadableResourceCount == currentPage.GetCurrentLoadingCount())
                return 0;

            if (currentPage.GetCurrentLoadingCount() <= 0) // 다 된거..
                return 1;

            return 1 - (float)currentPage.GetCurrentLoadingCount() / (float)episodeDownloadableResourceCount;
        }

        public bool GetCurrentPageInitialized()
        {
            return currentPage.IsPageInitialized();
        }

        /// <summary>
        /// 로딩 막히는부분 좀 찾을려고..
        /// </summary>
        /// <returns></returns>
        public string GetDebugDouwnloadFactor()
        {
            return currentPage.GetDebugLoadingCount();
        }

        #endregion

        #region 이어하기 로직

        /// <summary>
        /// 이어하기 게임의 경우 정지 포인트를 찾는다. 
        /// </summary>        
        bool CheckResumePlayStopPoint()
        {
            // 두가지를 판단한다. lastPlaySceneID에 도달했는지
            // lastScriptNo에 도달했는지 (0보다 클때만)

            if (lastPlayScriptNo > 0)
            { // 스크립트에 lastScriptNO가 일치하는게 있다면!

                if (lastPlayScriptNo == currentRow.script_no)
                {
                    Debug.Log("Resume Play, Stop at " + lastPlayScriptNo);
                    isResumePlay = false;
                }
            }
            else
            { // 유효한 scriptNO가 없는경우 (저장 시점 이후 대본이 변경됨)
                // lastPlayScriptNO가 없을때, scene_id 시작점에서 멈춘다.
                if (!string.IsNullOrEmpty(currentRow.scene_id) && currentRow.scene_id == lastPlaySceneId)
                {
                    Debug.Log("Resume Play, Stop at " + lastPlaySceneId);
                    isResumePlay = false;
                }
            }

            return isResumePlay;
        }

        /// <summary>
        /// 이어하기 유효성 검사 
        /// </summary>
        void CheckResumePlayValidation()
        {

            if (!isResumePlay)
                return;

            // 이어하기에서는 선택지 기록을 따라가기 때문에 
            // 불완전한 선택지가 있으면 안된다.
            if (currentPage.CheckIncompleteSelection())
            {
                SystemManager.ShowMessageAlert("불완전한 선택지가 존재하여 이어하기를 진행할 수 없습니다.", false);
                isResumePlay = false;
                return;
            }


            // lastPlayScriptNO 정보가 있는 경우 스크립트상 일치하는 행이 있어야 한다.
            // * 이어하기, 타겟 ScriptNO가 있으면 페이지에 실제로 있는지 체크 
            if (lastPlayScriptNo > 0)
            {
                hasLastPlayScriptNo = currentPage.CheckHasScriptNO(lastPlayScriptNo);

                // 대본이 저장시점 이후로 편집되어서 달라진 경우는 0으로 변경해놓는다.
                if (!hasLastPlayScriptNo)
                    lastPlayScriptNo = 0;
            } // ? 끝 
        }

        #endregion

        #region 게임 로직

        /// <summary>
        /// 에피소드 플레이 코루틴 
        /// </summary>
        /// <returns></returns>
        IEnumerator RoutineEpisodePlay()
        {
            // 중복 실행 가능성 제거 
            if (isPlaying)
            {
                Debug.LogError("RoutineEpisodePlay Double Called!!!");
                yield break;
            }

            Debug.Log("<color=yellow> RoutineEpisodePlay </color>");
            AdManager.main.InitGamePlayRowCount(); // 광고 Row 초기화 

            // 첫 실행히 page가 생성되어 있지 않다면 기다려준다.
            while (currentPage == null)
                yield return null;

            // 모든 리소스 로딩을 완료할때까지 기다리기!
            yield return new WaitUntil(() => currentPage.IsPageInitialized());
            Debug.Log("<color=yellow> Current Page is just initialized! </color>");
            Signal.Send(LobbyConst.STREAM_GAME, "gameLoadingComplete", string.Empty);

            // 페이지에게 실행할 Row를 요청한다.(얘는 첫번째 행)
            ScriptRow currentRow = currentPage.GetNextRow();
            isPlaying = true; // 플레이 시작함!

            // 시작 기록 업데이트를 호출한다.
            NetworkLoader.main.UpdateEpisodeStartRecord();
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork()); // 통신 완료까지 대기.

            Debug.Log("<<< UpdateEpisodeStartRecord done >>>");

            // * 2021.08.27 이어하기에 대한 처리. isResumePlay.... 
            // 1. 단순 스킵 useSkip = true
            // 2. 이어하기 isResumePlay = true
            // 3. 일반 (단순 스킵도 아니고 이어하기도 아니다) useSkip = false, isResumePlay = false

            // isResumePlay는 스킵과 비슷하게 동작을 한다. 
            // isResumePlay가 true이면, useSkkip도 true다.
            // isResumePlay의 lastPlaySceneID, lastScriptNO를 만날때까지 계속 Skip을 수행하고, 목표치에 도달하면
            // isResumePlay를 풀고, useSkip도 푼다. 
            // lastScriptNO는 스크립트를 편집하면 일치하는 행이 없을 수도 있다. 실행전에 미리 유효성 검사를 하고 진행한다.

            // ! 이어하기 유효성 검사 
            InitSelectionProgressRoute();
            CheckResumePlayValidation();

            // ! 띠배너 광고 
            // AdManager.main.LoadBanner();
            

            Firebase.Analytics.FirebaseAnalytics.LogEvent("EpisodeLoadingDone", new Firebase.Analytics.Parameter("project_id", StoryManager.main.CurrentProjectID)
            , new Firebase.Analytics.Parameter("episode_id", StoryManager.main.CurrentEpisodeID));


            // 모든 라인을, 혹은 종료 명령어를 만날때까지 계속해! 
            while (currentRow != null && isPlaying)
            {
                
                // * 이어하기 추가 처리
                if (isResumePlay)
                { // true일때만 호출해야한다.
                    // 정지 포인트에서 useSkip을 false로 바꿔야 한다. 
                    useSkip = CheckResumePlayStopPoint();

                    // 방금 스킵이 종료되었다면, 아래 변수를 true로 처리한다.
                    if (!useSkip)
                    {
                        // * skip으로 진행되다가, 마지막 중단지점에서 갑자기 이전 라인을 트윈시키기 때문에
                        // * 캐릭터 2인 스탠딩 구도에 변화가 필요할때 아래 변수가 필요하다.
                        // * 2인 스탠딩에서 1인 스탠딩으로 신규캐릭터가 등장하는 시점 등이 문제가 되었다. 
                        // * 이 변수는 현재는 오직 GameModelCtrl과 관련된 곳에서만 사용된다. 
                        isJustSkipStop = true;

                        // 22.01.19 isResumePlay인동안 skipable 체크가 true를 반환하고 있어서 이어하기가 딱 끝난 시점에서 한번 더 skipable을 체크할 필요가 있다
                        CheckSkipable(currentSceneId);
                    }
                }
                else
                {
                    if (currentRow.template.Equals(GameConst.TEMPLATE_SELECTION) || !string.IsNullOrEmpty(currentRow.requisite))
                    {
                        SystemManager.ShowNetworkLoading();
                        yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
                        SystemManager.HideNetworkLoading();
                    }
                }

                // 현재 '행'의 동작을 수행합니다. 
                currentRow.ProcessRowAction(OnFinishedRowAction, useSkip);


                // * 일반 플레이. 
                if (!useSkip)
                {
                    yield return null;

                    while (isThreadHold) // Row가 동작중에는 멈춰 있습니다. 
                        yield return null;
                    
                    while (isWaitingScreenTouch) // 화면을 터치해줘야 다음으로 넘어갑니다.
                    {
                        // 자동 재생을 사용하는 경우
                        if (isAutoPlay)
                        {
                            delayTime = PlayerPrefs.GetFloat(GameConst.AUTO_PLAY);
                            flowTime += Time.deltaTime;

                            // 자동재생 쓰는데 광고 재생되면 꺼질때까지 멈춥시다
                            if (AdManager.main.isAdShowing)
                            {
                                yield return new WaitUntil(() => !AdManager.main.isAdShowing);
                                yield return null;
                            }

                            // 보이스를 사용한다면 보이스가 다 재생될 때까지 기다리고
                            if (SoundGroup[1].GetIsPlaying)
                            {
                                while (SoundGroup[1].GetIsPlaying && isWaitingScreenTouch)
                                    yield return null;

                                if(isWaitingScreenTouch)
                                {
                                    delayTime = 2f;

                                    while (flowTime < delayTime)
                                    {
                                        flowTime += Time.deltaTime;
                                        yield return null;
                                    }
                                }
                            }

                            if (flowTime >= delayTime)
                            {
                                isWaitingScreenTouch = false;
                                flowTime = 0f;
                            }
                        }

                        yield return null;
                    }
                }

                isJustSkipStop = false; // 한번 액션이 일어났으면 false로 변경.

                // 선택지 입력이 필요할때는 기다려야해..!
                while (isSelectionInputWait)
                    yield return null;

                // 중단점 추가. 
                if (!isPlaying)
                    break;

                // 현재 행의 액션을 종료해줍니다. 
                currentRow.ExitRowAction();

                // 조건과 이동의 처리 
                HandleScriptJump(currentRow);

                currentRow = currentPage.GetNextRow(); // 새로운 행 받아오기

                // 광고 처리 추가 (SKIP 아닐때)
                if (!useSkip)
                    AdManager.main.AddGameRowCount();

            }


            GarbageCollect();

            // 띠배너 보여주고 있었다면 감추기.
            // AdManager.main.HideBanner();

            if (isPlaying)
            {
                if (UserManager.main.useRecord)
                    ShowGameEnd(null);
                else
                    SystemManager.ShowGamePopup(SystemManager.GetLocalizedText("6203"), RetryPlay, EndGame);
            }

            isPlaying = false;
        }

        void OnFinishedRowAction()
        {
            isThreadHold = false;
        }

        /// <summary>
        /// 스크립트 이동 처리 
        /// </summary>
        /// <param name="__row"></param>
        void HandleScriptJump(ScriptRow __row)
        {
            if (string.IsNullOrEmpty(__row.target_scene_id))
                return;

            // 선택지 템플릿은 이동 컬럼 무시하도록 처리 
            if (__row.template == GameConst.TEMPLATE_SELECTION)
                return;

            // 조건이 있을때. 
            if (!string.IsNullOrEmpty(__row.requisite))
            {
                if (ScriptExpressionParser.main.ParseScriptExpression(__row.requisite))
                    MoveToTargetSceneID(__row.target_scene_id);
                else
                {
                    Debug.Log(string.Format("<color=yellow>조건불충분 {0}</color>", __row.requisite));
                    return;
                }
            }

            MoveToTargetSceneID(__row.target_scene_id);
        }

        /// <summary>
        /// 타겟 상황ID로 이동시키기 
        /// </summary>
        public void MoveToTargetSceneID(string __sceneID)
        {
            targetRow = -1;

            if (__sceneID.Contains(GameConst.TAG_TARGET_EPISODE) || __sceneID.Contains(GameConst.TAG_TARGET_EPISODE2))
            {
                Debug.Log("Jump to other Episode :: " + __sceneID);
                ShowGameEnd(__sceneID);
                return;
            }


            for (int i = 0; i < currentPage.ListRows.Count; i++)
            {
                // 상황 ID를 찾는다
                if (currentPage.ListRows[i].scene_id == __sceneID)
                    targetRow = i;
            }

            if (targetRow < 0)
            {
                ShowMissingComponent("이동하려는 상황ID 없음", __sceneID);
                return;
            }

            currentPage.playRow = targetRow;

            if (!isPlaying)
                StartCoroutine(RoutineEpisodePlay());
        }


        /// <summary>
        /// 말풍선(대화 관련 처리 시작)
        /// </summary>
        public void SetTalkProcess(ScriptRow __row, Action __cb)
        {
            // 유효한 화자가 아니면 아래 코드는 작동X
            if (!__row.IsValidSpeaker)
            {
                ShowMissingComponent(__row.template, "잘못된 화자 입력");
                __cb?.Invoke();
                return;
            }

            // 스탠딩 표현이 없거나 화자 리소스가 존재하지 않으면 말풍선만 띄워주고 넘김
            if (string.IsNullOrEmpty(__row.character_expression) || !CheckModelResourceExists(__row.speaker))
            {
                ViewGame.main.MakeTalkBubble(__row, __cb);
                return;
            }

            #region 스탠딩 캐릭터 모델 처리
            // * 캐릭터 모델 처리를 시작.. 
            // speaker로 판단. 생성된 캐릭터 모델이 있고, 캐릭터 표현 있음
            int characterIndex = -1;     //캐릭터 모델 index 0:왼쪽, 1:중앙, 2:오른쪽
            int characterCount = CheckModelStanding(); // 현재 화면에 서있는 모델의 개수
            bool isSameTalker = CompareWithCurrentSpeaker(__row.speaker);  // 전의 화자와 같은지 체크, 의상이 변경되도 다른 화자로 인식

            // 화자에 등장 방향이 지정된 경우와 아닌 경우가 있다. 
            // 케일:L or 케일:R 이런식으로 지정되었을 때에 대한 처리 구분될 필요 있음.
            switch (__row.direction)
            {
                case GameConst.POS_LEFT:
                case GameConst.POS_RIGHT:

                    if (__row.direction.Equals(GameConst.POS_LEFT))
                        characterIndex = 0;
                    else
                        characterIndex = 2;

                    Debug.Log(string.Format("SetTalkProcess start [{0}] / [{1}]", __row.direction, __row.speaker));

                    // 동일한 사이드 캐릭터가 계속 말하는중
                    if (isSameTalker)
                    {
                        // 화면에 아직 1명 뿐인데 L or R이 들어오면 센터로
                        // * 같은 캐릭터이고 화면에 캐릭터가 혼자인경우, 방향 지정이 되어있으면, 무시하고 그냥 중앙에서 진행. 
                        if (characterCount < 2)
                            characterIndex = 1;

                        // 애니메이션 
                        characterModels[characterIndex].PlayCubismAnimation(__row, characterIndex);
                    }
                    else // ! 다른 화자 등장!
                    {
                        // * 다른 캐릭터가 방향을 지정받아 등장하면, 2인스탠딩이 시작된다. 
                        if (characterCount < 2) // 한명, 혹은 아무도 없을때. 
                        {
                            // * 캐릭터가 아무도 없었다. 
                            if (characterCount < 1)
                            {
                                // 방향 지정을 무시하고 위치는 센터로 fix한다. 
                                characterIndex = 1;

                                // 밀어낼 캐릭터가 없으니까 아무것도 하지 않는다.

                            }
                            else
                            { // * 다른 캐릭터 1명이 있었다. 

                                // 서있던 캐릭터 친구를 오른쪽 혹은 왼쪽으로 밀어낸다.
                                // 새등장 위치가 왼쪽이면 오른쪽으로 밀고, 새등장 위치가 오른쪽이면 왼쪽으로 민다.
                                PushFromCenter(characterIndex == 0 ? 2 : 0);
                                Debug.Log("Push Previous Character <<<<<");

                            }

                            // 동일한 캐릭터가 아니니까 캐릭터 준비시킨다.
                            SetCharacterReady(__row.speaker, characterIndex);

                            // 애니메이션 처리 
                            characterModels[characterIndex].PlayCubismAnimation(__row, characterIndex);
                        } // ? end of characterCount < 2
                        else // * 이미 다른 두명의 캐릭터가 있는 경우에 대한 처리 
                        {
                            // 등장 위치와 row를 파라매터로 전달. 
                            ChanageDuetSpeaker(characterIndex, __row);
                        }
                    } // ? 다른 화자 등장에 대한 처리 끝.

                    break; // ? 방향 지정 처리 끝. 

                default: // * 방향 지정이 되어있지 않은 경우에 대한 처리 시작.

                    // center index fix. 
                    characterIndex = 1;


                    // 2인 스탠딩 중이었을때.
                    // 새로운 화자가 등장했으면 나머지 2인은 퇴장시킨다. 
                    // 2인 스탠딩 중 하나의 캐릭터라면, 중앙으로 옮겨준다. (나머지 한명은 퇴장)
                    if (characterCount > 1)
                    {
                        // 이전 화자와 같지 않으면 전부 hide(퇴장)
                        for (int i = 0; i < characterModels.Length; i++)
                        {
                            if (characterModels[i] == null)
                                continue;

                            // 중앙에 쓸건데 중앙은 건들지 말자
                            // 조건을 걸어주지 않으면 아래 로직에서 기껏 중앙으로 옮겨놨는데 옮겨놓은 중앙까지 null처리 해버린다
                            if (i == 1)
                                continue;

                            // 현재 화자가 2인스탠딩에서 좌,우 중 한곳에 이미 배치되어있는 경우
                            // 중앙 index로 옮겨주고, 있던 사이드 위치는 null 로 비워준다. 
                            if (characterModels[i].speaker.Equals(__row.speaker))
                            {
                                characterModels[characterIndex] = characterModels[i];
                                characterModels[i] = null;
                            }
                            else // 다른 화자들은 모두 hide 처리. 
                            {

                                if (useSkip || __row.autoplay_row > 0)
                                    characterModels[i].HideModel(i, true);
                                else
                                    characterModels[i].HideModel(i, false);

                                // skip 사용 중에는 아예 바로 null 처리, skip 아닌 경우에는 퇴장을 기다린다. 
                                if (useSkip)
                                    characterModels[i] = null;
                                else
                                    StartCoroutine(RoutineWaitCharacterOut(i));
                            }

                        } // ? end of for
                    } // ? end of characterCount > 0

                    // 동일 화자가 아닐때에 대한 추가 처리. 
                    if (!isSameTalker)
                    {
                        // 중앙에 한명만 있을 때, 이전 화자가 다르면 hide
                        // 위의 로직을 그대로 사용하면 중앙에 왔을 때 건너 뛰어버려서 1인 스탠딩 hide가 실행이 안됨
                        // if (characterCount > 0 && characterCount < 2) // 이건 그냥... 1이자나? 
                        if (characterCount == 1)
                            characterModels[characterIndex].HideModel(characterIndex, useSkip);

                        // 동일한 화자가 아니라면 캐릭터 세팅
                        SetCharacterReady(__row.speaker, characterIndex);
                    }

                    characterModels[characterIndex].PlayCubismAnimation(__row, characterIndex);
                    break;
            }

            #endregion

            ViewGame.main.MakeTalkBubble(__row, __cb, characterIndex);
        }

        /// <summary>
        /// 모델 리소스 있는지 체크.
        /// </summary>
        bool CheckModelResourceExists(string __speaker)
        {
            if (DictModelByDress.ContainsKey(__speaker))
                return DictModels.ContainsKey(DictModelByDress[__speaker]);
            else
                return DictModels.ContainsKey(__speaker);
        }

        /// <summary>
        /// 화자가 변경될때 호출해서, 캐릭터를 지정한 인덱스에 준비시킨다. 
        /// </summary>
        /// <param name="speaker">화자</param>
        /// <param name="index">0=L, 1=C, 2=R</param>
        void SetCharacterReady(string speaker, int index)
        {
            standingSpeaker = GetConnectedModelMount(speaker);

            // * 모델 생성전이면 생성하고 처리 
            if (!standingSpeaker.isModelCreated)
            {
                standingSpeaker.InstantiateCubismModel();
            }

            // 모델 컨트롤러 할당
            characterModels[index] = standingSpeaker.modelController;

            Debug.Log(string.Format("SetCharacterReady done [{0}], [{1}]", speaker, index));
        }

        #region 2인 스탠딩 연출 관련

        /// <summary>
        /// 캐릭터 모델의 움직임이 완료되었는가? 
        /// </summary>
        /// <returns></returns>
        public bool CheckModelMoveComlete()
        {
            for (int i = 0; i < characterModels.Length; i++)
            {
                if (characterModels[i] == null)
                    continue;

                if (!characterModels[i].moveComplete)
                    return false;
            }

            return true;
        }

        void PushFromCenter(int index)
        {
            if (characterModels[1] != null)
                characterModels[index] = characterModels[1];

            characterModels[1] = null;
            characterModels[index].PushListenerToSide(index, useSkip || isJustSkipStop);
        }

        /// <summary>
        /// 2인 스탠딩 중 1명 변경
        /// </summary>
        /// <param name="newSpeakerIndex">새로 등장하는 캐릭터 포지션</param>
        /// <param name="remainSpeakerIndex">남아있는 캐릭터 포지션</param>
        void ChanageDuetSpeaker(int newSpeakerIndex, ScriptRow __row)
        {
            // 남아있는 캐릭터 index
            int remainSpeakerIndex = 0;
            // * 왼쪽 혹은 오른쪽에 신규 캐릭터가 들어오면, 어차피 남아있는 index는 그 반대편 index 아닌가? 
            // 왼쪽이면 2, 오른쪽이면 0 
            remainSpeakerIndex = newSpeakerIndex == 0 ? 2 : 0;


            // 새로 등장하는 위치에 다른 캐릭터가 있었으면, 있던 캐릭터는 HideModel 처리. 
            if (!characterModels[newSpeakerIndex].speaker.Equals(__row.speaker))
                characterModels[newSpeakerIndex].HideModel(newSpeakerIndex, useSkip);

            // 남아있는 캐릭터 친구는 후위로 밀어주기. 
            characterModels[remainSpeakerIndex].SetListener();

            // 새 캐릭터 준비시키고, 애니메이션
            SetCharacterReady(__row.speaker, newSpeakerIndex);
            characterModels[newSpeakerIndex].PlayCubismAnimation(__row, newSpeakerIndex);
        }

        /// <summary>
        /// 캐릭터의 퇴장연출을 기다렸다가 지정된 index를 null처리한다. 
        /// </summary>
        /// <param name="index">퇴장한 캐릭터</param>
        IEnumerator RoutineWaitCharacterOut(int index)
        {
            yield return new WaitUntil(() => CheckModelMoveComlete());
            characterModels[index] = null;
        }

        #endregion

        #endregion

        #region 인게임 필요 메소드

        /// <summary>
        /// 장소 이탈, 진입에 사용되는 연출 시간 계산
        /// </summary>
        /// <param name="movableWidth">움직일 수 있는 거리</param>
        public float CalcMoveBGAnimTime(ref float movableWidth)
        {
            float cameraHeight = 2 * Camera.main.orthographicSize;
            float cameraWidth = cameraHeight * Camera.main.aspect;

            // 스케일 조정되는 배경에 대한 처리 추가 
            movableWidth = Mathf.Abs(currentBG.spriteRenderer.size.x * currentBG.gameScale - cameraWidth) * 0.5f;
            float distanceProportion = 1f; // 거리에 다른 트윈 시간 비율

            if (movableWidth <= 0)
                distanceProportion = 0;
            else if (movableWidth > 0 && movableWidth <= 5)
                distanceProportion = 0.5f;
            else if (movableWidth > 5 && movableWidth <= 10)
                distanceProportion = 1f;
            else
                distanceProportion = 1.5f;

            // 거리에 따라 트윈 시간 설정 
            return 2f * distanceProportion;
        }


        /// <summary>
        /// 게임 로그에 표기명으로 전달해준다
        /// </summary>
        /// <returns>표기명 string값</returns>
        public string GetNotationName(ScriptRow __row)
        {
            // 대체 이름이 존재하지 않으면 화자 return
            if (string.IsNullOrEmpty(__row.controlAlternativeName))
                return StoryManager.main.GetNametagName(__row.speaker);
            // 대체 이름이 존재하면 대체 이름 return
            else
                return StoryManager.main.GetNametagName(__row.controlAlternativeName);
        }

        /// <summary>
        /// 찾고자 하는 템플릿이 맞는지 check
        /// </summary>
        /// <param name="__row">이전, 현재, 다음 scriptRow 값에 대해 받는다</param>
        /// <param name="template">찾을 template string value</param>
        public bool IsSameTemplate(ScriptRow __row, string template)
        {
            // playRow가 총 Count보다 같거나 많아지면 종료나 마찬가지이므로 체크할 수 없어서 false를 리턴한다
            if (currentPage.playRow > currentPage.ListRows.Count)
                return false;

            if (__row != null && __row.template.Contains(template))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 화면 렌더 건너뛰기
        /// </summary>
        /// <returns>true : 건너뛰기 가능/ false : 건너뛰기 불가능</returns>
        public bool RenderingPass()
        {
            for (int i = currentPage.playRow - 1; i < currentPage.ListRows.Count; i++)
            {
                // 뭔가 스크린 정리하는 템플릿이 있으면 true로 렌더링 스킵을 하자
                switch (currentPage.ListRows[i].template)
                {
                    case GameConst.TEMPLATE_BACKGROUND:
                    case GameConst.TEMPLATE_MOVEIN:
                    case GameConst.TEMPLATE_CLEAR_SCREEN:

                        // 말풍선 유지가 있으면 일단 렌더
                        if (currentPage.ListRows[i].bubble_hold > 0)
                            return false;

                        // 이어하기 중인데 조건에 걸린 부분이 멈춰야 할 구간보다 더 간 것이라면 Render skip no!
                        if (isResumePlay && currentPage.ListRows[i].script_no > lastPlayScriptNo)
                            return false;

                        return true;
                    case GameConst.TEMPLATE_SELECTION:
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 이동된 이후 행에 갤러리 공개용 일러스트(이미지, 일러스트)가 있는지 체크
        /// </summary>
        /// <returns></returns>
        public bool CheckNextSceneHasIllust(string __sceneID)
        {
            targetRow = -1;

            for (int i = 0; i < currentPage.ListRows.Count; i++)
            {
                // 상황 ID를 찾는다
                if (currentPage.ListRows[i].scene_id == __sceneID)
                    targetRow = i;
            }

            if (targetRow < 0)
            {
                ShowMissingComponent("이동하려는 상황ID 없음", __sceneID);
                return false;
            }

            for (int i = targetRow; i < currentPage.ListRows.Count; i++)
            {
                // 행을 진행하다가 다음 사건ID를 만나게 된다면 일러스트가 존재하지 않으므로 return false를 해준다
                if (!string.IsNullOrEmpty(currentPage.ListRows[i].scene_id) && currentPage.ListRows[i].scene_id != __sceneID)
                    return false;

                switch (currentPage.ListRows[i].template)
                {
                    case GameConst.TEMPLATE_ILLUST:
                    case GameConst.TEMPLATE_IMAGE:
                    case GameConst.TEMPLATE_LIVE_ILLUST:
                    case GameConst.TEMPLATE_LIVE_OBJECT:

                        // 공개용 일러스트인지 체크
                        if (UserManager.main.RevealedGalleryImage(currentPage.ListRows[i].script_data))
                            return true;
                        else
                            return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 마지막 화자와 파라매터의 화자가 동일한지 체크한다. 
        /// </summary>
        /// <param name="nextSpeaker">이번에 말할 화자</param>
        /// <returns>true = 마지막 화자 == 이번에 말할 화자 / false = 마지막 화자 != 이번에 말할 화자</returns>
        public bool CompareWithCurrentSpeaker(string nextSpeaker)
        {
            if (string.IsNullOrEmpty(nextSpeaker))
            {
                Debug.Log("NextSpeaker is empty");
                return false;
            }

            // originModelName과 speaker를 비교하는 부분 수정했음. 
            if (standingSpeaker != null && !string.IsNullOrEmpty(standingSpeaker.originModelName) && DictModelByDress.ContainsKey(nextSpeaker) && standingSpeaker.originModelName.Equals(DictModelByDress[nextSpeaker]))
                return true;
            else
                return false;
        }

        /// <summary>
        /// speaker model 있는지 체크 
        /// </summary>
        public bool GetSpeakerModelExists(string standSpeaker)
        {
            if (string.IsNullOrEmpty(standSpeaker))
                return false;

            if (CompareWithCurrentSpeaker(standSpeaker))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 모델 스탠딩 갯수
        /// </summary>
        public int CheckModelStanding()
        {
            int count = 0;

            for (int i = 0; i < characterModels.Length; i++)
            {
                if (characterModels[i] != null)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// 현재 캐릭터 모델의 키
        /// </summary>
        public int GetCurrentModelTall(string standSpeaker)
        {
            if (standingSpeaker != null && CompareWithCurrentSpeaker(standSpeaker))
                return standingSpeaker.modelController.tallGrade;

            else if (standingSpeaker.model == null)
                return 1;

            else
                return 0;
        }

        /// <summary>
        /// 페이지 초기화 마지막 단계에 의상 연결을 위해 화자를 정리한다. 
        /// </summary>
        public void CollectDistinctSpeaker()
        {
            JsonData dressProgress;

            // 앞에서부터. 한번 등록하면 continue한다. 
            // 유저 데이터를 참고해서 있으면 사용한다.
            for (int i = 0; i < currentPage.ListRows.Count; i++)
            {
                if (!currentPage.ListRows[i].IsSpeakable)
                    continue;

                if (DictModelByDress.ContainsKey(currentPage.ListRows[i].speaker))
                    continue;

                // 최초 등록시에는 key와 value 를 같게 등록한다. 
                DictModelByDress.Add(currentPage.ListRows[i].speaker, currentPage.ListRows[i].speaker);
            }
            
            /*
            dressProgress = UserManager.main.GetNodeDressProgress();

            // DictModelByDress 를 수집하고 유저 데이터 기반해서 재설정
            for (int i = 0; i < dressProgress.Count; i++)
            {
                if (DictModelByDress.ContainsKey(SystemManager.GetJsonNodeString(dressProgress[i], GameConst.COL_SPEAKER)))
                    DictModelByDress[SystemManager.GetJsonNodeString(dressProgress[i], GameConst.COL_SPEAKER)] = SystemManager.GetJsonNodeString(dressProgress[i], GameConst.COL_MODEL_NAME);
            }
            */

        }

        /// <summary>
        /// 의상 템플릿을 통해 연결점 수정 
        /// </summary>
        public void UpdateModelByDress(string __speaker, string __targetOriginModelName)
        {
            Debug.Log(string.Format("UpdateModelByDress speaker [{0}], targetModelName [{1}]", __speaker, __targetOriginModelName));

            if (!DictModelByDress.ContainsKey(__speaker))
            {
                ShowMissingComponent("의상", string.Format("{0} 화자 없음", __speaker));
                return;
            }

            if (!DictModels.ContainsKey(__targetOriginModelName))
            {
                ShowMissingComponent("의상", string.Format("{0} 의상과 연결된 모델", __targetOriginModelName));
                return;
            }

            DictModelByDress[__speaker] = __targetOriginModelName;
        }

        /// <summary>
        /// 스탠딩 캐릭터 생성될 오브젝트(Transform)
        /// </summary>
        public void SetModelParent(Transform __model)
        {
            __model.SetParent(modelPillar);
        }

        /// <summary>
        /// 라이브 일러스트, 라이브 오브제는 동일한 부모를 사용한다
        /// </summary>
        public void SetIllustParent(Transform __model)
        {
            __model.SetParent(illustPillar);
        }

        /// <summary>
        /// 스킵이 가능한지 체크하기
        /// </summary>
        public void CheckSkipable(string sceneID)
        {
            // 이어하기 중에는 스킵 가능 구간을 다르게 측정한다
            if (isResumePlay)
                skipable = CheckResumePlayStopPoint();
            else
                skipable = UserManager.main.CheckSceneHistory(sceneID);

            // 스킵중이었는데 skipable이 false라면
            if (useSkip && !skipable)
                useSkip = false;

            inGameMenu.ChangeSkipIcon(skipable);
        }

        /// <summary>
        /// 행에 딜레이 걸기
        /// </summary>
        /// <param name="delayTime">몇 초 걸건지</param>
        public void ExecuteDelayRow(int delayTime)
        {
            if (isThreadHold)
                Invoke("OnFinishedRowAction", delayTime);
        }

        /// <summary>
        /// 게임 플레이 지점을 저장한다.
        /// </summary>
        public void SaveCurrentPlay()
        {
            if (currentRow == null)
            {
                Debug.Log("CurrentRow is Null");
                return;
            }

            NetworkLoader.main.UpdateUserProjectCurrent(StoryManager.main.CurrentEpisodeID, currentSceneId, currentRow.script_no);
        }

        /// <summary>
        /// 게임 나가기 - 정상 종료 
        /// </summary>
        public void EndGame()
        {
            Debug.Log("EndGame");
            isPlaying = false;
            
            Signal.Send(LobbyConst.STREAM_COMMON, "LobbyBegin"); // 노드 제어
            

            // 네트워크 창을 띄우고. 로비 씬으로 돌아가야한다.
            SystemManager.ShowNetworkLoading();
            IntermissionManager.isMovingLobby = true;
            SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
        }

        /// <summary>
        /// 게임 중간에 나가기. 
        /// </summary>
        public void QuitGame()
        {
            // * 중간 종료되는 경우에는 tutorial 관련 처리 초기화 시킨다. 
            UserManager.main.tutorialFirstProjectID = 0; // 0으로 초기화 시켜버린다.
                     
            // 게임씬을 빠져나가기 전에 현재 상황을 저장하자!
            SaveCurrentPlay();

            EndGame();

        }

        /// <summary>
        /// 재시작 하기 
        /// </summary>
        public void RetryPlay()
        {
            Debug.Log("RetryPlay");
            isPlaying = false;
            
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_GAME_BEGIN); // 노드 제어

            // 씬 로드 
            IntermissionManager.isMovingLobby = false;
            SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
        }


        #region Resource Object Handling

        /// <summary>
        /// 게임 배경을 설정한다. 
        /// </summary>
        /// <param name="__imageName">데이터에 입력된 배경 리소스 값</param>
        /// <returns>현재 사용할 배경</returns>
        public GameSpriteCtrl SetGameBackground(string __imageName)
        {
            // 배경 바꿀때 화면 클리어
            if (currentBG)
            {
                currentBG.gameObject.SetActive(false);
                currentBG.transform.position = Vector3.zero;
                currentBG.spriteRenderer.color = Color.black;
            }

            CleanScreenWithoutBackground();
            if (currentBG != null && !IsDefaultBG() && currentBG.gameObject.activeSelf && currentBackgroundMount != null)
                currentBackgroundMount.EndImage();

            // 이미지가 없는 경우 
            if (!DictBackgroundMounts.ContainsKey(__imageName) || DictBackgroundMounts[__imageName] == null)
            {
                SetDefaultGameBG(); // 없으면 디폴트 배경을 설정하고 진행. 
                return null;
            }

            // 배경과 연결된 ScriptImageMount 불러오기. 
            currentBackgroundMount = DictBackgroundMounts[__imageName];

            // 필요할때 만들기. 
            if (!currentBackgroundMount.CreateRealtimeSprite())
            {
                SetDefaultGameBG();
                return null;
            }

            // 사용횟수 차감 
            currentBackgroundMount.DecreaseUseCount();

            // ! ScriptImageMount의 sprite를 pool의 GameSpriteCtrl과 연결시켜준다. 
            PoolSpriteBG[indexPoolBG].InitSprite(currentBackgroundMount.sprite, currentBackgroundMount.imageKey, currentBackgroundMount.gameScale);

            // currentBG 변경해주기 전에 scale이 반전이었다면 원래대로 일단 돌려준다.
            if (currentBG != null && currentBG.transform.localScale.x < 0f)
                currentBG.transform.localScale = new Vector3(currentBG.gameScale, currentBG.gameScale, 1f);

            currentBG = PoolSpriteBG[indexPoolBG++];

            // index 관련 처리 
            if (indexPoolBG >= PoolSpriteBG.Count)
                indexPoolBG = 0;

            return currentBG;
        }

        public bool IsDefaultBG()
        {
            return currentBG == defaultBG;
        }

        void SetDefaultGameBG()
        {
            currentBG = defaultBG;
            currentBG.gameObject.SetActive(true);
        }

        /// <summary>
        /// 미니컷 처리 
        /// </summary>
        /// <param name="__imageName"></param>
        public void SetMinicutImage(string __imageName)
        {
            // 미니컷 이미지를 보여주기 전에 미니컷이 보여지고 있다면, 
            if (currentMinicut.gameObject.activeSelf && currentMinicutMount != null)
                HideImageMinicut();

            // 없는 이미지를 사용하면, Default를 처리 
            if (!DictMinicutImages.ContainsKey(__imageName) || DictMinicutImages[__imageName] == null)
            {
                SetDefaultImage(); // 없으면 디폴트 배경을 설정하고 진행. 
                ShowMissingComponent("이미지", string.Format("{0} 이미지 없음!", __imageName));
                return;
            }

            // Dictionary에서 가져와서 처리
            currentMinicutMount = DictMinicutImages[__imageName];

            // 미리 Sprite Texture를 생성해두지 않고 필요할땜 만듭니다. 
            // 실패시 
            if (!currentMinicutMount.CreateRealtimeSprite())
                SetDefaultImage();

            currentMinicut.sprite = currentMinicutMount.sprite; // 스프라이트 설정 
            currentMinicut.SetNativeSize();
            currentMinicut.rectTransform.localPosition = new Vector3(currentMinicutMount.offsetX, currentMinicutMount.offsetY, 0);


            if (currentMinicutMount.isResized)  // 게임 크기에 맞게 선조정 되었으니까 따로 이미지를 더 조정할 필요가 없다. 
                currentMinicut.rectTransform.localScale = new Vector3(currentMinicutMount.gameScale, currentMinicutMount.gameScale, 1);
            else // 0.67 곱해준다. 너무 커서..
                currentMinicut.rectTransform.localScale = new Vector3(currentMinicutMount.gameScale * GameConst.IMAGE_SCALE_SMALL, currentMinicutMount.gameScale * GameConst.IMAGE_SCALE_SMALL, 1);

            currentMinicut.gameObject.SetActive(true);
            currentMinicutMount.DecreaseUseCount(); // 이미지 사용 횟수 차감한다.
        }

        /// <summary>
        /// 이미지가 없을때 가짜 이미지 처리 
        /// </summary>
        void SetDefaultImage()
        {
            currentMinicut.sprite = defaultMinicutSprite;
            currentMinicut.rectTransform.localScale = Vector3.one;
            currentMinicut.SetNativeSize();

            currentMinicut.gameObject.SetActive(true);
        }

        /// <summary>
        /// 게임 일러스트 처리 
        /// </summary>
        /// <param name="__imageName"></param>
        /// <returns></returns>
        public GameSpriteCtrl SetGameIllust(string __imageName)
        {
            if (!DictIllusts.ContainsKey(__imageName) || DictIllusts[__imageName] == null)
            {
                //SetDefaultImage(); // 없으면 디폴트 배경을 설정하고 진행. 
                ShowMissingComponent("일러스트", __imageName);
                return null;
            }

            currentIllust = DictIllusts[__imageName];
            return DictIllusts[__imageName];
        }

        /// <summary>
        /// 게임 내 라이브 일러스트 처리
        /// </summary>
        /// <param name="__illustName"></param>
        /// <returns></returns>
        public ScriptLiveMount SetGameLiveIllust(string __illustName)
        {
            // 플레이 중이던 것이 있다면 숨기자
            if (currentLiveIllust != null && currentLiveIllust.liveImageController != null && currentLiveIllust.liveImageController.gameObject.activeSelf)
            {
                currentLiveIllust.liveImageController.HideModel();
                currentLiveIllust.EndIllust();
            }

            if (!DictLiveIllusts.ContainsKey(__illustName) || DictLiveIllusts[__illustName] == null)
                return null;

            currentLiveIllust = DictLiveIllusts[__illustName];
            currentLiveIllust.DecreaseUseCount();

            return currentLiveIllust;
        }

        public ScriptLiveMount SetGameLiveObj(string __objName)
        {
            // 플레이 중이던 것이 있다면 숨기자
            if (currentLiveObj != null && currentLiveObj.liveImageController != null && currentLiveObj.liveImageController.gameObject.activeSelf)
                HideLiveObj();

            if (!DictLiveObjs.ContainsKey(__objName) || DictLiveObjs[__objName] == null)
                return null;

            Debug.Log("SetGameLiveObj");

            currentLiveObj = DictLiveObjs[__objName];

            // 만들어진적이 없으면 생성한다
            if (currentLiveObj.liveImageController == null)
                currentLiveObj.InstantiateCubismModel();

            currentLiveObj.DecreaseUseCount();

            return currentLiveObj;
        }

        /// <summary>
        /// 연결된 ScriptModelMount
        /// </summary>
        ScriptModelMount GetConnectedModelMount(string __speaker)
        {
            return DictModels[DictModelByDress[__speaker]];
        }

        public ScriptImageMount GetEmoticonSprite(string __key)
        {
            if (!DictEmoticon.ContainsKey(__key))
                return null;

            return DictEmoticon[__key];
        }

        #region Add Resource

        /// <summary>
        /// 배경 Mount 추가
        /// </summary>
        public void AddBackgroundMount(string __key, ScriptImageMount __mount)
        {
            if (DictBackgroundMounts.ContainsKey(__key))
                return;

            DictBackgroundMounts.Add(__key, __mount);
        }

        /// <summary>
        /// 에피소드에 추가되는 캐릭터 모델 이름 
        /// </summary>
        public bool AddLoadingModel(string __modelName)
        {
            // 기존 스토리매니저에 모델 정보 없으면 false 리턴하기 
            if (StoryManager.main.GetModelJsonByModelName(__modelName) == null)
                return false;

            if (ListLoadingModel.Contains(__modelName))
                return false;

            ListLoadingModel.Add(__modelName);
            return true;
        }

        /// <summary>
        /// 캐릭터 모델 추가
        /// </summary>
        /// <param name="__speaker">화자</param>
        public void AddGameModel(string __speaker, ScriptModelMount __modelMount)
        {
            if (DictModels.ContainsKey(__speaker))
                return;

            DictModels.Add(__speaker, __modelMount);
        }

        /// <summary>
        /// 일러스트 추가
        /// </summary>
        public void AddIllustSprite(string __key, Sprite __sprite)
        {
            if (indexPoolIllust >= PoolIllust.Count)
                return;

            if (DictIllusts.ContainsKey(__key))
                return;

            PoolIllust[indexPoolIllust].InitSprite(__sprite, __key);
            DictIllusts.Add(__key, PoolIllust[indexPoolIllust]);
            indexPoolIllust++;
        }

        /// <summary>
        /// 얘는 Pooling을 쓰지 않는다. 
        /// </summary>
        public void AddMinicutImage(string __key, ScriptImageMount __mount)
        {
            if (DictMinicutImages.ContainsKey(__key))
                return;

            DictMinicutImages.Add(__key, __mount);
        }

        /// <summary>
        /// 라이브 일러스트 게임매니저에게 준비 요청 
        /// </summary>
        public void AddLiveIllust(ScriptLiveMount __mount)
        {
            if (DictLiveIllusts.ContainsKey(__mount.liveName))
                return;

            DictLiveIllusts.Add(__mount.liveName, __mount);
        }

        public void AddLiveObject(ScriptLiveMount __mount)
        {
            if (DictLiveObjs.ContainsKey(__mount.liveName))
                return;

            DictLiveObjs.Add(__mount.liveName, __mount);
        }

        /// <summary>
        /// 이모티콘 dictionary 추가
        /// </summary>
        public void AddEmoticon(string __key, ScriptImageMount imageMount)
        {
            if (DictEmoticon.ContainsKey(__key))
                return;

            // 얘는 걍 sprite로 넣어준다. 
            DictEmoticon.Add(__key, imageMount);
        }

        /// <summary>
        /// 사운드 정보 추가
        /// </summary>
        /// <param name="groupIndex">0 = BGM, 1 = Voice, 2 = 효과음</param>
        public void AddSound(ScriptSoundMount soundMount, int groupIndex)
        {
            SoundGroup[groupIndex].InitSound(soundMount, soundMount.sound_name);
        }


        #endregion

        #region Resource Hide & Remove


        /// <summary>
        /// 화면상의 모든 캐릭터 비활성화
        /// 여기도 2명이상 쓰면 Dictionary 전부 for문 돌면서 null처리 시켜주기. 배경 변경시 적용
        /// </summary>
        public void HideCharacters()
        {
            standingSpeaker = null;

            for (int i = 0; i < characterModels.Length; i++)
            {
                if (characterModels[i] != null)
                {
                    characterModels[i].HideModel(i, true);
                    characterModels[i] = null;
                }
            }
        }

        /// <summary>
        /// 미니컷 + 라이브오브제, 일러스트 모두 정리
        /// </summary>
        public void HideImageResources()
        {
            // 일반 일러스트 비활성화
            if (currentIllust)
                currentIllust.gameObject.SetActive(false);

            // 라이브 일러스트 비활성화
            if (currentLiveIllust != null && currentLiveIllust.liveImageController != null && currentLiveIllust.liveImageController.gameObject.activeSelf)
            {
                currentLiveIllust.liveImageController.HideModel();
                currentLiveIllust.EndIllust();
            }

            // 미니컷 비활성화
            HideImageMinicut();

            // 라이브 오브제 비활성화
            HideLiveObj();
        }

        /// <summary>
        /// 배경 제외하고 싹다 정리
        /// </summary>
        public void CleanScreenWithoutBackground()
        {
            ViewGame.main.userCall = false;
            ViewGame.main.HIdePhoneImage();

            HideImageResources();

            HideCharacters();
            ViewGame.main.HideBubbles();
            ScreenEffectManager.main.RemoveAllScreenEffect();
        }

        public void HideLiveObj()
        {
            if (currentLiveObj != null && currentLiveObj.liveImageController != null)
            {
                currentLiveObj.liveImageController.HideModel();
                currentLiveObj.EndIllust();
                currentLiveObj = null;
            }
        }

        /// <summary>
        /// 이미지 리소스 제거 
        /// </summary>
        public void HideImageMinicut()
        {
            Debug.Log("Hide Image Called");
            currentMinicut.gameObject.SetActive(false);

            currentMinicut.color = new Color(currentMinicut.color.r, currentMinicut.color.g, currentMinicut.color.b, 1f);
            currentMinicut.transform.localScale = Vector3.one;
            currentMinicut.transform.localEulerAngles = Vector3.zero;

            // 기본이미지가 아닌 경우 사용횟수 차감 기록에 따른 Destory 처리
            if (currentMinicut.sprite != defaultImage && currentMinicutMount != null)
            {
                currentMinicutMount.EndImage();

                // 연결고리를 끊자. 
                currentMinicutMount = null;
                currentMinicut.sprite = null;
            }
        }

        /// <summary>
        /// 다 쓴 배경 제거 
        /// </summary>
        public void RemoveBackgroundFromDicionary(string __bgName)
        {
            if (!DictBackgroundMounts.ContainsKey(__bgName))
                return;

            // Page에서 제거!
            currentPage.RemoveImageMount(DictBackgroundMounts[__bgName]);
        }

        /// <summary>
        /// 다쓴 이미지를 Dictionary에서 제거하기
        /// </summary>
        public void RemoveImageFromDictionary(string __imageName)
        {
            if (!DictMinicutImages.ContainsKey(__imageName))
                return;

            // Page에서 제거!
            currentPage.RemoveImageMount(DictMinicutImages[__imageName]);

            // 딕셔너리에서 리무브!
            // 잘가요!
            DictMinicutImages.Remove(__imageName);
        }

        public void RemoveEmoticonFromDictionary(string emoticonExpression)
        {
            if (!DictEmoticon.ContainsKey(emoticonExpression))
                return;

            currentPage.RemoveImageMount(DictEmoticon[emoticonExpression]);
            DictEmoticon.Remove(emoticonExpression);
        }


        /// <summary>
        /// 다 쓴 liveObj 제거
        /// </summary>
        public void RemoveLiveObjFromDictionary(string liveObjName)
        {
            if (!DictLiveObjs.ContainsKey(liveObjName))
                return;

            currentPage.RemoveLiveObjectMount(DictLiveObjs[liveObjName]);
            DictLiveObjs.Remove(liveObjName);
        }

        public void RemoveLiveIllustFromDictionary(string liveIllustName)
        {
            if (DictLiveIllusts.ContainsKey(liveIllustName))
                return;

            DictLiveIllusts.Remove(liveIllustName);
        }

        #endregion


        #endregion


        public static void ShowMissingComponent(string __template, string __data)
        {
            // 스킵중에는 플레이 도중에 뜨던 popup이 뜨지 않도록 한다.
            if (main.useSkip)
                return;

            if(UserManager.main.CheckAdminUser())
                SystemManager.ShowMessageAlert(string.Format("{0} : {1}", __template, __data), false);
        }

        /// <summary>
        /// 일러스트 획득 팝업
        /// </summary>
        public void ShowAchieveIllust(string illustName)
        {
            // 일러스트 획득 팝업을 꺼둔 경우 일러스트 획득 팝업을 띄우지 않는다
            if (PlayerPrefs.GetInt(GameConst.ILLUST_POPUP) != 1)
                return;

            Debug.Log("ShowAchieveIllust : " + illustName);

            PopupBase popup = PopupManager.main.GetPopup(GameConst.POPUP_ACHIEVEMENT_ILLUST);
            if (popup == null)
            {
                Debug.LogError("No Popup");
                return;
            }

            popup.Data.SetImagesSprites(spriteIllustPopup);
            popup.Data.SetLabelsTexts(string.Format(SystemManager.GetLocalizedText("5085"), illustName));
            PopupManager.main.ShowPopup(popup, true, false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="__nextEpisodeID"></param>
        public void ShowGameEnd(string __nextEpisodeID)
        {
            Debug.Log("ShowGameEnd :: " + __nextEpisodeID);

            // AdManager.main.HideBanner();

            useSkip = false;
            isPlaying = false; // 게임 플레이 정지.

            // * 통신이 겹치는 문제로 인해서 코루틴으로 변경
            StartCoroutine(RoutineFinishGame(__nextEpisodeID));
        }

        IEnumerator RoutineFinishGame(string __nextEpisodeID)
        {

            // 안전을 위해..
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            
            
            Firebase.Analytics.FirebaseAnalytics.LogEvent("EpisodeEnd", new Firebase.Analytics.Parameter("project_id", StoryManager.main.CurrentProjectID)
            , new Firebase.Analytics.Parameter("episode_id", StoryManager.main.CurrentEpisodeID));
            


            EpisodeData nextEpisodeData = null; // 다음 에피소드 데이터

            // * 이동 컬럼에 아무 데이이터가 없는 경우와, 있는 경우 
            if (string.IsNullOrEmpty(__nextEpisodeID))
            {
                // 이동컬럼에 아무것도 없는 경우는 리스트상의 다음 에피소드를 지정한다. 
                __nextEpisodeID = string.Empty;

                // 챕터의 경우에만 처리. 사이드 엔딩은 진행하지 않음 
                if (currentEpisodeData.episodeType == EpisodeType.Chapter)
                {
                    nextEpisodeData = StoryManager.GetNextRegularEpisodeData(currentEpisodeData);

                    if (nextEpisodeData != null && nextEpisodeData.isValidData)
                        __nextEpisodeID = nextEpisodeData.episodeID;
                }
            }
            else
            { // 이동 컬럼 데이터 있음

                // #, @이 붙은 경우에 대한 처리 
                if (__nextEpisodeID.Contains(GameConst.TAG_TARGET_EPISODE))
                    __nextEpisodeID = __nextEpisodeID.Replace(GameConst.TAG_TARGET_EPISODE, "");

                if (__nextEpisodeID.Contains(GameConst.TAG_TARGET_EPISODE2))
                    __nextEpisodeID = __nextEpisodeID.Replace(GameConst.TAG_TARGET_EPISODE2, "");

                nextEpisodeData = StoryManager.GetNextFollowingEpisodeData(__nextEpisodeID);
            }

            // scene Id값 갱신하고 통신 완료까지 잠시 대기
            UserManager.main.UpdateSceneIDRecord(currentSceneId);
            SystemManager.ShowNetworkLoading();
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());

            // 에피소드 완료까지 통신 대기
            NetworkLoader.main.UpdateEpisodeCompleteRecord(nextEpisodeData);
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            SystemManager.HideNetworkLoading();
            
            
            // 게임 종료되었으면 사운드도 다 종료해주고, 가끔 스킵해도 배경음이 남아있는 경우도 있으니 Mute까지 해준다
            foreach (GameSoundCtrl sc in SoundGroup)
            {
                sc.StopAudioClip();
                sc.MuteAudioClip();
            }            
            
            
            
            // * 여기까지 통신 완료하고 후 처리 진행...!!! 2022.01.27 팝업 순서를 변경한다. 
            // * 사이드 => 엔딩 => 미션 => 첫클리어 보상의 순서. 
            
            
            // * 1. 에피소드 클리어 후 사이드 해금 체크 
            #region 사이드
            
            Debug.Log("### 사이드 에피소드 오픈 체크 시작 ###");
            
            JsonData sideData = UserManager.main.GetNodeUnlockSide();
            
            if(sideData != null) 
                Debug.Log(JsonMapper.ToStringUnicode(sideData));
            
            // 사이드 해금이 있는 경우
            if (sideData != null && sideData.Count > 0)
            {
                Debug.Log("### 해금된 사이드 에피소드 존재함!! :: " + sideData.Count);
                
                PopupBase sidePopup = PopupManager.main.GetPopup(GameConst.POPUP_SIDE_ALERT);

                for (int i = 0; i < sideData.Count; i++)
                {
                    string unlockSideID = SystemManager.GetJsonNodeString(sideData[i], CommonConst.COL_EPISODE_ID);
                    Debug.Log("### unlockSideID : " + unlockSideID);

                    for (int j = 0; j < StoryManager.main.SideEpisodeList.Count; j++)
                    {
                        if (StoryManager.main.SideEpisodeList[j].episodeID == unlockSideID)
                        {
                            sidePopup.Data.SetLabelsTexts(SystemManager.GetLocalizedText("6234"), StoryManager.main.SideEpisodeList[j].episodeTitle);
                            sidePopup.Data.imageURL = StoryManager.main.SideEpisodeList[j].popupImageURL;
                            sidePopup.Data.imageKey = StoryManager.main.SideEpisodeList[j].popupImageKey;
                            PopupManager.main.ShowPopup(sidePopup, true, false);
                            break;
                        }
                    }
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.Log("### 사이드 에피소드 오픈 체크 종료 ###");
                        
            #endregion
            
            // * 2. 열린 엔딩 체크 
            #region 엔딩
            // * 2022.03.17 엔딩은 에피소드 종료화면에서 처리하기로 변경 
            
            // Debug.Log("### 엔딩 오픈 체크 시작 ###");

            // 다음 에피소드가 엔딩인 경우
            /*
            if (nextEpisodeData != null && nextEpisodeData.episodeType == EpisodeType.Ending)
            {

                Debug.Log("<color=yellow>Ending Alert open </color>");

                PopupBase endingPopup = PopupManager.main.GetPopup(GameConst.POPUP_ENDING_ALERT);

                string endingType = string.Empty;

                if (nextEpisodeData.endingType == LobbyConst.COL_HIDDEN)
                    endingType = SystemManager.GetLocalizedText("5087");
                else
                    endingType = SystemManager.GetLocalizedText("5088");

                endingPopup.Data.SetLabelsTexts(endingType, string.Format(SystemManager.GetLocalizedText("6063"), nextEpisodeData.episodeTitle));
                endingPopup.Data.imageURL = nextEpisodeData.squareImageURL;
                endingPopup.Data.imageKey = nextEpisodeData.squareImageKey;


                PopupManager.main.ShowPopup(endingPopup, true, false);
                yield return new WaitForSeconds(0.1f);

                Debug.Log("<color=yellow>Ending Alert close </color>");
            }            
            
            Debug.Log("### 엔딩 오픈 체크 종료 ###");
            */
            #endregion
            

            // * 3. 미션 해금
            UserManager.main.ShowCompleteMissionByEpisode(false);
            
            // * 첫 클리어 보상 
            #region 첫 클리어 보상 
            JsonData firstReward = UserManager.main.GetNodeFirstClearResult();  // 최초 보상 노드 가져오기 
            if(firstReward != null && firstReward.Count > 0) {
                // 최초 보상 있으면 팝업 호출
                Debug.Log(">> First Reward exists : " + JsonMapper.ToStringUnicode(firstReward[0]));
                
                PopupBase p = PopupManager.main.GetPopup(GameConst.POPUP_EPISODE_FIRST_REWARD);
                if(p == null) {
                    Debug.LogError("First reward popup is null");
                }
                else {
                    p.Data.SetContentJson(firstReward[0]); // 첫번째 로우 (항상 1개만 온다)
                    PopupManager.main.ShowPopup(p, true, false); // 보여주기.  (queue 사용으로 변경)                    
                }
                
                yield return new WaitForSeconds(0.1f);

            } // 첫 클리어 보상 팝업 끝 
            #endregion
            

            Debug.Log("Signal Send for Episode END !!!!!!");

            // * 엔딩 연출 끝나면 아래 진행 (없으면 그냥 진행)
            Signal.Send(LobbyConst.STREAM_GAME, GameConst.SIGNAL_NEXT_DATA, nextEpisodeData, string.Empty); // 다음 에피소드 전달 
            Signal.Send(LobbyConst.STREAM_GAME, GameConst.SIGNAL_EPISODE_END, currentEpisodeData, string.Empty); // 현재 에피소드 전달
        }

        #endregion
        
        #region 이어하기 플레이, 선택지 루트 관련 메소드 추가 
        
        /// <summary>
        /// 에피소드 시작전 selection progress 노드 초기화 
        /// </summary>
        /// <param name="__episodeID"></param>
        public void InitSelectionProgressRoute() {
            
            // * 재귀 선택지 관련 처리 때문에 이 메소드 추가함. 
            // * 이어하기에서 지나친 선택지를 여기에서 저장해준다. 
            resumePlaySelectionJSON = JsonMapper.ToObject("[]"); // 배열로 만들어놓고 시작한다. 
        }
        
        /// <summary>
        /// 이어하기에서 선택지 지나쳤는지 체크하기 
        /// </summary>
        /// <param name="__targetData">선택지 정보</param>
        /// <returns></returns>
        public bool CheckResumeSelectionPassed(JsonData __targetData) {
            
            // 동일한 경로 있었으면 return true.
            for(int i=0; i<resumePlaySelectionJSON.Count;i++) {
                if(SystemManager.GetJsonNodeString(__targetData, "target_scene_id") == SystemManager.GetJsonNodeString(resumePlaySelectionJSON[i], "target_scene_id")) {
                  return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// 경로 추가 
        /// </summary>
        /// <param name="__j"></param>
        public void AddResumeSelectionRoute(JsonData __j) {
            resumePlaySelectionJSON.Add(__j);
            
            Debug.Log("### AddResumeSelectionRoute : " + JsonMapper.ToJson(resumePlaySelectionJSON));
        }
        
        #endregion
        
        
    }

}