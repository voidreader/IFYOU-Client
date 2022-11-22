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
using Doozy.Runtime.Signals;


namespace PIERStory {
    public class ViewStoryLoading : CommonView, IPointerClickHandler
    {
        
        // public AsyncOperationHandle<IList<IResourceLocation>> bundleCheckHandle;
        public AsyncOperationHandle<long> downloadSizeHandle;
        public AsyncOperationHandle downloadHandle;
        
        public bool hasDownloadBundle = false; // 하나라도 다운받을 번들이 있었는지 ? 
        public bool isCompleteCurrentDownload = false; 
        
        public ImageRequireDownload loadingImage;
        public static bool assetLoadComplete = false;

        
        public Image loadingBar;
        // public Image fadeImage;
        

        public TextMeshProUGUI textTitle; // 로딩 텍스트 타이틀 
        public TextMeshProUGUI textInfo; // 로딩 텍스트
        
        public TextMeshProUGUI textAddressable; // 어드레서블 다운로드 안내 텍스트 
        public TextMeshProUGUI textPercentage; // 어드레서블 다운로드 %
        int loadingTextIndex = 0;
        
        
        public override void OnStartView() {
            base.OnStartView();
            
            SystemManager.HideNetworkLoading();
            
            // 변수 초기화 
            assetLoadComplete = false;
            hasDownloadBundle = false;
            
            textTitle.text = string.Empty;
            textInfo.text = string.Empty;
            textPercentage.text = "0%";
            loadingBar.fillAmount = 0;
            
            // 리소스는 처음 한번만 다운로드 받습니다~
            SystemManager.SetText(textAddressable, SystemManager.GetLocalizedText("6215"));
            
            
            
            // 로딩 이미지와 텍스트 처리 
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
            
            
            StartCoroutine(DownloadingAddressable(StoryManager.main.CurrentProjectID));
            // StartCoroutine(CheckingBundleExists(StoryManager.main.CurrentProjectID));
        }
        
        void Update() {
            textPercentage.text = GetFillAmountPercentage();    
        }
               
        
        void FillProgressorOnly() {
            Debug.Log("### FillProgressorOnly ###");
            loadingBar.fillAmount = 0;
            loadingBar.DOFillAmount(1, 1).OnComplete(()=> {
                
            });
        }
        
