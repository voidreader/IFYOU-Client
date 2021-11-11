using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Toast.Gamebase;
using LitJson;
using BestHTTP;

namespace PIERStory {
    public class BillingManager : SerializedMonoBehaviour
    {
        public static BillingManager main = null;
        public static bool isInit = false;
        
        [SerializeField] List<GamebaseResponse.Purchase.PurchasableItem> ListGamebaseProducts; // 게임베이스에 등록된 인앱 상품 정보 
    
        JsonData productMasterJSON;
        JsonData productDetailJSON;
        JsonData userPurchaseHistoryJSON = null;
    
        const string NODE_PRODUCT_MASTER = "productMaster";
        const string NODE_PRODUCT_DETAIL = "productDetail";
        
        
        IEnumerator Start() {
            if(main != null) {
                Destroy(this.gameObject);
                yield break;
            }
            
            
            main = this;
            
            while(!UserManager.main.completeReadUserData)
                yield return null;
                
            isInit = true;
            
            Debug.Log("Billing is ready.....");
            DontDestroyOnLoad(this.gameObject);
            
            // ! 게임베이스와 게임서버의 상품정보를 둘다 받아와야 한다.
            
            // 게임서버의 상품 정보 받아오기 
            NetworkLoader.main.RequestGameProductList();
            
            // 유저의 구매 내역 받아오기
            NetworkLoader.main.RequestUserPurchaseHistory();
            
            
            // 게임베이스 상품 정보 받아오기 
            RequestGamebaseProductList();
        }
        
        
        /// <summary>
        /// 게임베이스 등록상품 정보 받아오기 
        /// </summary>
        void RequestGamebaseProductList() {
            RequestItemListPurchasable();
            RequestItemListOfNotConsumed();
        }
        
