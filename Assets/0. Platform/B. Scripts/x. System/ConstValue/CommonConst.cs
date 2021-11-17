using UnityEngine;

namespace PIERStory
{
    /// <summary>
    /// Lobby, Game scene 공통으로 사용되는 상수 모음
    /// </summary>
    public static class CommonConst
    {
        public const string LIVE_SERVER_URL = "https://www.plop-story.xyz:443/"; // 서버 URL
        public const string TEST_SERVER_URL = "https://pierstory.info:6370/"; // 테스트 서버 URL
        // public const string TEST_SERVER_URL = "http://localhost:6370/"; // 테스트 서버 URL

        public const string CLIENT_URL = "client/";

        public const string FUNC = "func";
        public const string COL_PROJECT_ID = "project_id";
        public const string COL_EPISODE_ID = "episode_id";

        public const string COL_KO = "KO";
        public const string COL_EN = "EN";
        public const string COL_JA = "JA";
        public const string COL_ZH = "ZH";

        public const string COL_FILE_URL = "file_url";
        public const string COL_FILE_KEY = "file_key";

        public const string MODEL3_JSON = ".model3.json";

        public const string COL_IMAGE_NAME = "image_name";
        public const string COL_IMAGE_URL = "image_url";
        public const string COL_IMAGE_KEY = "image_key";
        public const string COL_GAME_SCALE = "game_scale";
        public const string COL_OFFSET_X = "offset_x";
        public const string COL_OFFSET_Y = "offset_y";

        public const string COL_ONETIME = "onetime";
        public const string COL_TICKET = "ticket";
        public const string COL_FREEPASS = "freepass";

        public const string POPUP_CONFIRM = "Confirm";
        public const string POPUP_SIMPLE_MESSAGE = "SimpleMessage";

        // JSON COLUMN
        public const string JSON_EPISODE_SCENE_HISTORY = "sceneProgress";
        public const string JSON_EPISODE_PURCHASE_HISTORY = "episodePurchase";

        public static Color COLOR_IMAGE_TRANSPARENT = new Color(1, 1, 1, 0);
        public static Color COLOR_BLACK_TRANSPARENT = new Color(0, 0, 0, 0);
        public static Color COLOR_GRAY_TRANSPARENT = new Color(0.26275f, 0.26275f, 0.26275f, 0);
    }
}