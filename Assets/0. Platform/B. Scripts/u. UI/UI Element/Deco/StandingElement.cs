using UnityEngine;
using UnityEngine.EventSystems;

using LitJson;

namespace PIERStory
{
    public class StandingElement : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ImageRequireDownload standingImage;
        public ProfileItemElement profileItemElement;
        public RectTransform standingRect;

        public string currencyName = string.Empty;
        float posX = 0f, posY = 0f;
        float width = 800f, height = 300f;
        float angle = 0f;

        const float movalbeWidth = 250f;
        float startX = 0f, dragX = 0f, originX = 0f, calcPosX;

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

            posX = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_POS_X);
            posY = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_POS_Y);

            width = standingRect.sizeDelta.x;
            height = standingRect.sizeDelta.y;

            angle = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_ANGLE);

            standingRect.anchoredPosition = new Vector2(posX, posY);
            standingRect.sizeDelta = new Vector2(width, height);

            if (angle == 180)
                standingRect.localScale = new Vector3(-1f, 1f, 1f);
            else
                standingRect.localScale = Vector3.one;

            if (connectElement != null)
                profileItemElement = connectElement;
        }

        /// <summary>
        /// 캐릭터 픽스 후, 위치 고정
        /// </summary>
        public void SetRectTransInfo()
        {
            posX = standingRect.anchoredPosition.x;
            posY = standingRect.anchoredPosition.y;

            width = standingRect.sizeDelta.x;
            height = standingRect.sizeDelta.y;

            if (standingRect.localScale.x == -1)
                angle = 180f;
            else
                angle = 0f;
        }

        /// <summary>
        /// 화면에서 삭제됨
        /// </summary>
        public void RemoveFromScreen()
        {
            if (profileItemElement != null)
                profileItemElement.currentCount--;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startX = eventData.position.x;
            originX = GetComponent<RectTransform>().anchoredPosition.x;
        }

        
        public void OnDrag(PointerEventData eventData)
        {
            dragX = eventData.position.x;
            calcPosX = originX + (dragX - startX);

            if (calcPosX > -movalbeWidth && calcPosX < movalbeWidth)
                GetComponent<RectTransform>().anchoredPosition = new Vector2(calcPosX, 0f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (calcPosX <= -movalbeWidth)
                GetComponent<RectTransform>().anchoredPosition = new Vector2(-movalbeWidth, 0f);
            else if(calcPosX >= movalbeWidth)
                GetComponent<RectTransform>().anchoredPosition = new Vector2(movalbeWidth, 0f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ViewProfileDeco.OnDisableAllOptionals?.Invoke();
            ViewProfileDeco.OnControlStanding?.Invoke(this);
        }

        public JsonData SaveStandingData(int sortingOrder)
        {
            JsonData data = new JsonData();

            if (standingRect.localScale.x == -1)
                angle = 180f;
            else
                angle = 0f;

            data[LobbyConst.NODE_CURRENCY] = currencyName;
            data[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
            data[LobbyConst.NODE_POS_X] = standingRect.anchoredPosition.x;
            data[LobbyConst.NODE_POS_Y] = standingRect.anchoredPosition.y;
            data[LobbyConst.NODE_WIDTH] = standingRect.sizeDelta.x;
            data[LobbyConst.NODE_HEIGHT] = standingRect.sizeDelta.y;
            data[LobbyConst.NODE_ANGLE] = angle;

            return data;
        }

    }
}