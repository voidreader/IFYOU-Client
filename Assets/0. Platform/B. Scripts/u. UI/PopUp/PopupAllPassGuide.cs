using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {

    public class PopupAllPassGuide : PopupBase
    {
        
        public TextMeshProUGUI textTimer; 
        public bool isCountable = false;
        
        public override void Show()
        {
            base.Show();
            
            
            // 카운트 가능 여부 체크 
            if(!string.IsNullOrEmpty(UserManager.main.GetAllPassTimeDiff())) {
                isCountable = true;
            }
            else {
                isCountable = false;  // 만료되었으면 카운트 00:00:00으로 수동처리
                textTimer.text = "00:00:00";
            }
            
            
        }
        
        void Update() {
            if(!isCountable)
                return;
            
            
            if(Time.frameCount % 5 == 0) {
                textTimer.text = UserManager.main.GetAllPassTimeDiff();
                
                if(string.IsNullOrEmpty(textTimer.text)) {
                    isCountable = false;
                    textTimer.text = "00:00:00";
                }
            }
        }
    }
}