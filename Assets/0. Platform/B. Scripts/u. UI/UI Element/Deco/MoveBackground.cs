using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PIERStory
{
    public class MoveBackground : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform bgObject;
        public string currencyName = string.Empty;
        float movableWidth = 0f;

        float startX = 0f, dragX = 0f, originX = 0f, posX;

        private void OnEnable()
        {
            movableWidth = (bgObject.sizeDelta.x - GetComponent<RectTransform>().rect.width) * 0.5f;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startX = eventData.position.x;
            originX = bgObject.anchoredPosition.x;
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragX = eventData.position.x;
            posX = originX + (dragX - startX);

            if (posX > -movableWidth && posX < movableWidth)
                bgObject.anchoredPosition = new Vector2(posX, 0f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (posX <= -movableWidth)
                bgObject.anchoredPosition = new Vector2(-movableWidth, 0f);
            else if(posX >= movableWidth)
                bgObject.anchoredPosition = new Vector2(movableWidth, 0f);
        }
    }
}