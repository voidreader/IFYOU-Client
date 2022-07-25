using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class ViewCommonSetting : CommonView
    {
        public override void OnStartView() {
            base.OnStartView();
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
            
            Debug.Log("ViewCommonSetting  OnStartView");
            
            StartCoroutine(DelaySendingSignal());
            
        }
        
        public override void OnView() {
            base.OnView();

        }
        
        public override void OnHideView() {
            base.OnHideView();
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);

            /*
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, true, string.Empty);            
            */
        }

        IEnumerator DelaySendingSignal() {
            yield return null;
            yield return null;
            yield return null;
            
            Debug.Log("ViewCommonSetting  DelaySendingSignal");
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("8015"), string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);               
            
        }
    }
}