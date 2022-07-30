using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toast.Gamebase;
using TMPro;

namespace PIERStory {

    public class PopupPremiumChallenge : PopupBase
    {
        public TextMeshProUGUI textPrice;
        
        
        public TextMeshProUGUI textTitle; // 타이틀 
        
        
        public ImageRequireDownload storyImage;
                
        public bool isPurchasable = false; // 구매가능 상태 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        public StoryData currentStory;
        
       public override void Show() {
            if(isShow)
                return;
            
            base.Show();
            
            
            
            
            // 텍스트 세팅 
            
            
            currentStory = StoryManager.main.CurrentProject;
            
            SystemManager.SetText(textTitle, currentStory.title); // 타이틀      
            storyImage.SetDownloadURL(currentStory.coinBannerUrl, currentStory.coinBannerKey); // 이미지 처리
            
            // 게임베이스 아이템 정보 
            try {
                gamebaseItem = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumSaleID); // 연결된 상품ID로 조회한다. 
                textPrice.text = gamebaseItem.localizedPrice;
            }
            catch {
                isPurchasable = false;
                textPrice.text = "ERROR";
                Debug.Log("Windows standalone?");
            }
            
            
            
           
            // 원데이 패스 사용중일때, 아닐때의 분류하기. 
            // if(currentStory.IsValidOnedayPass()) { // 사용중 
            //     isPurchasable = false; 
                
                
            // }
            // else { // 사용중이지 않음. (구매가능)
            //     isPurchasable= true;
            // }
            
            
        }
        
        /// <summary>
        /// 챌린지 초기화 
        /// </summary>
        void InitChallenge() {
            
        }
                
        public void OnClickPurchase() {
            
            if(!isPurchasable) {
                Debug.LogError("It's not purchasable oneday pass");
                return;
            }
            
            // BillingManager.main.RequestPurchaseGamebase("oneday_pass", StoryManager.main.CurrentProjectID);
        }
        
        
        public void OnClickBenefit() { 
            
        }
    }
}