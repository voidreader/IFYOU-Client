using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PIERStory {

    public class ImageLocalizedUI : MonoBehaviour
    {
        
        public Image targetImage = null;
        public bool isNativeSize = false;
        
        public Sprite spriteEnglish;
        public Sprite spriteKorean;
        public Sprite spriteJapanese;
        
        void Awake() {
            if(targetImage) {
                targetImage = this.GetComponent<Image>();
            }
            
            
        }
        
        private void OnEnable() {
            SetImage();
        }
        
        void SetImage() {
            
            if(SystemManager.main == null)
                return;
                
            if(targetImage == null)
                return;
        
                
            if(SystemManager.main.currentAppLanguageCode == "EN") {
                targetImage.sprite = spriteEnglish;
            }
            else if(SystemManager.main.currentAppLanguageCode == "KO") {
                targetImage.sprite = spriteKorean;
            }
            else if(SystemManager.main.currentAppLanguageCode == "JA") {
                targetImage.sprite = spriteJapanese;
            }
            else {
                targetImage.sprite = spriteEnglish;
            }
            
            if(isNativeSize && targetImage != null && targetImage.sprite != null) {
                targetImage.SetNativeSize();
            }
        }
    }
}