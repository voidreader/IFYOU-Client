using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {

    public class PopupHowToPlay : PopupBase
    {
        
        public GameObject freeCheck; // 프리 봤니 
        public GameObject starCheck; // 스타 플레이 봤어? 
        public GameObject premiumCheck; // 프리미엄 패스도 봤어??
        
        public override void Show()
        {
            base.Show();
            
            
            
        }

        /// <summary>
        /// 클리어 리워드 요청 
        /// </summary>        
        public void RequestClearReward() {
            
            // 이미 보상 받았으면 return
            if(UserManager.main.isHowToPlayClear)
                return;
                
            // 3개 모두 활성화  상태여야 한다. 
            if(!freeCheck.activeSelf || !starCheck.activeSelf || !premiumCheck.activeSelf)
                return;
            
            
            // 통신 
            UserManager.main.RequestHowToPlayTutorialClear();
        }
    }
}