using UnityEngine;
using UnityEngine.EventSystems;

namespace PIERStory
{
    public class SoundGaugeControl : MonoBehaviour, IPointerClickHandler
    {
        public RectTransform center;
        Vector2 centerPos;
        Vector2 standard = new Vector3(0, -180);
        Vector2 changed;
        float angle = 0f;

        public void OnPointerClick(PointerEventData eventData)
        {
            changed = new Vector2(eventData.position.x - centerPos.x, eventData.position.y - centerPos.y);
            angle = changed.x < 0 ? Vector2.Angle(changed, standard) : 360f - Vector2.Angle(changed, standard);

            ViewSoundDetail.OnMovePlayTime?.Invoke(angle);
        }

        void Start()
        {
            centerPos = RectTransformUtility.WorldToScreenPoint(Camera.main, center.position);
        }
    }
}