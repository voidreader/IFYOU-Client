using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Toast.Gamebase;
using UnityEngine.UI;
using TMPro;

namespace PIERStory 
{
    public class GeneralPackProduct : MonoBehaviour
    {
        
        public string productID = string.Empty; // 마켓 등록 ID
        GamebaseResponse.Purchase.PurchasableItem gamebaseItem = null; // 게임베이스 기준정보 
        
        [SerializeField] ImageRequireDownload bannerImage; // 배너 이미지 
        [SerializeField] GameObject soldOut; // 판매완료 (한정품목)
        JsonData productMasterJSON; // 마스터 
        JsonData productDetailJSON; // 디테일
        
        [SerializeField] TextMeshProUGUI textPrice; // 가격 
        [SerializeField] string productMasterID = string.Empty; // 마스터 ID
        [SerializeField] bool hasPurchaseHistory = false; // 구매 기록 
        
        [SerializeField] string bannerURL = string.Empty; // 배너 URL
        [SerializeField] string bannerKey = string.Empty; // 배너 Key
        
        [SerializeField] string detailURL = string.Empty; // 디테일 이미지 
        [SerializeField] string detailKey = string.Empty; // 
        
        [SerializeField] int maxCount = 0; // 구매 가능 횟수
        [SerializeField] int currentPurchaseCount = 0; // 현재 패키지 구매 횟수
        
        
        /// <summary>
        /// 패키지 초기화
        /// </summary>
        /// <param name="__productID"></param>        
        public void InitPackage(string __productID, JsonData __productMasterJSON) {
            this.gameObject.SetActive(true);
            
            soldOut.SetActive(false);
            
            productID = __productID;
            gamebaseItem = BillingManager.main.GetGamebasePurchaseItem(productID);
            
            
            // 가격 정보 
            if(gamebaseItem != null) {
                Debug.Log("localizedPrice : " + gamebaseItem.localizedPrice);
                textPrice.text = gamebaseItem.localizedPrice;
            }
            else {
                Debug.Log(__productID + " is Null!!!!!");
            }
            
            // 게임 Product 정보 
            productMasterJSON = __productMasterJSON;
            
            if(productMasterJSON != null) {
                productMasterID = productMasterJSON["product_master_id"].ToString(); // master_id
   
                hasPurchaseHistory = BillingManager.main.CheckProductPurchaseHistory(productID); // 구매 내역
                
                productDetailJSON = BillingManager.main.GetGameProductItemDetailInfo(productMasterID); // 디테일 
            }            
            
            
            if(productDetailJSON == null) {
                Debug.Log(string.Format("[{0}] has no info in game server ", productID ));
                return; 
            }
            
            maxCount = SystemManager.GetJsonNodeInt(productMasterJSON, "max_count");
            
            if(maxCount > 0) {
                currentPurchaseCount = BillingManager.main.GetProductPurchaseCount(productID); // 구매 횟수 구한다.
                
                // 구매횟수가 maxCount 이상이면 솔드아웃 처리 
                if(maxCount <= currentPurchaseCount) {
                    soldOut.SetActive(true);
                }
            }
            
            
            
            
            bannerURL = SystemManager.GetJsonNodeString(productMasterJSON, "product_url");
            bannerKey = SystemManager.GetJsonNodeString(productMasterJSON, "product_key");
            
            detailURL = SystemManager.GetJsonNodeString(productMasterJSON, "product_detail_url");
            detailKey = SystemManager.GetJsonNodeString(productMasterJSON, "product_detail_key");
            
            
            // 배너 이미지 세팅 
            bannerImage.SetDownloadURL(bannerURL, bannerKey);
            
            // 디테일 다운로드 요청 
            SystemManager.RequestDownloadImage(detailURL, detailKey, null);
            
            
        }
        
        public void OnClickBanner() {
            
            if(productMasterJSON == null || productDetailJSON == null)
                return;
            
            PopupBase p = PopupManager.main.GetPopup("PackDetail");
            if (p == null)
            {
                Debug.LogError("No PackDetail popup");
                return;
            }
            
            // 데이터 세팅 
            p.Data.SetLabelsTexts(textPrice.text);
            p.Data.targetData = productID;
            p.Data.imageURL = detailURL;
            p.Data.imageKey = detailKey;
            
            // 열기
            PopupManager.main.ShowPopup(p, false);
            
            // BillingManager.main.RequestPurchaseGamebase(productID);
            
            AdManager.main.AnalyticsPackageButtonClick(productID);
        }
    }
    
}

