using System.Collections;
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
        public Image dayHighlight;
        public GameObject rewardToday;

        public ImageRequireDownload currencyIcon;
        public TextMeshProUGUI amountText;

        public Image coverOverlay;
        public Image checkIcon;

        string attendanceId = string.Empty;
        int daySeq = 0;

        string currency = string.Empty;
        string imageUrl = string.Empty;
        string imageKey = string.Empty;

        bool isReceive = false;
        bool clickCheck = false;
        bool current = false;

        public void InitAttendanceReward(JsonData __j)
        {
            attendanceId = SystemManager.GetJsonNodeString(__j, "attendance_id");
            daySeq = SystemManager.GetJsonNodeInt(__j, "day_seq");

            currency = SystemManager.GetJsonNodeString(__j, "currency");

            imageUrl = SystemManager.GetJsonNodeString(__j, "icon_image_url");
            imageKey = SystemManager.GetJsonNodeString(__j, "icon_image_key");

            isReceive = SystemManager.GetJsonNodeBool(__j, "is_receive");
            clickCheck = SystemManager.GetJsonNodeBool(__j, "click_check");
            current = SystemManager.GetJsonNodeBool(__j, "current");

            amountText.text = SystemManager.GetJsonNodeString(__j, "quantity");

            if(currency == "coin")
                currencyIcon.GetComponent<Image>().sprite = LobbyManager.main.spriteAttendanceCoin;
            else if(currency =="gem")
            {
                if(daySeq == 7)
                    currencyIcon.GetComponent<Image>().sprite = LobbyManager.main.spriteAttendanceStars;
                else
                    currencyIcon.GetComponent<Image>().sprite = LobbyManager.main.spriteAttendanceStar;
            }
            else
                currencyIcon.SetDownloadURL(imageUrl, imageKey);


            if (isReceive)
                coverOverlay.gameObject.SetActive(true);
            else
                coverOverlay.gameObject.SetActive(false);

            gameObject.SetActive(true);
        }

        private void Start()
        {
            if (current && clickCheck)
            {
                dayHighlight.gameObject.SetActive(true);
                rewardToday.SetActive(true);
                StartCoroutine(SpriteShiny());
            }
        }


        #region Material 연출

        IEnumerator SpriteShiny()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(dayHighlight.DOFade(0.3f, 1.5f).SetDelay(0.5f));
            seq.Append(dayHighlight.DOFade(1f, 1.5f));
            seq.SetLoops(-1);

            //dayHighlight.DOFade(0.3f, 1.5f).SetLoops(-1, LoopType.Yoyo);

            Material todayFrame = rewardToday.GetComponent<Image>().material;
            const float animTime = 1f;

            while(rewardToday.gameObject.activeSelf)
            {
                todayFrame.DOFloat(1f, "_ShineLocation", animTime);
                yield return new WaitForSeconds(animTime * 2);
                todayFrame.SetFloat("_ShineLocation", 0f);
            }
        }

        #endregion



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

            // 보상을 받았으니 리스트를 갱신해주자
            NetworkLoader.main.RequestAttendanceList();
            UserManager.main.SetNotificationInfo(JsonMapper.ToObject(res.DataAsText));

            dayHighlight.gameObject.SetActive(false);
            rewardToday.SetActive(false);
            coverOverlay.color = new Color(1, 1, 1, 0);
            checkIcon.color = new Color(1, 1, 1, 0);
            coverOverlay.gameObject.SetActive(true);
            coverOverlay.DOFade(1f, 0.4f);
            checkIcon.DOFade(1f, 0.2f).SetDelay(0.2f);
            checkIcon.transform.DOPunchScale(Vector3.one * 1.5f, 0.4f).SetDelay(0.3f);

            SystemManager.ShowSimpleAlertLocalize("6177", false);
        }
        
    }
}