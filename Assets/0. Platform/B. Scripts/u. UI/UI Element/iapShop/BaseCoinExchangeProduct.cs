using UnityEngine;

using TMPro;
using LitJson;


namespace PIERStory {
    public class BaseCoinExchangeProduct : MonoBehaviour
    {
        public string exchangeProductID = string.Empty; // 아이디 인스펙터에서 미리 지정 
        JsonData exchangeData = null;
        
        [SerializeField] TextMeshProUGUI textQuantity;
        [SerializeField] TextMeshProUGUI textPrice; // 일반 가격 
        // [SerializeField] TextMeshProUGUI textNormalBonus; // 일반 보너스 개수 
        
        // [SerializeField] GameObject groupNormalBonus; // 일반 보너스 그룹 
        // [SerializeField] GameObject specialFrame; // 프리 프레임 
        
        
        [SerializeField] GameObject btnNormal;
        [SerializeField] GameObject btnFree;
        [SerializeField] GameObject btnFreeDisable;

        [SerializeField] bool isAvailable = false; // 사용 가능 
        public int price = 0;
        public int quantity = 0;
        
        public int bonusQuantity = 0;
    
        /// <summary>
        /// 초기화 
        /// </summary>
        public void InitExchangeProduct() {
            btnFree.SetActive(false);
            btnNormal.SetActive(false);
            btnFreeDisable.SetActive(false);
            
            // groupNormalBonus.SetActive(false);
            // specialFrame.SetActive(false);
            
            exchangeData = BillingManager.main.GetCoinExchangeProductInfo(exchangeProductID);
            
            if(exchangeData == null)
                return;
                
            // 사용 가능 여부 
            isAvailable = SystemManager.GetJsonNodeBool(exchangeData, "exchange_check");
            
            if(exchangeProductID == "1") { // 1번은 특별 프리 아이템 
                if(isAvailable) {
                    btnFree.SetActive(true);
                    // specialFrame.SetActive(true);
                }
                else {
                    btnFreeDisable.SetActive(true);
                }
            }
            else {
                btnNormal.SetActive(true);
            }
            
            
            quantity = SystemManager.GetJsonNodeInt(exchangeData, "coin_quantity") + SystemManager.GetJsonNodeInt(exchangeData, "bonus_quantity");
            
            textQuantity.text = SystemManager.GetJsonNodeString(exchangeData, "coin_quantity");
            
            bonusQuantity = SystemManager.GetJsonNodeInt(exchangeData, "bonus_quantity");
            
            
            textPrice.text = SystemManager.GetJsonNodeString(exchangeData, "star_quantity");
            price = SystemManager.GetJsonNodeInt(exchangeData, "star_quantity");
            
            if(bonusQuantity > 0) {
                textQuantity.text += "<color=#FF629C>+" + bonusQuantity.ToString() +"</color>"; // 더하기 붙인다. 
            }
            
            
            /*
            if(SystemManager.GetJsonNodeInt(exchangeData, "bonus_quantity") > 0)
                groupNormalBonus.SetActive(true);
            */
                
            
            
            // this.gameObject.SetActive(true);
        }
        
                
        /// <summary>
        /// 버튼 클릭
        /// </summary>
        public void OnClickExchange() {
            if(exchangeData == null)
                return;
                
            Debug.Log("OnClickExchange");
            
            if(price <= 0) {
                Exchange();
                return;
            }
            
            // 확인 팝업 
            // 스타 {0}개를 사용하여 코인 {1}개를 구입하겠습니까? (6204)
            // 구매(5039) / 취소 (5038)
            SystemManager.ShowResourceConfirm(string.Format(SystemManager.GetLocalizedText("6204"), price.ToString(), quantity.ToString())
                                            , quantity
                                            , SystemManager.main.GetCurrencyImageURL(LobbyConst.COIN)
                                            , SystemManager.main.GetCurrencyImageKey(LobbyConst.COIN)
                                            , Exchange
                                            , SystemManager.GetLocalizedText("5039")
                                            , SystemManager.GetLocalizedText("5038"));
                                            
        }
        
        void Exchange() {
            // 환전 시작 
            BillingManager.main.ExchangeStarToCoin(exchangeProductID);
        }
    }
}