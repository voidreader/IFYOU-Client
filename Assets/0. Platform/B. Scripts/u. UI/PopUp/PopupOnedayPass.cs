using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toast.Gamebase;
using TMPro;

namespace PIERStory {
    public class PopupOnedayPass : PopupBase
    {
        public TextMeshProUGUI textPrice;
        public TextMeshProUGUI textChoicesSaleText;
        public TextMeshProUGUI textChoicesOff;
        public TextMeshProUGUI textChoicesOff2;
        
        public TextMeshProUGUI textTitle; // 타이틀 
        
        
        
        
        
        public bool isPurchasable = false; // 구매가능 상태 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        
        public override void Show() {
            if(isShow)
                return;
            
            base.Show();
            
            
            // 게임베이스 아이템 정보 
            gamebaseItem = BillingManager.main.GetGamebasePurchaseItem("oneday_pass");
            
            
            // 텍스트 세팅 
            
            Debug.Log(string.Format(SystemManager.GetLocalizedText("6455"), BillingManager.main.ifyouPassChoiceSale));
            SystemManager.SetText(textChoicesSaleText, string.Format(SystemManager.GetLocalizedText("6455"), BillingManager.main.ifyouPassChoiceSale.ToString()));
            textChoicesOff.text = BillingManager.main.ifyouPassChoiceSale.ToString() +"%\n<size=12>OFF</size>" ;
            
            
            
            // 이프유 패스 사용중일때, 아닐때의 분류하기. 
            /*
            if(UserManager.main.CheckIFyouPassUsing()) {
                textPrice.text = UserManager.main.GetIFyouPassExpireMessage();
                if(UserManager.main.ifyouPassDay >= 30) // 마지막날은 재구매 가능함. 
                    isPurchasable = true;
                else
                    isPurchasable = false;
            }
            else {
                // Price 표시
                textPrice.text = gamebaseItem.localizedPrice;
                isPurchasable = true;
            }
            */
            
        }
        
        public void OnClickPurchase() {
            
            if(!isPurchasable) {
                //Debug.LogError("It's not purchasable : " + UserManager.main.ifyouPassDay);
                return;
            }
            
            BillingManager.main.RequestPurchaseGamebase("oneday_pass", StoryManager.main.CurrentProjectID);
        }
    }
}