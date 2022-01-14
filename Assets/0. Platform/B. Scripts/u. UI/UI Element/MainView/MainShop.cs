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
            packageToggle.SetIsOn(true);
        }
        
        /// <summary>
        /// 상점 상단 초기화 
        /// </summary>
        public void InitShopTop() {
            topSpecialProduct.InitPackage("starter_pack");
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
            
            if(string.IsNullOrEmpty(SystemManager.main.coinShopURL)) {
                Debug.LogError("No Coinshop url");
                return;
            }
            
            
            
            
            string uidParam = string.Format("?uid={0}", UserManager.main.GetUserPinCode());
            string langParam = string.Format("&lang={0}", SystemManager.main.currentAppLanguageCode);
            
            string finalURL = SystemManager.main.coinShopURL + uidParam + langParam;
            Debug.Log("Coinshop : " + finalURL);
            
            
            if(UniWebViewSafeBrowsing.IsSafeBrowsingSupported) {
                
                var safeBrowsing = UniWebViewSafeBrowsing.Create(finalURL);
                safeBrowsing.OnSafeBrowsingFinished += (browsing) => {
                Debug.Log("UniWebViewSafeBrowsing is closed.");
                NetworkLoader.main.RequestUserBaseProperty();
                
                Destroy(safeBrowsing);
                
                };
            
                safeBrowsing.Show();
            }
            else {
                Debug.Log(">>>>>> Now support SafeBrowsingSupported");
                
                GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration = new GamebaseRequest.Webview.GamebaseWebViewConfiguration();
                configuration.title = "";
                configuration.orientation = GamebaseScreenOrientation.PORTRAIT;
                // configuration.colorR = 98;
                // configuration.colorG = 132;
                // configuration.colorB = 207;
                // configuration.colorA = 255;
                configuration.barHeight = 30;
                configuration.isBackButtonVisible = false;
                // configuration.contentMode = GamebaseWebViewContentMode.MOBILE;

                
                Gamebase.Webview.ShowWebView(finalURL, configuration, (error) =>{ 
                    Debug.Log("Webview Closed");
                    NetworkLoader.main.RequestUserBaseProperty();
                }, null, null);
                
            }
            
            
            
            
            // SystemManager.main.ShowDefaultWebview(finalURL);
            
            //  var color = new Color(1, 0.83f, 0.83f);
            
            /*
            if(UniWebViewSafeBrowsing.IsSafeBrowsingSupported) {
                Debug.Log("Safe Browsing Support");
            
                var safeBrowsing = UniWebViewSafeBrowsing.Create(finalURL);
                
                // v.SetShowToolbar()
                // safeBrowsing.SetToolbarColor(color);
                // safeBrowsing.SetToolbarItemColor(Color.white);

            }
            else {
                Debug.Log("Safe Browsing not Support");
                // SystemManager.main.ShowDefaultWebview(finalURL);
                
            }
            */
            
            
        }
    }
}