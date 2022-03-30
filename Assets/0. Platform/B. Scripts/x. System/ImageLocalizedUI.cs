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
            
            SetImage();
            
        }
        
        void SetImage() {
            
            if(SystemManager.main == null)
                return;
                
            if(targetImage == null)
                return;
            
            if(ES3.KeyExists(SystemConst.KEY_LANG))
                SystemManager.main.currentAppLanguageCode = ES3.Load<string>(SystemConst.KEY_LANG);        
            else {
                
            }
                
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
        
        void OnEnable() {
            SetImage();
        }

    }
}