using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PIERStory {

    public class SimpleTabButtonCtrl : MonoBehaviour
    {
        
        [SerializeField] Image buttonBody;
        
        [SerializeField] Sprite normalSprite;
        [SerializeField] Sprite pressedSprite;

        void Awake() {
            buttonBody.sprite = normalSprite;    
        }
        
        
        public void OnToggle() {
            buttonBody.sprite = pressedSprite;
        }
        
        public void OffToggle() {
            buttonBody.sprite = normalSprite;
        }
    }
}