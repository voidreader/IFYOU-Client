using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

namespace PIERStory
{
    public class ScriptPage : MonoBehaviour
    {
        public int playRow = 0; // 현재 플레이 되는 행!

        public List<ScriptRow> ListRows;
        public List<ScriptImageMount> ListImageMount = new List<ScriptImageMount>();        // 배경,이미지,일러스트,이모티콘 친구들
        public List<ScriptModelMount> ListModelMount = new List<ScriptModelMount>();        // 캐릭터 모델 Live2D 친구들
        public List<ScriptLiveMount> ListLiveIllustMount = new List<ScriptLiveMount>();     // Live2D 일러스트
        public List<ScriptLiveMount> ListLiveObjectMount = new List<ScriptLiveMount>();     // Live2D 오브제
        public List<ScriptBubbleMount> ListBubbleMount = new List<ScriptBubbleMount>();     // 말풍선 친구들
        public List<ScriptSoundMount> ListSoundMount = new List<ScriptSoundMount>();        // bgm, voice, se(Sound effect)

        JsonData pageData = null;
        JsonData scriptData = null;

        int pageResourceCount = 0;          // 페이지에서 불러오는 모든 리소스 카운트
        int pageImageResourceCount = 0;     // 페이지 이미지 리소스 카운트 (미니컷, 일러스트, 이모티콘) 
        int pageBubbleResourceCount = 0;    // 페이지 말풍선 리소스 카운트 
        int pageModelCount = 0;             // 페이지 모델 카운트
        int pageLiveIllustCount = 0;        // 페이지 라이브 일러스트 카운트
        int pageLiveObjectCount = 0;        // 페이지 라이브 오브젝트 카운트
        int pageSoundResourceCount = 0;     // 페이지 사운드 리소스 카운트


        public bool isPageImagesInitialized = false; // 페이지 초기화 완료 여부

        static readonly string COL_SOUND_EFFECT = "se";

        /// <summary>
        /// 페이지 초기화
        /// </summary>
        /// <param name="__data"></param>
        public void InitPage(JsonData __data)
        {
            #region 변수 초기화 

            playRow = 0;
            pageData = __data; // JsonData 받고, 
            scriptData = pageData[GameConst.NODE_SCRIPT]; // 스크립트 데이터만 따로 뽑아낸다.

            ListRows = new List<ScriptRow>(); // 리스트 새로 생성한다.

            ListImageMount.Clear();
            ListModelMount.Clear();
            ListBubbleMount.Clear();
            ListSoundMount.Clear();
            ListLiveIllustMount.Clear();
            ListLiveObjectMount.Clear();


            #endregion

            // 스크립트 기반으로 Row를 생성한다. 
            // 매번 New 하지 말고 Pooling 하는 것도 고려해보자. 
            for (int i = 0; i < scriptData.Count; i++)
            {
                ScriptRow row = new ScriptRow(scriptData[i], OnRowInitialized); // 각 Row에 해당하는 ScriptRow를 생성합니다. (생성자에서 초기화 호출함)
                ListRows.Add(row);
            }
            // ScriptRow 생성 끝. 


            // ViewLoading에서 Progressor MaxValue 체크를 위해 미리 몇개를 불러올건지 설정한다.
            SetResourceCounts();

            // 체크 
            Debug.Log(string.Format("Total Image[{0}], Model[{1}], Live illust[{2}], Live Object[{3}] Bubble[{4}], Sound[{5}]", pageImageResourceCount, pageModelCount, pageLiveIllustCount, pageLiveObjectCount, pageBubbleResourceCount, pageSoundResourceCount));

            // 다운로드와 로딩 처리 시작.
            StartCoroutine(RoutineLoadingResource());
        }

