using System;

using LitJson;

namespace PIERStory
{
    /// <summary>
    /// IFYOU 업적 Data
    /// </summary>
    [Serializable]
    public class AchievementData
    {
        JsonData achievementData;

        public int achievementId = 0;
        public string achievementType = string.Empty;
        public int experience = 0;                  // 달성시 지급해주는 등급 경험치
        public int achievementPoint = 0;            // 현 레벨에서 달성해야 하는 point
        public string achievementName = string.Empty;

        public int currentLevel = 1;
        public int currentPoint = 0;                // 현재 보유중인 point
        public bool isClaer = false;

        public float achievementDegree = 0f;

        public int achievementIconId = 0;
        public string achievementIconUrl = string.Empty;
        public string achievementIconKey = string.Empty;

        public string achievementSummary = string.Empty;


        public AchievementData(JsonData __j)
        {
            achievementData = __j;

            if (achievementData == null)
                return;

            achievementId = SystemManager.GetJsonNodeInt(__j, "achievement_id");
            achievementType = SystemManager.GetJsonNodeString(__j, "achievement_type");
            experience = SystemManager.GetJsonNodeInt(__j, "experience");
            achievementPoint = SystemManager.GetJsonNodeInt(__j, "achievement_point");
            achievementName = SystemManager.GetJsonNodeString(__j, "name");

            currentLevel = SystemManager.GetJsonNodeInt(__j, "current_level");
            currentPoint = SystemManager.GetJsonNodeInt(__j, "current_point");
            isClaer = SystemManager.GetJsonNodeBool(__j, "is_clear");

            achievementIconId = SystemManager.GetJsonNodeInt(__j, "achievement_icon_id");
            achievementIconUrl = SystemManager.GetJsonNodeString(__j, "achievement_icon_url");
            achievementIconKey = SystemManager.GetJsonNodeString(__j, "achievement_icon_key");

            achievementSummary = SystemManager.GetJsonNodeString(__j, "summary");

            achievementDegree = (float)currentPoint / achievementPoint;
        }
    }
}