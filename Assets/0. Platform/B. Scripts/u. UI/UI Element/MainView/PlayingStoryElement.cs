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
        
        JsonData storyJSON = null; // 작품 정보
        
        
        /// <summary>
        /// 초기화 
        /// </summary>
        /// <param name="__j"></param>
        public void InitElement(JsonData __j) {
            this.gameObject.SetActive(true);
            storyJSON = __j;
            
            projectID = storyJSON[LobbyConst.STORY_ID].ToString(); 
            imageURL = storyJSON[LobbyConst.IFYOU_PROJECT_BANNER_URL].ToString();
            imageKey = storyJSON[LobbyConst.IFYOU_PROJECT_BANNER_KEY].ToString();
            progressValue = float.Parse(storyJSON[LobbyConst.STORY_PROJECT_PROGRESS].ToString());
            
            bannerImage.SetDownloadURL(imageURL, imageKey);
        }
        
        
        /// <summary>
        /// 클릭!
        /// </summary>
        public void OnClickElement() {
            StoryManager.main.RequestStoryInfo(projectID, storyJSON);
        }
    }

}