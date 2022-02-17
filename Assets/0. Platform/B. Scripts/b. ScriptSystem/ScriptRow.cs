using System;
using UnityEngine;

using LitJson;

namespace PIERStory
{
    [Serializable]
    public class ScriptRow
    {
        public JsonData rowData = null;

        Action OnRowInitialized = delegate { };     // ScriptRow 초기화 완료 후 호출 처리

        public string resource_key = string.Empty; // 캐릭터 모델을 제외한 리소스의 key 입니다. 
        public string speaker_key = string.Empty;  // 캐릭터 모델 키 


        public int project_id = -1;
        public int episode_id = -1;
        public long script_no = -1;

        public string scene_id = string.Empty;              // 사건ID
        public string template = string.Empty;              // 템플릿
        public string speaker = string.Empty;               // 화자 
        public string direction = string.Empty;             // 화자 등장 방향 
        public string script_data = string.Empty;           // 데이터
        public string target_scene_id = string.Empty;       // 이동  
        public string requisite = string.Empty;             // 조건 
        public string character_expression = string.Empty;  // 캐릭터 표현 
        public string emoticon_expression = string.Empty;   // 이모티콘 표현 


        public string in_effect = string.Empty;     // 등장 연출
        public string out_effect = string.Empty;    // 퇴장 연출

        public int bubble_size = -1;    // 말풍선 사이즈 
        public int bubble_pos = -1;     // 말풍선 위치 
        public int bubble_hold = -1;    // 말풍선 유지 
        public int bubble_reverse = -1; // 말꼬리 반전 


        public string voice = string.Empty; // 음성
        public string se = string.Empty;    // sound effect

        public int autoplay_row = -1;

        public string control = string.Empty;                   // 행 '제어' 파라매터 2021.07.02 추가
        public string[] controlParams = null;                   // 제어파라매터 배열 
        public string controlAlternativeName = string.Empty;    // 대체 이름
        public string controlMouthCommand = string.Empty; // 립싱크 제어 
        public string controlCallCommand = string.Empty;        // 전화 제어
        

        public string selection_group = string.Empty;
        public string selection_no = string.Empty;


        // 액션 인터페이스 
        public IRowAction rowAction = null;

        #region Properties

