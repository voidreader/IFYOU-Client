using System;
using System.Collections;
using UnityEngine;

using LitJson;
using BestHTTP;

// Live2D
using Live2D.Cubism.Core;
using Live2D.Cubism.Viewer;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Framework.Json;

namespace PIERStory
{
    [Serializable]
    public class ScriptLiveMount
    {
        Action OnMountCompleted = delegate { };

        static readonly string ILLUST_VER = "illust_ver";
        static readonly string LIVEOBJ_VER = "object_ver";

        public CubismModel liveImage = null;   // Live2D 오브제, 일러스트
        public Animation anim = null;

        JsonData resourceData = null;

        public string liveName = string.Empty;
        public bool isMounted = false;

        public int totalAssetCount = 0;
        public int unloadAssetCount = 0;

        int modelVersion = 0;         // 저장된 모델 버전
        int downloadModelVersion = 0; // 다운로드 모델 버전

        public GameLiveImageCtrl liveImageController = null;

        // 크기와 위치정보 
        public float gameScale = 10;
        float offsetX = 0;
        float offsetY = 0;

        MonoBehaviour pageParent = null;

        public int useCount = 0;    // 하나의 에피소드에서 몇번 사용하는지 체크
        bool isMinicut = false;     // 라이브 오브제인가?

        public ScriptLiveMount(JsonData __j, Action __cb, MonoBehaviour __parent, bool __liveObj)
        {
            liveName = SystemManager.GetJsonNodeString(__j, GameConst.COL_SCRIPT_DATA);

            Initialize(__cb, __parent, __liveObj);
            LoadVersion(liveName);
        }

        public ScriptLiveMount(string __live, Action __cb, MonoBehaviour __parent, bool __liveObj)
        {
            liveName = __live;

            Initialize(__cb, __parent, __liveObj);
            LoadVersion(liveName);
        }


        void Initialize(Action __cb, MonoBehaviour __parent, bool __liveObj)
        {
            pageParent = __parent;
            OnMountCompleted = __cb;
            isMinicut = __liveObj;
        }

        /// <summary>
        /// 관련 데이터를 StoryManager에서 이미 들고있으니까. 받아서 처리하기.
        /// </summary>
        /// <param name="__modelName"></param>
        public void SetModelDataFromStoryManager()
        {
            resourceData = StoryManager.main.GetLiveObjectJsonByName(liveName);

            if (resourceData == null || resourceData.Count == 0) // 데이터가 없을 경우 
            {
                SendFailMessage();
                return;
            }

            // 데이터 가져왔으면, 모델 초기화 시작한다. 
            InitCubismModel();
        }

        void LoadVersion(string __liveName)
        {
            modelVersion = PlayerPrefs.GetInt(__liveName, 0);
        }

        void SaveModelVersion(int __newVersion)
        {
            PlayerPrefs.SetInt(liveName, __newVersion);
            PlayerPrefs.Save();
        }

        string GetCubismAbsolutePath(string __key)
        {
            return Application.persistentDataPath + "/" + StoryManager.main.CurrentProjectID + "/" + __key;
        }

        string GetCubismRelativePath(string __key)
        {
            return StoryManager.main.CurrentProjectID + "/" + __key;
        }

        void SetMinusAssetCount()
        {
            unloadAssetCount--;

            if (unloadAssetCount == 0)
            {
                // Serializable은 코루틴을 쓰지 못하니, 엄마한테 부탁한다.
                pageParent.StartCoroutine(CheckFileSavedAndStartInitModel());
                string.Format(" <color=lime>{0} Model files are downloaded</color>", liveName);
            }
        }

