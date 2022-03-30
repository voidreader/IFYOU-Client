using System;

using LitJson;

namespace PIERStory
{
    [Serializable]
    public class AbilityData
    {
        JsonData abilityJson;

        public int abilityId = 0;
        public string speaker = string.Empty;               // 화자
        public string abilityName = string.Empty;           // 능력치 명칭(localized)
        public string originAbilityName = string.Empty; // 능력치 원 이름(한글)
        
        public bool isMain = false;
        public int maxValue = 1000;
        
        public string iconDesignUrl = string.Empty;         // 능력치 아이콘 url
        public string iconDesignKey = string.Empty;
        
        public string emoticonDesignUrl = string.Empty;     // 캐릭터 이모티콘 url
        public string emoticonDesignKey = string.Empty;

        public string backgroundUrl = string.Empty;         // 능력치 View에서 사용되는 이미지 Url
        public string backgroundKey = string.Empty;

        public int currentValue = 0;     // 현재 능력치양

        public float abilityPercent = 0f;   // currentValue / maxValue

        public AbilityData(JsonData __j)
        {
            abilityJson = __j;

            if (abilityJson == null)
                return;

            abilityId = SystemManager.GetJsonNodeInt(__j, "ability_id");
            speaker = SystemManager.GetJsonNodeString(__j, GameConst.COL_SPEAKER);
            
            
            abilityName = SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, "local_id"));
            originAbilityName = SystemManager.GetJsonNodeString(__j, "ability_name"); 
            
            
            isMain = SystemManager.GetJsonNodeBool(__j, "is_main");
            maxValue = SystemManager.GetJsonNodeInt(__j, "max_value");
            
            iconDesignUrl = SystemManager.GetJsonNodeString(__j, "icon_design_url");
            iconDesignKey = SystemManager.GetJsonNodeString(__j, "icon_design_key");

            emoticonDesignUrl = SystemManager.GetJsonNodeString(__j, "emoticon_design_url");
            emoticonDesignKey = SystemManager.GetJsonNodeString(__j, "emoticon_design_key");

            backgroundUrl = SystemManager.GetJsonNodeString(__j, "background_url");
            backgroundKey = SystemManager.GetJsonNodeString(__j, "background_key");

            currentValue = SystemManager.GetJsonNodeInt(__j, "current_value");

            // 현재 값이 최대값 이상인 경우 maxValue를 넣어준다
            if (currentValue >= maxValue)
                currentValue = maxValue;

            abilityPercent = (float)((float)currentValue / (float)maxValue);
        }
    }
}