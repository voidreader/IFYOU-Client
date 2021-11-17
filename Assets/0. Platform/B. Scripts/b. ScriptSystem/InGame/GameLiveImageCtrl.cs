using UnityEngine;
using UnityEngine.UI;

using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;

namespace PIERStory
{
    public class GameLiveImageCtrl : MonoBehaviour
    {
        public CubismRenderController cubismRender = null;
        public string modelType = GameConst.MODEL_TYPE_LIVE2D;
        public CubismModel model = null;
        public RawImage textureImage;

        bool isOnFadeIn = false;

        public float originScale = 0f;
        int frameCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            cubismRender = gameObject.GetComponent<CubismRenderController>();
        }

        
        void LateUpdate()
        {
            if (cubismRender == null)
                return;

            // 라이브 오브제의 경우 lateupdate를 돌 필요가 없다
            if (textureImage != null)
                return;

            if (!isOnFadeIn)
                return;

            // 0.2~4초 정도 간격을 주고 싶은데. 
            if (isOnFadeIn)
                frameCount++;

            if (frameCount <= 2)
                return;


            if (isOnFadeIn && frameCount > 2 && cubismRender.Opacity < 0.4f)
            {
                cubismRender.Opacity = 0.4f;
                return;
            }

            cubismRender.Opacity += 0.12f;

            if (cubismRender.Opacity >= 1)
            {
                isOnFadeIn = false;
                cubismRender.Opacity = 1;
            }
        }

        public void SetModel(CubismModel __model)
        {
            model = __model;
        }


        public void SetParentRawImage()
        {
            if (textureImage == null)
                textureImage = GameManager.main.liveObjectTexture;
        }


        public void ActivateModel(bool __instant = false)
        {
            if (cubismRender == null)
                cubismRender = GetComponent<CubismRenderController>();

            SetParentRawImage();

            if (textureImage != null)
                cubismRender.Opacity = 1f;
            else
            {
                cubismRender.Opacity = 0f;
                frameCount = 0;
                isOnFadeIn = true;
            }

            gameObject.SetActive(true);
        }

        public void HideModel()
        {
            isOnFadeIn = false;
            gameObject.SetActive(false);

            transform.localScale = new Vector3(originScale, originScale, 1f);
            if (textureImage != null)
                textureImage.color = new Color(textureImage.color.r, textureImage.color.g, textureImage.color.b, 1f);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}