using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BestHTTP;
using LitJson;

namespace PIERStory {
    
    /// <summary>
    /// 에피소드 클리어 보상 팝업 
    /// </summary>
    public class PopupEpisodeClear : PopupBase
    {
        [SerializeField] ImageRequireDownload iconImage;
        
        [SerializeField] TextMeshProUGUI textQuantity;
        
        [SerializeField] string iconURL = string.Empty;
        [SerializeField] string iconKey = string.Empty;
        string currency = string.Empty;
        int quantity = 0;
        
        
        
        public override void Show()
        {
            Debug.Log(">>> PopupEpisodeClear SHOW");
            
            base.Show();
            
            // 아이콘 이미지 
            currency = SystemManager.GetJsonNodeString(Data.contentJson, "currency");
            iconURL = SystemManager.GetJsonNodeString(Data.contentJson, "icon_url");
            iconKey = SystemManager.GetJsonNodeString(Data.contentJson, "icon_key");
            
            iconImage.SetDownloadURL(iconURL, iconKey);
            
            quantity = SystemManager.GetJsonNodeInt(Data.contentJson, "quantity");
            
            // 수량
            textQuantity.text = quantity.ToString();
            
            Invoke("OnShow", 0.5f);
        }
        
        public void OnShow() {
            
            Debug.Log(">>>>> Update EXP <<<<<<");
            
            // * 최초 클리어 경험치 연계하기  
            NetworkLoader.main.UpdateUserExp(10, "episode_clear", -1); // 경험치 현재 10으로 고정됨. 
            
        }
        
        /// <summary>
        /// 즉시 획득
        /// </summary>
        public void OnClickGet() {
            Hide();
            UserManager.main.RefreshIndicators(); // 상단 갱신만 하면 된다. (이미 재화는 들어온 상태)
        }

        /// <summary>
        /// 광고 2배 받기 
        /// </summary>        
        public void OnClickDouble() {
            
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
            NetworkLoader.main.AddUserProperty(currency, quantity, "first_clear_double", OnDoubleReward);
            
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
            Hide(); 
        }
        
    }
}