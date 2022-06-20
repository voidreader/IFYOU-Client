using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {
    public class RecommendStory : MonoBehaviour
    {
        public StoryData story;
        public TextMeshProUGUI textTitle;
        public ImageRequireDownload readyImage;
        
        public string hitCount = string.Empty;
        public string likeCount = string.Empty;
        
        /// <summary>
        /// 초기화 하자..
        /// </summary>
        /// <param name="__story"></param>
        public void Init(StoryData __story) {
            
            story = __story;
            // SetData();
        }
        
        
        public void SetData() {
            
            Debug.Log("Set Data ");
            
            // 심플 스크롤 스냅과의 연계 떄문에 
            // 데이터 설정은 모두 로드하고 나서 처리
            textTitle.text = story.title;
            readyImage.SetDownloadURL(story.thumbnailURL, story.thumbnailKey);
            
            hitCount = AbbrevationUtility.AbbreviateNumber(story.hitCount);
            likeCount = AbbrevationUtility.AbbreviateNumber(story.likeCount);
        }
        
        public void OnClickPlay() {
            
        }
        
    }
}