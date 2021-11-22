using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using LitJson;

namespace PIERStory
{
    public class GameManager : MonoBehaviour
    {
        /*
         * 현재 아직 미생성한 클래스들에 대해서 변수들을 주석해놨음
         */

        public static GameManager main = null;      // singleton
        public static List<GameSelectionCtrl> ListAppearSelection = new List<GameSelectionCtrl>();

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
        public ScriptPage currentPage;  // 생성된 현재 페이지

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
        public GameSpriteCtrl defaultImage; // 디폴트 이미지
        public Transform illustPillar;        // live일러스트, liveObject 부모 transform
        public RawImage liveObjectTexture;


        [Space][Header("스탠딩 모델")]
        public ScriptModelMount standingSpeaker = null;
        public GameModelCtrl[] characterModels = new GameModelCtrl[3];  // 0 : 좌, 1:중앙, 2:우
        public Sprite defaultCharacterSprite;
        public GameObject infoPrefab;           // 더미 캐릭터 정보 담은 prefab
        public Transform modelPillar;           // 캐릭터 모델의 부모 transform.


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
            get { return currentPage.GetNextRowWithoutIncrement(); }
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
                SystemManager.ShowAlert("불완전한 선택지가 존재하여 이어하기를 진행할 수 없습니다.");
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
                return __row.speaker;
            // 대체 이름이 존재하면 대체 이름 return
            else
                return __row.controlAlternativeName;
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

            dressProgress = UserManager.main.GetNodeDressProgress();

            // DictModelByDress 를 수집하고 유저 데이터 기반해서 재설정
            for (int i = 0; i < dressProgress.Count; i++)
            {
                if (DictModelByDress.ContainsKey(SystemManager.GetJsonNodeString(dressProgress[i], GameConst.COL_SPEAKER)))
                    DictModelByDress[SystemManager.GetJsonNodeString(dressProgress[i], GameConst.COL_SPEAKER)] = SystemManager.GetJsonNodeString(dressProgress[i], GameConst.COL_MODEL_NAME);
            }

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

            //inGameMenu.ChangeSkipIcon(skipable);
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
        /// 나가기 
        /// </summary>
        public void EndGame()
        {
            Debug.Log("EndGame");
            isPlaying = false;

            // 네트워크 창을 띄우고. 로비 씬으로 돌아가야한다.
            SystemManager.ShowNetworkLoading();
            IntermissionManager.isMovingLobby = true;
            SceneManager.LoadSceneAsync("Intermission", LoadSceneMode.Single).allowSceneActivation = true;
        }

        /// <summary>
        /// 재시작 하기 
        /// </summary>
        public void RetryPlay()
        {
            Debug.Log("RetryPlay");
            isPlaying = false;

            // 씬 로드 
            IntermissionManager.isMovingLobby = false;
            SceneManager.LoadSceneAsync("Intermission", LoadSceneMode.Single).allowSceneActivation = true;
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

        }

        /// <summary>
        /// 일러스트 획득 팝업
        /// </summary>
        public void ShowAchieveIllust(string illustName)
        {
            
        }


        public void ShowGameEnd(string __nextEpisodeId)
        {

        }

        #endregion
    }

}