        /// <summary>
        /// 리소스 카운트 미리 세팅하기 
        /// </summary>
        void SetResourceCounts()
        {
            // 각 리소스 정보는 DB에서 Distinct로 날아오기 때문에 중복되지 않는다. 
            // 이미지 
            pageImageResourceCount = 0;
            pageImageResourceCount += pageData[GameConst.TEMPLATE_BACKGROUND].Count;            // 배경
            pageImageResourceCount += pageData[GameConst.TEMPLATE_IMAGE].Count;                 // 이미지
            pageImageResourceCount += pageData[GameConst.TEMPLATE_ILLUST].Count;                // 일러스트 
            pageImageResourceCount += pageData[StoryManager.BUBBLE_VARIATION_EMOTICON].Count;   // 이모티콘 

            // 사운드
            pageSoundResourceCount = 0;
            pageSoundResourceCount += pageData[GameConst.TEMPLATE_BGM].Count;   // BGM
            pageSoundResourceCount += pageData[GameConst.COL_VOICE].Count;      // 음성
            pageSoundResourceCount += pageData[COL_SOUND_EFFECT].Count;         // 효과음

            // Live2D 캐릭터 Model
            pageModelCount = 0;
            pageLiveIllustCount = 0;
            pageLiveObjectCount = 0;

            // 모델은 mount를 먼저 생성만 해놓는다.
            CollectAllEpisodeModels();
            pageModelCount = ListModelMount.Count;
            pageLiveObjectCount = ListLiveObjectMount.Count;
            pageLiveIllustCount = ListLiveIllustMount.Count;


            // 말풍선 - StoryManager에서 받아온다. 
            pageBubbleResourceCount = StoryManager.main.bubbleID_Dictionary.Keys.Count;

            // 합산하기!
            pageResourceCount = pageImageResourceCount + pageModelCount + pageLiveIllustCount + pageLiveObjectCount + pageBubbleResourceCount + pageSoundResourceCount;
        }

        /// <summary>
        /// 리소스 로딩 시작 
        /// </summary>
        /// <returns></returns>
        IEnumerator RoutineLoadingResource()
        {
            Debug.Log("RoutineLoadingResource Start");

            // 동시진행
            StartCoroutine(RoutineLoadingLiveObjects());                                    // 라이브 오브제
            StartCoroutine(RoutineLoadingModels());                                         // 캐릭터 모델 
            StartCoroutine(RoutineLoadingImage(GameConst.TEMPLATE_BACKGROUND));             // 배경 
            StartCoroutine(RoutineLoadingImage(GameConst.TEMPLATE_ILLUST));                 // 일러스트 
            StartCoroutine(RoutineLoadingLiveIllusts());                                    // 라이브 일러스트

            StartCoroutine(RoutineLoadingBubble());                                         // 말풍선 
            StartCoroutine(RoutineLoadingImage(GameConst.TEMPLATE_IMAGE));                  // 이미지
            StartCoroutine(RoutineLoadingImage(StoryManager.BUBBLE_VARIATION_EMOTICON));    // 이모티콘 


            StartCoroutine(RoutineLoadingSound(GameConst.TEMPLATE_BGM));                    // 배경음
            StartCoroutine(RoutineLoadingSound(GameConst.COL_VOICE));                       // 음성 
            StartCoroutine(RoutineLoadingSound(COL_SOUND_EFFECT));                          // SE

            yield return new WaitUntil(() => GetCurrentLoadingCount() <= 0);
            yield return new WaitForSeconds(0.5f);

            Debug.Log("RoutineLoadingResource End");
            StartCoroutine(RoutineLoadingPostProcess());
        }

