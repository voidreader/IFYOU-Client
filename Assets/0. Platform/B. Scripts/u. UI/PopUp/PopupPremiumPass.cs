using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toast.Gamebase;
using TMPro;

namespace PIERStory {
    public class PopupPremiumPass : PopupBase
    {
        public TextMeshProUGUI textPrice;
        
        
        public TextMeshProUGUI textTitle; // 타이틀 
        public TextMeshProUGUI textRefund; // 환급 안내 메세지 
        
        
        public ImageRequireDownload storyImage;
                
        public bool isPurchasable = false; // 구매가능 상태 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        public StoryData currentStory;
        
       public override void Show() {
            if(isShow)
                return;
            
            base.Show();
            
            

            
            // 텍스트 세팅 
          
            currentStory = SystemListener.main.introduceStory; // 리스너에서 받아온다. 
            // 게임베이스 아이템 정보 
            try {
                gamebaseItem = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumSaleID); // 연결된 상품ID로 조회한다. 
                textPrice.text = gamebaseItem.localizedPrice;
            }
            catch {
                Debug.Log("Windows standalone?");
            }            
            
            
            SystemManager.SetText(textTitle, currentStory.title); // 타이틀      
            storyImage.SetDownloadURL(currentStory.coinBannerUrl, currentStory.coinBannerKey);
            
            isPurchasable = !UserManager.main.HasProjectPremiumPassOnly(currentStory.projectID);
            
            if(!isPurchasable) {
                SystemManager.SetText(textPrice, SystemManager.GetLocalizedText("6464"));
            }

        }
                
        public void OnClickPurchase() {
            
            if(!isPurchasable) {
                Debug.LogError("It's not purchasable Premium pass");
                return;
            }
            
            BillingManager.main.RequestPurchaseGamebase(currentStory.premiumSaleID, currentStory.projectID);
        }
    }
}