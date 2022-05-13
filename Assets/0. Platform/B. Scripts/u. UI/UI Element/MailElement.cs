using UnityEngine;
using UnityEngine.UI;

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
        
        public Image imageBaseCurrencyIcon; // 기본 재화 아이콘 
        public TextMeshProUGUI textBaseCurrencyQuantity; // 기본 재화 개수 
        
        [SerializeField] string currencyName = string.Empty;
        [SerializeField] string currencyURL = string.Empty;
        [SerializeField] string currencyKey = string.Empty;
        [SerializeField] string projectTitle = string.Empty; // 연결 프로젝트 타이틀
        
        
        string mailType = string.Empty;
        string mailNo = string.Empty;
        string currency = string.Empty;
        string quantity = string.Empty;
        int hour = 0, min = 0;

        const string MAIL_NO = "mail_no";
        const string MAIL_TYPE_TEXTID = "mail_type_textid";
        
        const string REMAIN_HOURS = "remain_hours";
        const string REMAIN_MINS = "remain_mins";

        public void InitMailInfo(JsonData __j)
        {
            gameObject.SetActive(true);
            mailNo = SystemManager.GetJsonNodeString(__j, MAIL_NO);
            mailType = SystemManager.GetJsonNodeString(__j, "mail_type");
            currency = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            quantity = SystemManager.GetJsonNodeString(__j, CommonConst.NODE_QUANTITY);
            mailTitle.text = SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, MAIL_TYPE_TEXTID));
            hour = int.Parse(SystemManager.GetJsonNodeString(__j, REMAIN_HOURS));
            min = int.Parse(SystemManager.GetJsonNodeString(__j, REMAIN_MINS));
            
            
            // 재화 아이콘 처리 (기본재화, 다운로드 아이콘 재화 구분 )
            if(currency == LobbyConst.GEM || currency == LobbyConst.COIN) {
                
                // * 여기는 일반 재화 (젬, 코인)
                currencyIcon.gameObject.SetActive(false);
                imageBaseCurrencyIcon.gameObject.SetActive(true);
                
                if(currency == LobbyConst.GEM)
                    imageBaseCurrencyIcon.sprite = SystemManager.main.spriteStar;
                else if(currency == LobbyConst.COIN)
                    imageBaseCurrencyIcon.sprite = SystemManager.main.spriteCoin;
                    
                imageBaseCurrencyIcon.SetNativeSize();
                textBaseCurrencyQuantity.text = quantity.ToString(); // 개수 
                
            }
            else {
                // * 기본재화 아닌 경우 
                currencyURL = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_IMAGE_URL);
                currencyKey = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_IMAGE_KEY);
                
                currencyIcon.gameObject.SetActive(true);
                imageBaseCurrencyIcon.gameObject.SetActive(false);
                currencyIcon.SetDownloadURL(currencyURL, currencyKey);
            }
            
            // 재화 이름 
            currencyName = SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, "local_code"));
            
            projectTitle = SystemManager.GetJsonNodeString(__j, "connected_project_title");

            
            mailContent.text = string.Format(SystemManager.GetLocalizedText("6104"), quantity, currencyName);
            
            // 예외적으로 mail_type inapp_origin은 아이콘 고정 처리 
            if(mailType == "inapp_origin") {
                currencyIcon.gameObject.SetActive(false);
                imageBaseCurrencyIcon.gameObject.SetActive(true);
                imageBaseCurrencyIcon.sprite = SystemManager.main.spriteInappOriginIcon;
                imageBaseCurrencyIcon.SetNativeSize();
                // currencyIcon.SetTexture2D(SystemManager.main.spriteInappOriginIcon.texture);
                textBaseCurrencyQuantity.text = string.Empty; 
                    
                mailContent.text = SystemManager.GetLocalizedText("80083");
            }

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
            SystemManager.ShowNetworkLoading();
            NetworkLoader.main.RequestSingleMail(CallbackRecievedSingleMail, mailNo);
        }

        /// <summary>
        /// 메일 받기 완료 처리
        /// </summary>
        void CallbackRecievedSingleMail(HTTPRequest req, HTTPResponse res)
        {
            SystemManager.HideNetworkLoading();
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRecievedSingleMail");
                return;
            }

            JsonData data = JsonMapper.ToObject(res.DataAsText);

            // 재화 갱신
            UserManager.main.SetRefreshInfo(data);

            // 메일 리스트 갱신
            //ViewMail.OnRequestMailList?.Invoke(SystemManager.GetJsonNode(data, MAIL_LIST));
            PopupMail.OnRequestMailList?.Invoke();
            // UserManager.OnFreepassPurchase?.Invoke();

            SystemManager.ShowSimpleAlertLocalize("80082");
        }
    }
}
