using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Toast.Gamebase;

namespace PIERStory {

    public class PopupExpireToken : PopupBase
    {
        
        public TextMeshProUGUI textExplain;
        
        
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            
            string lastLoggedInProvider = Gamebase.GetLastLoggedInProvider();
            if(string.IsNullOrEmpty(lastLoggedInProvider)) {
                lastLoggedInProvider = "GUEST";
            }
            
            // 설명글 추가 
            textExplain.text = string.Format(SystemManager.GetLocalizedText("6241"), lastLoggedInProvider);
            
        }
        
        
        
        
        
        public void OnClickGoogle() {
            SystemManager.main.LoginByExpireToken(GamebaseAuthProvider.GOOGLE);
            
            base.Hide();
        }
        
        public void OnClickApple() {
            SystemManager.main.LoginByExpireToken(GamebaseAuthProvider.APPLEID);
            
            base.Hide();
        }
        
        public void OnClickGuest() {
            SystemManager.main.LoginByExpireToken(GamebaseAuthProvider.GUEST);
            
            base.Hide();
        }
        
    }
}