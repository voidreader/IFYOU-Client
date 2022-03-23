using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public abstract class CommonView : MonoBehaviour
    {
        public virtual void OnStartView() {
            // Debug.Log(string.Format("[{0}] OnStart <<", this.gameObject.name));
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_CHANGE_OWNER, this.gameObject.name);
        }
        
        public virtual void OnView() {
            // Debug.Log(string.Format("[{0}] OPEN <<", this.gameObject.name));
        }
        
        public virtual void OnHideView() {
            // Debug.Log(string.Format("[{0}] HIDE <<", this.gameObject.name));
        }
    }
}