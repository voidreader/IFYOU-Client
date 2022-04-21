using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class CoinStarHistoryElement :MonoBehaviour
    {
        public TextMeshProUGUI historyContent;
        public TextMeshProUGUI date;
        public TextMeshProUGUI currencyAmount;

        const string KEY_LOG_TYPE = "log_type";
        const string KEY_LOG_CODE_TEXTID = "log_code_textid";
        const string KEY_CURRENCY_TEXTID = "currency_textid";
        const string KEY_LOG_TYPE_TEXTID = "log_type_textid";
        const string KEY_QUANTITY = "quantity";
        const string KEY_ACTION_DATE = "action_date";
        const string USE = "use";
         
        public void InitHistoryInfo(JsonData __j)
        {
            string sign = SystemManager.GetJsonNodeString(__j, KEY_LOG_TYPE).Equals(USE) ? "-" : "+";

            historyContent.text = string.Format("{0} {1} {2}", SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, KEY_LOG_CODE_TEXTID)), SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, KEY_CURRENCY_TEXTID)), SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, KEY_LOG_TYPE_TEXTID)));
            date.text = SystemManager.GetJsonNodeString(__j, KEY_ACTION_DATE);

            currencyAmount.text = string.Format("{0}{1}", sign, SystemManager.GetJsonNodeString(__j, KEY_QUANTITY));
        }
    }
}
