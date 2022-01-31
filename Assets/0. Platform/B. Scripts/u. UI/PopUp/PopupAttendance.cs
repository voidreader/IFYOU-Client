using LitJson;

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

            for (int i = 0; i < attendanceList["attendance"].Count; i++)
            {
                attendanceId = attendanceList["attendance"][i].ToString();

                for (int j = 0; j < attendanceList[attendanceId].Count; j++)
                {
                    if (SystemManager.GetJsonNodeBool(attendanceList[attendanceId][j], "current") && SystemManager.GetJsonNodeBool(attendanceList[attendanceId][j], "click_check"))
                    {
                        allReceive = false;
                        break;
                    }
                }

                // 모두 받지 않았으면 이 반복문을 빠져나간다
                if (!allReceive)
                    break;

                // 마지막 날까지 다 받았고, 다음으로 넘어갈게 있지만 아직 다음날이 아닌경우 이 반복문을 빠져나가야 해
                if (i + 1 < attendanceList["attendance"].Count && !SystemManager.GetJsonNodeBool(attendanceList[attendanceList["attendance"][i + 1].ToString()][0], "is_receive") && !SystemManager.GetJsonNodeBool(attendanceList[attendanceList["attendance"][i + 1].ToString()][0], "click_check") && !SystemManager.GetJsonNodeBool(attendanceList[attendanceList["attendance"][i + 1].ToString()][0], "current"))
                {
                    break;
                }
            }


            for (int i = 0; i < attendanceList[attendanceId].Count; i++)
                attendanceElements[i].InitAttendanceReward(attendanceList[attendanceId][i]);

        }
    }
}