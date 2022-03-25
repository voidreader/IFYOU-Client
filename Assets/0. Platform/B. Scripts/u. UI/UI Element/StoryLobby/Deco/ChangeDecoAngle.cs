using UnityEngine;
using UnityEngine.EventSystems;

namespace PIERStory
{
    public class ChangeDecoAngle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GameObject[] buttons;
        public RectTransform decoObject;

        Vector2 screenPos;
        float angleOffset = 0f, angle = 0f;

        public void OnBeginDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(false);

            screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, decoObject.position);
            Vector2 v2 = eventData.position - screenPos;
            angleOffset = (Mathf.Atan2(decoObject.right.y, decoObject.right.x) - Mathf.Atan2(v2.y, v2.x)) * Mathf.Rad2Deg;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 v2 = eventData.position - screenPos;
            angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
            decoObject.eulerAngles = new Vector3(0, 0, angle + angleOffset);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(true);
        }
    }
}