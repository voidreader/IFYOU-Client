using UnityEngine;
using UnityEngine.EventSystems;

using LitJson;

namespace PIERStory
{
    public class StickerElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public ImageRequireDownload stickerImage;
        public GameObject[] controlButtons;
        public GameObject controlBox;

        RectTransform elementRect;
        public ProfileItemElement currencyElement;
        public string currencyName = string.Empty;
        float posX = 0f, posY = 0f;
        float width = 300f, height = 300f, angle = 0f;

        /// <summary>
        /// 스티커 생성시 호출
        /// </summary>
        public void CreateSticker(JsonData __j, ProfileItemElement connectedElement)
        {
            string url = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL);
            string key = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY);
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

            SystemManager.ShowNetworkLoading();
            stickerImage.OnDownloadImage = SystemManager.HideNetworkLoading;
            stickerImage.SetDownloadURL(url, key, true);
            elementRect = GetComponent<RectTransform>();
            currencyElement = connectedElement;
        }

        /// <summary>
        /// 생성한 스티커 세팅
        /// </summary>
        /// <param name="__j"></param>
        public void SetStickerElement(JsonData __j)
        {
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            stickerImage.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY));

            posX = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_POS_X);
            posY = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_POS_Y);

            width = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_WIDTH);
            height = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_HEIGHT);

            angle = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_ANGLE);

            elementRect = GetComponent<RectTransform>();
            elementRect.anchoredPosition = new Vector2(posX, posY);
            elementRect.sizeDelta = new Vector2(width, height);
            elementRect.eulerAngles = new Vector3(0, 0, angle);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            ViewStoryLobby.OnDisableAllOptionals?.Invoke();
            controlBox.SetActive(true);
        }


        public void DisableControlBox()
        {
            controlBox.SetActive(false);
        }

        #region Drag Action

        public void OnBeginDrag(PointerEventData eventData)
        {
            ViewStoryLobby.OnDisableAllOptionals?.Invoke();
            controlBox.SetActive(true);

            foreach (GameObject g in controlButtons)
                g.SetActive(false);
        }


        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in controlButtons)
                g.SetActive(true);
        }

        #endregion

        public JsonData StickerJsonData(int sortingOrder)
        {
            JsonData data = new JsonData();

            data[LobbyConst.NODE_CURRENCY] = currencyName;
            data[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
            data[LobbyConst.NODE_POS_X] = elementRect.anchoredPosition.x;
            data[LobbyConst.NODE_POS_Y] = elementRect.anchoredPosition.y;
            data[LobbyConst.NODE_WIDTH] = elementRect.sizeDelta.x;
            data[LobbyConst.NODE_HEIGHT] = elementRect.sizeDelta.y;
            data[LobbyConst.NODE_ANGLE] = elementRect.eulerAngles.z;

            return data;
        }

    }
}