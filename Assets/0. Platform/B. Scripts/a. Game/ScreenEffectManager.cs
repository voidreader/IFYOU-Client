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

        #region 카메라 오브젝트 component에 스크립트로 제어하는 연출

        CameraFilterPack_Color_GrayScale bgGrayScale, screenGrayScale;
        CameraFilterPack_Blur_Blurry blur;
        CameraFilterPack_TV_Artefact glitch, screenGlitch;
        CameraFilterPack_TV_Old_Movie_2 reminisce;
        CameraFilterPack_Broken_Screen brokenScreen;

        #endregion

        #region 파티클로 제작된 연출

        [Space(20)][Header("Particle system effect")]
        [Tooltip("불 파티클")] public ParticleSystem fire;
        [Tooltip("반짝이 파티클")] public ParticleSystem glitter;
        [Tooltip("원형빛")] public ParticleSystem bokeh;
        [Tooltip("육각형빛")] public ParticleSystem hexagonLight;       // 안옮겼음
        [Tooltip("배경만 안개")] public ParticleSystem bgFog;
        [Tooltip("스크린 안개")] public ParticleSystem screenFog;
        [Tooltip("폭우")] public ParticleSystem heavyRain;
        [Tooltip("비")] public ParticleSystem rain;
        [Tooltip("폭설")] public ParticleSystem heavySnow;
        [Tooltip("눈")] public ParticleSystem snow;
        [Tooltip("렌즈플레어")] public ParticleSystem lensFlare;         // 타입 1개만 있음 아직
        [Tooltip("집중선")] public ParticleSystem radiLine;
        [Tooltip("출혈 타입1")] public ParticleSystem bleeding_1;
        [Tooltip("출혈 타입2")] public ParticleSystem bleeding_2;
        [Tooltip("출혈 타입3")] public ParticleSystem bleeding_3;
        [Tooltip("둔기 타격")] public ParticleSystem bluntStrike;
        [Tooltip("검 베기")] public ParticleSystem blade;
        [Tooltip("비눗방울")] public ParticleSystem bubble;
        [Tooltip("회상, 밝음")] public ParticleSystem reminisceLight;
        [Tooltip("신비로운 경계라인")] public ParticleSystem waveLine;

        #endregion

        // zoom용 float값
        float originY = 0f;
        float mainCamOriginSize = 0f, modelCamOriginSize = 0f;

        [Space(20)]
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

            bgGrayScale = mainCam.GetComponent<CameraFilterPack_Color_GrayScale>();
            screenGrayScale = generalCam.GetComponent<CameraFilterPack_Color_GrayScale>();

            blur = generalCam.GetComponent<CameraFilterPack_Blur_Blurry>();

            glitch = mainCam.GetComponent<CameraFilterPack_TV_Artefact>();
            screenGlitch = generalCam.GetComponent<CameraFilterPack_TV_Artefact>();

            brokenScreen = generalCam.GetComponent<CameraFilterPack_Broken_Screen>();
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

        #region 흑백 Coroutine

        IEnumerator GrayScaleEffectAnimation(CameraFilterPack_Color_GrayScale cam, float fadeValue, float __animTime)
        {
            cam._Fade = fadeValue;

            float animTime = 0f;

            if(fadeValue == 0)
            {
                while(cam._Fade < 1)
                {
                    animTime += Time.deltaTime / __animTime;
                    cam._Fade = Mathf.Lerp(0, 1, animTime);
                    yield return null;
                }
            }
            else
            {
                while (cam._Fade > 0)
                {
                    animTime += Time.deltaTime / __animTime;
                    cam._Fade = Mathf.Lerp(1, 0, animTime);
                    yield return null;
                }
            }
        }

        void SetGrayScaleFade(string[] __params, CameraFilterPack_Color_GrayScale grayScale)
        {
            string optionalValue = string.Empty;
            float animTime = 2f;

            ScriptRow.GetParam<string>(__params, GameConst.KR_PARAM_VALUE_ANIMATION, ref optionalValue);
            ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_TIME, ref animTime);

            if (optionalValue.Equals(GameConst.KR_SCREEN_EFFECT_GRAYSCALE))
                StartCoroutine(GrayScaleEffectAnimation(grayScale, 0f, animTime));
            else
                StartCoroutine(GrayScaleEffectAnimation(grayScale, 1f, animTime));
        }

        #endregion

        #region 흔들기 Callback

        void OnCompleteShake()
        {
            generalCam.transform.position = new Vector3(0, 0, -10);
            mainCam.transform.position = new Vector3(0, 0, -10);
        }

        #endregion

        #region 블러 Coroutine

        IEnumerator BlurAnimation(float blurAmount, float blurTime)
        {
            float amount = 0f;
            float lastTime = 0f;

            while (amount < blurAmount)
            {
                lastTime += Time.deltaTime / (blurTime * 0.25f);
                amount = Mathf.Lerp(0, blurAmount, lastTime);
                blur.Amount = amount;
                yield return null;
            }

            yield return new WaitForSeconds(blurTime * 0.5f);

            lastTime = 0f;

            while (amount > 0)
            {
                lastTime += Time.deltaTime / (blurTime * 0.25f);
                amount = Mathf.Lerp(blurAmount, 0, lastTime);
                blur.Amount = amount;
                yield return null;
            }

            blur.enabled = false;
        }

        #endregion

        #region 글리치 function

        void GlitchSetting(CameraFilterPack_TV_Artefact glitch, string[] __params)
        {
            int glitchType = 2;

            if (__params != null)
                ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_LEVEL, ref glitchType);

            switch (glitchType)
            {
                case 1:
                    glitch.Noise = -5f;
                    break;
                case 2:
                default:
                    glitch.Noise = 0f;
                    break;
                case 3:
                    glitch.Noise = 5f;
                    break;
            }

            glitch.enabled = true;
        }

        #endregion

        /// <summary>
        /// 카메라에 연결되는 화면 연출
        /// </summary>
        /// <param name="__effect">연출 명령어</param>
        /// <param name="__params">파라매터들</param>
        public void StartScreenEffectCamera(string __effect, string[] __params)
        {
            switch (__effect)
            {
                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE:
                    screenGrayScale.enabled = true;

                    if (__params != null)
                        SetGrayScaleFade(__params, screenGrayScale);

                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_BG:
                    bgGrayScale.enabled = true;

                    if (__params != null)
                        SetGrayScaleFade(__params, bgGrayScale);

                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_CH:

                    const string GREYSCALE_ON = "GREYSCALE_ON";
                    const string GREYSCALE_BLEND = "_GreyscaleBlend";

                    if (__params != null)
                    {
                        string optionalValue = string.Empty;
                        float animTime = 2f;

                        ScriptRow.GetParam<string>(__params, GameConst.KR_PARAM_VALUE_ANIMATION, ref optionalValue);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_TIME, ref animTime);

                        if (optionalValue.Equals(GameConst.KR_SCREEN_EFFECT_GRAYSCALE))
                        {
                            ViewGame.main.modelRenders[1].material.EnableKeyword(GREYSCALE_ON);
                            ViewGame.main.modelRenders[1].material.SetFloat(GREYSCALE_BLEND, 0f);
                            ViewGame.main.modelRenders[1].material.DOFloat(1f, GREYSCALE_BLEND, animTime);
                        }
                        else
                            ViewGame.main.modelRenders[1].material.DOFloat(0f, GREYSCALE_BLEND, animTime).OnComplete(() => ViewGame.main.modelRenders[1].material.DisableKeyword(GREYSCALE_ON));
                    }
                    else
                    {
                        ViewGame.main.modelRenders[1].material.EnableKeyword(GREYSCALE_ON);

                        if (ViewGame.main.modelRenders[1].material.GetFloat(GREYSCALE_BLEND) < 1f)
                            ViewGame.main.modelRenders[1].material.SetFloat(GREYSCALE_BLEND, 1f);
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_SHAKE:

                    float shakeTime = 0.5f;
                    float shakeStrength = 0.5f;

                    if(__params != null)
                    {
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_TIME, ref shakeTime);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_STRENGTH, ref shakeStrength);
                    }

                    shakeTime = Mathf.Clamp(shakeTime, 0f, 3f);
                    shakeStrength = Mathf.Clamp(shakeStrength, 0f, 3f);

                    generalCam.transform.DOShakePosition(shakeTime, 0.6f, (int)(shakeStrength * 30), 90).OnComplete(OnCompleteShake);
                    mainCam.transform.DOShakePosition(shakeTime, 0.2f, (int)(shakeStrength * 30), 90);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BROKEN:

                    float brokenShadow = 1f;
                    float fadeForce = 1f;

                    bool isRecover = false, isAnim = false;

                    if(__params!=null)
                    {
                        ScriptRow.GetParam<float>(__params, "그림자", ref brokenShadow);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_FORCE, ref fadeForce);

                        for(int i=0;i<__params.Length;i++)
                        {
                            if (__params[i].Contains(GameConst.KR_PARAM_VALUE_ANIMATION))
                                isAnim = true;

                            if (__params[i].Contains("복원"))
                                isRecover = true;
                        }
                    }

                    brokenScreen.Init(fadeForce, brokenShadow);

                    if (isAnim)
                        brokenScreen.SetAnim(isRecover ? "복원" : "브레이크");

                    brokenScreen.enabled = true;

                    break;

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

                case GameConst.KR_SCREEN_EFFECT_BLUR:

                    int blurLevel = 2;
                    float blurTime = 4f;

                    if(__params != null)
                    {
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_LEVEL, ref blurLevel);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_TIME, ref blurTime);
                    }

                    switch (blurLevel)
                    {
                        case 1:
                            blur.Amount = 1.5f;
                            break;

                        case 2:
                        default:
                            blur.Amount = 2f;
                            break;
                        case 3:
                            blur.Amount = 2.5f;
                            break;
                    }

                    blur.enabled = true;
                    StartCoroutine(BlurAnimation(blur.Amount, blurTime));

                    break;

                case GameConst.KR_SCREEN_EFFECT_GLITCH:
                    GlitchSetting(glitch, __params);
                    break;

                case GameConst.KR_SCREEN_EFFECT_GLITCH_SCREEN:
                    GlitchSetting(screenGlitch, __params);
                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:

                    int reminisceForce = 1;

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_LEVEL, ref reminisceForce);

                    switch (reminisceForce)
                    {
                        case 1:
                            reminisce.FramePerSecond = 15f;
                            break;
                        case 2:
                        default:
                            reminisce.FramePerSecond = 20f;
                            break;
                        case 3:
                            reminisce.FramePerSecond = 25f;
                            break;
                    }

                    reminisce.enabled = true;

                    break;
            }
        }

        /// <summary>
        /// 파티클 시스템으로 하는 연출
        /// </summary>
        /// <param name="__effect">연출 명령어</param>
        /// <param name="__params">파라매터들</param>
        public void StartParticleEffect(string __effect, string[] __params)
        {
            switch (__effect)
            {
                case GameConst.KR_SCREEN_EFFECT_FIRE:

                    int fireLevel = 3;

                    if(__params != null)
                        ScriptRow.GetParam<int>(__params, "분포", ref fireLevel);

                    fireLevel = Mathf.Clamp(fireLevel, 1, 5);

                    switch (fireLevel)
                    {
                        case 1:
                            fire.gameObject.transform.position = new Vector3(1, -6, 1);
                            break;

                        case 2:
                            fire.gameObject.transform.position = new Vector3(1, -5, 1);
                            break;

                        case 3:
                            fire.gameObject.transform.position = new Vector3(1, -4, 1);
                            break;

                        case 4:
                            fire.gameObject.transform.position = new Vector3(1, -2, 1);
                            break;

                        case 5:
                            fire.gameObject.transform.position = new Vector3(1, 0, 1);
                            break;
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_BLING:

                    glitter.gameObject.SetActive(true);
                    // 반짝이는 분포가 있어서 파티클 갯수를 조절해줘야한다

                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:
                    bokeh.gameObject.SetActive(true);
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