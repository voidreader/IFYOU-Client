using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using BestHTTP;
using LitJson;


namespace PIERStory {
    public class PopupAdvertisementShow : PopupBase
    {
        
        
        [SerializeField] TextMeshProUGUI textTimer; // 타이머 
        
        
        [SerializeField] float timer = 5;
        float maxTimer = 0;
        
        
        public override void Show() {
            if(isShow)
                return;
            
            base.Show();
            
            StartCoroutine(RoutineTimer());
        }
        
        IEnumerator RoutineTimer() {
            
            yield return null;
            
            
            timer = 3.2f ;
            maxTimer = timer;
            
            while(timer > 0) {
                timer -= Time.deltaTime;
                textTimer.text = Mathf.RoundToInt(timer).ToString();
                
                yield return null;
            }
            
            yield return null;
           
           // 종료 후 광고 노출. 
           this.Hide();
            
        }
        
        public override void Hide() {
            base.Hide();
            
            AdManager.main.ShowInGameInterstitial();
        }
    }
}
