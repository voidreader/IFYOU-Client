using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;


namespace PIERStory {
    
    
    
    /// <summary>
    /// 상점에서 사용하는 프리미엄 패스 타임딜. 
    /// </summary>
    public class ShopPassTimeDeal : MonoBehaviour
    {
        public StoryData passStory = null; // 대상 스토리
        
        public ImageRequireDownload imagePass;
        [SerializeField] TextMeshProUGUI textTimer; // 타이머 
        [SerializeField] TextMeshProUGUI textDiscount; // 할인율 
        [SerializeField] DateTime endDate;
        [SerializeField] TimeSpan timeDifference; // 타임딜 종료와의 시간차
        
        public int timeDealID = 0;
        [SerializeField] long end_date_tick = 0; // 서버에서 받아오는 타임딜 종료시간 tick
        [SerializeField] bool isCountable = false; // 타이머 카운팅이 가능한지 
        [SerializeField] float discountFloat = 0; // 할인율 
        [SerializeField] int discountInt = 0;
        
        
        [SerializeField] TextMeshProUGUI textOriginPrice; // 원 가격 
        [SerializeField] TextMeshProUGUI textSalePrice; // 할인 가격
        
        public int originFreepassPrice = 0;
        public int saleFreepassPrice = 0;
        
        
        public PassTimeDealData passTimeDeal;        
        
        void Update() {
           if(!isCountable)
                return;
                
            // 5 프레임마다 갱신해주자. 
            if(Time.frameCount % 5 == 0)
                textTimer.text = GetDiffTime();            
        }
        
        /// <summary>
        /// 초기화 
        /// </summary>
        /// <param name="__passTimeDeal"></param>
        public void Init(PassTimeDealData __passTimeDeal) {
            passTimeDeal = __passTimeDeal;
            
            // 유효성 검증 
            if(passTimeDeal == null || !passTimeDeal.isValidData) {
                Debug.LogError("Wrong pass time deal data");
                return;
            }
           
            passStory = StoryManager.main.FindProject(passTimeDeal.projectID);
            
            if(passStory == null || !passStory.isValidData) {
                Debug.LogError("Wrong passStory data");
                return;
            }
            
            // 프리미엄 패스 소유권 체크 
            if(UserManager.main.HasProjectFreepass(passStory.projectID))
                return;
            
                
            
            // 세팅 시작
            timeDealID = passTimeDeal.timedealID;
            
            // 할인율 처리 (작품진행도에 따른 할인율 + 타임딜 할인율)
            discountInt = passTimeDeal.discountINT;
            discountFloat = (float)discountInt * 0.01f;
            discountFloat += passStory.passDiscount; // 원래 작품의 진행도에 따른 할인율을 더해준다. 
            discountInt += (int)(passStory.passDiscount * 100);
            textDiscount.text = discountInt.ToString() + "%";
            
            // 가격 설정
            originFreepassPrice = passStory.passPrice;
            textOriginPrice.text = originFreepassPrice.ToString();
            
            // 할인 가격 
            saleFreepassPrice = SystemConst.GetSalePrice(originFreepassPrice, discountFloat);
            textSalePrice.text = saleFreepassPrice.ToString();
            
            // 시간 준비 
            end_date_tick = SystemConst.ConvertServerTimeTick(passTimeDeal.expireTick);
            endDate = new DateTime(end_date_tick); // 틱으로 생성
            timeDifference = endDate - System.DateTime.UtcNow;
            
            // 타임딜 오버되면 종료. 
            if(timeDifference.Ticks <= 0)
                return;
                
            textTimer.text = GetDiffTime(); // 최초값 넣어준다. 
            isCountable = true;
            
            
            this.gameObject.SetActive(true);
            
            // 이미지 
            imagePass.SetDownloadURL(passStory.premiumPassURL, passStory.premiumPassKey);
        }
        
        
        string GetDiffTime() {
            timeDifference = endDate - System.DateTime.UtcNow; // 현재 시간과의 차이를 구해서 열심히.. 
            
            if(timeDifference.Ticks <= 0) {
                isCountable = false;
                
                // 타임딜 종료되었음을 알려주고, refresh 해준다. 
                // SystemManager.ShowSimpleMessagePopUp("프리패스 타임딜이 종료되었습니다", TimeOver);
                TimeOver();
                return string.Empty;
            }
            
            return string.Format ("{0:D2}:{1:D2}:{2:D2}",timeDifference.Hours ,timeDifference.Minutes, timeDifference.Seconds);
        }
        
        void TimeOver() {
            this.gameObject.SetActive(true); // 타임딜 종료되면 비활성화 하기 
        }
        
        public void OnClick() {
            
            
            SystemManager.ShowPopupPass(passStory.projectID, true);
            
            // 구매 콜백을 상점 Refresh로 변경한다.
            UserManager.OnFreepassPurchase = MainShop.OnRefreshPackageShop;
        }
        
    }
}