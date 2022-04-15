using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

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
        JsonData galleryData = null;
        
        public Color colorDisable; 
        public Image buttonImage; 
        
        
        
        /// <summary>
        /// 초기화
        /// </summary>
        public void InitContentsButton() {
            
            try {
                
                
                newSign.SetActive(false); 
                
                
                // 컨텐츠 타입에 따라 처리가 다르다. 
                switch(contentsType) {
                    case StoryContentsType.Gallery:
                    CheckNewGalleryData();
                    break;
                    
                    case StoryContentsType.Ending:
                    CheckNewEndingData();
                    break;
                    
                    case StoryContentsType.Special:
                    CheckNewSpecialEpisodes();
                    break;
                    
                    case StoryContentsType.Mission:
                    CheckUnlockMission(); 
                    break;
                    
                }
            } catch(System.Exception e) {
                
            }
            
        }
        
        
        /// <summary>
        /// 알림 표시 처리 
        /// </summary>
        /// <param name="__flag"></param>
        void SetNotification(bool __flag) {
            newSign.SetActive(__flag);
        }
        
        
        /// <summary>
        /// 미션 데이터 체크 
        /// </summary>
        void CheckUnlockMission() {
            
            // * 미션은 보상받지 않은 미션이 있는 경우에 보여준다. 
            int unlockMissionCount = UserManager.main.GetUnlockStateMissionCount();
            
            if(unlockMissionCount > 0) {
                SetNotification(true);
            }
            else {
                SetNotification(false);
            }
        }        
        
        /// <summary>
        /// 신규 스페셜 에피소드 체크 
        /// </summary>
        void CheckNewSpecialEpisodes() {
            
           
            // 스페셜 에피소드 => 스페셜 에피소드 보유개수에 따라서 버튼 색상 조정 
            if(StoryManager.main.sideEpisodeCount <= 0) {
                buttonImage.color = colorDisable;
                

            }
            else {
                buttonImage.color = Color.white;;
            }
            
            
            for(int i=0; i<StoryManager.main.SideEpisodeList.Count;i++) {
                if(StoryManager.main.SideEpisodeList[i].isUnlock && StoryManager.main.SideEpisodeList[i].purchaseState == PurchaseState.None) {
                    SetNotification(true);
                    return;
                }
            }
            
            SetNotification(false);
        }


        void CheckNewEndingData()
        {
            SetNotification(UserManager.main.GetInCompleteEndingCount() > 0);
            Debug.Log("미열람 엔딩 갯수 = " + UserManager.main.GetInCompleteEndingCount());
        }
        
        /// <summary>
        /// 갤러리 ! 
        /// </summary>
        void CheckNewGalleryData() {
            galleryData = UserManager.main.GetUserGalleryImage();
            
            if(galleryData == null) {
                SetNotification(false);
                return;
            }
            
            // valid, gallery_open 체크 
            // 오픈했으면서, valid true 이면서 gallery_open false인 경우 새로운 일러스트 있다고 판단.
            for(int i=0; i<galleryData.Count;i++) {
                if(SystemManager.GetJsonNodeBool(galleryData[i], "illust_open") 
                    && SystemManager.GetJsonNodeBool(galleryData[i], "valid") 
                    && !SystemManager.GetJsonNodeBool(galleryData[i], "gallery_open")) {
                    
                    SetNotification(true);
                    return;
                }
            }
            
            SetNotification(false);
        }
        
        public void OnClickContentsButton() {
            if(contentsType == StoryContentsType.Special) {
                
                if(StoryManager.main.SideEpisodeList.Count == 0) {
                    SystemManager.ShowSimpleAlertLocalize("6304");
                    return;
                }
                else {
                    // 스페셜 에피소드 띄우기. 
                    Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, "showSpecialEpisode", string.Empty);        
                    return;
                }
            }
            
            
            
        }
        
    }
}