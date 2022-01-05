using UnityEngine;
using UnityEngine.EventSystems;

using LitJson;

namespace PIERStory
{
    public class ItemElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public ImageRequireDownload itemImage;
        public GameObject[] buttons;
        public GameObject optionals;

        RectTransform decoRect;
        public ProfileItemElement profileDecoElement;
        public string currencyName = string.Empty;
        float posX = 0f, posY = 0f;
        float width = 300f, height = 300f;
        float angle = 0f;

        /// <summary>
        /// Instantitate로 생성되는 아이템 세팅
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="connectedElement">연결된 목록 element</param>
        public void NewProfileItem(JsonData __j, ProfileItemElement connectedElement)
        {
            string url = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL);
            string key = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY);
            string currency = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

            itemImage.SetDownloadURL(url, key, true);
            decoRect = GetComponent<RectTransform>();
            currencyName = currency;
            profileDecoElement = connectedElement;
        }

        /// <summary>
        /// 저장해두었던 프로필 아이템 세팅
        /// </summary>
        public void SetProfileItem(JsonData __j, ProfileItemElement connectElement = null)
        {
            decoRect = GetComponent<RectTransform>();
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            itemImage.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY));

            posX = float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_POS_X));
            posY = float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_POS_Y));

            width = float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_WIDTH));
            height = float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_HEIGHT));

            angle = float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ANGLE));

            decoRect.anchoredPosition = new Vector2(posX, posY);
            decoRect.sizeDelta = new Vector2(width, height);
            decoRect.eulerAngles = new Vector3(0, 0, angle);

            if (connectElement != null)
                profileDecoElement = connectElement;
        }

        #region DragAction

        public void OnBeginDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(true);
        }

        #endregion

        public void OnClickDeleteObject()
        {
            profileDecoElement.currentCount--;

            if (profileDecoElement.countText != null)
                profileDecoElement.SetCountText();

            Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ViewProfileDeco.OnDisableAllOptionals?.Invoke();
            optionals.SetActive(true);
        }

        public void DisableOptionals()
        {
            optionals.SetActive(false);
        }

        /// <summary>
        /// 꾸미기 object가 가지고 있는 정보를 JsonData로 만들어서 return해준다
        /// </summary>
        /// <param name="sortingOrder">정렬 순서</param>
        /// <returns></returns>
        public JsonData SaveJsonData(int sortingOrder)
        {
            JsonData data = new JsonData();

            data[LobbyConst.NODE_CURRENCY] = currencyName;
            data[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
            data[LobbyConst.NODE_POS_X] = decoRect.anchoredPosition.x;
            data[LobbyConst.NODE_POS_Y] = decoRect.anchoredPosition.y;
            data[LobbyConst.NODE_WIDTH] = decoRect.sizeDelta.x;
            data[LobbyConst.NODE_HEIGHT] = decoRect.sizeDelta.y;
            data[LobbyConst.NODE_ANGLE] = decoRect.eulerAngles.z;

            return data;
        }
    }
}