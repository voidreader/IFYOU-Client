﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using DG.Tweening;
using AkilliMum.Standard.D2WeatherEffects;

namespace PIERStory
{
    public class ScreenEffectManager : MonoBehaviour
    {
        public static ScreenEffectManager main = null;

        public Camera mainCam;
        public Camera modelRenderCamC; // 모델용 중앙 카메라 
        public Camera modelRenderCamL; // 모델용 L 카메라 
        public Camera modelRenderCamR; // 모델용 R 카메라 

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

        [Space(20)]
        [Header("Particle system effect")]
        [Tooltip("불 파티클")] public ParticleSystem firePrefab;
        ParticleSystem fire;
        [Tooltip("반짝이 파티클")] public ParticleSystem glitterPrefab;
        ParticleSystem glitter;
        ParticleSystem[] glitters;
        [Tooltip("원형빛1")] public ParticleSystem bokeh1Prefab;
        ParticleSystem bokeh1;
        [Tooltip("원형빛2")] public ParticleSystem bokeh2Prefab;
        ParticleSystem bokeh2;
        [Tooltip("육각형빛")] public ParticleSystem hexagonLightPrefab;
        ParticleSystem hexagonLight;
        public ParticleSystem fogPrefab;
        [Tooltip("배경만 안개")] ParticleSystem bgFog;
        ParticleSystem[] bgFogs;
        [Tooltip("스크린 안개")] ParticleSystem screenFog;
        ParticleSystem[] screenFogs;
        [Tooltip("폭우")] public ParticleSystem heavyRainParticle;
        [Tooltip("비")] public ParticleSystem rainParticle;
        [Tooltip("폭설")] public ParticleSystem heavySnowParticle;
        public ParticleSystem snowPrefab;
        [Tooltip("눈")] ParticleSystem snow;
        List<ParticleSystem> snowParticles;
        [Tooltip("렌즈플레어1")] public ParticleSystem lensFlare1Prefab;
        ParticleSystem lensFlare1;
        ParticleSystem flareGlow1;
        [Tooltip("렌즈플레어2")] public ParticleSystem lensFlare2Prefab;
        ParticleSystem lensFlare2;
        ParticleSystem flareGlow2;
        [Tooltip("렌즈플레어3")] public ParticleSystem lensFlare3Prefab;
        ParticleSystem lensFlare3;
        ParticleSystem flareGlow3;
        [Tooltip("렌즈플레어에서 사용되는 빛알갱이")] public ParticleSystem lightDust;
        [Tooltip("집중선")] public ParticleSystem radiLinePrefab;
        ParticleSystem radiLine;
        List<ParticleSystem> radiLines;                              // 집중선 색 변경을 위한 변수
        [Tooltip("출혈 타입1")] public ParticleSystem bleeding_1Prefab;
        ParticleSystem bleeding_1;
        [Tooltip("출혈 타입2")] public ParticleSystem bleeding_2Prefab;
        ParticleSystem bleeding_2;
        [Tooltip("출혈 타입3")] public ParticleSystem bleeding_3Prefab;
        ParticleSystem bleeding_3;
        [Tooltip("둔기 타격")] public ParticleSystem bluntStrikePrefab;
        ParticleSystem bluntStrike;
        [Tooltip("검 베기")] public ParticleSystem bladePrefab;
        ParticleSystem blade;
        [Tooltip("비눗방울")] public ParticleSystem bubblePrefab;
        ParticleSystem bubble;
        List<ParticleSystem> bubbles;
        [Tooltip("회상, 밝음")] public ParticleSystem reminisceLightPrefab;
        ParticleSystem reminisceLight;
        [Tooltip("신비로운 경계라인")] public ParticleSystem waveLinePrefab;
        ParticleSystem waveLine;

        #endregion

        Dictionary<string, int> DictParticle = new Dictionary<string, int>();

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

            dizzy = generalCam.GetComponent<CameraFilterPack_Distortion_Dream2>();

            heavySnow = generalCam.GetComponent<CameraFilterPack_3D_Snow>();
            heavyRain = generalCam.GetComponent<CameraFilterPack_Atmosphere_Rain>();
            rain = generalCam.GetComponent<D2RainsPE>();
        }

