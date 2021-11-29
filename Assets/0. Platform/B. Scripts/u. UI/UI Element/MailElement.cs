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

        const string MAIL_NO = "mail_no";
        const string MAIL_TYPE_TEXTID = "mail_type_textid";
        const string MAIL_LIST = "mailList";

        public void InitMailInfo(JsonData __j)
        {
            gameObject.SetActive(true);
            mailNo = SystemManager.GetJsonNodeString(__j, MAIL_NO);
            mailTitle.text = SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, MAIL_TYPE_TEXTID));

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
