using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using DG.Tweening;

namespace PIERStory
{
    public class AttendanceElement : MonoBehaviour
    {
        public ImageRequireDownload currencyIcon;
        public TextMeshProUGUI amountText;

        public Image coverOverlay;
        public Image checkIcon;

        string attendanceId = string.Empty;
        int daySeq = 0;

        string imageUrl = string.Empty;
        string imageKey = string.Empty;

        bool isReceive = false;
        bool clickCheck = false;

        public void InitAttendanceReward(JsonData __j)
        {
            attendanceId = SystemManager.GetJsonNodeString(__j, "attendance_id");
            daySeq = SystemManager.GetJsonNodeInt(__j, "day_seq");

            imageUrl = SystemManager.GetJsonNodeString(__j, "icon_image_url");
            imageKey = SystemManager.GetJsonNodeString(__j, "icon_image_key");

            isReceive = SystemManager.GetJsonNodeBool(__j, "is_receive");
            clickCheck = SystemManager.GetJsonNodeBool(__j, "click_check");

            currencyIcon.SetDownloadURL(imageUrl, imageKey);
            amountText.text = SystemManager.GetJsonNodeString(__j, "quantity");

            if(isReceive)
                coverOverlay.gameObject.SetActive(true);
            else
                coverOverlay.gameObject.SetActive(false);

            gameObject.SetActive(true);
        }

        /// <summary>
        /// 출석보상 받기
        /// </summary>
        public void OnClickRecieveReward()
        {
            // 받은 적 있으면 아무 행동도 하지 맙시다
            if (isReceive || !clickCheck)
                return;

            clickCheck = false;

            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "sendAttendanceReward";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending["attendance_id"] = attendanceId;
            sending["day_seq"] = daySeq;

            NetworkLoader.main.SendPost(CallbackAttendanceReward, sending, true);
        }

        void CallbackAttendanceReward(HTTPRequest req, HTTPResponse res)
        {
            if(!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackAttendanceReward");
                return;
            }

            coverOverlay.color = new Color(1, 1, 1, 0);
            checkIcon.color = new Color(1, 1, 1, 0);
            coverOverlay.gameObject.SetActive(true);
            coverOverlay.DOFade(1f, 0.4f);
            checkIcon.DOFade(1f, 0.2f).SetDelay(0.2f);
            checkIcon.transform.DOPunchScale(Vector3.one * 1.5f, 0.4f).SetDelay(0.3f);
        }
        
    }
}