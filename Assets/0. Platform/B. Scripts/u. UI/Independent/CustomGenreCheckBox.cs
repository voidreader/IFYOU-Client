using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

namespace PIERStory {

    public class CustomGenreCheckBox : MonoBehaviour
    {
        
        public Action<string, bool> OnSelectedCheckBox = null;
        
        
        public Image bodyImage; 
        
        public TextMeshProUGUI textSelected; // 활성 텍스트 
        public TextMeshProUGUI textUnselected; // 비활성 텍스트
        
        public Sprite spriteSelect; // 스프라이트 
        public Sprite spriteUnselect; // 비활성 스프라이트 
        
        public bool isSelected = false; // 선택된 상태인지 
        public bool isMasterCheckBox = false; // 마스터 체크박스(혼자만 선택가능 )
        
        public string originText = string.Empty;
        public string localizedText = string.Empty;
        
        
        /// <summary>
        /// 초기화 
        /// </summary>
        /// <param name="__text"></param>
        public void Init(string __text) {
            
            isSelected = false;
            originText = __text;
            localizedText = __text;
                        
            SystemManager.SetText(textUnselected, originText);
            SystemManager.SetText(textSelected, originText);
            
            SetState(isSelected);
            
            this.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__j"></param>
        public void Init(JsonData __j) {
            isSelected = false;
            originText = __j["origin_name"].ToString();
            localizedText = __j["genre_name"].ToString();
            
            SystemManager.SetText(textUnselected, localizedText);
            SystemManager.SetText(textSelected, localizedText);
            
            SetState(isSelected);
            this.gameObject.SetActive(true);
        }
        
        
        public void OnClickCheckBox() {
            isSelected = !isSelected; // 반대로 설정한다.
            
            
            // 이벤트 호출. 
            OnSelectedCheckBox?.Invoke(originText, isSelected);
            
            // 상태 변경
            SetState(isSelected);
            
        }
        
        
        
        /// <summary>
        /// 강제 비선택 처리 
        /// </summary>
        public void Unselect() {
            
            if(!this.gameObject.activeSelf)
                return;
            
            
            isSelected = false;
            SetState(false);
        }
        
        
        /// <summary>
        /// 상태값 설정
        /// </summary>
        public void SetState(bool __state) {
            
            textSelected.gameObject.SetActive(false);
            textUnselected.gameObject.SetActive(false);
            
            if(__state) {
                bodyImage.sprite = spriteSelect;
                textSelected.gameObject.SetActive(true);
            }
            else {
                bodyImage.sprite = spriteUnselect;
                textUnselected.gameObject.SetActive(true);
            }
            
            bodyImage.SetNativeSize();
        }
        
    }
}