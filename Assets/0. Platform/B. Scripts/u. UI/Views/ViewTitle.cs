using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

using TMPro;
using DG.Tweening;
using Doozy.Runtime.Signals;


namespace PIERStory {
    public class ViewTitle : CommonView
    {
        
        public RawImage mainImage; // 다운로드받아서 보여주는 플랫폼 로딩 화면 
        
        [SerializeField] TextMeshProUGUI textLoading;
        public string currentStep = string.Empty;
        
        public GameObject circleLoading;
        public GameObject downloadProgressParent;
        [SerializeField] Image downloadProgressBar; // 에셋번들 다운로드 게이지 
        
        
        public bool isCheckingAssetBundle = false; 
        
        
        [SerializeField] GameObject baseScreen; // 기본 스크린 
        const string fontAssetBundle = "Font";
        const string bubbleAssetBundle = "Bubble";
        

        static string currentAppLang = string.Empty;
        
        // 타이틀 진입 => 게임베이스 Initialize 대기, 로그인, 초기 에셋번들 다운로드 필요하다. 

        private void Awake() {
            textLoading.text = string.Empty;
            
            isCheckingAssetBundle = false;
            
            // 다운로드 프로그레서는 비활성화 해놓고 시작한다.
            downloadProgressParent.SetActive(false);
            downloadProgressBar.fillAmount = 0;
            
            
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
            StartCoroutine(LaunchingApplication()); // 첫 진입 코루틴 시작 
            
        }
        
        
        /// <summary>
        /// 앱 진입 시작 
        /// </summary>
        /// <returns></returns>
        IEnumerator LaunchingApplication() {
            DownloadStatus downloadStatus;
            
            circleLoading.SetActive(true);
            
            
            yield return null;
            
            Debug.Log("LaunchingApplication #1");
            
            // 게임베이스 초기화 시작 
            // 1단계. 서버에 접속합니다. 
            SystemManager.main.GameBaseInitialize();
            
            // 유저 정보를 불러올때까지 대기 (게임서버 접속 후 true로 변경)
            while(!UserManager.main.completeReadUserData)
                yield return null;
                
            
            circleLoading.SetActive(false); // 원형 로딩은 바이바이 
            
            // ----------------------------------------------------------------
                
            // 2단계 폰트 에셋번들을 불러온다.
            // 게이지 보여주기 시작한다. 
            UpdateLoadingText(2); // 플랫폼 정보를 불러오고 있습니다.
            SystemManager.main.InitAddressableCatalog();  // 카탈로그 업데이트 
            downloadProgressBar.fillAmount = 0;
            downloadProgressParent.SetActive(true); // 게이지 활성화
            
            // 카탈로그 업데이트가 완료될때까지 대기 
            while(!SystemManager.main.isAddressableCatalogUpdated)
                yield return null;
                
                
            // 어드레서블 카탈로그 업데이트 후, 기본 어드레서블 다운로드 시작 
            
            
            // 기본 폰트 어드레서블을 다운로드 시작한다. 
            UpdateLoadingText(3); //  레이블 처리
            
            
            AsyncOperationHandle<IList<IResourceLocation>> fontBundleCheckHandle = Addressables.LoadResourceLocationsAsync(fontAssetBundle);
            yield return fontBundleCheckHandle;
            
            if(fontBundleCheckHandle.Status != AsyncOperationStatus.Succeeded) { // 실패
                Debug.Log("<color=cyan>## No Font bundle </color>");
                
                // Font 에셋번들 다운받지 받지 못하면 게임에 접속 할 수 없음
                SystemManager.ShowSystemPopup(SystemManager.GetDefaultServerErrorMessage(), NetworkLoader.OnFailedServer, NetworkLoader.OnFailedServer, false, false);
                NetworkLoader.main.ReportRequestError(fontBundleCheckHandle.OperationException.ToString(), "Font LoadResourceLocationsAsync");
                yield break;
            }
            
            
            // 폰트 다운로드 사이즈 체크 
            AsyncOperationHandle<long> getFontDownloadSizeHandle = Addressables.GetDownloadSizeAsync(fontAssetBundle);
            yield return getFontDownloadSizeHandle;
            
            
            
            // 이미 다운로드 받았음 
            if(getFontDownloadSizeHandle.Result <= 0) {
                Debug.Log("### [Font] GetDownloadFontSizeAsync END, size : " + getFontDownloadSizeHandle.Result);    
                SystemManager.main.LoadAddressableFont(); // 폰트 로드.
            }
            else {
                Debug.Log("### [Font] Download START");
                
                // 폰트 다운로드 필요함!! 
                AsyncOperationHandle fontDownloadHandle = Addressables.DownloadDependenciesAsync(fontAssetBundle);
                fontDownloadHandle.Completed += (op) => {

                    if (op.Status != AsyncOperationStatus.Succeeded)
                    {
                        // 다운로드 실패!?
                        SystemManager.ShowSystemPopup(SystemManager.GetDefaultServerErrorMessage(), NetworkLoader.OnFailedServer, NetworkLoader.OnFailedServer, false, false);
                        NetworkLoader.main.ReportRequestError(fontBundleCheckHandle.OperationException.ToString(), "Font DownloadDependenciesAsync");
                    }
                };
                

                // 게이지 채우기 
                while (!fontDownloadHandle.IsDone)
                {
                    downloadStatus = fontDownloadHandle.GetDownloadStatus();
                    downloadProgressBar.fillAmount = downloadStatus.Percent;
                    yield return null;
                }
                
                Debug.Log("<color=cyan>font bundle downloading is done!!!</color>");
                yield return null;

                // 폰트 로드 처리. 
                SystemManager.main.LoadAddressableFont();                
            }
            
            // ----------------------------------------------------------------
            
            // 3 단계 말풍선 이미지 다운로드 처리 
            Debug.Log("<color=cyan>Start Bubble Font Process!!!</color>");
            downloadProgressBar.fillAmount = 0;
            UpdateLoadingText(4); //  레이블 처리
            
            // 기본 말풍선 어드레서블을 다운로드 시작한다. 
            // ! 말풍선 어드레서블은 신규 말풍선 이미지가 있으면 어드레서블에 이미지를 추가한다. 
            AsyncOperationHandle<IList<IResourceLocation>> bubbleBundleCheckHandle = Addressables.LoadResourceLocationsAsync(bubbleAssetBundle);
            yield return bubbleBundleCheckHandle;
            
            if(bubbleBundleCheckHandle.Status != AsyncOperationStatus.Succeeded) { // 실패
                Debug.Log("<color=cyan>## Fail Get Bubble bundle </color>");
                FillProgressorOnly();
                // ! 말풍선 에셋번들은 다운로드를 받지 못해도 게임진입이 가능하다. 
                yield break;
            }
            
            
            // 말풍선  다운로드 사이즈 체크 
            AsyncOperationHandle<long> getBubbleDownloadSizeHandle = Addressables.GetDownloadSizeAsync(bubbleAssetBundle);
            yield return getBubbleDownloadSizeHandle;
            Debug.Log("### [Bubble] GetDownloadBubbleSizeAsync END, size : " + getBubbleDownloadSizeHandle.Result);
            
            
            // 이미 다운로드 받았음 
            if(getBubbleDownloadSizeHandle.Result <= 0) {
                FillProgressorOnly(); // 게이지 자동 채워지고 진입 완료 처리 
                yield break; // 코루틴 종료 
            }
            
            // 말풍선 다운로드 필요함!! 
            AsyncOperationHandle bubbleDownloadHandle = Addressables.DownloadDependenciesAsync(bubbleAssetBundle);
            bubbleDownloadHandle.Completed += (op) => {

                if (op.Status != AsyncOperationStatus.Succeeded)
                {
                    // 다운로드 실패!?
                    SystemManager.ShowSystemPopup(SystemManager.GetDefaultServerErrorMessage(), NetworkLoader.OnFailedServer, NetworkLoader.OnFailedServer, false, false);
                    NetworkLoader.main.ReportRequestError(bubbleDownloadHandle.OperationException.ToString(), "Bubble DownloadDependenciesAsync");
                }
            };
            
            // 게이지 채우기 
            while (!bubbleDownloadHandle.IsDone)
            {
                downloadStatus = bubbleDownloadHandle.GetDownloadStatus();
                downloadProgressBar.fillAmount = downloadStatus.Percent;
                yield return null;
            }
            
            Debug.Log("<color=cyan>Bubble bundle downloading is done!!!</color>");
            yield return null;
            
            // ----------------------------------------------------------------
            
            
            // 완료 진입 완료 처리 
            StartCoroutine(MovingNextScene());
        }
        

        
        
