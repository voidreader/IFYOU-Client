using System;
using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class EndingHintAbility : MonoBehaviour
    {
        public GameObject radioButton;

        public ImageRequireDownload emoticonImage;
        public ImageRequireDownload abilityIcon;

        public TextMeshProUGUI abilityPercent;


        public void InitHintAbility(JsonData __j)
        {
            AbilityData data = UserManager.main.GetAbilityData(SystemManager.GetJsonNodeString(__j, GameConst.COL_SPEAKER), SystemManager.GetJsonNodeString(__j, "ability_name"));
            emoticonImage.SetDownloadURL(data.emoticonDesignUrl, data.emoticonDesignKey);
            abilityIcon.SetDownloadURL(data.iconDesignUrl, data.iconDesignKey);

            string __operator = string.Empty;
            int neccesaryValue = SystemManager.GetJsonNodeInt(__j, "value");

            switch (SystemManager.GetJsonNodeString(__j, "operator"))
            {
                case ">=":
                    __operator = "≥";
                    radioButton.SetActive(neccesaryValue >= data.currentValue);
                    break;
                case "<=":
                    __operator = "≤";
                    radioButton.SetActive(neccesaryValue <= data.currentValue);
                    break;
                case ">":
                    __operator = SystemManager.GetJsonNodeString(__j, "operator");
                    radioButton.SetActive(neccesaryValue > data.currentValue);
                    break;
                case "<":
                    __operator = SystemManager.GetJsonNodeString(__j, "operator");
                    radioButton.SetActive(neccesaryValue < data.currentValue);
                    break;
                case "=":
                    __operator = SystemManager.GetJsonNodeString(__j, "operator");
                    radioButton.SetActive(neccesaryValue == data.currentValue);
                    break;
            }

            float percent = (float)neccesaryValue / (float)data.maxValue;

            abilityPercent.text = string.Format("{0}    {1}%", __operator, Math.Truncate(percent * 100f));
        }
    }
}