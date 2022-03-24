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

            if (LobbyManager.main == null)
                return;

            textTarget.color = colorActive;
            
            // OnClickGenre();
        }        
    }
}