using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PIERStory {

    public class PopupPackDetail : PopupBase
    {
        
        [SerializeField] ImageRequireDownload detailImage;
        [SerializeField] string productID = string.Empty;
   
        
        public override void Show() {
            if(isShow)
                return;
            
            base.Show();
            
            productID = Data.targetData; // 제품 ID
            detailImage.SetDownloadURL(Data.imageURL, Data.imageKey); // 이미지 다운로드
        }
        
        public void OnClickBanner() {
            BillingManager.main.RequestPurchaseGamebase(productID);
        }
        
        
        
    }
}