using System.Collections;
using UnityEngine;

using DG.Tweening;

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

        #region 화면 연출 screen effect

        #region 틴트

        /// <summary>
        /// 화면 전체 틴트
        /// </summary>
        /// <param name="__color">색상</param>
        /// <param name="__activeTime">활성시간</param>
        public void StartScreenEffectTint(Color __color, float __activeTime)
        {
            screenTint.color = __color;

            if (__activeTime > 0)
                StartCoroutine(RoutineScreenTint(screenTint, __activeTime));
            else
                screenTint.gameObject.SetActive(true); // 즉각 실행 
        }

        /// <summary>
        /// 배경 틴트
        /// </summary>
        public void StartBackgroundTint(Color __color, float __activeTime)
        {
            bgTint.color = __color;

            if (__activeTime > 0)
                StartCoroutine(RoutineScreenTint(bgTint, __activeTime));
            else
                bgTint.gameObject.SetActive(true); // 즉각 실행 
        }

        /// <summary>
        /// 캐릭터 틴트
        /// </summary>
        public void StartCharacterTint(Color __color, float __activeTime)
        {
            ViewGame.main.modelRenders[1].color = new Color(__color.r, __color.g, __color.b);

            if (__activeTime > 0)
                StartCoroutine(RoutineStayTintState(__activeTime));
        }


        IEnumerator RoutineStayTintState(float __activeTime)
        {
            yield return new WaitForSeconds(__activeTime);
            ViewGame.main.modelRenders[1].color = Color.white;
        }

        IEnumerator RoutineScreenTint(SpriteRenderer __sp, float __activeTime)
        {
            __sp.gameObject.SetActive(true);
            yield return new WaitForSeconds(__activeTime);
            __sp.gameObject.SetActive(false);
        }

        #endregion

        /// <summary>
        /// 카메라에 연결되는 스크린 이펙트 
        /// </summary>
        /// <param name="__effect">연출 명령어</param>
        /// <param name="__params">파라매터들</param>
        public void StartScreenEffectCamera(string __effect, string[] __params)
        {
            switch (__effect)
            {
                case GameConst.KR_SCREEN_EFFECT_ZOOMIN:

                    #region 줌인
                    const float zoomAnimTime = 0.7f;
                    int zoomLevel = 1;

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, "단계", ref zoomLevel);

                    switch (zoomLevel)
                    {
                        case 1:
                            mainCam.DOOrthoSize(mainCamOriginSize - 1f, zoomAnimTime);
                            modelRenderCamC.DOOrthoSize(modelCamOriginSize - 1f, zoomAnimTime);
                            if (modelRenderCamC.transform.position.y != originY)
                                modelRenderCamC.transform.DOMoveY(originY, 1f);
                            break;
                        case 2:
                            mainCam.DOOrthoSize(mainCamOriginSize - 1.5f, zoomAnimTime);
                            modelRenderCamC.DOOrthoSize(modelCamOriginSize - 1.5f, zoomAnimTime);
                            modelRenderCamC.transform.DOMoveY(originY + 0.4f, zoomAnimTime);
                            break;
                        case 3:
                            mainCam.DOOrthoSize(mainCamOriginSize - 2f, zoomAnimTime);
                            modelRenderCamC.DOOrthoSize(modelCamOriginSize - 2f, zoomAnimTime);
                            modelRenderCamC.transform.DOMoveY(originY + 0.8f, zoomAnimTime);
                            break;
                    }
                    break;

                #endregion

                case GameConst.KR_SCREEN_EFFECT_ZOOMOUT:

                    mainCam.DOOrthoSize(mainCamOriginSize, zoomAnimTime);
                    modelRenderCamC.DOOrthoSize(modelCamOriginSize, zoomAnimTime);
                    modelRenderCamC.transform.DOMoveY(originY, zoomAnimTime);
                    break;
            }
        }

        #region 카메라 플래시

        /// <summary>
        /// 플래시 이펙트 효과 연출
        /// </summary>
        /// <param name="__params">시간 파라미터값</param>
        public void DirectiveFlash(string[] __params)
        {
            // 연출 시간 기본값 = 0.1초
            float animTime = 0.1f;

            if (__params != null)
                ScriptRow.GetParam<float>(__params, "시간", ref animTime);

            Sequence flash = DOTween.Sequence();
            ViewGame.main.fadeImage.color = new Color(1, 1, 1, 0f);
            ViewGame.main.fadeImage.gameObject.SetActive(true);
            flash.Append(ViewGame.main.fadeImage.DOFade(1f, animTime));
            flash.Append(ViewGame.main.fadeImage.DOFade(0f, animTime));
            flash.OnComplete(() => { ViewGame.main.fadeImage.gameObject.SetActive(false); });
        }

        #endregion

        #endregion

        #region 화면연출 제거

        public void RemoveGeneralEffect(string __effect)
        {
            switch (__effect)
            {
                case GameConst.KR_SCREEN_EFFECT_TINT:
                    screenTint.gameObject.SetActive(false);
                    break;
                case GameConst.KR_SCREEN_EFFECT_TINT_BG:
                    bgTint.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_TINT_CH:
                    ViewGame.main.modelRenders[1].color = Color.white;
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLING:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_FIRE:
                    //RemoveFireEffect();
                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:
                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                    
                    break;
            }
        }

        /// <summary>
        /// 카메라 이펙트 제거 
        /// </summary>
        public void RemoveCameraEffect(string __effect)
        {
            switch (__effect)
            {
                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_BG:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_CH:
                    ViewGame.main.modelRenders[1].material.DisableKeyword("GREYSCALE_ON");
                    break;

                case GameConst.KR_SCREEN_EFFECT_BROKEN:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_ANOMALY:
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:
                    
                    break;
                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:
                    
                    break;
                case GameConst.KR_SCREEN_EFFECT_FOCUS:
                    
                    break;
                case GameConst.KR_SCREEN_EFFECT_HEAVYRAIN:
                    
                    break;
                case GameConst.KR_SCREEN_EFFECT_RAIN:
                    
                    break;
                case GameConst.KR_SCREEN_EFFECT_HEAVYSNOW:
                    
                    break;


                case GameConst.KR_SCREEN_EFFECT_ZOOMIN:
                    mainCam.orthographicSize = mainCamOriginSize;
                    modelRenderCamC.orthographicSize = modelCamOriginSize;
                    modelRenderCamC.transform.position = new Vector3(modelRenderCamC.transform.position.x, 0, modelRenderCamC.transform.position.z);
                    break;

                case GameConst.KR_SCREEN_EFFECT_DIZZY:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_GLITCH:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_GLITCH_SCREEN:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:
                    
                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:
                    
                    break;
            }
        }


        /// <summary>
        /// 모든 화면 연출 제거
        /// </summary>
        public void RemoveAllScreenEffect()
        {
            for (int i = 0; i < RowActionScreenEffect.ListGeneralEffect.Count; i++)
                RemoveGeneralEffect(RowActionScreenEffect.ListGeneralEffect[i]);
            
            for (int i = 0; i < RowActionScreenEffect.ListCameraEffect.Count; i++)
                RemoveCameraEffect(RowActionScreenEffect.ListCameraEffect[i]);
        }

        #endregion
    }
}