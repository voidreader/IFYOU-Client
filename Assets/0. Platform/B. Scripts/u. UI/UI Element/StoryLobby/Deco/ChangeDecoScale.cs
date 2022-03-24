using UnityEngine;
using UnityEngine.EventSystems;

namespace PIERStory
{
    public class ChangeDecoScale : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GameObject[] buttons;
        public RectTransform decoObject;

        const float sizeFactor = 3.8f;
        float startX = 0f, dragX = 0f;
        Vector2 originSize;

        public void OnBeginDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(false);

            startX = eventData.position.x;
            originSize = decoObject.sizeDelta;
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragX = eventData.position.x;

            float calcSize = (startX - dragX) * sizeFactor;

            // 가로나 세로의 길이가 80 밑으로, 540 위로 가지 못하게 막음
            if (decoObject.sizeDelta.x + calcSize >= 80 && decoObject.sizeDelta.y + calcSize >= 80 && decoObject.sizeDelta.x + calcSize <=540 && decoObject.sizeDelta.y + calcSize <= 540)
                decoObject.sizeDelta = new Vector2(originSize.x + calcSize, originSize.y + calcSize);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(true);
        }
    }
}