using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Toast.Gamebase;
using TMPro;

namespace PIERStory {

    public class PopupIFyouPass : PopupBase
    {
        public TextMeshProUGUI textPrice;
        public TextMeshProUGUI textDirectStarQuantity;
        public TextMeshProUGUI textDailyStarQuantity;
        public TextMeshProUGUI textChoicesSaleText;
        public TextMeshProUGUI textChoicesOff;
        
        
        
        public bool isPurchasable = false; // 구매가능 상태 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        
        public override void Show() {
            
            
            if(isShow)
                return;
            
            base.Show();
            
            
            try {
            
                // 게임베이스 아이템 정보 
                gamebaseItem = BillingManager.main.GetGamebasePurchaseItem("ifyou_pass");
                
                
                // 텍스트 세팅 
                textDirectStarQuantity.text = BillingManager.main.ifyouPassDirectStar.ToString();
                textDailyStarQuantity.text = BillingManager.main.ifyouPassDailyStar.ToString();
                
                Debug.Log(string.Format(SystemManager.GetLocalizedText("6455"), BillingManager.main.ifyouPassChoiceSale));
                
                SystemManager.SetText(textChoicesSaleText, string.Format(SystemManager.GetLocalizedText("6455"), BillingManager.main.ifyouPassChoiceSale.ToString()));
                textChoicesOff.text = BillingManager.main.ifyouPassChoiceSale.ToString() +"%\n<size=12>OFF</size>" ;
            }
            catch (System.Exception e) {
                NetworkLoader.main.ReportRequestError(e.StackTrace, "IFyouPass #1");
                Debug.LogError(e.StackTrace);
            }
            
            
            try {
                // 이프유 패스 사용중일때, 아닐때의 분류하기. 
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
            }
            catch (System.Exception e) {
                NetworkLoader.main.ReportRequestError(e.StackTrace, "IFyouPass #2");
                Debug.LogError(e.StackTrace);
            }
            
        }
        
        public void OnClickPurchase() {
            
            if(!isPurchasable) {
                Debug.LogError("It's not purchasable : " + UserManager.main.ifyouPassDay);
                return;
            }
            
            BillingManager.main.RequestPurchaseGamebase("ifyou_pass");
        }
    }
}