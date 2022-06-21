using UnityEngine;

using TMPro;
using LitJson;
using Toast.Gamebase;

namespace PIERStory {

    public class BaseStarProduct : MonoBehaviour
    {
        
        public string productID = string.Empty; // 마켓 등록 ID
        [SerializeField] GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        
        // 게임서버 기준정보 
        JsonData productMasterJSON; // 마스터 
        JsonData productDetailJSON; // 디테일
        
        
        [SerializeField] string productMasterID = string.Empty; // 마스터 ID 
        [SerializeField] bool hasPurchaseHistory = false; // 구매 기록 
        
        public int mainGemQuantity = 0; // 메인 젬 수량 
        public int subGemQuantity = 0; // 서브 젬 수량 
        [SerializeField] int coinQuantity = 0; // 코인 수량
        public int firstPurchaseBonusGem = 0; // 첫 구매 보너스 젬 
        
        
        [SerializeField] TextMeshProUGUI textMainStar; // 스타 재화 
        [SerializeField] TextMeshProUGUI textCoinQuantity; // 코인 재화 수량
        
        [SerializeField] TextMeshProUGUI textPrice; // 가격 
        [SerializeField] TextMeshProUGUI textFirstPurchaseBonus; // 첫 구매 보너스
        
        [SerializeField] GameObject groupFirstPurchaseBonus; // 첫구매 보너스 그룹 
        
        
        /// <summary>
        /// 상품 초기화
        /// </summary>
        public void InitProduct() {
            
            if(!gameObject.activeSelf)
                return;

            InitProduct(productID);
        }

        public void InitProduct(string __productId)
        {
            // 게임서버, 게임베이스에서 각각 정보를 가져온다. 
            productID = __productId;
            gamebaseItem = BillingManager.main.GetGamebasePurchaseItem(productID);


            // 가격 정보 
            if (gamebaseItem != null)
            {
                Debug.Log("localizedPrice : " + gamebaseItem.localizedPrice);
                textPrice.text = gamebaseItem.localizedPrice;
            }
            else
            {
                Debug.Log("gamebaseItem is Null!!!!!");
            }

            // 게임 Product 정보 
            productMasterJSON = BillingManager.main.GetGameProductItemMasterInfo(productID);

            if (productMasterJSON != null)
            {
                productMasterID = SystemManager.GetJsonNodeString(productMasterJSON, "product_master_id"); // master_id
                hasPurchaseHistory = BillingManager.main.CheckProductPurchaseCount(productMasterID) > 0; // 구매 내역

                productDetailJSON = BillingManager.main.GetGameProductItemDetailInfo(productMasterID); // 디테일 
            }


            if (productDetailJSON == null)
            {
                Debug.Log(string.Format("[{0}] has no info in game server ", productID));
                return;
            }

            SetMainStarQuantity();
            SetCoinQuantity();

            if (hasPurchaseHistory)
            {
                groupFirstPurchaseBonus.SetActive(false);
                return;
            }

            // 첫구매 기록이 없는 경우.
            SetFirstPurchaseBonus();
        }
        
        
        /// <summary>
        /// 메인 젬 수량 설정, 타이틀 설정. 
        /// </summary>
        void SetMainStarQuantity() {
            mainGemQuantity = getMainStarQuantity();
            subGemQuantity = getSubStarQuantity();
            
            
            textMainStar.text = mainGemQuantity.ToString(); 
            
            if(subGemQuantity > 0) {
                textMainStar.text += "<color=#F6C261>+" + subGemQuantity.ToString() +"</color>"; // 더하기 붙인다. 
            }
            
        }
        
        /// <summary>
        /// 코인 수량 설정 
        /// </summary>
        void SetCoinQuantity() {
            coinQuantity = getCoinQuantity();
            textCoinQuantity.text = "+ " + string.Format("{0:#,0}", coinQuantity); 
            
        }
        
