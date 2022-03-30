using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public abstract class CommonView : MonoBehaviour
    {
        public static List<CommonView> ListActiveViews = new List<CommonView>();
        public static void ClearActiveViews() {
            ListActiveViews.Clear();
        }
        
        public static void DeleteDumpViews() {
            for(int i = ListActiveViews.Count-1; i>=0; i--) {
                if(ListActiveViews[i] == null) {
                    ListActiveViews.RemoveAt(i);
                }
            }
        }
        
        public virtual void OnStartView() {
            // Debug.Log(string.Format("[{0}] OnStart <<", this.gameObject.name));
            // Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_CHANGE_OWNER, this.gameObject.name);
            
            if(ListActiveViews.Contains(this))
                return;
            
            ListActiveViews.Add(this);
            
        }
        
        public virtual void OnView() {
            // Debug.Log(string.Format("[{0}] OPEN <<", this.gameObject.name));
        }
        
        public virtual void OnHideView() {
            // Debug.Log(string.Format("[{0}] HIDE <<", this.gameObject.name));
            
            if(ListActiveViews.Contains(this)) {
                ListActiveViews.Remove(this);
            }
            
        }
    }
}