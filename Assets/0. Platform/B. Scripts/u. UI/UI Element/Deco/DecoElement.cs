using UnityEngine;
using UnityEngine.EventSystems;

namespace PIERStory
{
    public class DecoElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public GameObject[] buttons;
        public GameObject optionals;

        public void OnBeginDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (GameObject g in buttons)
                g.SetActive(true);
        }

        public void OnClickDeleteObject()
        {
            Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ViewProfileDeco.OnDisableAllOptionals?.Invoke();
            optionals.SetActive(true);
        }

        public void DisableOptionals()
        {
            optionals.SetActive(false);
        }
    }
}