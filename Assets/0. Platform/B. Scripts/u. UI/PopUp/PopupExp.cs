using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using DG.Tweening;

namespace PIERStory {

    public class PopupExp : PopupBase
    {
        [SerializeField] Image imageBar; // 경험치 바
        [SerializeField] TextMeshProUGUI textExp; // 경험치 
        [SerializeField] TextMeshProUGUI textlevel; // 레벨 
        [SerializeField] TextMeshProUGUI textProgress; // 게이지 중앙 수치 처리 
        [SerializeField] TextMeshProUGUI textExpCounter; // 카운트 효과용도.
        
        [Space]
        [Space]
        
        JsonData beforeData = null; // 경험치 획득 전 
        JsonData currentData = null; // 경험치 획득 후 
         
        [SerializeField] int maxExp = 0; // 이전 레벨의 최대 경험치 
        
        [SerializeField] int getExp = 0; // 획득 경험치
        [SerializeField] int currentExp = 0; // 현재 경험치 
        [SerializeField] int beforeExp = 0; // 이전 경험치
        
        [Space]
        [SerializeField] int currentLevel = 0; // 현재 레벨 
        [SerializeField] int beforeLevel = 0; // 이전 레벨
        
        void Update() {
            if(content.inTransition)
                return;
            
            // 프로그레스 텍스트 갱신처리 
            textProgress.text = textExpCounter.text +"/" + maxExp.ToString();
        }
        
        public override void Show() {
            
            base.Show();
            // Data.conetntj
            
            if(Data.contentJson == null) {
                Debug.LogError("No Level exp data");
                return;
            }
            
            // 
            beforeData = Data.contentJson["before"];
            currentData = Data.contentJson["current"];
            
            // 획득 경험치 
            getExp = SystemManager.GetJsonNodeInt(beforeData, "get_experience"); 
            
            
            
            // 이전 
            beforeExp = SystemManager.GetJsonNodeInt(beforeData, "experience");
            beforeLevel = SystemManager.GetJsonNodeInt(beforeData, "level");
            
            // 현재
            currentExp = SystemManager.GetJsonNodeInt(currentData, "experience");
            currentLevel = SystemManager.GetJsonNodeInt(currentData, "level");
            
            // 레벨 최대 경험치 
            maxExp = SystemManager.main.GetLevelMaxExp((beforeLevel+1).ToString()); // before의 경험치를 구한다.
            
            imageBar.fillAmount = 0; // 게이지 초기화 
            
            // 레벨 텍스트 
            textlevel.text = "LV. " + currentLevel.ToString();
            
            // 획득 경험치
            textExp.text = "+" + getExp.ToString();
            textExpCounter.text = getExp.ToString(); // 카운터용도 텍스트
            
            
            // 게이지 처리 
            float beforeProgress = (float)beforeExp / (float)maxExp;
            imageBar.fillAmount = beforeProgress; // fillAmount 미리 설정. 
            
           
        }
        
        /// <summary>
        /// Show 애니메이션 끝나고 호출 
        /// </summary>
        public void OnShow() {
            float nextProgress = 0;
            
            
            if(beforeLevel != currentLevel) {
                Debug.Log(">>> Level Up expected <<<");
                nextProgress= 1; // 레벨이 달라진 경우 1.
            }
            else {
                nextProgress = (float)currentExp / (float)maxExp;
            }
            
            // 경험치 게이지가 올라가는 효과 
            imageBar.DOFillAmount(nextProgress, 1.5f); // fillAmount 처리한다. 
            textExpCounter.DOCounter(beforeExp, currentLevel==beforeLevel?currentExp:maxExp, 1.5f, false);
        }
    
    }
}