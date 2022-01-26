using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace PIERStory {
    
    
    /// <summary>
    /// * ThreeEpisodeRow, EpisodeElement, EndingEpisodeElement 
    /// * 위 3개의 클래스는 ThreeEpisodeRow를 매개체로 서로에게 관여합니다.
    /// </summary>
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
        
        const int originHeight = 300; // 소속 엔딩이 하나도 없는 상태에서의 전체 높이 
        const int eachEndingHeight = 180; // 엔딩 각자의 높이 
        const int foldingEndingHeight = 340; // 접혀있는 상태에서의 전체 높이 
        
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
            
            if(maxEpisodeIndex >= __listEpisode.Count) {
                maxEpisodeIndex = __listEpisode.Count - 1;
            }
            
            for(int i=minEpisodeIndex; i<= maxEpisodeIndex; i++) {
                
                ListRows[currentRowIndex].InitElement(this, __listEpisode[i], currentRowIndex);
                currentRowIndex++;
            }
            
            // * 귀속 엔딩 유무 체크 
            for(int i=0; i<ListRows.Count;i++) {
                if(!ListRows[i].gameObject.activeSelf)  
                    continue;
                    
                // 귀속 엔딩이 있다..! 
                // 한 라인에서 하나만 체크하면 된다. 
                if(ListRows[i].hasDependentEnding) {
                    currentTransform.sizeDelta = foldingEndingSize;
                    currentLayoutElement.minHeight = foldingEndingHeight;
                    break;
                }
            }
            
            // * 귀속 엔딩중에 현재 플레이 차례가 있는지
            for(int i=0; i<ListRows.Count;i++) {
                
                if(!ListRows[i].gameObject.activeSelf)  
                    continue;
                
                if(ListRows[i].hasCurrentDependentEnding) {
                    ListRows[i].DelayOnClickEndingSpread(); // 약간 시간차 걸어준다.   
                }
            }            
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
        
        /// <summary>
        /// 엔딩 펼치기 
        /// </summary>
        /// <param name="__dependentEndings">소속된 엔딩 리스트</param>
        /// <param name="__owner">주인 에피소드의 column index</param>
        public void SpreadEnding(List<EpisodeData> __dependentEndings, int __owner) {
            
            int addHeight = 0;
            
            if(__dependentEndings.Count > ListEndings.Count) {
                Debug.LogError("Too many dependent ending!!!");
                return;
            }
            
            for(int i=0; i<__dependentEndings.Count;i++) {
                // 마지막 행에 대한 판단 값 추가
                ListEndings[i].InitEndingElement(this, __dependentEndings[i], i, __owner, i == __dependentEndings.Count -1?true:false);
                addHeight += eachEndingHeight;
            }
            
            currentLayoutElement.minHeight = originHeight + addHeight;
            currentTransform.sizeDelta = new Vector2(noEndingSize.x, currentLayoutElement.minHeight);
            
            // 펼치는 순간 모든 펼침 버튼을 보여주지 않는다.
            for(int i=0; i<ListRows.Count;i++) {
                if(ListRows[i].gameObject.activeSelf)
                    ListRows[i].HideSpreadButton();
            }
            
        }
        
        /// <summary>
        /// 엔딩 접기.
        /// </summary>
        public void FoldEnding() {
            for(int i=0; i < ListEndings.Count;i++) {
                ListEndings[i].gameObject.SetActive(false);
            }
            
            // 접고나서는 다시 접기 버튼만 남아있는 상태로 돌린다. 
            currentTransform.sizeDelta = foldingEndingSize;
            currentLayoutElement.minHeight = foldingEndingHeight;
            
            // 접었으니까 다시 펼치는 버튼은 보여줘야한다. (전체를 다 호출한다.)
            // 다 감췄으니까...
            for(int i=0; i<ListRows.Count;i++) {
                if(ListRows[i].gameObject.activeSelf)
                    ListRows[i].ShowSpreadButton();
            }
        }
        
        /// <summary>
        /// 에피소드의 구매 상태 업데이트 
        /// </summary>
        public void RefreshEpisodePurchaseState() {
            for(int i=0; i<ListRows.Count;i++) {
                if(ListRows[i].gameObject.activeSelf) {
                    ListRows[i].SetPurchaseState();
                }
            }
        }
    }
}