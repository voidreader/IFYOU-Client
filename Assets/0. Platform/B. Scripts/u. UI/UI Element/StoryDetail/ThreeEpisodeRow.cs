using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace PIERStory {

    public class ThreeEpisodeRow : MonoBehaviour
    {
        [SerializeField] List<EpisodeElement> ListRows; // 정규, 사이드 친구들 
        [SerializeField] int rowIndex = 0;
        [SerializeField] int minEpisodeIndex, maxEpisodeIndex;
        
        [SerializeField] List<EndingEpisodeElement> ListEndings; // 엔딩 친구들. 
        [SerializeField] RectTransform currentTransform;
        [SerializeField] LayoutElement currentLayoutElement;
        
        readonly Vector2 noEndingSize = new Vector2(720, 300); // 해금 엔딩이 없는 경우에 대한 사이즈
        readonly Vector2 foldingEndingSize = new Vector2(720, 340); // 해금 엔딩이 있지만, 접혀있는 상태에 대한 사이즈
        
        const int eachEndingHeight = 180; // 엔딩 높이 
        
        /// <summary>
        ///  초기화
        /// </summary>
        /// <param name="__rowIndex"></param>
        public void InitRow(List<EpisodeData> __listEpisode,  int __rowIndex) {
            this.gameObject.SetActive(true);
            
            int currentRowIndex = 0;
            
            ResetRow();
            rowIndex = __rowIndex;

            // * 에피소드 설정             
            // 행 순서에 따라서 작품의 min, max 인덱스 설정하기 
            minEpisodeIndex = rowIndex * 3;
            maxEpisodeIndex = minEpisodeIndex + 2;
            
            for(int i=minEpisodeIndex; i<= maxEpisodeIndex; i++) {
                
                ListRows[currentRowIndex].InitElement(__listEpisode[i]);
                currentRowIndex++;
            }
            
            // * 귀속 엔딩 설정
            
        }
        
        /// <summary>
        /// 비활성 상태에서 시작 
        /// </summary>
        void ResetRow() {
            for(int i=0; i<ListRows.Count;i++) {
                ListRows[i].gameObject.SetActive(false);
            }
            
            for(int i=0; i<ListEndings.Count;i++) {
                ListEndings[i].gameObject.SetActive(false);
            }
            
            // 사이즈 처리 
            currentTransform.sizeDelta = noEndingSize;
            currentLayoutElement.minHeight = noEndingSize.y;
        }
    }
}