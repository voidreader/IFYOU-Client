using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {

    public class ThreeEpisodeRow : MonoBehaviour
    {
        [SerializeField] List<EpisodeElement> ListRows;
        [SerializeField] int rowIndex = 0;
        [SerializeField] int minEpisodeIndex, maxEpisodeIndex;
        
        /// <summary>
        ///  초기화
        /// </summary>
        /// <param name="__rowIndex"></param>
        public void InitRow(int __rowIndex) {
            this.gameObject.SetActive(true);
            
            int currentRowIndex = 0;
            ResetRow();
            rowIndex = __rowIndex;
            
            // 행 순서에 따라서 작품의 min, max 인덱스 설정하기 
            minEpisodeIndex = rowIndex * 3;
            maxEpisodeIndex = minEpisodeIndex + 2;
            
            for(int i=minEpisodeIndex; i<= maxEpisodeIndex; i++) {
                
                //ListRows[currentRowIndex].InitElement()
                
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