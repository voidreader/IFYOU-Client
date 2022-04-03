using System.Collections;
using System.Collections.Generic;
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
        float posX = 0f, posY = 0f;
        float width = 300f, height = 300f, angle = 0f;

        Vector2 originPos = Vector2.zero, startPos = Vector2.zero, dragPos = Vector2.zero;
        const float moveSpeed = 90f;        
        
        JsonData currencyData = null;
        
        /// <summary>
        /// 말풍선 만들기. 생성시 호출
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="connectedElement"></param>
        public void CreateBubble(JsonData __j, ProfileItemElement connectedElement) {
            
            // SystemManager.ShowNetworkLoading();
            
            currencyData = __j;
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            
            elementRect = GetComponent<RectTransform>();
            elementRect.anchoredPosition = new Vector2(0, 200);
            currencyElement = connectedElement;
            
            bubbleCtrl.SetProfileBubble(SystemManager.GetJsonNodeString(currencyData, "bubble_text"));
            elementRect.sizeDelta = new Vector2(bubbleCtrl.rtransform.sizeDelta.x + 80, bubbleCtrl.rtransform.sizeDelta.y + 80);
        }
        
        /// <summary>
        /// 생성한 말풍선 세팅
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="endCallback"></param>
        public void SetBubbleElement(JsonData __j, System.Action endCallback) {
            currencyData = __j;
            currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

            posX = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_POS_X);
            posY = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_POS_Y);

            // * 저장된 width, height 쓰지 않음 
            // width = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_WIDTH);
            // height = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_HEIGHT);

            angle = SystemManager.GetJsonNodeFloat(__j, LobbyConst.NODE_ANGLE);

            elementRect = GetComponent<RectTransform>();
            elementRect.anchoredPosition = new Vector2(posX, posY);
            // elementRect.sizeDelta = new Vector2(width, height);
            elementRect.eulerAngles = new Vector3(0, 0, angle);   
            
            // 말풍선 세팅 
            bubbleCtrl.SetProfileBubble(SystemManager.GetJsonNodeString(currencyData, "bubble_text"));
            elementRect.sizeDelta = new Vector2(bubbleCtrl.rtransform.sizeDelta.x + 80, bubbleCtrl.rtransform.sizeDelta.y + 80);
            
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
            OnClickObect();

            foreach (GameObject g in controlButtons)
                g.SetActive(false);

            startPos = eventData.position;
            originPos = elementRect.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragPos = eventData.position;

            elementRect.anchoredPosition = new Vector2(CalcMovePos(originPos.x, dragPos.x, startPos.x), CalcMovePos(originPos.y, dragPos.y, startPos.y));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in controlButtons)
                g.SetActive(true);
        }

        #endregion


        public void OnClickObect()
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
            data[LobbyConst.NODE_ANGLE] = elementRect.eulerAngles.z;

            return data;
        }

        float CalcMovePos(float origin, float drag, float start)
        {
            return origin + ((drag - start));
        }  
  
    }
}