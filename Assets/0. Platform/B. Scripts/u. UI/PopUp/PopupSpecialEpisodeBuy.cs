using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {
    public class PopupSpecialEpisodeBuy : PopupBase
    {
        public GameObject groupDoubleButton; // 프리미엄 패스 미소유 유저 버튼 2개
        public GameObject groupSingleButton; // 프리미엄 패스 소유 유저용 단일 버튼 
        
        public int playPrice = 0;
        public bool hasPremiumPass = false;
        
        [SerializeField] EpisodeData targetEpisode; // 플레이 대상 스페셜 에피소드
        
        [SerializeField] TextMeshProUGUI textPrice; // 리셋 코인 가격
        
        public override void Show() {
            
            base.Show();
            
            hasPremiumPass = UserManager.main.HasProjectFreepass();
            // 프리미엄 패스 보유여부에 따라서 버튼 뜨는게 다르다. 
            groupDoubleButton.SetActive(!hasPremiumPass);
            groupSingleButton.SetActive(hasPremiumPass);
            
            targetEpisode = Data.contentEpisode;
            
            textPrice.text = Data.contentEpisode.priceStarPlaySale.ToString();
        }
        
        /// <summary>
        /// 돈내고 플레이 
        /// </summary>
        public void OnClickPlay() {
            UserManager.OnRequestEpisodePurchase = PurchasePostProcess;
            NetworkLoader.main.PurchaseEpisode(targetEpisode.episodeID, PurchaseState.Permanent, targetEpisode.currencyStarPlay, targetEpisode.priceStarPlaySale.ToString());
        }
        
        
        /// <summary>
        /// 프리미엄 패스 화면 오픈 
        /// </summary>
        public void OnClickPremiumPass() {
            
            this.Hide(); // 닫고 
            
            // 프리미엄 팝업 오픈 
            PopupBase p = PopupManager.main.GetPopup("PremiumPass");
                
            PopupManager.main.ShowPopup(p, false, false);
        }
        
        
        /// <summary>
        /// 구매 후 처리 
        /// </summary>
        /// <param name="__isPurchaseSuccess"></param>
        void PurchasePostProcess(bool __isPurchaseSuccess) {
            if (!__isPurchaseSuccess)
            {
                Debug.LogError("Error in purchase");
                SystemManager.HideNetworkLoading();
                return;
            }
            
            targetEpisode.SetPurchaseState();
            Data.positiveButtonCallback?.Invoke();
        }
        
    }
}