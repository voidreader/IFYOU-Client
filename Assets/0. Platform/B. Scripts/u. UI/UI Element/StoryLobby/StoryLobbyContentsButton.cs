using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {

    /// <summary>
    /// 스토리 컨텐츠 타입
    /// </summary>
    public enum StoryContentsType {
        Gallery,
        Ending,
        Selection,
        Mission,
        Special
    }

    public class StoryLobbyContentsButton : MonoBehaviour
    {
        public StoryContentsType contentsType;
        
        public bool hasNewContents = false; // 확인하지 않은 신규 컨텐츠 존재
        public GameObject newSign; // 신규 컨텐츠 존재시 표시 사인 
        
        
        /// <summary>
        /// 초기화
        /// </summary>
        public void InitContentsButton() {
            
            newSign.SetActive(false); 
            
            switch(contentsType) {
                case StoryContentsType.Gallery:
                break;
                
                case StoryContentsType.Ending:
                break;
                
                case StoryContentsType.Selection:
                break;
                
                case StoryContentsType.Mission:
                break;
                
            }
            
        }        
        
    }
}