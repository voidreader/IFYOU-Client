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
        int currentCount = 0;

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
    }
}