
using UnityEngine;
namespace PIERStory {

    // 로비에서 사용하는 상수들 모음
    public static class LobbyConst {

        public const string COL_LANG = "lang";

        public const string TITLE_IMAGE_URL = "title_image_url";
        public const string TITLE_IMAGE_KEY = "title_image_key";
        public const string POPUP_IMAGE_URL = "popup_image_url";
        public const string POPUP_IMAGE_KEY = "popup_image_key";
        
        public const string EPISODE_SALE_PRICE = "sale_price";
        public const string EPISODE_PRICE = "price";
        
        public const string IFYOU_PROJECT_BANNER_URL = "ifyou_image_url";
        public const string IFYOU_PROJECT_BANNER_KEY = "ifyou_image_key";
        public const string IFYOU_PROJECT_THUMBNAIL_URL = "ifyou_thumbnail_url";
        public const string IFYOU_PROJECT_THUMBNAIL_KEY = "ifyou_thumbnail_key";
        public const string IFYOU_PROJECT_CIRCLE_URL = "circle_image_url";
        public const string IFYOU_PROJECT_CIRCLE_KEY = "circle_image_key";
        public const string IFYOU_PROJECT_MAIN_COLOR = "color_rgb";
        public const string STORY_TITLE = "title";
        public const string STORY_ID = "project_id";
        public const string STORY_BUBBLE_ID = "bubble_set_id";
        public const string STORY_PROJECT_PROGRESS = "project_progress";
        public const string STORY_IS_PLAYING = "is_playing";
        
        public const string STORY_EPISODE_ID = "episode_id";
        public const string STORY_EPISODE_TYPE = "episode_type";
        public const string STORY_EPISODE_TITLE = "title";

        public const string THUMBNAIL_URL = "thumbnail_url";
        public const string THUMBNAIL_KEY = "thumbnail_key";

        public const string BANNER_URL = "banner_url";
        public const string BANNER_KEY = "banner_key";

        public const string ILLUST_NAME = "illust_name";

        public const string PUBLIC_NAME = "public_name";
        public const string SUMMARY = "summary";
        public const string WRITER = "writer";
        
        public const string ORIGINAL = "original";
        
        public const string SORTKEY = "sortkey";
        
        public const string IS_LOCK = "is_lock";
        public const string IS_CREDIT = "is_credit";

        public const string ENDING_TYPE = "ending_type";
        public const string COL_HIDDEN = "hidden";
        public const string COL_FINAL = "final";

        public const string MISSION_NAME = "mission_name";

        public const string NODE_DETAIL = "detail";

        // 프로필 꾸미기 관련 const string
        public const string FUNC_GET_PROFILE_CURRENCY_OWN_LIST = "getProfileCurrencyOwnList";       // 소유한 프로필 재화 리스트
        public const string FUNC_GET_PROFILE_CURRENCY_CURRENT = "getProfileCurrencyCurrent";        // 현재 저장된 프로필 재화 정보
        public const string FUNC_USER_PROFILE_SAVE = "userProfileSaveVer2";
        public const string FUNC_SAVE_USER_STORY_PROFILE = "saveUserStoryProfile";
        public const string NODE_PORTRAIT = "portrait";
        public const string NODE_FRAME = "frame";
        public const string NODE_WALLPAPER = "wallpaper";
        public const string NODE_STANDING = "standing";
        public const string NODE_BADGE = "badge";
        public const string NODE_STICKER = "sticker";
        public const string NODE_BUBBLE = "bubble";
        public const string NODE_TEXT = "text";
        public const string NODE_TEXT_LIST = "textList";
        public const string NODE_CURRENCY = "currency";
        public const string NODE_CURRENCY_LIST = "currencyList";
        public const string NODE_SORTING_ORDER = "sorting_order";
        public const string NODE_CURRENCY_TYPE = "currency_type";
        public const string NODE_ICON_URL = "icon_url";
        public const string NODE_ICON_KEY = "icon_key";
        public const string NODE_CURRENCY_URL = "currency_url";
        public const string NODE_CURRENCY_KEY = "currency_key";
        public const string NODE_TOTAL_COUNT = "total_cnt";
        public const string NODE_CURRENT_COUNT = "current_cnt";
        public const string NODE_POS_X = "pos_x";
        public const string NODE_POS_Y = "pos_y";
        public const string NODE_WIDTH = "width";
        public const string NODE_HEIGHT = "height";
        public const string NODE_PRFILE_SCALE = "profile_scale";
        public const string NODE_ANGLE = "angle";
        public const string NODE_INPUT_TEXT = "input_text";
        public const string NODE_FONT_SIZE = "font_size";
        public const string NODE_COLOR_RGB = "color_rgb";


        // 프로모션 관련
        public const string NODE_PROMOTION_TYPE = "promotion_type";
        public const string COL_PROJECT = "project";
        public const string COL_PAGE = "page";
        public const string NODE_PROMOTION_BANNER_URL = "promotion_banner_url";
        public const string NODE_PROMOTION_BANNER_KEY = "promotion_banner_key";
        public const string COL_IFYOU = "ifyou";


