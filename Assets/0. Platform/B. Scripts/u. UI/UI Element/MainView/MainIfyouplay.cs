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
        public static bool ScreenSetComplete = false;
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

        private void Start()
        {
            OnRefreshIfyouplay = EnterIfyouplay;
        }

        public void EnterIfyouplay()
        {
            scroll.verticalNormalizedPosition = 1f;

            attendanceData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_ATTENDANCE_MISSION);

            // 출석 관련 세팅
            InitContinuousAttendance();
            InitDailyAttendance();

            dailyMissionData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, LobbyConst.NODE_DAILY_MISSION);

            InitDailyMission();

            ViewMain.OnRefreshIfyouplayNewSign?.Invoke();

            ScreenSetComplete = true;
        }

        #region 출석 관련


        /// <summary>
        /// 연속출석 정보 세팅
        /// </summary>
        void InitContinuousAttendance()
        {
            continuousAttendanceDate.text = string.Format(SystemManager.GetLocalizedText("5205"), SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "attendance_day"));
            continuousAttendanceDate2.text = continuousAttendanceDate.text;
            attendacneEventDeadline.text = string.Format(SystemManager.GetLocalizedText("5206"), SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "remain_day"));

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

            attendanceDay = SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "attendance_day");
            chargingDay = UserManager.main.TodayAttendanceCheck() ? SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "reset_day") : SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "reset_day") - 1;
            remainDay = SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "remain_day");
            isAttendance = SystemManager.GetJsonNodeBool(attendanceData[LobbyConst.NODE_USER_INFO][0], "is_attendance");

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
                        if (chargingDay <= 0)
                            continue;

                        // 출석일 + 충전해야하는 갯수가 i 보다 클때는 무조건 스트록 표기
                        if (i <= attendanceDay + chargingDay)
                        {
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
            disableContinuousBox.SetActive(!isAttendance);
            continuousTag.SetActive(!disableContinuousBox.activeSelf && chargingDay < 1);

            if(disableContinuousBox.activeSelf)
            {
                // 연속출석 끊겼는데 안 받은 보상이 있는 경우
                obtainableRewards[0].gameObject.SetActive(obtainableRewards[0].rewardHalo.gameObject.activeSelf);
                obtainableRewards[1].gameObject.SetActive(obtainableRewards[1].rewardHalo.gameObject.activeSelf);
                obtainableRewards[2].gameObject.SetActive(obtainableRewards[2].rewardHalo.gameObject.activeSelf);
                obtainableRewards[3].gameObject.SetActive(obtainableRewards[3].rewardHalo.gameObject.activeSelf);
            }

            // 출석 충전할 일이 있으면 무조건 출석충전 버튼을 띄운다
            attendanceChargeButton.gameObject.SetActive(chargingDay > 0);
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
            // 코인 부족한거 먼저 체크
            if(!UserManager.main.CheckCoinProperty(chargingDay * 100))
            {
                SystemManager.ShowConnectingShopPopup(SystemManager.main.spriteCoin, (chargingDay * 100) - UserManager.main.coin);
                return;
            }


            SystemManager.ShowResourceConfirm(string.Format(SystemManager.GetLocalizedText("6307"), chargingDay * 100)
                                            , chargingDay * 100
                                            , SystemManager.main.GetCurrencyImageURL(LobbyConst.COIN)
                                            , SystemManager.main.GetCurrencyImageKey(LobbyConst.COIN)
                                            , ChargeAttendance
                                            , SystemManager.GetLocalizedText("5067")
                                            , SystemManager.GetLocalizedText("5038"));
        }


        void ChargeAttendance()
        {
            if(!ScreenSetComplete)
            {
                Debug.LogWarning("화면 갱신이 아직 안됨!");
                return;
            }

            ScreenSetComplete = false;

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
        /// Daily Mission 남은시간 카운트 다운
        /// </summary>
        IEnumerator CountDownDailyMission()
        {
            while (gameObject.activeSelf && UserManager.main.dailyMissionTimer.Ticks > 0)
            {
                timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", UserManager.main.dailyMissionTimer.Hours, UserManager.main.dailyMissionTimer.Minutes, UserManager.main.dailyMissionTimer.Seconds);
                yield return null;
            }
        }

        #endregion
    }
}