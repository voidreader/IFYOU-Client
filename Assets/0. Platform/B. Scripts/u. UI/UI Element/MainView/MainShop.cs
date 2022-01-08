using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {

    public class MainShop : MonoBehaviour
    {
        public void OnClickCoinShop() {
            
            if(string.IsNullOrEmpty(SystemManager.main.coinShopURL)) {
                Debug.LogError("No Coinshop url");
                return;
            }
            
            
            
            
            string uidParam = string.Format("?uid={0}", UserManager.main.GetUserPinCode());
            string langParam = string.Format("&lang={0}", SystemManager.main.currentAppLanguageCode);
            
            string finalURL = SystemManager.main.coinShopURL + uidParam + langParam;
            Debug.Log("Coinshop : " + finalURL);
            
            SystemManager.main.ShowDefaultWebview(finalURL);
            
            //  var color = new Color(1, 0.83f, 0.83f);
            
            /*
            if(UniWebViewSafeBrowsing.IsSafeBrowsingSupported) {
                Debug.Log("Safe Browsing Support");
            
                var safeBrowsing = UniWebViewSafeBrowsing.Create(finalURL);
                
                // v.SetShowToolbar()
                // safeBrowsing.SetToolbarColor(color);
                // safeBrowsing.SetToolbarItemColor(Color.white);
                safeBrowsing.OnSafeBrowsingFinished += (browsing) => {
                    Debug.Log("UniWebViewSafeBrowsing is closed.");
                };
                safeBrowsing.Show();
            }
            else {
                Debug.Log("Safe Browsing not Support");
                // SystemManager.main.ShowDefaultWebview(finalURL);
                
            }
            */
            
            
        }
    }
}