        /// <summary>
        /// 메인 재화 수량 받기 
        /// </summary>
        /// <returns></returns>                
        int getMainStarQuantity() {
            
            int quantity = 0;
            
            if(productDetailJSON == null) 
                return quantity;
                
            for(int i=0; i< productDetailJSON.Count; i++) {
                // 메인이면서 화폐가 보석인것만. 
                if (!SystemManager.GetJsonNodeBool(productDetailJSON[i], "first_purchase")
                    && SystemManager.GetJsonNodeBool(productDetailJSON[i], "is_main")
                    && SystemManager.GetJsonNodeString(productDetailJSON[i], LobbyConst.NODE_CURRENCY) == LobbyConst.GEM) { 
                    
                    quantity += int.Parse(SystemManager.GetJsonNodeString(productDetailJSON[i], CommonConst.NODE_QUANTITY));
                }
            }
            
            return quantity;
        }
        
        int getSubStarQuantity() {
            int quantity = 0;
            
            if(productDetailJSON == null) 
                return quantity;
                
            for(int i=0; i< productDetailJSON.Count; i++) {
                // 메인이 아니면서 화폐가 보석인것만. 
                if(!SystemManager.GetJsonNodeBool(productDetailJSON[i], "first_purchase")
                    && !SystemManager.GetJsonNodeBool(productDetailJSON[i], "is_main")
                    && SystemManager.GetJsonNodeString(productDetailJSON[i], LobbyConst.NODE_CURRENCY) == LobbyConst.GEM) { 
                    
                    quantity += int.Parse(SystemManager.GetJsonNodeString(productDetailJSON[i], CommonConst.NODE_QUANTITY));
                }
            }
            
            return quantity;
        }
        
        
        /// <summary>
        /// 코인 재화 수량 
        /// </summary>
        /// <returns></returns>
        int getCoinQuantity() {
            
            int quantity = 0;
            
            if(productDetailJSON == null) 
                return quantity;
                
            for(int i=0; i< productDetailJSON.Count; i++) {
                // 메인이면서 화폐가 보석인것만. 
                if(SystemManager.GetJsonNodeString(productDetailJSON[i], LobbyConst.NODE_CURRENCY) == LobbyConst.COIN) { 

                    

                    // 첫 구매 코인 보너스 체크할것. 
                    if(SystemManager.GetJsonNodeBool(productDetailJSON[i], "first_purchase")) {
                        
                        if(!hasPurchaseHistory)
                            quantity += int.Parse(SystemManager.GetJsonNodeString(productDetailJSON[i], CommonConst.NODE_QUANTITY));    
                    }
                    else {
                        quantity += int.Parse(SystemManager.GetJsonNodeString(productDetailJSON[i], CommonConst.NODE_QUANTITY));    
                    }
                }
            }
            
            return quantity;
        }
        
        /// <summary>
        /// 첫구매 보너스 설정
        /// </summary>
        void SetFirstPurchaseBonus() {
            groupFirstPurchaseBonus.SetActive(true);
            
            firstPurchaseBonusGem = getFirstPurchaseStar();
            
            // textFirstPurchaseBonus.text = SystemManager.GetLocalizedText("5018") + string.Format(" <color=#FF59C2>+ {0}</color>", firstPurchaseBonusGem);
            textFirstPurchaseBonus.text = "+" + firstPurchaseBonusGem.ToString();
        }
        
        /// <summary>
        /// 첫 구매 보너스 보석 
        /// </summary>
        /// <returns></returns>
        int getFirstPurchaseStar() {
            if(productDetailJSON == null) 
                return 0;
                
            for(int i=0; i< productDetailJSON.Count; i++) {
                // 메인이면서 화폐가 보석인것만. 
                if(SystemManager.GetJsonNodeString(productDetailJSON[i], LobbyConst.NODE_CURRENCY) == LobbyConst.GEM
                    && SystemManager.GetJsonNodeBool(productDetailJSON[i], "first_purchase")) { 
                        
                    return SystemManager.GetJsonNodeInt(productDetailJSON[i], CommonConst.NODE_QUANTITY);
                }
            }
            
            return 0;
        }
        
        
        
        /// <summary>
        /// 구매 시작 
        /// </summary>
        public void OnClickProduct() { 
            BillingManager.main.RequestPurchaseGamebase(productID);
        }
        
    }
}