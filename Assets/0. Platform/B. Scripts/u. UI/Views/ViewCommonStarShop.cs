using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;


namespace PIERStory {

    public class ViewCommonStarShop : CommonView
    {
        public static bool isCommonShopOpen = false;
        
        public override void OnStartView() {
            base.OnStartView();
            
            if(GameManager.main == null)
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
            
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
            
            isCommonShopOpen = false;
        }
        
        IEnumerator DelaySendingSignal() {
            yield return null;
            yield return null;
            yield return null;
            
        }        
        
    }
}