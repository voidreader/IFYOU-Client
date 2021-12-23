

namespace PIERStory
{
    /// <summary>
    /// 게임씬에서 사용하는 상수 모음
    /// </summary>
    public static class GameConst
    {
        
        #region 게임씬 통신 관련
        
        public const string FUNC_GET_EPISODE_SCRIPT = "getEpisodeScript";

        public const string NODE_LOADING = "loading";
        public const string NODE_LOADING_DETAIL = "loadingDetail";
        public const string NODE_SCRIPT = "script";
        #endregion


        #region 게임씬 플레이 관련
        
        public const string VOICE_MUTE = "voiceMute";
        public const string BGM_MUTE = "bgmMute";
        public const string SOUNDEFFECT_MUTE = "seMute";

        public const string AUTO_PLAY = "autoplay";

        public const float fastDelay = 3f;
        public const float normalDelay = 5f;
        public const float slowDelay = 7f;

        public const string SIGNAL_EPISODE_END = "episodeEnd";
        public const string SIGNAL_NEXT_DATA = "nextData";
        public const string SIGNAL_UPDATE_EPISODE = "updateCurrentData";
        public const string SIGNAL_NEXT_EPISODE = "nextEpisode";

        #region 게임 스크립트 관련

        public const string LAYER_UI = "UI";
        public const string LAYER_MODEL = "Models";
        public const string LAYER_ILLUST = "ILLUST";
        public const string LAYER_LIVE_OBJ = "LiveObj";
        public const string LAYER_MODEL_L = "Model-L";
        public const string LAYER_MODEL_C = "Model-C";
        public const string LAYER_MODEL_R = "Model-R";

        public const string MOTION3_JSON = ".motion3.json";

        public const string MODEL_TYPE_SPINE = "spine";

        public const string SPLIT_SCREEN_EFFECT = ":"; // 화면연출 데이터 구분자 
        public const string SPLIT_SCREEN_EFFECT_V = ","; // 화면연출 데이터 변수 구분자

        public const string SPLIT_OPTIONAL_PARAM = "`"; // 옵셔널 파라매터 구분자.
        public const string TAG_TARGET_EPISODE = "#"; // 에피소드 이동 태그 구분자.

        
        // 시선 방향
        public const string VIEWDIRECTION_LEFT = "left";
        public const string VIEWDIRECTION_RIGHT = "right";
        public const string VIEWDIRECTION_CENTER = "center";
        // 캐릭터 위치 관련
        public const string POS_LEFT = "L";
        public const string POS_CENTER = "C";
        public const string POS_RIGHT = "R";

        
        // 컬럼!
        public const string COL_SCRIPT_NO = "script_no";
        public const string COL_SCENE_ID = "scene_id";
        public const string COL_TEMPLATE = "template";
        public const string COL_SPEAKER = "speaker";
        public const string COL_DIRECTION = "direction";
        public const string COL_SCRIPT_DATA = "script_data";
        public const string COL_TARGET_SCENE_ID = "target_scene_id";
        public const string COL_REQUISITE = "requisite";
        public const string COL_CHARACTER_EXPRESSION = "character_expression";
        public const string COL_EMOTICON_EXPRESSION = "emoticon_expression";
        public const string COL_IN_EFFECT = "in_effect";
        public const string COL_OUT_EFFECT = "out_effect";
        public const string COL_BUBBLE_SIZE = "bubble_size";
        public const string COL_BUBBLE_POS = "bubble_pos";
        public const string COL_BUBBLE_HOLD = "bubble_hold";
        public const string COL_BUBBLE_REVERSE = "bubble_reverse";
        public const string COL_EMOTICON_SIZE = "emoticon_size";
        public const string COL_VOICE = "voice";
        public const string COL_SOUND_EFFECT = "sound_effect";
        public const string COL_AUTOPLAY_ROW = "autoplay_row";
        public const string COL_CONTROL = "control";

        public const string COL_DRESS_ID = "dress_id";
        public const string COL_DRESS_NAME = "dress_name";
        public const string COL_DEFAULT_DRESS_ID = "default_dress_id";
        public const string COL_DRESSMODEL_NAME = "dressmodel_name";
        public const string COL_MODEL_NAME = "model_name";


        // 제어 컬럼 기능 추가
        public const string ROW_CONTROL_ALTERNATIVE_NAME = "화자";    // 대체 이름
        public const string ROW_CONTROL_MAINTAIN = "유지";            // 라이브 오브제 지속시간
        public const string ROW_CONTROL_REVERSAL = "반전";            // 배경 반전


