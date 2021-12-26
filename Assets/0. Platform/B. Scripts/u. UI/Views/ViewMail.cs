using System;
using UnityEngine;

using LitJson;
using BestHTTP;

namespace PIERStory
{
    public class ViewMail : CommonView
    {
        public static Action<JsonData> OnRequestMailList = null;

        public MailElement[] mailElements;

        bool isEmptyMailBox = false;

        const string MAIL_LIST = "mailList";

        public override void OnStartView()
        {
            base.OnStartView();

            OnRequestMailList = SetMailList;
            NetworkLoader.main.RequestUnreadMailList(CallbackRequestUnreadMail);
        }

        /// <summary>
        /// 메일 리스트 세팅
        /// </summary>
        void SetMailList(JsonData __j)
        {
            // 비활성화
            foreach (MailElement me in mailElements)
                me.gameObject.SetActive(false);


            // 메일함이 비어 있는지 체크
            if(__j == null || __j.Count == 0)
            {
                isEmptyMailBox = true;
                return;
            }


            for (int i = 0; i < __j.Count; i++)
                mailElements[i].InitMailInfo(__j[i]);
        }

        public void GetAllMail()
        {
            // 메일함이 비어있으니 아무것도 할 필요 없음
            if (isEmptyMailBox)
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
            OnRequestMailList?.Invoke(SystemManager.GetJsonNode(data, MAIL_LIST));
            UserManager.main.SetRefreshInfo(data);
            

            // 우편을 모두 수령했습니다.
            SystemManager.ShowSimpleMessagePopUpWithLocalize("80063");
        }

        void CallbackRequestUnreadMail(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRequestUnreadMail");
                return;
            }

            OnRequestMailList?.Invoke(SystemManager.GetJsonNode(UserManager.main.notReceivedMailJson, MAIL_LIST));
        }
    }
}
