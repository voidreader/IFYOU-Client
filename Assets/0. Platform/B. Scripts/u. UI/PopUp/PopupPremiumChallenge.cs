using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toast.Gamebase;
using TMPro;

namespace PIERStory {

    public class PopupPremiumChallenge : PopupBase
    {
        public TextMeshProUGUI textPrice;
        
        public TextMeshProUGUI textOriginStarPrice; // 스타 판매 원가격
        public TextMeshProUGUI textDiscountStarPrice; // 스타 판매 할인가격
        public GameObject btnStarPurchase;
        
        public TextMeshProUGUI textTitle; // 타이틀 
        
        
        public ImageRequireDownload storyImage;
                
        public bool isPurchasable = false; // 구매가능 상태 
        public bool hasPremiumPass = false; // 프리미엄 패스 보유 여부 
        GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        
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
                
                gamebaseItem = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumSaleID); // 연결된 상품ID로 조회한다. 
                textPrice.text = gamebaseItem.localizedPrice;
            }
            catch {
                isPurchasable = false;
                textPrice.text = "ERROR";
                Debug.Log("Windows standalone?");
                
                // return;
            }
            
            // 구매 가능여부에 대한 체크 
            hasPremiumPass = currentStory.hasPremiumPass;
            isPurchasable = !hasPremiumPass;
            
            if(!isPurchasable) {
                SystemManager.SetText(textPrice, SystemManager.GetLocalizedText("6464"));
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