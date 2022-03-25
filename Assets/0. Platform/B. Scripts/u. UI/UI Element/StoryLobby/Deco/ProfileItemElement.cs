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
        string currencyType = string.Empty;
        public int totalCount = 1, currentCount = 0;


        public void InitCurrencyListElement(JsonData __j)
        {
            currencyJson = __j;

            foreach (GameObject g in abilities)
                g.SetActive(false);

            currencyType = SystemManager.GetJsonNodeString(currencyJson, LobbyConst.NODE_CURRENCY_TYPE);

            icon.OnDownloadImage = BackgroundResize;
            icon.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_KEY), currencyType == LobbyConst.NODE_WALLPAPER);
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
            int abilityIndex = 0;

            for (int i = 0; i < abilityData.Count; i++)
            {
                abilityIcons[abilityIndex].SetDownloadURL(SystemManager.GetJsonNodeString(abilityData[i], "ability_icon_image_url"), SystemManager.GetJsonNodeString(abilityData[i], "ability_icon_image_key"));
                abilityValueTexts[abilityIndex].text = string.Format("+ {0}", SystemManager.GetJsonNodeInt(abilityData[i], "add_value"));
                abilities[abilityIndex].SetActive(true);
                abilityIndex++;
            }
        }

        #region OnClick event

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

        #endregion

        public void SetCountText()
        {
            countText.text = string.Format("({0}/{1})", (totalCount - currentCount), totalCount);
        }

        void BackgroundResize()
        {
            // 재화 타입이 배경이 아니면 되돌아 가고
            if (currencyType != LobbyConst.NODE_WALLPAPER)
                return;

            // y축 사이즈가 1200 미만인 경우는 scale을 0.2로, 이상인 경우는 0.1로 설정
            if (icon.GetComponent<RectTransform>().sizeDelta.y < 1200f)
                icon.transform.localScale = Vector3.one * 0.2f;
            else
                icon.transform.localScale = Vector3.one * 0.1f;
        }
    }
}