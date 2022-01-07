using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.Signals;
using LitJson;

namespace PIERStory {

    /// <summary>
    /// 진행중인 작품 Component
    /// </summary>
    public class PlayingStoryElement : MonoBehaviour
    {
        [SerializeField] ImageRequireDownload bannerImage;
        [SerializeField] string projectID = string.Empty;
        [SerializeField] string imageURL = string.Empty;
        [SerializeField] string imageKey = string.Empty;
        [SerializeField] float progressValue = 0;
        [SerializeField] Image progressor;
        
        StoryData storyData = null; // 작품 정보
        
        
        /// <summary>
        /// 초기화 
        /// </summary>
        /// <param name="__j"></param>
        public void InitElement(StoryData data) {
            this.gameObject.SetActive(true);
            storyData = data;
            
            progressValue = storyData.projectProgress;
            
            progressor.fillAmount = progressValue;
            
            bannerImage.SetDownloadURL(storyData.circleImageURL, storyData.circleImageKey);
        }
        
        
        /// <summary>
        /// 클릭!
        /// </summary>
        public void OnClickElement() {
            StoryManager.main.RequestStoryInfo(storyData);
        }
    }

}