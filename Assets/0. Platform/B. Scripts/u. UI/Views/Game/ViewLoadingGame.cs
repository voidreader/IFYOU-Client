using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

namespace PIERStory
{
    public class ViewLoadingGame :CommonView, IPointerClickHandler
    {
        public ImageRequireDownload loadingImage;
        
        public Image Cover; // 커버 
        
        public Image loadingBar;
        // public Image fadeImage;
        public TextMeshProUGUI textPercentage;

        public TextMeshProUGUI textTitle;
        public TextMeshProUGUI textInfo;

        int loadingTextIndex = 0;

        const string KEY_LOADING_TEXT = "loading_text";
        
        void Start() {
            textTitle.text = string.Empty;
            textInfo.text = string.Empty;
            textPercentage.text = string.Empty;
        }

        public override void OnStartView()
        {
            base.OnStartView();
            
            Debug.Log("GameLoading OnStartView");
            
            
            
            // 커버 이미지 처리 
            Cover.color = new Color(0,0,0,1);
            Cover.gameObject.SetActive(true);
            
            // * 페이드 이미지보다 로딩이 빨리 끝나는 경우가 있더라....
            /*
            fadeImage.color = new Color(0, 0, 0, 1);
            fadeImage.gameObject.SetActive(true);
            */

            // loadingImage.OnDownloadImage = CallbackDownloadLoadingImage;

            textTitle.text = string.Empty;
            loadingBar.fillAmount = 0;
            
            StartCoroutine(RoutineGameLoading());
        }

        void CallbackDownloadLoadingImage()
        {
            Cover.DOFade(0, 0.4f);    
        }

        IEnumerator RoutineGameLoading()
        {
            // 게임 매니저에서 에피소드 스크립트 정보 가져올 때까지 대기
            yield return new WaitUntil(() => GameManager.main.isScriptFetch);
            
            
            Debug.Log("<color=cyan>Script Fetched</color>");
            SystemManager.SetText(textTitle, GameManager.main.currentEpisodeData.episodeTitle);
            
            StartCoroutine(RoutineDebugLoading());
            
            loadingImage.OnDownloadImage = CallbackDownloadLoadingImage; // 로딩 이미지 다운로드 콜백. 

            if (GameManager.main.loadingJson.Count > 0) {
                loadingImage.SetDownloadURL(SystemManager.GetJsonNodeString(GameManager.main.loadingJson[0], CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(GameManager.main.loadingJson[0], CommonConst.COL_IMAGE_KEY), true);
            }
            else {
                loadingImage.SetDownloadURL(string.Empty, string.Empty);
            }

            if (GameManager.main.loadingDetailJson.Count > 0)
            {
                loadingTextIndex = 0;
                textInfo.text = SystemManager.GetJsonNodeString(GameManager.main.loadingDetailJson[loadingTextIndex], KEY_LOADING_TEXT);
            }
            
            // 로딩 광고는 로딩 할때 처리하고, 로딩 완료하고 플레이하는 것으로 변경됨.
            // AdManager.main.PlayLoadingAD();
            
            while(loadingBar.fillAmount < 1) {
                yield return new WaitForSeconds(0.05f);
                loadingBar.fillAmount = GameManager.main.GetPagePrepValue();
                textPercentage.text = Mathf.RoundToInt(loadingBar.fillAmount * 100).ToString() + "%";
            }
            
            Debug.Log("Page Resource loading Done!!!");

            yield return new WaitUntil(() => GameManager.main.GetCurrentPageInitialized());
            
            Debug.Log("Page Init Done!!! gameLoadingComplete");
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_GAME, "gameLoadingComplete", string.Empty);
        }
        
        IEnumerator RoutineDebugLoading() {
            while(!GameManager.main.GetCurrentPageInitialized()) {
                yield return new WaitForSeconds(1);
                Debug.Log(GameManager.main.GetDebugDouwnloadFactor());
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 아직 jsonData가 없는 경우 터치해도 아무 동작하지 않도록 한다
            if (GameManager.main.loadingDetailJson == null || GameManager.main.loadingDetailJson.Count == 0)
                return;

            loadingTextIndex++;

            if (loadingTextIndex == GameManager.main.loadingDetailJson.Count)
                loadingTextIndex = 0;

            SystemManager.SetText(textInfo, SystemManager.GetJsonNodeString(GameManager.main.loadingDetailJson[loadingTextIndex], KEY_LOADING_TEXT));
        }
    }
}
