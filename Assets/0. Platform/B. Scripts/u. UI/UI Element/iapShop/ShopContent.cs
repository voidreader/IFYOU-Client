using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {

    public class ShopContent : MonoBehaviour
    {
        public void OnClickAboutAllPass() {
            // 올패스 about 눌렀을때 팝업 처리 
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_ALL_PASS_GUIDE);
            PopupManager.main.ShowPopup(p, true);
            
        }
    }
}