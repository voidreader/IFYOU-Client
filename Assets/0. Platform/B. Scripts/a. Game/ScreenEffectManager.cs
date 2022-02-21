using System.Collections;
using UnityEngine;

using DG.Tweening;
using AkilliMum.Standard.D2WeatherEffects;

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
        CameraFilterPack_Blur_Bloom bloom;
        CameraFilterPack_TV_Old_Movie_2 reminisce;
        CameraFilterPack_Broken_Screen brokenScreen;
        CameraFilterPack_Distortion_Dream2 dizzy;
        CameraFilterPack_3D_Snow heavySnow;
        CameraFilterPack_Atmosphere_Rain heavyRain;
        D2RainsPE rain;
        

        #endregion

        #region 파티클로 제작된 연출

        [Space(20)][Header("Particle system effect")]
        [Tooltip("불 파티클")] public ParticleSystem fire;
        [Tooltip("반짝이 파티클")] public ParticleSystem glitter;
        ParticleSystem[] glitters;
        [Tooltip("원형빛1")] public ParticleSystem bokeh1;
        [Tooltip("원형빛2")] public ParticleSystem bokeh2;
        [Tooltip("육각형빛")] public ParticleSystem hexagonLight;
        [Tooltip("배경만 안개")] public ParticleSystem bgFog;
        ParticleSystem[] bgFogs;
        [Tooltip("스크린 안개")] public ParticleSystem screenFog;
        ParticleSystem[] screenFogs;
        [Tooltip("폭우")] public ParticleSystem heavyRainParticle;
        [Tooltip("비")] public ParticleSystem rainParticle;
        [Tooltip("폭설")] public ParticleSystem heavySnowParticle;
        [Tooltip("눈")] public ParticleSystem snow;
        public ParticleSystem[] snowParticles;
        [Tooltip("렌즈플레어1")] public ParticleSystem lensFlare1;
        public ParticleSystem flareGlow1;
        [Tooltip("렌즈플레어2")] public ParticleSystem lensFlare2;
        public ParticleSystem flareGlow2;
        [Tooltip("렌즈플레어3")] public ParticleSystem lensFlare3;
        public ParticleSystem flareGlow3;
        [Tooltip("렌즈플레어에서 사용되는 빛알갱이")] public ParticleSystem lightDust;
        [Tooltip("집중선")] public ParticleSystem radiLine;
        public ParticleSystem[] radiLines;                              // 집중선 색 변경을 위한 변수
        [Tooltip("출혈 타입1")] public ParticleSystem bleeding_1;
        [Tooltip("출혈 타입2")] public ParticleSystem bleeding_2;
        [Tooltip("출혈 타입3")] public ParticleSystem bleeding_3;
        [Tooltip("둔기 타격")] public ParticleSystem bluntStrike;
        [Tooltip("검 베기")] public ParticleSystem blade;
        [Tooltip("비눗방울")] public ParticleSystem bubble;
        public ParticleSystem[] bubbles;
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

            bloom = generalCam.GetComponent<CameraFilterPack_Blur_Bloom>();
            reminisce = generalCam.GetComponent<CameraFilterPack_TV_Old_Movie_2>();
            brokenScreen = generalCam.GetComponent<CameraFilterPack_Broken_Screen>();

            glitters = glitter.GetComponentsInChildren<ParticleSystem>();

            bgFogs = bgFog.GetComponentsInChildren<ParticleSystem>();
            screenFogs = screenFog.GetComponentsInChildren<ParticleSystem>();

            dizzy = generalCam.GetComponent<CameraFilterPack_Distortion_Dream2>();

            heavySnow = generalCam.GetComponent<CameraFilterPack_3D_Snow>();
            heavyRain = generalCam.GetComponent<CameraFilterPack_Atmosphere_Rain>();
            rain = generalCam.GetComponent<D2RainsPE>();
            

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
            ViewGame.main.modelRenders[1].color = __color;

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
                cam._Fade = 1f;

                while(cam._Fade < 1)
                {
                    animTime += Time.deltaTime / __animTime;
                    cam._Fade = Mathf.Lerp(0, 1, animTime);
                    yield return null;
                }
            }
            else
            {
                cam._Fade = 0f;

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

        #region 집중선

        IEnumerator FocusRemain(float __activeTime)
        {
            radiLine.gameObject.SetActive(true);
            yield return new WaitForSeconds(__activeTime);
            radiLine.gameObject.SetActive(false);
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
                    screenGrayScale._Fade = 1f;

                    if (__params != null)
                        SetGrayScaleFade(__params, screenGrayScale);

                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_BG:
                    bgGrayScale.enabled = true;
                    bgGrayScale._Fade = 1f;

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

                case GameConst.KR_SCREEN_EFFECT_HEAVYSNOW:

                    float heavySnowIntensity = 0.95f, heavySnowSize = 1f;

                    if (__params != null)
                    {
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_FORCE, ref heavySnowIntensity);
                        ScriptRow.GetParam<float>(__params, "크기", ref heavySnowSize);
                    }

                    heavySnow.Intensity = heavySnowIntensity;
                    heavySnow.Size = heavySnowSize;

                    heavySnow.enabled = true;

                    break;

                case GameConst.KR_SCREEN_EFFECT_HEAVYRAIN:

                    heavyRain.Distortion = 0;

                    float heavyRainFade = 0.5f, heavyRainIntensity = 0.5f, heavyRainDirectionX = 0.12f, heavyRainStormFlash = 0f;

                    if (__params != null)
                    {
                        ScriptRow.GetParam<float>(__params, "선명함", ref heavyRainFade);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_FORCE, ref heavyRainIntensity);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_DIR, ref heavyRainDirectionX);
                        ScriptRow.GetParam<float>(__params, "번개", ref heavyRainStormFlash);
                    }

                    heavyRain.Fade = heavyRainFade;
                    heavyRain.Intensity = heavyRainIntensity;
                    heavyRain.DirectionX = heavyRainDirectionX;
                    heavyRain.StormFlashOnOff = heavyRainStormFlash;

                    heavyRain.enabled = true;

                    break;

                case GameConst.KR_SCREEN_EFFECT_RAIN:

                    float rainSpeed = 1f, rainDirection = 1f, rainZoom = 2;
                    float particleMultiplier = 10f;

                    if (__params != null)
                    {
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_SPEED, ref rainSpeed);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_DIR, ref rainDirection);
                        ScriptRow.GetParam<float>(__params, "시야", ref rainZoom);
                    }

                    rain.ParticleMultiplier = particleMultiplier;
                    rain.Speed = rainSpeed;
                    rain.Direction = rainDirection;
                    rain.Zoom = rainZoom;
                    rain.enabled = true;

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

                case GameConst.KR_SCREEN_EFFECT_DIZZY:

                    int dizzyLevel = 2;

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_LEVEL, ref dizzyLevel);

                    dizzyLevel = Mathf.Clamp(dizzyLevel, 1, 3);

                    switch (dizzyLevel)
                    {
                        case 1:
                            dizzy.Distortion = 4f;
                            dizzy.Speed = 2.5f;
                            break;
                        case 2:
                            dizzy.Distortion = 6f;
                            dizzy.Speed = 5f;
                            break;
                        case 3:
                            dizzy.Distortion = 8f;
                            dizzy.Speed = 7.5f;
                            break;
                    }

                    dizzy.enabled = true;

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
                    {
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_LEVEL, ref reminisceForce);

                        if (__params[0].Contains("밝음"))
                        {
                            reminisceLight.gameObject.SetActive(true);
                            reminisceLight.Play(true);
                            break;
                        }
                    }

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
            int fogLevel = 3;

            switch (__effect)
            {
                case GameConst.KR_SCREEN_EFFECT_FIRE:

                    int fireLevel = 3;

                    if(__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_DISTRIBUTION, ref fireLevel);

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

                    fire.gameObject.SetActive(true);

                    break;

                case GameConst.KR_SCREEN_EFFECT_BLING:

                    int glitterLevel = 3;

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_DISTRIBUTION, ref glitterLevel);

                    glitterLevel = Mathf.Clamp(glitterLevel, 1, 5);

                    switch (glitterLevel)
                    {
                        case 1:
                            GlitterSet(1, 1);
                            break;

                        case 2:
                            GlitterSet(2, 1);
                            break;

                        case 3:
                            GlitterSet(3, 2);
                            break;

                        case 4:
                            GlitterSet(4, 3);
                            break;

                        case 5:
                            GlitterSet(10, 8);
                            break;
                    }

                    glitter.gameObject.SetActive(true);

                    break;

                case GameConst.KR_SCREEN_EFFECT_FOCUS:

                    var colorMain = radiLines[0].main;

                    string focusColor = string.Empty;
                    float focusIntensity = 1f, focusTime = 2f;

                    if(__params !=null)
                    {
                        ScriptRow.GetParam<string>(__params, "색", ref focusColor);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_FORCE, ref focusIntensity);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_TIME, ref focusTime);
                    }

                    Color lineColor = HexCodeChanger.HexToColor(focusColor);
                    lineColor = new Color(lineColor.r, lineColor.g, lineColor.b, focusIntensity);

                    for(int i=0;i<radiLines.Length;i++)
                    {
                        colorMain = radiLines[i].main;
                        colorMain.startColor = lineColor;
                    }

                    StartCoroutine(FocusRemain(focusTime));
                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:

                    int bokehType = 1;

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_TYPE, ref bokehType);

                    bokehType = Mathf.Clamp(bokehType, 1, 2);

                    switch (bokehType)
                    {
                        case 1:
                            bokeh1.gameObject.SetActive(true);
                            break;
                        case 2:
                            bokeh2.gameObject.SetActive(true);
                            break;
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                    hexagonLight.gameObject.SetActive(true);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_DISTRIBUTION, ref fogLevel);

                    fogLevel = Mathf.Clamp(fogLevel, 1, 5);

                    switch (fogLevel)
                    {
                        case 1:
                            FogSet(1, false);
                            break;
                        case 2:
                            FogSet(2, false);
                            break;
                        case 3:
                            FogSet(3, false);
                            break;
                        case 4:
                            FogSet(4, false);
                            break;
                        case 5:
                            FogSet(5, false);
                            break;
                    }

                    bgFog.gameObject.SetActive(true);

                    break;

                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_DISTRIBUTION, ref fogLevel);

                    fogLevel = Mathf.Clamp(fogLevel, 1, 5);

                    switch (fogLevel)
                    {
                        case 1:
                            FogSet(1, true);
                            break;
                        case 2:
                            FogSet(2, true);
                            break;
                        case 3:
                            FogSet(3, true);
                            break;
                        case 4:
                            FogSet(4, true);
                            break;
                        case 5:
                            FogSet(5, true);
                            break;
                    }

                    screenFog.gameObject.SetActive(true);

                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:

                    string lensDir = "L";
                    int lightIntensity = 2;
                    int typeValue = 1;

                    if(__params != null)
                    {
                        ScriptRow.GetParam<string>(__params, GameConst.KR_PARAM_VALUE_DIR, ref lensDir);
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_STRENGTH, ref lightIntensity);
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_TYPE, ref typeValue);
                    }

                    lensDir = lensDir.ToUpper();
                    lightIntensity = Mathf.Clamp(lightIntensity, 1, 3);

                    ParticleSystem currLensFlare = null;
                    var flareGlow = flareGlow1.main.startColor.color;

                    // 렌즈 플레어 타입 설정
                    switch (typeValue)
                    {
                        case 1:
                            currLensFlare = lensFlare1;
                            flareGlow = flareGlow1.main.startColor.color;

                            // 렌즈플레어 타입에 따른 세기 설정
                            if (lightIntensity == 1)
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.01568628f);
                            else if(lightIntensity ==2)
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.02745098f);
                            else
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.05882353f);
                            break;
                        case 2:
                            currLensFlare = lensFlare2;
                            flareGlow = flareGlow2.main.startColor.color;

                            if (lightIntensity == 1)
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.1568628f);
                            else if (lightIntensity == 2)
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.2352941f);
                            else
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.3137255f);

                            break;
                        case 3:
                            currLensFlare = lensFlare3;
                            flareGlow = flareGlow3.main.startColor.color;

                            if (lightIntensity == 1)
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.01960784f);
                            else if (lightIntensity == 2)
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.05882353f);
                            else
                                flareGlow = new Color(flareGlow.r, flareGlow.g, flareGlow.b, 0.09803922f);

                            break;
                    }

                    // 방향 설정
                    if (lensDir.Contains(GameConst.POS_LEFT))
                        currLensFlare.transform.position = new Vector3(1, 1, 1);
                    else
                        currLensFlare.transform.position = new Vector3(-1, 1, 1);

                    bloom.enabled = true;
                    lightDust.gameObject.SetActive(true);
                    currLensFlare.gameObject.SetActive(true);

                    break;

                case GameConst.KR_SCREEN_EFFECT_SNOW:

                    int snowLevel = 2;

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_LEVEL, ref snowLevel);

                    snowLevel = Mathf.Clamp(snowLevel, 1, 3);

                    var snowBackEmission = snowParticles[0].emission;
                    var snowFront1Emission = snowParticles[1].emission;
                    var snowFront2Emission = snowParticles[2].emission;

                    switch(snowLevel)
                    {
                        case 1:
                            snowBackEmission.rateOverTime = 4;
                            snowFront1Emission.rateOverTime = 1;
                            snowFront2Emission.rateOverTime = 1;
                            break;
                        case 2:
                            snowBackEmission.rateOverTime = 8;
                            snowFront1Emission.rateOverTime = 2;
                            snowFront2Emission.rateOverTime = 2;
                            break;
                        case 3:
                            snowBackEmission.rateOverTime = 12;
                            snowFront1Emission.rateOverTime = 4;
                            snowFront2Emission.rateOverTime = 3;
                            break;
                    }

                    snow.gameObject.SetActive(true);
                    snow.Play(true);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:

                    int bloodType = 1;
                    float bloodTime = 2f;

                    if(__params != null)
                    {
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_TYPE, ref bloodType);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_TIME, ref bloodTime);
                    }

                    var bleedMain = bleeding_1.main;

                    switch (bloodType)
                    {
                        case 1:
                            bleedMain = bleeding_1.main;
                            bleedMain.startLifetime = bloodTime;
                            bleeding_1.gameObject.SetActive(true);
                            break;
                        case 2:
                            bleedMain = bleeding_2.main;
                            bleedMain.startLifetime = bloodTime;
                            bleeding_2.gameObject.SetActive(true);
                            break;
                        case 3:
                        default:
                            bleedMain = bleeding_3.main;
                            bleedMain.startLifetime = bloodTime;
                            bleeding_3.gameObject.SetActive(true);
                            break;
                    }

                    StartCoroutine(DisableBloodEffect(bloodTime));

                    break;

                case GameConst.KR_SCREEN_EFFECT_BUBBLES:

                    int bubbleLevel = 2;

                    if (__params != null)
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_LEVEL, ref bubbleLevel);

                    bubbleLevel = Mathf.Clamp(bubbleLevel, 1, 3);

                    switch (bubbleLevel)
                    {
                        case 1:
                            SoapBubbleSet(3);
                            break;
                        case 2:
                            SoapBubbleSet(5);
                            break;
                        case 3:
                            SoapBubbleSet(10);
                            break;
                    }

                    bubble.gameObject.SetActive(true);
                    bubble.Play(true);
                    break;

                case GameConst.KR_SCREEN_EFFECT_DAMAGE:

                    if(__params == null || __params[0].Contains("둔기"))
                    {
                        bluntStrike.gameObject.SetActive(true);
                        bluntStrike.Play(true);
                        break;
                    }

                    if(__params[0].Contains("검"))
                    {
                        blade.gameObject.SetActive(true);
                        blade.Play(true);
                        break;
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_WAVE_LINE:

                    float waveAngle = 0f, waveY = 1f;

                    if(__params != null)
                    {
                        ScriptRow.GetParam<float>(__params, "각도", ref waveAngle);
                        ScriptRow.GetParam<float>(__params, "높이", ref waveY);
                    }

                    waveAngle = Mathf.Clamp(waveAngle, 0, 360);
                    waveY = Mathf.Clamp(waveY, -6, 8);

                    waveLine.gameObject.transform.eulerAngles = new Vector3(0, 0, waveAngle);
                    waveLine.gameObject.transform.position = new Vector3(0, waveY, 0);

                    waveLine.gameObject.SetActive(true);
                    waveLine.Play(true);
                    break;
            }
        }

        /// <summary>
        /// lifeTime동안 활성화 되어있다가 비활성화 함
        /// </summary>
        IEnumerator DisableBloodEffect(float lifeTIme)
        {
            yield return new WaitForSeconds(lifeTIme);
            bleeding_1.gameObject.SetActive(false);
            bleeding_2.gameObject.SetActive(false);
            bleeding_3.gameObject.SetActive(false);
        }

        /// <summary>
        /// 반짝이 세팅
        /// </summary>
        /// <param name="__max">최대값</param>
        /// <param name="__rateOverTime">시간당 나오는 갯수</param>
        void GlitterSet(int __max, int __rateOverTime)
        {
            var glitterMain = glitter.main;
            var glitterEmission = glitter.emission;

            for(int i=2;i<glitters.Length;i++)
            {
                glitterMain = glitters[i].main;
                glitterMain.maxParticles = __max;

                glitterEmission = glitters[i].emission;
                glitterEmission.rateOverTime = __rateOverTime;
            }
        }


        /// <summary>
        /// 안개 셋팅
        /// </summary>
        /// <param name="__rateOverTime">시간당 갯수</param>
        /// <param name="isScreen">배경만인지? 스크린 전체인지</param>
        void FogSet(int __rateOverTime, bool isScreen)
        {
            var fogEmission = bgFog.emission;

            if(!isScreen)
            {
                for(int i=2;i<bgFogs.Length;i++)
                {
                    fogEmission = bgFogs[i].emission;
                    fogEmission.rateOverTime = __rateOverTime;
                }
            }
            else
            {
                for (int i = 2; i < screenFogs.Length; i++)
                {
                    fogEmission = screenFogs[i].emission;
                    fogEmission.rateOverTime = __rateOverTime;
                }
            }
        }



        /// <summary>
        /// 비눗방울 단계 조절
        /// </summary>
        /// <param name="__max"></param>
        void SoapBubbleSet(int __max)
        {
            var bubbleMain = bubble.main;

            for (int i = 0; i < bubbles.Length; i++)
            {
                bubbleMain = bubbles[i].main;
                bubbleMain.maxParticles = __max;
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

        /// <summary>
        /// 일반 이펙트 삭제
        /// </summary>
        /// <param name="__effect"></param>
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
                    glitter.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FIRE:
                    fire.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOCUS:
                    radiLine.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:
                    lensFlare1.gameObject.SetActive(false);
                    lensFlare2.gameObject.SetActive(false);
                    lensFlare3.gameObject.SetActive(false);
                    bloom.enabled = false;
                    lightDust.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:
                    bgFog.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:
                    screenFog.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:
                    bokeh1.gameObject.SetActive(false);
                    bokeh2.gameObject.SetActive(false);
                    break;
                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                    hexagonLight.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_SNOW:
                    snow.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BUBBLES:
                    bubble.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_DAMAGE:
                    bluntStrike.gameObject.SetActive(false);
                    blade.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_WAVE_LINE:
                    waveLine.gameObject.SetActive(false);
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
                    bgGrayScale.enabled = false;
                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_BG:
                    screenGrayScale.enabled = false;
                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_CH:
                    ViewGame.main.modelRenders[1].material.DisableKeyword("GREYSCALE_ON");
                    break;

                case GameConst.KR_SCREEN_EFFECT_BROKEN:
                    brokenScreen.enabled = false;
                    break;

                case GameConst.KR_SCREEN_EFFECT_HEAVYRAIN:
                    heavyRain.enabled = false;
                    break;
                case GameConst.KR_SCREEN_EFFECT_RAIN:
                    rain.enabled = false;
                    break;
                case GameConst.KR_SCREEN_EFFECT_HEAVYSNOW:
                    heavySnow.enabled = false;
                    break;

                case GameConst.KR_SCREEN_EFFECT_ZOOMIN:
                    mainCam.orthographicSize = mainCamOriginSize;
                    modelRenderCamC.orthographicSize = modelCamOriginSize;
                    modelRenderCamC.transform.position = new Vector3(modelRenderCamC.transform.position.x, 0, modelRenderCamC.transform.position.z);
                    break;

                case GameConst.KR_SCREEN_EFFECT_DIZZY:
                    dizzy.enabled = false;
                    break;

                case GameConst.KR_SCREEN_EFFECT_GLITCH:
                    glitch.enabled = false;
                    break;

                case GameConst.KR_SCREEN_EFFECT_GLITCH_SCREEN:
                    screenGlitch.enabled = false;
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:
                    bleeding_1.gameObject.SetActive(false);
                    bleeding_2.gameObject.SetActive(false);
                    bleeding_3.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:
                    reminisce.enabled = false;

                    if (reminisceLight.gameObject.activeSelf)
                        reminisceLight.gameObject.SetActive(false);

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