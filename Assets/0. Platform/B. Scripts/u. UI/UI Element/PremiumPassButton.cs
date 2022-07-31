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
            
            this.gameObject.SetActive(true);
            
            
            notificationObject.SetActive(false);
            
            
            // 프리미엄 패스 보유 여부 
            if(story.hasPremiumPass) {
                SetBadge();
                ticketObject.SetActive(false);
            }
            else {
                badgeImage.gameObject.SetActive(false);
                ticketObject.SetActive(true);
            }
            
            SetNotificationInfo();
        }
        
        void SetBadge() {
            badgeImage.gameObject.SetActive(true);
            badgeImage.SetDownloadURL(story.premiumBadgeURL, story.premiumBadgeKey);
        }
        
        void SetNotificationInfo() {
            if(isOutSide)
                return;
            
            // 조건이 충족하는 챌린지가 있는지 체크한다. 
        }
        
        
        public void OnClickButton() {
            // 작품 진입 전인지 아닌지 체크로 서로 다른 팝업을 오픈한다.
            
            if(isOutSide) {
                SystemManager.ShowNoDataPopup(CommonConst.POPUP_PREMIUM_PASS);
            }
            else {
                SystemManager.ShowNoDataPopup(CommonConst.POPUP_PREMIUM_CHALLENGE);
            }
        }
    }
}