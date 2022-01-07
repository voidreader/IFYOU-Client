using UnityEngine;
using UnityEngine.EventSystems;

using LitJson;

namespace PIERStory
{
    public class ProfileBriefElement : MonoBehaviour, IPointerClickHandler
    {
        public ImageRequireDownload thumbnail;
        public GameObject selectBox;

        string currencyName = string.Empty;
        public int currentCount = 0;

        public void InitProfileBrief(JsonData __j)
        {
            string url = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_URL);
            string key = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_KEY);

            string currency = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

            currencyName = currency;
            thumbnail.SetDownloadURL(url, key);

            currentCount = int.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENT_COUNT));

            if (currentCount == 1)
                selectBox.SetActive(true);
            else
                selectBox.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ViewProfileAdornment.OnSelectCanel?.Invoke();

            selectBox.SetActive(true);

            currentCount = 1;
        }

        /// <summary>
        /// 선택 취소
        /// </summary>
        public void SelectCancel()
        {
            selectBox.SetActive(false);

            currentCount = 0;
        }

        public JsonData SaveJsonData()
        {
            JsonData data = new JsonData();

            data[LobbyConst.NODE_CURRENCY] = currencyName;
            data[LobbyConst.NODE_SORTING_ORDER] = 0;
            data[LobbyConst.NODE_POS_X] = 0f;
            data[LobbyConst.NODE_POS_Y] = 0f;
            data[LobbyConst.NODE_WIDTH] = thumbnail.downloadedSprite.rect.width;
            data[LobbyConst.NODE_HEIGHT] = thumbnail.downloadedSprite.rect.height;
            data[LobbyConst.NODE_ANGLE] = 0f;

            return data;
        }
    }
}