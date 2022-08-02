using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;


namespace PIERStory {
    public class IFyouPackProduct : GeneralPackProduct
    {   
        public TextMeshProUGUI textLabel1, textLabel2, textExplain1, textExplain2, textInfo;
        
        
        public override void InitPackage(string __productID, JsonData __productMasterJSON) {
            base.InitPackage(__productID, __productMasterJSON);
            
            Debug.Log("Init IFyouPackProduct");
            
           
            // 추가 처리 
            SystemManager.SetText(textExplain1, string.Format(SystemManager.GetLocalizedText("6475"), BillingManager.main.ifyouPassDirectStar));
            SystemManager.SetText(textExplain2, string.Format(SystemManager.GetLocalizedText("6476"), BillingManager.main.ifyouPassDailyStar));
            
            if(UserManager.main.CheckIFyouPassUsing()) {
                textPrice.text = UserManager.main.GetIFyouPassExpireMessage();
            }
        }
    }
}