using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {

    public class toggleText : MonoBehaviour
    {
        
        public TextMeshProUGUI textTarget;
        public Color colorActive; 
        public Color colorInactive; 
        
        public void SetOff(){

            if (LobbyManager.main == null)
                return;

            textTarget.color = colorInactive;    
        }
        
        public void SetOn() {

            Debug.Log("toggleText SetON #1 : " + textTarget.text);
            
            if (LobbyManager.main == null)
                return;
                
            Debug.Log("toggleText SetON #2 : " + textTarget.text);

            textTarget.color = colorActive;
            
            // OnClickGenre();
        }        
    }
}