using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory {

    public abstract class PopupBase : MonoBehaviour
    {
        public PopupContent Data;
         
        public UIContainer overlay; // 오버레이 
        public UIContainer content; // 컨텐츠 
        
        
        
        // 자동파괴까지 걸리는 시간
        public float autoDestroyTime = 0;  // 0인 경우 자동파괴 사용하지 않음 
        public bool isOverlayUse = false; // 오버레이 사용 여부 
         
        
        
        void Awake() {
            // overlay.gameObject.SetActive(false);   
            // content.gameObject.SetActive(false);
        }
        
        void OnDisable() {
            
        }
        
        public void SetAutoDestroy() {
            if(autoDestroyTime <= 0 )
                return;
                
            StartCoroutine(RoutineAutoDestroy());
        }
        
        IEnumerator RoutineAutoDestroy() {
            yield return new WaitForSeconds(autoDestroyTime);
            
            Hide();
            
        }
        
        
        public virtual void Show() {
            
            StartCoroutine(ShowEnumerator());
            
            SetAutoDestroy();
        }
        
        IEnumerator ShowEnumerator() {
            yield return null;
            
            if(isOverlayUse) {
                overlay.enabled = true;
                overlay.Show();
            }
            
            content.enabled = true;
            content.Show();
            
        }
        
        
        public virtual void Hide() {
            
            if(isOverlayUse)
                overlay.Hide();
                
                
            content.Hide();
        }
        
        public void InstanteShow() {
            if(isOverlayUse)
                overlay.InstantShow();
                
                
            content.InstantShow();
            SetAutoDestroy();
        }
        
        public void InstanteHide() {
            if(isOverlayUse)
                overlay.InstantHide();
                
                
            content.InstantHide();
            SelfDestroy();
        }   
        
        public void SelfDestroy() {
            Destroy(this.gameObject);
        }
    }
}