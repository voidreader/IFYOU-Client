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
        
        public List<StoryHashtag> listTags; // 해시태그
        
        public TextMeshProUGUI textEye; // 조회수 
        public TextMeshProUGUI textLike; // 선호작수
        
       

        
        /// <summary>
        /// 초기화 하자..
        /// </summary>
        /// <param name="__story"></param>
        public void Init(StoryData __story) {
            
            for(int i=0;i<listTags.Count;i++) {
                listTags[i].gameObject.SetActive(false);
            }
            
            story = __story;
            // SetData();
        }
        
        
        public void SetData() {
            
            Debug.Log("Set Data ");
            
            // 심플 스크롤 스냅과의 연계 떄문에 
            // 데이터 설정은 모두 로드하고 나서 처리
            textTitle.text = story.title;
            readyImage.SetDownloadURL(story.thumbnailURL, story.thumbnailKey);
            
            hitCount = AbbrevationUtility.intToSimple(story.hitCount);
            likeCount = AbbrevationUtility.intToSimple(story.likeCount);
            textEye.text = hitCount;
            textLike.text = likeCount;
            
            // 해시태그 지정 
            for(int i=0; i<story.arrHashtag.Length;i++) {
                if(i >= listTags.Count)
                    break;
                    
                if(!string.IsNullOrEmpty(story.arrHashtag[i]))
                    listTags[i].Init(story.arrHashtag[i]);
            }
        }
        
        public void OnClickPlay() {
            
        }
        
    }
}