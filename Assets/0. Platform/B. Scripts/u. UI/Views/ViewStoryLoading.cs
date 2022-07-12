using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

using TMPro;
using DG.Tweening;



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
            
            loadingBar.DOFillAmount(1, 3).OnComplete(()=> {
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, "showStoryLobby", "Testing");
                //SceneManager.LoadSceneAsync(CommonConst.SCENE_STORY_LOBBY, LoadSceneMode.Single).allowSceneActivation = true;
            });
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
                loadingBar.fillAmount = 0.2f;
                StoryManager.main.SetLobbyBubbleMaster();
                Debug.Log("어드레서블 번들 없거나 실패하고 말풍선 스프라이트 추가 끝!");
                loadingBar.fillAmount = 0.3f;
                // 스토리 로비 View에게 장착된 꾸미기 아이템 준비시킨다. 
                //ViewStoryLobby.OnDecorateSet?.Invoke();
                //yield return new WaitUntil(() => ViewStoryLobby.loadComplete);
                loadingBar.fillAmount = 0.4f;
                Debug.Log("어드레서블 번들 없거나 실패하고 꾸미기형 로비 세팅 끝!");
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
                loadingBar.fillAmount = 0.2f;
                StoryManager.main.SetLobbyBubbleMaster();
                Debug.Log("어드레서블 번들 다운로드 데이터 없고 말풍선 스프라이트 추가 끝!");
                // 스토리 로비 View에게 장착된 꾸미기 아이템 준비시킨다. 
                //ViewStoryLobby.OnDecorateSet?.Invoke();
                //yield return new WaitUntil(() => ViewStoryLobby.loadComplete);
                Debug.Log("어드레서블 번들 다운로드 데이터 없고 꾸미기형 로비 세팅 끝!");
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
                //Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_STORY_DETAIL, "open!");
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
            StoryManager.main.SetLobbyBubbleMaster();
            Debug.Log("어드레서블 번들 다운 완료하고 말풍선 스프라이트 추가 끝!");
            // 스토리 로비 View에게 장착된 꾸미기 아이템 준비시킨다. 
            //ViewStoryLobby.OnDecorateSet?.Invoke();
            //yield return new WaitUntil(() => ViewStoryLobby.loadComplete);
            Debug.Log("어드레서블 번들 다운 완료하고 꾸미기형 로비 세팅 끝!");

            Addressables.Release(downloadHandle);

            yield return new WaitForSeconds(0.1f);
            
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

            SystemManager.SetText(textInfo, SystemManager.GetJsonNodeString(StoryManager.main.loadingDetailJson[loadingTextIndex], "loading_text"));
            
        }
        
    }
}