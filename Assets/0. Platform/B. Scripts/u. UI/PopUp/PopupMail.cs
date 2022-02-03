using System;
using UnityEngine;

using LitJson;
using BestHTTP;

namespace PIERStory
{
    public class PopupMail : PopupBase
    {
        public static Action OnRequestMailList = null;
        public MailElement[] mailElements;

        public GameObject mailScroll;
        public GameObject noMail;

        public override void Show()
        {
            base.Show();

            OnRequestMailList = SetMailList;
            OnRequestMailList?.Invoke();
        }


        /// <summary>
        /// 메일 리스트 세팅
        /// </summary>
        void SetMailList()
        {
            if (UserManager.main.notReceivedMailJson["mailList"] == null || UserManager.main.notReceivedMailJson["mailList"].Count == 0)
            {
                mailScroll.SetActive(false);
                noMail.SetActive(true);
                return;
            }

            foreach (MailElement me in mailElements)
                me.gameObject.SetActive(false);

    

            for (int i = 0; i < UserManager.main.notReceivedMailJson["mailList"].Count; i++) {
                
                if(i >= mailElements.Length) 
                    break;
                
                mailElements[i].InitMailInfo(UserManager.main.notReceivedMailJson["mailList"][i]);
            }
        }


        public void OnClickRecievedAllMail()
        {
            if (noMail.activeSelf)
                return;

            SystemManager.ShowNetworkLoading();
            NetworkLoader.main.RequestAllMail(CallbackRecievedAllMail);
        }

        void CallbackRecievedAllMail(HTTPRequest req, HTTPResponse res)
        {
            SystemManager.HideNetworkLoading();
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRecieveAllMail");
                return;
            }

            JsonData data = JsonMapper.ToObject(res.DataAsText);
            UserManager.main.SetRefreshInfo(data);
            OnRequestMailList?.Invoke();

            // 우편을 모두 수령했습니다.
            SystemManager.ShowSimpleAlertLocalize("80063");
        }

        public void OnClickCoinShop()
        {
            SystemManager.main.OpenCoinShopWebview();
            AdManager.main.AnalyticsCoinShopOpen("top");
        }
    }
}