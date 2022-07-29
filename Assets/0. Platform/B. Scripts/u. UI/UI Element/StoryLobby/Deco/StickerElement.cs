using UnityEngine;
using UnityEngine.EventSystems;

using LitJson;

namespace PIERStory
{
    public class StickerElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ImageRequireDownload stickerImage;
        public GameObject[] controlButtons;
        public GameObject controlBox;

        RectTransform elementRect;
        public ProfileItemElement currencyElement;

        public string currencyName = string.Empty;
        float posX = 0f, posY = 0f;
        float width = 300f, height = 300f, angle = 0f;

        Vector2 startPos = Vector2.zero, dragPos = Vector2.zero;

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
        public void SetStickerElement(JsonData __j, System.Action endCallback)
        {
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            stickerImage.OnDownloadImage = endCallback;
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
        

        /// <summary>
        /// 제어 박스 비활성화
        /// </summary>
        public void DisableControlBox()
        {
            controlBox.SetActive(false);
        }

        /// <summary>
        /// 삭제
        /// </summary>
        public void OnClickDeleteObject()
        {
            currencyElement.currentCount--;
            currencyElement.SetCountText();

            Destroy(gameObject);
        }


        #region Drag Action

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnClickObject();

            foreach (GameObject g in controlButtons)
                g.SetActive(false);

            startPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragPos = eventData.position;
            Vector2 calcVector = dragPos - startPos;

            Vector2 newPos = elementRect.anchoredPosition + calcVector;
            Vector2 oldPos = elementRect.anchoredPosition;

            elementRect.anchoredPosition = newPos;

            if (!IsRectInsideSceen())
                elementRect.anchoredPosition = oldPos;

            startPos = dragPos;

            //elementRect.anchoredPosition = new Vector2(CalcMovePos(originPos.x, dragPos.x, startPos.x), CalcMovePos(originPos.y, dragPos.y, startPos.y));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in controlButtons)
                g.SetActive(true);
        }

        #endregion


        public void OnClickObject()
        {
            ViewStoryLobby.OnDisableAllOptionals?.Invoke();
            controlBox.SetActive(true);
        }


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

        bool IsRectInsideSceen()
        {
            bool isInside = false;

            Vector3[] corners = new Vector3[4];
            elementRect.GetWorldCorners(corners);

            int visableCornders = 0;
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            for(int i=0;i<corners.Length;i++)
                corners[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[i]);

            foreach (Vector3 corner in corners)
            {
                if (rect.Contains(corner))
                    visableCornders++;
            }

            if (visableCornders == 4)
                isInside = true;

            return isInside;
        }
    }
}