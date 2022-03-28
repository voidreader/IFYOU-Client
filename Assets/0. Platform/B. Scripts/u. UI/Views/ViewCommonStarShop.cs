using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;


namespace PIERStory {

    public class ViewCommonStarShop : CommonView
    {
        public override void OnStartView() {
            base.OnStartView();
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
            
            StartCoroutine(DelaySendingSignal());
            
        }
        
        public override void OnView() {
            base.OnView();

        }
        
        public override void OnHideView() {
            base.OnHideView();
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);
        }
        
        IEnumerator DelaySendingSignal() {
            yield return null;
            yield return null;
            yield return null;
            
        }        
        
    }
}