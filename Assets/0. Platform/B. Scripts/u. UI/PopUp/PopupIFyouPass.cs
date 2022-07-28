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
        public bool isPurchasable = false; // 구매가능 상태 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        
        public override void Show() {
            if(isShow)
                return;
            
            base.Show();
            
            
            // 게임베이스 아이템 정보 
            gamebaseItem = BillingManager.main.GetGamebasePurchaseItem("ifyou_pass");
            
            // 이프유 패스 사용중일때, 아닐때의 분류하기. 
            if(UserManager.main.CheckIFyouPassUsing()) {
                textPrice.text = UserManager.main.GetIFyouPassExpireMessage();
            }
            else {
                // Price 표시
                textPrice.text = gamebaseItem.localizedPrice;
            }
            
        }
        
        public void OnClickPurchase() {
            BillingManager.main.RequestPurchaseGamebase("ifyou_pass");
        }
    }
}