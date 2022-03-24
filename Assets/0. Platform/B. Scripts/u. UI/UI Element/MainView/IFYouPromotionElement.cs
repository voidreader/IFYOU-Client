using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace PIERStory {

    /// <summary>
    /// 신규 이프유 프로모션 
    /// </summary>
    public class IFYouPromotionElement : MonoBehaviour
    {
        
        public ImageRequireDownload promotionBanner;
        
        [SerializeField] string bannerURL = string.Empty;
        [SerializeField] string bannerKey = string.Empty;
        
        JsonData detail;
        
        public void SetPromotion(JsonData __detail) {
            detail = __detail;
            
            for (int i = 0; i < detail.Count; i++)
            {
                // 해당 국가 코드에 맞는 promotion banner 이미지를 세팅한다
                if (SystemManager.GetJsonNodeString(detail[i], LobbyConst.COL_LANG) == SystemManager.main.currentAppLanguageCode.ToUpper())
                {
                    bannerURL = SystemManager.GetJsonNodeString(detail[i], LobbyConst.NODE_PROMOTION_BANNER_URL);
                    bannerKey = SystemManager.GetJsonNodeString(detail[i], LobbyConst.NODE_PROMOTION_BANNER_KEY);
                    
                    promotionBanner.SetDownloadURL(bannerURL, bannerKey, true);
                    break;
                }
                
            }            
            
        }
        
        public void OnClickPromotion() {
            
        }
    }
}