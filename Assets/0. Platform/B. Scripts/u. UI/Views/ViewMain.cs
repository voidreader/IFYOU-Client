using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class ViewMain : CommonView
    {
        
        [SerializeField] List<PlayingStoryElement> ListPlayingStoryElements; // 진행중 이야기 
        [SerializeField] List<MainStoryRow> ListNewStoryRow; // 새로운 이야기의 3개짜리 행들. 
        
        public override void OnView()
        {
            base.OnView();
            
        }
        
        public override void OnStartView() {
            
            base.OnStartView();
            
            Signal.Send(LobbyConst.STREAM_IFYOU, "initNavigation", string.Empty);
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_OFF_BACK_BUTTON, string.Empty);
            
            InitPlayingStoryElements(); // 진행중인 이야기 Area 초기화 
            InitNewStoryRows(); // 새로운 이야기 Area 초기화
            
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
        void InitNewStoryRows() {
            ResetNewStoryRows();
            
            // 작품개수를 3으로 나눈다. 
            int dividedIntoThree = Mathf.FloorToInt((float)StoryManager.main.totalStoryListJson.Count / 3f );
            
            // 3배수로 나눈 수만큼 초기화 시작.
            for(int i=0; i<dividedIntoThree; i++) {
                ListNewStoryRow[i].InitRow(i);
            }
            
        }
        
        void ResetNewStoryRows() {
            for(int i=0; i<ListNewStoryRow.Count; i++) {
                ListNewStoryRow[i].gameObject.SetActive(false);
            }
        }
        
    }
}