        /// <summary>
        /// 진입에 필요한 과정이 모드 완료되면 호출 
        /// </summary>
        /// <returns></returns>
        IEnumerator MovingNextScene() {
            Debug.Log("<color=cyan>MoveingNextScene START</color>");

            // UserManager.main.RequestUserGradeInfo(UserManager.main.CallbackUserGreadeInfo);
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
                
                downloadProgressBar.DOFillAmount(1, 2).OnComplete(()=> {
                    StartCoroutine(MovingNextScene());
                });
        }
        
        
        
        /// <summary>
        /// 타이틀 화면 텍스트 설정
        /// </summary>
        /// <param name="step"></param>
        void UpdateLoadingText(int step) {
            string loadingText = GetPlatformLoadingText(step);
            SystemManager.SetText(textLoading, loadingText);
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
        /// 플랫폼 로딩의 텍스트 
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformLoadingText(int step) {
            
            Debug.Log("> GetPlatformLoadingText : " + step);
            
            
            
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
                    
                    case SystemLanguage.Arabic:
                    currentAppLang = "AR";
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
                else if(currentAppLang == "AR") 
                    return "الاتصال بالخادم.";
                else
                    return "Connecting to server.";
                
                case 2: // 게임베이스, 플랫폼 로그인 
                if(currentAppLang == "KO") 
                    return "플랫폼 정보를 불러오고 있습니다.";
                else if(currentAppLang == "JA") 
                    return "プラットフォーム情報の要求。";
                else if(currentAppLang == "AR") 
                    return "طلب معلومات النظام الأساسي.";
                else
                    return "Requesting platform information.";
                
                case 3: // 폰트 에셋번들 다운로드
                if(currentAppLang == "KO") 
                    return "게임에 필요한 데이터를 다운받고 있습니다. [1/2]";
                else if(currentAppLang == "JA") 
                    return "ゲームに必要なデータをダウンロードしています [1/2]";
                else if(currentAppLang == "AR") 
                    return "تنزيل بيانات اللعبة الضرورية [1/2]";
                else
                    return "Downloading necessary game data. [1/2]";
                    
                case 4: // 말풍선 에셋번들 다운로드
                if(currentAppLang == "KO") 
                    return "게임에 필요한 데이터를 다운받고 있습니다. [2/2]";
                else if(currentAppLang == "JA") 
                    return "ゲームに必要なデータをダウンロードしています [2/2]";
                else if(currentAppLang == "AR") 
                    return "تنزيل بيانات اللعبة الضرورية [2/2]";
                else
                    return "Downloading necessary game data. [2/2]";
                
            }
            
            return string.Empty;
        }
    }
}