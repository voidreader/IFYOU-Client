using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


namespace PIERStory {
    public class ViewStoryLoading : CommonView, IPointerClickHandler
    {
        public ImageRequireDownload loadingImage;
        
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
            
            hasBundle = false;
            
            textTitle.text = string.Empty;
            textInfo.text = string.Empty;
            textPercentage.text = string.Empty;
            loadingBar.fillAmount = 0;
            
            
            if (StoryManager.main.loadingJson.Count > 0) {
                loadingImage.SetDownloadURL(SystemManager.GetJsonNodeString(StoryManager.main.loadingJson[0], CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(StoryManager.main.loadingJson[0], CommonConst.COL_IMAGE_KEY));
            }
            else {
                loadingImage.SetDownloadURL(string.Empty, string.Empty);
            }

            if (StoryManager.main.loadingDetailJson.Count > 0)
            {
                loadingTextIndex = 0;
                textInfo.text = SystemManager.GetJsonNodeString(StoryManager.main.loadingDetailJson[loadingTextIndex], "loading_text");
            }
            
        }
        
        
        public override void OnView() {
            base.OnView();
            
            Debug.Log("### Current Loading Story :: " + StoryManager.main.CurrentProject.title);
            
            
            StartCoroutine(CheckingBundleExists(StoryManager.main.CurrentProjectID));
        }
        
        void FillProgressorOnly() {
            
            Debug.Log("### FillProgressorOnly ###");
            
            loadingBar.DOFillAmount(1, 3).OnComplete(()=> {
                //Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_STORY_DETAIL, "open!");
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, "showStoryLobby", "Testing");
            });
        }
        
        
        IEnumerator CheckingBundleExists(string __projectID) {
            
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
                ViewStoryLobby.OnDecorateSet?.Invoke();
                
                yield return new WaitUntil(() => ViewStoryLobby.loadComplete);

                FillProgressorOnly();
                yield break;
            }
            
            
            // 있으면 다운로드 체크 
            AsyncOperationHandle<long> getDownloadSizeHandle = Addressables.GetDownloadSizeAsync(__projectID);
            
            yield return getDownloadSizeHandle;
            
            Debug.Log("### GetDownloadSizeAsync END, size : " + getDownloadSizeHandle.Result);
            
            // 다운로드 할 데이터 없음 
            if(getDownloadSizeHandle.Result <= 0) {
                ViewStoryLobby.OnDecorateSet?.Invoke();
                
                // yield return new WaitUntil(() => ViewStoryLobby.loadComplete);

                FillProgressorOnly();
                yield break;
            }
            
            
            Debug.Log("### Asset bundle download start ###");
            // 다운로드 해야한다. 
            AsyncOperationHandle downloadHandle =  Addressables.DownloadDependenciesAsync(__projectID);
            downloadHandle.Completed += (op) => {
                //Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_STORY_DETAIL, "open!");

            };
            
            while(!downloadHandle.IsDone) {
                loadingBar.fillAmount = downloadHandle.PercentComplete;
                yield return null;
            }

            ViewStoryLobby.OnDecorateSet?.Invoke();
            // yield return new WaitUntil(() => ViewStoryLobby.loadComplete);
            
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, "showStoryLobby", "Testing");

            Debug.Log("#### This project bundle download doen! ####");
        }
            

        public void OnPointerClick(PointerEventData eventData)
        {
            
            // 아직 jsonData가 없는 경우 터치해도 아무 동작하지 않도록 한다
            if (StoryManager.main.loadingDetailJson == null || StoryManager.main.loadingDetailJson.Count == 0)
                return;

            loadingTextIndex++;

            if (loadingTextIndex == StoryManager.main.loadingDetailJson.Count)
                loadingTextIndex = 0;

            textInfo.text = SystemManager.GetJsonNodeString(StoryManager.main.loadingDetailJson[loadingTextIndex], "loading_text");
            
        }
        
    }
}