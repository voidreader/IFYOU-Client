using LitJson;
using UnityEngine;

namespace PIERStory
{
    public class PopupAttendance : PopupBase
    {
        JsonData attendanceList = null;
        string attendanceId = string.Empty;

        public AttendanceElement[] attendanceElements;


        public override void Show()
        {
            base.Show();

            // 목록 초기화
            foreach (AttendanceElement ae in attendanceElements)
                ae.gameObject.SetActive(false);

            attendanceList = UserManager.main.userAttendanceList;
            bool allReceive = true;

            for (int i = 0; i < attendanceList["atendance"].Count; i++)
            {
                attendanceId = attendanceList["atendance"][i].ToString();

                for (int j = 0; j < attendanceList[attendanceId].Count; j++)
                {
                    if (!SystemManager.GetJsonNodeBool(attendanceList[attendanceId][j], "is_receive"))
                    {
                        allReceive = false;
                        break;
                    }
                }

                // 모두 받지 않았으면 이 반복문을 빠져나간다
                if (!allReceive)
                    break;
            }


            for (int i = 0; i < attendanceList[attendanceId].Count; i++)
                attendanceElements[i].InitAttendanceReward(attendanceList[attendanceId][i]);

        }

        public override void Hide()
        {
            base.Hide();

            if (SystemManager.appFirstExecute && !PlayerPrefs.HasKey("noticeOneday"))
            {
                PopupBase p = PopupManager.main.GetPopup("Notice");
                PopupManager.main.ShowPopup(p, true);
            }
        }

    }
}