using UnityEngine;

namespace PIERStory
{
    public class ScreenEffectManager : MonoBehaviour
    {
        public static ScreenEffectManager main = null;

        public Camera mainCam;
        public Camera modelRenderCamC;
        public Camera generalCam;

        // zoom용 float값
        float originY = 0f;
        float mainCamOriginSize = 0f, modelCamOriginSize = 0f;

        public SpriteRenderer bgTint;
        public SpriteRenderer screenTint;

        private void Awake()
        {
            main = this;
        }

        private void Start()
        {
            mainCamOriginSize = mainCam.orthographicSize;
            modelCamOriginSize = modelRenderCamC.orthographicSize;
        }
    }
}