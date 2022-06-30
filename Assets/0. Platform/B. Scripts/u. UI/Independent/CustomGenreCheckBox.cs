using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {

    public class CustomGenreCheckBox : MonoBehaviour
    {
        
        public static Action<string> OnSelectedCheckBox = null;
        
        
        public Image bodyImage; 
        
        public TextMeshProUGUI textSelected; // 활성 텍스트 
        public TextMeshProUGUI textUnselected; // 비활성 텍스트
        
        public Sprite spriteSelect; // 스프라이트 
        public Sprite spriteUnselect; // 비활성 스프라이트 
        
        public bool isSelected = false; // 선택된 상태인지 
        public bool isMasterCheckBox = false; // 마스터 체크박스(혼자만 선택가능 )
        
        public string originText = string.Empty;
        
        
        /// <summary>
        /// 초기화 
        /// </summary>
        /// <param name="__text"></param>
        public void Init(string __text) {
            
            isSelected = false;
            originText = __text;
                        
            SystemManager.SetText(textUnselected, originText);
            SystemManager.SetText(textSelected, originText);
            
            SetState();
        }
        
        public void OnClickCheckBox() {
            isSelected = !isSelected; // 반대로 설정한다.
            
            if(isSelected) {
                // 이벤트 호출. 
                OnSelectedCheckBox?.Invoke(originText);
            }
        }
        
        
        /// <summary>
        /// 상태값 설정
        /// </summary>
        void SetState() {
            
            if(isSelected) {
                bodyImage.sprite = spriteSelect;
            }
            else {
                bodyImage.sprite = spriteUnselect;
            }
            
            bodyImage.SetNativeSize();
        }
        
    }
}