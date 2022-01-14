using UnityEngine;
using UnityEngine.EventSystems;

namespace PIERStory
{
    public class SoundHandlerControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform center;
        Vector2 centerPos;
        Vector2 standard = new Vector3(0, -180);
        Vector2 changed;
        float angle = 0f;

        private void Start()
        {
            centerPos = RectTransformUtility.WorldToScreenPoint(Camera.main, center.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            changed = new Vector2(eventData.position.x - centerPos.x, eventData.position.y - centerPos.y);
            angle = Vector2.Angle(changed, standard);

            ViewSoundDetail.OnMovePlayTime?.Invoke(angle);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            changed = new Vector2(eventData.position.x - centerPos.x, eventData.position.y - centerPos.y);
            angle = Vector2.Angle(changed, standard);

            ViewSoundDetail.OnMovePlayTime?.Invoke(angle);
        }
    }
}