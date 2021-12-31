using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class ProfileItemElement : MonoBehaviour
    {
        public ImageRequireDownload icon;
        public TextMeshProUGUI countText;

        JsonData currencyData;
        int totalCount = 1;
        public int currentCount = 0;
        

        public void InitCurrencyListElement(JsonData __j)
        {
            currencyData = __j;

            icon.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_KEY));
            totalCount = int.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_TOTAL_COUNT));
            currentCount = int.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENT_COUNT));

            if (countText != null)
                SetCountText();
        }

        public void OnClickSelectBackground()
        {
            ViewProfileDeco.OnBackgroundSetting?.Invoke(currencyData, this);
        }

        public void OnClickSelectBadge()
        {
            if (totalCount <= currentCount)
                return;

            currentCount++;
            ViewProfileDeco.OnBadgeSetting?.Invoke(currencyData, this);
        }

        /// <summary>
        /// 스티커 선택시 list에서 차감
        /// </summary>
        public void OnClickSelectSticker()
        {
            if (totalCount <= currentCount)
                return;

            currentCount++;
            SetCountText();
        }

        public void SetCountText()
        {
            countText.text = string.Format("({0}/{1})", (totalCount - currentCount), totalCount);
        }
    }
}