        /// <summary>
        /// 모델 본격 로드!
        /// </summary>
        void InitCubismModel()
        {
            string file_key = string.Empty;
            string file_url = string.Empty;

            totalAssetCount = resourceData.Count; // 파일이 몇개인지 체크한다. 
            unloadAssetCount = totalAssetCount;

            gameScale = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_GAME_SCALE));
            offsetX = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_OFFSET_X));
            offsetY = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_OFFSET_Y));

            // 다운로드 모델 버전 
            downloadModelVersion = isMinicut? int.Parse(SystemManager.GetJsonNodeString(resourceData[0], LIVEOBJ_VER)) : int.Parse(SystemManager.GetJsonNodeString(resourceData[0], ILLUST_VER));
            Debug.Log(JsonMapper.ToStringUnicode(resourceData));


            // 가져온 데이터 기반으로 파일 존재 유무를 체크해서 없으면 다운로드를 받는다. 
            for (int i = 0; i < resourceData.Count; i++)
            {
                file_key = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_KEY);
                file_url = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_URL);

                // 파일이 없거나, 다운로드 버전이 더 높으면!
                if (!ES3.FileExists(GetCubismRelativePath(file_key)) || modelVersion < downloadModelVersion)
                {
                    // 파일이 없으면 network로 불러온다.
                    var req = new HTTPRequest(new Uri(file_url), OnModelDownloaded);
                    req.Tag = JsonMapper.ToJson(resourceData[i]); // 태그의 데이터로 얘를 넘겨버리자. 
                    req.Send();
                }
                else
                    SetMinusAssetCount();
            }
        }

        void OnModelDownloaded(HTTPRequest req, HTTPResponse res)
        {
            // 인게임 리소스 다운로드는 CheckInGameDownloadValidation 사용 
            // CheckInGameDownloadValidation에서 3번 시도 후 실패하면 팝업 띄운다. 
            if (!NetworkLoader.CheckInGameDownloadValidation(req, res))
                return;

            // Allow Missing Resource에 의해서 true로 넘어오는 경우도 있다.
            if (!res.IsSuccess)
            {
                Debug.LogError("Download Failed : " + req.Uri.ToString());
                SendFailMessage();
                return;
            }

            JsonData requestJSON = JsonMapper.ToObject(req.Tag.ToString());

            // byte로 받기 때문에 파일을 저장한다. 
            // 로컬 세이브 
            ES3.SaveRaw(res.Data, GetCubismRelativePath(SystemManager.GetJsonNodeString(requestJSON, CommonConst.COL_FILE_KEY))); // requestJSON의 키를 저장경로로 사용 할 것!

            SetMinusAssetCount();
        }

        /// <summary>
        /// 파일 다 저장되었는지 확인하고, 모델 생성시작하기.
        /// </summary>
        IEnumerator CheckFileSavedAndStartInitModel()
        {
            string file_key = string.Empty;
            bool isFilesExist = false;

            yield return new WaitForSeconds(0.2f);

            while (!isFilesExist)
            {
                isFilesExist = true;

                for (int i = 0; i < resourceData.Count; i++)
                {
                    file_key = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_KEY);

                    // 파일의 저장이 아직 이루어지지 않았다.
                    if (!ES3.FileExists(GetCubismRelativePath(file_key)))
                        isFilesExist = false;
                }

                // 파일 저장이 이루어지지 않았다면 0.2초씩 대기 
                if (!isFilesExist)
                    yield return new WaitForSeconds(0.2f);
            }

            // ! 2021.08.18 라이브 오브제도 갤러리에서 쓰게 생겼다
            // 얘는 미리 생성안시키고 필요할때 불러오기 떄문에 LiveIllustMount랑 쫌 다르구나
            if (isMinicut && GameManager.main != null)
                SendSuccessMessage();
            else
                InstantiateCubismModel();

        }

        /// <summary>
        /// 모든 파일을 불러왔으면 이제 시작한다. 
        /// </summary>
        public void InstantiateCubismModel()
        {
            string file_key = string.Empty;

            for (int i = 0; i < resourceData.Count; i++)
            {
                file_key = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_KEY); 

                if (file_key.Contains(CommonConst.MODEL3_JSON)) // main model 처리
                {
                    CreateCubismModel(file_key);
                    break;
                }
            } // 여기까지 모델 생성 처리 

            // 구성파일이 잘못되어서 생성을 못한 경우 더이상 함수를 진행하지 않는다
            if (liveImage == null)
                return;

            // Animation Component 추가한다. (애니메이션용)
            if (anim == null)
                anim = liveImage.gameObject.AddComponent<Animation>();

            // 세팅된 모션 준비하기
            PrepareCubismMotions();

            if (isMinicut && LobbyManager.main != null)
                SendSuccessMessage();
        }

        void CreateCubismModel(string __path)
        {
            CubismModel3Json model3Json = CubismModel3Json.LoadAtPath(GetCubismAbsolutePath(__path), CubismViewerIo.LoadAsset);

            liveImage = model3Json.ToModel();

            if (liveImage == null)
            {
                Debug.LogError("Error in creating Live2D Model : " + liveName);
                SendFailMessage();
                return;
            }

            // GameLiveImageCtrl attach
            liveImageController = liveImage.gameObject.AddComponent<GameLiveImageCtrl>();
            liveImageController.originScale = gameScale;
            liveImageController.modelType = CommonConst.MODEL_TYPE_LIVE2D;
            liveImageController.SetModel(liveImage); // 모델 할당.

            if (GameManager.main != null)
                GameManager.main.SetIllustParent(liveImage.transform);
            else
                LobbyManager.main.SetIllustParent(liveImage.transform);


            // 스케일 조정 및 크기 조정 
            liveImage.transform.localScale = new Vector3(gameScale, gameScale, 1);
            liveImage.transform.localPosition = new Vector3(offsetX, offsetY, 0);

            // 레이어 변경
            if (GameManager.main != null)
            {
                if(isMinicut)
                    liveImage.GetComponent<CubismRenderController>().SortingLayer = GameConst.LAYER_LIVE_OBJ;
                else
                    liveImage.GetComponent<CubismRenderController>().SortingLayer = GameConst.LAYER_ILLUST;
            }
            else
                // Lobby의 Gallery에서 일러스트 상세보기 할 때
                liveImage.GetComponent<CubismRenderController>().SortingLayer = GameConst.LAYER_UI;

            liveImage.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;
            ChangeLayerRecursively(liveImage.transform);
        }

        void ChangeLayerRecursively(Transform trans)
        {
            if(isMinicut)
                trans.gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_LIVE_OBJ);
            else
                trans.gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_ILLUST);

            foreach (Transform tr in trans)
                ChangeLayerRecursively(tr);
        }

        /// <summary>
        /// 세팅된 모션 준비하기!
        /// </summary>
        void PrepareCubismMotions()
        {
            string file_key = string.Empty;
            string motion_name = string.Empty;
            CubismMotion3Json motionJson = null;
            AnimationClip clip = null;
            string motion_path = string.Empty;

            for (int i = 0; i < resourceData.Count; i++)
            {
                file_key = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_KEY);
                motion_name = string.IsNullOrEmpty(SystemManager.GetJsonNodeString(resourceData[i], CommonConst.MOTION_NAME)) ? string.Empty : SystemManager.GetJsonNodeString(resourceData[i], CommonConst.MOTION_NAME);

                // 파일이름과 세팅된 모션이름이 있는지 체크해서 진행 
                if (!file_key.Contains(GameConst.MOTION3_JSON) || string.IsNullOrEmpty(motion_name))
                    continue;

                // 모션을 만들자. 
                motion_path = GetCubismAbsolutePath(file_key);

                // 모션 파일을 읽어와서 애니메이션 클립으로 만들기.
                motionJson = CubismMotion3Json.LoadFrom(CubismViewerIo.LoadAsset<string>(motion_path));
                clip = motionJson.ToAnimationClip();

                // 라이브오브제인 경우
                if (isMinicut)
                    clip.wrapMode = WrapMode.Loop;
                else
                {
                    // 라이브 일러스트인 경우
                    if (motion_name.Equals("시작"))
                        clip.wrapMode = WrapMode.Clamp;
                    else
                        clip.wrapMode = WrapMode.Loop;
                }

                clip.name = motion_name;
                clip.legacy = true;

                anim.AddClip(clip, motion_name); // 클립추가!
            }
        }

        public void PlayCubismAnimation()
        {
            // 모델이 비활성 상태일때 활성화가 되면 페이드인 처리를 한다. 
            if (!liveImage.gameObject.activeSelf)
                liveImageController.ActivateModel();

            // 클립 재생 
            if(isMinicut)
                anim.PlayQueued("루프");
            else
            {
                anim.CrossFade("시작", 0.3f);
                anim.PlayQueued("루프", QueueMode.CompleteOthers);
            }
        }

        void SendFailMessage()
        {
            isMounted = false;
            OnMountCompleted();
        }

        void SendSuccessMessage()
        {
            isMounted = true;
            OnMountCompleted();

            SaveModelVersion(downloadModelVersion);
        }

        public void SetLiveImageUseCount(int __count)
        {
            useCount = __count;
        }

        public void DecreaseUseCount()
        {
            useCount--;
        }

        public void EndIllust()
        {
            if (useCount < 1)
            {
                if(isMinicut)
                    GameManager.main.RemoveLiveIllustFromDictionary(liveName);
                else
                    GameManager.main.RemoveLiveObjFromDictionary(liveName);

                liveImageController.DestroySelf();
            }
        }
    }
}
