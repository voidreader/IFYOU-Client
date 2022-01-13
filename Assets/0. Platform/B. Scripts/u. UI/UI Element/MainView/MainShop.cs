using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory {

    public class MainShop : MonoBehaviour
    {
        // refresh 용도의 action
        public static System.Action OnRefreshNormalShop = null;
        public static System.Action OnRefreshPackageShop = null;
        public List<BaseStarProduct> listBaseStarProducts; // 일반 스타 상품 
        public List<GeneralPackProduct> listGeneralPackProducts; // 일반 패키지 상품 
        
        [SerializeField] UIToggle packageToggle;
        [SerializeField] UIToggle normalToggle;
        
        
        
        void Start() {
            OnRefreshNormalShop = InitNormalContainer;
            OnRefreshPackageShop = InitPackContainer;
                 
        }
        
        public void OnCompleteShowAnimation() {
            packageToggle.SetIsOn(true);
        }
        
        
        /// <summary>
        /// 일반 컨테이너 초기화
        /// </summary>
        public void InitNormalContainer() {
            
            // * container 콜백에서 실행된다. 
            
            for(int i=0; i<listBaseStarProducts.Count;i++) {
                listBaseStarProducts[i].InitProduct();
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
            
            /*
            if(UniWebViewSafeBrowsing.IsSafeBrowsingSupported) {
            }
            */
            
            var safeBrowsing = UniWebViewSafeBrowsing.Create(finalURL);
            safeBrowsing.OnSafeBrowsingFinished += (browsing) => {
                Debug.Log("UniWebViewSafeBrowsing is closed.");
                NetworkLoader.main.RequestUserBaseProperty();
            };
            safeBrowsing.Show();
            
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