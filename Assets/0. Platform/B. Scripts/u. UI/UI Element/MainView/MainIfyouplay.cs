using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class MainIfyouplay : MonoBehaviour
    {
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
            JsonData continuousData = SystemManager.GetJsonNode(UserManager.main.userAttendanceMission, "continuous_attendance");

            if(continuousData == null || continuousData.Count == 0)
            {
                Debug.LogError("연속 출석 정보 없음");
                return;
            }

            for (int i = 0; i < continuousRewards.Length; i++)
                continuousRewards[i].InitContinuousAttendanceReward(continuousData[i]);
        }


        /// <summary>
        /// 매일출석 정보 세팅
        /// </summary>
        void InitDailyAttendance()
        {
            JsonData dailyData = SystemManager.GetJsonNode(UserManager.main.userAttendanceMission, "attendance");
            string attendanceKey = SystemManager.GetJsonNodeString(dailyData, "attendance");
            dailyData = SystemManager.GetJsonNode(dailyData, attendanceKey);

            for(int i=0;i < dailyAttendanceRewards.Length;i++)
                dailyAttendanceRewards[i].InitIfyouPlayReward(dailyData[i]);
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