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
        JsonData storyJSON = null; // 작품 정보
        
        
        public void InitElement(JsonData __j) {
            this.gameObject.SetActive(true);
            storyJSON = __j;
        }
    }

}