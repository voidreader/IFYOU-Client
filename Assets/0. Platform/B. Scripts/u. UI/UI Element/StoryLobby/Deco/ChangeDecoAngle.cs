using UnityEngine;
using UnityEngine.EventSystems;

namespace PIERStory
{
    public class ChangeDecoAngle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GameObject[] buttons;
        public RectTransform decoObject;

        Vector3 startPos = Vector3.zero, lastPos = Vector3.forward;
        float angle = 0f;

        public void OnBeginDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(false);

            startPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            lastPos = eventData.position;
            angle = Mathf.Atan2(lastPos.y - startPos.y, lastPos.x - startPos.x) * Mathf.Rad2Deg;

            //decoObject.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            decoObject.eulerAngles = new Vector3(0, 0, angle - 90);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(true);
        }
    }
}