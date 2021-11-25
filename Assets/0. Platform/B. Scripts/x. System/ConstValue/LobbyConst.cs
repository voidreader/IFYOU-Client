
using UnityEngine;
namespace PIERStory {

    // 로비에서 사용하는 상수들 모음
    public static class LobbyConst {

        public const string TITLE_IMAGE_URL = "title_image_url";
        public const string TITLE_IMAGE_KEY = "title_image_key";
        public const string POPUP_IMAGE_URL = "popup_image_url";
        public const string POPUP_IMAGE_KEY = "popup_image_key";
        
        public const string EPISODE_SALE_PRICE = "sale_price";
        
        public const string IFYOU_PROJECT_BANNER_URL = "ifyou_image_url";
        public const string IFYOU_PROJECT_BANNER_KEY = "ifyou_image_key";
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

        public const string ILLUST_NAME = "illust_name";

        public const string PUBLIC_NAME = "public_name";
        public const string SUMMARY = "summary";

        public const string ENDING_TYPE = "ending_type";
        public const string COL_HIDDEN = "hidden";
        public const string COL_FINAL = "final";

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
        
        public const string SIGNAL_MOVE_STORY_DETAIL = "moveStoryDetail";
        public const string SIGNAL_PURCHASE_FREEPASS = "PurchaseFreepass";
        public const string SIGNAL_CLOSE_RESET = "closeEpisodeReset";
        public const string SIGNAL_UPDATE_EPISODE_SCENE_COUNT = "updateEpisodeSceneCount";
        public const string SIGNAL_CONNECT_SERVER = "connectingDone";
        public const string SIGNAL_ON_BACK_BUTTON = "OnBackButton";
        public const string SIGNAL_OFF_BACK_BUTTON = "OffBackButton";

        #endregion

        public static Color colorBlueBlue = new Color(0.384f,0.5176f,1,1);
       


    }
}