
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
        
        public const string SORTKEY = "sortkey";
        
        public const string IS_LOCK = "is_lock";
        public const string IS_CREDIT = "is_credit";

        public const string ENDING_TYPE = "ending_type";
        public const string COL_HIDDEN = "hidden";
        public const string COL_FINAL = "final";

        public const string MISSION_NAME = "mission_name";

        public const string NODE_DETAIL = "detail";
        

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
        
        
        public const string SIGNAL_EPISODE_START = "EpisodeStart";
        public const string SIGNAL_EPISODE_RESET = "EpisodeReset";
        
        public const string TOP_SIGNAL_SHOW_BACKGROUND = "topSignalShowBackground"; // 상단 배경 보여주기
        // public const string TOP_SIGNAL_HIDE_BACKGROUND = "topSignalHideBackground"; // 상단 배경 감추기
        public const string TOP_SIGNAL_VIEW_NAME_EXIST = "topSignalShowViewName"; // 상단 뷰 이름 보여주기
        public const string TOP_SIGNAL_VIEW_NAME = "topSignalViewName";             // 상단 뷰 이름
        
        public const string TOP_SIGNAL_SHOW_PROPERTY_GROUP = "topSignalShowPropertyGroup"; // 재화 그룹 보여주기
        // public const string TOP_SIGNAL_HIDE_PROPERTY_GROUP = "topSignalHidePropertyGroup"; // 재화 그룹 감추기 
        
        public const string TOP_SIGNAL_SHOW_BACK_BUTTON = "topSignalShowBackButton"; // 백버튼 처리

        public const string TOP_SIGNAL_CHANGE_OWNER = "topSignalChangeOwner"; // 오너 변경 

        public const string SIGNAL_SHOW_ILLUSTDETAIL = "showIllustDetail";
        public const string SIGNAL_SHOW_ENDINGDETAIL = "showEndingDetail";
        public const string SIGNAL_ENDINGDATA = "endingData";

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