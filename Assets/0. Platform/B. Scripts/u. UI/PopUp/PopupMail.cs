using System;
using UnityEngine;

using LitJson;
using BestHTTP;

namespace PIERStory
{
    public class PopupMail : PopupBase
    {
        public static Action<JsonData> OnRequestMailList = null;
        public MailElement[] mailElements;

        public GameObject mailScroll;
        public GameObject noMail;

        public override void Show()
        {
            base.Show();

            OnRequestMailList = SetMailList;
            OnRequestMailList?.Invoke(UserManager.main.notReceivedMailJson["mailList"]);
        }


        /// <summary>
        /// 메일 리스트 세팅
        /// </summary>
        void SetMailList(JsonData __j)
        {
            if (__j == null || __j.Count == 0)
            {
                mailScroll.SetActive(false);
                noMail.SetActive(true);
                return;
            }

            for (int i = 0; i < __j.Count; i++)
                mailElements[i].InitMailInfo(__j[i]);
        }


        public void OnClickRecievedAllMail()
        {
            if (noMail.activeSelf)
                return;

            NetworkLoader.main.RequestAllMail(CallbackRecievedAllMail);
        }

        void CallbackRecievedAllMail(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRecieveAllMail");
                return;
            }

            JsonData data = JsonMapper.ToObject(res.DataAsText);
            OnRequestMailList?.Invoke(SystemManager.GetJsonNode(data, "mailList"));
            UserManager.main.SetRefreshInfo(data);

            // 우편을 모두 수령했습니다.
            SystemManager.ShowSimpleAlertLocalize("80063");
        }
    }
}