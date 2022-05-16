using UnityEngine;

using TMPro;
using LitJson;


namespace PIERStory
{
    public class PopupAttendance : PopupBase
    {
        JsonData attendanceList = null;

        public TextMeshProUGUI textCurrentDay; // 현재 일자 표시 
        public GameObject tipAttendance; // 팁 

        public AttendanceElement[] attendanceElements;
        public int currentDay = 0;


        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();

            // 목록 초기화
            foreach (AttendanceElement ae in attendanceElements)
                ae.gameObject.SetActive(false);

            string attendanceId = UserManager.main.userIfyouPlayJson[LobbyConst.NODE_ATTENDANCE_MISSION][LobbyConst.NODE_ATTENDANCE][LobbyConst.NODE_ATTENDANCE][0].ToString();
            attendanceList = SystemManager.GetJsonNode(UserManager.main.userIfyouPlayJson[LobbyConst.NODE_ATTENDANCE_MISSION][LobbyConst.NODE_ATTENDANCE], attendanceId);

            for (int i = 0; i < attendanceList.Count; i++)
            {
                attendanceElements[i].InitAttendanceReward(attendanceList[i]);

                if (SystemManager.GetJsonNodeBool(attendanceList[i], "current"))
                    currentDay = SystemManager.GetJsonNodeInt(attendanceList[i], LobbyConst.NODE_DAY_SEQ);
            }
            
            // 현재 일차 처리 
            textCurrentDay.text = string.Format(SystemManager.GetLocalizedText("6259"), currentDay.ToString());

        }
    }
}