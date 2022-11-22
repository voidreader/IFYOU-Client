using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toast.Gamebase;
using TMPro;

namespace PIERStory {
    public class PopupPremiumPass : PopupBase
    {
        public GameObject btnStarPurchase;
        
        public TextMeshProUGUI textInAppNoSale; // 인앱상품 일반 가격
        public GameObject groupDiscountInApp; // 할인 그룹 
        public TextMeshProUGUI textInAppNoSale2; // 인앱상품 일반 가격
        public TextMeshProUGUI textInappDiscountPrice; // 인앱상품 할인가격
        
        [Space]
        public TextMeshProUGUI textOriginStarPrice; // 스타 판매 원가격
        public TextMeshProUGUI textDiscountStarPrice; // 스타 판매 할인가격
        
        
        public TextMeshProUGUI textTitle; // 타이틀 
        public TextMeshProUGUI textRefund; // 환급 안내 메세지 
        
        
        public ImageRequireDownload storyImage;
                
        public bool isPurchasable = false; // 구매가능 상태 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItemNoDiscount = null; // 게임베이스 기준정보 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItemDiscount = null; 
        
        
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
                gamebaseItemDiscount = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumSaleID);  // 할인 인앱 
                gamebaseItemNoDiscount = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumProductID); // 미할인 인앱 
                // SystemManager.SetText(textPrice, gamebaseItem.localizedPrice);
                
                textInAppNoSale.text = gamebaseItemNoDiscount.localizedPrice;
                textInAppNoSale2.text = gamebaseItemNoDiscount.localizedPrice;
                textInappDiscountPrice.text = gamebaseItemDiscount.localizedPrice;
                
            }
            catch {
                Debug.Log("Windows standalone?");
            }
            
            // 할인 설정된 경우에 대한 처리
            if(currentStory.premiumProductID == currentStory.premiumSaleID) { // 같은 경우는 할인중이 아님 
                textInAppNoSale.gameObject.SetActive(true);
                groupDiscountInApp.SetActive(false);
            }
            else { // 다른 경우 (할인중)
                textInAppNoSale.gameObject.SetActive(false);
                groupDiscountInApp.SetActive(true);
            }
            
            
            // 스타 판매 가격 설정 
            textOriginStarPrice.text = currentStory.passPrice.ToString();
            textDiscountStarPrice.text = currentStory.discountPassPrice.ToString();
            
            
            SystemManager.SetText(textTitle, currentStory.title); // 타이틀      
            storyImage.SetDownloadURL(currentStory.coinBannerUrl, currentStory.coinBannerKey);
            
            // 구매 가능한지 체크한다. 
            isPurchasable = !UserManager.main.HasProjectPremiumPassOnly(currentStory.projectID);
            
            if(!isPurchasable) {
                SystemManager.SetText(textInAppNoSale, SystemManager.GetLocalizedText("6464"));
                // 구매 가능하지 않은 경우는 group 비활성화 
                textInAppNoSale.gameObject.SetActive(true);
                groupDiscountInApp.SetActive(false);
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
            
            // 구매는 saleID로 진행 
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