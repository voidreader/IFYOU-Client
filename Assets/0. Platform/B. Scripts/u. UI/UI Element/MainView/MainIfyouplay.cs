using System;
using System.Collections;
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
        public ScrollRect scroll;

        //[Header("프로모션 파트")][Space(15)]

        [Header("출석 관련 파트")]
        public TextMeshProUGUI continuousAttendanceDate;
        public TextMeshProUGUI attendacneEventDeadline;
        public Image continuousAttendanceGauge;
        public RectTransform chargeAttendanceButton;

        [Tooltip("연속 출석")]
        public IFYOURewardElement[] continuousRewards = new IFYOURewardElement[4];
        public Image[] progressDots;
        [Tooltip("매일 출석")]
        public IFYOURewardElement[] dailyAttendanceRewards = new IFYOURewardElement[7];
        JsonData attendanceData = null;


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
        }

        #region 출석 관련


        /// <summary>
        /// 연속출석 정보 세팅
        /// </summary>
        void InitContinuousAttendance()
        {
            continuousAttendanceDate.text = string.Format(SystemManager.GetLocalizedText("5205"), SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "attendance_day"));
            attendacneEventDeadline.text = string.Format(SystemManager.GetLocalizedText("5206"), SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "remain_day"));

            JsonData continuousData = SystemManager.GetJsonNode(attendanceData, LobbyConst.NODE_CONTINUOUS_ATTENDANCE);

            if(continuousData == null || continuousData.Count == 0)
            {
                Debug.LogError("연속 출석 정보 없음");
                return;
            }

            for (int i = 0; i < continuousRewards.Length; i++)
                continuousRewards[i].InitContinuousAttendanceReward(continuousData[i]);

            for (int i = 1; i <= progressDots.Length; i++)
            {
                if (i == 3 || i == 7 || i == 10 || i == 14)
                    continue;


                if (i <= SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "attendance_day"))
                    progressDots[i - 1].color = HexCodeChanger.HexToColor("FF0080");
                else
                {
                    if(SystemManager.GetJsonNodeBool(attendanceData[LobbyConst.NODE_USER_INFO][0], "is_attendance"))
                        progressDots[i - 1].color = HexCodeChanger.HexToColor("FFAFD7");
                    else
                        progressDots[i - 1].color = HexCodeChanger.HexToColor("E1E1E1");
                }
            }


            continuousAttendanceGauge.fillAmount = SystemManager.GetJsonNodeFloat(attendanceData[LobbyConst.NODE_USER_INFO][0], "attendance_day") / 14;
            chargeAttendanceButton.gameObject.SetActive(!SystemManager.GetJsonNodeBool(attendanceData[LobbyConst.NODE_USER_INFO][0], "is_attendance"));
            chargeAttendanceButton.anchoredPosition = new Vector2(600 * (SystemManager.GetJsonNodeFloat(attendanceData[LobbyConst.NODE_USER_INFO][0], "attendance_day") + SystemManager.GetJsonNodeFloat(attendanceData[LobbyConst.NODE_USER_INFO][0], "reset_day")) / 14, 35f);
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
            SystemManager.ShowResourceConfirm(string.Format(SystemManager.GetLocalizedText("6307"), SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "reset_day") * 100)
                                            , SystemManager.GetJsonNodeInt(attendanceData[LobbyConst.NODE_USER_INFO][0], "reset_day") * 100
                                            , SystemManager.main.GetCurrencyImageURL(LobbyConst.COIN)
                                            , SystemManager.main.GetCurrencyImageKey(LobbyConst.COIN)
                                            , ChargeAttendance
                                            , SystemManager.GetLocalizedText("5067")
                                            , SystemManager.GetLocalizedText("5038"));
        }


        void ChargeAttendance()
        {
            chargeAttendanceButton.GetComponent<Button>().interactable = false;

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
                chargeAttendanceButton.GetComponent<Button>().interactable = true;
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
            chargeAttendanceButton.GetComponent<Button>().interactable = true;
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
            while (gameObject.activeSelf)
            {
                timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", UserManager.main.dailyMissionTimer.Hours, UserManager.main.dailyMissionTimer.Minutes, UserManager.main.dailyMissionTimer.Seconds);
                yield return null;
                yield return null;
                yield return null;
            }
        }

        #endregion
    }
}