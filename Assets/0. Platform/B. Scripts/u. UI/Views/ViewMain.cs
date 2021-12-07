using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class ViewMain : CommonView
    {
        [SerializeField] ScrollRect mainScrollRect;
        
        [SerializeField] List<PlayingStoryElement> ListPlayingStoryElements; // 진행중 이야기 
        [SerializeField] List<MainStoryRow> ListRecommendStoryRow; // 추천 스토리의 2열짜리 행 
        [SerializeField] List<NewStoryElement> ListNewStoryElement; // 새로운 이야기 개별 개체 
        
        float mainScrollRectY = 0;
        
        public override void OnView()
        {
            base.OnView();
            
        }
        
        public override void OnStartView() {
            
            base.OnStartView();
            
            Signal.Send(LobbyConst.STREAM_IFYOU, "initNavigation", string.Empty);
            
            InitPlayingStoryElements(); // 진행중인 이야기 Area 초기화 
            InitRecommendStory(); // 추천스토리 Area 초기화
            InitNewStoryElements(); // 새로운 이야기 Area 초기화
            
            
            // 탑 처리 추가 
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_VIEW_NAME, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            
            
        }
        
        void Update() {
        
            /*
            if(ViewCommonTop.staticCurrentTopOwner != this.gameObject.name)
                return;
            
            if(mainScrollRect.content.transform.localPosition.y > 150f && !ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
                return;
            }
            
            
            if(mainScrollRect.content.transform.localPosition.y <= 150f && ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
                return;
            }
            */
            
        }
        
        
        /// <summary>
        /// 진행중인 이야기 초기화
        /// </summary>
        void InitPlayingStoryElements() {
            ResetPlayingStoryElements();
            
            int elementIndex = 0;
            
            for(int i=0;i<StoryManager.main.totalStoryListJson.Count;i++) {
                
                if(!SystemManager.GetJsonNodeBool(StoryManager.main.totalStoryListJson[i], LobbyConst.STORY_IS_PLAYING))
                    continue;
                
                // 진행기록이 있는 작품만 가져온다.                 
                ListPlayingStoryElements[elementIndex++].InitElement(StoryManager.main.totalStoryListJson[i]);
            }
        }
        
        /// <summary>
        /// 진행중인 이야기 Reset
        /// </summary>
        void ResetPlayingStoryElements() {
            for(int i=0; i<ListPlayingStoryElements.Count;i++) {
                ListPlayingStoryElements[i].gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 새로운 이야기 Area 초기화 
        /// </summary>
        void InitRecommendStory() {
            ResetRecommendStory();
            
            // 작품개수를 2로 나눈다. 
            int dividedIntoTwo = Mathf.FloorToInt((float)StoryManager.main.totalStoryListJson.Count / 2f );
            
            // 2배수로 나눈 수만큼 초기화 시작.
            for(int i=0; i<dividedIntoTwo; i++) {
                ListRecommendStoryRow[i].InitRow(i);
            }
            
        }
        
        /// <summary>
        /// 신규 스토리 세팅 
        /// </summary>
        void InitNewStoryElements() {
            ResetNewStory();
            
            
            for(int i=0; i<StoryManager.main.totalStoryListJson.Count;i++) {
                ListNewStoryElement[i].InitStoryElement(StoryManager.main.totalStoryListJson[i]);
            }   
        }
        
        
        void ResetRecommendStory() {
            for(int i=0; i<ListRecommendStoryRow.Count; i++) {
                ListRecommendStoryRow[i].gameObject.SetActive(false);
            }
        }
        
        void ResetNewStory() {
            for(int i=0; i<ListNewStoryElement.Count; i++) {
                ListNewStoryElement[i].gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 메인ScrollRect 상하 변경시.. 상단 제어 
        /// </summary>
        /// <param name="vec"></param>
        public void OnValueChangedMainScroll(Vector2 vec) {
            
            if(mainScrollRectY == vec.y)
                return;
                
            mainScrollRectY = vec.y;
            
            if(mainScrollRectY < 0.95f && !ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
                return;
            }
           
            
            if(mainScrollRectY >= 0.95f && ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
                return;
            }
        }
        
    }
}