using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using LitJson;
using BestHTTP;

namespace PIERStory {
    public class PopupNickname : PopupBase
    {
        public TMP_InputField inputField;
        public TextMeshProUGUI message; // 결과 메세지
        
        
        public override void Show() {
            base.Show();
        }
        
        /// <summary>
        /// 버튼 클릭!
        /// </summary>
        public void OnClickSubmit() {
            
            if(string.IsNullOrEmpty(inputField.text)) {
                SystemManager.SetLocalizedText(message, "6119");
                return;
            }
            
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "updateUserNickname";
            sending["nickname"] = inputField.text;
            
            
            NetworkLoader.main.SendPost(OnUpdateNickname, sending);
        }
        
        void OnUpdateNickname(HTTPRequest req, HTTPResponse res)
        {
            
            message.text = string.Empty;
            
            JsonData result = null;
            if(!NetworkLoader.CheckResponseValidation(req,res))
            {
                try {
                    Debug.Log("Failed OnUpdateNickname : " + res.DataAsText);
                    
                    // 실패에 대한 처리 
                    result = JsonMapper.ToObject(res.DataAsText);
                    if(result != null && result.ContainsKey("code")) {
                        SystemManager.SetText(message, SystemManager.GetLocalizedText(result["code"].ToString()));
                    }
                    
                }
                catch(System.Exception e) {
                    Debug.Log(e.StackTrace);
                }
                return;
            }
            
            // 성공
            Debug.Log("OnUpdateNickname : " + res.DataAsText);
            result = JsonMapper.ToObject(res.DataAsText);
            // result["nickname"]

            
            // 닉네임 변경 호출
            UserManager.main.SetNewNickname(result["nickname"].ToString());
            
            Hide(); // 성공 후 닫는다.
            SystemManager.ShowSimpleAlertLocalize("6118");
        }
        
        
    }
}