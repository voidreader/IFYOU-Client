using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;


namespace PIERStory {
    public class BaseCoinExchangeProduct : MonoBehaviour
    {
        public string exchangeProductID = string.Empty; // 아이디 인스펙터에서 미리 지정 
        JsonData exchangeData = null;
        
        [SerializeField] TextMeshProUGUI textQuantity;
        [SerializeField] TextMeshProUGUI textPrice; // 일반 가격 
        [SerializeField] TextMeshProUGUI textNormalBonus; // 일반 보너스 개수 
        
        [SerializeField] GameObject groupNormalBonus; // 일반 보너스 그룹 
        [SerializeField] GameObject specialFrame; // 프리 프레임 
        
        
        [SerializeField] GameObject btnNormal;
        [SerializeField] GameObject btnFree;
        [SerializeField] GameObject btnFreeDisable;

        [SerializeField] bool isAvailable = false; // 사용 가능 
    
        /// <summary>
        /// 초기화 
        /// </summary>
        public void InitExchangeProduct() {
            btnFree.SetActive(false);
            btnNormal.SetActive(false);
            btnFreeDisable.SetActive(false);
            
            groupNormalBonus.SetActive(false);
            specialFrame.SetActive(false);
            
            exchangeData = BillingManager.main.GetCoinExchangeProductInfo(exchangeProductID);
            
            if(exchangeData == null)
                return;
                
            // 사용 가능 여부 
            isAvailable = SystemManager.GetJsonNodeBool(exchangeData, "exchange_check");
            
            if(exchangeProductID == "1") { // 1번은 특별 프리 아이템 
                if(isAvailable) {
                    btnFree.SetActive(true);
                    specialFrame.SetActive(true);
                }
                else {
                    btnFreeDisable.SetActive(true);
                }
            }
            else {
                btnNormal.SetActive(true);
            }
            
            
            textQuantity.text = SystemManager.GetJsonNodeString(exchangeData, "coin_quantity");
            textNormalBonus.text = SystemManager.GetJsonNodeString(exchangeData, "bonus_quantity");
            textPrice.text = SystemManager.GetJsonNodeString(exchangeData, "star_quantity");
            
            if(SystemManager.GetJsonNodeInt(exchangeData, "bonus_quantity") > 0)
                groupNormalBonus.SetActive(true);
                
            
            
            // this.gameObject.SetActive(true);
        }
        
        
        public void OnClickExchange() {
            if(exchangeData == null)
                return;
                
            Debug.Log("OnClickExchange");
                
            // 환전 시작 
            BillingManager.main.ExchangeStarToCoin(exchangeProductID);
        }
    }
}