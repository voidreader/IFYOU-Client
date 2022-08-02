using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using BestHTTP;
using AppsFlyerSDK;
using Toast.Gamebase;

namespace PIERStory {
    public class BillingManager : MonoBehaviour
    {
        public static BillingManager main = null;
        public static bool isInit = false;
        
        [SerializeField] List<GamebaseResponse.Purchase.PurchasableItem> ListGamebaseProducts; // 게임베이스에 등록된 인앱 상품 정보 
    
        public JsonData productMasterJSON;
        JsonData productDetailJSON;
        JsonData coinExchangeJSON; // 코인 환전 
        
        JsonData userPurchaseHistoryJSON = null; // 유저 구매내역(일반)
    
        const string NODE_PRODUCT_MASTER = "productMaster";
        const string NODE_PRODUCT_DETAIL = "productDetail";
        
        [Header("이프유 패스")]
        public int ifyouPassDirectStar = 0;  // 즉시 지급 스타 
        public int ifyouPassDailyStar = 0;  // 매일 지급 스타
        public int ifyouPassChoiceSale = 0;
        public float ifyouPassChoiceSaleFloat = 0; // 선택지 할인율 
        
        [Header("원데이 패스")]
        public int onedayPassChoiceSale = 0;
        public float onedayPassChoiceSaleFloat = 0; // 선택지 할인율 
        
        
        
        
        IEnumerator Start() {
            if(main != null) {
                Destroy(gameObject);
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
            
            // 코인 환전 상품 정보 가져오기
            NetworkLoader.main.RequestCoinExchangeProductList();
            
            
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
                    
                    for(int i=0; i<purchasableReceiptList.Count; i++) {
                        RequestPurchaseReward(purchasableReceiptList[i]); // 완료처리 되지 않은 결제 상품에 대한 처리 시작 
                    }                    
                    
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
        public void RequestPurchaseGamebase(string gamebaseProductId, string projectId = "") {
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
                    Debug.Log("###### Purchase succeeded.");
                    
                    /*
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
                    */
                    
                    RequestPurchaseReward(purchasableReceipt, projectId);
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
        void RequestPurchaseReward(GamebaseResponse.Purchase.PurchasableReceipt receipt, string projectId = "") {
            
            Debug.Log("##### RequestPurchaseReward ####");
            
            JsonData sendData = new JsonData();
            sendData["func"] = "purchaseInappProduct"; // userPurchase => purchaseInappProduct
            sendData["product_id"] = receipt.gamebaseProductId;
            sendData["receipt"] = receipt.paymentId;
            sendData["paymentSeq"] = receipt.paymentSeq;
            sendData["purchaseToken"] = receipt.purchaseToken;
            
            try {
                sendData["price"] = System.Math.Round(receipt.price, 2);
            }
            catch {
                sendData["price"] = receipt.price;
            }
            
            sendData["currency"] = receipt.currency;

            if (!string.IsNullOrEmpty(projectId))
                sendData[CommonConst.COL_PROJECT_ID] = projectId;
            
           
            
               
            // 이프유 패스에서는 다른 메소드 사용 
            if(receipt.gamebaseProductId == "ifyou_pass") {
                sendData["func"] = "purchaseInappProductByMail";
            }
            
            // 결제 실패 대비
            if(receipt.gamebaseProductId == "oneday_pass") {
                
                if(string.IsNullOrEmpty(projectId) || projectId == "-1") {
                    projectId = PlayerPrefs.GetString(CommonConst.KEY_LAST_ONEDAY_PASS_PROJECT, "-1");
                    sendData[CommonConst.COL_PROJECT_ID] = projectId;
                }
                else {               
                    PlayerPrefs.SetString(CommonConst.KEY_LAST_ONEDAY_PASS_PROJECT, projectId);
                    PlayerPrefs.Save();
                }
            }
            else if(receipt.gamebaseProductId.Contains("story_pack")) {
                
                if(string.IsNullOrEmpty(projectId) || projectId == "-1") {
                    projectId = PlayerPrefs.GetString(CommonConst.KEY_LAST_PREMIUM_PASS_PROJECT, "-1");
                    sendData[CommonConst.COL_PROJECT_ID] = projectId;
                }
                else {               
                    PlayerPrefs.SetString(CommonConst.KEY_LAST_PREMIUM_PASS_PROJECT, projectId);
                    PlayerPrefs.Save();
                }                
            }

            
            NetworkLoader.main.SendPost(OnRequestPurchaseReward, sendData, true);
            
            
            // 통합 인앱결제
            try {
            Dictionary<string, string> eventValues = new Dictionary<string, string>();
            eventValues.Add(AFInAppEvents.CURRENCY, receipt.currency);
            eventValues.Add(AFInAppEvents.REVENUE, receipt.price.ToString());
            eventValues.Add(AFInAppEvents.ORDER_ID, receipt.gamebaseProductId);
            eventValues.Add(AFInAppEvents.QUANTITY, "1");
            AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventValues);
            }
            catch {
                Debug.LogError("Eroor in AppsFlyerSDK");
            }
            
  
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
            
            Debug.Log("[OnRequestPurchaseReward]");
            Debug.Log(response.DataAsText);
            
            // 받은 데이터 처리
            // bank, userPurchaseHistory
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            // UserManager.main.SetNotificationInfo(result); // unreadMailCount 이제 메일로 보내지 않음. 
            
            // 히스토리 갱신 
            if(result.ContainsKey("userPurchaseHistory"))
                userPurchaseHistoryJSON = result["userPurchaseHistory"];
                
            // * 2022.05.24 allpass_expire_tick 올패스 만료시간 갱신 
            if(result.ContainsKey("allpass_expire_tick")) {
                UserManager.main.SetAllpassExpire(SystemManager.GetJsonNodeLong(result, "allpass_expire_tick"));
            }
            
            // * 2022.07.29 원데이 패스
            if(result.ContainsKey("oneday_pass_expire_tick") && result.ContainsKey("oneday_pass_expire")) {
                // 하나만 고치면 연결된 나머지도 갱신이 되는걸까? 
                SystemListener.main.introduceStory.SetOnedayPassTick(SystemManager.GetJsonNodeString(result, "oneday_pass_expire"),  SystemManager.GetJsonNodeLong(result, "oneday_pass_expire_tick"));
            }
            
            // 재화 바로 지급으로 변경됨(2022.06.20)
            UserManager.main.SetRefreshInfo(result);

            // Shop 리프레시 탭 3개와 상단...
            MainShop.OnRefreshNormalShop?.Invoke();
            MainShop.OnRefreshPackageShop?.Invoke();
            MainShop.OnRefreshEventShop?.Invoke();
            MainShop.OnRefreshTopShop?.Invoke();

            // 모든 활성 팝업 제거
            PopupManager.main.HideActivePopup();
            
            
            // 구매한 상품 ID
            string purchasedProductID = SystemManager.GetJsonNodeString(result, "product_id");

            StartCoroutine(DelayShowBillingCompletePopup(purchasedProductID));
        }
        
        IEnumerator DelayShowBillingCompletePopup(string __productID) {
            
            yield return null;
            yield return null;
            yield return null;
            
            yield return new WaitForSeconds(0.1f);
            
            // 구매 상품에 따라 각자 다른 메세지. 
            Debug.Log(">>>>>>>>>>>>>>> DelayShowBillingCompletePopup : " + __productID);
            
            if(__productID == "pre_reward_pack") {
                SystemManager.ShowSystemPopupLocalize("6300", null, null, true, false); // 사전예약보상
                yield break;
            }
            
            
            if(__productID.Contains("allpass_")) {
                SystemManager.ShowSystemPopupLocalize("6435", null, null, true, false); // 올패스 구매 완료
                
                // * 올패스 구매시에 화면 갱신처리. 
                // 화면 갱신
                if(GameManager.main != null) {
                    EpisodeEndControls.OnPassPurchase?.Invoke();
                }
                
                if(StoryLobbyManager.main != null) {
                    StoryLobbyMain.OnPassPurchase?.Invoke();
                }
                
                yield break;
            }

            // 원데이 패스 구매 완료
            if(__productID == "oneday_pass")
            {
                Debug.Log("Oneday purchased <<<<<< ");
                
                SystemManager.ShowSystemPopup(string.Format(SystemManager.GetLocalizedText("6444"), SystemListener.main.introduceStory.title), null, null, true, false);
                
                // 원데이 패스 refresh 처리 
                CallPassButtonsRefresh();
            }
            else if (__productID == "ifyou_pass") {
                
                Debug.Log("ifyou_pass purchased <<<<<< ");
                
                // 이프유 패스에 대한 메세지 
                UserManager.main.ifyouPassDay = 1; // 1일차 시작으로 한다.
                // 6441
                // 이프유 패스를 구매했어요!
                SystemManager.ShowSystemPopupLocalize("6441", null, null, true, false); 
                
                CallPassButtonsRefresh();
                
            }
            else if (__productID.Contains("story_pack")) {
                
                Debug.Log("Premiumpass purchased <<<<<< ");
                
                Debug.Log(string.Format("Introduce : [{0}], Current : [{1}]", SystemListener.main.introduceStory.projectID, StoryManager.main.CurrentProject.projectID));
                

                SystemManager.ShowSystemPopup(string.Format(SystemManager.GetLocalizedText("6445"), SystemListener.main.introduceStory.title), null, null, true, false);
                
                // 프리미엄 패스 보유중으로 변경 
                SystemListener.main.introduceStory.hasPremiumPass = true;
                
                CallPassButtonsRefresh();

            }
            else { // 일반 구매 
                SystemManager.ShowSystemPopupLocalize("6113", null, null, true, false);  // 일반구매     
            }

        }
        
        void CallPassButtonsRefresh() {
            
            Debug.Log("CallPassButtonsRefresh");
            
            if(GameManager.main != null) {
                EpisodeEndControls.OnPassPurchase?.Invoke();
            }
            
            if(StoryLobbyManager.main != null) {
                StoryLobbyMain.OnPassPurchase?.Invoke();
            }
            
            ViewIntroduce.OnPassPurchase?.Invoke();
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
            
            // 일반 내역만 받아서 쓴다. 
            if(userPurchaseHistoryJSON.ContainsKey("normal"))
                userPurchaseHistoryJSON = userPurchaseHistoryJSON["normal"];
            
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
            
            
            InitIfyouPass();
            InitOnedayPass();
        }
        
        /// <summary>
        /// 이프유 패스 정보 설정 
        /// </summary>
        void InitIfyouPass() {
            string productMasterId = string.Empty;
            for (int i = 0; i < productMasterJSON.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(productMasterJSON[i], "product_type") == "ifyou_pass")
                {
                    productMasterId = SystemManager.GetJsonNodeString(productMasterJSON[i], "product_master_id");
                    break;
                }
            }
            JsonData ifyouPassData = GetGameProductItemDetailInfo(productMasterId);
            
            if(ifyouPassData != null && ifyouPassData.Count > 0)
                ifyouPassData = ifyouPassData[0];
                
            if(ifyouPassData == null)
                return;
            
            // 이프유 패스 정보 설정 
            ifyouPassDirectStar = SystemManager.GetJsonNodeInt(ifyouPassData, "star_directly_count");
            ifyouPassDailyStar = SystemManager.GetJsonNodeInt(ifyouPassData, "star_daily_count");
            ifyouPassChoiceSale = SystemManager.GetJsonNodeInt(ifyouPassData, "selection_sale");
            ifyouPassChoiceSaleFloat = ifyouPassChoiceSale * 0.01f;
            
            
        }
        
        /// <summary>
        /// 원데이 패스 정보 설정
        /// </summary>
        void InitOnedayPass() {
            string productMasterId = string.Empty;
            for (int i = 0; i < productMasterJSON.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(productMasterJSON[i], "product_type") == "oneday_pass")
                {
                    productMasterId = SystemManager.GetJsonNodeString(productMasterJSON[i], "product_master_id");
                    break;
                }
            }
            JsonData onedayPassData = GetGameProductItemDetailInfo(productMasterId);
            
            if(onedayPassData != null && onedayPassData.Count > 0)
                onedayPassData = onedayPassData[0];
                
            if(onedayPassData == null)
                return;
            
            
            onedayPassChoiceSale = SystemManager.GetJsonNodeInt(onedayPassData, "selection_sale");
            onedayPassChoiceSaleFloat = onedayPassChoiceSale * 0.01f;
        }
        
        
        public void OnRequestCoinExchangeProductList(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            
            coinExchangeJSON = JsonMapper.ToObject(response.DataAsText); 
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
        /// <param name="__id">product_id 값</param>
        /// <returns></returns>
        public JsonData GetGameProductItemMasterInfo(string __id) {
            for(int i=0; i<productMasterJSON.Count;i++) {
                if(SystemManager.GetJsonNodeString(productMasterJSON[i], "product_id") == __id)
                    return productMasterJSON[i];
            }

            return null;
        }


        /// <summary>
        /// 게임 서버 상품 - 디테일정보 가져오기
        /// </summary>
        /// <param name="__productMasterID"></param>
        /// <returns></returns>
        public JsonData GetGameProductItemDetailInfo(string __productMasterID)
        {
            if (!productDetailJSON.ContainsKey(__productMasterID))
                return null;

            return productDetailJSON[__productMasterID];
        }


        /// <summary>
        /// 마스터ID에 해당하는 상품의 구매 횟수 가져오기 
        /// </summary>
        /// <param name="__masterID"></param>
        /// <returns></returns>
        public int CheckProductPurchaseCount(string __masterID) {
            
            int purchaseCount = 0; // 구매 카운트 
            
            if(userPurchaseHistoryJSON == null)
                return 0;
            
            for(int i=0; i<userPurchaseHistoryJSON.Count;i++) {
                if(SystemManager.GetJsonNodeString(userPurchaseHistoryJSON[i], "product_master_id") == __masterID)
                    purchaseCount++;
                    
            }
            
            return purchaseCount;
        }
        
         
        public JsonData GetCoinExchangeProductInfo(string __productID) {
            if(coinExchangeJSON == null)
                return null;
                
            for(int i=0; i<coinExchangeJSON.Count;i++) {
                if(SystemManager.GetJsonNodeString(coinExchangeJSON[i], "exchange_product_id") == __productID)
                    return coinExchangeJSON[i];
            }
            
            
            return null;
        }






        /// <summary>
        /// 환전 고고 
        /// </summary>
        /// <param name="exchangeProductID"></param>
        public void ExchangeStarToCoin(string exchangeProductID) {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "coinExchangePurchase";
            sending["exchange_product_id"] = exchangeProductID;
            
            NetworkLoader.main.SendPost(OnRequestCoinExchange, sending, true);
        }
        
        void OnRequestCoinExchange(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            JsonData result = JsonMapper.ToObject(response.DataAsText); 
            
            coinExchangeJSON = result["coinExchangeProduct"]; // 상품 리스트 갱신 
            
            UserManager.main.SetBankInfo(result); // 뱅크 갱신 
            
            MainShop.OnRefreshNormalShop?.Invoke();
            
            // 코인을 몇개 받았습니다.
            SystemManager.ShowMessageAlert(string.Format(SystemManager.GetLocalizedText("6121"), result["gotCoin"].ToString()));
        }
        
    } // ? end of class
}