        public void InstantiateEffect(string __data)
        {
            string effect = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[0] : __data;
            string[] __params = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) && __data.Contains(GameConst.KR_PARAM_VALUE_TYPE) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[1].Split(GameConst.SPLIT_SCREEN_EFFECT_V[0]) : null;
            int effectType = 1;

            if (!AddInEffectDictionary(__data))
                return;

            switch (effect)
            {
                case GameConst.KR_SCREEN_EFFECT_FIRE:
                    fire = Instantiate(firePrefab, transform);
                    ChangeJustLayerRecursively(fire.transform);
                    fire.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLING:
                    glitter = Instantiate(glitterPrefab, transform);
                    ChangeJustLayerRecursively(glitter.transform);
                    glitters = glitter.GetComponentsInChildren<ParticleSystem>();
                    glitter.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOCUS:
                    radiLine = Instantiate(radiLinePrefab, transform);
                    ChangeJustLayerRecursively(radiLine.transform);
                    radiLines = new List<ParticleSystem>();

                    for (int i = 0; i < radiLine.transform.GetChild(0).GetChild(0).childCount; i++)
                    {
                        radiLines.Add(radiLine.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
                        radiLine.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    }

                    radiLine.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:

                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref effectType);

                    if (effectType == 1)
                    {
                        bokeh1 = Instantiate(bokeh1Prefab, transform);
                        ChangeJustLayerRecursively(bokeh1.transform);
                        bokeh1.gameObject.SetActive(false);
                    }
                    else
                    {
                        bokeh2 = Instantiate(bokeh2Prefab, transform);
                        ChangeJustLayerRecursively(bokeh2.transform);
                        bokeh2.gameObject.SetActive(false);
                    }
                    break;

                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                    hexagonLight = Instantiate(hexagonLightPrefab, transform);
                    ChangeJustLayerRecursively(hexagonLight.transform);
                    hexagonLight.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:
                    bgFog = Instantiate(fogPrefab, transform);
                    bgFogs = bgFog.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem __ps in bgFogs)
                        __ps.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    bgFog.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:
                    screenFog = Instantiate(fogPrefab, transform);
                    ChangeJustLayerRecursively(screenFog.transform);
                    screenFogs = screenFog.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem __ps in screenFogs)
                        __ps.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    screenFog.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:

                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref effectType);

                    if (effectType == 1)
                    {
                        lensFlare1 = Instantiate(lensFlare1Prefab, transform);
                        ChangeJustLayerRecursively(lensFlare1.transform);
                        flareGlow1 = lensFlare1.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                        lensFlare1.gameObject.SetActive(false);
                    }
                    else if (effectType == 2)
                    {
                        lensFlare2 = Instantiate(lensFlare2Prefab, transform);
                        ChangeJustLayerRecursively(lensFlare2.transform);
                        flareGlow2 = lensFlare2.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                        lensFlare2.gameObject.SetActive(false);
                    }
                    else
                    {
                        lensFlare3 = Instantiate(lensFlare3Prefab, transform);
                        ChangeJustLayerRecursively(lensFlare3.transform);
                        flareGlow3 = lensFlare3.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                        lensFlare3.gameObject.SetActive(false);
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_SNOW:
                    snow = Instantiate(snowPrefab, transform);
                    ChangeJustLayerRecursively(snow.transform);
                    snowParticles = new List<ParticleSystem>();
                    for (int i = 1; i < snow.transform.GetChild(0).childCount; i++)
                        snowParticles.Add(snow.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
                    snow.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:
                    
                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref effectType);
                        
                    

                    if (effectType == 1)
                    {
                        Debug.Log("출혈 인스턴시에이트 #1");
                        bleeding_1 = Instantiate(bleeding_1Prefab, transform);
                        ChangeJustLayerRecursively(bleeding_1.transform);
                        bleeding_1.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                        bleeding_1.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                        bleeding_1.gameObject.SetActive(false);
                    }
                    else if (effectType == 2)
                    {
                        Debug.Log("출혈 인스턴시에이트 #2");
                        bleeding_2 = Instantiate(bleeding_2Prefab, transform);
                        ChangeJustLayerRecursively(bleeding_2.transform);
                        bleeding_2.gameObject.SetActive(false);
                        bleeding_2.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                        bleeding_2.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    }
                    else
                    {
                        Debug.Log("출혈 인스턴시에이트 #3");
                        bleeding_3 = Instantiate(bleeding_3Prefab, transform);
                        ChangeJustLayerRecursively(bleeding_3.transform);
                        bleeding_3.gameObject.SetActive(false);
                        bleeding_3.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                        bleeding_3.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    }
                    break;

                case GameConst.KR_SCREEN_EFFECT_BUBBLES:
                    bubble = Instantiate(bubblePrefab, transform);
                    ChangeJustLayerRecursively(bubble.transform);
                    bubbles = new List<ParticleSystem>();
                    for (int i = 2; i < bubble.transform.GetChild(0).childCount; i++)
                        bubbles.Add(bubble.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
                    bubble.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_DAMAGE:

                    __params = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0]) : null;

                    if (__params == null || __params[1].Contains("둔기"))
                    {
                        bluntStrike = Instantiate(bluntStrikePrefab, transform);
                        ChangeJustLayerRecursively(bluntStrike.transform);
                        bluntStrike.gameObject.SetActive(false);
                        break;
                    }
                    else if (__params[1].Contains("검"))
                    {
                        blade = Instantiate(bladePrefab, transform);
                        ChangeJustLayerRecursively(blade.transform);
                        blade.gameObject.SetActive(false);
                        break;
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:

                    if (!__data.Contains("밝음"))
                        break;

                    reminisceLight = Instantiate(reminisceLightPrefab, transform);
                    ChangeJustLayerRecursively(reminisceLight.transform);
                    reminisceLight.gameObject.SetActive(false);
                    reminisceLight.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    reminisceLight.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().material.shader = Shader.Find("Mobile/Particles/Additive");
                    break;

                case GameConst.KR_SCREEN_EFFECT_WAVE_LINE:
                    waveLine = Instantiate(waveLinePrefab, transform);
                    ChangeJustLayerRecursively(waveLine.transform);
                    for (int i = 0; i < waveLine.transform.GetChild(0).childCount; i++)
                    {
                        for (int j = 0; j < waveLine.transform.GetChild(0).GetChild(i).childCount; j++)
                            waveLine.transform.GetChild(0).GetChild(i).GetChild(j).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Legacy Shaders/Particles/Additive (Soft)");
                    }

                    waveLine.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Legacy Shaders/Particles/Additive (Soft)");
                    waveLine.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Legacy Shaders/Particles/Additive (Soft)");
                    waveLine.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystemRenderer>().material = null;
                    waveLine.gameObject.SetActive(false);
                    break;
            }
        }

        void ChangeJustLayerRecursively(Transform trans)
        {
            trans.gameObject.layer = LayerMask.NameToLayer("Particles");

            foreach (Transform tr in trans)
                ChangeLayerRecursively(tr);
        }

        bool AddInEffectDictionary(string __data)
        {
            string effect = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[0] : __data;
            string[] __params = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) && __data.Contains(GameConst.KR_PARAM_VALUE_TYPE) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[1].Split(GameConst.SPLIT_SCREEN_EFFECT_V[0]) : null;
            int effectType = 1;
            string effectKey = string.Empty;

            switch (effect)
            {
                case GameConst.KR_SCREEN_EFFECT_FIRE:
                    effectKey = effect;
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLING:
                    effectKey = effect;
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOCUS:
                    effectKey = effect;
                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:

                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref effectType);

                    effectKey = effect + effectType.ToString();

                    break;

                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                    effectKey = effect;
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:
                    effectKey = effect;
                    break;

                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:
                    effectKey = effect;
                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:

                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref effectType);

                    effectKey = effect + effectType.ToString();

                    break;

                case GameConst.KR_SCREEN_EFFECT_SNOW:
                    effectKey = effect;
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:

                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref effectType);

                    effectKey = effect + effectType.ToString();

                    break;

                case GameConst.KR_SCREEN_EFFECT_BUBBLES:
                    effectKey = effect;
                    break;

                case GameConst.KR_SCREEN_EFFECT_DAMAGE:

                    __params = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0]) : null;

                    if (__params == null || __params[1].Contains("둔기"))
                    {
                        effectKey = effect + "둔기";
                        break;
                    }
                    else if (__params[1].Contains("검"))
                    {
                        effectKey = effect + "검";
                        break;
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:
                    if (!__data.Contains("밝음"))
                        return false;
                    else
                        effectKey = effect + "밝음";
                    break;

                case GameConst.KR_SCREEN_EFFECT_WAVE_LINE:
                    effectKey = effect;
                    break;
            }


            if (DictParticle.ContainsKey(effectKey))
                return false;

            DictParticle.Add(effectKey, 1);
            return true;
        }


        #region 실패한 Addressable....총체적 난국

        public void InitScreenEffect(string __data)
        {
            string effect = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[0] : __data;
            string[] __params = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) && __data.Contains(GameConst.KR_PARAM_VALUE_TYPE) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[1].Split(GameConst.SPLIT_SCREEN_EFFECT_V[0]) : null;

            switch (effect)
            {
                case GameConst.KR_SCREEN_EFFECT_FIRE:
                    LoadAddressableEffect("fx_Flame_001", fire, effect);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLING:
                    LoadAddressableEffect("fx_Glitter_001", glitter, effect);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOCUS:
                    LoadAddressableEffect("fx_RadiLine_001", radiLine, effect);
                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:

                    int bokehType = 1;

                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref bokehType);

                    if (bokehType == 1)
                        LoadAddressableEffect("fx_Bokeh_001", bokeh1, effect, false, bokehType);
                    else
                        LoadAddressableEffect("fx_Bokeh_002", bokeh2, effect, false, bokehType);
                    break;

                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                    LoadAddressableEffect("HexLightEffect", hexagonLight, effect);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:
                    LoadAddressableEffect("fx_Mist_001", bgFog, effect, true);
                    break;

                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:
                    LoadAddressableEffect("fx_Mist_001", screenFog, effect);
                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:

                    int flareType = 1;

                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref flareType);

                    if (flareType == 1)
                        LoadAddressableEffect("fx_LensFlare_001", lensFlare1, effect, false, flareType);
                    else if (flareType == 2)
                        LoadAddressableEffect("fx_LensFlare_002", lensFlare2, effect, false, flareType);
                    else
                        LoadAddressableEffect("fx_LensFlare_003", lensFlare3, effect, false, flareType);

                    break;

                case GameConst.KR_SCREEN_EFFECT_SNOW:
                    LoadAddressableEffect("fx_Snow", snow, effect);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:

                    int bloodType = 1;

                    if (__params != null)
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TYPE, ref bloodType);

                    if (bloodType == 1)
                        LoadAddressableEffect("fx_Blood_001", bleeding_1, effect, false, bloodType);
                    else if (bloodType == 2)
                        LoadAddressableEffect("fx_Blood_002", bleeding_2, effect, false, bloodType);
                    else
                        LoadAddressableEffect("fx_Blood_003", bleeding_3, effect, false, bloodType);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BUBBLES:
                    LoadAddressableEffect("fx_Bubble_001", bubble, effect);
                    break;

                case GameConst.KR_SCREEN_EFFECT_DAMAGE:

                    __params = __data.Contains(GameConst.SPLIT_SCREEN_EFFECT) ? __data.Split(GameConst.SPLIT_SCREEN_EFFECT[0]) : null;

                    if (__params == null || __params[1].Contains("둔기"))
                    {
                        LoadAddressableEffect("fx_HitStrike_001", bluntStrike, effect, false, 1);
                        break;
                    }
                    else if (__params[1].Contains("검"))
                    {
                        LoadAddressableEffect("fx_HitSword_001", blade, effect, false, 2);
                        break;
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:

                    if (__data.Contains("밝음"))
                        LoadAddressableEffect("fx_BoxGlow_001", reminisceLight, effect);

                    break;


                case GameConst.KR_SCREEN_EFFECT_WAVE_LINE:
                    LoadAddressableEffect("fx_WaveLine_002", waveLine, effect);
                    break;
            }
        }

        void LoadAddressableEffect(string __key, ParticleSystem __particle, string __effect, bool isDefaultLayer = false, int __type = 1)
        {
            string addressableKey = "ScreenEffect/" + __key + ".prefab";

            // 해당 파티클은 이미 생성했음
            if (DictParticle.ContainsKey(addressableKey))
                return;

            DictParticle.Add(addressableKey, 1);

            Addressables.InstantiateAsync(addressableKey, Vector3.zero, Quaternion.identity, transform).Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    __particle = handle.Result.GetComponent<ParticleSystem>();

                    // 레이어를 모두 파티클로 변경해주기
                    if (!isDefaultLayer)
                        ChangeLayerRecursively(__particle.transform);

                    InitCreateEffectAsset(__particle, __effect, __type);

                    // 완료되고는 비활성화
                    __particle.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError(addressableKey + "InstantiateAsync failed. " + handle.Status);
#if UNITY_IOS
                    SystemManager.ShowSystemPopup(addressableKey + "InstantiateAsync failed. " + handle.Status, null, null, false, false);
#endif
                }
            };
        }

        void InitCreateEffectAsset(ParticleSystem ps, string effect, int __type = 1)
        {
            if (ps == null)
            {
                Debug.LogError("Create particle failed : " + effect);
#if UNITY_IOS
                    SystemManager.ShowSystemPopup("Create particle failed : " + effect, null, null, false, false);
#endif
                return;
            }


            switch (effect)
            {
                case GameConst.KR_SCREEN_EFFECT_FIRE:
                    fire = ps;
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLING:
                    glitter = ps;
                    glitters = glitter.GetComponentsInChildren<ParticleSystem>();
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOCUS:
                    radiLine = ps;

                    for (int i = 0; i < radiLine.transform.GetChild(0).GetChild(0).childCount; i++)
                    {
                        radiLines.Add(radiLine.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
                        radiLine.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:

                    if (__type == 1)
                        bokeh1 = ps;
                    else
                        bokeh2 = ps;
                    break;

                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                    hexagonLight = ps;
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:
                    bgFog = ps;
                    bgFogs = bgFog.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem __ps in bgFogs)
                        __ps.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    break;

                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:
                    screenFog = ps;
                    screenFogs = screenFog.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem __ps in screenFogs)
                        __ps.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:

                    if (__type == 1)
                    {
                        lensFlare1 = ps;
                        flareGlow1 = lensFlare1.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                    }
                    else if (__type == 2)
                    {
                        lensFlare2 = ps;
                        flareGlow2 = lensFlare2.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                    }
                    else
                    {
                        lensFlare3 = ps;
                        flareGlow3 = lensFlare3.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                    }
                    break;

                case GameConst.KR_SCREEN_EFFECT_SNOW:
                    snow = ps;
                    for (int i = 1; i < snow.transform.GetChild(0).childCount; i++)
                        snowParticles.Add(snow.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:

                    if (__type == 1)
                    {
                        bleeding_1 = ps;
                        bleeding_1.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                        bleeding_1.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    }
                    else if (__type == 2)
                    {
                        bleeding_2 = ps;
                        bleeding_2.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                        bleeding_2.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    }
                    else
                    {
                        bleeding_3 = ps;
                        bleeding_3.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                        bleeding_3.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    }
                    break;

                case GameConst.KR_SCREEN_EFFECT_BUBBLES:
                    bubble = ps;
                    for (int i = 2; i < bubble.transform.GetChild(0).childCount; i++)
                        bubbles.Add(bubble.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());

                    break;

                case GameConst.KR_SCREEN_EFFECT_DAMAGE:

                    if (__type == 1)
                        bluntStrike = ps;
                    else
                        blade = ps;
                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:
                    reminisceLight = ps;
                    reminisceLight.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                    reminisceLight.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().material.shader = Shader.Find("Mobile/Particles/Additive");
                    break;

                case GameConst.KR_SCREEN_EFFECT_WAVE_LINE:
                    waveLine = ps;
                    waveLine.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystemRenderer>().material = null;
                    break;
            }
        }

        void ChangeLayerRecursively(Transform trans)
        {
            trans.gameObject.layer = LayerMask.NameToLayer("Particles");

            if (trans.GetComponent<ParticleSystemRenderer>() != null)
                trans.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Mobile/Particles/Additive");

            foreach (Transform tr in trans)
                ChangeLayerRecursively(tr);
        }

        #endregion

        
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

            if (fadeValue == 0)
            {
                cam._Fade = 1f;

                while (cam._Fade < 1)
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

                    if (__params != null)
                    {
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_TIME, ref shakeTime);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_STRENGTH, ref shakeStrength);
                    }

                    shakeTime = Mathf.Clamp(shakeTime, 0f, 3f);
                    shakeStrength = Mathf.Clamp(shakeStrength, 0f, 3f);

                    // generalCam.transform.DOShakePosition(shakeTime, 0.6f, (int)(shakeStrength * 30), 90).OnComplete(OnCompleteShake);
                    // mainCam.transform.DOShakePosition(shakeTime, 0.2f, (int)(shakeStrength * 30), 90);

                    // * generalCam을 흔들면 캐릭터가 지직 거려서, 모델캠을 흔들도록 변경 2022.04.28

                    modelRenderCamC.transform.DOShakePosition(shakeTime, 0.2f, (int)(shakeStrength * 30), 90);
                    modelRenderCamL.transform.DOShakePosition(shakeTime, 0.2f, (int)(shakeStrength * 30), 90);
                    modelRenderCamR.transform.DOShakePosition(shakeTime, 0.2f, (int)(shakeStrength * 30), 90);
                    mainCam.transform.DOShakePosition(shakeTime, 0.6f, (int)(shakeStrength * 30), 90).OnComplete(OnCompleteShake);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BROKEN:

                    float brokenShadow = 1f;
                    float fadeForce = 1f;

                    bool isRecover = false, isAnim = false;

                    if (__params != null)
                    {
                        ScriptRow.GetParam<float>(__params, "그림자", ref brokenShadow);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_FORCE, ref fadeForce);

                        for (int i = 0; i < __params.Length; i++)
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

                    if (__params != null)
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

                    if (__params != null)
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

                    ParticleSystem.MainModule colorMain;

                    string focusColor = string.Empty;
                    float focusIntensity = 1f, focusTime = 2f;

                    if (__params != null)
                    {
                        ScriptRow.GetParam(__params, "색", ref focusColor);
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_FORCE, ref focusIntensity);
                        ScriptRow.GetParam(__params, GameConst.KR_PARAM_VALUE_TIME, ref focusTime);
                    }

                    Color lineColor = HexCodeChanger.HexToColor(focusColor);
                    lineColor = new Color(lineColor.r, lineColor.g, lineColor.b, focusIntensity);

                    for (int i = 0; i < radiLines.Count; i++)
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

                    if (__params != null)
                    {
                        ScriptRow.GetParam<string>(__params, GameConst.KR_PARAM_VALUE_DIR, ref lensDir);
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_STRENGTH, ref lightIntensity);
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_TYPE, ref typeValue);
                    }

                    lensDir = lensDir.ToUpper();
                    lightIntensity = Mathf.Clamp(lightIntensity, 1, 3);

                    ParticleSystem currLensFlare = null;
                    ParticleSystem.MinMaxGradient flareGlow;

                    // 렌즈 플레어 타입 설정
                    switch (typeValue)
                    {
                        case 1:
                            currLensFlare = lensFlare1;
                            flareGlow = flareGlow1.main.startColor;

                            // 렌즈플레어 타입에 따른 세기 설정
                            if (lightIntensity == 1)
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.01568628f);
                            else if (lightIntensity == 2)
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.02745098f);
                            else
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.05882353f);
                            break;
                        case 2:
                            currLensFlare = lensFlare2;
                            flareGlow = flareGlow2.main.startColor;

                            if (lightIntensity == 1)
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.1568628f);
                            else if (lightIntensity == 2)
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.2352941f);
                            else
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.3137255f);

                            break;
                        case 3:
                            currLensFlare = lensFlare3;
                            flareGlow = flareGlow3.main.startColor;
                            
                            if (lightIntensity == 1)
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.01960784f);
                            else if (lightIntensity == 2)
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.05882353f);
                            else
                                flareGlow.color = new Color(flareGlow.color.r, flareGlow.color.g, flareGlow.color.b, 0.09803922f);

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

                    ParticleSystem.MinMaxCurve snowBackEmission = snowParticles[0].emission.rateOverTime, snowFront1Emission = snowParticles[1].emission.rateOverTime, snowFront2Emission = snowParticles[2].emission.rateOverTime;
                    
                    switch (snowLevel)
                    {
                        case 1:
                            snowBackEmission = 4;
                            snowFront1Emission = 1;
                            snowFront2Emission = 1;
                            break;
                        case 2:
                            snowBackEmission = 8;
                            snowFront1Emission = 2;
                            snowFront2Emission = 2;
                            break;
                        case 3:
                            snowBackEmission = 12;
                            snowFront1Emission = 4;
                            snowFront2Emission = 3;
                            break;
                    }

                    snow.gameObject.SetActive(true);
                    snow.Play(true);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:

                    int bloodType = 1;
                    float bloodTime = 2f;

                    if (__params != null)
                    {
                        ScriptRow.GetParam<int>(__params, GameConst.KR_PARAM_VALUE_TYPE, ref bloodType);
                        ScriptRow.GetParam<float>(__params, GameConst.KR_PARAM_VALUE_TIME, ref bloodTime);
                    }

                    ParticleSystem.MainModule bleedMain;

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

                    if (__params == null || __params[0].Contains("둔기"))
                    {
                        bluntStrike.gameObject.SetActive(true);
                        bluntStrike.Play(true);
                        break;
                    }

                    if (__params[0].Contains("검"))
                    {
                        blade.gameObject.SetActive(true);
                        blade.Play(true);
                        break;
                    }

                    break;

                case GameConst.KR_SCREEN_EFFECT_WAVE_LINE:

                    float waveAngle = 0f, waveY = 1f;

                    if (__params != null)
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
            if (bleeding_1 != null)
                bleeding_1.gameObject.SetActive(false);
            if (bleeding_2 != null)
                bleeding_2.gameObject.SetActive(false);
            if (bleeding_3 != null)
                bleeding_3.gameObject.SetActive(false);
        }

        /// <summary>
        /// 반짝이 세팅
        /// </summary>
        /// <param name="__max">최대값</param>
        /// <param name="__rateOverTime">시간당 나오는 갯수</param>
        void GlitterSet(int __max, int __rateOverTime)
        {
            ParticleSystem.MainModule glitterMain;
            ParticleSystem.EmissionModule glitterEmission;

            for (int i = 2; i < glitters.Length; i++)
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
            ParticleSystem.EmissionModule fogEmission;

            if (!isScreen)
            {
                for (int i = 2; i < bgFogs.Length; i++)
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
            ParticleSystem.MainModule bubbleMain;

            for (int i = 0; i < bubbles.Count; i++)
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
                    if (glitter != null)
                        glitter.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FIRE:
                    if (fire != null)
                        fire.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOCUS:
                    if (radiLine != null)
                        radiLine.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:
                    if (lensFlare1 != null)
                        lensFlare1.gameObject.SetActive(false);
                    if (lensFlare2 != null)
                        lensFlare2.gameObject.SetActive(false);
                    if (lensFlare3 != null)
                        lensFlare3.gameObject.SetActive(false);

                    bloom.enabled = false;
                    lightDust.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:
                    if (bgFog != null)
                        bgFog.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:
                    if (screenFog != null)
                        screenFog.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:
                    if (bokeh1 != null)
                        bokeh1.gameObject.SetActive(false);
                    if (bokeh2 != null)
                        bokeh2.gameObject.SetActive(false);
                    break;
                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                    if (hexagonLight != null)
                        hexagonLight.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_SNOW:
                    if (snow != null)
                        snow.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BUBBLES:
                    if (bubble != null)
                        bubble.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_DAMAGE:
                    if (bluntStrike != null)
                        bluntStrike.gameObject.SetActive(false);
                    if (blade != null)
                        blade.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_WAVE_LINE:
                    if (waveLine != null)
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
                    if (bleeding_1 != null)
                        bleeding_1.gameObject.SetActive(false);
                    if (bleeding_2 != null)
                        bleeding_2.gameObject.SetActive(false);
                    if (bleeding_3 != null)
                        bleeding_3.gameObject.SetActive(false);
                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:
                    reminisce.enabled = false;

                    if (reminisceLight != null && reminisceLight.gameObject.activeSelf)
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