using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BestHTTP;
using LitJson;

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
            currency = SystemManager.GetJsonNodeString(Data.contentJson, "currency");
            
            if(currency == "coin") {
                iconImage.sprite = SystemManager.main.spriteCoin;
            }
            else {
                iconImage.sprite = SystemManager.main.spriteStar;
            }
            
            
            quantity = SystemManager.GetJsonNodeInt(Data.contentJson, "quantity"); // 보상 
            bonusQuantity = quantity * 5; // 보너스 수량 
            
            // 수량
            textBaseQuantity.text = quantity.ToString() + " " + SystemManager.GetLocalizedText("6240");
            textBonusQuantity.text = bonusQuantity.ToString() + " " + SystemManager.GetLocalizedText("6240");

        }
        
        
        /// <summary>
        /// 즉시 획득
        /// </summary>
        public void OnClickGet() {
            Hide();
            // UserManager.main.RefreshIndicators(); // 상단 갱신만 하면 된다. (이미 재화는 들어온 상태)
            
            // 재화 요청
            NetworkLoader.main.RequestEpisodeFirstClearReward(currency, quantity, false);
            
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
            
            // 서버에 2배 보상 요청 
            //NetworkLoader.main.AddUserProperty(currency, quantity, "first_clear_double", OnDoubleReward);
            
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