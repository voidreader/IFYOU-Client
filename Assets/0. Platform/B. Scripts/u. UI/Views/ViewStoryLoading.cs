using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;


namespace PIERStory {
    public class ViewStoryLoading : CommonView, IPointerClickHandler
    {
        public ImageRequireDownload loadingImage;
        
        public Image loadingBar;
        // public Image fadeImage;
        public TextMeshProUGUI textPercentage;

        public TextMeshProUGUI textTitle;
        public TextMeshProUGUI textInfo;        
        int loadingTextIndex = 0;
        
        
        public override void OnStartView() {
            base.OnStartView();
            
            SystemManager.HideNetworkLoading();
            
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
            
            loadingBar.DOFillAmount(1, 3).OnComplete(()=> {
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_STORY_DETAIL, "open!");
            });
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