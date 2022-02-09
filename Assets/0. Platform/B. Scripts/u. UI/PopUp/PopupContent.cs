using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using LitJson;

namespace PIERStory {
    [Serializable]
    public class PopupContent 
    {
        public string targetData = string.Empty; // 공용으로 사용되는 스트링 데이터 
        public bool HasImages { get { return ImagesCount > 0; } }
        public bool HasLabels { get { return LabelsCount > 0; } }    
        public int ImagesCount { get { return Images.Count; } }   
        public int LabelsCount { get { return Labels.Count; } }
        public bool HasButtons { get { return Buttons.Count > 0; } }
        
        public List<Button> Buttons = new List<Button>();
        
        public List<Image> Images = new List<Image>();
        public List<TextMeshProUGUI> Labels = new List<TextMeshProUGUI>();
        
        public JsonData contentJson = null;
        
        
        public Action positiveButtonCallback = null; // 긍정 버튼 콜백 
        public Action negativeButtonCallback = null; // 부정 버튼 콜백

        public string imageURL = string.Empty;
        public string imageKey = string.Empty;

        /*
         * true = 예/아니오, false = 확인
         * 버튼 존재
         */
        public bool isConfirm = true;
        public bool isPositive = true;      // 팝업의 타입이 긍정타입인지 부정타입인지

        
        /// <summary>
        /// 버튼 텍스트 처리 
        /// </summary>
        /// <param name="buttonTexts"></param>
        public void SetButtonsText(params string[] buttonTexts)
        {
            if (buttonTexts == null || buttonTexts.Length == 0 || !HasButtons) {
                Debug.Log("SetButtonsText, No Button here!");
                return;
            }
            
            for (int i = 0; i < Buttons.Count; i++)
            {
                Button button = Buttons[i];
                if (button == null) continue;
                
                TextMeshProUGUI textButton = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                
                if(textButton == null) continue;
                
                
                textButton.text = buttonTexts[i];
            }
        }
        


        /// <summary>
        /// 타겟 데이터 지정
        /// </summary>
        /// <param name="__t"></param>
        public void SetTargetData(string __t) {
            targetData = __t;
        }
        
        
        /// <summary>
        ///  JSON 데이터 지정 
        /// </summary>
        /// <param name="__j"></param>
        public void SetContentJson(JsonData __j) {
            contentJson = __j;
        }

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