        /// <summary>
        /// 로딩 완료 후 후처리 
        /// </summary>
        /// <returns></returns>
        IEnumerator RoutineLoadingPostProcess()
        {
            Debug.Log("<color=white>RoutineLoadingPostProcess Start</color>");

            #region ListImageMount 처리 

            for (int i = 0; i < ListImageMount.Count; i++)
            {
                if (!ListImageMount[i].isMounted)
                    continue;

                // 각 마운트 이미지를 게임매니저에게 전달
                if (ListImageMount[i].template.Equals(GameConst.TEMPLATE_BACKGROUND) || ListImageMount[i].template.Equals(GameConst.TEMPLATE_MOVEIN)) // ! 배경 
                {
                    GameManager.main.AddBackgroundMount(ListImageMount[i].imageName, ListImageMount[i]);

                    // count 추가
                    ListImageMount[i].SetImageUseCount(GetBackgroundUseCount(ListImageMount[i].imageName));

                }
                else if (ListImageMount[i].template == GameConst.TEMPLATE_IMAGE) // ! 미니컷
                {
                    // ! 미니컷은 ImageMount를 전달한다. 
                    GameManager.main.AddMinicutImage(ListImageMount[i].imageName, ListImageMount[i]);
                    // count 추가 
                    ListImageMount[i].SetImageUseCount(GetSpecificImageUsageCount(ListImageMount[i].imageName));
                }
                else if (ListImageMount[i].template == GameConst.TEMPLATE_ILLUST)
                    GameManager.main.AddIllustSprite(ListImageMount[i].imageName, ListImageMount[i].sprite);
                else
                {
                    // 이모티콘 추가
                    GameManager.main.AddEmoticon(ListImageMount[i].imageName, ListImageMount[i]);

                    // count 추가
                    ListImageMount[i].SetImageUseCount(GetEmoticonUseCount(ListImageMount[i].imageName));
                }
            } // end of ListImageMount 

            #endregion

            #region Bubble Mount 

            for (int i = 0; i < ListBubbleMount.Count; i++)
            {
                if (!ListBubbleMount[i].isMounted)
                    continue;

                while (ListBubbleMount[i].isMounted && ListBubbleMount[i].sprite == null)
                    yield return null;

                // 버블매니저에 추가해주기.
                BubbleManager.main.AddBubbleSprite(ListBubbleMount[i].spriteId, ListBubbleMount[i].sprite);
            }


            #endregion

            #region 캐릭터 모델, LiveObject 처리 

            // Live2D 오브제 처리 
            for (int i = 0; i < ListLiveObjectMount.Count; i++)
            {
                if (!ListLiveObjectMount[i].isMounted)
                    continue;

                // 게임매니저한테 정상적으로 불러온거 전달해준다. 
                GameManager.main.AddLiveObject(ListLiveObjectMount[i]);
                ListLiveObjectMount[i].SetLiveImageUseCount(GetLiveObjUseCount(ListLiveObjectMount[i].liveName));
            }
            // Live2D 오브제 처리 끝


            for (int i = 0; i < ListLiveIllustMount.Count; i++)
            {
                if (!ListLiveIllustMount[i].isMounted)
                    continue;

                // 게임매니저에게 전달
                GameManager.main.AddLiveIllust(ListLiveIllustMount[i]);
                ListLiveIllustMount[i].SetLiveImageUseCount(GetLiveIllustUseCount(ListLiveIllustMount[i].liveName));
            }


            // 캐릭터 모델 처리 
            for (int i = 0; i < ListModelMount.Count; i++)
            {
                if (!ListModelMount[i].isMounted)
                    continue;

                // GameManaer에게 추가해준다. 
                GameManager.main.AddGameModel(ListModelMount[i].originModelName, ListModelMount[i]);
            }

            GameManager.main.CollectDistinctSpeaker(); // 의상 시스템 연결고리 설정

            yield return null;

            for (int i = 0; i < ListModelMount.Count; i++)
                ListModelMount[i].HideModel();

            #endregion

            #region ListSoundMount

            Debug.Log("<color=white>MountValidation Sound Check</color>");

            for (int i = 0; i < ListSoundMount.Count; i++)
            {
                if (!ListSoundMount[i].isMounted)
                    continue;

                if (ListSoundMount[i].template.Contains(GameConst.TEMPLATE_BGM))
                    GameManager.main.AddSound(ListSoundMount[i], 0);
                else if (ListSoundMount[i].type.Contains(GameConst.COL_VOICE))
                    GameManager.main.AddSound(ListSoundMount[i], 1);
                else if (ListSoundMount[i].type.Contains(COL_SOUND_EFFECT))
                    GameManager.main.AddSound(ListSoundMount[i], 2);
            }

            #endregion

            Debug.Log("<color=white>RoutineLoadingPostProcess Done</color>");

            isPageImagesInitialized = true; // 다 했으면 초기화 했다고 알린다.

        }

