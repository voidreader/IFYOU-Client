using UnityEngine;
using UnityEngine.UI;


namespace PIERStory
{
    public class CharacterAbilityBriefElement : MonoBehaviour
    {
        public ImageRequireDownload characterEmoticon;          // 캐릭터 이모티콘
        public ImageRequireDownload characterMainAbilityIcon;   // 캐릭터 메인 능력치 아이콘
        public Image mainAbilityGauge;

        AbilityData abilityData;

        public void InitAbilityBrief(AbilityData __abilityData)
        {
            abilityData = __abilityData;

            characterEmoticon.SetDownloadURL(abilityData.emoticonDesignUrl, abilityData.emoticonDesignKey);
            characterMainAbilityIcon.SetDownloadURL(abilityData.iconDesignUrl, abilityData.iconDesignKey);

            mainAbilityGauge.fillAmount = abilityData.abilityPercent;
        }
    }
}