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

            // 가로나 세로의 길이가 80 밑으로 내려가지 못하도록 조절
            if (decoObject.sizeDelta.x >= 80 || decoObject.sizeDelta.y >= 80)
                decoObject.sizeDelta = new Vector2(originSize.x + (startX - dragX) * sizeFactor, originSize.y + (startX - dragX) * sizeFactor);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(true);
        }
    }
}