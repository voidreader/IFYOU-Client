using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Components;
using Toast.Gamebase;

namespace PIERStory {

    public class MainShop : MonoBehaviour
    {
        // refresh 용도의 action
        public static System.Action OnRefreshNormalShop = null; // 노멀 탭 리프레시
        public static System.Action OnRefreshPackageShop = null; // 패키지 탭 리프레시
        public static System.Action OnRefreshTopShop = null; // 상점 탑 리프레이 
        
        public List<BaseStarProduct> listBaseStarProducts; // 일반 스타 상품 
        public List<GeneralPackProduct> listGeneralPackProducts; // 일반 패키지 상품 
        
        public List<BaseCoinExchangeProduct> listCoinExchangeProducts; // 코인 환전 상품
        
        public GeneralPackProduct topSpecialProduct; // 상단 패키지 상품 나중에 롤링으로 변경하자..
        
        [SerializeField] UIToggle packageToggle;
        [SerializeField] UIToggle normalToggle;
        
        
        
        void Start() {
            OnRefreshNormalShop = InitNormalContainer;
            OnRefreshPackageShop = InitPackContainer;
            OnRefreshTopShop = InitShopTop;
                 
        }
        
        public void OnCompleteShowAnimation() {
            // packageToggle.SetIsOn(true);
        }
        
        public void DelayInitShop() {
            StartCoroutine(RoutineInitShop());
        }
        
        IEnumerator RoutineInitShop() {
            yield return new WaitForSeconds(0.1f);
            InitShopTop();
        }
        
        /// <summary>
        /// 상점 상단 초기화 
        /// </summary>
        public void InitShopTop() {
            topSpecialProduct.InitPackage("starter_pack");
            packageToggle.SetIsOn(true);
            InitPackContainer();
        }
        
        /// <summary>
        /// 일반 컨테이너 초기화
        /// </summary>
        public void InitNormalContainer() {
            
            // * container 콜백에서 실행된다. 
            
            // 기본 스타 상품
            for(int i=0; i<listBaseStarProducts.Count;i++) {
                listBaseStarProducts[i].InitProduct();
            }
            
            
            // 코인 환전
            for(int i=0; i<listCoinExchangeProducts.Count;i++) {
                listCoinExchangeProducts[i].InitExchangeProduct();   
            }
            
        }
        
        /// <summary>
        /// 패키지 컨테이너 초기화 
        /// </summary>
         public void InitPackContainer() {
             
             for (int i=0; i<listGeneralPackProducts.Count;i++) {
                 listGeneralPackProducts[i].gameObject.SetActive(false);
             }
            
            
            if(BillingManager.main == null || BillingManager.main.productMasterJSON == null)
                return;
             
            int packIndex = 0;
             
            for(int i=0; i<BillingManager.main.productMasterJSON.Count;i++) {
                
                // general_pack 만 해당한다. 
                if(SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_id").Contains("general_pack")) {
                    
                    // index 체크
                    if(packIndex >= listGeneralPackProducts.Count)    
                        return;
                    
                    listGeneralPackProducts[packIndex++].InitPackage(SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_id"));
                    
                }
            }
         }
        
        
        public void OnClickCoinShop() {
            
            SystemManager.main.OpenCoinShopWebview();
            
            AdManager.main.AnalyticsCoinShopOpen("shop");

        }
    }
}