        /// <summary>
        /// 화자만 체크
        /// </summary>
        public bool IsValidSpeaker
        {
            // 이 두 컬럼이 제작자가 작성할때 공란으로 작성하는 경우가 꽤 있다.
            // 이 Property를 통해 걸러내도록 한다. 
            get
            {
                if (string.IsNullOrEmpty(speaker))
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// 캐릭터 표현이 사용되는지 체크합니다. 
        /// </summary>
        public bool IsSpeakable
        {
            get
            {
                if (template != GameConst.TEMPLATE_TALK
                    && template != GameConst.TEMPLATE_YELL
                    && template != GameConst.TEMPLATE_FEELING
                    && template != GameConst.TEMPLATE_WHISPER
                    && template != GameConst.TEMPLATE_MONOLOGUE
                    && template != GameConst.TEMPLATE_SPEECH)
                    return false; // 위 템플릿들이 아니면 false


                // 화자 없으면 노노
                if (string.IsNullOrEmpty(speaker))
                    return false;

                // 캐릭터 표현 없으면 안됨!
                if (string.IsNullOrEmpty(character_expression))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// 유효한 의상 템플릿인가요?
        /// </summary>
        public bool IsValidDress
        {
            get
            {

                if (template != GameConst.TEMPLATE_DRESS)
                    return false;

                // 화자 없으면 노노
                if (string.IsNullOrEmpty(speaker))
                    return false;

                // 데이터 없으면 노노 
                if (string.IsNullOrEmpty(script_data))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// 이미지 리소스 template 갖고 있는 행인지 판단한다.
        /// </summary>
        public bool hasImageResource
        {
            get
            {
                // 배경, 일러스트, 이미지 only
                if (template.Equals(GameConst.TEMPLATE_BACKGROUND) || template.Equals(GameConst.TEMPLATE_ILLUST) || template.Equals(GameConst.TEMPLATE_IMAGE) || template.Equals(GameConst.TEMPLATE_LIVE_OBJECT) || template.Equals(GameConst.TEMPLATE_LIVE_ILLUST))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 이미지 리소스 행의 유효성 체크 
        /// </summary>
        public bool CheckValidImageRow
        {
            get
            {
                return !string.IsNullOrEmpty(script_data);
            }
        }

        /// <summary>
        /// 이모티콘 표현 정보 있는지!
        /// </summary>
        public bool hasEmoticonResource
        {
            // 화자가 있고, 이모티콘 표현 정보 있을 경우 true 
            get
            {
                return !string.IsNullOrEmpty(speaker) && !string.IsNullOrEmpty(emoticon_expression);
            }
        }

        /// <summary>
        /// 이모티콘이 가능한 템플릿만..!
        /// </summary>
        public bool CheckValidEmoticonRow
        {
            get
            {
                if (template == GameConst.TEMPLATE_TALK
                    || template == GameConst.TEMPLATE_WHISPER
                    || template == GameConst.TEMPLATE_YELL
                    || template == GameConst.TEMPLATE_FEELING
                    || template == GameConst.TEMPLATE_SPEECH
                    || template == GameConst.TEMPLATE_MONOLOGUE
                    || template == GameConst.TEMPLATE_MESSAGE_PARTNER
                    || template == GameConst.TEMPLATE_MESSAGE_SELF
                    || template == GameConst.TEMPLATE_PHONE_PARTNER
                    || template == GameConst.TEMPLATE_PHONE_SELF)
                    return true;

                return false;
            }
        }

        #endregion

        public ScriptRow(JsonData __j, Action __cb)
        {
            rowData = __j;
            OnRowInitialized = __cb;

            InitJson();
        }

        void InitJson()
        {
            project_id = ParseCol<int>(CommonConst.COL_PROJECT_ID);
            episode_id = ParseCol<int>(CommonConst.COL_EPISODE_ID);
            script_no = ParseCol<long>(GameConst.COL_SCRIPT_NO);
            scene_id = ParseCol<string>(GameConst.COL_SCENE_ID);
            template = ParseCol<string>(GameConst.COL_TEMPLATE);

            speaker = ParseCol<string>(GameConst.COL_SPEAKER);
            direction = ParseCol<string>(GameConst.COL_DIRECTION);

            if (!string.IsNullOrEmpty(direction))
            {
                // 어떤 값이 들어오던 대문자 변환을 한다
                direction = direction.ToUpper().Trim();

                // 대문자 변환을 했는데 L도 아니고 R도 아니면 무조건 C
                if (!direction.Equals(GameConst.POS_LEFT) && !direction.Equals(GameConst.POS_RIGHT))
                    direction = GameConst.POS_CENTER;

            }

            script_data = ParseCol<string>(GameConst.COL_SCRIPT_DATA);
            target_scene_id = ParseCol<string>(GameConst.COL_TARGET_SCENE_ID);
            requisite = ParseCol<string>(GameConst.COL_REQUISITE);

            character_expression = ParseCol<string>(GameConst.COL_CHARACTER_EXPRESSION);
            emoticon_expression = ParseCol<string>(GameConst.COL_EMOTICON_EXPRESSION);

            in_effect = ParseCol<string>(GameConst.COL_IN_EFFECT);
            out_effect = ParseCol<string>(GameConst.COL_OUT_EFFECT);


            bubble_size = ParseCol<int>(GameConst.COL_BUBBLE_SIZE); //
            bubble_pos = ParseCol<int>(GameConst.COL_BUBBLE_POS);
            bubble_hold = ParseCol<int>(GameConst.COL_BUBBLE_HOLD);
            bubble_reverse = ParseCol<int>(GameConst.COL_BUBBLE_REVERSE);

            voice = ParseCol<string>(GameConst.COL_VOICE);
            se = ParseCol<string>(GameConst.COL_SOUND_EFFECT);

            autoplay_row = ParseCol<int>(GameConst.COL_AUTOPLAY_ROW);

            control = ParseCol<string>(GameConst.COL_CONTROL);
            selection_group = ParseCol<string>(GameConst.COL_SELECTION_GROUP);
            selection_no = ParseCol<string>(GameConst.COL_SELECTION_NO);

            SetControlParams();

            // 리소스 키 만들기
            CreateResourceKey();

            HandlePreProcessTemplate();

        }

        /// <summary>
        /// 제어 컬럼 파라미터 값 도출
        /// </summary>
        void SetControlParams()
        {
            if (string.IsNullOrEmpty(control))
                return;

            // controlParams 뽑아내고, 
            controlParams = control.Split(GameConst.SPLIT_SCREEN_EFFECT_V[0]);

            // 제어 요소 가져온다.

            // 대체 이름! (화자=철수)
            GetParam<string>(controlParams, GameConst.ROW_CONTROL_ALTERNATIVE_NAME, ref controlAlternativeName);

            // 라이브 오브제 지속시간 관련 (유지=2)
            GetParam<string>(controlParams, GameConst.ROW_CONTROL_MAINTAIN, ref controlAlternativeName);

            // 나레이션, 배경 관련 (반전=배경)
            GetParam<string>(controlParams, GameConst.ROW_CONTROL_REVERSAL, ref controlAlternativeName);
            
            // 립싱크 관련 (입=닫아)
            GetParam<string>(controlParams, GameConst.ROW_CONTROL_MOUTH, ref controlMouthCommand);

            // 전화 관련 (전화 = 받기, 끊기, 선택, 걸기, 제거)
            GetParam<string>(controlParams, GameConst.ROW_CONTROL_PHONE, ref controlCallCommand);

            // 게임 메시지 관련
            GetParam<string>(controlParams, GameConst.ROW_CONTROL_STATE, ref controlAlternativeName);
        }

        void CreateResourceKey()
        {
            // 템플릿 없으면 안함! 
            if (string.IsNullOrEmpty(template))
            {
                resource_key = string.Empty;
                return;
            }

            // 일반 이미지 리소스 
            if (hasImageResource && CheckValidImageRow)
            {
                resource_key = string.Format("{0}/{1}", template, script_data);
                return;
            }

            // 이모티콘 
            if (hasEmoticonResource && CheckValidEmoticonRow)
                resource_key = string.Format("{0}/{1}", speaker, emoticon_expression.ToString());

            // 모델 
            if (IsSpeakable)
            {
                speaker_key = string.Format("{0}/{1}/{2}", "model", template, speaker);

                if (string.IsNullOrEmpty(resource_key))
                    resource_key = speaker_key;
            }
        }

        public virtual void HandlePreProcessTemplate()
        {
            switch (template)
            {
                case GameConst.TEMPLATE_ANGLE_MOVE:
                    rowAction = new RowActionAngleMove(this);
                    break;

                case GameConst.TEMPLATE_BACKGROUND:
                    rowAction = new RowActionBG(this);
                    break;

                case GameConst.TEMPLATE_BGM:
                    rowAction = new RowActionBGM(this);
                    break;

                case GameConst.TEMPLATE_BGM_REMOVE:
                    rowAction = new RowActionBGMRemove(this);
                    break;

                case GameConst.TEMPLATE_CLEAR_SCREEN:
                    rowAction = new RowActionClearScreen(this);
                    break;

                case GameConst.TEMPLATE_DRESS:
                    rowAction = new RowActionDress(this);
                    break;

                case GameConst.TEMPLATE_EXIT:
                    rowAction = new RowActionExit(this);
                    break;

                case GameConst.TEMPLATE_FAVOR:
                    rowAction = new RowActionFavor(this);
                    break;

                case GameConst.TEMPLATE_FLOWTIME:
                    rowAction = new RowActionFlowTime(this);
                    break;

                case GameConst.TEMPLATE_GAME_MESSAGE:
                    rowAction = new RowActionGameMessage(this);
                    break;

                case GameConst.TEMPLATE_ILLUST:
                case GameConst.TEMPLATE_LIVE_ILLUST:
                    rowAction = new RowActionIllust(this, template);
                    break;

                case GameConst.TEMPLATE_IMAGE:
                    rowAction = new RowActionImage(this);
                    break;

                case GameConst.TEMPLATE_IMAGE_REMOVE:
                    rowAction = new RowActionImageRemove(this);
                    break;

                case GameConst.TEMPLATE_LIVE_OBJECT:
                    rowAction = new RowActionLiveObject(this);
                    break;

                case GameConst.TEMPLATE_LIVE_OBJECT_REMOVE:
                    rowAction = new RowActionLiveObjectRemove(this);
                    break;

                case GameConst.TEMPLATE_MESSAGE_RECEIVE:
                case GameConst.TEMPLATE_MESSAGE_CALL:
                case GameConst.TEMPLATE_MESSAGE_PARTNER:
                case GameConst.TEMPLATE_MESSAGE_SELF:
                case GameConst.TEMPLATE_MESSAGE_IMAGE:
                case GameConst.TEMPLATE_MESSAGE_END:
                    rowAction = new RowActionMessenger(this);
                    break;

                case GameConst.TEMPLATE_MISSION:
                    rowAction = new RowActionMission(this);
                    break;

                case GameConst.TEMPLATE_MOVEIN:
                    rowAction = new RowActionMoveIn(this);
                    break;

                case GameConst.TEMPLATE_MOVEOUT:
                    rowAction = new RowActionMoveOut(this);
                    break;

                case GameConst.TEMPLATE_NARRATION:
                    rowAction = new RowActionNarration(this);
                    break;

                case GameConst.TEMPLATE_PHONECALL:
                case GameConst.TEMPLATE_PHONE_PARTNER:
                case GameConst.TEMPLATE_PHONE_SELF:
                    rowAction = new RowActionPhoneCall(this);
                    break;

                case GameConst.TEMPLATE_SCREEN_EFFECT:
                    rowAction = new RowActionScreenEffect(this);
                    break;

                case GameConst.TEMPLATE_SCREEN_EFFECT_REMOVE:
                    rowAction = new RowActionScreenEffectRemove(this);
                    break;

                case GameConst.TEMPLATE_SELECTION:
                    rowAction = new RowActionSelection(this);
                    break;

                case GameConst.TEMPLATE_TALK:
                case GameConst.TEMPLATE_FEELING:
                case GameConst.TEMPLATE_MONOLOGUE:
                case GameConst.TEMPLATE_SPEECH:
                case GameConst.TEMPLATE_WHISPER:
                case GameConst.TEMPLATE_YELL:
                    rowAction = new RowActionTalk(this);
                    break;
            }

            OnRowInitialized?.Invoke();
        }

        #region 액션 처리

        /// <summary>
        /// 액션 처리 시작
        /// </summary>
        /// <param name="__cb">콜백</param>
        /// <param name="__isInstant">즉시실행 여부. 스킵 사용여부</param>
        public virtual void ProcessRowAction(Action __cb, bool __isInstant = false)
        {
            // 상황에 대한 처리
            if (!string.IsNullOrEmpty(scene_id))
            {
                GameManager.main.CheckSkipable(scene_id);

                // 입력된 사건ID와 입력했던 사건ID가 비교되는 순간은 사건ID 한 개가 완료된 것
                if (!GameManager.main.currentSceneId.Equals(scene_id))
                {
                    // 처음 currentSceneId는 비어있기 때문에 통신하지 않음
                    // 완료된 사건ID를 가지고 통신해준다
                    if (!string.IsNullOrEmpty(GameManager.main.currentSceneId))
                        UserManager.main.UpdateSceneIDRecord(GameManager.main.currentSceneId);

                    // 통신을 보내줬으니 사건ID를 갱신
                    GameManager.main.currentSceneId = scene_id;
                }

                // isResumePlay가 해제되기 전까지는 통신을 보내지 않는다
                // 이어하기로 들어온 경우 skip을 통해서 마지막 저장된 위치까지 플레이하기 때문에 또 통신을 할 필요가 없음. 
                if (!GameManager.isResumePlay)
                    NetworkLoader.main.UpdateUserProjectCurrent(StoryManager.main.CurrentEpisodeID, scene_id, script_no);
            }


            if (rowAction == null)
            {
                __cb?.Invoke();
                return;
            }

            // 액션 시작시에는 무조건 true로 변경, 풀어야하는 경우는 각 액션에서 개별적으로 처리한다.
            GameManager.main.isThreadHold = true;
            GameManager.main.isWaitingScreenTouch = true;

            // 자동 진행 처리 해제해준다. 
            if (autoplay_row > 0 && template != GameConst.TEMPLATE_EXIT)
            {
                // Debug.Log("Wait Off [ProcessRowAction]");
                GameManager.main.isThreadHold = false;
                GameManager.main.isWaitingScreenTouch = false;
            }

            // 다음 행으로 넘어갔으면 재생중이던 보이스는 정지
            GameManager.main.SoundGroup[1].StopAudioClip();

            // 자동 진행이 아닐때만 재생
            if (autoplay_row < 1)
            {
                if (!string.IsNullOrEmpty(voice))
                {
                    // 21.09.27 대화 관련 템플릿에서는 다르도록 처리
                    switch (template)
                    {
                        case GameConst.TEMPLATE_TALK:
                        case GameConst.TEMPLATE_SPEECH:
                        case GameConst.TEMPLATE_WHISPER:
                        case GameConst.TEMPLATE_YELL:
                        case GameConst.TEMPLATE_MONOLOGUE:
                        case GameConst.TEMPLATE_FEELING:
                            // GameBubbleCtrl에서 재생한다
                            // 그러니까 여기서는 아무것도 안함~
                            break;
                        case GameConst.TEMPLATE_FLOWTIME:
                            // 21.10.12 시간흐름은 글자가 나올 때 재생시키자
                            break;
                        default:
                            GameManager.main.SoundGroup[1].PlayVoice(voice);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(se) && !GameManager.main.useSkip)
                    GameManager.main.SoundGroup[2].PlaySoundEffect(se);
            }

            rowAction.DoAction(__cb, __isInstant);
        }


        /// <summary>
        /// 액션 종료 처리 
        /// </summary>
        public void ExitRowAction()
        {
            if (rowAction == null)
                return;

            rowAction.EndAction();
        }

        #endregion

        #region 컬럼 값 가져오기

        /// <summary>
        /// 컬럼 값 가져오기 
        /// </summary>
        T ParseCol<T>(string __columnName)
        {
            T ret;
            if (!TryParseCol(__columnName, out ret))
            {
                //Debug.LogError("Error in ParseCol with " + __columnName);
            }

            return ret;

        }

        /// <summary>
        /// 컬럼 파싱!
        /// </summary>
        public bool TryParseCol<T>(string __columnName, out T __val)
        {

            // 빈 컬럼인지 체크 
            if (IsEmptyCol(__columnName))
            {

                __val = default(T);
                return false;
            }

            if (TryParse<T>(rowData[__columnName].ToString(), out __val))
                return true;
            else
            {
                Debug.LogError("Parsing Error in " + __columnName);
                return false;
            }

        }

        /// <summary>
        /// 빈 컬럼인지 체크 
        /// </summary>
        public bool IsEmptyCol(string __columnName)
        {
            if (!rowData.ContainsKey(__columnName))
            {
                // Debug.LogError("[" + __columnName + "] required!");
                return true;
            }


            if (rowData[__columnName] == null || string.IsNullOrEmpty(rowData[__columnName].ToString()))
                return true;

            return false;
        }

        public static bool TryParse<T>(string str, out T val)
        {
            try
            {
                System.Type type = typeof(T);
                if (type == typeof(string))
                {
                    val = (T)(object)str;
                }
                else if (type.IsEnum)
                {
                    val = (T)System.Enum.Parse(typeof(T), str);
                }
                else if (type == typeof(int))
                {
                    val = (T)(object)int.Parse(str);
                }
                /*
                else if (type == typeof(float))
                {
                    val = (T)(object)WrapperUnityVersion.ParseFloatGlobal(str);
                }
                else if (type == typeof(double))
                {
                    val = (T)(object)WrapperUnityVersion.ParseDoubleGlobal(str);
                }
                */
                else if (type == typeof(bool))
                {
                    val = (T)(object)bool.Parse(str);
                }
                else
                {
                    System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                    val = (T)converter.ConvertFromString(str);
                }
                return true;
            }
            catch
            {
                val = default(T);
                return false;
            }
        }

        /// <summary>
        /// 파라미터 값 도출
        /// </summary>
        public static void GetParam<T>(string[] __params, string __paramName, ref T v)
        {
            string paramValue = string.Empty;

            for (int i = 0; i < __params.Length; i++)
            {

                if (__params[i].Contains(__paramName + "="))
                {
                    paramValue = __params[i].Split('=')[1];
                    try
                    {
                        v = (T)Convert.ChangeType(paramValue, typeof(T));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        v = default(T);
                    }
                }
            }
        }


        #endregion
    }

}