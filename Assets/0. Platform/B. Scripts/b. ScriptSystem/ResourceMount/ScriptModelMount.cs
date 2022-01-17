using System;
using System.Collections;
using System.Collections.Generic;
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
    public class ScriptModelMount
    {
        Action OnMountCompleted = delegate { };

        static readonly string MODEL_VER = "model_ver";

        public CubismModel model = null; 
        Animation anim = null;
        Dictionary<string, AnimationClip> DictMotion;

        #region DefaultCharacter와 관련된 변수 모음
        
        GameObject spriteObject;
        SpriteRenderer defaultSprite;

        #endregion

        JsonData resourceData = null;
        
        [SerializeField] bool isResourceDownloadComplete = false; // 리소스 다운로드 완료 
        public bool isModelCreated = false; // 모델 생성되었는지 체크 .

        public string originModelName = string.Empty;   // 원래 모델명(의상시스템 관련)
        public string speaker = string.Empty;           // 화자
        public bool isMounted = false;

        public int totalAssetCount = 0;     // 소속된 모든 파일 개수 
        public int unloadAssetCount = 0;    // 아직 로딩이 되지 않은 파일 개수 

        int modelVersion = 0; // 저장된 모델 버전.
        int downloadModelVersion = 0; // 다운로드 모델 버전 

        string pathModel = string.Empty;
        List<string> ListMotionPath = new List<string>();
        // 더미와 스탠딩 모두를 소유할 부모 생성.
        // modelCharacter가 GameModelCtrl 스크립트를 포함한다.
        public GameObject modelCharacter;
        public GameModelCtrl modelController = null;

        // 크기와 위치정보 
        float gameScale = 10;
        float offset_x = 0;
        float offset_y = 0;
        // 시선 방향
        string direction = GameConst.VIEWDIRECTION_CENTER;

        bool isDressModel = false;

        // path 설정할때, string path = Application.persistentDataPath + /프로젝트id/ + file_key로 설정해주어야 한다. 
        MonoBehaviour pageParent = null;

        /// <summary>
        /// ScriptRow 기반으로 모델을 가져온다.
        /// </summary>
        public ScriptModelMount(JsonData __j, Action __cb, MonoBehaviour __parent)
        {
            speaker = SystemManager.GetJsonNodeString(__j, GameConst.COL_SPEAKER);
            originModelName = speaker; // row 기반의 생성은 모델명 = 화자 이다. 

            Initialize(__cb, __parent, false);
        }

        /// <summary>
        /// 의상 템플릿에서 생성된 캐릭터 모델
        /// </summary>
        /// <param name="__originModelName">모델의 원본 이름</param>
        /// <param name="__speaker">대표 화자명</param>
        /// <param name="__cb">콜백</param>
        public ScriptModelMount(string __originModelName, string __speaker, Action __cb, MonoBehaviour __parent)
        {
            speaker = __speaker;
            originModelName = __originModelName;

            Initialize(__cb, __parent, true);
        }

        /// <summary>
        /// 초기화 작업
        /// </summary>
        /// <param name="dressCheck">의상 체크</param>
        void Initialize(Action __cb, MonoBehaviour __parent, bool dressCheck)
        {
            pageParent = __parent;
            OnMountCompleted = __cb;
            DictMotion = new Dictionary<string, AnimationClip>();
            isDressModel = dressCheck;

            LoadModelVersion(originModelName); // 모델 버전 로드
        }

        /// <summary>
        /// 모델 데이터 처리 (StoryManager 에서 받아오기!)
        /// 모델 데이터는 프로젝트 상세 화면 진입시에 미리 받아놓는다.
        /// </summary>
        public void SetModelDataFromStoryManager()
        {
            Debug.Log("SetModelDataFromStoryManager : " + originModelName);
            resourceData = StoryManager.main.GetModelJsonByModelName(originModelName);

            // 새로운 GameObject 생성
            modelCharacter = new GameObject();
            modelCharacter.name = originModelName;

            // 생성된 GameObject를 ModelLoader에 붙인다
            GameManager.main.SetModelParent(modelCharacter.transform);
            modelCharacter.transform.localPosition = Vector3.zero;

            // GameModelCtrl은 생성한 GameObejct에 Addcomponent해준다
            modelController = modelCharacter.AddComponent<GameModelCtrl>();
            modelController.speaker = speaker;
            modelController.originModelName = originModelName;

            // 데이터 가져왔으면, 모델 초기화 시작한다. 
            InitCubismModel();
        }

        /// <summary>
        /// 모델 버전 로드 
        /// </summary>
        void LoadModelVersion(string __modelName)
        {
            // 모델 버전 조회한다. 
            modelVersion = PlayerPrefs.GetInt(__modelName, 0);
        }

        /// <summary>
        /// 모델 버전 세이브 
        /// </summary>
        void SaveModelVersion(int __newVersion)
        {
            PlayerPrefs.SetInt(originModelName, __newVersion);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Live2D 모델 경로
        /// </summary>
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
                pageParent.StartCoroutine(CheckFileSavedAndStartInitModel());
                Debug.Log(string.Format(" <color=lime>{0} Model files are downloaded</color>", originModelName));

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

            // Debug.Log(JsonMapper.ToStringUnicode(resourceData));

            // scale과 offset 위치 정보
            gameScale = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_GAME_SCALE));
            offset_x = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_OFFSET_X));
            offset_y = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_OFFSET_Y));
            direction = SystemManager.GetJsonNodeString(resourceData[0], GameConst.COL_DIRECTION);

            // 다운로드 모델 버전 
            downloadModelVersion = int.Parse(SystemManager.GetJsonNodeString(resourceData[0], MODEL_VER));
            Debug.Log(string.Format("InitCubismModel, totalFiles[{0}], ver[{1}], newVer[{2}]", totalAssetCount, modelVersion, downloadModelVersion));

            // 데이터
            if (resourceData.Count <= 1)
            {
                Debug.Log("Not enougth Data");
                SendFailMessage();
                return;
            }


            // 가져온 데이터 기반으로 파일 존재 유무를 체크해서 없으면 다운로드를 받는다. 
            for (int i = 0; i < resourceData.Count; i++)
            {
                file_key = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_KEY);
                file_url = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_URL);

                // 파일이 없거나, 다운로드 버전이 더 높으면!
                if (!ES3.FileExists(GetCubismRelativePath(file_key)) || modelVersion < downloadModelVersion)
                {
                    // 파일이 없으면 network로 불러온다.
                    var req = new HTTPRequest(new System.Uri(file_url), OnModelDownloaded);
                    req.Tag = JsonMapper.ToJson(resourceData[i]); // 태그의 데이터로 얘를 넘겨버리자. 
                    req.Timeout = System.TimeSpan.FromSeconds(40);
                    req.Send();
                }
                else
                    SetMinusAssetCount();
            }
        }

        /// <summary>
        /// 모델 신규 다운로드
        /// </summary>
        void OnModelDownloaded(HTTPRequest req, HTTPResponse res)
        {

            // 다운로드 실패시, 재시도를 2회 더 진행한다. 
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
            ES3.SaveRaw(res.Data, GetCubismRelativePath(SystemManager.GetJsonNodeString(requestJSON, CommonConst.COL_FILE_KEY)), SystemManager.noEncryptionSetting); // requestJSON의 키를 저장경로로 사용 할 것!

            SetMinusAssetCount();
        }

        /// <summary>
        /// 파일 다 저장되었는지 확인하고, 모델 생성시작하기.
        /// </summary>
        IEnumerator CheckFileSavedAndStartInitModel()
        {
            string file_key = string.Empty;
            isResourceDownloadComplete = false;

            yield return new WaitForSeconds(0.1f);

            while (!isResourceDownloadComplete)
            {
                isResourceDownloadComplete = true;

                for (int i = 0; i < resourceData.Count; i++)
                {
                    file_key = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_KEY);

                    // 파일의 저장이 아직 이루어지지 않았다.
                    if (!ES3.FileExists(GetCubismRelativePath(file_key)))
                        isResourceDownloadComplete = false;
                }

                if (!isResourceDownloadComplete)
                    yield return null;
            }
            
            isResourceDownloadComplete = true; // 리소스 다운로드 완료 
            

            // ! 테스트를 위해 감춘다.!
            // InstantiateCubismModel();
            
            // ! 여기서 성공 메세지 
            SendSuccessMessage();
        }

        /// <summary>
        /// 모든 파일을 불러왔으면 이제 시작한다. 
        /// </summary>
        public void InstantiateCubismModel()
        {
            string file_key = string.Empty;

            for (int i = 0; i < resourceData.Count; i++)
            {
                file_key = resourceData[i]["file_key"].ToString(); // file_key 기반으로.. 

                if (file_key.Contains(".model3.json")) // main model 처리 
                {
                    CreateCubismModel(file_key);
                    break;
                }
            } // 여기까지 모델 생성 처리 

            // Animation Component 추가한다. (애니메이션용)
            if (anim == null)
                anim = model.gameObject.AddComponent<Animation>();

            // 세팅된 모션 준비하기
            PrepareCubismMotions();
            // SendSuccessMessage();
            
            isModelCreated = true;
            
            // ! 키 체크를 위해 박스 컬라이더 추가
            // SetBoxColliders();
            
            if(modelController != null) {
                modelController.SetBoxColliders();
            }
            
        }

        /// <summary>
        /// 키 체크를 위해 충돌체 설정 
        /// </summary>
        /*
        public void SetBoxColliders()
        {
            if (model == null) {
                Debug.Log(this.originModelName + " is not created");
                return;
            }

            // 모든 drawables에 boxCollider 추가 
            for (int i = 0; i < model.Drawables.Length; i++)
                model.Drawables[i].gameObject.AddComponent<BoxCollider>();
        }
        */

        /// <summary>
        /// 메인 파일의 모델화 처리 (Live2D)
        /// </summary>
        void CreateCubismModel(string __path)
        {
            pathModel = GetCubismAbsolutePath(__path);

            CubismModel3Json model3Json = CubismModel3Json.LoadAtPath(GetCubismAbsolutePath(__path), CubismViewerIo.LoadAsset);
            model = model3Json.ToModel();

            if (model == null)
            {
                Debug.LogError("Error in creating Live2D Model : " + originModelName);
                SendFailMessage();
                return;
            }

            modelController.SetModel(model, direction); // 모델 할당.

            // 새로 생성한 GameObject의 자식으로 설정
            model.transform.SetParent(modelCharacter.transform);

            // 스케일 조정 및 크기 조정 
            model.transform.localScale = new Vector3(gameScale, gameScale, 1);
            model.transform.localPosition = new Vector3(offset_x, offset_y, 0);

            // 레이어 변경 
            model.GetComponent<CubismRenderController>().SortingLayer = GameConst.LAYER_MODEL;
            model.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;
            modelController.ChangeLayerRecursively(modelCharacter.transform, GameConst.LAYER_MODEL_C);
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
                motion_name = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.MOTION_NAME);

                // 파일이름과 세팅된 모션이름이 있는지 체크해서 진행 
                if (!file_key.Contains(GameConst.MOTION3_JSON) || string.IsNullOrEmpty(motion_name))
                    continue;

                // 모션을 만들자. 
                motion_path = GetCubismAbsolutePath(file_key);
                ListMotionPath.Add(motion_path);

                // 모션 파일을 읽어와서 애니메이션 클립으로 만들기.
                motionJson = CubismMotion3Json.LoadFrom(CubismViewerIo.LoadAsset<string>(motion_path));
                clip = motionJson.ToAnimationClip();


                clip.wrapMode = WrapMode.Loop;
                clip.name = motion_name;
                clip.legacy = true;

                if (!DictMotion.ContainsKey(motion_name))
                {
                    DictMotion.Add(motion_name, clip);
                    anim.AddClip(clip, motion_name); // 클립추가 !
                }

            } // end of for

            // modelController에 추가된 animation 이식
            modelController.modelAnim = anim;
            modelController.DictMotion = DictMotion;
        }

        /// <summary>
        /// 모델 감추기!
        /// </summary>
        public void HideModel()
        {
            if(!isModelCreated)
                return;
            
            modelCharacter.SetActive(false);
        }

        void SendFailMessage()
        {
            Debug.Log("SendFailMessage : " + originModelName);
            // Fail 메세지를 보낼때는 isMounted = true로 주고, default 값을 사용하는 것으로 한다 
            isMounted = true;
            OnMountCompleted();

            // 실패했을 경우에는 더미 캐릭터를 생성해준다.
            spriteObject = new GameObject();
            spriteObject.name = "DefaultCharacter";
            spriteObject.transform.SetParent(modelCharacter.transform);
            // 여기에선 스프라이트만 생성
            defaultSprite = spriteObject.AddComponent<SpriteRenderer>();
        }

        void SendSuccessMessage()
        {
            Debug.Log("SendSuccessMessage : " + originModelName);

            isMounted = true;
            OnMountCompleted();

            SaveModelVersion(downloadModelVersion);
        }
    }
}