        string GetFillAmountPercentage() {
            return Mathf.RoundToInt(loadingBar.fillAmount * 100).ToString() + "%";
        }
        
        
           
        
        /// <summary>
        /// 어드레서블 다운로드 
        /// </summary>
        /// <param name="__projectID"></param>
        /// <returns></returns>
        IEnumerator DownloadingAddressable(string __projectID) {
            
            Debug.Log("START DownloadingAddressable : " + __projectID);
            StoryManager.main.CurrentProject.isPlaying = true; // 현재 프로젝트 플레이 처리 
            
            // 하나의 프로젝트는 아래 5개의 어드레서블 그룹으로 나누어져 있다. 
            string voiceBundle = __projectID + "_voice";
            string imageBundle = __projectID + "_image"; // 
            string modelBundle = __projectID + "_model"; // 캐릭터
            string soundBundle = __projectID + "_sound"; // 배경음, 효과음 
            string liveBundle = __projectID + "_live"; // 라이브 오브젝트, 라이브 일러스트 
            
            yield return null;
            
            // 하나씩 다운받기 시작한다. 
            // 다운로드 실패를 고려해서 3번까지 체크한다.
            
            
            yield return StartCoroutine(DownloadingAddressableGroup(modelBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(modelBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(modelBundle));
            
            
            yield return StartCoroutine(DownloadingAddressableGroup(liveBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(liveBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(liveBundle));
            
            
            yield return StartCoroutine(DownloadingAddressableGroup(imageBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(imageBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(imageBundle));
            
            
            yield return StartCoroutine(DownloadingAddressableGroup(soundBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(soundBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(soundBundle));
            
            yield return StartCoroutine(DownloadingAddressableGroup(voiceBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(voiceBundle));
            if(!isCompleteCurrentDownload)
                yield return StartCoroutine(DownloadingAddressableGroup(voiceBundle));
            
            
            yield return null;
            
            // 다운로드한 번들이 하나도 없는 경우 
            if(!hasDownloadBundle) {
                FillProgressorOnly();
            }
        
            
            // 다 받았으면 넘어간다. 
            // 혹시라도 말풍선 정보가 완료되지 않았으면 기다린다.
            yield return new WaitUntil(() => StoryManager.main.LoadingBubbleCount <= 0);
            
            Debug.Log("#### Addressable Check donn! ####");
            assetLoadComplete = true; // 
            Signal.Send(LobbyConst.STREAM_IFYOU, "storyLobbyLoadComplete", string.Empty);
            
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
        
        
        /// <summary>
        /// 텍스트 처리 
        /// </summary>
        /// <param name="__groupName"></param>
        void SetAddressableDownloadText(string __groupName) {
            if(__groupName.Contains(CommonConst.POSTFIX_MODEL_BUNDLE)) 
                SystemManager.SetText(textAddressable, SystemManager.GetLocalizedText("6478"));
            else if(__groupName.Contains(CommonConst.POSTFIX_LIVE_BUNDLE)) 
                SystemManager.SetText(textAddressable, SystemManager.GetLocalizedText("6479"));
            else if(__groupName.Contains(CommonConst.POSTFIX_IMAGE_BUNDLE)) 
                SystemManager.SetText(textAddressable, SystemManager.GetLocalizedText("6480"));
            else if(__groupName.Contains(CommonConst.POSTFIX_SOUND_BUNDLE)) 
                SystemManager.SetText(textAddressable, SystemManager.GetLocalizedText("6481"));
            else if(__groupName.Contains(CommonConst.POSTFIX_VOICE_BUNDLE)) 
                SystemManager.SetText(textAddressable, SystemManager.GetLocalizedText("6482"));
        }
        
        
        /// <summary>
        /// 어드레서블 그룹 다운로드 처리 
        /// </summary>
        /// <param name="__groupName"></param>
        /// <returns></returns>
        IEnumerator DownloadingAddressableGroup(string __groupName) {
            Debug.Log(">> DownloadingAddressableGroup : " + __groupName);
            
            isCompleteCurrentDownload = false; // 다운로드 완료 체크 용도의 변수 
            
            // 다음의 순서로 진행된다.
            // 그룹 유무 체크 => 다운로드 필요여부 => 다운로드 (있으면) 
            
            
            // 대상 그룹이 있는지 체크한다.
            AsyncOperationHandle<IList<IResourceLocation>> existsHandle = Addressables.LoadResourceLocationsAsync(__groupName);
            yield return existsHandle;
            
            if(existsHandle.Status != AsyncOperationStatus.Succeeded) {
                Debug.LogError(string.Format("[{0}] : [{1}]", existsHandle.Status.ToString(), existsHandle.OperationException.Message));   
                Addressables.Release(existsHandle);
                yield break;
            }
            else {
                // 없음 
                if(existsHandle.Result.Count <= 0) {
                    Debug.Log("No Group : " + __groupName);
                    Addressables.Release(existsHandle);
                    isCompleteCurrentDownload = true; // true로 처리하고 코루틴 종료
                    yield break;
                }
            }
            Addressables.Release(existsHandle);
            
            
            
            // 다운로드 사이즈 체크 
            downloadSizeHandle = Addressables.GetDownloadSizeAsync(__groupName);
            isCompleteCurrentDownload = true;
            yield return downloadSizeHandle;
            
            // 성공 및 Result가 0보다 클때만. (다운로드 받아야 함)
            if(downloadSizeHandle.Status == AsyncOperationStatus.Succeeded && downloadSizeHandle.Result > 0) {
                Debug.Log(__groupName + "need to be downloaded ###");
                
                
                Addressables.ClearDependencyCacheAsync(__groupName); // 이전에 받았던 데이터는 지우고 시작한다.
               
                loadingBar.fillAmount = 0;
                hasDownloadBundle = true; // 다운로드 번들 있음!
                SetAddressableDownloadText(__groupName); // 텍스트 설정 
                
                // 텍스트 변경 처리 및 로딩 게이지 처리 
                downloadHandle = Addressables.DownloadDependenciesAsync(__groupName);
                while(downloadHandle.Status == AsyncOperationStatus.None) {
                    loadingBar.fillAmount = downloadHandle.GetDownloadStatus().Percent;
                    yield return null;
                }
                
                if(downloadHandle.Status == AsyncOperationStatus.Succeeded) { // 다운로드 실패에 대한 처리 
                    isCompleteCurrentDownload = true;  // 정상적으로 다운로드 받음 
                }
                else {
                    // 에러 리포트 
                    Debug.LogError(string.Format("[{0}] : [{1}]", downloadHandle.Status.ToString(), downloadHandle.OperationException.Message));
                    NetworkLoader.main.ReportRequestError(downloadHandle.OperationException.Message, "DownloadingAddressableGroup");
                }
                
                Addressables.Release(downloadHandle);
            } 
            else {
                
                // 성공, 다운로드 받을게 없음 
                if(downloadSizeHandle.Status == AsyncOperationStatus.Succeeded) {
                    isCompleteCurrentDownload = true;    
                }
                else { // 실패 
                    Debug.Log(string.Format("[{0}] : [{1}]", downloadSizeHandle.Status.ToString(), downloadSizeHandle.OperationException.Message));
                    isCompleteCurrentDownload = false;    
                }
            }
        }
        
    } //? End of DownloadingAddressableGroup
}