using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;

namespace PIERStory
{
    public class MainIfyouplay : MonoBehaviour
    {
        public static Action OnRefreshIfyouplay = null;
        public static Action OnCooldownAdEnable = null;
        public ScrollRect scroll;

        //[Header("프로모션 파트")][Space(15)]

        [Space(15)][Header("출석 관련 파트")]
        public GameObject continuousTag;
        public TextMeshProUGUI continuousAttendanceDate;
        public Button attendanceChargeButton;
        public TextMeshProUGUI continuousAttendanceDate2;
        public TextMeshProUGUI attendacneEventDeadline;

        [Tooltip("연속 출석")]
        public IFYOURewardElement[] continuousRewards = new IFYOURewardElement[4];
        public List<Image> progressDots;
        public GameObject disableContinuousBox;
        public List<IFYOURewardElement> obtainableRewards;
        int attendanceDay = 0;
        int chargingDay = 0;
        int remainDay = 0;
        bool isAttendance = false;
        [Tooltip("매일 출석")]
        public IFYOURewardElement[] dailyAttendanceRewards = new IFYOURewardElement[7];
        JsonData attendanceData = null;

        public Sprite spriteDotLimit;
        public Sprite spriteDotLimitWhite;


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

        JsonData timerAdData = null;


        private void Start()
        {
            OnRefreshIfyouplay = EnterIfyouplay;
            OnCooldownAdEnable = InitCooldownAdButton;
        }

        public void EnterIfyouplay()
        {
            attendanceData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_ATTENDANCE_MISSION);

            // 출석 관련 세팅
            //InitContinuousAttendance();
            InitDailyAttendance();

            dailyMissionData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_DAILY_MISSION);

            InitDailyMission();

            InitAdCompensation();

