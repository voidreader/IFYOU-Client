using UnityEngine;
using UnityEngine.EventSystems;

using LitJson;

namespace PIERStory
{
    public class StandingElement : MonoBehaviour, IPointerClickHandler
    {
        public ImageRequireDownload standingImage;
        public ProfileItemElement profileItemElement;
        public RectTransform standingRect;

        public string currencyName = string.Empty;
        float posX = 0f, posY = 0f;
        float width = 300f, height = 300f;
        float angle = 0f;

        public void NewStanding(JsonData __j, ProfileItemElement connectElement)
        {
            string url = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL);
            string key = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY);
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

            SystemManager.ShowNetworkLoading();
            standingImage.OnDownloadImage = SystemManager.HideNetworkLoading;
            standingImage.SetDownloadURL(url, key, true);
            profileItemElement = connectElement;
        }

        public void SetProfileStanding(JsonData __j, ProfileItemElement connectElement = null)
        {
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            standingImage.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY), true);

            posX = float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_POS_X));
            posY = float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_POS_Y));

            width = standingRect.sizeDelta.x;
            height = standingRect.sizeDelta.y;

            angle = float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ANGLE));

            RollbackTransform();

            if (connectElement != null)
                profileItemElement = connectElement;
        }

        
        public void SetRectTransInfo()
        {
            posX = standingRect.anchoredPosition.x;
            posY = standingRect.anchoredPosition.y;

            width = standingRect.sizeDelta.x;
            height = standingRect.sizeDelta.y;

            angle = standingRect.eulerAngles.y;
        }

        /// <summary>
        /// 위치 값, 뒤집기 등 원상복귀
        /// </summary>
        public void RollbackTransform()
        {
            standingRect.anchoredPosition = new Vector2(posX, posY);
            standingRect.sizeDelta = new Vector2(width, height);
            standingRect.eulerAngles = new Vector3(0f, angle, 0f);
        }

        /// <summary>
        /// 화면에서 삭제됨
        /// </summary>
        public void RemoveFromScreen()
        {
            if (profileItemElement != null)
                profileItemElement.currentCount--;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            ViewProfileDeco.OnDisableAllOptionals?.Invoke();
            ViewProfileDeco.OnControlStanding?.Invoke(this);
        }

        public JsonData SaveStandingData(int sortingOrder)
        {
            JsonData data = new JsonData();

            data[LobbyConst.NODE_CURRENCY] = currencyName;
            data[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
            data[LobbyConst.NODE_POS_X] = posX;
            data[LobbyConst.NODE_POS_Y] = posY;
            data[LobbyConst.NODE_WIDTH] = width;
            data[LobbyConst.NODE_HEIGHT] = height;
            data[LobbyConst.NODE_ANGLE] = angle;

            return data;
        }
    }
}