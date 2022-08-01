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
        
        
        public ImageRequireDownload storyImage;
        
        public StoryData currentStory;
        
        
        
        
        
        public bool isPurchasable = false; // 구매가능 상태 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        
        public override void Show() {
            if(isShow)
                return;
            
            base.Show();
            
            
            // 게임베이스 아이템 정보 
            try {
                gamebaseItem = BillingManager.main.GetGamebasePurchaseItem("oneday_pass");
                textPrice.text = gamebaseItem.localizedPrice;
            }
            catch {
                Debug.Log("Windows standalone?");
            }
            
            // 텍스트 세팅 
            
            Debug.Log(string.Format(SystemManager.GetLocalizedText("6455"), BillingManager.main.onedayPassChoiceSale));
            SystemManager.SetText(textChoicesSaleText, string.Format(SystemManager.GetLocalizedText("6455"), BillingManager.main.onedayPassChoiceSale.ToString()));
            textChoicesOff.text = BillingManager.main.onedayPassChoiceSale.ToString() +"%\n<size=12>OFF</size>" ;
            textChoicesOff2.text = BillingManager.main.onedayPassChoiceSale.ToString() +"%\n<size=12>OFF</size>" ;
            
            currentStory = SystemListener.main.introduceStory; // 리스너에서 받아온다. 
            
            SystemManager.SetText(textTitle, currentStory.title); // 타이틀 
            
            
            storyImage.SetDownloadURL(currentStory.coinBannerUrl, currentStory.coinBannerKey);
            
           
            // 원데이 패스 사용중일때, 아닐때의 분류하기. 
            if(currentStory.IsValidOnedayPass()) { // 사용중 
                isPurchasable = false; 
                SystemManager.SetText(textPrice, SystemManager.GetLocalizedText("6464"));
                
            }
            else { // 사용중이지 않음. (구매가능)
                isPurchasable= true;
            }
            
            
        }
        
        public void OnClickPurchase() {
            
            if(!isPurchasable) {
                Debug.LogError("It's not purchasable oneday pass");
                return;
            }
            
            BillingManager.main.RequestPurchaseGamebase("oneday_pass", currentStory.projectID);
        }
    }
}