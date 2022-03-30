using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;


namespace PIERStory {

    public class ViewCommonStarShop : CommonView
    {
        public static bool isCommonShopOpen = false;
        
        public static Action storedAction = null;
        
        public override void OnStartView() {
            base.OnStartView();
            
            if(GameManager.main == null)
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
            else {
                // 게임씬에서 불렀음. 
                if(ViewCommonTop.OnBackAction != null) {
                    
                    Debug.Log("ViewCommonStarShop :: BackACtion Save");
                    
                    storedAction = ViewCommonTop.OnBackAction; // 저장해놓는다. 
                    ViewCommonTop.OnBackAction = null; // 널로 변경해놓는다.
                }
                else {
                    Debug.Log("ViewCommonStarShop :: BackACtion IS NULL");
                }
            }
            
            StartCoroutine(DelaySendingSignal());
            
            isCommonShopOpen = true;
            
        }
        
        public override void OnView() {
            base.OnView();

        }
        
        public override void OnHideView() {
            base.OnHideView();
            
            if(GameManager.main == null)
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);
            else {
                // 게임씬에서 돌아갈때. 
                if(storedAction != null) {
                    
                    Debug.Log("ViewCommonStarShop :: BackACtion Restore");
                    
                    ViewCommonTop.OnBackAction = storedAction; // 콜백 돌려준다.
                    // storedAction = null;
                }
            }
            
            isCommonShopOpen = false;
        }
        
        IEnumerator DelaySendingSignal() {
            yield return null;
            yield return null;
            yield return null;
            
        }        
        
    }
}