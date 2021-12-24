using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {
    public class MainStoryRow : MonoBehaviour
    {
        [SerializeField] List<NewStoryElement> ListRows;
        [SerializeField] int rowIndex = 0;
        [SerializeField] int minStoryIndex, maxStoryIndex;
        
        /// <summary>
        /// 
        /// </summary>
        public void InitRow(int __rowIndex) {
            
            this.gameObject.SetActive(true);
            
            int currentRowIndex = 0;
            
            ResetRow();
            
            rowIndex = __rowIndex;
            
            // 행 순서에 따라서 작품의 min, max 인덱스 설정하기 
            minStoryIndex = rowIndex * 2;
            maxStoryIndex = minStoryIndex + 1;
            
            // 3개씩 활성화 
            for(int i=minStoryIndex; i<= maxStoryIndex; i++) {
                
                // 작품 개수 오버하면 끝..!
                if(StoryManager.main.listRecommendStory.Count <= i)
                    break;
                
                ListRows[currentRowIndex].InitStoryElement(StoryManager.main.listRecommendStory[i]);
                currentRowIndex++;
            }
           
        }
        
        
        /// <summary>
        /// 비활성 상태에서 시작 
        /// </summary>
        void ResetRow() {
            for(int i=0; i<ListRows.Count;i++) {
                ListRows[i].gameObject.SetActive(false);
            }
        }
    }
}