        // 스크립트 템플릿
        public const string TEMPLATE_NARRATION = "narration";
        public const string TEMPLATE_TALK = "talk";                 // 대화 
        public const string TEMPLATE_WHISPER = "whisper";           // 속삭임 
        public const string TEMPLATE_YELL = "yell";                 // 외침 
        public const string TEMPLATE_FEELING = "feeling";           // 속마음 
        public const string TEMPLATE_MONOLOGUE = "monologue";       // 독백
        public const string TEMPLATE_SPEECH = "speech";             // 중요대사

        public const string TEMPLATE_SELECTION = "selection";
        public const string TEMPLATE_PHONECALL = "phonecall";               //전화
        public const string TEMPLATE_PHONE_SELF = "phone_self";             //전화본인
        public const string TEMPLATE_PHONE_PARTNER = "phone_partner";       //전화상대
        public const string TEMPLATE_MESSAGE_RECEIVE = "message_receive";   //메시지 도착
        public const string TEMPLATE_MESSAGE_SELF = "message_self";         //메신저 본인
        public const string TEMPLATE_MESSAGE_PARTNER = "message_partner";   //메신저 상대
        public const string TEMPLATE_MESSAGE_CALL = "message_call";         //메신저 알림
        public const string TEMPLATE_MESSAGE_IMAGE = "message_image";       //메신저 이미지
        public const string TEMPLATE_MESSAGE_END = "message_end";           //메신저 종료
        public const string TEMPLATE_BACKGROUND = "background";
        public const string TEMPLATE_ILLUST = "illust";
        public const string TEMPLATE_LIVE_ILLUST = "live_illust"; // 라이브 일러스트
        public const string TEMPLATE_IMAGE = "image";
        public const string TEMPLATE_LIVE_OBJECT = "live_object";                   //라이브 오브제
        public const string TEMPLATE_LIVE_OBJECT_REMOVE = "live_object_remove";     //라이브 오브제 제거
        public const string TEMPLATE_IMAGE_REMOVE = "image_remove";
        public const string TEMPLATE_SCREEN_EFFECT = "screen_effect";
        public const string TEMPLATE_SCREEN_EFFECT_REMOVE = "screen_effect_remove";
        public const string TEMPLATE_EXIT = "exit";
        public const string TEMPLATE_DRESS = "dress";
        public const string TEMPLATE_MOVEIN = "move_in";            //장소 진입
        public const string TEMPLATE_MOVEOUT = "move_out";          //장소 이탈
        public const string TEMPLATE_FLOWTIME = "flow_time";        //시간 흐름
        public const string TEMPLATE_BGM = "bgm";                   //배경음
        public const string TEMPLATE_BGM_REMOVE = "bgm_remove";     //배경음 제거
        public const string TEMPLATE_FAVOR = "favor";               //호감도
        public const string TEMPLATE_MISSION = "mission";           //미션
        public const string TEMPLATE_ANGLE_MOVE = "angle_move";     //앵글 이동
        public const string TEMPLATE_CLEAR_SCREEN = "clear_screen"; //화면 정리


        // 화면 연출
        public const string KR_SCREEN_EFFECT_TINT = "틴트스크린"; //
        public const string KR_SCREEN_EFFECT_TINT_BG = "틴트";
        public const string KR_SCREEN_EFFECT_TINT_CH = "틴트인물";
        public const string KR_SCREEN_EFFECT_BROKEN = "깨짐";
        public const string KR_SCREEN_EFFECT_ANOMALY = "미정";
        public const string KR_SCREEN_EFFECT_DIZZY = "울렁임";
        public const string KR_SCREEN_EFFECT_BLUR = "블러";

        public const string KR_SCREEN_EFFECT_SHAKE = "흔들기";
        public const string KR_SCREEN_EFFECT_FOCUS = "집중선";
        public const string KR_SCREEN_EFFECT_HEAVYSNOW = "폭설";
        public const string KR_SCREEN_EFFECT_SNOW = "눈";
        public const string KR_SCREEN_EFFECT_HEAVYRAIN = "폭우";
        public const string KR_SCREEN_EFFECT_RAIN = "비";
        public const string KR_SCREEN_EFFECT_FLOWER = "꽃잎";
        public const string KR_SCREEN_EFFECT_FIRE = "불";

        public const string KR_SCREEN_EFFECT_BLOOD_HIT = "출혈";

        public const string KR_SCREEN_EFFECT_GLITCH = "글리치";
        public const string KR_SCREEN_EFFECT_GLITCH_SCREEN = "글리치스크린";

        public const string KR_SCREEN_EFFECT_REMINISCE = "회상";

        public const string KR_SCREEN_EFFECT_FOG = "안개";
        public const string KR_SCREEN_EFFECT_SCREEN_FOG = "스크린안개";

