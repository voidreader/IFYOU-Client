using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toast.Gamebase;
using TMPro;

namespace PIERStory {

    public class PopupPremiumChallenge : PopupBase
    {
        public TextMeshProUGUI textInAppNoSale;
        public GameObject groupDiscountInApp; // 할인 그룹 
        public TextMeshProUGUI textInAppNoSale2; // 인앱상품 일반 가격
        public TextMeshProUGUI textInappDiscountPrice; // 인앱상품 할인가격
        
        [Space]
        public TextMeshProUGUI textOriginStarPrice; // 스타 판매 원가격
        public TextMeshProUGUI textDiscountStarPrice; // 스타 판매 할인가격
        public GameObject btnStarPurchase;
        
        public TextMeshProUGUI textTitle; // 타이틀 
        
        
        public ImageRequireDownload storyImage;
                
        public bool isPurchasable = false; // 구매가능 상태 
        public bool hasPremiumPass = false; // 프리미엄 패스 보유 여부 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItemNoDiscount = null; // 게임베이스 기준정보 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItemDiscount = null; 
        
        [Header("챌린지")]
        public List<ChallengeRow> listRows;
        
        [Header("대상 작품 ")]
        public StoryData currentStory;
        
       public override void Show() {
            if(isShow)
                return;
            
            base.Show();
        
            
            
            currentStory = StoryManager.main.CurrentProject;
            
            SystemManager.SetText(textTitle, currentStory.title); // 타이틀      
            storyImage.SetDownloadURL(currentStory.coinBannerUrl, currentStory.coinBannerKey); // 이미지 처리
            
            // 챌린지 세팅 
            InitChallenge();
            
            // 게임베이스 아이템 정보 
            try {
                
                // 스타 판매 가격 설정 
                textOriginStarPrice.text = currentStory.passPrice.ToString();
                textDiscountStarPrice.text = currentStory.discountPassPrice.ToString();
                
                gamebaseItemDiscount = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumSaleID);  // 할인 인앱 
                gamebaseItemNoDiscount = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumProductID); // 미할인 인앱 
                
                
                textInAppNoSale.text = gamebaseItemNoDiscount.localizedPrice;
                textInAppNoSale2.text = gamebaseItemNoDiscount.localizedPrice;
                textInappDiscountPrice.text = gamebaseItemDiscount.localizedPrice;
            }
            catch {
                isPurchasable = false;
                textInAppNoSale.text = "ERROR";
                Debug.Log("Windows standalone?");
                // return;
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
            
            // 구매 가능여부에 대한 체크 
            hasPremiumPass = currentStory.hasPremiumPass;
            isPurchasable = !hasPremiumPass;
            
            if(!isPurchasable) {
                SystemManager.SetText(textInAppNoSale, SystemManager.GetLocalizedText("6464"));
                // 구매 가능하지 않은 경우는 group 비활성화 
                textInAppNoSale.gameObject.SetActive(true);
                groupDiscountInApp.SetActive(false);
            }
            
            
            btnStarPurchase.SetActive(isPurchasable);
        }
        
        /// <summary>
        /// 챌린지 초기화 
        /// </summary>
        void InitChallenge() {
            
            Debug.Log(">> InitChallenge");
            
            for(int i=0; i<listRows.Count; i++) {
                listRows[i].gameObject.SetActive(false);
            }
            
            for(int i=0; i<StoryManager.main.listChallenges.Count; i++) {
                listRows[i].SetChallengeRow(StoryManager.main.listChallenges[i]);
            }
        }
                
        public void OnClickPurchase() {
            
            Debug.Log("OnClickPurchase Premium Challenge #1 :: " + currentStory.premiumSaleID);
            
            if(!isPurchasable) {
                Debug.LogError("It's not purchasable premium pass");
                return;
            }
            Debug.Log("OnClickPurchase Premium Challenge #2 :: " + currentStory.premiumSaleID);
            
            BillingManager.main.RequestPurchaseGamebase(currentStory.premiumSaleID, StoryManager.main.CurrentProjectID);
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
            
            // SystemManager.ShowResourceConfirm(SystemManager.GetLocalizedText("6477"), )
            // 물어보고 진행한다. 
            SystemManager.ShowSystemPopup(string.Format(SystemManager.GetLocalizedText("6477"), currentStory.discountPassPrice), PurchasePremiumPassByStar, null);
           
        }
        
        void PurchasePremiumPassByStar() {
            // 통신 처리 
            NetworkLoader.main.PurchasePremiumPassByStar(currentStory.projectID, currentStory.discountPassPrice);
        }
        
        
        public void OnClickBenefit() { 
            SystemManager.ShowNoDataPopup(CommonConst.POPUP_PREMIUM_PASS);
        }

        public override void Hide()
        {
            base.Hide();
            
            // 닫힐때 refresh. 
            if(StoryLobbyManager.main != null) {
                StoryLobbyTop.OnInitializeStoryLobbyTop?.Invoke();
            }
            
            if(GameManager.main != null) {
                EpisodeEndControls.OnRefreshPassButton?.Invoke();
            }
        }
    }
}