using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    
    public class ViewShop : CommonView
    {
        // Start is called before the first frame update
        public override void OnView()
        {
            base.OnView();
        }
        
        public override void OnStartView() {
            Signal.Send(LobbyConst.STREAM_IFYOU, "activateShop", string.Empty);
        }
        
        
        public void OnClickCoinShop() {
            
            if(string.IsNullOrEmpty(SystemManager.main.coinShopURL)) {
                Debug.LogError("No Coinshop url");
                return;
            }
            
            
            
            
            
            string uidParam = string.Format("?uid={0}", UserManager.main.GetUserPinCode());
            string langParam = string.Format("&lang={0}", SystemManager.main.currentAppLanguageCode);
            
            string finalURL = SystemManager.main.coinPrizeURL + uidParam + langParam;
            Debug.Log("Coinshop : " + finalURL);
            
            var color = new Color(1, 0.83f, 0.83f);
            

            
        }
    }
}