        // 이프유플레이 관련
        public const string NODE_ATTENDANCE_MISSION = "attendanceMission";
        public const string NODE_USER_INFO = "user_info";
        public const string NODE_CONTINUOUS_ATTENDANCE = "continuous_attendance";
        public const string NODE_ATTENDANCE = "attendance";
        public const string NODE_DAY_SEQ = "day_seq";
        public const string NODE_DAILY_MISSION = "dailyMission";


        public const string NODE_ICON_IMAGE_URL = "icon_image_url";
        public const string NODE_ICON_IMAGE_KEY = "icon_image_key";


        #region 재화 
        public const string COIN = "coin";
        public const string GEM = "gem";
        public const string TICKET = "Ticket";
        public const string ONETIME = "OneTime";
        
        public const string FREEPASS = "Free";
        #endregion
        
        #region 시그널
        public const string STREAM_IFYOU = "IFYOU";
        public const string STREAM_GAME = "Game";
        public const string STREAM_COMMON = "Common";
        public const string STREAM_TOP = "Top";
        
        public const string SIGNAL_MOVE_STORY_DETAIL = "moveStoryDetail";
        public const string SIGNAL_PURCHASE_FREEPASS = "PurchaseFreepass";
        public const string SIGNAL_CLOSE_RESET = "closeEpisodeReset";
        public const string SIGNAL_UPDATE_EPISODE_SCENE_COUNT = "updateEpisodeSceneCount";
        public const string SIGNAL_CONNECT_SERVER = "connectingDone";
        public const string SIGNAL_MOVE_DECO_MODE = "moveDecoMode";
        public const string SIGNAL_SAVE_PROFILE_DECO = "saveProfileDeco";

        public const string SIGNAL_GAME_BEGIN = "GameBegin";

        public const string SIGNAL_EPISODE_RESET = "EpisodeReset";
        
        public const string SIGNAL_LANGUAGE = "language";
        public const string SIGNAL_COUPON = "coupon";
        
        
        public const string TOP_SIGNAL_SHOW_BACKGROUND = "topSignalShowBackground"; // 상단 배경 보여주기
        // public const string TOP_SIGNAL_HIDE_BACKGROUND = "topSignalHideBackground"; // 상단 배경 감추기
        public const string TOP_SIGNAL_VIEW_NAME_EXIST = "topSignalShowViewName"; // 상단 뷰 이름 보여주기
        public const string TOP_SIGNAL_VIEW_NAME = "topSignalViewName";             // 상단 뷰 이름
        
        public const string TOP_SIGNAL_SHOW_PROPERTY_GROUP = "topSignalShowPropertyGroup"; // 재화 그룹 보여주기
        public const string TOP_SIGNAL_SHOW_MAIL_BUTTON = "topSignalShowMailButton";        // 재화 그룹의 메일버튼 활성화
        
        public const string TOP_SIGNAL_SHOW_BACK_BUTTON = "topSignalShowBackButton"; // 백버튼 처리

        public const string TOP_SIGNAL_CHANGE_OWNER = "topSignalChangeOwner"; // 오너 변경 
        public const string TOP_SIGNAL_RECOVER = "topSignalRecover"; // 상단 상태 복원 
        public const string TOP_SIGNAL_SAVE_STATE = "topSignalSaveState"; // 상단 상태 저장 

        public const string TOP_SIGNAL_ATTENDANCE = "topSignalAttendance";
        public const string TOP_SIGNAL_PARENT = "topParent";

        public const string SIGNAL_SHOW_ILLUSTDETAIL = "showIllustDetail";
        public const string SIGNAL_SHOW_ENDINGDETAIL = "showEndingDetail";
        public const string SIGNAL_ENDINGDATA = "endingData";
        
        public const string SIGNAL_ENDING_PLAY = "EndingPlay";
        public const string SIGNAL_INTRODUCE = "introduceStory";

        #endregion

        #region 팝업 이름

        public const string POPUP_ATTENDANCE = "Attendance";
        public const string POPUP_COUPON = "Coupon";
        public const string POPUP_GRADE_BENEFIT_INFO = "GradeBenefitInfo";
        public const string POPUP_GRADE_EXP = "GradeExp";
        public const string POPUP_GRADE_UP = "GradeUp";
        public const string POPUP_ENDING_HINT = "EndingHint";
        public const string POPUP_SPECIAL_HINT = "SpecialHint";
        public const string POPUP_MISSION_HINT = "MissionHint";

        public const string POPUP_ALL_CLEAR_REWARD_LIST = "AllClearRewardList";
        public const string POPUP_MISSION_CLEAR_REWARD = "MissionClearReward";

        public const string POPUP_COIN_STAR_HISTORY = "CoinStarHistory";
        public const string POPUP_DATA_MANAGER = "DataManager";

        #endregion


        public static Color colorBlueBlue = new Color(0.384f,0.5176f,1,1);

        #region PlayerPrefs Key Value

        public const string KEY_SEARCH_RECORD = "searchRecord";
        
        
        // * 스토리 컨텐츠 관련 키 
        public const string KEY_GALLERY_DATA = "keyGalleryData";
        public const string KEY_OPEN_ENDING_COUNT = "keyEndingData";
        public const string KEY_SELECTION_DATA = "keySelectionData";
        public const string KEY_MISSION_DATA = "keyMissionData";
        

        #endregion

    }
}