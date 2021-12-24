using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PIERStory
{
    public class MoveBackground : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform bgObject;
        float movableWidth = 0f;

        private void OnEnable()
        {
            movableWidth = (bgObject.sizeDelta.x - GetComponent<RectTransform>().sizeDelta.x) * 0.5f;
            Debug.Log("움직일 수 있는 여분 = " + movableWidth);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}