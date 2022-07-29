using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PIERStory {
    public class PremiumPassButton : MonoBehaviour
    {
        
        public bool isOutSide = false; // 외부에 노출된 버튼인지? 
        
        public StoryData story;
        
        public GameObject notificationObject; // 알림 
        
        public ImageRequireDownload badgeImage; // 뱃지 이미지
        public GameObject ticketObject; // 티켓 오브젝트 
        public bool isPurchasable = false;
        
        
        /// <summary>
        /// 설정 
        /// </summary>
        /// <param name="__story"></param>
        public void SetPass(StoryData __story) {
            story = __story;
            
            notificationObject.SetActive(false);
        }
        
        
        
        public void OnClickButton() {
            
        }
    }
}