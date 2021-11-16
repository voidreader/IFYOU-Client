using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PIERStory {
    public abstract class CommonView : MonoBehaviour
    {
        public virtual void OnStartView() {
            Debug.Log(string.Format("[{0}] OnStart <<", this.gameObject.name));
        }
        
        public virtual void OnView() {
            Debug.Log(string.Format("[{0}] OPEN <<", this.gameObject.name));
        }
    }
}