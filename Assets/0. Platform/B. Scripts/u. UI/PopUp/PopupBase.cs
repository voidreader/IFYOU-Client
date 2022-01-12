using System.Collections;
using UnityEngine;

using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory {

    /// <summary>
    /// Doozy와 비슷한 팝업시스템
    /// </summary>
    public abstract class PopupBase : MonoBehaviour
    {
        public PopupContent Data;
         
        public UIContainer overlay; // 오버레이 
        public UIContainer content; // 컨텐츠 
        
        
        
        // 자동파괴까지 걸리는 시간
        public float autoDestroyTime = 0;  // 0인 경우 자동파괴 사용하지 않음 
        public bool isOverlayUse = false; // 오버레이 사용 여부 
        public bool addQueue = false; // 팝업 큐 사용여부
        
         
        
        
        public void SetAutoDestroy() {
            
            BackButton.blockBackInput = true; // block 
            
            if(PopupManager.main != null)
                PopupManager.main.AddActivePopup(this); // 팝업매니저에 등록하기
            
            
            if(autoDestroyTime <= 0 )
                return;
                
            StartCoroutine(RoutineAutoDestroy());
        }
        
        IEnumerator RoutineAutoDestroy() {
            yield return new WaitForSeconds(autoDestroyTime);
            
            Hide();
            
        }

        public virtual void Show()
        {

            StartCoroutine(ShowEnumerator());
            SetAutoDestroy();
        }
        
        IEnumerator ShowEnumerator() {
            
            if(content.inTransition)
                yield break;
            
            yield return null;
            
            if(isOverlayUse) {
                overlay.enabled = true;
                
                if(overlay.OnStartBehaviour != Doozy.Runtime.UIManager.ContainerBehaviour.Show)
                    overlay.Show();
            }
            
            content.enabled = true;
            
            Debug.Log(gameObject.name + " " + content.OnStartBehaviour.ToString());
            
            if( content.OnStartBehaviour != Doozy.Runtime.UIManager.ContainerBehaviour.Show) {
                Debug.Log("content show");
                content.Show();
            }
            
        }
        
        
        /// <summary>
        /// 팝업 Hide
        /// </summary>
        public virtual void Hide() {
            
            if(content.inTransition)
                return;
            
            BackButton.blockBackInput = false; // block 해제
            
            
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
        
        /// <summary>
        /// Hide의 콜백으로 꼭 걸어줄것!
        /// </summary>
        public void SelfDestroy() {
            
            BackButton.blockBackInput = false; // block 해제
            
            if(PopupManager.main != null)
                PopupManager.main.RemoveActivePopup(this); // remove
            
            Destroy(gameObject);
        }
        
        
        /// <summary>
        /// 긍정 버튼 클릭 
        /// </summary>
        public void OnClickPositive() {
            
            Data.positiveButtonCallback?.Invoke();
            
            Hide();
        }
        
        public void OnClickNegative() {
            
            Data.negativeButtonCallback?.Invoke();
            
            Hide();
        }
    }
}