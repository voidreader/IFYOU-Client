using UnityEngine;

using TMPro;
using LitJson;
using BestHTTP;


namespace PIERStory
{
    public class PopupCoupon : PopupBase
    {
        public TMP_InputField couponCode;
        public TextMeshProUGUI message;

        public override void Show()
        {
            base.Show();
        }

        public void OnClickSubmit()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "useCoupon";
            sending["coupon_code"] = couponCode.text.ToUpper();

            NetworkLoader.main.SendPost(CallbackUseCoupon, sending, true);
        }

        void CallbackUseCoupon(HTTPRequest req, HTTPResponse res)
        {
            if(!NetworkLoader.CheckResponseValidation(req,res))
            {
                Debug.LogError("Failed CallbackUseCoupon");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            if(result.ContainsKey("code"))
            {
                if(SystemManager.GetJsonNodeString(result, "code").Equals("80058"))
                    message.text = string.Format("<color=#6284FF>{0}</color>", SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(result, "code")));
                else
                    message.text = string.Format("<color=#FF0000>{0}</color>", SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(result, "code")));
            }

            UserManager.main.SetNotificationInfo(result);


            // 해금된 에피소드 추가
        }
    }
}