        public const string KR_SCREEN_EFFECT_BLING = "반짝이";
        public const string KR_SCREEN_EFFECT_ZOOMIN = "줌인";
        public const string KR_SCREEN_EFFECT_ZOOMOUT = "줌아웃";

        public const string KR_SCREEN_EFFECT_LENS_FLARE = "렌즈플레어";
        public const string KR_SCREEN_EFFECT_CIRCLE_LIGHT = "원형빛";
        public const string KR_SCREEN_EFFECT_HEX_LIGHT = "육각형빛";

        public const string KR_SCREEN_EFFECT_GRAYSCALE = "흑백";
        public const string KR_SCREEN_EFFECT_GRAYSCALE_BG = "흑백배경";
        public const string KR_SCREEN_EFFECT_GRAYSCALE_CH = "흑백인물";

        public const string KR_SCREEN_EFFECT_CAMERA_FLASH = "플래시";

        // 화면 연출 파라미터 설정값
        public const string KR_PARAM_VALUE_ANIMATION = "애니메이션";
        public const string KR_PARAM_VALUE_TIME = "시간";
        public const string KR_PARAM_VALUE_STRENGTH = "세기";
        public const string KR_PARAM_VALUE_FORCE = "강도";
        public const string KR_PARAM_VALUE_LEVEL = "단계";
        public const string KR_PARAM_VALUE_TYPE = "타입";
        public const string KR_PARAM_VALUE_DISTRIBUTION = "분포";
        public const string KR_PARAM_VALUE_DIR = "방향";
        public const string KR_PARAM_VALUE_SPEED = "속도";        // 사실 속력


        // 등장과 퇴장연출
        public const string INOUT_EFFECT_SCALEUP = "scaleup";
        public const string INOUT_EFFECT_SCALEDOWN = "scaledown";
        public const string INOUT_EFFECT_SHAKE = "shake";
        public const string INOUT_EFFECT_FADEIN = "fadein";
        public const string INOUT_EFFECT_FADEOUT = "fadeout";

        

        // 말풍선 세트 컬럼
        public const string COL_BUBBLE_SPRITE_ID = "bubble_sprite_id";
        public const string COL_BUBBLE_SPRITE_URL = "bubble_sprite_url";
        public const string COL_BUBBLE_SPRITE_KEY = "bubble_sprite_key";
        public const string COL_BUBBLE_OUTLINE_ID = "outline_sprite_id";
        public const string COL_BUBBLE_OUTLINE_URL = "outline_sprite_url";
        public const string COL_BUBBLE_OUTLINE_KEY = "outline_sprite_key";
        public const string COL_BUBBLE_TAIL_ID = "tail_sprite_id";
        public const string COL_BUBBLE_TAIL_URL = "tail_sprite_url";
        public const string COL_BUBBLE_TAIL_KEY = "tail_sprite_key";

        public const string COL_BUBBLE_TAIL_OUTLINE_ID = "tail_outline_sprite_id";
        public const string COL_BUBBLE_TAIL_OUTLINE_URL = "tail_outline_sprite_url";
        public const string COL_BUBBLE_TAIL_OUTLINE_KEY = "tail_outline_sprite_key";

        public const string COL_BUBBLE_R_TAIL_ID = "reverse_tail_sprite_id";
        public const string COL_BUBBLE_R_TAIL_URL = "reversed_tail_sprite_url";
        public const string COL_BUBBLE_R_TAIL_KEY = "reversed_tail_sprite_key";

        public const string COL_BUBBLE_R_TAIL_OUTLINE_ID = "reverse_tail_outline_sprite_id";
        public const string COL_BUBBLE_R_TAIL_OUTLINE_URL = "reverse_tail_outline_sprite_url";
        public const string COL_BUBBLE_R_TAIL_OUTLINE_KEY = "reverse_tail_outline_sprite_key";

        public const string COL_BUBBLE_TAG_ID = "tag_sprite_id";
        public const string COL_BUBBLE_TAG_URL = "tag_sprite_url";
        public const string COL_BUBBLE_TAG_KEY = "tag_sprite_key";

        public const string COL_MAIN_COLOR = "main_color";
        public const string COL_SUB_COLOR = "sub_color";

        public const string COL_DEFAULT = "default";


        public const string COLOR_BLACK_RGB = "000000FF";
        public const string COLOR_WHITE_RGB = "FFFFFFFF";

        public const float MODEL_PARENT_ORIGIN_POS_Y = -13f;        // Safe Area 없는 폰에서 모델 기준 좌표 
        public const float MODEL_PARENT_SAFEAREA_POS_Y = -13.8f;    // Safe Area 폰에서 위치 살짝 내림 

        public const float IMAGE_SCALE_SMALL = 0.67F; // 미니컷 이미지 게임에 맞게 고정 값

        #endregion

        #endregion
    }
}

