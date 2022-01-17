using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace PIERStory {
    public class PopupAdvertisementShow : PopupBase
    {
        [SerializeField] Image progressBar;
        [SerializeField] float timer = 0;
        
        bool isInvoked = false;
        
        public override void Show() {
            base.Show();
            
            StartCoroutine(RoutineTimer());
        }
        
        IEnumerator RoutineTimer() {
            
            timer = 0 ;
            
            while(timer < 1) {
                
                yield return new WaitForSeconds(0.05f);
                                
                timer += 0.015f;
                
                if(timer >= 1)
                    timer = 1;
                
                progressBar.fillAmount = timer;
            }
            
            
            Hide();
        }
        
        public override void Hide() {
            base.Hide();
            
            if(isInvoked)
                return;
            
            AdManager.OnShowAdvertisement?.Invoke();
            isInvoked = true;
        }
    }
}
