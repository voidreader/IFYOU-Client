using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;


namespace PIERStory {

    public class PassBanner : MonoBehaviour
    {
        [SerializeField] StoryData passStory = null; // 대상 스토리
        [SerializeField] TextMeshProUGUI textTitle; // 타이틀.. 
        
        [Header("타임딜 그룹")]
        [SerializeField] GameObject groupTimedeal;
        [SerializeField] TextMeshProUGUI textTimedealPrice;
        [SerializeField] TextMeshProUGUI textTimedealSale;
        [SerializeField] GameObject groupTimer; // 타이머 그룹 
        [SerializeField] TextMeshProUGUI textTimer; // 타이머 
         public const long addTick = 621355968000000000; // C#과 javascript 타임 Tick 차이 
        public string freepass_no = string.Empty;
        [SerializeField] long end_date_tick = 0; // 서버에서 받아오는 타임딜 종료시간 tick
        
        [SerializeField] DateTime endDate;
        [SerializeField] TimeSpan timeDifference; // 타임딜 종료와의 시간차 
        [SerializeField] bool isCountable = false; // 타이머 카운팅이 가능한지 
        [SerializeField] float discountFloat = 0; // 할인율 
        [SerializeField] int discountInt = 0;
        
        [SerializeField] bool useTimer = false;
        
        
        
        [Space]
        [Header("일반 그룹")]
        [SerializeField] GameObject groupNormal;
        [SerializeField] TextMeshProUGUI textNormalPrice;
        [SerializeField] TextMeshProUGUI textNormalSale;
        
        
        [Space]
        [SerializeField] ImageRequireDownload bannerImage;
        [SerializeField] Image frame; // 프레임 이미지 
        
        [SerializeField] Sprite spriteTimedealFrame; // 타임딜 프레임 스프라이트
        [SerializeField] Sprite spriteNormalFrame; // 노멀 프레임 스프라이트 
        
        
        public int originFreepassPrice = 0;
        public int saleFreepassPrice = 0;
        
        JsonData userFreepassTimedealJSON; // 대상 작품의 유저 프리패스 타임딜 
         
       
        
        void Update() {
            
            if(!useTimer)
                return;
            
            if(!isCountable)
                return;
                
            // 5 프레임마다 갱신해주자. 
            if(Time.frameCount % 5 == 0)
                textTimer.text = GetDiffTime();
        }
        
        /// <summary>
        /// 프리미엄 배너 설정하기 
        /// </summary>
        public void SetPremiumPass(bool __useTimer) {
            Debug.Log("SetPremiumPass");
            
            this.gameObject.SetActive(true);
            
            passStory = StoryManager.main.CurrentProject;
            
            textTitle.text = StoryManager.main.CurrentProject.title; // 타이틀
            
            useTimer = __useTimer;
            
            // 가격. 
            originFreepassPrice = StoryManager.main.GetProjectFreepassPrice(true);
            saleFreepassPrice = StoryManager.main.GetProjectFreepassPrice(false);
            
            // 타임딜 정보 가져오기 
            userFreepassTimedealJSON = UserManager.main.GetUserFreepassTimedeal();
            
            // 배너 
            bannerImage.SetDownloadURL(StoryManager.main.freepassBannerURL, StoryManager.main.freepassBannerKey);
            
            // 일반 
            if(userFreepassTimedealJSON == null || userFreepassTimedealJSON.Count == 0) {
                Debug.Log("No Freepass User >> Normal Product");
                
                frame.sprite = spriteNormalFrame;
                
                textNormalPrice.text = saleFreepassPrice.ToString();
                textNormalSale.text = "10%";
                
                groupNormal.SetActive(true);
                groupTimedeal.SetActive(false);
                
            }
            else {
                
                Debug.Log("No Freepass User >> TimeDeal Product");
                
                frame.sprite = spriteTimedealFrame;
                
                groupNormal.SetActive(false);
                groupTimedeal.SetActive(true);
                
                
                // * 타임딜
                SetTimedeal();
            }
            
        }
        
        /// <summary>
        /// 프로젝트ID로 세팅하기 (미구현)
        /// </summary>
        /// <param name="__projectID"></param>
        /// <param name="__useTimer"></param>
        public void SetPremiumPassByID(string __projectID, bool __useTimer) {
            // isTargetBanner = true; 
            // passStory = 아이디로 스토리를 찾아서 블라블라. 
        }
        
        /// <summary>
        /// 배너 클릭
        /// </summary>
        public void OnClickBanner() {
            PopupBase p = PopupManager.main.GetPopup("PremiumPass");
            
            if(p == null) {
                Debug.LogError("No Premium Pass popup");
                return;
            }
            
            p.Data.targetData = passStory.projectID;
            
            PopupManager.main.ShowPopup(p, false, false);
        }
        
        
        
                /// <summary>
        /// 타임딜 세팅!
        /// </summary>
        /// <param name="_data">타임딜 데이터</param>
        void SetTimedeal() {
            
            // 유저 타임딜
            userFreepassTimedealJSON = userFreepassTimedealJSON[0]; //  2개일때도 있다.
            
            
            freepass_no = userFreepassTimedealJSON["target_id"].ToString();
            
            
            
            // 시간 준비 
            end_date_tick = ConvertServerTimeTick(long.Parse(userFreepassTimedealJSON["end_tick"].ToString()));
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
            textTimedealSale.text = discountInt.ToString() + "%";
            
            // 할인 가격 구하기 
            saleFreepassPrice = (int)(originFreepassPrice * (1-discountFloat));
            if(saleFreepassPrice < 3)
                saleFreepassPrice = 3;
                
            if(originFreepassPrice < 3) {
                originFreepassPrice = 3;
            }
            
            textTimedealPrice.text = saleFreepassPrice.ToString(); 
            
            // 타이머 사용!
            groupTimer.SetActive(useTimer);
            
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
            UserManager.main.SetUserFreepassTimedeal(null);
            
            // 그리고 리프레시
            UserManager.OnFreepassPurchase?.Invoke();
        }
        
        public static long ConvertServerTimeTick(long __serverTick) {
            return (__serverTick * 10000) + addTick;
        }        
        
        
    }
}