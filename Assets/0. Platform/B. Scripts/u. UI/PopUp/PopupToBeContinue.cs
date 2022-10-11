using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {

    public class PopupToBeContinue : PopupBase
    {
        
        public static bool isShowingToBeContinue = false;
        
        public TextMeshProUGUI textContents;
        
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();

            Invoke("SetFlag", 4);
            Invoke("AutoHide", 6);
            
        }
        
        void AutoHide() {
            this.Hide();
            
            // 이 팝업은 게임씬에서만 사용해야 한다!
            // 전면광고 추가한다.
            AdManager.main.ShowAdmobInterstitial();
        }
        
        void SetFlag() {
            isShowingToBeContinue = false; // 하이드 시점에 false 로 처리 
        }
        

    }
}