        /// <summary>
        /// 게임베이스에 등록된 상품 리스트 받아오기 . 
        /// </summary>
        public void RequestItemListPurchasable()
        {
            Gamebase.Purchase.RequestItemListPurchasable((purchasableItemList, error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    Debug.Log("RequestItemListPurchasable list received : " + purchasableItemList.Count);
                    ListGamebaseProducts = purchasableItemList;
                    
                    foreach(GamebaseResponse.Purchase.PurchasableItem item in purchasableItemList) {
                        Debug.Log(string.Format("{0}/{1}/{2}", item.gamebaseProductId, item.marketItemId, item.localizedDescription));
                    }
                }
                else
                {
                    Debug.Log(string.Format("Get list failed. error is {0}", error));
                }
            });
        }
        
        
        /// <summary>
        /// 구매 처리가 완료되지 못하고 중단된 상품들 
        /// </summary>        
        public void RequestItemListOfNotConsumed()
        {
            Gamebase.Purchase.RequestItemListOfNotConsumed((purchasableReceiptList, error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    Debug.Log("Get list succeeded, " + purchasableReceiptList.Count);
                    

                    // Should Deal With This non-consumed Items.
                    // Send this item list to the game(item) server for consuming item.
                }
                else
                {
                    Debug.Log(string.Format("Get list failed. error is {0}", error));
                }
            });
        }        
        
        /// <summary>
        /// 게임베이스 구매 처리 
        /// </summary>
        /// <param name="gamebaseProductId"></param>
        public void RequestPurchaseGamebase(string gamebaseProductId) {
            Debug.Log(string.Format("RequestPurchase Called [{0}]", gamebaseProductId));
            
            if(string.IsNullOrEmpty(gamebaseProductId)) {
                return;
            }
            
            // 약간 지연이 있기 때문에 네트워크 로딩 표시 
            SystemManager.ShowNetworkLoading();
            
            // 구매 프로세스의 시작 
            Gamebase.Purchase.RequestPurchase(gamebaseProductId, (purchasableReceipt, error) =>
            {
                if (Gamebase.IsSuccess(error)) // * 성공
                {
                    Debug.Log("Purchase succeeded.");
                    string logMessage = string.Empty;
                    
                    logMessage += string.Format("gamebaseProductID : [{0}]", purchasableReceipt.gamebaseProductId);
                    logMessage += string.Format("\nitemSeq : [{0}]", purchasableReceipt.itemSeq);
                    logMessage += string.Format("\nprice : [{0}]", purchasableReceipt.price);
                    logMessage += string.Format("\ncurrency : [{0}]", purchasableReceipt.currency);
                    logMessage += string.Format("\npaymentSeq : [{0}]", purchasableReceipt.paymentSeq);
                    logMessage += string.Format("\npurchaseToken : [{0}]", purchasableReceipt.purchaseToken);
                    logMessage += string.Format("\nmarketItemId : [{0}]", purchasableReceipt.marketItemId);
                    logMessage += string.Format("\npaymentId : [{0}]", purchasableReceipt.paymentId);
                       
                    Debug.Log(logMessage);
                    
                    RequestPurchaseReward(purchasableReceipt);
                }
                else
                {
                    SystemManager.HideNetworkLoading(); 
                    
                    if (error.code == (int)GamebaseErrorCode.PURCHASE_USER_CANCELED)
                    {
                        Debug.Log("User canceled purchase.");
                    }
                    else
                    {
                        Debug.Log(string.Format("Purchase failed. error is {0}", error));
                    }
                }
            });
        } // ? end of RequestPurchaseGamebase
        
        /// <summary>
        /// 인앱 결제 완료 후 요청
        /// </summary>
        /// <param name="receipt"></param>
        void RequestPurchaseReward(GamebaseResponse.Purchase.PurchasableReceipt receipt) {
            JsonData sendData = new JsonData();
            sendData["func"] = "userPurchase";
            sendData["product_id"] = receipt.gamebaseProductId;
            sendData["receipt"] = receipt.paymentId;
            sendData["paymentSeq"] = receipt.paymentSeq;
            sendData["purchaseToken"] = receipt.purchaseToken;
            sendData["price"] = receipt.price;
            sendData["currency"] = receipt.currency;
            
            NetworkLoader.main.SendPost(OnRequestPurchaseReward, sendData, true);
            
            // 구매 내역. (첫 구매 판단 여부)
            /*
            bool hasPurchaseHistory = BillingManager.main.CheckProductPurchaseHistory(receipt.gamebaseProductId); 
            string eventName = string.Empty;
            
            if(hasPurchaseHistory)
                eventName = "USER_PURCHASE_";
            else 
                eventName = "USER_FRIST_PURCHASE_";
            
            // AppsFlyer 수익 처리 추가             
            eventName += receipt.gamebaseProductId;
            AppsFlyerSDK.AppsFlyer.sendEvent(eventName, null); 
            
            // 추가 (통합 인앱결제)
            Dictionary<string, string> appsflyerParam = new Dictionary<string, string>();
            // id, price, 통화
            appsflyerParam.Add("af_product", receipt.gamebaseProductId); 
            appsflyerParam.Add("af_revenue", receipt.price.ToString());
            appsflyerParam.Add("af_currency", receipt.currency);
            AppsFlyerSDK.AppsFlyer.sendEvent("IN_APP_PURCHASE", appsflyerParam);
            */
  
        }
        
        /// <summary>
        /// callback
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void OnRequestPurchaseReward(HTTPRequest request, HTTPResponse response) {
            
            SystemManager.HideNetworkLoading();
            
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                
                // 결제 보상 지급 실패. 메세지 안내처리 
                return;
            }
            
            Debug.Log(response.DataAsText);
            
            // 받은 데이터 처리
            // bank, unreadMailCount, userPurchaseHistory
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            UserManager.main.SetNotificationInfo(result); // unreadMailCount
            
            // 히스토리 갱신 
            if(result.ContainsKey("userPurchaseHistory"))
                userPurchaseHistoryJSON = result["userPurchaseHistory"];
                
            // Shop 리프레시
            // ViewShop.RefreshShopView?.Invoke();
            
            SystemManager.ShowSimpleMessagePopUp("결제가 완료되었습니다. 우편함을 확인해주세요");
        }
        
        
        /// <summary>
        /// 유저의 유료 상품 구매내역 callback
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void OnRequestUserPurchaseHistory(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            Debug.Log("OnRequestUserPurchaseHistory : " + response.DataAsText);
            
            // userPurchaseHistory
            userPurchaseHistoryJSON = JsonMapper.ToObject(response.DataAsText);
        }
        
        
        
        /// <summary>
        /// 게임 서버의 상품 리스트 받아오기 callback
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void OnRequestGameProductList(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            productMasterJSON = result[NODE_PRODUCT_MASTER];
            productDetailJSON = result[NODE_PRODUCT_DETAIL];
            
        }
        
        
        /// <summary>
        /// 게임베이스에 등록된 상품 정보 ID로 가져오기 
        /// </summary>
        /// <param name="__id"></param>
        /// <returns></returns>
        public GamebaseResponse.Purchase.PurchasableItem GetGamebasePurchaseItem(string __id) {
            if(ListGamebaseProducts == null)
                return null;
                
            for(int i=0; i<ListGamebaseProducts.Count;i++) {
                if(ListGamebaseProducts[i].gamebaseProductId == __id) 
                    return ListGamebaseProducts[i];
            }
            
            return null;
        }
        
        
        /// <summary>
        /// 게임 서버 상품 - 마스터정보 가져오기
        /// </summary>
        /// <param name="__id"></param>
        /// <returns></returns>
        public JsonData GetGameProductItemMasterInfo(string __id) {
            for(int i=0; i<productMasterJSON.Count;i++) {
                if(productMasterJSON[i]["product_id"].ToString() == __id)
                    return productMasterJSON[i];
            }
            
            return null;
        }
        
        /// <summary>
        /// 게임 서버 상품 - 디테일정보 가져오기
        /// </summary>
        /// <param name="__productMasterID"></param>
        /// <returns></returns>
        public JsonData GetGameProductItemDetailInfo(string __productMasterID) {
            if(!productDetailJSON.ContainsKey(__productMasterID))
                return null;
            
            
            return productDetailJSON[__productMasterID];
        }
        
        
        /// <summary>
        /// 전달받은 ID로 구매 내역이 있는지 체크
        /// </summary>
        /// <param name="__productMasterID"></param>
        /// <returns></returns>
        public bool CheckProductPurchaseHistory(string __product_id) {
            
            Debug.Log(">> CheckProductPurchaseHistory : " + __product_id);
            
            if(userPurchaseHistoryJSON == null)
                return false;
            
            /*    
            if(userPurchaseHistoryJSON.ContainsKey(__productMasterID))
                return true;
            */
            for(int i=0; i<userPurchaseHistoryJSON.Count;i++) {
                if(SystemManager.GetJsonNodeString(userPurchaseHistoryJSON[i], "product_id") == __product_id)
                    return true;
            }
            
            return false;
        }
        
        
    } // ? end of class
}