using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using LitJson;
using BestHTTP;

// Live2D
using Live2D.Cubism.Core;
using Live2D.Cubism.Viewer;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Framework.Motion;


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
        
        // * isLoaded => isMounted의 순서로 완료 
        public bool isMounted = false; // 최종 완료 인스턴스화까지 완료했음
        public bool isLoaded = false; // 다운로드 혹은 에셋번들 로드 완료 

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
        
        // * Addressable 관련 추가
        public bool isAddressable = false; // 에셋번들이니 아니니 
        public string addressableKey = string.Empty; // 어드레서블 키 
        public AsyncOperationHandle<GameObject> mountedModelAddressable;
        Dictionary<string, AnimationClip> DictMotion; 
        public CubismMotionController motionController = null;
        
        public bool isImmediateInstance = false;

        /// <summary>
        /// 게임 플레이에서 호출
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="__cb"></param>
        /// <param name="__parent"></param>
        /// <param name="__liveObj"></param>
        public ScriptLiveMount(JsonData __j, Action __cb, MonoBehaviour __parent, bool __liveObj)
        {
            liveName = SystemManager.GetJsonNodeString(__j, GameConst.COL_SCRIPT_DATA);

            Initialize(__cb, __parent, __liveObj);
            LoadVersion(liveName);
        }


        /// <summary>
        /// 갤러리에서 호출 
        /// </summary>
        /// <param name="__live"></param>
        /// <param name="__cb"></param>
        /// <param name="__parent"></param>
        /// <param name="__liveObj"></param>
        public ScriptLiveMount(string __live, Action __cb, MonoBehaviour __parent, bool __liveObj)
        {
            liveName = __live;

            Initialize(__cb, __parent, __liveObj);
            LoadVersion(liveName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="__cb"></param>
        /// <param name="__parent"></param>
        /// <param name="__liveObj">라이브 오브젝트일때 true</param>
        void Initialize(Action __cb, MonoBehaviour __parent, bool __liveObj)
        {
            pageParent = __parent;
            OnMountCompleted = __cb;
            isMinicut = __liveObj;
            
            DictMotion = new Dictionary<string, AnimationClip>();
        }

        /// <summary>
        /// 관련 데이터를 StoryManager에서 이미 들고있으니까. 받아서 처리하기.
        /// </summary>
        /// <param name="__modelName"></param>
        public void SetModelDataFromStoryManager(bool __isImmediateInstance = false)
        {
            
            isImmediateInstance = __isImmediateInstance;
            
            if(isMinicut)
                resourceData = StoryManager.main.GetLiveObjectJsonByName(liveName);
            else
                resourceData = StoryManager.main.GetLiveIllustJsonByName(liveName);

            if (resourceData == null || resourceData.Count == 0) // 데이터가 없을 경우 
            {
                NetworkLoader.main.ReportRequestError(liveName, "No resources");
                SendFailMessage();
                return;
            }
            
            // scale 및 offset 지정 
            gameScale = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_GAME_SCALE));
            offsetX = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_OFFSET_X));
            offsetY = float.Parse(SystemManager.GetJsonNodeString(resourceData[0], CommonConst.COL_OFFSET_Y));


            // 데이터 가져왔으면, 모델 초기화 시작한다. 
            //InitCubismModel();
            InitAddressableCubismModel();
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
            
            // 모든 파일 다운로드가 받았으면 다음 단계 진행 
            if (unloadAssetCount == 0)
            {
                // Serializable은 코루틴을 쓰지 못하니, 엄마한테 부탁한다.
                string.Format(" <color=lime>{0} Model files are downloaded</color>", liveName);
                
                isLoaded = true; // 다운로드 완료 
                if(isImmediateInstance) {
                    InstantiateCubismModel();
                }
            }
        }
        
        
        /// <summary>
        /// 어드레서블 키 가져오기 
        /// </summary>
        /// <returns></returns>
        string GetAddressableKey() {
            string key = string.Empty;
            
            // 라이브 오브젝트와 일러스트 분리 
            if(isMinicut) 
                key = StoryManager.main.CurrentProjectID + "/live_object/" + liveName + ".prefab";
            else 
                key = StoryManager.main.CurrentProjectID + "/live_illust/" + liveName + ".prefab";
            

            return key;
        }        
        
        /// <summary>
        /// 에셋번들로 등록된 모델 로드
        /// </summary>
        void InitAddressableCubismModel() {
            addressableKey = GetAddressableKey();
            Debug.Log("## Check Live asset key :: " + addressableKey);
            
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
                    Debug.Log(string.Format("<color=yellow>[{0} ]Not in AssetBundle</color>", liveName));
                    InitCubismModel(); // 기존 캐릭터 로드 로직 호출 
                }
            }; // ? end of LoadResourceLocationsAsync            
        }
        
        /// <summary>
        /// 어드레서블 불불러오기 성공 후 처리 
        /// </summary>
        /// <param name="__result"></param>
        void SetAddressableCubismModel(AsyncOperationHandle<GameObject> __result) {
            // * 에셋번들로 불러왔지만, FadeMotion 과 실제 모션 개수가 일치하지 않는 경우는 다시 파괴하고
            // * 기존 로딩 로직을 시작해야한다. 
            mountedModelAddressable = __result;
            liveImage = mountedModelAddressable.Result.GetComponent<CubismModel>(); 
            
            // CubismModel이 없다..!? 
            if(liveImage == null) {
                Debug.LogError("Error in creating Live2D Model in addressable : " + liveName);
                NetworkLoader.main.ReportRequestError(liveName, "It's failed to create CubismModel in addressable");
                SendFailMessage();
                return;
            }
            
            // * 모션 유효성 체크 
            if(!SystemManager.CheckAddressableCubismValidation(liveImage)) {
                Debug.LogError("#### Not match fade and motion Clips :: " + liveName);
                // ! 생성한 에셋을 release 시키고, 다시 기존 로직을 호출한다. 
                Addressables.ReleaseInstance(mountedModelAddressable);
                InitCubismModel();
                return;
            }
            
                     
            
            
            ModelClips clips = liveImage.gameObject.GetComponent<ModelClips>();  // 에셋번들에서 받아온 AnimationClips. 
            CubismMotionController cubismMotionController = liveImage.gameObject.GetComponent<CubismMotionController>(); //
            
            isAddressable = true;
            SetGameModelController();  // 크기 및 레이어 처리 
            
            // 모션 처리
            string file_key = string.Empty;
            string motion_name = string.Empty;
            string clip_name = string.Empty;
            string debugMotionName = string.Empty;
            
            // 생으로 다운받는것과 방식이 다르다. 
            for(int i=0; i<clips.ListClips.Count;i++) {
                
                file_key = string.Empty;
                motion_name = string.Empty;
                clip_name = clips.ListClips[i].name.Replace(".anim", "");
                
                for(int j=0; j<resourceData.Count;j++) {
                    file_key = SystemManager.GetJsonNodeString(resourceData[j], CommonConst.COL_FILE_KEY);
                    motion_name = SystemManager.GetJsonNodeString(resourceData[j], CommonConst.MOTION_NAME);
                    
                    if (!file_key.Contains(GameConst.MOTION3_JSON) || string.IsNullOrEmpty(motion_name))
                        continue;
                        
                    // file_key에 이름+ motion3.json 있으면 dict에 넣는다. 
                    if(file_key.Contains("/" + clip_name + GameConst.MOTION3_JSON)) {
                        
                        // Dict에 추가하기. 
                        if(!DictMotion.ContainsKey(motion_name)) {
                            DictMotion.Add(motion_name, clips.ListClips[i]); // ADD    
                            debugMotionName += motion_name +", ";
                        }
                    }
                    
                } // ? end of j for
                
            } // ? end of i for
            // ? motion 처리 완료 
            Debug.Log(liveName + " motions : " + debugMotionName);
            this.motionController = cubismMotionController; // Addressable 에서는 Live2D 고유 모션 컨트롤러 연결 
            anim = null; 
            
            // 로딩 완료
            SendSuccessMessage();
            
        }

        /// <summary>
        /// 파괴처리 
        /// </summary>
        public void DestroyAddressableModel() {
            
            if(isAddressable && isMounted) {
                bool result = Addressables.ReleaseInstance(mountedModelAddressable);
                
                liveImageController = null;
                Debug.Log("DestroyAddressable :: " + liveName + " / " + result);
                return;
            }
            
            // 어드레서블 아니면 직접 destroy
            if(liveImageController != null) {
                liveImageController.DestroySelf();
            }
            
        }

        /// <summary>
        /// 모델 본격 로드!
        /// </summary>
        void InitCubismModel()
        {
            isAddressable = false; // 에셋번들 아니다!
            
            string file_key = string.Empty;
            string file_url = string.Empty;

            totalAssetCount = resourceData.Count; // 파일이 몇개인지 체크한다. 
            unloadAssetCount = totalAssetCount;



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
                    req.Timeout = System.TimeSpan.FromSeconds(40);
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
                        Debug.LogError(">> Failed InstantiateAsync " + liveName + " / " + handle.OperationException.Message);
                        
                        // 실패하면 개별 다운로드로 돌아간다. 
                        InitCubismModel();
                    }
                    
                }; // ? end of InstantiateAsync
            }
            else {
                InstantiateDownloadedCubismModel();
            }
        }
        
        

        /// <summary>
        /// 개별 다운로드로 내려받은 Live2D 모델 인스턴스 생성하기 
        /// </summary>
        public void InstantiateDownloadedCubismModel()
        {
            Debug.Log(">> ## InstantiateDownloadedCubismModel : " + liveName);
            
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
            
            // 완료 
            SendSuccessMessage();
        }
        
        /// <summary>
        /// 게임플레이용 컨트롤러 세팅 
        /// </summary>
        void SetGameModelController() {
            
            // GameLiveImageCtrl attach
            liveImageController = liveImage.gameObject.AddComponent<GameLiveImageCtrl>();
            liveImageController.originScale = gameScale;
            liveImageController.modelType = CommonConst.MODEL_TYPE_LIVE2D;               
            
            liveImageController.SetModel(liveImage); // 모델 할당.

            // 부모 transform 설정 
            if (GameManager.main != null)
                GameManager.main.SetIllustParent(liveImage.transform);
            else
                StoryLobbyManager.main.SetLiveParent(liveImage.transform);


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
            
            if(isAddressable) {
                // * 어드레서블 에셋을 통한 생성인 경우는 Shader 처리 추가 필요. 
                SystemManager.SetAddressableCubismShader(liveImage);
            }
        }


        /// <summary>
        /// Live2D 모델 Instanitate 하기 (일반)
        /// </summary>
        /// <param name="__path"></param>
        void CreateCubismModel(string __path)
        {
            CubismModel3Json model3Json = CubismModel3Json.LoadAtPath(GetCubismAbsolutePath(__path), CubismViewerIo.LoadAsset);

            liveImage = model3Json.ToModel();

            if (liveImage == null)
            {
                Debug.LogError("Error in creating Live2D Model : " + liveName);
                
                NetworkLoader.main.ReportRequestError(liveName, "It's failed to create CubismModel");
                
                SendFailMessage();
                return;
            }

            SetGameModelController();
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

        /// <summary>
        /// 애니메이션 재생
        /// </summary>
        public void PlayCubismAnimation()
        {
            // 모델이 비활성 상태일때 활성화가 되면 페이드인 처리를 한다. 
            if (!liveImage.gameObject.activeSelf)
                liveImageController.ActivateModel();
                
            // 일반 다운로드 버전 
            if(anim != null) {
                // 클립 재생 
                if(isMinicut) 
                    anim.PlayQueued("루프");
                else
                {
                    anim.CrossFade("시작", 0.3f);
                    anim.PlayQueued("루프", QueueMode.CompleteOthers);
                }
            }
            else { // 어드레서블 버전 
                if(isMinicut) 
                    motionController.PlayAnimation(DictMotion["루프"], 0, CubismMotionPriority.PriorityForce);
                else {
                    
                    Debug.Log(liveName + " : Addressable Play ###");
                    
                    motionController.PlayAnimation(DictMotion["시작"], 0, CubismMotionPriority.PriorityForce, false);
                    motionController.AnimationEndHandler = OnStartAnimationComplete;
                    // motionController.(DictMotion["시작"], 0, CubismMotionPriority.PriorityForce);
                }
            }
        }
        
        void OnStartAnimationComplete(float __f) {
            Debug.Log(liveName + " : Start Animation End ###");
            motionController.AnimationEndHandler = null;
            motionController.PlayAnimation(DictMotion["루프"], 0, CubismMotionPriority.PriorityForce);
        }
        

        void SendFailMessage()
        {
            isMounted = false;
            isLoaded = true; // 실패여도 로드는 완료했다고 처리한다. 
            
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
    }
}
