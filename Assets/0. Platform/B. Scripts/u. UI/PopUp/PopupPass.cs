using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LitJson;

namespace PIERStory {
    public class PopupPass : PopupBase
    {
        
        public ImageRequireDownload imagePass;
        public GameObject normnalTitle; // 노멀 타이틀 
        public GameObject timedealTitle; // 타임딜 타이틀 
        public TextMeshProUGUI textSale; // 할인율 
        
        [SerializeField] StoryData passStory = null; // 대상 스토리
        [SerializeField] TextMeshProUGUI textOriginPrice; // 원 가격 
        [SerializeField] TextMeshProUGUI textSalePrice; // 할인 가격
        
        public int originFreepassPrice = 0;
        public int saleFreepassPrice = 0;
        
        JsonData userFreepassTimedealJSON; // 대상 작품의 유저 프리패스 타임딜 
        
        public const long addTick = 621355968000000000; // C#과 javascript 타임 Tick 차이 
        public string freepass_no = string.Empty;
        [SerializeField] TextMeshProUGUI textTimer; // 타이머 
        [SerializeField] long end_date_tick = 0; // 서버에서 받아오는 타임딜 종료시간 tick
        
        [SerializeField] DateTime endDate;
        [SerializeField] TimeSpan timeDifference; // 타임딜 종료와의 시간차 
        [SerializeField] bool isCountable = false; // 타이머 카운팅이 가능한지 
        [SerializeField] float discountFloat = 0; // 할인율 
        [SerializeField] int discountInt = 0;
        
        [SerializeField] bool useTimer = false;        
        
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            InitPremiumPass();
        }
        
        void Update() {
            
            if(!isCountable)
                return;
                
            // 5 프레임마다 갱신해주자. 
            if(Time.frameCount % 5 == 0)
                textTimer.text = GetDiffTime();
        }
        
        void InitPremiumPass() {
            
            normnalTitle.SetActive(false);
            timedealTitle.SetActive(false);
            
            // 현재 띄워놓은 작품 
            passStory = StoryManager.main.CurrentProject;
            
            // 가격. 
            originFreepassPrice = StoryManager.main.GetProjectPremiumPassOriginPrice();
            saleFreepassPrice = StoryManager.main.GetProjectPremiumPassCurrentPrice();
         
            
            // 타임딜 정보 가져오기 
            userFreepassTimedealJSON = null;
            
            // 이미지 
            imagePass.SetDownloadURL(passStory.premiumPassURL, passStory.premiumPassKey);
            
            
            normnalTitle.SetActive(true);
            textOriginPrice.text = originFreepassPrice.ToString();
            textSalePrice.text = saleFreepassPrice.ToString();
            textSale.text = StoryManager.main.GetProjectPremiumPassCurrentDiscountString() + "%";

            // 일반 
            /*
            if(userFreepassTimedealJSON == null || userFreepassTimedealJSON.Count == 0) {
                Debug.Log("No Freepass User >> Normal Product");
                
                normnalTitle.SetActive(true);
                
                // 가격 설정 
                textOriginPrice.text = originFreepassPrice.ToString();
                textSalePrice.text = saleFreepassPrice.ToString();
                textSale.text = "10%";
                
            }
            else { // 타임딜 
                
                Debug.Log("No Freepass User >> TimeDeal Product");
                
                timedealTitle.SetActive(true);
                
                
                // * 타임딜
                SetTimedeal();
            }            
            */
        }
        
        void SetTimedeal() {
            // 유저 타임딜
            userFreepassTimedealJSON = userFreepassTimedealJSON[0]; //  2개일때도 있다.
            freepass_no = userFreepassTimedealJSON["target_id"].ToString();
            
            // 시간 준비 
            end_date_tick = StoryLobbyMain.ConvertServerTimeTick(long.Parse(userFreepassTimedealJSON["end_tick"].ToString()));
            endDate = new DateTime(end_date_tick); // 틱으로 생성
            
            
            Debug.Log("EndDate : " + endDate);
            Debug.Log("Now : " + DateTime.Now);
            Debug.Log("UTC : " + DateTime.UtcNow);
            timeDifference = endDate - System.DateTime.UtcNow;
            
            if(timeDifference.Ticks <= 0) {
                isCountable = false;
            }
            else {
                isCountable = true;
            }
            
            textTimer.text = GetDiffTime(); // 일단 값 넣어주고.
            
            // 할인율 처리
            discountFloat = float.Parse(userFreepassTimedealJSON["discount"].ToString());
            discountInt = (int)(discountFloat * 100);
            
            // 할인율 표시
            textSale.text = discountInt.ToString() + "%";
            
            // 할인 가격 구하기 
            saleFreepassPrice = (int)(originFreepassPrice * (1-discountFloat));
            if(saleFreepassPrice < 3)
                saleFreepassPrice = 3;
                
            if(originFreepassPrice < 3) {
                originFreepassPrice = 3;
            }
            
            textOriginPrice.text = originFreepassPrice.ToString();
            textSalePrice.text = saleFreepassPrice.ToString();
        }
        
        /// <summary>
        /// 시간 차 구해서 남은 시간 포맷에 맞게 주기. 
        /// </summary>
        /// <returns></returns>
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
            
            // 유저 프리패스 타임딜 노드를 없애준다. 
            // UserManager.main.SetUserFreepassTimedeal(null);
            
            // 그리고 리프레시
            UserManager.OnFreepassPurchase?.Invoke();
        }
                

        
        /// <summary>
        /// 구매 처리 
        /// </summary>
        public void OnClickPurchase() {
            
            if(UserManager.main.CheckGemProperty(saleFreepassPrice)) {
                NetworkLoader.main.PurchaseProjectFreepass(freepass_no, originFreepassPrice, saleFreepassPrice);
                return;
            }
           
            
            // 젬 부족시. 
            SystemManager.ShowMessageWithLocalize("80014");
        }
    }
}