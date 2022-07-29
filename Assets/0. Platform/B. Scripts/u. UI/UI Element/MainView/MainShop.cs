using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory {

    public class MainShop : MonoBehaviour
    {
        public static bool isMainNavigationShop = false; // 메인에서 샵을 사용중인 경우 . 
        // refresh 용도의 action
        public static System.Action OnRefreshNormalShop = null; // 노멀 탭 리프레시
        public static System.Action OnRefreshPackageShop = null; // 패키지 탭 리프레시
        public static System.Action OnRefreshEventShop = null; // 이벤트 탭 리프레시
        public static System.Action OnRefreshTopShop = null; // 상점 탑 리프레이 
        
        public GameObject eventPackText;
        public bool isNormalContainerSet = false;
        
        public List<BaseStarProduct> listBaseStarProducts; // 일반 스타 상품 
        
        
        // 용도 나누자 
        
        [Header("이프유 패스 패키지")]
        public GeneralPackProduct ifyouPassPackage; // 이프유 패스 패키지 
        
        [Header("노멀탭 패키지")]
        public List<GeneralPackProduct> listNormalTabPackages; // 노멀탭의 패키지 상품 (상단 노출)
        
        
        [Header("패키지탭 패키지")]
        public List<GeneralPackProduct> listLimitPackProducts; // 패키지 탭의 기간한정 상품 
        public List<GeneralPackProduct> listGeneralPackProducts; // 일반 패키지 상품 
        
        public List<GeneralPackProduct> listSpecialEventPackProducts; // 스페셜 이벤트 패키지
        
        
        [Space]
        public List<BaseCoinExchangeProduct> listCoinExchangeProducts; // 코인 환전 상품
        
                
        [Space]
        [SerializeField] UIToggleGroup toggleGroup;
        [SerializeField] UIToggle packageToggle;
        [SerializeField] UIToggle normalToggle;
        [SerializeField] UIToggle eventToggle;
        public RectTransform shopTop;
        public List<RectTransform> productContainers;


        public void DelayEnterFromMain()
        {
            Debug.Log("## DelayEnterFromMain ##");
            
            StartCoroutine(RoutineEnterFromMain());
            OnRefreshNormalShop = InitNormalContainer;
            OnRefreshPackageShop = InitPackContainer;
            OnRefreshEventShop = InitEventContainer;
            OnRefreshTopShop = EnterFromMain;
        }
        
        void InitToggles() {
            eventToggle.SetIsOn(false, false);
            packageToggle.SetIsOn(false, false);
            normalToggle.SetIsOn(false, false);
        }

        /// <summary>
        /// 메인 하단 탭을 통해서 상점 접근
        /// </summary>
        IEnumerator RoutineEnterFromMain()
        {
            InitToggles();
            
            
            Firebase.Analytics.FirebaseAnalytics.LogEvent("store_open");
            
            if(CheckExistsEventTabProduct()) {
                eventToggle.gameObject.SetActive(true);
                toggleGroup.FirstToggle = eventToggle;
                toggleGroup.FirstToggle.SetIsOn(true, true);
                shopTop.sizeDelta = new Vector2(720, 170);

                foreach (RectTransform rt in productContainers)
                    rt.offsetMax = new Vector2(0, -170);

                yield return new WaitForSeconds(0.1f);
                Debug.Log("Exists Event Product");
                

                EnterFromMain();
                yield return new WaitForSeconds(0.2f);
                InitEventContainer();               
            }
            else {
                eventToggle.gameObject.SetActive(false);
                toggleGroup.FirstToggle = packageToggle;
                toggleGroup.FirstToggle.SetIsOn(true, true);
                shopTop.sizeDelta = new Vector2(720, 0);
                
                foreach (RectTransform rt in productContainers)
                    rt.offsetMax = new Vector2(0, -100);
                yield return new WaitForSeconds(0.1f);
                Debug.Log("NO Event Product");
                
                
                EnterFromMain();
                yield return new WaitForSeconds(0.2f);
                InitPackContainer();
      
            }    
        }

        public void DelayEnterFromSignal()
        {
            Debug.Log("## DelayEnterFromSignal ##");
            
            StartCoroutine(RoutineEnterFromSignal());
            OnRefreshNormalShop = InitNormalContainer;
            OnRefreshPackageShop = InitPackContainer;
            OnRefreshEventShop = InitEventContainer;
            OnRefreshTopShop = EnterFromSignal;
        }

        /// <summary>
        /// 메인 하단 탭이 아닌 상단바의 재화버튼 혹은 팝업을 통한 signal로 열리는 등의 상점 접근
        /// </summary>
        IEnumerator RoutineEnterFromSignal()
        {
            InitToggles();
            
            Firebase.Analytics.FirebaseAnalytics.LogEvent("store_open");
            
            if(CheckExistsEventTabProduct()) {
                eventToggle.gameObject.SetActive(true);
                toggleGroup.FirstToggle = eventToggle;
                toggleGroup.FirstToggle.SetIsOn(true, true);
                shopTop.sizeDelta = new Vector2(720, 170);

                foreach (RectTransform rt in productContainers)
                    rt.offsetMax = new Vector2(0, -170);

                yield return new WaitForSeconds(0.1f);
                Debug.Log("Exists Event Product");
                

                EnterFromSignal();
                yield return new WaitForSeconds(0.2f);
                InitEventContainer();               
            }
            else {
                eventToggle.gameObject.SetActive(false);
                toggleGroup.FirstToggle = packageToggle;
                toggleGroup.FirstToggle.SetIsOn(true, true);
                shopTop.sizeDelta = new Vector2(720, 0);

                foreach (RectTransform rt in productContainers)
                    rt.offsetMax = new Vector2(0, -100);

                yield return new WaitForSeconds(0.1f);
                Debug.Log("NO Event Product");
                
                
                EnterFromSignal();
                yield return new WaitForSeconds(0.2f);
                InitPackContainer();
      
            }
        }



        /// <summary>
        /// 메인 로비로부터 진입할 때 상단 제어
        /// </summary>
        void EnterFromMain()
        {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
            
            
            
        }


        /// <summary>
        /// 시그널로 호출되는 CommonView로 보여질 때 상단 제어
        /// </summary>
        void EnterFromSignal()
        {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5133"), string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);


            // 게임씬에서 진입했는데 그게 튜토리얼 중에 진입한거라면
            if(GameManager.main != null && (UserManager.main.tutorialStep == 1 && UserManager.main.tutorialClear) || (UserManager.main.tutorialStep == 2 && !UserManager.main.tutorialClear))
            {
                // 모든 팝업을 hide해준다
                foreach (PopupBase pb in PopupManager.main.ListShowingPopup)
                    pb.Hide();
            }
        }

        public void HideShop()
        {
            if (GameManager.main != null && GameManager.main.isPlaying) 
            {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);

                // 게임 튜토리얼 도중에 나온거니까 다시 팝업 On
                if (((UserManager.main.tutorialStep == 1 && UserManager.main.tutorialClear) || (UserManager.main.tutorialStep == 2 && !UserManager.main.tutorialClear)))
                {
                    PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_TUTORIAL_MISSION_2);
                    PopupManager.main.ShowPopup(p, false);
                }
            }
        }


        /// <summary>
        /// 일반 컨테이너 초기화
        /// </summary>
        public void InitNormalContainer()
        {
            Debug.Log(">> InitNormalContainer");


            if(!normalToggle.isOn)
                return;
            
            
            try {
            
                if(!this.gameObject.activeSelf)
                    return;
            }
            catch {
                return;
            }
            
            for(int i=0; i<listNormalTabPackages.Count;i++) {
                 listNormalTabPackages[i].gameObject.SetActive(false);
             }
             
            JsonData masterData;
            int packIndex = 0;
            string productID = string.Empty;
            string productMasterID = string.Empty;
            int maxCount = 0;
            
            
            for(int i=0; i<BillingManager.main.productMasterJSON.Count;i++) {
                
                masterData = BillingManager.main.productMasterJSON[i];
                productID =SystemManager.GetJsonNodeString(masterData, "product_id");  // ID 
                productMasterID =SystemManager.GetJsonNodeString(masterData, "product_master_id");  // ID 
                
                maxCount = SystemManager.GetJsonNodeInt(masterData, "max_count"); // 구매 제한 횟수 
                
                // 이벤트 상품 
                if(SystemManager.GetJsonNodeString(masterData, "product_id").Contains("general_pack")) {            
                    
                    if(packIndex >= listNormalTabPackages.Count)
                        break;
   
                    // 구매된 상품은 제거한다.
                    if(maxCount > 0 && BillingManager.main.CheckProductPurchaseCount(productMasterID) >= maxCount) {
                        continue;
                    }
                    
                    listNormalTabPackages[packIndex++].InitPackage(SystemManager.GetJsonNodeString(masterData, "product_id"), masterData);
   
                }
                
            } // ? end of for                     
             
             /*
             if(isNormalContainerSet)
                return;
            */
            // * container 콜백에서 실행된다. 
            // 기본 스타 상품
            for (int i = 0; i < listBaseStarProducts.Count; i++)
                listBaseStarProducts[i].InitProduct();

            // 코인 환전
            for (int i = 0; i < listCoinExchangeProducts.Count; i++)
                listCoinExchangeProducts[i].InitExchangeProduct();
            
            isNormalContainerSet = true;
        }
        
        /// <summary>
        /// 패키지 컨테이너 초기화 
        /// </summary>
         public void InitPackContainer() {

            if(!packageToggle.isOn)
                return;
                         
             Debug.Log("## InitPackContainer");
             
             try {
                if(!this.gameObject.activeSelf)
                    return;
             }
             catch {
                 
                 // NetworkLoader.main.ReportRequestError("InitPackContainer #1", "InitPackContainer #1");
                 return;
             }
             
             
            if(UserManager.main == null || !UserManager.main.completeReadUserData)
                return;
             
            if(BillingManager.main == null || BillingManager.main.productMasterJSON == null) {
                Debug.Log("## BillingManager is not inited");
                return;
            }
            
             
             // 기간한정 팩
             for(int i=0; i<listLimitPackProducts.Count;i++) {
                 listLimitPackProducts[i].gameObject.SetActive(false);
             }
             
             // 일반 팩 
             for (int i=0; i<listGeneralPackProducts.Count;i++) {
                 listGeneralPackProducts[i].gameObject.SetActive(false);
             }
             
             

            int packIndex = 0;
            int eventPackIndex = 0;
            JsonData masterData;
            bool hasEventPack = false;
            
            string productID = string.Empty;
            string productMasterID = string.Empty;
            int maxCount = 0;
            
            string productType = string.Empty; // 제품 타입 
            

            
            
            // 패키지 초기화 
            try {
                Debug.Log(string.Format("### productMasterJSON : [{0}]", BillingManager.main.productMasterJSON.Count));
                
                for(int i=0; i<BillingManager.main.productMasterJSON.Count;i++) {
                    
                    masterData = BillingManager.main.productMasterJSON[i];
                    productID =SystemManager.GetJsonNodeString(masterData, "product_id");  // ID 
                    productMasterID = SystemManager.GetJsonNodeString(masterData, "product_master_id"); // masterID 
                    maxCount = SystemManager.GetJsonNodeInt(masterData, "max_count"); // 구매 제한 횟수 
                    productType = SystemManager.GetJsonNodeString(masterData, "product_type"); // 제품 타입 
                    
                    // 사전 예약 패키지 제거 
                    if(productID.Contains("pre_reward_pack"))
                        continue;
                        
                    // 이프유 패스 패키지 처리 
                    if(productID.Contains("ifyou_pass")) {
                        ifyouPassPackage.InitPackage(productID, masterData);
                        continue;
                    }
                    
                    // 기간한정과 일반 상품으로 분리시킨다. 
                    if(productType == "limited") {
                        if(eventPackIndex >= listLimitPackProducts.Count)
                            break;
                            
                            
                        // 구매된 상품은 제거한다.
                        if(maxCount > 0 && BillingManager.main.CheckProductPurchaseCount(productMasterID) >= maxCount) {
                            continue;
                        }
                        
                        listLimitPackProducts[eventPackIndex++].InitPackage(SystemManager.GetJsonNodeString(masterData, "product_id"), masterData);
                        hasEventPack = true;                        
                    }
                    else if (productType == "base") {
                        if(packIndex >= listGeneralPackProducts.Count)
                            break; 
                            
                        if(!productID.Contains("pack"))
                            continue;
                            
                        // 구매된 상품은 제거한다.
                        if(maxCount > 0 && BillingManager.main.CheckProductPurchaseCount(productMasterID) >= maxCount) {
                            continue;
                        }
                        
                        listGeneralPackProducts[packIndex++].InitPackage(SystemManager.GetJsonNodeString(masterData, "product_id"), masterData);                        
                    }
                   
                } // ? end of for 
            }
            catch {
                NetworkLoader.main.ReportRequestError("InitPackContainer #3", "InitPackContainer #3");
                return;
            }
            
            eventPackText.SetActive(hasEventPack);
            
         }
         
         
        
        
        /// <summary>
        /// 이벤트 컨테이너 
        /// </summary>
        public void InitEventContainer() {
            
            if(!eventToggle.isOn)
                return;
            
            try {
                if(!this.gameObject.activeSelf)
                    return;
            }
            catch {
                return;
            }
            
            if(UserManager.main == null || !UserManager.main.completeReadUserData)
                return;
             
            if(BillingManager.main == null || BillingManager.main.productMasterJSON == null) {
                return;
            }
            
            // 상품 비활성화에서 시작 
            for(int i=0; i<listSpecialEventPackProducts.Count;i++) {
                listSpecialEventPackProducts[i].gameObject.SetActive(false);
            }
            
            int packIndex = 0;
            JsonData masterData;
            
            string productID = string.Empty;
            string productType = string.Empty;
            int maxCount = 0;
            
            // 패키지 초기화 
            try {
                
                for(int i=0; i<BillingManager.main.productMasterJSON.Count;i++) {
                    
                    masterData = BillingManager.main.productMasterJSON[i];
                    productID =SystemManager.GetJsonNodeString(masterData, "product_id");  // ID 
                    maxCount = SystemManager.GetJsonNodeInt(masterData, "max_count"); // 구매 제한 횟수 
                    productType = SystemManager.GetJsonNodeString(masterData, "product_type"); // 제품 타입 
                    
                    // 올패스와 event 타입만 걸러낸다. 
                    if(productType != "allpass" && productType != "event")
                        continue;
                    
                    
                    // 구매 상품을 화면에서 제거하지 않는다. 
                    listSpecialEventPackProducts[packIndex++].InitPackage(SystemManager.GetJsonNodeString(masterData, "product_id"), masterData);
                    
                    if(packIndex >= listSpecialEventPackProducts.Count)
                        break;
                } // ? end of for 
            }
            catch {
                NetworkLoader.main.ReportRequestError("InitPackContainer #3", "InitPackContainer #3");
                return;
            }            
            
        }
        
        
        public void OnClickCoinShop() {
            
            SystemManager.main.OpenCoinShopWebview();
            
            AdManager.main.AnalyticsCoinShopOpen("shop");

        }
        
        
        /// <summary>
        /// 이벤트탭에 들어갈 상품이 있는지 체크 
        /// </summary>
        /// <returns></returns>
        public bool CheckExistsEventTabProduct() {
            JsonData masterData;
            
            // * 
            for(int i =0; i<BillingManager.main.productMasterJSON.Count;i++) {
                masterData = BillingManager.main.productMasterJSON[i];
                
                // 있음.
                if(SystemManager.GetJsonNodeString(masterData, "product_type") == "allpass"
                    || SystemManager.GetJsonNodeString(masterData, "product_type") == "event") {
                    return true;
                }
            }
            
            return false;
            
        }
    }
}