            ViewMain.OnRefreshIfyouplayNewSign?.Invoke();
        }

        public void EnterComplete()
        {
            scroll.verticalNormalizedPosition = 1f;
        }


        #region 출석 관련


        /// <summary>
        /// 연속출석 정보 세팅
        /// </summary>
        void InitContinuousAttendance()
        {
            attendanceDay = SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "attendance_day");
            chargingDay = SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "reset_day");
            remainDay = SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "remain_day");
            isAttendance = SystemManager.GetJsonNodeBool(attendanceData[LobbyConst.NODE_USER_INFO][0], "is_attendance");

            continuousAttendanceDate.text = string.Format(SystemManager.GetLocalizedText("5205"), attendanceDay);
            continuousAttendanceDate2.text = continuousAttendanceDate.text;
            attendacneEventDeadline.text = string.Format(SystemManager.GetLocalizedText("5206"), remainDay);

            JsonData continuousData = SystemManager.GetJsonNode(attendanceData, LobbyConst.NODE_CONTINUOUS_ATTENDANCE);

            if(continuousData == null || continuousData.Count == 0)
            {
                Debug.LogError("연속 출석 정보 없음");
                return;
            }

            for (int i = 0; i < continuousRewards.Length; i++)
            {
                continuousRewards[i].InitContinuousAttendanceReward(continuousData[i]);
                obtainableRewards[i].InitContinuousAttendanceReward(continuousData[i]);
            }

            // 보충 관련 초기화
            foreach (Image img in progressDots)
                img.transform.GetChild(0).gameObject.SetActive(false);


            for (int i = 1; i <= progressDots.Count; i++)
            {
                // 연속 출석으로 채운 날은 진분홍(자주색)
                if (i <= attendanceDay)
                    progressDots[i - 1].color = HexCodeChanger.HexToColor("FF0080");
                else
                {
                    // 연속출석 중인가?
                    if(isAttendance)
                    {
                        // 앞으로 도달할 수 있는 만큼은 하얀색
                        progressDots[i - 1].color = HexCodeChanger.HexToColor("FFFFFF");

                        // 출석 보충할거 딱히 없으면 슈루루루룩
                        if (chargingDay <= 0 || attendanceDay == 0)
                            continue;

                        // 출석일 + 충전해야하는 갯수가 i 보다 클때는 무조건 스트록 표기
                        if (i <= attendanceDay + chargingDay)
                        {
                            // 오늘치 출석을 아직 하지 않았다면 오늘 출석 일단 한 것으로 판단한다
                            if (!UserManager.main.TodayAttendanceCheck())
                                attendanceDay++;

                            // progressDots 오브젝트에는 자식 오브젝트가 1개 있는데, 해당 오브젝트를 활성화해주고, 시즌의 남은 기간 동안 연속출석으로 받을 수 있는지 없는지를 체크해서 해당 오브젝트의 sprite를 변경해준다
                            progressDots[i - 1].transform.GetChild(0).gameObject.SetActive(true);
                            progressDots[i - 1].transform.GetChild(0).GetComponent<Image>().sprite = i <= attendanceDay + remainDay ? spriteDotLimitWhite : spriteDotLimit;

                                                        
                            // 연속출석일은 3, 7, 10, 14일이므로 남은 기간동안 연속출석 보상을 받을수 있는지 없는지 체크해서 해당 오브젝트의 sprite를 변경해준다
                            if (i == 3)
                                continuousRewards[0].rewardMask.sprite = i <= attendanceDay + remainDay ? LobbyManager.main.spriteCircleLimitWhite : LobbyManager.main.spriteCircleLimit;
                            else if (i == 7)
                                continuousRewards[1].rewardMask.sprite = i <= attendanceDay + remainDay ? LobbyManager.main.spriteCircleLimitWhite : LobbyManager.main.spriteCircleLimit;
                            else if (i == 10)
                                continuousRewards[2].rewardMask.sprite = i <= attendanceDay + remainDay ? LobbyManager.main.spriteCircleLimitWhite : LobbyManager.main.spriteCircleLimit;
                            else if (i == 14)
                                continuousRewards[3].rewardMask.sprite = i <= attendanceDay + remainDay ? LobbyManager.main.spriteCircleLimitWhite : LobbyManager.main.spriteCircleLimit;

                            // 잠시 실데이터를 조작한 것이므로 다시 원상복구
                            if (!UserManager.main.TodayAttendanceCheck())
                                attendanceDay--;
                        }

                        // 충전하지 않으면 아예 도달하지 못하는 곳은 비활성화쳐럼 보이게
                        if (i > attendanceDay + remainDay)
                        {
                            progressDots[i - 1].color = HexCodeChanger.HexToColor("E1E1E1");

                            if (i == 3)
                                continuousRewards[0].disableBox.SetActive(true);
                            else if (i == 7)
                                continuousRewards[1].disableBox.SetActive(true);
                            else if (i == 10)
                                continuousRewards[2].disableBox.SetActive(true);
                            else if (i == 14)
                                continuousRewards[3].disableBox.SetActive(true);
                        }
                    }
                    else
                        progressDots[i - 1].color = HexCodeChanger.HexToColor("E1E1E1");
                }
            }

            // 연속 출석이 끊어졌으면 무조건 비활성화 overlay를 씌운다
            // 22.05.24 시즌 중간에 들어온 0일차 복귀 또는 신규 유저는 일일 출석으로 attendanceDay를 1로 올리기 전까지는 연속출석으로 보이게 한다
            disableContinuousBox.SetActive(!isAttendance);
            continuousTag.SetActive((!disableContinuousBox.activeSelf && chargingDay < 1) || (isAttendance && attendanceDay == 0));

            if(disableContinuousBox.activeSelf)
            {
                // 연속출석 끊겼는데 안 받은 보상이 있는 경우
                obtainableRewards[0].gameObject.SetActive(obtainableRewards[0].rewardHalo.gameObject.activeSelf);
                obtainableRewards[1].gameObject.SetActive(obtainableRewards[1].rewardHalo.gameObject.activeSelf);
                obtainableRewards[2].gameObject.SetActive(obtainableRewards[2].rewardHalo.gameObject.activeSelf);
                obtainableRewards[3].gameObject.SetActive(obtainableRewards[3].rewardHalo.gameObject.activeSelf);
            }

            // 출석 충전할 일이 있으면 무조건 출석충전 버튼을 띄운다
            attendanceChargeButton.gameObject.SetActive(chargingDay > 0 && !continuousTag.activeSelf);
            continuousAttendanceDate2.gameObject.SetActive(attendanceChargeButton.gameObject.activeSelf && isAttendance);
        }


        /// <summary>
        /// 매일출석 정보 세팅
        /// </summary>
        void InitDailyAttendance()
        {
            JsonData dailyData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson[LobbyConst.NODE_ATTENDANCE_MISSION], LobbyConst.NODE_ATTENDANCE);
            string attendanceKey = dailyData[LobbyConst.NODE_ATTENDANCE][0].ToString();
            dailyData = SystemManager.GetJsonNode(dailyData, attendanceKey);

            for(int i=0;i < dailyAttendanceRewards.Length;i++)
                dailyAttendanceRewards[i].InitDailyAttendanceReward(dailyData[i]);
        }


        /// <summary>
        /// 연속 출석 충전
        /// </summary>
        public void OnClickChargeContinuousAttendance()
        {
            SystemManager.ShowResourceConfirm(string.Format(SystemManager.GetLocalizedText("6307"), chargingDay)
                                            , chargingDay * 100
                                            , SystemManager.main.GetCurrencyImageURL(LobbyConst.COIN)
                                            , SystemManager.main.GetCurrencyImageKey(LobbyConst.COIN)
                                            , ChargeAttendance
                                            , SystemManager.GetLocalizedText("5067")
                                            , SystemManager.GetLocalizedText("5038"));
        }


        void ChargeAttendance()
        {
            // 코인 부족한거 먼저 체크
            if (!UserManager.main.CheckCoinProperty(chargingDay * 100))
            {
                //SystemManager.ShowConnectingShopPopup(SystemManager.main.spriteCoin, (chargingDay * 100) - UserManager.main.coin);
                return;
            }

            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "resetAttendanceMission";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;

            NetworkLoader.main.SendPost(CallbackChargeAttendanceMission, sending, true);
        }

        void CallbackChargeAttendanceMission(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackChargeAttendanceMission");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            // 출석 보충을 했으니 재화 갱신해주고
            UserManager.main.SetBankInfo(result);
            UserManager.main.SetNotificationInfo(result);

            // 데이터를 갱신해주기 전에 팝업 띄워주기
            SystemManager.ShowMessageAlert(string.Format(SystemManager.GetLocalizedText("6314"), chargingDay));

            // 연속출석이 끊긴 상태에서 오늘치 일일출석을 하지 않았을 때 뜨는 팝업
            if (!isAttendance && !UserManager.main.TodayAttendanceCheck())
                SystemManager.ShowSimpleAlertLocalize("6177", false);

            // 출석 관련 정보도 갱신해주고
            UserManager.main.RefreshIfyouplayJsonData(result);

            // 이프유플레이 화면 갱신이 필요함
            OnRefreshIfyouplay?.Invoke();
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
            if (dailyMissionData == null)
                return;

            dailyMissionGauge.fillAmount = SystemManager.GetJsonNodeFloat(dailyMissionData["all"][0], "current_result") / SystemManager.GetJsonNodeInt(dailyMissionData["all"][0], "limit_count");
            dailyMissionReward.InitDailyTotalReward(dailyMissionData["all"][0]);

            for (int i = 0; i < dailyMissionElements.Length; i++)
                dailyMissionElements[i].InitDailyMission(dailyMissionData["single"][i]);

            StartCoroutine(CountDownDailyMission());
        }


        /// <summary>
        /// Daily Mission 남은시간 카운트 다운.
        /// (22.06.27 추가) 오늘의 광고 보상은 Daily Mission과 동일한 시간을 갖는다
        /// </summary>
        IEnumerator CountDownDailyMission()
        {
            while (gameObject.activeSelf && UserManager.main.dailyMissionTimer.Ticks > 0)
            {
                timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", UserManager.main.dailyMissionTimer.Hours, UserManager.main.dailyMissionTimer.Minutes, UserManager.main.dailyMissionTimer.Seconds);
                dailyAdTimerText.text = timerText.text;
                yield return new WaitForSeconds(0.1f);
            }
        }

        #endregion

        #region 광고 보고 보상받기

        void InitAdCompensation()
        {
            missionAdData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_MISSION_AD_REWARD);

            if(missionAdData == null)
            {
                Debug.LogError("미션 광고 JsonData 없음!");
                return;
            }

            missionAdTitle.text = SystemManager.GetJsonNodeString(missionAdData[0], "name");

            adRewardAmountTexts[0].text = SystemManager.GetJsonNodeString(missionAdData[0], "first_" + CommonConst.NODE_QUANTITY);
            adRewardAmountTexts[1].text = SystemManager.GetJsonNodeString(missionAdData[0], "second_" + CommonConst.NODE_QUANTITY);
            adRewardAmountTexts[2].text = SystemManager.GetJsonNodeString(missionAdData[0], "third_" + CommonConst.NODE_QUANTITY);

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

            timerAdData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_TIMER_AD_REWARD);

            if (timerAdData == null)
            {
                Debug.LogError("쿨타임 광고 JsonData 없음!");
                return;
            }

            timerAdTitle.text = SystemManager.GetJsonNodeString(timerAdData[0], "name");
            timerAdContent.text = SystemManager.GetJsonNodeString(timerAdData[0], "content");
            rewardAmount.text = SystemManager.GetJsonNodeString(timerAdData[0], "first_" + CommonConst.NODE_QUANTITY);

            InitCooldownAdButton();
        }

        void InitCooldownAdButton()
        {
            
            if(!this.gameObject.activeSelf)
                return;

            
            showCooldownAdButton.SetActive(UserManager.main.adCoolDownTimer.Ticks <= 0);
            cooldownState.SetActive(UserManager.main.adCoolDownTimer.Ticks > 0);

            if (cooldownState.activeSelf)
                StartCoroutine(RoutineNextAdCooldown());
        }

        
        /// <summary>
        /// 광고 시청하기
        /// </summary>
        public void OnClickWatchingCommercial(int adNo)
        {
            if (adNo == 1)
                AdManager.main.ShowRewardAdWithCallback(RequestAdMissionAccumulate);
            else if (adNo == 2)
                AdManager.main.ShowRewardAdWithCallback(RequestCooldownAdReward);
        }

        /// <summary>
        /// 광고 보상 받기
        /// </summary>
        public void OnClickGetAdReward()
        {
            NetworkLoader.main.RequestAdReward(1);
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
            sending[CommonConst.FUNC] = "increaseMissionAdReward";
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

            UserManager.main.userIfyouPlayJson = JsonMapper.ToObject(res.DataAsText);
            OnRefreshIfyouplay?.Invoke();
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

        IEnumerator RoutineNextAdCooldown()
        {
            while (gameObject.activeSelf && UserManager.main.adCoolDownTimer.Ticks > 0)
            {
                nextAdCooldown.text = string.Format("{0:D2}:{1:D2}", UserManager.main.adCoolDownTimer.Minutes, UserManager.main.adCoolDownTimer.Seconds);
                yield return new WaitForSeconds(0.1f);
            }

            OnRefreshIfyouplay?.Invoke();
        }    

        #endregion
    }
}