        /// <summary>
        /// 라이브 오브젝트 모델 로딩
        /// </summary>
        IEnumerator RoutineLoadingLiveObjects()
        {
            int checker = 0;

            for (int i = 0; i < ListLiveObjectMount.Count; i++)
            {
                checker = pageLiveObjectCount;
                ListLiveObjectMount[i].SetModelDataFromStoryManager();

                yield return new WaitUntil(() => checker > pageLiveObjectCount);

                yield return null;
                yield return null;
                yield return null;

                if (ListLiveObjectMount[i].isMounted && ListLiveObjectMount[i].liveImageController != null)
                    ListLiveObjectMount[i].liveImageController.HideModel();
            }
        }

        /// <summary>
        /// 라이브 일러스트 로딩
        /// </summary>
        IEnumerator RoutineLoadingLiveIllusts()
        {
            int checker = 0;

            for (int i = 0; i < ListLiveIllustMount.Count; i++)
            {
                checker = pageLiveIllustCount;
                ListLiveIllustMount[i].SetModelDataFromStoryManager();

                yield return new WaitUntil(() => checker > pageLiveIllustCount);

                yield return null;
                yield return null;
                yield return null;

                if (ListLiveIllustMount[i].isMounted && ListLiveIllustMount[i].liveImageController != null)
                    ListLiveIllustMount[i].liveImageController.HideModel();

            }
        }

        /// <summary>
        /// 캐릭터 모델 로딩 
        /// </summary>
        IEnumerator RoutineLoadingModels()
        {
            int checker = 0;

            for (int i = 0; i < ListModelMount.Count; i++)
            {
                checker = pageModelCount;
                ListModelMount[i].SetModelDataFromStoryManager();

                yield return new WaitUntil(() => checker > pageModelCount); // 모델 하나씩.. 
            }

            yield return null;

            // 모델들 한테 충돌체 주고 키 체크 
            for (int i = 0; i < ListModelMount.Count; i++)
            {
                if (ListModelMount[i].modelController != null)
                    ListModelMount[i].SetBoxColliders();
            }

            // 잠깐.. 충돌체에게 시간을 주자.. 
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            // 다 했으면, 키 체크 안당한 모델까지 해서 RemoveColliders => 굳이 Colliders를 계속 갖고 있을 이유는 없으니까. 
            for (int i = 0; i < ListModelMount.Count; i++)
            {
                if (ListModelMount[i].modelController != null)
                    ListModelMount[i].modelController.RemoveColliders();
            }

            Debug.Log("Character Model Loading Done");
        }

        /// <summary>
        /// 이미지 로딩하기 (종류별)
        /// </summary>
        /// <param name="__col">배경, 이미지, 일러스트, 이모티콘 구분</param>
        IEnumerator RoutineLoadingImage(string __col)
        {
            int checker = 0;
            JsonData currentData = null;

            // 이미지 리소스 순차적으로 진행한다. 
            currentData = pageData[__col];
            for (int i = 0; i < currentData.Count; i++)
            {
                checker = pageImageResourceCount;
                ListImageMount.Add(new ScriptImageMount(__col, currentData[i], OnImageMountInitialized));

                // 현재 이미지 로딩 완료할때까지 대기. 
                yield return new WaitUntil(() => checker > pageImageResourceCount);
            }

            Debug.Log(string.Format("Image [{0}] loading done.", __col));
        }

        /// <summary>
        /// 사운드 로딩하기 (종류별)
        /// </summary>
        /// <param name="__col">BGM, VOICE, SE</param>
        IEnumerator RoutineLoadingSound(string __col)
        {
            // 이미지랑 거의 유사하다.

            int checker = 0;
            JsonData currentData = null;

            // 이미지 리소스 순차적으로 진행한다. 
            currentData = pageData[__col];
            for (int i = 0; i < currentData.Count; i++)
            {
                checker = pageSoundResourceCount;
                ListSoundMount.Add(new ScriptSoundMount(__col, currentData[i], OnSoundMountInitialized));

                // 현재 사운드 로딩 완료할때까지 대기. 
                yield return new WaitUntil(() => checker > pageSoundResourceCount);
            }

            Debug.Log(string.Format("Sound [{0}] loading done.", __col));
        }

