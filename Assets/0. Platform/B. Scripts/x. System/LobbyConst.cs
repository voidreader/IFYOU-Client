
using UnityEngine;
namespace PIERStory {

    // 로비에서 사용하는 상수들 모음
    public static class LobbyConst {

        public const string TITLE_IMAGE_URL = "title_image_url";
        public const string TITLE_IMAGE_KEY = "title_image_key";
        public const string POPUP_IMAGE_URL = "popup_image_url";
        public const string POPUP_IMAGE_KEY = "popup_image_key";
        public const string IFYOU_PROJECT_BANNER_URL = "ifyou_image_url";
        public const string IFYOU_PROJECT_BANNER_KEY = "ifyou_image_key";
        public const string IFYOU_PROJECT_MAIN_COLOR = "color_rgb";
        
        
        
        
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
        #endregion
        
        public static Color colorBlueBlue = new Color(0.384f,0.5176f,1,1);

    }
}