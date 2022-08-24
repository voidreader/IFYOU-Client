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
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.MotionFade;

// Addressable
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        
        // * isLoaded => isMounted의 순서로 완료 
        public bool isMounted = false; // 최종 완료. 인스턴스까지 완료했음. 
        public bool isLoaded = false; // 다운로드 혹은 에셋번들 로드 완료 

        public int totalAssetCount = 0;     // 소속된 모든 파일 개수 
        public int unloadAssetCount = 0;    // 아직 로딩이 되지 않은 파일 개수 

        int modelVersion = 0; // 저장된 모델 버전.
        int downloadModelVersion = 0; // 다운로드 모델 버전 

        List<string> ListMotionPath = new List<string>();
        // 더미와 스탠딩 모두를 소유할 부모 생성.
        // modelCharacter가 GameModelCtrl 스크립트를 포함한다.
        public GameObject modelCharacter;
        public GameModelCtrl modelController = null;

        // 크기와 위치정보 
        [SerializeField] float gameScale = 10;
        [SerializeField] float offset_x = 0;
        [SerializeField] float offset_y = 0;
        // 시선 방향
        [SerializeField] string direction = GameConst.VIEWDIRECTION_CENTER;
        [SerializeField] int tallGrade = 0; // 키 추가 

        // path 설정할때, string path = Application.persistentDataPath + /프로젝트id/ + file_key로 설정해주어야 한다. 
        
        // * Addressable 관련 추가
        public bool isAddressable = false;
        public string addressableKey = string.Empty; // 어드레서블 키 
        public AsyncOperationHandle<GameObject> mountedModelAddressable; 
        
        public bool isImmediateInstance = false;

        /// <summary>
        /// ScriptRow 기반으로 모델을 가져온다.
        /// </summary>
        public ScriptModelMount(JsonData __j, Action __cb)
        {
            speaker = SystemManager.GetJsonNodeString(__j, GameConst.COL_SPEAKER);
            originModelName = speaker; // row 기반의 생성은 모델명 = 화자 이다. 

            Initialize(__cb);
        }

        /// <summary>
        /// 의상 템플릿에서 생성된 캐릭터 모델
        /// </summary>
        /// <param name="__originModelName">모델의 원본 이름</param>
        /// <param name="__speaker">대표 화자명</param>
        /// <param name="__cb">콜백</param>
        public ScriptModelMount(string __originModelName, string __speaker, Action __cb)
        {
            speaker = __speaker;
            originModelName = __originModelName;

            Initialize(__cb);
        }

        /// <summary>
        /// 꾸미기형 로비에서 스탠딩 캐릭터를 생성할 때 호출하는 함수
        /// </summary>
        /// <param name="__originModelName"></param>
        /// <param name="__cb"></param>
        /// <param name="__parent"></param>
        public ScriptModelMount(string __originModelName, Action __cb)
        {
            originModelName = __originModelName;

            Initialize(__cb);
        }


        /// <summary>
        /// 초기화 작업
        /// </summary>
        /// <param name="dressCheck">의상 체크</param>
        void Initialize(Action __cb)
        {
            OnMountCompleted = __cb;
            DictMotion = new Dictionary<string, AnimationClip>();

            LoadModelVersion(originModelName); // 모델 버전 로드
        }

        /// <summary>
        /// 모델 데이터 준비하기
        /// </summary>
        /// <param name="isImmediateInstance">로딩 후 즉시 생성여부</param>
        public void SetModelDataFromStoryManager(bool __isImmediateInstance = false)
        {
            Debug.Log("SetModelDataFromStoryManager : " + originModelName);
            isImmediateInstance = __isImmediateInstance;
            resourceData = StoryManager.main.GetModelJsonByModelName(originModelName);

            // 새로운 GameObject 생성
            modelCharacter = new GameObject();
            modelCharacter.name = originModelName;

            // 생성된 GameObject를 ModelLoader에 붙인다
            if (GameManager.main != null)
                GameManager.main.SetModelParent(modelCharacter.transform);
            else
                StoryLobbyManager.main.SetLiveParent(modelCharacter.transform);

            modelCharacter.transform.localPosition = Vector3.zero;

            // GameModelCtrl은 생성한 GameObejct에 Addcomponent해준다
            modelController = modelCharacter.AddComponent<GameModelCtrl>();
            modelController.speaker = speaker;
            modelController.originModelName = originModelName;
            
            if(resourceData.Count > 0) {
                // scale과 offset 위치 정보
                gameScale = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_GAME_SCALE));
                offset_x = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_OFFSET_X));
                offset_y = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_OFFSET_Y));
                direction = SystemManager.GetJsonNodeString(resourceData[0], GameConst.COL_DIRECTION);
                tallGrade = SystemManager.GetJsonNodeInt(resourceData[0], "tall_grade");
                
                if(tallGrade > 0) {
                    Debug.Log(originModelName + " tall_grade : " + tallGrade);
                }
            }

            // * InitCubismModel 사용하지 않고 InitAddressableCubismModel 먼저 호출 
            // 2022.02.21
            InitAddressableCubismModel(); 
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
                Debug.Log(string.Format(" <color=lime>{0} Model files are downloaded</color>", originModelName));
                isLoaded = true; // 다운로드 완료 
                
                if(isImmediateInstance) {
                    InstantiateCubismModel();
                }
            }
        }
        
        
        /// <summary>
        /// 에셋번들로 등록된 모델 로드 
        /// </summary>
        void InitAddressableCubismModel() {
            addressableKey = GetAddressableKey();
            Debug.Log("## Check Model asset key :: " + addressableKey);
            
            // 에셋번들 있는지 체크 
            Addressables.LoadResourceLocationsAsync(addressableKey).Completed += (op) => {
                if(op.Status == AsyncOperationStatus.Succeeded && op.Result.Count > 0) {
                    
                    isAddressable = true; // 에셋번들 있음
                    isLoaded = true; // 로딩 완료 
                    
                    if(isImmediateInstance) {
                        InstantiateCubismModel();
                    }
                    
                    
                }
                else {  // 에셋번들에 없는 캐릭터. 
                    Debug.Log(string.Format("<color=yellow>[{0} ]Not in AssetBundle</color>", originModelName));
                    InitCubismModel(); // 기존 캐릭터 로드 로직 호출 
                }
            }; // ? end of LoadResourceLocationsAsync
            
        }
        
        
        /// <summary>
        /// 불러오기 성공 후 초기화 처리 
        /// </summary>
        void SetAddressableCubismModel(AsyncOperationHandle<GameObject> __result) {
            
            // * 에셋번들로 불러왔지만, FadeMotion 과 실제 모션 개수가 일치하지 않는 경우는 다시 파괴하고
            // * 기존 로딩 로직을 시작해야한다. 
            
            mountedModelAddressable = __result;
            model = mountedModelAddressable.Result.GetComponent<CubismModel>(); 
            
            // CubismModel이 없다..!? 
            if(model == null) {
                Debug.LogError("Error in creating Live2D Model in addressable : " + originModelName);
                NetworkLoader.main.ReportRequestError(originModelName, "It's failed to create CubismModel in addressable");
                SendFailMessage();
                return;
            }
            
            
            // * 모션 체크를 최우선으로 시작한다. 
            ModelClips clips = model.gameObject.GetComponent<ModelClips>();  // 에셋번들에서 받아온 AnimationClips. 
            
            
            // * 페이드 모션 체크. 리스트에 저장된 페이드오브젝트와 클립 개수가 안맞으면 사용하지 않는다
            CubismFadeController cubismFadeController = model.gameObject.GetComponent<CubismFadeController>();
            CubismMotionController cubismMotionController = model.gameObject.GetComponent<CubismMotionController>();
            if(cubismFadeController == null ||
                cubismMotionController == null ||
                cubismFadeController.CubismFadeMotionList == null || 
                cubismFadeController.CubismFadeMotionList.CubismFadeMotionObjects.Length != clips.ListClips.Count) {
                
                Debug.LogError(originModelName + " #### Not match fade and motion Clips");
                
                // ! 생성한 에셋을 release 시키고, 다시 기존 로직을 호출한다. 
                Addressables.ReleaseInstance(mountedModelAddressable);
                InitCubismModel();
                return;
                
            }  // ? 모션 체크 종료                 
            
            
            isAddressable = true; // 트루!
            
            // * CreateCubismModel
            modelController.SetModel(model, direction, isAddressable);
            modelController.SetTallGradeByAdmin(tallGrade);
            
            model.transform.SetParent(modelCharacter.transform);
            // 스케일 조정 및 크기 조정 
            model.transform.localScale = new Vector3(gameScale, gameScale, 1);
            model.transform.localPosition = new Vector3(offset_x, offset_y, 0);
            // 레이어 변경 
            model.GetComponent<CubismRenderController>().SortingLayer = GameConst.LAYER_MODEL;
            model.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;
            modelController.ChangeLayerRecursively(modelCharacter.transform, GameConst.LAYER_MODEL_C);
            

            // * 어드레서블 에셋을 통한 생성인 경우는 Shader 처리 추가 필요. 
            SystemManager.SetAddressableCubismShader(model);  
            
            
            string file_key = string.Empty;
            string motion_name = string.Empty;
            string clip_name = string.Empty;
            string debugMotionName = string.Empty;
            
            // * legacy는 이전에 생으로 다운받아 생성하는 그 방식. 
            for(int i=0; i< resourceData.Count;i++ ) {
                file_key = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.COL_FILE_KEY);
                motion_name = SystemManager.GetJsonNodeString(resourceData[i], CommonConst.MOTION_NAME);
                
                if (!file_key.Contains(GameConst.MOTION3_JSON) || string.IsNullOrEmpty(motion_name))
                        continue;
                
                
                for(int j=0; j<clips.ListClips.Count;j++) {
                    
                    clip_name = clips.ListClips[j].name.Replace(".anim", "");
                    
                    // file_key에 이름+ motion3.json 있으면 dict에 넣는다. 
                    if(file_key.Contains("/" + clip_name + GameConst.MOTION3_JSON)) {
                        
                        // Dict에 추가하기. 
                        if(!DictMotion.ContainsKey(motion_name)) {
                            DictMotion.Add(motion_name, clips.ListClips[j]); // ADD    
                            modelController.motionLists.Add(motion_name);
                            debugMotionName += motion_name +", ";
                        }
                    }
                }
                
            } // ? motion 처리 완료 
            

            Debug.Log(originModelName + " motions : " + debugMotionName);
            
            modelController.motionController = cubismMotionController; // Live2D 고유 모델 컨트롤러 
            modelController.modelAnim = null;
            modelController.DictMotion = DictMotion; 
            modelController.isAddressable = true;
            
            
            // 마무리 
            isModelCreated = true;
            
            // 로딩 완료!
            SendSuccessMessage();            
        } // ? 어드레서블 모델 생성 완료 
        
        
        /// <summary>
        /// 어드레서블인 경우 파괴 
        /// </summary>
        public void DestroyAddressableModel() {
            
            if(!isAddressable && !isMounted)
                return;
            
            modelController = null;
            bool result = Addressables.ReleaseInstance(mountedModelAddressable);
            Debug.Log("DestroyAddressableModel :: " + originModelName + " / " + result);
            
        }


        /// <summary>
        /// 모델 본격 로드!
        /// </summary>
        void InitCubismModel()
        {
            isAddressable = false;
            
            string file_key = string.Empty;
            string file_url = string.Empty;

            totalAssetCount = resourceData.Count; // 파일이 몇개인지 체크한다. 
            unloadAssetCount = totalAssetCount;

            // 다운로드 모델 버전 
            downloadModelVersion = int.Parse(SystemManager.GetJsonNodeString(resourceData[0], MODEL_VER));
            Debug.Log(string.Format("InitCubismModel, totalFiles[{0}], ver[{1}], newVer[{2}]", totalAssetCount, modelVersion, downloadModelVersion));

            // 데이터
            if (resourceData.Count <= 1)
            {
                Debug.Log("Not enougth Data");
                NetworkLoader.main.ReportRequestError(originModelName, "No resources");
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
                    var req = new HTTPRequest(new Uri(file_url), OnModelDownloaded);
                    req.Tag = JsonMapper.ToJson(resourceData[i]); // 태그의 데이터로 얘를 넘겨버리자. 
                    req.Timeout = TimeSpan.FromSeconds(40);
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
        /// 모델 인스턴스화 하기 
        /// </summary>
        public void InstantiateCubismModel() {
            if(isAddressable) { // 어드레서블 에셋 번들 
                // ! Instantiate
                Addressables.InstantiateAsync(addressableKey, Vector3.zero, Quaternion.identity).Completed += (handle) => {
                    
                    if(handle.Status == AsyncOperationStatus.Succeeded) { // * 성공 
                        
                        // 불러오고 추가 세팅 시작 
                        SetAddressableCubismModel(handle);
                        
                    }
                    else { // * 실패 
                        Debug.LogError(">> Failed InstantiateAsync " + originModelName + " / " + handle.OperationException.Message);
                        
                        // 실패하면 개별 다운로드로 돌아간다. 
                        // InitCubismModel();
                        SendFailMessage();
                    }
                    
                }; // ? end of InstantiateAsync
            }
            else {
                InstantiateDownloadedCubismModel();
            }
        }        
        

        /// <summary>
        /// 모든 파일을 불러왔으면 이제 시작한다. 
        /// </summary>
        public void InstantiateDownloadedCubismModel()
        {
            string file_key = string.Empty;

            for (int i = 0; i < resourceData.Count; i++)
            {
                file_key = SystemManager.GetJsonNodeString(resourceData[i], "file_key"); // file_key 기반으로.. 

                if (file_key.Contains(".model3.json")) // main model 처리 
                {
                    CreateCubismModel(file_key);
                    break;
                }
            } // 여기까지 모델 생성 처리 

            // Animation Component 추가한다. (애니메이션용)
            if (anim == null && model != null)
                anim = model.gameObject.AddComponent<Animation>();

            // 세팅된 모션 준비하기
            PrepareCubismMotions();
            isModelCreated = true;
            
            // 완료 
            SendSuccessMessage();
        }


        /// <summary>
        /// 메인 파일의 모델화 처리 (Live2D)
        /// </summary>
        void CreateCubismModel(string __path)
        {
            CubismModel3Json model3Json = CubismModel3Json.LoadAtPath(GetCubismAbsolutePath(__path), CubismViewerIo.LoadAsset);
            model = model3Json.ToModel();

            if (model == null)
            {
                Debug.LogError("Error in creating Live2D Model : " + originModelName);
                NetworkLoader.main.ReportRequestError(originModelName, "It's failed to create CubismModel");
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
                    modelController.motionLists.Add(motion_name);
                    anim.AddClip(clip, motion_name); // 클립추가 !
                }

            } // end of for

            // modelController에 추가된 animation 이식
            modelController.modelAnim = anim;
            modelController.DictMotion = DictMotion;
            modelController.isAddressable = false;
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
            isLoaded = true; // 실패했어도 로드는 완료라고 처리 
            
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
        
        
        /// <summary>
        /// 어드레서블 에셋용 키 
        /// </summary>
        /// <returns></returns>
        string GetAddressableKey() {
            string key = StoryManager.main.CurrentProjectID + CommonConst.POSTFIX_MODEL_BUNDLE + "/model/" + originModelName + ".prefab";

            return key;
        }
    }
}