        /// <summary>
        /// 말풍선 리소스 수집 
        /// </summary>
        IEnumerator RoutineLoadingBubble()
        {
            Debug.Log("<color=yellow>Collecting Bubble Resources</color>");
            string currentURL = string.Empty;
            string currentKEY = string.Empty;

            foreach (string id in StoryManager.main.bubbleID_Dictionary.Keys)
            {
                currentURL = StoryManager.main.bubbleID_Dictionary[id];
                currentKEY = StoryManager.main.bubbleURL_Dictionary[currentURL];

                ListBubbleMount.Add(new ScriptBubbleMount(id, currentURL, currentKEY, OnBubbleMountInitialized)); // 이건 그냥 한번에 불러보자.. 
            }

            yield return new WaitUntil(() => pageBubbleResourceCount <= 0);

            Debug.Log("Bubble loading done");
        }

        #region Collect functions

        /// <summary>
        /// 에피소드안의 모든 캐릭터 모델 수집 
        /// </summary>
        void CollectAllEpisodeModels()
        {
            Debug.Log("<color=yellow>Collecting Model Resources</color>");

            for (int i = 0; i < ListRows.Count; i++)
            {
                if (string.IsNullOrEmpty(ListRows[i].resource_key))
                    continue;

                // 모델 리소스 수집 
                if (ListRows[i].IsSpeakable)
                    CollectDemandedModelResource(ListRows[i]);

                // 라이브 오브제 
                if (ListRows[i].template.Equals(GameConst.TEMPLATE_LIVE_OBJECT))
                    CollectDemandedLiveObjectResource(ListRows[i]);

                // 라이브 일러스트 
                if (ListRows[i].template.Equals(GameConst.TEMPLATE_ILLUST))
                    CollectDemandedLiveIllustResource(ListRows[i]);
            }

            // 의상 템플릿 수집
            for (int i = 0; i < ListRows.Count; i++)
            {
                if (ListRows[i].IsValidDress)
                    CollectDemandedDressModelResource(ListRows[i]);
            }

            // 유저 드레스 Progress의 모델 수집 
            CollectUserDressProgressModelResource(UserManager.main.GetNodeDressProgress());
        }


        /// <summary>
        /// 모델 리소스 수집하기
        /// </summary>
        /// <param name="__row"></param>
        void CollectDemandedModelResource(ScriptRow __row)
        {
            if (GameManager.main.AddLoadingModel(__row.speaker))
            {
                ScriptModelMount mounter = new ScriptModelMount(__row.rowData, OnModelMountInitialized, this);
                ListModelMount.Add(mounter);
            }
        }

