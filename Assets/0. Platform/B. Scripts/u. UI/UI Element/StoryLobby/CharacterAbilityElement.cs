using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory
{
    public class CharacterAbilityElement : MonoBehaviour
    {
        public ImageRequireDownload imageThumbnail;
        public TextMeshProUGUI speakerName;

        [Header("메인 능력치 관련")]
        public ImageRequireDownload mainAbilityIcon;
        public TextMeshProUGUI mainAbilityName;
        public GameObject mainLock; // lock
        public GameObject mainOpen; // open
        public Image mainAbilityGauge;
        public TextMeshProUGUI mainAbilityPercent;

        [Header("첫번째 서브 능력치")]
        public GameObject subAbility_1;
        public ImageRequireDownload subAbilityIcon_1;
        public TextMeshProUGUI subAbilityName_1;
        public GameObject subAbilityLock_1; // lock
        public GameObject subAbilityOpen_1; // open
        public Image subAbilityGauge_1;
        public TextMeshProUGUI subAbilityPercent_1;

        [Header("두번째 서브 능력치")]
        public GameObject subAbility_2;
        public ImageRequireDownload subAbilityIcon_2;
        public TextMeshProUGUI subAbilityName_2;
        public GameObject subAbilityLock_2; // lock
        public GameObject subAbilityOpen_2; // open
        public Image subAbilityGauge_2;
        public TextMeshProUGUI subAbilityPercent_2;

        ScrollRect scroll;
        RectTransform scrollRect;

        public void InitMainAbility(AbilityData __abilityData)
        {
            subAbility_1.SetActive(false);
            subAbility_2.SetActive(false);
            mainOpen.SetActive(false);

            imageThumbnail.SetDownloadURL(__abilityData.backgroundUrl, __abilityData.backgroundKey, true);
            speakerName.text =  StoryManager.main.GetNametagName(__abilityData.speaker);

            mainAbilityIcon.SetDownloadURL(__abilityData.iconDesignUrl, __abilityData.iconDesignKey);
            mainAbilityName.text = __abilityData.abilityName;
            mainAbilityGauge.fillAmount = __abilityData.abilityPercent;

            mainAbilityPercent.text = string.Format("{0}%", Math.Truncate(__abilityData.abilityPercent * 100f));

            if ((__abilityData.abilityPercent * 100f) >= 1)
            {
                mainOpen.SetActive(true);
                mainLock.SetActive(false);
            } 
            else
            {
                mainOpen.SetActive(false);
                mainLock.SetActive(true);
            }

            gameObject.SetActive(true);

            scroll = GetComponentInParent<ScrollRect>();
            scrollRect = scroll.GetComponent<RectTransform>();
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, scrollRect.rect.height);
        }


        
        public void InitFirstSubAbility(AbilityData __subAbilityData)
        {
            subAbility_1.SetActive(true);

            subAbilityIcon_1.SetDownloadURL(__subAbilityData.iconDesignUrl, __subAbilityData.iconDesignKey);
            subAbilityName_1.text = __subAbilityData.abilityName;


            subAbilityGauge_1.fillAmount = __subAbilityData.abilityPercent;


            subAbilityPercent_1.text = string.Format("{0}%", Math.Truncate(__subAbilityData.abilityPercent * 100f));

            if ((__subAbilityData.abilityPercent * 100f) >= 1)
            {
                subAbilityOpen_1.SetActive(true);
                subAbilityLock_1.SetActive(false);
            }
            else
            {
                subAbilityOpen_1.SetActive(false);
                subAbilityLock_1.SetActive(true);
            }
        }

        public void InitSecondSubAbility(AbilityData __subAbilityData)
        {
            subAbility_2.SetActive(true);

            subAbilityIcon_2.SetDownloadURL(__subAbilityData.iconDesignUrl, __subAbilityData.iconDesignKey);
            subAbilityName_2.text = __subAbilityData.abilityName;

            subAbilityGauge_2.fillAmount = __subAbilityData.abilityPercent;

            subAbilityPercent_2.text = string.Format("{0}%", Math.Truncate(__subAbilityData.abilityPercent * 100f));

            if ((__subAbilityData.abilityPercent * 100f) >= 1)
            {
                subAbilityOpen_2.SetActive(true);
                subAbilityLock_2.SetActive(false);
            }
            else
            {
                subAbilityOpen_2.SetActive(false);
                subAbilityLock_2.SetActive(true);
            }
        }
        
    }
}