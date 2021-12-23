using UnityEngine;

using TMPro;
using LitJson;
using BestHTTP;

namespace PIERStory
{
    public class MailElement : MonoBehaviour
    {
        public ImageRequireDownload currencyIcon;
        public TextMeshProUGUI mailTitle;
        public TextMeshProUGUI mailContent;
        public TextMeshProUGUI remainTime;

        string mailNo = string.Empty;
        string currency = string.Empty;
        string quantity = string.Empty;
        int hour = 0, min = 0;

        const string MAIL_NO = "mail_no";
        const string MAIL_TYPE_TEXTID = "mail_type_textid";
        const string MAIL_LIST = "mailList";
        const string CURRENCY = "currency";
        const string QUANTITY = "quantity";
        const string REMAIN_HOURS = "remain_hours";
        const string REMAIN_MINS = "remain_mins";

        public void InitMailInfo(JsonData __j)
        {
            gameObject.SetActive(true);
            mailNo = SystemManager.GetJsonNodeString(__j, MAIL_NO);
            currency = SystemManager.GetJsonNodeString(__j, CURRENCY);
            quantity = SystemManager.GetJsonNodeString(__j, QUANTITY);
            mailTitle.text = SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, MAIL_TYPE_TEXTID));
            hour = int.Parse(SystemManager.GetJsonNodeString(__j, REMAIN_HOURS));
            min = int.Parse(SystemManager.GetJsonNodeString(__j, REMAIN_MINS));

            // 21.12.23 임시 표기, 이후 수정 필요
            mailContent.text = currency == "gem" ? string.Format("스타 {0}개 획득", quantity) : string.Format("코인 {0}개 획득", quantity);

            int day = hour / 24, h = hour % 24;

            if (day > 0)
                remainTime.text = string.Format(SystemManager.GetLocalizedText("5091"), day.ToString(), h.ToString());
            else
            {
                if (h > 0)
                    remainTime.text = string.Format(SystemManager.GetLocalizedText("5092"), hour.ToString());
                else
                    remainTime.text = string.Format(SystemManager.GetLocalizedText("5093"), min.ToString());
            }
        }

        /// <summary>
        /// 메일 받기
        /// </summary>
        public void GetMail()
        {
            NetworkLoader.main.RequestSingleMail(CallbackRecievedSingleMail, mailNo);
        }

        /// <summary>
        /// 메일 받기 완료 처리
        /// </summary>
        void CallbackRecievedSingleMail(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRecievedSingleMail");
                return;
            }

            JsonData data = JsonMapper.ToObject(res.DataAsText);

            // 재화 갱신
            UserManager.main.SetRefreshInfo(data);

            // 메일 리스트 갱신
            ViewMail.OnRequestMailList?.Invoke(SystemManager.GetJsonNode(data, MAIL_LIST));
            UserManager.OnFreepassPurchase?.Invoke();

            SystemManager.ShowSimpleMessagePopUp(SystemManager.GetLocalizedText("80082"));
        }
    }
}
