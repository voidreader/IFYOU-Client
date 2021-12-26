using UnityEngine;

namespace PIERStory
{
    public class TouchEffect : MonoBehaviour
    {
        public GameObject touchEffect;
        GameObject effect;
        Vector3 touchInPos = Vector3.forward;
        Vector3 touchOutPos = Vector3.back;

        private void Update()
        {
            if(Application.isEditor)
            {
                if (Input.GetMouseButtonDown(0))
                    touchInPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if(Input.GetMouseButtonUp(0))
                {
                    touchOutPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    CheckOutPosition();
                }
            }
            else
            {
                if (Input.touchCount < 1)
                    return;

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                    touchInPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                if(Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    touchOutPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    CheckOutPosition();
                }
            }
        }

        void CheckOutPosition()
        {
            if (touchInPos.x >= touchOutPos.x - 0.4f && touchInPos.x <= touchOutPos.x + 0.4f &&
                touchInPos.y >= touchOutPos.y - 0.4f && touchInPos.y <= touchOutPos.y + 0.4f)
                CreateTouchEffect(touchOutPos);
        }

        void CreateTouchEffect(Vector3 touchPos)
        {
            Vector3 touchedPos = touchPos;
            touchInPos = Vector3.forward;
            touchOutPos = Vector3.back;
            touchedPos = new Vector3(touchedPos.x, touchedPos.y, 0);
            effect = Instantiate(touchEffect, touchedPos, Quaternion.identity);

            Destroy(effect, 0.5f);
        }
    }
}
