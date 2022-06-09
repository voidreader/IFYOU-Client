using UnityEngine;
using UnityEngine.InputSystem;

namespace PIERStory
{
    public class TouchEffect : MonoBehaviour
    {
        public GameObject touchEffect;
        GameObject effect;
        Vector2 touchInPos = Vector2.one;
        Vector3 touchOutPos = Vector3.one;


        public void OnTouchScreen(InputAction.CallbackContext context)
        {
            try {
                if(context.action.phase == InputActionPhase.Canceled)
                {
                    touchOutPos = Camera.main.ScreenToWorldPoint(touchInPos);
                    touchOutPos = new Vector3(touchOutPos.x, touchOutPos.y, 0f);

                    effect = Instantiate(touchEffect, touchOutPos, Quaternion.identity);
                    Destroy(effect, 0.5f);
                }
            } catch{
                
            }
        }

        public void OnTouchPosition(InputAction.CallbackContext context)
        {
            touchInPos = context.ReadValue<Vector2>();
        }
    }
}

