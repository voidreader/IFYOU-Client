using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace PIERStory {
    
    
    /// <summary>
    /// 원데이 패스 뱃지
    /// </summary>
    public class OnedayPassButton : MonoBehaviour
    {
        
        // 원데이 패스의 경우 한번만 구매가 가능하다.
        // 유효기간이 종료되면 버튼 자체가 비활성된다. 
        
        public StoryData story;
        public GameObject badgeObject; // 뱃지 이미지
        public GameObject ticketObject; // 티켓 오브젝트 
        public TextMeshProUGUI textTimer;
        
        public bool isPurchasable = false;
        
        
        void Update() {
            if(!badgeObject.activeSelf)
                return;
                
            if(Time.frameCount % 5 != 0)
                return;
                
           // 원데이패스 타이머 
            textTimer.text = story.GetOnedayRemainTime();
            if(string.IsNullOrEmpty(textTimer.text)) {
                // 끝나면 비활성화시킨다. 
                this.gameObject.SetActive(false);
            }
        }
        
        
        public void SetPass(StoryData __story) {
            story = __story;
            
            // 원데이 패스 구매내역, 유효기간을 체크한다.
            ticketObject.SetActive(string.IsNullOrEmpty(story.onedayExpireDate));
            badgeObject.SetActive(!string.IsNullOrEmpty(story.onedayExpireDate) && story.IsValidOnedayPass());
            
            // 뱃지가 활성화되는 경우는 유효시간이 남았을때.
            this.gameObject.SetActive(ticketObject.activeSelf || badgeObject.activeSelf);
            
            isPurchasable = ticketObject.activeSelf; // 구매 가능여부에 대한 처리 
        }
        
        
        public void OnClickButton() {
            
            SystemManager.ShowNoDataPopup(CommonConst.POPUP_ONEDAY_PASS);
        }
        
    }
}