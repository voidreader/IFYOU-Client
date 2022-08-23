using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

using TMPro;
using DG.Tweening;



namespace PIERStory {
    public class ViewStoryLoading : CommonView, IPointerClickHandler
    {
        
        // public AsyncOperationHandle<IList<IResourceLocation>> bundleCheckHandle;
        public AsyncOperationHandle<long> downloadSizeHandle;
        public AsyncOperationHandle downloadHandle;
        
        public ImageRequireDownload loadingImage;
        public static bool assetLoadComplete = false;
        
        [SerializeField] bool hasBundle = false;
        
        public Image loadingBar;
        // public Image fadeImage;
        public TextMeshProUGUI textPercentage;

        public TextMeshProUGUI textTitle;
        public TextMeshProUGUI textInfo;        
        int loadingTextIndex = 0;
        
        
        public override void OnStartView() {
            base.OnStartView();
            
            SystemManager.HideNetworkLoading();
            assetLoadComplete = false;

            hasBundle = false;
            
            textTitle.text = string.Empty;
            textInfo.text = string.Empty;
            textPercentage.text = "0%";
            loadingBar.fillAmount = 0;
            
            
            if (StoryManager.main.loadingJson != null && StoryManager.main.loadingJson.Count > 0) {
                loadingImage.SetDownloadURL(SystemManager.GetJsonNodeString(StoryManager.main.loadingJson[0], CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(StoryManager.main.loadingJson[0], CommonConst.COL_IMAGE_KEY), true);
            }
            else {
                loadingImage.SetDownloadURL(string.Empty, string.Empty);
            }

            loadingTextIndex = 0;

            if (StoryManager.main.loadingDetailJson != null && StoryManager.main.loadingDetailJson.Count > 0)
                SystemManager.SetText(textInfo, SystemManager.GetJsonNodeString(StoryManager.main.loadingDetailJson[loadingTextIndex], "loading_text"));
            
        }
        
        
        public override void OnView() {
            base.OnView();
            
            Debug.Log("### Current Loading Story :: " + StoryManager.main.CurrentProject.title);
            
            // 가짜 말풍선 활성화 처리 
            if(BubbleManager.main != null) {
                BubbleManager.main.ShowFakeBubbles(true); 
            }
            
            
            
            StartCoroutine(CheckingBundleExists(StoryManager.main.CurrentProjectID));
        }
        
        void Update() {
            textPercentage.text = GetFillAmountPercentage();    
        }
               
        
        void FillProgressorOnly() {
            
            Debug.Log("### FillProgressorOnly ###");
            assetLoadComplete = true;
            loadingBar.DOFillAmount(1, 3);
        }
        
        string GetFillAmountPercentage() {
            return Mathf.RoundToInt(loadingBar.fillAmount * 100).ToString() + "%";
        }
        
        
        IEnumerator CheckingBundleExists(string __projectID) {
            
            StoryManager.main.CurrentProject.isPlaying = true;

            AsyncOperationHandle<IList<IResourceLocation>> bundleCheckHandle = Addressables.LoadResourceLocationsAsync(__projectID);
            yield return bundleCheckHandle;
            
            if( bundleCheckHandle.Status != AsyncOperationStatus.Succeeded) { // 실패
                    Debug.Log("#### This project LoadResourceLocationsAsync failed !!!! ####");
            
                    hasBundle = false;
            }
            else { // 성공이지만 
                if(bundleCheckHandle.Result.Count > 0) {
                    // 번들 있음
                    hasBundle  = true;
                   Debug.Log("#### This project has bundle!!!! ####");
                   
                }
                else {
                    
                    Debug.Log("#### This project has zero bundle!!!! ####");
                    
                    // 번들 없음 
                    hasBundle = false;
                }
            }
            
            Debug.Log("### LoadResourceLocationsAsync END, hasBundle : " + hasBundle);
            
            if(!hasBundle) {
                
                yield return new WaitUntil(() => StoryManager.main.LoadingBubbleCount <= 0); // 대화 템플릿 말풍선 로딩 체크 추가
                Debug.Log("어드레서블 번들 없거나 실패하고 말풍선 로딩 끝!");
                loadingBar.fillAmount = 0.4f;
                yield return null;

                FillProgressorOnly();
                yield break;
            }
            
            
            // 있으면 다운로드 체크 
            AsyncOperationHandle<long> getDownloadSizeHandle = Addressables.GetDownloadSizeAsync(__projectID);
            
            yield return getDownloadSizeHandle;
            
            Debug.Log("### GetDownloadSizeAsync END, size : " + getDownloadSizeHandle.Result);
            
            // 다운로드 할 데이터 없음 
            if(getDownloadSizeHandle.Result <= 0) {
                
                yield return new WaitUntil(() => StoryManager.main.LoadingBubbleCount <= 0); // 대화 템플릿 말풍선 로딩 체크 추가
                Debug.Log("어드레서블 번들 다운로드 데이터 없고 말풍선 말풍선 로딩 끝!");
                loadingBar.fillAmount = 0.4f;
                yield return null;

                FillProgressorOnly();
                yield break; // 여기서 종료. 
            }
            
            // * 다운로드할 데이터가 있는 경우에 대한 처리 시작!!!
            // * 2022.06.29 다운로드가 시작되기 전에 이전 버전을 삭제한다.
            Addressables.ClearDependencyCacheAsync(__projectID);
            
            
            Debug.Log("### Asset bundle download start ###");
            // 다운로드 해야한다. 
            AsyncOperationHandle downloadHandle =  Addressables.DownloadDependenciesAsync(__projectID);
            
            downloadHandle.Completed += (op) => {
                
            };
            
            
            DownloadStatus downloadStatus;
            
            while(!downloadHandle.IsDone) {
                
                // 다운로드 얼만큼 되었는지 표시하기 
                downloadStatus = downloadHandle.GetDownloadStatus();
                loadingBar.fillAmount = downloadStatus.Percent;
                yield return null;
            }


            yield return new WaitUntil(() => StoryManager.main.LoadingBubbleCount <= 0); // 대화 템플릿 말풍선 로딩 체크 추가
            Debug.Log("어드레서블 번들 다운 완료하고 말풍선 말풍선 로딩 끝!");

            Addressables.Release(downloadHandle);
            Debug.Log("#### This project bundle download doen! ####");
            assetLoadComplete = true;
        }
            
        
        /// <summary>
        /// 어드레서블 다운로드 
        /// </summary>
        /// <param name="__projectID"></param>
        /// <returns></returns>
        IEnumerator DownloadingAddressable(string __projectID) {
            
            
            
            // 하나의 프로젝트는 아래 5개의 어드레서블 그룹으로 나누어져 있다. 
            string voiceBundle = __projectID + "_voice";
            string imageBundle = __projectID + "_image";
            string modelBundle = __projectID + "_model";
            string soundBundle = __projectID + "_sound";
            string liveBundle = __projectID + "_live";
            
            bool hasDownloadBundle = false;
            
            StoryManager.main.CurrentProject.isPlaying = true; // 현재 프로젝트 플레이 처리 
            
            yield return null;
            
            // 하나씩 다운받기 시작한다. 
            // * 캐릭터 모델부터 받는다. 
            downloadSizeHandle = Addressables.GetDownloadSizeAsync(modelBundle);
            yield return downloadSizeHandle;
            
            // 성공 및 Result가 0보다 클때만. 
            if(downloadSizeHandle.Status == AsyncOperationStatus.Succeeded && downloadSizeHandle.Result > 0) {
                hasDownloadBundle = true; // 다운받을것이 있음. 
                Debug.Log(modelBundle + "need to be downloaded ###");
                
                loadingBar.fillAmount = 0;
                // 텍스트 변경 처리 및 로딩 게이지 처리 

                downloadHandle = Addressables.DownloadDependenciesAsync(modelBundle);
                while(downloadHandle.Status == AsyncOperationStatus.None) {
                    loadingBar.fillAmount = downloadHandle.GetDownloadStatus().Percent;
                    yield return null;
                }
                
                if(downloadHandle.Status != AsyncOperationStatus.Succeeded) { // 다운로드 실패에 대한 처리 
                    // 메세지 알림 후, 
                    // 다시 처음부터.. 진행해야하나? 
                    Debug.LogError(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
                }
                
                Addressables.Release(downloadHandle);
                
                
            } 
            else {
                Debug.Log(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
            }
            // * 캐릭터 모델 처리 종료
            
            
            // * 이미지 그룹 다운로드
            downloadSizeHandle = Addressables.GetDownloadSizeAsync(imageBundle);
            yield return downloadSizeHandle;
            
            // 성공 및 Result가 0보다 클때만. 
            if(downloadSizeHandle.Status == AsyncOperationStatus.Succeeded && downloadSizeHandle.Result > 0) {
                hasDownloadBundle = true; // 다운받을것이 있음. 
                Debug.Log(imageBundle + "need to be downloaded ###");
                
                loadingBar.fillAmount = 0;
                // 텍스트 변경 처리 및 로딩 게이지 처리 

                downloadHandle = Addressables.DownloadDependenciesAsync(imageBundle);
                while(downloadHandle.Status == AsyncOperationStatus.None) {
                    loadingBar.fillAmount = downloadHandle.GetDownloadStatus().Percent;
                    yield return null;
                }
                
                if(downloadHandle.Status != AsyncOperationStatus.Succeeded) { // 다운로드 실패에 대한 처리 
                    // 메세지 알림 후, 
                    // 다시 처음부터.. 진행해야하나? 
                    Debug.LogError(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
                }
                
                Addressables.Release(downloadHandle);
            } 
            else {
                Debug.Log(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
            }
            // * 이미지 그룹 처리 종료            


            // * 라이브 그룹 다운로드
            downloadSizeHandle = Addressables.GetDownloadSizeAsync(liveBundle);
            yield return downloadSizeHandle;
            
            // 성공 및 Result가 0보다 클때만. 
            if(downloadSizeHandle.Status == AsyncOperationStatus.Succeeded && downloadSizeHandle.Result > 0) {
                hasDownloadBundle = true; // 다운받을것이 있음. 
                Debug.Log(liveBundle + "need to be downloaded ###");
                
                loadingBar.fillAmount = 0;
                // 텍스트 변경 처리 및 로딩 게이지 처리 

                downloadHandle = Addressables.DownloadDependenciesAsync(liveBundle);
                while(downloadHandle.Status == AsyncOperationStatus.None) {
                    loadingBar.fillAmount = downloadHandle.GetDownloadStatus().Percent;
                    yield return null;
                }
                
                if(downloadHandle.Status != AsyncOperationStatus.Succeeded) { // 다운로드 실패에 대한 처리 
                    // 메세지 알림 후, 
                    // 다시 처음부터.. 진행해야하나? 
                    Debug.LogError(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
                }
                
                Addressables.Release(downloadHandle);
            } 
            else {
                Debug.Log(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
            }
            // * 라이브 그룹 처리 종료      
            


            // * 사운드 그룹 다운로드
            downloadSizeHandle = Addressables.GetDownloadSizeAsync(soundBundle);
            yield return downloadSizeHandle;
            
            // 성공 및 Result가 0보다 클때만. 
            if(downloadSizeHandle.Status == AsyncOperationStatus.Succeeded && downloadSizeHandle.Result > 0) {
                hasDownloadBundle = true; // 다운받을것이 있음. 
                Debug.Log(soundBundle + "need to be downloaded ###");
                
                loadingBar.fillAmount = 0;
                // 텍스트 변경 처리 및 로딩 게이지 처리 

                downloadHandle = Addressables.DownloadDependenciesAsync(soundBundle);
                while(downloadHandle.Status == AsyncOperationStatus.None) {
                    loadingBar.fillAmount = downloadHandle.GetDownloadStatus().Percent;
                    yield return null;
                }
                
                if(downloadHandle.Status != AsyncOperationStatus.Succeeded) { // 다운로드 실패에 대한 처리 
                    // 메세지 알림 후, 
                    // 다시 처음부터.. 진행해야하나? 
                    Debug.LogError(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
                }
                
                Addressables.Release(downloadHandle);
            } 
            else {
                Debug.Log(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
            }
            // * 사운드 그룹 처리 종료
            

            // * 보이스 그룹 다운로드
            downloadSizeHandle = Addressables.GetDownloadSizeAsync(voiceBundle);
            yield return downloadSizeHandle;
            
            // 성공 및 Result가 0보다 클때만. 
            if(downloadSizeHandle.Status == AsyncOperationStatus.Succeeded && downloadSizeHandle.Result > 0) {
                hasDownloadBundle = true; // 다운받을것이 있음. 
                Debug.Log(voiceBundle + "need to be downloaded ###");
                
                loadingBar.fillAmount = 0;
                // 텍스트 변경 처리 및 로딩 게이지 처리 

                downloadHandle = Addressables.DownloadDependenciesAsync(voiceBundle);
                while(downloadHandle.Status == AsyncOperationStatus.None) {
                    loadingBar.fillAmount = downloadHandle.GetDownloadStatus().Percent;
                    yield return null;
                }
                
                if(downloadHandle.Status != AsyncOperationStatus.Succeeded) { // 다운로드 실패에 대한 처리 
                    // 메세지 알림 후, 
                    // 다시 처음부터.. 진행해야하나? 
                    Debug.LogError(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
                }
                
                Addressables.Release(downloadHandle);
            } 
            else {
                Debug.Log(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
            }
            // * 보이스 그룹 처리 종료            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
            // 아직 jsonData가 없는 경우 터치해도 아무 동작하지 않도록 한다
            if (StoryManager.main.loadingDetailJson == null || StoryManager.main.loadingDetailJson.Count == 0)
                return;

            loadingTextIndex++;

            if (loadingTextIndex == StoryManager.main.loadingDetailJson.Count)
                loadingTextIndex = 0;

            SystemManager.SetText(textInfo, SystemManager.GetJsonNodeString(StoryManager.main.loadingDetailJson[loadingTextIndex], "loading_text"));
            
        }
        
    }
}