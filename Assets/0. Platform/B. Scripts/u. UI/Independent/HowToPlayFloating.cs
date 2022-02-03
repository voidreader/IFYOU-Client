using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PIERStory {
    public class HowToPlayFloating : MonoBehaviour
    {
        
        public static Action RefreshHowToPlayState = null;
        
        public GameObject starBonus; // 스타보너스 
        
        void OnEnable() {
            
            RefreshHowToPlayState = OnEnable;
            
            if(UserManager.main == null) {
                starBonus.SetActive(false);
                return;
            }
                
            starBonus.SetActive(!UserManager.main.isHowToPlayClear);
        }
    }
}