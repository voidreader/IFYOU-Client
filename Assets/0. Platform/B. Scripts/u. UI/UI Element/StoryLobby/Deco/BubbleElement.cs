using UnityEngine;
using UnityEngine.EventSystems;

using LitJson;

namespace PIERStory {
    public class BubbleElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // * StickerElement 따라함 
        public GameBubbleCtrl bubbleCtrl; // 인겜에서 쓰는 말풍선..
        
        public GameObject[] controlButtons;
        public GameObject controlBox;
        
        RectTransform elementRect;
        public ProfileItemElement currencyElement;

        public string currencyName = string.Empty;
        float posX = 0f, posY = 0f, angle = 0f;

        Vector2 startPos = Vector2.zero, dragPos = Vector2.zero;
        
        JsonData currencyData = null;

        /// <summary>
        /// 말풍선 만들기. 생성시 호출
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="connectedElement"></param>
        public void CreateBubble(JsonData __j, ProfileItemElement connectedElement)
        {
            currencyData = __j;
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

            elementRect = GetComponent<RectTransform>();
            elementRect.anchoredPosition = new Vector2(0, 720);
            currencyElement = connectedElement;

            bubbleCtrl.SetProfileBubble(SystemManager.GetJsonNodeString(currencyData, "bubble_text"));
            elementRect.sizeDelta = new Vector2(bubbleCtrl.rtransform.sizeDelta.x + 80, bubbleCtrl.rtransform.sizeDelta.y + 80);
        }

        /// <summary>
        /// 생성한 말풍선 세팅
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="endCallback"></param>
        public void SetBubbleElement(JsonData __j, System.Action endCallback)
        {
            currencyData = __j;
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

            posX = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_POS_X);
            posY = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_POS_Y);

            angle = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_ANGLE);

            elementRect = GetComponent<RectTransform>();
            elementRect.anchoredPosition = new Vector2(posX, posY);

            BubbleManager.main.SetLobbyFakeBubbles(SystemManager.GetJsonNodeString(__j, "origin_name"));

            // 말풍선 세팅 
            bubbleCtrl.SetProfileBubble(SystemManager.GetJsonNodeString(currencyData, "origin_name"), endCallback);
            elementRect.sizeDelta = new Vector2(bubbleCtrl.rtransform.sizeDelta.x + 80, bubbleCtrl.rtransform.sizeDelta.y + 80);

            bubbleCtrl.rtransform.Rotate(new Vector3(0, angle, 0));
            bubbleCtrl.textContents.transform.Rotate(new Vector3(0, angle, 0));
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

            Destroy(gameObject);
        }

        /// <summary>
        /// 말풍선 뒤집기
        /// </summary>
        public void OnClickFlipScriptBubble()
        {
            bubbleCtrl.rtransform.Rotate(new Vector3(0, 180, 0));
            bubbleCtrl.textContents.transform.Rotate(new Vector3(0, 180, 0));
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

            dragPos = eventData.position;
            Vector2 calcVector = dragPos - startPos;

            Vector2 newPos = elementRect.anchoredPosition + calcVector;
            Vector2 oldPos = elementRect.anchoredPosition;

            elementRect.anchoredPosition = newPos;

            if (!IsRectInsideSceen())
                elementRect.anchoredPosition = oldPos;

            startPos = dragPos;
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


        /// <summary>
        /// 저장용 데이터 생성 
        /// </summary>
        /// <param name="sortingOrder"></param>
        /// <returns></returns>
        public JsonData BubbleJsonData(int sortingOrder)
        {
            JsonData data = new JsonData();

            data[LobbyConst.NODE_CURRENCY] = currencyName;
            data[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
            data[LobbyConst.NODE_POS_X] = elementRect.anchoredPosition.x;
            data[LobbyConst.NODE_POS_Y] = elementRect.anchoredPosition.y;
            data[LobbyConst.NODE_WIDTH] = elementRect.sizeDelta.x;
            data[LobbyConst.NODE_HEIGHT] = elementRect.sizeDelta.y;
            data[LobbyConst.NODE_ANGLE] = bubbleCtrl.rtransform.eulerAngles.y;

            return data;
        }

        /// <summary>
        /// 사각박스가 액정(화면) 밖으로 나가는지 체크해주는 함수
        /// </summary>
        /// <returns></returns>
        bool IsRectInsideSceen()
        {
            bool isInside = false;

            Vector3[] corners = new Vector3[4];
            elementRect.GetWorldCorners(corners);

            int visableCornders = 0;
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            for (int i = 0; i < corners.Length; i++)
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