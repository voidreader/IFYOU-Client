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

        //[Header("프로모션 파트")][Space(15)]

        [Header("출석 관련 파트")]
        public TextMeshProUGUI continuousAttendanceDate;
        public TextMeshProUGUI attendacneEventDeadline;
        public Image continuousAttendanceGauge;
        public RectTransform chargeAttendanceButton;

        [Tooltip("연속 출석")]
        public IFYOURewardElement[] continuousRewards = new IFYOURewardElement[4];
        [Tooltip("매일 출석")]
        public IFYOURewardElement[] dailyAttendanceRewards = new IFYOURewardElement[7];

        [Space(15)][Header("Daily Mission")]
        public Image dailyMissionGauge;
        public IFYOURewardElement dailyMissionReward;

        private void Start()
        {
            OnRefreshIfyouplay = EnterIfyouplay;
        }

        public void EnterIfyouplay()
        {
            InitContinuousAttendance();
            InitDailyAttendance();

            //StartCoroutine(CountDownDailyMission());
        }


        /// <summary>
        /// Daily Mission 남은시간 카운트 다운
        /// </summary>
        IEnumerator CountDownDailyMission()
        {
            while (gameObject.activeSelf)
            {


                yield return null;
            }
        }


        /// <summary>
        /// 연속출석 정보 세팅
        /// </summary>
        void InitContinuousAttendance()
        {
            continuousAttendanceDate.text = string.Format(SystemManager.GetLocalizedText("5205"), SystemManager.GetJsonNodeInt(UserManager.main.userIfyouPlayJson["user_info"][0], "attendance_day"));
            attendacneEventDeadline.text = string.Format(SystemManager.GetLocalizedText("5206"), SystemManager.GetJsonNodeInt(UserManager.main.userIfyouPlayJson["user_info"][0], "remain_day"));

            JsonData continuousData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, "continuous_attendance");

            if(continuousData == null || continuousData.Count == 0)
            {
                Debug.LogError("연속 출석 정보 없음");
                return;
            }

            for (int i = 0; i < continuousRewards.Length; i++)
                continuousRewards[i].InitContinuousAttendanceReward(continuousData[i], SystemManager.GetJsonNodeInt(UserManager.main.userIfyouPlayJson["user_info"][0], "attendance_day") >= SystemManager.GetJsonNodeInt(continuousData[i], "day_seq"));

            continuousAttendanceGauge.fillAmount = SystemManager.GetJsonNodeFloat(UserManager.main.userIfyouPlayJson["user_info"][0], "attendance_day") / 14;
            chargeAttendanceButton.gameObject.SetActive(!SystemManager.GetJsonNodeBool(UserManager.main.userIfyouPlayJson["user_info"][0], "is_attendance"));
            chargeAttendanceButton.anchoredPosition = new Vector2(600 * (SystemManager.GetJsonNodeFloat(UserManager.main.userIfyouPlayJson["user_info"][0], "attendance_day") + SystemManager.GetJsonNodeFloat(UserManager.main.userIfyouPlayJson["user_info"][0], "reset_day")) / 14, 55f);
        }


        /// <summary>
        /// 매일출석 정보 세팅
        /// </summary>
        void InitDailyAttendance()
        {
            JsonData dailyData = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson, "attendance");
            string attendanceKey = dailyData["attendance"][0].ToString();
            dailyData = SystemManager.GetJsonNode(dailyData, attendanceKey);

            for(int i=0;i < dailyAttendanceRewards.Length;i++)
                dailyAttendanceRewards[i].InitIfyouPlayReward(dailyData[i]);
        }


        /// <summary>
        /// 연속 출석 충전
        /// </summary>
        public void OnClickChargeContinuousAttendance()
        {
            SystemManager.ShowResourceConfirm(string.Format(SystemManager.GetLocalizedText("6307"), SystemManager.GetJsonNodeInt(UserManager.main.userIfyouPlayJson["user_info"][0], "reset_day") * 100)
                                            , SystemManager.GetJsonNodeInt(UserManager.main.userIfyouPlayJson["user_info"][0], "reset_day") * 100
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
            
        }

        
        /// <summary>
        /// 매일 출석 help 버튼 클릭
        /// </summary>
        public void OnClickDailyAttendanceInfo()
        {

        }
    }
}