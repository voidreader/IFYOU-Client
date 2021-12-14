using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace PIERStory {
    
    /// <summary>
    /// 스토리 컨텐츠 타입
    /// </summary>
    public enum StoryContentsType {
        Gallery,
        Ending,
        Selection,
        Mission
    }
    
    /// <summary>
    /// 갤러리, 엔딩, 선택지, 미션 컨텐츠 버튼
    /// </summary>
    public class StoryContentsButton : MonoBehaviour
    {
        public StoryContentsType contentsType;
        
        public bool hasNewContents = false; // 확인하지 않은 신규 컨텐츠 존재
        public GameObject newContentsFrame; // 신규 컨텐츠 존재시 씌워지는 프레임 
        
        JsonData originData = null;
        
        /// <summary>
        /// 컨텐츠 버튼 초기화 
        /// </summary>
        public void InitContentsButton() {
            // 컨텐츠 버튼은 초기화 될때마다 로컬에 받아온 정보를 저장해놓는다. 
            // 예를들어.. 개수 정도라도 충분할듯.. 
            // 이전에 로컬 저장했던 데이터와 지금의 데이터가 다르면 새로운게 있다고 표시해준다.
            SetNewContentsNotification(false);
            
            switch(contentsType) {
                case StoryContentsType.Gallery:
                break;
                
                case StoryContentsType.Ending:
                CheckEndingData();
                break;
                
                case StoryContentsType.Selection:
                break;
                
                case StoryContentsType.Mission:
                CheckMissionData();
                break;
                
            }
            
        }
        
        /// <summary>
        /// 미션 데이터 체크 
        /// </summary>
        void CheckMissionData() {
            
            // * 미션은 보상받지 않은 미션이 있는 경우에 보여준다. 
            int unlockMissionCount = UserManager.main.GetUnlockStateMissionCount();
            
            if(unlockMissionCount > 0) {
                SetNewContentsNotification(true);
            }
        }
        
        /// <summary>
        /// 엔딩 데이터 체크 
        /// </summary>
        void CheckEndingData() {
            
            // 프로젝트의 이전에 오픈 엔딩 카운트와 현재의 오픈 엔딩 카운트를 비교해서 설정 
            int previousOpenEndingCount = PlayerPrefs.GetInt(LobbyConst.KEY_OPEN_ENDING_COUNT + StoryManager.main.CurrentProjectID, 0);
            
            SetNewContentsNotification(StoryManager.main.unlockEndingCount > previousOpenEndingCount);
            
        }
        
        /// <summary>
        /// 선택지 데이터 체크 
        /// </summary>
        void CheckSelectionData() {
            
        }
        
        
        /// <summary>
        /// 제일 복잡한 갤러리 데이터 
        /// </summary>
        void CheckGalleryData() {
            
        }
        
        void SetNewContentsNotification(bool __flag) {
            hasNewContents = __flag;
            newContentsFrame.SetActive(hasNewContents);
        }
    }
}