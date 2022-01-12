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
        JsonData productMasterJSON; // 마스터 
        JsonData productDetailJSON; // 디테일
        
        [SerializeField] TextMeshProUGUI textPrice; // 가격 
        [SerializeField] string productMasterID = string.Empty; // 마스터 ID
        [SerializeField] bool hasPurchaseHistory = false; // 구매 기록 
        
        [SerializeField] string bannerURL = string.Empty; // 배너 URL
        [SerializeField] string bannerKey = string.Empty; // 배너 Key
        
        [SerializeField] string detailURL = string.Empty; // 디테일 이미지 
        [SerializeField] string detailKey = string.Empty; // 
        
        
        /// <summary>
        /// 패키지 초기화
        /// </summary>
        /// <param name="__productID"></param>        
        public void InitPackage(string __productID) {
            this.gameObject.SetActive(true);
            
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
            productMasterJSON = BillingManager.main.GetGameProductItemMasterInfo(productID);
            if(productMasterJSON != null) {
                productMasterID = productMasterJSON["product_master_id"].ToString(); // master_id
   
                hasPurchaseHistory = BillingManager.main.CheckProductPurchaseHistory(productID); // 구매 내역
                
                productDetailJSON = BillingManager.main.GetGameProductItemDetailInfo(productMasterID); // 디테일 
            }            
            
            
            if(productDetailJSON == null) {
                Debug.Log(string.Format("[{0}] has no info in game server ", productID ));
                return; 
            }
            
            
            
        }
    }
    
}

