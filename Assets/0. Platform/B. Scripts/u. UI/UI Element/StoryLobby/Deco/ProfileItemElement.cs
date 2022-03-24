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

        public GameObject[] abilities;
        public ImageRequireDownload[] abilityIcons;
        public TextMeshProUGUI[] abilityValueTexts;

        JsonData currencyJson;

        public string currencyName = string.Empty;
        public string modelName = string.Empty;
        public int totalCount = 1, currentCount = 0;


        public void InitCurrencyListElement(JsonData __j)
        {
            currencyJson = __j;

            foreach (GameObject g in abilities)
                g.SetActive(false);

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

            if (!__j.ContainsKey(GameConst.TEMPLATE_ABILITY))
                return;

            JsonData abilityData = SystemManager.GetJsonNode(__j, GameConst.TEMPLATE_ABILITY);

            for (int i = 0; i < abilityData.Count; i++)
            {
                abilityIcons[0].SetDownloadURL(SystemManager.GetJsonNodeString(abilityData[i], "ability_icon_image_url"), SystemManager.GetJsonNodeString(abilityData[i], "ability_icon_image_key"));
                abilityValueTexts[0].text = string.Format("+ {0}", SystemManager.GetJsonNodeInt(abilityData[i], "add_value"));
                abilities[0].SetActive(true);
            }
        }

        public void OnClickSelectBackground()
        {
            ViewStoryLobby.OnSelectBackground?.Invoke(currencyJson);
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
            ViewStoryLobby.OnStickerSetting?.Invoke(currencyJson, this);
        }

        public void OnClickSelectStanding()
        {
            ViewStoryLobby.OnSelectStanding?.Invoke(currencyJson, this);
        }


        public void SetCountText()
        {
            countText.text = string.Format("({0}/{1})", (totalCount - currentCount), totalCount);
        }
    }
}