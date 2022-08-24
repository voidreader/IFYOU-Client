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
        
        public string popupName = string.Empty;
        
        
        
        // 자동파괴까지 걸리는 시간
        public float autoDestroyTime = 0;  // 0인 경우 자동파괴 사용하지 않음 
        public bool isOverlayUse = false; // 오버레이 사용 여부 
        public bool addQueue = false; // 팝업 큐 사용여부
        
        public CanvasGroup mainCanvasGroup = null; // 메인 캔버스 그룹 
         
        public bool isShow = false; // Show 중복 호출 방지
        public bool isOnShow = false; // OnShow 중복 호출 방지.. 
        
        public bool isBlockBackButton = false; // 백버튼 조작 방지 (닫히지 않음 )
        
        void Awake() {
            if(mainCanvasGroup != null)
                mainCanvasGroup.alpha = 0;
        }
        
        public void InitPopup() {
            if(mainCanvasGroup != null)
                mainCanvasGroup.alpha = 0;
        }
        
        public void SetAutoDestroy() {
            
            
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
            isShow = true;
            
            StartCoroutine(ShowEnumerator());
            SetAutoDestroy();
        }
        
        IEnumerator ShowEnumerator() {
            
            
            yield return null;
            
            if(isOverlayUse) {
                overlay.enabled = true;
                
                if(overlay.OnStartBehaviour != Doozy.Runtime.UIManager.ContainerBehaviour.Show && !overlay.inTransition)
                    overlay.Show();
            }
            
            content.enabled = true;
            
            // Debug.Log(gameObject.name + " " + content.OnStartBehaviour.ToString());
            
            if( content.OnStartBehaviour != Doozy.Runtime.UIManager.ContainerBehaviour.Show && !content.inTransition) {
                Debug.Log("content show");
                content.Show();
            }
            
            yield return null;
            
            if(mainCanvasGroup != null)
                mainCanvasGroup.alpha = 1;               
            
        }
        
        IEnumerator RoutineHide() {
            
            if(content == null)                 {
                yield break;
            }
            
                
            
            while(content.inTransition) {
                yield return null;   
            }
            
            while(isOverlayUse && overlay != null && overlay.inTransition)  {
                yield return null;
            }
            
            while(content.DisableGameObjectWhenHidden && content.gameObject.activeSelf)
                yield return null;
            
            // doozy에서는 hide 애니메이션, instantHide에서도 마지막에 3 frame 정도 delay를 둔다.
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            
            
            SelfDestroy();
            
        }
        
        
        /// <summary>
        /// 팝업 Hide
        /// </summary>
        public virtual void Hide() {

            try {
            
                // 중복 호출을 막음. 
                if(content.inTransition)
                    return;
                
                
                if(isOverlayUse && overlay != null)
                    overlay.Hide();
                    
                    
                content.Hide();
            }
            catch {
                return;   
            }
            
            StartCoroutine(RoutineHide());
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
            
            
            
            if(PopupManager.main != null)
                PopupManager.main.RemoveActivePopup(this); // remove
            
            Destroy(gameObject, 0.1f);
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