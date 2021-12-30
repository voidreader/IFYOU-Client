using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Components;
using TMPro;

namespace PIERStory {
    [Serializable]
    public class PopupContent 
    {
        
        public bool HasImages { get { return ImagesCount > 0; } }
        public bool HasLabels { get { return LabelsCount > 0; } }    
        public int ImagesCount { get { return Images.Count; } }   
        public int LabelsCount { get { return Labels.Count; } }
        public bool HasButtons { get { return Buttons.Count > 0; } }
        
        public List<UIButton> Buttons = new List<UIButton>();
        
        public List<Image> Images = new List<Image>();
        public List<TextMeshProUGUI> Labels = new List<TextMeshProUGUI>();
        
        
        public Action positiveButtonCallback = null; // 긍정 버튼 콜백 
        public Action negativeButtonCallback = null; // 부정 버튼 콜백
        
        /*
        public void SetButtonsCallbacks(params Action[] callbacks)
        {
            if (callbacks == null || callbacks.Length == 0 || !HasButtons) {
                Debug.Log("SetButtonsCallbacks, WrUnityActionong!");
                return;
            }
            
            for (int i = 0; i < Buttons.Count; i++)
            {
                UIButton button = Buttons[i];
                if (button == null) continue;
                if (callbacks[i] == null) continue;
                
                // button.set
                button.behaviours.HasBehaviour()
            }
        }
        */

        /// <summary>
        /// 긍정 버튼 콜백 설정
        /// </summary>
        /// <param name="__action"></param>        
        public void SetPositiveButtonCallback(Action __action) {
            positiveButtonCallback = __action;
        }
        
        /// <summary>
        /// 부정 버튼 콜백 설정 
        /// </summary>
        /// <param name="__action"></param>
        public void SetNegativeButtonCallback(Action __action) {
            negativeButtonCallback = __action;
        }
        
        
        /// <summary>
        /// 배열로 받아서 text 설정 
        /// </summary>
        /// <param name="labels"></param>
        public void SetLabelsTexts(params string[] __text) {
            if (__text == null || __text.Length == 0 || !HasLabels) return;
            
            for (int i = 0; i < __text.Length; i++)
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
            for (int i = 0; i < sprites.Length; i++)
            {
                if(Images.Count <= i) {
                    Debug.LogError("Too many sprites");
                    break;
                }
                
                Image image = Images[i];
                if (image == null) continue;
                image.sprite = sprites[i];
                image.SetNativeSize();
            }
            
        }
        
    }
}