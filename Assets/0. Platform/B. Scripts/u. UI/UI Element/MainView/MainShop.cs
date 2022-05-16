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
        public static System.Action OnRefreshTopShop = null; // 상점 탑 리프레이 
        
        public GameObject eventPackText;
        public bool isNormalContainerSet = false;
        
        public List<BaseStarProduct> listBaseStarProducts; // 일반 스타 상품 
        
        
        // 용도 나누자 
        [Header("노멀탭 패키지")]
        public List<GeneralPackProduct> listNormalTabPackages; // 노멀탭의 패키지 상품 (상단 노출)
        
        
        [Header("패키지탭 패키지")]
        public List<GeneralPackProduct> listEventPackProducts; // 이벤트 패키지 상품 
        public List<GeneralPackProduct> listGeneralPackProducts; // 일반 패키지 상품 
        
        [Space]
        public List<BaseCoinExchangeProduct> listCoinExchangeProducts; // 코인 환전 상품
        
        [Space]
        public List<ShopPassTimeDeal> listPassTimeDeal; // 패스 타임딜 최대 6개
        public GameObject passTimeDealTitle; // 타임딜 타이틀 
        public RectTransform passTimeDealGrid; // 타임딜 그리드 
        
        [Space]
        [SerializeField] UIToggle packageToggle;
        [SerializeField] UIToggle normalToggle;


        public void DelayEnterFromMain()
        {
            
            Debug.Log("## DelayEnterFromMain ##");
            
            StartCoroutine(RoutineEnterFromMain());
            OnRefreshNormalShop = InitNormalContainer;
            OnRefreshPackageShop = InitPackContainer;
            OnRefreshTopShop = EnterFromMain;
        }

        /// <summary>
        /// 메인 하단 탭을 통해서 상점 접근
        /// </summary>
        IEnumerator RoutineEnterFromMain()
        {
            yield return new WaitForSeconds(0.1f);
            EnterFromMain();
            yield return new WaitForSeconds(0.2f);
            InitPackContainer();
            
            // packageToggle.SetIsOn(true, true);
        }

        public void DelayEnterFromSignal()
        {
            
            Debug.Log("## DelayEnterFromSignal ##");
            
            StartCoroutine(RoutineEnterFromSignal());
            OnRefreshNormalShop = InitNormalContainer;
            OnRefreshPackageShop = InitPackContainer;
            OnRefreshTopShop = EnterFromSignal;
        }

        /// <summary>
        /// 메인 하단 탭이 아닌 상단바의 재화버튼 혹은 팝업을 통한 signal로 열리는 등의 상점 접근
        /// </summary>
        IEnumerator RoutineEnterFromSignal()
        {
            yield return new WaitForSeconds(0.1f);
            EnterFromSignal();
            yield return new WaitForSeconds(0.2f);
            InitPackContainer();
            
            // packageToggle.SetIsOn(true, true);
        }

        /*
        public void DelayInitShop() {
            StartCoroutine(RoutineInitShop());
        }
        
        IEnumerator RoutineInitShop() {
            
            Debug.Log("## RoutineInitShop ##");
            
            yield return new WaitForSeconds(0.1f);
            InitShopTop();
            yield return new WaitForSeconds(0.2f);
            InitPackContainer();
        }
        
        /// <summary>
        /// 상점 상단 초기화 
        /// </summary>
        public void InitShopTop() {
            
            Debug.Log("### InitShopTop() ###");
            

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);

            // packageToggle.SetIsOn(true);
            // InitPackContainer();
        }
        */

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
            int maxCount = 0;
            
            
            for(int i=0; i<BillingManager.main.productMasterJSON.Count;i++) {
                
                masterData = BillingManager.main.productMasterJSON[i];
                productID =SystemManager.GetJsonNodeString(masterData, "product_id");  // ID 
                maxCount = SystemManager.GetJsonNodeInt(masterData, "max_count"); // 구매 제한 횟수 
                
                // 이벤트 상품 
                if(SystemManager.GetJsonNodeString(masterData, "product_id").Contains("general_pack")) {            
                    
                    if(packIndex >= listNormalTabPackages.Count)
                        break;
   
                    // 구매된 상품은 제거한다.
                    if(maxCount > 0 && BillingManager.main.GetProductPurchaseCount(productID) >= maxCount) {
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
             
             Debug.Log("## InitPackContainer");
             
             try {
                if(!this.gameObject.activeSelf)
                    return;
             }
             catch {
                 
                 NetworkLoader.main.ReportRequestError("InitPackContainer #1", "InitPackContainer #1");
                 return;
             }
             
             
            if(UserManager.main == null || !UserManager.main.completeReadUserData)
                return;
             
            if(BillingManager.main == null || BillingManager.main.productMasterJSON == null) {
                Debug.Log("## BillingManager is not inited");
                return;
            }
             
             // 이벤트 팩
             for(int i=0; i<listEventPackProducts.Count;i++) {
                 listEventPackProducts[i].gameObject.SetActive(false);
             }
             
             // 일반 팩 
             for (int i=0; i<listGeneralPackProducts.Count;i++) {
                 listGeneralPackProducts[i].gameObject.SetActive(false);
             }
             
             // 타임딜 
             passTimeDealTitle.SetActive(false);
             passTimeDealGrid.gameObject.SetActive(false);
             for(int i=0; i<listPassTimeDeal.Count;i++) {
                 listPassTimeDeal[i].gameObject.SetActive(false);
             }
             
            int packIndex = 0;
            int eventPackIndex = 0;
            JsonData masterData;
            bool hasEventPack = false;
            
            string productID = string.Empty;
            int maxCount = 0;
            int activePassTimeDealCount = 0; // 활성화 타임딜 카운트 
            
            
            // 타임딜 설정
            try {
                
                if(UserManager.main.userActiveTimeDeal != null) {
                    Debug.Log(string.Format("### userActiveTimeDeal : [{0}]", UserManager.main.userActiveTimeDeal.Count));
                    for(int i=0; i<UserManager.main.userActiveTimeDeal.Count; i++) {
                        
                        // 최대 6개
                        if(i > 5)
                            break;
                        
                        listPassTimeDeal[i].Init(new PassTimeDealData(UserManager.main.userActiveTimeDeal[i]));
                        
                        if(listPassTimeDeal[i].gameObject.activeSelf)
                            activePassTimeDealCount++;
                    }
                    
                    // 활성 타임딜에 따라서 그리드 높이 조정
                    if(activePassTimeDealCount > 0) {
                        passTimeDealTitle.SetActive(true); // 타이틀 보여주고 
                        passTimeDealGrid.gameObject.SetActive(true);
                        
                        if(activePassTimeDealCount <= 2)
                            passTimeDealGrid.sizeDelta = new Vector2(660, 300);
                        else if(activePassTimeDealCount > 2 && activePassTimeDealCount <= 4) 
                            passTimeDealGrid.sizeDelta = new Vector2(660, 600);
                        else
                            passTimeDealGrid.sizeDelta = new Vector2(660, 930);
                        
                    }
                    // ? 타임딜 설정 종료                     
                }
                else {
                    Debug.Log("### No Active Time Deal");
                }
            }
            catch {
                NetworkLoader.main.ReportRequestError("InitPackContainer #2", "InitPackContainer #2");
                return;
            }
            
            
            // 패키지 초기화 
            try {
                Debug.Log(string.Format("### productMasterJSON : [{0}]", BillingManager.main.productMasterJSON.Count));
                for(int i=0; i<BillingManager.main.productMasterJSON.Count;i++) {
                    
                    masterData = BillingManager.main.productMasterJSON[i];
                    productID =SystemManager.GetJsonNodeString(masterData, "product_id");  // ID 
                    maxCount = SystemManager.GetJsonNodeInt(masterData, "max_count"); // 구매 제한 횟수 
                    // Debug.Log(SystemManager.GetJsonNodeString(masterData, "product_id"));
                    
                    // 이벤트 상품. 이름을 꼭 조건으로 걸어야한다. 
                    if(SystemManager.GetJsonNodeString(masterData, "product_id").Contains("story_pack") 
                        && SystemManager.GetJsonNodeBool(masterData, "is_event")) {
                            
                        if(eventPackIndex >= listEventPackProducts.Count)
                            break;
                            
                        // 구매된 상품은 제거한다.
                        if(maxCount > 0 && BillingManager.main.GetProductPurchaseCount(productID) >= maxCount) {
                            continue;
                        }
                            
                        
                        listEventPackProducts[eventPackIndex++].InitPackage(SystemManager.GetJsonNodeString(masterData, "product_id"), masterData);
                        hasEventPack = true;
    
                    }
                    else if(SystemManager.GetJsonNodeString(masterData, "product_id").Contains("pack") 
                            && !SystemManager.GetJsonNodeBool(masterData, "is_event")) {
                        
                        if(productID.Contains("pre_reward_pack"))
                            continue;
                        
                        if(packIndex >= listGeneralPackProducts.Count)
                            break; 
                            
                        // 구매된 상품은 제거한다.
                        if(maxCount > 0 && BillingManager.main.GetProductPurchaseCount(productID) >= maxCount) {
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
        
        
        public void OnClickCoinShop() {
            
            SystemManager.main.OpenCoinShopWebview();
            
            AdManager.main.AnalyticsCoinShopOpen("shop");

        }
    }
}