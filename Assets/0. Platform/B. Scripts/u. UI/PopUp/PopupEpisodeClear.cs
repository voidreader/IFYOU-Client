using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;

namespace PIERStory {
    
    /// <summary>
    /// 에피소드 클리어 보상 팝업 
    /// </summary>
    public class PopupEpisodeClear : PopupBase
    {
        [SerializeField] Image iconImage;
        
        [SerializeField] TextMeshProUGUI textBaseQuantity;
        [SerializeField] TextMeshProUGUI textBonusQuantity;

        string currency = string.Empty;
        public int quantity = 0;
        public int bonusQuantity = 0;
        
        
        public override void Show()
        {
            if(isShow)
                return;
            
            Debug.Log(">>> PopupEpisodeClear SHOW");
            
            base.Show();
            
            // 아이콘 이미지 
            currency = SystemManager.GetJsonNodeString(Data.contentJson, LobbyConst.NODE_CURRENCY);
            
            string currencyName = string.Empty;
            
            if(currency == LobbyConst.COIN) {
                iconImage.sprite = SystemManager.main.spriteCoin; // 코인
                currencyName = SystemManager.GetLocalizedText("2001");
            }
            else {
                iconImage.sprite = SystemManager.main.spriteStar; // 스타 
                currencyName = SystemManager.GetLocalizedText("2000");
            }
            
            
            quantity = SystemManager.GetJsonNodeInt(Data.contentJson, CommonConst.NODE_QUANTITY); // 보상 
            bonusQuantity = quantity * 5; // 보너스 수량 
            
            // 수량
            textBaseQuantity.text = quantity.ToString() + " " + currencyName;
            textBonusQuantity.text = bonusQuantity.ToString() + " " + currencyName;

        }
        
        
        /// <summary>
        /// 즉시 획득
        /// </summary>
        public void OnClickGet() {
            
            // 재화 요청
            NetworkLoader.main.RequestEpisodeFirstClearReward(currency, quantity, false);
            
            Hide();
        }

        /// <summary>
        /// 광고 2배 받기 
        /// </summary>        
        public void OnClickDouble() {
            
            // 광고 재생이 가능한지 체크 
            if(!AdManager.main.CheckRewardedAdPossible()) {
                
                SystemManager.ShowSimpleAlertLocalize("6093");
                return;
            }
            
            // 광고 호출한다.
            AdManager.main.ShowRewardAdWithCallback(DoubleReward);
        }
        
        /// <summary>
        /// 광고를 다 보면 호출된다.
        /// </summary>
        /// <param name="__flag"></param>
        void DoubleReward(bool __flag) {
            
            if(!__flag) {
                SystemManager.ShowSimpleAlertLocalize("6094"); // 광고가 끝까지 재생되지 않았습니다.
                return;
            }
            
            Debug.Log("### DoubleReward ###");
            
            Hide();
            
            // 재화 요청
            NetworkLoader.main.RequestEpisodeFirstClearReward(currency, quantity, true);
        }
        
        /// <summary>
        /// 2배 받기 통신 완료
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void OnDoubleReward(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            
            // 뱅크 
            UserManager.main.SetBankInfo(result);
            
            // 팝업 끝.
            base.InstanteHide();
            
            SystemManager.ShowMessageWithLocalize("6190");
        }
        
    }
}