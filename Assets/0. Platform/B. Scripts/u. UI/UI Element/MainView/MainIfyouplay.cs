using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace PIERStory
{
    public class MainIfyouplay : MonoBehaviour
    {
        public static Action OnRefreshIfyouplay = null; // 이프유 플레이 페이지 전체 리프레시
        
        public static Action OnRefreshAttendance = null; // 출석체크 부분 
        public static Action OnRefreshDailyMissionPart = null; // 데일리 미션 부분 리프레시 
        public static Action OnRefreshTimerAdvertisementPart = null; 
        public static Action OnRefreshMissionAdvertisementPart = null;
        public ScrollRect scroll;

        

 
        [Tooltip("매일 출석")]
        public IFYOURewardElement[] dailyAttendanceRewards = new IFYOURewardElement[7];
        JsonData attendanceData = null;



        [Space(15)][Header("Daily Mission")]
        public TextMeshProUGUI timerText;
        public Image dailyMissionGauge;
        public IFYOURewardElement dailyMissionReward;
        public IFYOUDailyMissionElement[] dailyMissionElements = new IFYOUDailyMissionElement[4];

        JsonData dailyMissionData = null;


        [Space(15)][Header("광고 보고 재화얻기")]
        public TextMeshProUGUI missionAdTitle;
        public TextMeshProUGUI dailyAdTimerText;
        public List<TextMeshProUGUI> adRewardAmountTexts;
        public List<Image> rewardAura;
        public List<GameObject> adRewardChecks;
        public TextMeshProUGUI currentAdRewardLevelText;
        public Image adMissionGauge;
        public TextMeshProUGUI adMissionProgress;
        public GameObject showAdMissionButton;
        public GameObject getAdMissionRewardButton;
        public GameObject adsMissionComplete;

        JsonData missionAdData = null;

        [Space]
        public TextMeshProUGUI timerAdTitle;
        public TextMeshProUGUI timerAdContent;
        public TextMeshProUGUI rewardAmount;
        public GameObject showCooldownAdButton;
        public GameObject cooldownState;
        public TextMeshProUGUI nextAdCooldown;
        
        public GameObject buttonIFyouRefresh; // 이프유 플레이 리프레시 버튼 

        JsonData timerAdData = null;


        private void Start()
        {
            OnRefreshIfyouplay = EnterIfyouplay;
                        
            OnRefreshAttendance = InitDailyAttendance;
            OnRefreshDailyMissionPart = InitDailyMission;
            
            OnRefreshMissionAdvertisementPart = InitMissionAdvertisementPart;
            OnRefreshTimerAdvertisementPart = InitTimerAdvertisementPart;
            
        }
        
        
        void Update() {
            if(!this.gameObject.activeSelf)
                return;
                
            if(UserManager.main == null)
                return;
                
            // Daily mission 남은시간 카운트 다운 처리 
            // 10프레임마다 체크하자.
            if(Time.frameCount % 10 == 0) {
                TimerDailyMission();
                TimerAdvertisement();
            }
        }
        
        
        /// <summary>
        /// 일일 미션 타이머 처리 
        /// </summary>
        void TimerDailyMission() {
           
            if(UserManager.main.dailyMissionTimer == null)
                return;
            
            timerText.text = UserManager.main.GetDailyMissionRefreshTimeText();
            dailyAdTimerText.text = timerText.text;
            
            // 시간이 다 흐른 경우에는 리프레시 버튼을 보이게 한다. 
            buttonIFyouRefresh.SetActive(string.IsNullOrEmpty(timerText.text));
        }
        
        
        /// <summary>
        /// 광고 타이머 처리 
        /// </summary>
        void TimerAdvertisement() {
            if(UserManager.main.adCoolDownTimer == null)
                return;
                
            //  || UserManager.main.adCoolDownTimer.Ticks <= 0
            nextAdCooldown.text = UserManager.main.GetIFyouPlayAdRefreshTimeText();
            
            if(string.IsNullOrEmpty(nextAdCooldown.text)) {
                
            }
            
            // 광고 재생 버튼 
            showCooldownAdButton.SetActive(string.IsNullOrEmpty(nextAdCooldown.text));
            
            // 타이머 돌아갈때
            cooldownState.SetActive(!string.IsNullOrEmpty(nextAdCooldown.text));
            
        }
        
        
        /// <summary>
        /// 이프유 플레이 진입시 호출 
        /// </summary>
        public void EnterIfyouplay()
        {
            AdManager.main.CheckOfferwallCredit();

            // 출석 관련 세팅
            //InitContinuousAttendance();
            InitDailyAttendance();

            InitDailyMission();

            InitMissionAdvertisementPart();
            InitTimerAdvertisementPart();

            ViewMain.OnRefreshIfyouplayNewSign?.Invoke();
        }

        public void EnterComplete()
        {
            scroll.verticalNormalizedPosition = 1f;
        }
        
        
        /// <summary>
        /// 이프유 플레이 페이지 리프레시 
        /// </summary>
        public void OnClickRefreshIFyouPlay() {
            NetworkLoader.main.RequestIfyouplayList(true);
        }


        #region 출석 관련




        /// <summary>
        /// 매일출석 정보 세팅
        /// </summary>
        void InitDailyAttendance()
        {
            attendanceData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_ATTENDANCE_MISSION);
            
            JsonData dailyData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson[LobbyConst.NODE_ATTENDANCE_MISSION], LobbyConst.NODE_ATTENDANCE);
            string attendanceKey = dailyData != null ? dailyData[LobbyConst.NODE_ATTENDANCE][0].ToString() : string.Empty;

            if(string.IsNullOrEmpty(attendanceKey))
            {
                SystemManager.ShowMessageAlert("System Error");
                NetworkLoader.main.ReportRequestError("Daily attendance error", JsonMapper.ToStringUnicode(dailyData) + "\nattendanceKey = " + attendanceKey);
                return;
            }

            dailyData = SystemManager.GetJsonNode(dailyData, attendanceKey);

            for(int i=0;i < dailyAttendanceRewards.Length;i++)
                dailyAttendanceRewards[i].InitDailyAttendanceReward(dailyData[i]);
        }





        /// <summary>
        /// 연속출석 help 버튼 클릭
        /// </summary>
        public void OnClickContinuousInfo()
        {
            PopupBase p = PopupManager.main.GetPopup("HelpBox");

            if(p == null)
            {
                Debug.LogError("도움말 박스 팝업이 없음");
                return;
            }

            p.Data.SetLabelsTexts(SystemManager.GetLocalizedText("6308"));
            p.Data.contentValue = 226;
            PopupManager.main.ShowPopup(p, false);
        }

        
        /// <summary>
        /// 매일 출석 help 버튼 클릭
        /// </summary>
        public void OnClickDailyAttendanceInfo()
        {
            PopupBase p = PopupManager.main.GetPopup("HelpBox");

            if (p == null)
            {
                Debug.LogError("도움말 박스 팝업이 없음");
                return;
            }

            p.Data.SetLabelsTexts(SystemManager.GetLocalizedText("6309"));
            p.Data.contentValue = 166;
            PopupManager.main.ShowPopup(p, false);
        }

        #endregion

        #region 데일리 미션

        void InitDailyMission()
        {
            dailyMissionData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_DAILY_MISSION);
            
            if (dailyMissionData == null)
                return;
                
            if(dailyMissionData.Count == 0)
                return;
                
            // 미션 리스트에서 첫번째는 전체 일일미션 클리어에 대한 미션이다. 

            dailyMissionGauge.fillAmount = SystemManager.GetJsonNodeFloat(dailyMissionData[0], "current_result") / SystemManager.GetJsonNodeInt(dailyMissionData[0], "limit_count");
            dailyMissionReward.InitDailyTotalReward(dailyMissionData[0]);
            
            
            int index = 1; // 1부터 시작 
            for (int i = 0; i < dailyMissionElements.Length; i++)
                dailyMissionElements[i].InitDailyMission(dailyMissionData[index++]);

            // StartCoroutine(CountDownDailyMission());
        }


        /// <summary>
        /// Daily Mission 남은시간 카운트 다운.
        /// (22.06.27 추가) 오늘의 광고 보상은 Daily Mission과 동일한 시간을 갖는다
        /// </summary>
        IEnumerator CountDownDailyMission()
        {
            if(gameObject == null)
                yield break;
            
            while (gameObject.activeSelf && UserManager.main.dailyMissionTimer.Ticks > 0)
            {
                timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", UserManager.main.dailyMissionTimer.Hours, UserManager.main.dailyMissionTimer.Minutes, UserManager.main.dailyMissionTimer.Seconds);
                dailyAdTimerText.text = timerText.text;
                yield return new WaitForSeconds(0.1f);
            }
        }

        #endregion

        #region 설문조사

        public void OnClickOpenSurvey()
        {
            if (SystemManager.main.isWebViewOpened)
                return;

            if (Application.isEditor)
                return;

            string uidParam = string.Format("?uid={0}", UserManager.main.GetUserPinCode());
            string langParam = string.Format("&lang={0}", SystemManager.main.currentAppLanguageCode);

            string finalURL = SystemManager.main.surveyUrl + uidParam + langParam;
            Debug.Log("Survey : " + finalURL);

            SystemManager.main.webView = WebView.CreateInstance();
            WebView.OnHide += OnHideWebview;


            Debug.Log(">> OnHideWebview LoadURL");
            // SystemManager.main.webView.ClearCache();
            SystemManager.main.webView.SetFullScreen();
            SystemManager.main.webView.ScalesPageToFit = true;
            SystemManager.main.webView.LoadURL(URLString.URLWithPath(finalURL));
            SystemManager.main.webView.Show();

            SystemManager.main.isWebViewOpened = true;
            SystemManager.SetBlockBackButton(true);
        }

        void OnHideWebview(WebView __view)
        {
            SystemManager.main.isWebViewOpened = false;  // 닫힐때 false로 변경 
            SystemManager.SetBlockBackButton(false);

            Debug.Log(">> OnHideWebview in IFYouplay");
            WebView.OnHide -= OnHideWebview;

            __view.gameObject.SetActive(false);
            Destroy(__view);

            // 신규 메일이 온 것이 있는지 확인확인
            NetworkLoader.main.RequestUnreadMailList(CallbackCheckUnreadMail);
        }

        void CallbackCheckUnreadMail(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackCheckUnreadMail");
                return;
            }

            UserManager.main.SetNotificationInfo(JsonMapper.ToObject(res.DataAsText));
        }

        #endregion

        #region 광고 보고 보상받기

        /// <summary>
        /// 미션 광고 부분 초기화 
        /// </summary>
        void InitMissionAdvertisementPart()
        {
            missionAdData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_MISSION_AD_REWARD);

            if(missionAdData == null)
            {
                Debug.LogError("미션 광고 JsonData 없음!");
                return;
            }

            SystemManager.SetText(missionAdTitle, SystemManager.GetJsonNodeString(missionAdData[0], "name"));

            SystemManager.SetText(adRewardAmountTexts[0], SystemManager.GetJsonNodeString(missionAdData[0], "first_" + CommonConst.NODE_QUANTITY));
            SystemManager.SetText(adRewardAmountTexts[1], SystemManager.GetJsonNodeString(missionAdData[0], "second_" + CommonConst.NODE_QUANTITY));
            SystemManager.SetText(adRewardAmountTexts[2], SystemManager.GetJsonNodeString(missionAdData[0], "third_" + CommonConst.NODE_QUANTITY));

            adRewardChecks[0].SetActive(SystemManager.GetJsonNodeBool(missionAdData[0], "first_clear"));
            adRewardChecks[1].SetActive(SystemManager.GetJsonNodeBool(missionAdData[0], "second_clear"));
            adRewardChecks[2].SetActive(SystemManager.GetJsonNodeBool(missionAdData[0], "third_clear"));

            int currentResult = 0, totalCount = 1, level = 1;
            
            level = SystemManager.GetJsonNodeInt(missionAdData[0], "step");
            currentAdRewardLevelText.text = string.Format("Lv.{0}", level);

            currentResult = SystemManager.GetJsonNodeInt(missionAdData[0], "current_result");
            totalCount = SystemManager.GetJsonNodeInt(missionAdData[0], "total_count");
            adMissionProgress.text = string.Format("{0}/{1}", currentResult, totalCount);
            adMissionGauge.fillAmount = (float)currentResult / totalCount;

            showAdMissionButton.SetActive(currentResult < totalCount);
            getAdMissionRewardButton.SetActive(currentResult >= totalCount);
            adsMissionComplete.SetActive(level >= 3 && currentResult >= totalCount && dailyAdTimerText && !string.IsNullOrEmpty(SystemManager.GetJsonNodeString(missionAdData[0], "clear_date")));

            // 현재 상황에 따른 Aura처리
            for (int i = 0; i < rewardAura.Count; i++)
                rewardAura[i].color = level == i + 1 && !adsMissionComplete.activeSelf ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);

            

        }
        
        /// <summary>
        /// 타이머 광고 리워드 부분 수정 
        /// </summary>
        void InitTimerAdvertisementPart() {
            timerAdData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_TIMER_AD_REWARD);

            if (timerAdData == null)
            {
                Debug.LogError("쿨타임 광고 JsonData 없음!");
                return;
            }

            SystemManager.SetText(timerAdTitle, SystemManager.GetJsonNodeString(timerAdData[0], "name"));
            SystemManager.SetText(timerAdContent, SystemManager.GetJsonNodeString(timerAdData[0], "content"));
            rewardAmount.text = SystemManager.GetJsonNodeString(timerAdData[0], "first_" + CommonConst.NODE_QUANTITY);
        }



        
        /// <summary>
        /// 광고 시청하기
        /// </summary>
        public void OnClickWatchingCommercial(int adNo)
        {
            if (adNo == 1)
                AdManager.main.ShowRewardAdWithCallback(RequestAdMissionAccumulate); // 미션 광고 보상
            else if (adNo == 2)
                AdManager.main.ShowRewardAdWithCallback(RequestCooldownAdReward); // 시간 광고 보상 
        }

        /// <summary>
        /// 광고 보상 받기
        /// </summary>
        public void OnClickGetAdReward()
        {
            NetworkLoader.main.RequestAdReward(1);
        }
        
        public void OnClickOfferwall() {
            AdManager.main.ShowIronSourceOfferwall();
        }


        /// <summary>
        /// 광고 미션 누적하기
        /// </summary>
        /// <param name="isRewarded"></param>
        void RequestAdMissionAccumulate(bool isRewarded)
        {
            if (!isRewarded)
                return;

            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "increaseMissionAdRewardOptimized";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;

            NetworkLoader.main.SendPost(CallbackAccumulateAdCount, sending, true);
        }

        void CallbackAccumulateAdCount(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackAccumulateAdCount");
                return;
            }
            
            JsonData result = JsonMapper.ToObject(res.DataAsText);
            
            if(UserManager.main.userIfyouPlayJson == null || !result.ContainsKey(LobbyConst.NODE_MISSION_AD_REWARD)) {
                Debug.LogError("Wrong response in CallbackAccumulateAdCount");
                return;
            }
                

            UserManager.main.userIfyouPlayJson[LobbyConst.NODE_MISSION_AD_REWARD] = result[LobbyConst.NODE_MISSION_AD_REWARD];
            
            // 데일리 미션도 업데이트 해준다. 
            if(result.ContainsKey(LobbyConst.NODE_DAILY_MISSION)) 
                UserManager.main.userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION] = result[LobbyConst.NODE_DAILY_MISSION];
            
            OnRefreshMissionAdvertisementPart?.Invoke();
            OnRefreshDailyMissionPart?.Invoke();
            ViewMain.OnRefreshIfyouplayNewSign?.Invoke();
        }


        /// <summary>
        /// 쿨타임 광고 보상 받기
        /// </summary>
        /// <param name="isRewarded"></param>
        void RequestCooldownAdReward(bool isRewarded)
        {
            if (!isRewarded)
                return;

            NetworkLoader.main.RequestAdReward(2);
        }


        #endregion
    }
}