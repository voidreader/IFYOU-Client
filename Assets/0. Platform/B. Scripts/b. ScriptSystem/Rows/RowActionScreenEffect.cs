using System;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory
{

    /// <summary>
    /// 화면연출 동작
    /// </summary>
    public class RowActionScreenEffect : IRowAction
    {
        public static List<string> ListAllCommands = new List<string>();

        public static List<string> ListGeneralEffect = new List<string>(); // 일반 이펙트
        public static List<string> ListCameraEffect = new List<string>(); // 카메라에 연결되는 이펙트 

        ScriptRow scriptRow;
        Action callback = delegate { };
        string script_data = string.Empty;

        string command = string.Empty;
        string paramOriginal = string.Empty;
        string param1 = string.Empty;
        string param2 = string.Empty;
        string[] paramArray; // 파라매터 array

        bool isValidCommand = false;        // 유효한 명령어인지..!
        Color effectColor = Color.black;    // 연출에 사용되는 컬러! 
        float effectTime = 0;               // 이펙트 활성화 되는 시간

        public RowActionScreenEffect(ScriptRow __row)
        {
            InitStaticList();

            scriptRow = __row;
            script_data = scriptRow.script_data;

            // 만약 데이터 값이 없거나 기호가 없는 경우 동작하지 않음
            if (string.IsNullOrEmpty(script_data))
                return; //

            script_data = script_data.Replace(" ", ""); // 공백제거 


            // 구분자가 없는 경우 있음 
            if (!script_data.Contains(GameConst.SPLIT_SCREEN_EFFECT))
            {
                Debug.Log(string.Format("<color=white>No screen effect split : {0}</color>", script_data));

                command = script_data;
                paramOriginal = string.Empty;
                param1 = string.Empty;
                param2 = string.Empty;
            }
            else
            {
                // 명령어 쪼개기를 시작한다.
                command = script_data.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[0]; // command 받아오고 
                paramOriginal = script_data.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[1]; // param으로 분리 

                if (paramOriginal.Contains(GameConst.SPLIT_SCREEN_EFFECT_V))
                {
                    param2 = paramOriginal.Split(GameConst.SPLIT_SCREEN_EFFECT_V[0])[1];

                    // array 값 사용 
                    paramArray = paramOriginal.Split(GameConst.SPLIT_SCREEN_EFFECT_V[0]);

                    if (paramArray.Length >= 1)
                        param1 = paramArray[0];

                    if (paramArray.Length >= 2)
                        param2 = paramArray[1];
                }
                else
                {
                    paramArray = paramOriginal.Split(GameConst.SPLIT_SCREEN_EFFECT_V[0]);
                    param1 = paramOriginal; // 구분 기호가 없으면 하나!
                }
            }

            // 파라매터 처리 후 유효성 체크 !
            isValidCommand = CheckEffectCommandValidation();
        }


        /// <summary>
        /// 한번만 실행
        /// </summary>
        void InitStaticList()
        {
            if (ListAllCommands.Count > 0)
                return;

            // 모든 명령어
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_TINT);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_TINT_BG);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_TINT_CH);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_GRAYSCALE);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_GRAYSCALE_BG);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_GRAYSCALE_CH);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_BROKEN);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_ANOMALY);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_DIZZY);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_BLUR);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_BLOOD_HIT);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_GLITCH);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_GLITCH_SCREEN);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_REMINISCE);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_SHAKE);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_BLING);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_FOG);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_SCREEN_FOG);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_FIRE);

            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_FOCUS);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_HEAVYSNOW);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_SNOW);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_RAIN);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_HEAVYRAIN);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_ZOOMIN);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_ZOOMOUT);

            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_LENS_FLARE);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_HEX_LIGHT);
            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT);

            ListAllCommands.Add(GameConst.KR_SCREEN_EFFECT_CAMERA_FLASH);

            // 일반 이펙트 명령어 
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_TINT);
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_TINT_BG);
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_TINT_CH);
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_BLING);
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_FIRE);
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_LENS_FLARE);
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_HEX_LIGHT);
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT);
            ListGeneralEffect.Add(GameConst.KR_SCREEN_EFFECT_CAMERA_FLASH);

            // 카메라 이펙트 명령어 
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_GRAYSCALE);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_GRAYSCALE_BG);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_GRAYSCALE_CH);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_BROKEN);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_ANOMALY);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_DIZZY);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_BLUR);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_SHAKE);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_FOG);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_SCREEN_FOG);

            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_FOCUS);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_HEAVYSNOW);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_SNOW);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_HEAVYRAIN);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_RAIN);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_ZOOMIN);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_ZOOMOUT);

            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_BLOOD_HIT);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_GLITCH);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_GLITCH_SCREEN);
            ListCameraEffect.Add(GameConst.KR_SCREEN_EFFECT_REMINISCE);
        }

        /// <summary>
        /// 명령어에 따른 param 등 체크 
        /// </summary>
        /// <returns></returns>
        bool CheckEffectCommandValidation()
        {
            // 명령어가 없으면...
            if (!ListAllCommands.Contains(command))
                return false;

            switch (command)
            {
                case GameConst.KR_SCREEN_EFFECT_TINT: // 틴트.. 
                case GameConst.KR_SCREEN_EFFECT_TINT_BG:
                case GameConst.KR_SCREEN_EFFECT_TINT_CH:
                    // param1 : 컬러값 6,8자리 hex 코드, param2 시간. (숫자)
                    if (param1.Length != 6 && param1.Length != 8)
                        return false;

                    effectColor = HexCodeChanger.HexToColor(param1);

                    // param1이 hex 코드일때, 2가 있는지 체크
                    if (!string.IsNullOrEmpty(param2))
                    {
                        // 숫자형태인지 체크 필요. 
                        if (!float.TryParse(param2, out effectTime))
                        {
                            Debug.LogError("Wrong param2 : " + param2);
                            effectTime = 0;
                        }
                    }
                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE:
                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_CH:
                    break;

                case GameConst.KR_SCREEN_EFFECT_BROKEN:
                    break;
            }

            return true;
        }


        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            Debug.Log(string.Format("Screen Effect {0}", command));

            callback = __actionCallback;

            if (__isInstant && GameManager.main.RenderingPass())
            {
                callback();
                return;
            }

            callback(); // 바로 콜백 실행해서 다음으로 넘어가도록 처리 
            GameManager.main.isWaitingScreenTouch = false;

            if (string.IsNullOrEmpty(script_data) || !isValidCommand)
            {
                // 이펙트 올바르지 않은 명령어일때!
                //Debug.Log("No Effect in Row");

                GameManager.ShowMissingComponent(scriptRow.template, command);

                callback();
                return;
            }

            switch (command)
            {
                case GameConst.KR_SCREEN_EFFECT_TINT:
                    ScreenEffectManager.main.StartScreenEffectTint(effectColor, effectTime);
                    break;
                case GameConst.KR_SCREEN_EFFECT_TINT_BG:

                    ScreenEffectManager.main.StartBackgroundTint(effectColor, effectTime);
                    break;
                case GameConst.KR_SCREEN_EFFECT_TINT_CH:
                    ScreenEffectManager.main.StartCharacterTint(effectColor, effectTime);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLING:

                    if (paramArray == null)
                    {
                        paramArray = new string[2];
                        paramArray[0] = "타입=C";
                        paramArray[1] = "분포=3";
                    }

                 //   ScreenEffectManager.main.StartBlingEffect(paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FIRE:
                 //   ScreenEffectManager.main.StartFireEffect(paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE:
                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_BG:
                case GameConst.KR_SCREEN_EFFECT_GRAYSCALE_CH:
                  //  ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BROKEN:
                 //   ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_SHAKE:
                  //  ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_ANOMALY:
                  //  ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_DIZZY:
                  //  ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLUR:
                   // ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_FOG:
                case GameConst.KR_SCREEN_EFFECT_SCREEN_FOG:
                    //ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_HEAVYRAIN:
                   // ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;
                case GameConst.KR_SCREEN_EFFECT_RAIN:
                    //ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;
                case GameConst.KR_SCREEN_EFFECT_HEAVYSNOW:
                    //ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;
                case GameConst.KR_SCREEN_EFFECT_SNOW:
                    //ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;
                case GameConst.KR_SCREEN_EFFECT_FOCUS:
                    //ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_ZOOMIN:
                    ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;
                case GameConst.KR_SCREEN_EFFECT_ZOOMOUT:
                    ScreenEffectManager.main.StartScreenEffectCamera(command, null);
                    break;

                case GameConst.KR_SCREEN_EFFECT_LENS_FLARE:
                   // ScreenEffectManager.main.StartScreenEffectLensFlare(paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_CIRCLE_LIGHT:
                case GameConst.KR_SCREEN_EFFECT_HEX_LIGHT:
                   // ScreenEffectManager.main.StartScreenEffectLightDust(command);
                    break;

                case GameConst.KR_SCREEN_EFFECT_BLOOD_HIT:
                  //  ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_GLITCH:
                  //  ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;
                case GameConst.KR_SCREEN_EFFECT_GLITCH_SCREEN:
                  //  ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_REMINISCE:
                  //  ScreenEffectManager.main.StartScreenEffectCamera(command, paramArray);
                    break;

                case GameConst.KR_SCREEN_EFFECT_CAMERA_FLASH:
                    ScreenEffectManager.main.DirectiveFlash(paramArray);
                    break;

            }
        }

        /// <summary>
        /// 동작을 마무리합니다.
        /// </summary>
        public void EndAction() { }
       
    }
}