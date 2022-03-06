using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class ProfileItemElement : MonoBehaviour
    {
        public ImageRequireDownload icon;
        public TextMeshProUGUI countText;
        public GameObject useCheckIcon;

        JsonData currencyData;
        public string currencyName = string.Empty;
        public string modelName = string.Empty;
        public int totalCount = 1, currentCount = 0;
        

        public void InitCurrencyListElement(JsonData __j)
        {
            currencyData = __j;

            icon.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_KEY));
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            modelName = SystemManager.GetJsonNodeString(__j, GameConst.COL_MODEL_NAME);
            totalCount = int.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_TOTAL_COUNT));
            currentCount = int.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENT_COUNT));

            if (countText != null)
                SetCountText();

            if(useCheckIcon != null)
            {
                if (currentCount == 1)
                    useCheckIcon.SetActive(true);
                else
                    useCheckIcon.SetActive(false);
            }
        }

        public void OnClickSelectBackground()
        {
            ViewStoryLobby.OnSelectBackground?.Invoke(currencyData);
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
            //ViewProfileDeco.OnStickerSetting?.Invoke(currencyData, this);
        }

        public void OnClickSelectStanding()
        {
            ViewStoryLobby.OnSelectStanding?.Invoke(currencyData, this);
        }


        public void SetCountText()
        {
            countText.text = string.Format("({0}/{1})", (totalCount - currentCount), totalCount);
        }
    }
}