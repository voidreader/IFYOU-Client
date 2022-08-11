using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toast.Gamebase;
using TMPro;

namespace PIERStory {
    public class PopupPremiumPass : PopupBase
    {
        public GameObject btnStarPurchase;
        
        public TextMeshProUGUI textPrice; // 인앱상품 가격
        public TextMeshProUGUI textOriginStarPrice; // 스타 판매 원가격
        public TextMeshProUGUI textDiscountStarPrice; // 스타 판매 할인가격
        
        
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
            if(StoryLobbyManager.main != null || GameManager.main != null) 
                currentStory = StoryManager.main.CurrentProject;            
            
            // 게임베이스 아이템 정보 
            try {
                gamebaseItem = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumSaleID); // 연결된 상품ID로 조회한다. 
                // SystemManager.SetText(textPrice, gamebaseItem.localizedPrice);
                textPrice.text = gamebaseItem.localizedPrice;
            }
            catch {
                Debug.Log("Windows standalone?");
            }
            
            // 스타 판매 가격 설정 
            textOriginStarPrice.text = currentStory.passPrice.ToString();
            textDiscountStarPrice.text = currentStory.discountPassPrice.ToString();
            
            
            SystemManager.SetText(textTitle, currentStory.title); // 타이틀      
            storyImage.SetDownloadURL(currentStory.coinBannerUrl, currentStory.coinBannerKey);
            
            isPurchasable = !UserManager.main.HasProjectPremiumPassOnly(currentStory.projectID);
            
            if(!isPurchasable) {
                SystemManager.SetText(textPrice, SystemManager.GetLocalizedText("6464"));
            }
            
            btnStarPurchase.SetActive(isPurchasable);

        }
                
        /// <summary>
        /// 인앱 상품으로 구매 
        /// </summary>
        public void OnClickPurchase() {
            
            if(!isPurchasable) {
                Debug.LogError("It's not purchasable Premium pass");
                return;
            }
            
            BillingManager.main.RequestPurchaseGamebase(currentStory.premiumSaleID, currentStory.projectID);
        }
        
        /// <summary>
        /// 스타로 구매 
        /// </summary>
        public void OnClickStarPurchase() {
            
            // 젬 보유 체크 
            if(!UserManager.main.CheckGemProperty(currentStory.discountPassPrice)) {
                
                SystemManager.ShowLackOfCurrencyPopup(true, "6323", currentStory.discountPassPrice); // 부족하면 팝업 띄운다.
                return;
            }
            
            // 통신 처리 
            SystemManager.ShowSystemPopup(string.Format(SystemManager.GetLocalizedText("6477"), currentStory.discountPassPrice), PurchasePremiumPassByStar, null);
        }
        
        void PurchasePremiumPassByStar() {
            // 통신 처리 
            NetworkLoader.main.PurchasePremiumPassByStar(currentStory.projectID, currentStory.discountPassPrice);
        }
    }
}