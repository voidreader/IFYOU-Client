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
                gamebaseItem = BillingManager.main.GetGamebasePurchaseItem(currentStory.premiumSaleID); // 연결된 상품ID로 조회한다. 
                textPrice.text = gamebaseItem.localizedPrice;
            }
            catch {
                isPurchasable = false;
                textPrice.text = "ERROR";
                Debug.Log("Windows standalone?");
                
                return;
            }
            
            // 구매 가능여부에 대한 체크 
            hasPremiumPass = currentStory.hasPremiumPass;
            isPurchasable = !hasPremiumPass;
            
            if(!isPurchasable) {
                SystemManager.SetText(textPrice, SystemManager.GetLocalizedText("6464"));
            }
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