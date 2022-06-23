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
            int hashtagIndex = 1;
            
            // 심플 스크롤 스냅과의 연계 떄문에 
            // 데이터 설정은 모두 로드하고 나서 처리
            SystemManager.SetText(textTitle, story.title);
            
            readyImage.SetDownloadURL(story.thumbnailURL, story.thumbnailKey);
            
            hitCount = AbbrevationUtility.intToSimple(story.hitCount);
            likeCount = AbbrevationUtility.intToSimple(story.likeCount);
            textEye.text = hitCount;
            textLike.text = likeCount;
            
            listTags[0].Init(story.represenativeGenre); // 대표장르를 첫번째 태그로 설정
            
            
            
            
            // 해시태그 지정 
            for(int i=0; i<story.arrHashtag.Length;i++) {
                if(i >= listTags.Count)
                    break;
                    
                if(hashtagIndex > 2) // 해시태그 3개까지만 설정하고 끝낸다. 
                    break;
                    
                if(!string.IsNullOrEmpty(story.arrHashtag[i]))
                    listTags[hashtagIndex++].Init(story.arrHashtag[i]);
            }
        }
        
        public void OnClickPlay() {
            
        }
        
    }
}