using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {

    public class CustomToggle : MonoBehaviour
    {
        
        public TextMeshProUGUI textToggle;
        public string identifier = string.Empty;
        
        
        public void InitToggle(string __toggleText) {
            SystemManager.SetText(textToggle, __toggleText);
        }
        
        public void OnValueChanged(bool __newValue) {
            // 처리 
        }
    }
}