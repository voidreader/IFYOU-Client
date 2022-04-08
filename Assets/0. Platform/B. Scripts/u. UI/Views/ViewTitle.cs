using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;
using TMPro;
using DG.Tweening;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace PIERStory {
    public class ViewTitle : CommonView
    {
        
        public static Action<string> ActionTitleLoading = null;
        
        public RawImage mainImage; // 다운로드받아서 보여주는 플랫폼 로딩 화면 
        
        [SerializeField] TextMeshProUGUI textLoading;
        [SerializeField] int totalDownloadingImageCount = 0;
        public string currentStep = string.Empty;
        
        public GameObject downloadProgressParent;
        [SerializeField] Image downloadProgressBar; // 에셋번들 다운로드 게이지 
        
        public Doozy.Runtime.Reactor.Progressor progressor; //  접속 로딩 게이지 
        public bool isCheckingAssetBundle = false; 
        
        
        [SerializeField] GameObject baseScreen; // 기본 스크린 
        const string fontAssetBundle = "Font";
        
        private void Awake() {
            textLoading.text = string.Empty;
            
            isCheckingAssetBundle = false;
            
            // 다운로드 프로그레서는 비활성화 해놓고 시작한다.
            downloadProgressParent.SetActive(false);
            downloadProgressBar.fillAmount = 0;
            
            // 접속 프로그레서 활성화 
            progressor.gameObject.SetActive(true);
            
            ActionTitleLoading = UpdateTitleLoading;
            
            
            mainImage.gameObject.SetActive(false);
            baseScreen.SetActive(true);
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void OnStartView() {
            base.OnStartView();
            
            // 타이틀 이미지 설정             
            SetTitleTexture();
            
            UpdateLoadingText(1); // 텍스트 설정
        }
        
        public override void OnView() {
            base.OnView();
            
            Debug.Log("<color=cyan>ViewTitle OnView</color>");
            AdManager.main.AnalyticsEnter("titleEnter");
            
            // 언어변경등으로 강제로 씬로딩이 진행될때. 
            if(SystemManager.IsGamebaseInit && UserManager.main.completeReadUserData && string.IsNullOrEmpty(currentStep)) {
                UpdateTitleLoading("login");
            }
            
        }
        
        
        /// <summary>
        /// 타이틀 로딩 처리 
        /// </summary>
        /// <param name="__step"></param>        
        void UpdateTitleLoading(string __step) {
            
            Debug.Log(string.Format("<color=cyan>## UpdateTitleLoading [{0}] </color>", __step));
            
            switch(__step) {
                case "gamebase": 
                // progressor.SetProgressAt(0.5f);
                progressor.PlayToProgress(0.5f);
                break;
                
                case "login": // 게임 서버 로그인 완료 
                UpdateLoadingText(2);
                // progressor.SetProgressAt(1f);
                progressor.PlayToProgress(1);
                break;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__v"></param>
        public void OnProgressChanged(float __v) {
            if(__v >= 1 && !isCheckingAssetBundle) {
                StartCoroutine(CheckingAssetBundle());
                isCheckingAssetBundle = true; // 혹시 여러번 돌릴까봐...
                
                downloadProgressParent.SetActive(true);
                progressor.gameObject.SetActive(false); 
            }
        }
        
        IEnumerator CheckingAssetBundle() {
            
            Debug.Log("<color=cyan>## CheckingAssetBundle START</color>");
            
            UpdateLoadingText(3);
            
            // 카탈로그 업데이트까지 대기한다. 
            while(!SystemManager.main.isAddressableCatalogUpdated)
                yield return null;
                
            Debug.Log("<color=cyan>## CheckingAssetBundle #1 </color>");
            
            bool hasDownloadableBundle = false;
            
            AsyncOperationHandle<IList<IResourceLocation>> bundleCheckHandle = Addressables.LoadResourceLocationsAsync(fontAssetBundle);
            yield return bundleCheckHandle;
            
            if( bundleCheckHandle.Status != AsyncOperationStatus.Succeeded) { // 실패
                    Debug.Log("<color=cyan>## No Font bundle </color>");
                    hasDownloadableBundle = false;
                    
                    // Font 에셋번들 다운받지 받지 못하면 게임에 접속 할 수 없음
                    SystemManager.ShowSystemPopup(SystemManager.GetDefaultServerErrorMessage(), NetworkLoader.OnFailedServer, NetworkLoader.OnFailedServer, false, false);
                    NetworkLoader.main.ReportRequestError(bundleCheckHandle.OperationException.ToString(), "Font LoadResourceLocationsAsync");
                    yield break;
            }
            else { // 성공이지만 
                if(bundleCheckHandle.Result.Count > 0) {
                    // 번들 있음
                    hasDownloadableBundle  = true;
                   Debug.Log("<color=cyan>## Font bundle is downloading </color>");
                }
                else {
                    
                    Debug.Log("<color=cyan> all font bundle is downloaded </color>");
                    
                    // 번들 없음 
                    hasDownloadableBundle = false;
                }
            }
            
            // 다운로드 할 거 없음 
            if(!hasDownloadableBundle) {
                
                FillProgressorOnly();
            
            }
            
            // * 번들 체크 완료 후 있으면 다운로드 시작 
            AsyncOperationHandle<long> getDownloadSizeHandle = Addressables.GetDownloadSizeAsync(fontAssetBundle);
            yield return getDownloadSizeHandle;
            Debug.Log("### GetDownloadSizeAsync END, size : " + getDownloadSizeHandle.Result);
            // 다운로드 할 데이터 없음 
            if(getDownloadSizeHandle.Result <= 0) {
                
                // 폰트 부른다. 
                SystemManager.main.LoadAddressableFont();
                FillProgressorOnly();
                yield break;
            }
            
            Debug.Log("### Asset bundle download start ###");
            AsyncOperationHandle downloadHandle =  Addressables.DownloadDependenciesAsync(fontAssetBundle);
            downloadHandle.Completed += (op) => {
                
                if(op.Status != AsyncOperationStatus.Succeeded) {
                    // 다운로드 실패!?
                    SystemManager.ShowSystemPopup(SystemManager.GetDefaultServerErrorMessage(), NetworkLoader.OnFailedServer, NetworkLoader.OnFailedServer, false, false);
                    NetworkLoader.main.ReportRequestError(bundleCheckHandle.OperationException.ToString(), "Font DownloadDependenciesAsync");
                }
            };            
            
            DownloadStatus downloadStatus;
            
            // 게이지 채우기 
            while(!downloadHandle.IsDone) {
                downloadStatus = downloadHandle.GetDownloadStatus();
                downloadProgressBar.fillAmount = downloadStatus.Percent;
                yield return null;
            }            
            
            // 다운로드 완료됨 
            Debug.Log("<color=cyan>font bundle downloading is done!!!</color>");
            
            yield return null;
            
            // 폰트 부른다. 
            SystemManager.main.LoadAddressableFont();
            
            StartCoroutine(MovingNextScene());
        }
        
        
        /// <summary>
        /// 진입에 필요한 과정이 모드 완료되면 호출 
        /// </summary>
        /// <returns></returns>
        IEnumerator MovingNextScene() {
            Debug.Log("<color=cyan>MoveingNextScene START</color>");

            UserManager.main.RequestUserGradeInfo();
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork()); // 서버 통신 종료되길 기다린다. 
            yield return new WaitUntil(() =>SystemManager.main.mainAssetFont != null); // 폰트 불러오길 기다린다. 
            
            Debug.Log("<color=cyan>MoveingNextScene END</color>");
            
            Signal.Send(LobbyConst.STREAM_IFYOU, "moveMain", "open!"); // ViewMain으로 이동한다.
        }
        
        
        
    
        /// <summary>
        /// 에셋번들 다운로드 게이지 채우기만 하고 넘기기 
        /// </summary>
        void FillProgressorOnly() {
                
                Debug.Log("### FillProgressorOnly ###");
                
                downloadProgressBar.DOFillAmount(1, 3).OnComplete(()=> {
                    StartCoroutine(MovingNextScene());
                });
        }
        
        
        
                
        
        
        
        
        /// <summary>
        /// 로딩 텍스트 변환 
        /// </summary>
        /// <param name="step"></param>
        void UpdateLoadingText(int step) {
            string loadingText = GetPlatformLoadingText(step);
            Debug.Log(loadingText);
            
            textLoading.text = loadingText;
        }
        
        

  
        
        IEnumerator RoutinePrepareMainPage() {

            SystemManager.main.QueryPushTokenInfo();
            NetworkLoader.main.RequestPlatformServiceEvents(); // 공지사항, 프로모션, 장르 조회 
            
            // NetworkLoader.main.RequestComingSoonList(); // 커밍순 리스트 요청
            NetworkLoader.main.RequestAttendanceList(); // 출석 보상 리스트 요청
            DownloadStoryMainImages(); // 여기 추가 

            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());

            /*
            DownloadStoryMainImages(); // 다운로드 요청 
            
            // 다운로드 완료될때까지 기다린다.
            yield return new WaitUntil(()=> totalDownloadingImageCount <= 0);
            
            // 게이지 다 찰때까지 기다린다.
            // yield return new WaitUntil(()=> progressBar.fillAmount >= 1);
         
            // 준비 끝났으면 signal 전송 
            Signal.Send(LobbyConst.STREAM_IFYOU, "moveMain", "open!");
            */
        }
        
        
        /// <summary>
        /// 메인화면에 필요한 이미지 다운받기 
        /// </summary>
        void DownloadStoryMainImages() {
            totalDownloadingImageCount = 0;
            StoryData currentData;
            string imageURL = string.Empty;
            string imageKey = string.Empty;
            
            for(int i=0; i<StoryManager.main.listTotalStory.Count;i++) {
                
                currentData = StoryManager.main.listTotalStory[i];
                   
                imageURL = currentData.categoryImageURL;
                imageKey = currentData.categoryImageKey;
                
                if(string.IsNullOrEmpty(imageURL) || string.IsNullOrEmpty(imageKey))
                    continue;

                // 이미 있으면 continue;                
                if(ES3.FileExists(imageKey))
                    continue;
                
                // Debug.Log(imageURL +" / " + imageKey);
                
                // 다운로드 요청 
                totalDownloadingImageCount++; // 유효한 URL당 
                SystemManager.RequestDownloadImage(imageURL, imageKey, OnDownloadEachMainImage);
            }
        }
        
        // 다운로드 이미지 콜백, 성공시에 1씩 차감한다.
        void OnDownloadEachMainImage(HTTPRequest request, HTTPResponse response) {
            
            Debug.Log(string.Format("OnDownloadEachMainImage [{0}] / [{1}]", request.State, response.IsSuccess));
            
            // 성공시, 로컬 저장 
            if(request.State == HTTPRequestStates.Finished && response.IsSuccess) {
                totalDownloadingImageCount--;
            }
        }
        
        
        /// <summary>
        /// 타이틀 texture 설정 
        /// </summary>
        void SetTitleTexture() {
            Texture2D downloadedLoadingTexture = LobbyManager.main.GetRandomPlatformLoadingTexture();
            
            // 다운로드 텍스쳐가 올바르면 교체한다. 
            if(downloadedLoadingTexture != null) {
                mainImage.gameObject.SetActive(true);
                baseScreen.SetActive(false);
                mainImage.texture = downloadedLoadingTexture;
            }
            else { // 다운로드 텍스쳐가 없는경우 
                baseScreen.SetActive(true);
                mainImage.gameObject.SetActive(false);
            }
        }
        
        
        
        /// <summary>
        /// 랜덤 로딩 화면 조회하기.
        /// </summary>
        /// <returns></returns>
        Texture2D GetRandomPlatformLoadingTexture() {
            
            JsonData data = null; // 로컬에 저장된 로딩 이미지 목록 
            int imageIndex = 0; // 랜덤 index 
            string imageKey = string.Empty; // 불러올 이미지 key 
            string imageURL = string.Empty; // 불러올 이미지 url
            Texture2D selectedTexture = null;
            
            
            if(!ES3.KeyExists(SystemConst.KEY_PLATFORM_LOADING))
                return null;
                
            
            data = JsonMapper.ToObject(ES3.Load<string>(SystemConst.KEY_PLATFORM_LOADING));
            Debug.Log("GetRandomPlatformLoadingTexture : " + JsonMapper.ToStringUnicode(data));
            
            if(data == null || data.Count == 0)
                return null;
            
            imageIndex = UnityEngine.Random.Range(0, data.Count);
            imageKey = data[imageIndex][SystemConst.IMAGE_KEY].ToString();
            imageURL = data[imageIndex][SystemConst.IMAGE_URL].ToString();
            
            if(string.IsNullOrEmpty(imageKey) || string.IsNullOrEmpty(imageURL)) 
                return null;
                
            selectedTexture = SystemManager.GetLocalTexture2D(imageKey);
            
           
            return selectedTexture;
        }
        
        
        /// <summary>
        /// 플랫폼 로딩의 텍스트 
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformLoadingText(int step) {
            
            Debug.Log("> GetPlatformLoadingText : " + step);
            
            string currentAppLang = string.Empty;
            
            // 로컬라이징 정보가 최초 실행시에는 없다.
            if(!ES3.KeyExists(SystemConst.KEY_LANG)) { // 없는 경우 
            
                Debug.Log("GetPlatformLoadingText : " + Application.systemLanguage);
                
                switch(Application.systemLanguage) {
                    case SystemLanguage.Korean:
                    currentAppLang = "KO";
                    break;
                    
                    case SystemLanguage.Japanese:
                    currentAppLang = "JA";
                    break;
                    
                    default:
                    currentAppLang = "EN";
                    break;
                }
                
                
            }
            else { // 있는 경우 
                currentAppLang = ES3.Load<string>(SystemConst.KEY_LANG);
                currentAppLang = currentAppLang.ToUpper();
            }
            
            switch(step) {
                
                case 1: // 디폴트
                if(currentAppLang == "KO") 
                    return "서버에 접속합니다.";
                else if(currentAppLang == "JA") 
                    return "サーバーに接続します。";
                else
                    return "Connecting to server.";
                
                case 2: // 게임베이스, 플랫폼 로그인 
                if(currentAppLang == "KO") 
                    return "플랫폼 정보를 불러오고 있습니다.";
                else if(currentAppLang == "JA") 
                    return "プラットフォーム情報の要求。";
                else
                    return "Requesting platform information.";
                
                case 3: // 에셋번들 다운로드
                Debug.Log("33333333333333333");
                if(currentAppLang == "KO") 
                    return "게임에 필요한 데이터를 다운받고 있습니다.";
                else if(currentAppLang == "JA") 
                    return "ゲームに必要なデータをダウンロードしています。";
                else
                    return "Downloading necessary game data.";
                
            }
            
            return string.Empty;
            
            
        }
    }
}