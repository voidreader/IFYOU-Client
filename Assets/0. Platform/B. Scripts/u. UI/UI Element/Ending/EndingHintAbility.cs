using System;
using UnityEngine;

using TMPro;


namespace PIERStory
{
    public class EndingHintAbility : MonoBehaviour
    {
        public GameObject radioButton;

        public ImageRequireDownload emoticonImage;
        public ImageRequireDownload abilityIcon;

        public TextMeshProUGUI abilityPercent;


        public void InitHintAbility(string speaker, string abilityName, string __oper, int value)
        {
            AbilityData data = UserManager.main.GetAbilityData(speaker, abilityName);
            emoticonImage.SetDownloadURL(data.emoticonDesignUrl, data.emoticonDesignKey);
            abilityIcon.SetDownloadURL(data.iconDesignUrl, data.iconDesignKey);

            string __operator = string.Empty;

            switch (__oper)
            {
                case ">=":
                    __operator = "≥";
                    radioButton.SetActive(value >= data.currentValue);
                    break;
                case "<=":
                    __operator = "≤";
                    radioButton.SetActive(value <= data.currentValue);
                    break;
                case ">":
                    __operator = __oper;
                    radioButton.SetActive(value > data.currentValue);
                    break;
                case "<":
                    __operator = __oper;
                    radioButton.SetActive(value < data.currentValue);
                    break;
                case "=":
                    __operator = __oper;
                    radioButton.SetActive(value == data.currentValue);
                    break;
            }

            float percent = (float)value / (float)data.maxValue;
            abilityPercent.text = string.Format("{0}    {1}%", __operator, Math.Truncate(percent * 100f));
        }
    }
}