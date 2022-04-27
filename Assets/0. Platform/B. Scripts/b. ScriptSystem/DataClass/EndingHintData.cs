using System.Collections.Generic;
using System;

using LitJson;

namespace PIERStory
{
    [Serializable]
    public class EndingHintData
    {
        JsonData endingHintJson;

        public string endingId = string.Empty;

        public string endingTitle = string.Empty;
        public string endingType = string.Empty;
        public bool isHidden = false;
        public EpisodeData dependEpisodeData = null;

        string scenes = string.Empty;
        public string[] unlockScenes;

        public struct AbilityCondition
        {
            public string speaker;
            public string abilityName;
            public string oper;         // 연산자
            public int value;
        }

        public List<AbilityCondition> abilityConditions = new List<AbilityCondition>();

        public CurrencyType currency;
        public int price = -1;


        public EndingHintData(JsonData __j)
        {
            endingHintJson = __j;

            if (endingHintJson == null)
                return;

            endingId = SystemManager.GetJsonNodeString(__j, "ending_id");
            FindEndingData();

            scenes = SystemManager.GetJsonNodeString(__j, "unlock_scenes");
            unlockScenes = scenes.Split(',');

            FillAbilityConditionList();

            if (SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY) == LobbyConst.COIN)
                currency = CurrencyType.Coin;
            else if(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY) == LobbyConst.GEM)
                currency = CurrencyType.Gem;

            price = SystemManager.GetJsonNodeInt(__j, "price");
        }

        /// <summary>
        /// 해당 엔딩 데이터 찾아서 세팅해주기
        /// </summary>
        void FindEndingData()
        {
            foreach(EpisodeData epi in StoryManager.main.ListCurrentProjectEpisodes)
            {
                // 에피소드 타입이 엔딩이 아니면 건너뛰기. 동일한 에피소드 Id가 아니어도 건너뜀
                if (epi.episodeType != EpisodeType.Ending || endingId != epi.episodeID)
                    continue;

                endingTitle = epi.episodeTitle;
                isHidden = epi.endingType == LobbyConst.COL_HIDDEN;

                if (epi.endingType == LobbyConst.COL_HIDDEN)
                    endingType = SystemManager.GetLocalizedText("5087");
                else
                    endingType = SystemManager.GetLocalizedText("5088");

                dependEpisodeData = StoryManager.GetRegularEpisodeByID(epi.dependEpisode);
                break;
            }
        }

        /// <summary>
        /// 능력치 해금 조건 리스트 채우기
        /// </summary>
        void FillAbilityConditionList()
        {
            JsonData abilityHint = SystemManager.GetJsonNode(endingHintJson, "ability_condition");

            if (abilityHint == null || abilityHint.Count == 0)
                return;

            for (int i = 0; i < abilityHint.Count; i++)
            {
                AbilityCondition condition = new AbilityCondition();
                condition.speaker = SystemManager.GetJsonNodeString(abilityHint[i], GameConst.COL_SPEAKER);
                condition.abilityName = SystemManager.GetJsonNodeString(abilityHint[i], "ability_name");
                condition.oper = SystemManager.GetJsonNodeString(abilityHint[i], "operator");
                condition.value = SystemManager.GetJsonNodeInt(abilityHint[i], "value");

                abilityConditions.Add(condition);
            }
        }

    }
}