        /// <summary>
        /// 유저 의상 정보 모델이 현재 스크립트에 화자로서 등장하는지 체크 
        /// </summary>
        bool CheckDressProgressSpeakerExistsInScript(string __speaker)
        {
            for (int i = 0; i < ListModelMount.Count; i++)
            {
                if (ListModelMount[i].speaker == __speaker)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 유저 의상 저장 정보의 모델 리소스 수집 
        /// </summary>
        void CollectUserDressProgressModelResource(JsonData __progress)
        {
            string speaker = string.Empty;
            string dress_id = string.Empty;
            string targetModelName = string.Empty;

            for (int i = 0; i < __progress.Count; i++)
            {
                speaker = SystemManager.GetJsonNodeString(__progress[i], GameConst.COL_SPEAKER);
                dress_id = SystemManager.GetJsonNodeString(__progress[i], GameConst.COL_DEFAULT_DRESS_ID);

                // 2021.06.15 스크립트에서 등장하지 않는 인물도 로드하는것을 확인해서 
                // 실제 등장 인물에 대한 데이터만 불러오도록 수정함!
                if (!CheckDressProgressSpeakerExistsInScript(speaker))
                    continue;


                // 화자와 의상 ID로 타겟 모델 - 일치하는 모델 이름을 찾는다.
                targetModelName = StoryManager.main.GetTargetDressModelNameByDressID(speaker, dress_id);
                // 없으면 문제인데.? 
                if (string.IsNullOrEmpty(targetModelName))
                {
                    Debug.Log(">> Wrong targetModelName, is null");
                    continue;
                }

                // 중복 로딩 체크 후 없으면 로딩 
                if (GameManager.main.AddLoadingModel(targetModelName))
                {
                    Debug.Log(string.Format("Model Loading Begin {0}", targetModelName));
                    ScriptModelMount mounter = new ScriptModelMount(targetModelName, speaker, OnModelMountInitialized, this);
                    ListModelMount.Add(mounter);
                }

            } // end of for i
        }

        /// <summary>
        /// 의상 템플릿을 통한 모델 리소스 수집
        /// </summary>
        /// <param name="__row"></param>
        void CollectDemandedDressModelResource(ScriptRow __row)
        {
            string dress_name = __row.script_data; // 의상 이름
            string speaker = __row.speaker; // // row.speaker는 의상모델 이름이다. 

            string targetModelName = string.Empty;

            // 의상이름으로 모델 이름이 있는지 찾아보자 
            targetModelName = StoryManager.main.GetTargetDressModelNameByDressName(speaker, dress_name);

            // 일치하는 드레스 코드 없으면 끝!
            if (string.IsNullOrEmpty(targetModelName))
                return;

            // 중복 로딩을 막기 위해 똑같이 쓴다.
            if (GameManager.main.AddLoadingModel(targetModelName))
            {
                ScriptModelMount mounter = new ScriptModelMount(targetModelName, speaker, OnModelMountInitialized, this);
                ListModelMount.Add(mounter);
            }
        }



        /// <summary>
        /// 라이브 일러스트 수집
        /// </summary>
        void CollectDemandedLiveIllustResource(ScriptRow __row)
        {
            // 템플릿을 illust 똑같은걸 쓰기 때문에 
            // StoryManager를 통해서 라이브 일러스트 정보가 있는지 체크한다. 
            string illustName = __row.script_data;
            if (string.IsNullOrEmpty(illustName))
                return;

            // 기준정보 있는지 체크 
            if (!StoryManager.main.CheckExistsLiveIllust(illustName))
                return;

            // 있으면 그만!
            if (CheckLiveIllustMountExists(illustName))
                return;

            ScriptLiveMount m = new ScriptLiveMount(__row.rowData, OnLiveIllustMountInitialized, this, false);
            ListLiveIllustMount.Add(m);
        }

        /// <summary>
        /// 라이브 일러스트 중복 체크용 메소드 
        /// </summary>
        /// <param name="__illustName"></param>
        bool CheckLiveIllustMountExists(string __illustName)
        {
            for (int i = 0; i < ListLiveIllustMount.Count; i++)
            {
                if (ListLiveIllustMount[i].liveName == __illustName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 라이브 오브젝트 수집!
        /// </summary>
        void CollectDemandedLiveObjectResource(ScriptRow __row)
        {
            // data 값 없으면 return.
            if (string.IsNullOrEmpty(__row.script_data))
                return;

            // 중복 막는다.
            for (int i = 0; i < ListLiveObjectMount.Count; i++)
            {
                if (ListLiveObjectMount[i].liveName == __row.script_data)
                    return;
            }

            //중복 아니니까 List에 추가한다. 
            ScriptLiveMount m = new ScriptLiveMount(__row.rowData, OnLiveObjectMountInitialized, this, true);
            ListLiveObjectMount.Add(m);
        }

        #endregion

        /// <summary>
        /// 페이지 로딩 값 얻기 위해서.. 
        /// </summary>
        public int GetPrepCount()
        {
            return pageResourceCount;
        }

        public int GetCurrentLoadingCount()
        {
            return pageImageResourceCount + pageModelCount + pageLiveIllustCount + pageLiveObjectCount + pageBubbleResourceCount + pageSoundResourceCount;
        }


        /// <summary>
        /// 페이지의 각 행들이 모두 초기화 되었는지 체크 
        /// </summary>
        public bool IsPageInitialized()
        {
            // rowcount 0이고 페이지 이미지 초기화 되었을때만!
            return GetCurrentLoadingCount() < 1 && isPageImagesInitialized;
        }

        /// <summary>
        /// 행 초기화 될때마다 실행되도록, callback이다.
        /// </summary>
        void OnRowInitialized()
        {
            // 현재로서는 아무것도 안함.
        }

        #region Mount Callback

        void OnImageMountInitialized()
        {
            pageImageResourceCount--;
        }

        void OnBubbleMountInitialized()
        {
            pageBubbleResourceCount--;
        }

        void OnSoundMountInitialized()
        {
            pageSoundResourceCount--;
        }

        void OnModelMountInitialized()
        {
            pageModelCount--;
            Debug.Log("OnModelMountInitialized : " + pageModelCount);
        }

        void OnLiveIllustMountInitialized()
        {
            pageLiveIllustCount--;
        }

        void OnLiveObjectMountInitialized()
        {
            pageLiveObjectCount--;
        }

        #endregion

        /// <summary>
        /// 다음 실행할 ScriptRow를 요청한다. 
        /// </summary>
        public ScriptRow GetNextRow()
        {
            if (playRow >= ListRows.Count)
                return null;

            return ListRows[playRow++];
        }

        /// <summary>
        /// 다음 행이 뭐에요~
        /// </summary>
        public ScriptRow GetNextRowWithoutIncrement()
        {
            // GetNextRow 가 호출된 시점에 이미 playRow는 증가 되어있기 때문에 
            // playRow 그대로 쓰면 된다.
            if (playRow >= ListRows.Count)
                return null;

            return ListRows[playRow];
        }

        /// <summary>
        /// 현재 진행중인 행
        /// </summary>
        /// <returns></returns>
        public ScriptRow GetCurrentRow()
        {
            if (playRow - 1 < 0)
                return null;

            return ListRows[playRow - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>이전 행에 대해서</returns>
        public ScriptRow GetPrevRow()
        {
            if (playRow - 2 < 0)
                return null;

            return ListRows[playRow - 2];
        }

        /// <summary>
        /// playRow 조정 
        /// </summary>
        /// <param name="__sceneID"></param>
        public bool SetCurrentRowBySceneID(string __sceneID)
        {
            Debug.Log("SetCurrentRowBySceneID :: " + __sceneID);

            // _sceneID를 찾는다. 
            for (int i = 0; i < ListRows.Count; i++)
            {
                if (ListRows[i].scene_id == __sceneID)
                {
                    playRow = i;
                    return true;
                }
            }

            Debug.Log("Can't find scene : " + __sceneID);

            return false;
        }

        /// <summary>
        /// 에피소드안에서 이미지(미니컷)의 사용횟수 조회 
        /// </summary>
        int GetSpecificImageUsageCount(string __imageName)
        {
            int count = 0;

            for (int i = 0; i < ListRows.Count; i++)
            {
                if (string.IsNullOrEmpty(ListRows[i].script_data) || string.IsNullOrEmpty(ListRows[i].template))
                    continue;

                if (ListRows[i].template != GameConst.TEMPLATE_IMAGE)
                    continue;

                // 이미지 있다!
                if (ListRows[i].script_data == __imageName)
                    count++;
            }
            return count;
        }


        /// <summary>
        /// 대상 배경의 사용 카운트 
        /// </summary>
        int GetBackgroundUseCount(string __bgName)
        {
            int count = 0;

            for (int i = 0; i < ListRows.Count; i++)
            {
                if (string.IsNullOrEmpty(ListRows[i].script_data) || string.IsNullOrEmpty(ListRows[i].template))
                    continue;


                // 배경 템플릿 혹은 장소진입 템플릿 
                if (ListRows[i].template == GameConst.TEMPLATE_BACKGROUND && ListRows[i].script_data == __bgName)
                    count++;
                else if (ListRows[i].template == GameConst.TEMPLATE_MOVEIN && ListRows[i].script_data.Contains(__bgName))
                    count++;
            }

            return count;
        }

        int GetEmoticonUseCount(string __expression)
        {
            int count = 0;

            for (int i = 0; i < ListRows.Count; i++)
            {
                if (string.IsNullOrEmpty(ListRows[i].emoticon_expression) || string.IsNullOrEmpty(ListRows[i].template))
                    continue;

                // 이모티콘이 사용되는 대화 관련 템플릿
                switch (ListRows[i].template)
                {
                    case GameConst.TEMPLATE_TALK:
                    case GameConst.TEMPLATE_YELL:
                    case GameConst.TEMPLATE_WHISPER:
                    case GameConst.TEMPLATE_FEELING:
                    case GameConst.TEMPLATE_MONOLOGUE:
                    case GameConst.TEMPLATE_SPEECH:
                    case GameConst.TEMPLATE_PHONE_PARTNER:
                    case GameConst.TEMPLATE_PHONE_SELF:
                    case GameConst.TEMPLATE_MESSAGE_PARTNER:
                    case GameConst.TEMPLATE_MESSAGE_SELF:
                        if (ListRows[i].emoticon_expression.Equals(__expression))
                            count++;
                        break;
                }
            }

            return count;
        }

        /// <summary>
        /// 라이브 오브제 사용 카운트
        /// </summary>
        int GetLiveObjUseCount(string ObjName)
        {
            int count = 0;

            for (int i = 0; i < ListRows.Count; i++)
            {
                if (string.IsNullOrEmpty(ListRows[i].script_data) || string.IsNullOrEmpty(ListRows[i].template))
                    continue;

                if (ListRows[i].template.Equals(GameConst.TEMPLATE_LIVE_OBJECT) && ListRows[i].script_data.Equals(ObjName))
                    count++;
            }

            return count;
        }


        int GetLiveIllustUseCount(string illustName)
        {
            int count = 0;

            for (int i = 0; i < ListRows.Count; i++)
            {
                if (string.IsNullOrEmpty(ListRows[i].script_data) || string.IsNullOrEmpty(ListRows[i].template))
                    continue;

                if (ListRows[i].template.Equals(GameConst.TEMPLATE_ILLUST) && ListRows[i].script_data.Equals(illustName))
                    count++;
            }

            return count;
        }


        /// <summary>
        /// 리스트에서 다쓴 이미지 제거하기
        /// </summary>
        public void RemoveImageMount(ScriptImageMount __mount)
        {
            ListImageMount.Remove(__mount);
        }

        /// <summary>
        /// 리스트에서 다 쓴 라이브 오브제 제거
        /// </summary>
        public void RemoveLiveObjectMount(ScriptLiveMount __mount)
        {
            ListLiveObjectMount.Remove(__mount);
        }

        /// <summary>
        /// 리스트에서 다 쓴 사운드 제거
        /// </summary>
        public void RemoveSoundMount(ScriptSoundMount __mount)
        {
            ListSoundMount.Remove(__mount);
        }

        /// <summary>
        /// 리스트에서 다 쓴 라이브 일러스트 제거
        /// </summary>
        public void RemoveLiveIllustMount(ScriptLiveMount __mount)
        {
            ListLiveIllustMount.Remove(__mount);
        }

        /// <summary>
        /// Row 중에서 파라매터로 전달된 scriptNO가 있는지 체크한다.
        /// </summary>
        public bool CheckHasScriptNO(long __scriptNO)
        {
            for (int i = 0; i < ListRows.Count; i++)
            {
                if (ListRows[i].script_no == __scriptNO) // 있다!
                    return true;
            }

            // 없으면 false
            return false;
        }

        /// <summary>
        /// 불완전한 선택지 존재 유무 체크 
        /// </summary>
        public bool CheckIncompleteSelection()
        {
            for (int i = 0; i < ListRows.Count; i++)
            {
                if (ListRows[i].template == GameConst.TEMPLATE_SELECTION && string.IsNullOrEmpty(ListRows[i].target_scene_id)) // 사건 이동 컬럼이 비어있으면 안돼!
                    return true;
            }

            return false;
        }
    }
}

