using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace PIERStory {
    [Serializable]
    public class PopupContent 
    {
        
        public bool HasImages { get { return ImagesCount > 0; } }
        public bool HasLabels { get { return LabelsCount > 0; } }    
        public int ImagesCount { get { return Images.Count; } }   
        public int LabelsCount { get { return Labels.Count; } }
        
        public List<Image> Images = new List<Image>();
        public List<TextMeshProUGUI> Labels = new List<TextMeshProUGUI>();
        
        
        /// <summary>
        /// 배열로 받아서 text 설정 
        /// </summary>
        /// <param name="labels"></param>
        public void SetLabelsTexts(params string[] __text) {
            if (__text == null || __text.Length == 0 || !HasLabels) return;
            
            for (int i = 0; i < Labels.Count; i++)
            {
                if(Labels.Count <= i) {
                    Debug.LogError("Too many text labels");
                    break;
                }
                
                Labels[i].text = __text[i];

            }
        }
        
        /// <summary>
        /// Image에 Sprite 할당 
        /// </summary>
        /// <param name="sprites"></param>
        public void SetImagesSprites(params Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0 || !HasImages) return;
            for (int i = 0; i < Images.Count; i++)
            {
                Image image = Images[i];
                if (image == null) continue;
                image.sprite = sprites[i];
                image.SetNativeSize();
            }
            
        }
        
    }
}