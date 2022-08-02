using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace PIERStory {
    public class IfyouPassButton : MonoBehaviour
    {
        public TextMeshProUGUI textDayLeft;
        
        
        public void SetPass() {
            
            if(UserManager.main == null)
                return;
            
            this.gameObject.SetActive(UserManager.main.ifyouPassDay > 0);
            
            if(!this.gameObject.activeSelf)
                return;
                
            
            // 잔여일자 표시
            SystemManager.SetText(textDayLeft, UserManager.main.GetIFyouPassExpireMessage());
        }
        
        public void OnClickButton() {
            SystemManager.ShowNoDataPopup(CommonConst.POPUP_IFYOU_PASS);
        }
    }
}