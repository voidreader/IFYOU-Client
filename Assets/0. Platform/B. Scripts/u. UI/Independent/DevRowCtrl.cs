using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {
    public class DevRowCtrl : MonoBehaviour
    {
        public ScriptRow row;
        
        public TextMeshProUGUI textLineNumber; // 라인넘버
        public TextMeshProUGUI textTemplate; // 템플릿
        
        public TextMeshProUGUI textSceneID;
        public TextMeshProUGUI textData;
        
        
        
        /// <summary>
        /// 초기화 
        /// </summary>
        /// <param name="__row"></param>
        public void InitDevRow(ScriptRow __row) {
            row = __row;
            
            textLineNumber.text = row.lineNumber.ToString();
            textTemplate.text = row.template;
            textSceneID.text = row.scene_id;
            textData.text = row.script_data;
            
            this.gameObject.SetActive(true);
        }
        
        
        /// <summary>
        /// 버튼 눌렀을때 스크립트 이동 
        /// </summary>
        public void OnClickRowButton() {
            if(GameManager.main == null)
                return;
                
            GameManager.main.JumpForce(row);    
        }
    }
}