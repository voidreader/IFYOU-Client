
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;


namespace PIERStory {

    public class PassButton : MonoBehaviour
    {
        [SerializeField] StoryData passStory = null; // 대상 스토리
        public LoopScaleEffect loopScaleEffect;
        
        public Image iconButton;
        
        [Header("타임딜 그룹")]
        [SerializeField] GameObject groupTimedeal;
        [SerializeField] TextMeshProUGUI textTimer; // 타이머 
        [SerializeField] TextMeshProUGUI textDiscount; // 할인율 
         
        public int timeDealID = 0;
        [SerializeField] long end_date_tick = 0; // 서버에서 받아오는 타임딜 종료시간 tick
        
        [SerializeField] DateTime endDate;
        [SerializeField] TimeSpan timeDifference; // 타임딜 종료와의 시간차 
        [SerializeField] bool isCountable = false; // 타이머 카운팅이 가능한지 
        [SerializeField] float discountFloat = 0; // 할인율 
        [SerializeField] int discountInt = 0;
        
        
        
        public PassTimeDealData passTimeDeal;
        
        
        
        public Sprite spriteNoTime; // 상시 스프라이트 
        public Sprite spriteTimedeal; // 타임딜 스프라이트 
        
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(!isCountable)
                return;
                
            // 5 프레임마다 갱신해주자. 
            if(Time.frameCount % 5 == 0)
                textTimer.text = GetDiffTime();
        }
        
        /// <summary>
        /// 배너 클릭
        /// </summary>
        public void OnClickButton() {
            PopupBase p = PopupManager.main.GetPopup("PremiumPass");
            
            if(p == null) {
                Debug.LogError("No Premium Pass popup");
                return;
            }
            
            // p.Data.targetData = passStory.projectID;
            
            PopupManager.main.ShowPopup(p, false, false);
        }
        
        
        /// <summary>
        /// 프리미엄 배너 설정하기 
        /// </summary>
        public void SetPremiumPass() {
            Debug.Log("SetPremiumPass Button");
            
            this.gameObject.SetActive(true);
            
            passStory = StoryManager.main.CurrentProject;
            
            
            
           
            // 타임딜 정보 가져오기 
            passTimeDeal = UserManager.main.GetProjectActiveTimeDeal(passStory.projectID);
            
            if(passTimeDeal == null || !passTimeDeal.isValidData) { // 일반 (타임딜 X)
                // Debug.Log("No Freepass User >> Normal Product");
                groupTimedeal.SetActive(false);
                iconButton.sprite = spriteNoTime;
                
                loopScaleEffect.enabled = false;
            }
            else {
                iconButton.sprite = spriteTimedeal;
                groupTimedeal.SetActive(true);
                
                
                // * 타임딜
                SetTimedeal();
                
                loopScaleEffect.enabled = true;
            }
            
            
            iconButton.SetNativeSize();
            
        }        
        
        
        /// <summary>
        /// 타임딜 세팅!
        /// </summary>
        /// <param name="_data">타임딜 데이터</param>
        void SetTimedeal() {
            
            // 유저 타임딜
            
            
            timeDealID = passTimeDeal.timedealID;
            
           
            // 시간 준비 
            end_date_tick = SystemConst.ConvertServerTimeTick(passTimeDeal.expireTick);
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
            discountFloat = passStory.passDiscount;
            discountFloat += (float)passTimeDeal.discountINT * 0.01f;
            discountInt = Mathf.RoundToInt(discountFloat * 100);
            
            
            // 할인율 표시
            textDiscount.text = discountInt.ToString() + "%";
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
        
        /// <summary>
        /// 타임딜 시간초과 
        /// </summary>
        void TimeOver() {
            
            SetPremiumPass();
        }        
    }
}