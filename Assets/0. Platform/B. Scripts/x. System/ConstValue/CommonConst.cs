﻿using UnityEngine;

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
        public const string COL_USERKEY = "userkey";

        public const string COL_PROJECT_ID = "project_id";
        public const string COL_EPISODE_ID = "episode_id";
        public const string COL_EPISODE_TYPE = "episode_type";
        public const string COL_EPISODE_NO = "episode_no";
        public const string COL_CHAPTER = "chapter";
        public const string COL_ENDING = "ending";
        public const string COL_SIDE = "side";
        public const string COL_TITLE = "title";

        public const string COL_PURCHASE_TYPE = "purchase_type";

        public const string COL_KO = "KO";
        public const string COL_EN = "EN";
        public const string COL_JA = "JA";
        public const string COL_ZH = "ZH";
        public const string COL_AR = "AR";

        public const string NONE = "none";

        public const string COL_FILE_URL = "file_url";
        public const string COL_FILE_KEY = "file_key";

        public const string MODEL3_JSON = ".model3.json";
        public const string MOTION_NAME = "motion_name";

        public const string COL_PUBLIC_NAME = "public_name";
        

        public const string COL_IMAGE_NAME = "image_name";
        public const string COL_IMAGE_URL = "image_url";
        public const string COL_IMAGE_KEY = "image_key";
        public const string COL_GAME_SCALE = "game_scale";
        public const string COL_OFFSET_X = "offset_x";
        public const string COL_OFFSET_Y = "offset_y";

        public const string ILLUST_TYPE = "illust_type";
        public const string ILLUST_OPEN = "illust_open";

        public const string MODEL_TYPE_LIVE2D = "live2d";

        public const string SOUND_NAME = "sound_name";
        public const string SOUND_URL = "sound_url";
        public const string SOUND_KEY = "sound_key";

        public const string IS_OPEN = "is_open";

        public const string NODE_QUANTITY = "quantity";

        public const string COL_ONETIME = "onetime";
        public const string COL_TICKET = "ticket";
        public const string COL_FREEPASS = "freepass";


        // Popup name
        public const string POPUP_CONFIRM = "Confirm";
        public const string POPUP_MESSAGE_ALERT = "MessageAlert";
        public const string POPUP_SIMPLE_ALERT = "SimpleAlert";


        public const string POPUP_TUTORIAL_MISSION_1 = "TutorialMission1";
        public const string POPUP_TUTORIAL_MISSION_2 = "TutorialMission2";
        public const string POPUP_TUTORIAL_MISSION_3 = "TutorialMission3";


        public const string POPUP_LACK_OF_CURRENCY = "LackOfCurrency";
        public const string POPUP_SPECIAL_EPISODE_BUY = "SpecialEpisodeBuy";
        public const string POPUP_EXPIRE_TOKEN = "ExpireToken";
        public const string POPUP_DEV_JUMP = "DevJump";
        public const string POPUP_ALL_PASS_GUIDE = "AllPassGuide";
        public const string POPUP_INTRO = "Intro";
        public const string POPUP_RECOMMEND = "Recommend";
        public const string POPUP_IFYOU_PASS = "ifyou_pass";
        public const string POPUP_ONEDAY_PASS = "oneday_pass";
        public const string POPUP_PREMIUM_PASS = "premium_pass_new";
        public const string POPUP_PREMIUM_CHALLENGE = "premium_challenge";
        
        // JSON COLUMN
        public const string JSON_EPISODE_SCENE_HISTORY = "sceneProgress";
        public const string JSON_EPISODE_PURCHASE_HISTORY = "episodePurchase";
        
        
        public const string NODE_LEVEL = "current_level";
        public const string NODE_EXP = "current_experience";

        public static Color COLOR_IMAGE_TRANSPARENT = new Color(1, 1, 1, 0);
        public static Color COLOR_BLACK_TRANSPARENT = new Color(0, 0, 0, 0);
        public static Color COLOR_GRAY_TRANSPARENT = new Color(0.26275f, 0.26275f, 0.26275f, 0);


        // Scene name
        public const string SCENE_MAIN_LOBBY = "MainLobby";
        public const string SCENE_STORY_LOBBY = "StoryLobby";
        public const string SCENE_GAME = "Game";
        public const string SCENE_INTERMISSION = "Intermission";
        
        
        public const string KEY_LAST_ONEDAY_PASS_PROJECT = "LastOnedayPassProjectID";
        public const string KEY_LAST_PREMIUM_PASS_PROJECT = "LastPremiumPassProjectID";
        
        
        // Firebase Event
        public const string FIREBASE_NOTICE = "notice_popup_open";
        
    }
}