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
        
        
        
        [SerializeField] UIToggle packageToggle;
        [SerializeField] UIToggle normalToggle;


        void Start()
        {
            OnRefreshNormalShop = InitNormalContainer;
            OnRefreshPackageShop = InitPackContainer;
        }

        public void DelayEnterFromMain()
        {
            StartCoroutine(RoutineEnterFromMain());
            OnRefreshTopShop = EnterFromMain;
        }

        IEnumerator RoutineEnterFromMain()
        {
            yield return new WaitForSeconds(0.1f);
            EnterFromMain();
            yield return new WaitForSeconds(0.2f);
            InitPackContainer();
        }

        public void DelayEnterFromSignal()
        {
            StartCoroutine(RoutineEnterFromSignal());
            OnRefreshTopShop = EnterFromSignal;
        }

        IEnumerator RoutineEnterFromSignal()
        {
            yield return new WaitForSeconds(0.1f);
            EnterFromSignal();
            yield return new WaitForSeconds(0.2f);
            InitPackContainer();
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
                    
                    listNormalTabPackages[packIndex++].InitPackage(SystemManager.GetJsonNodeString(masterData, "product_id"));
   
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
             
             
             
             
             // 이벤트 팩
             for(int i=0; i<listEventPackProducts.Count;i++) {
                 listEventPackProducts[i].gameObject.SetActive(false);
             }
             
             // 일반 팩 
             for (int i=0; i<listGeneralPackProducts.Count;i++) {
                 listGeneralPackProducts[i].gameObject.SetActive(false);
             }
            
            
            if(BillingManager.main == null || BillingManager.main.productMasterJSON == null) {
                Debug.Log("## BillingManager is not inited");
                return;
            }
             
            int packIndex = 0;
            int eventPackIndex = 0;
            JsonData masterData;
            bool hasEventPack = false;
            
            string productID = string.Empty;
            int maxCount = 0;
            
            Debug.Log("## InitPackContainer ###2");
            
            // 패키지 초기화 
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
                        
                    
                    listEventPackProducts[eventPackIndex++].InitPackage(SystemManager.GetJsonNodeString(masterData, "product_id"));
                    hasEventPack = true;
   
                }
                else if(SystemManager.GetJsonNodeString(masterData, "product_id").Contains("pack") 
                        && !SystemManager.GetJsonNodeBool(masterData, "is_event")) {
                    
                    if(packIndex >= listGeneralPackProducts.Count)
                        break; 
                        
                    // 구매된 상품은 제거한다.
                    if(maxCount > 0 && BillingManager.main.GetProductPurchaseCount(productID) >= maxCount) {
                        continue;
                    }
                    
                    listGeneralPackProducts[packIndex++].InitPackage(SystemManager.GetJsonNodeString(masterData, "product_id"));
                }
            } // ? end of for 
            
            eventPackText.SetActive(hasEventPack);
            
         }
        
        
        public void OnClickCoinShop() {
            
            SystemManager.main.OpenCoinShopWebview();
            
            AdManager.main.AnalyticsCoinShopOpen("shop");

        }
    }
}