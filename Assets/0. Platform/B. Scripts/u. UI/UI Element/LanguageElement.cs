using UnityEngine;

using TMPro;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class LanguageElement : MonoBehaviour
    {
        UIToggle toggle;
        public bool nowLang;
        public string elementLang;
        public TextMeshProUGUI languageText;
        public TMP_FontAsset fontAsset;

        Color toggleOnColor = new Color32(243, 140, 161, 255);
        Color toggleOffColor = new Color32(153, 153, 153, 255);


        public void InitElement()
        {
            if(toggle == null)
                toggle = GetComponent<UIToggle>();

            if (elementLang.Equals(SystemManager.main.currentAppLanguageCode))
                toggle.isOn = true;
            else
                toggle.isOn = false;
        }


        public void ToggleOn()
        {
            if (toggle == null)
                return;

            languageText.color = toggleOnColor;
            // 현재 앱 설정된 언어와 같은지 체크
            nowLang = elementLang.Equals(SystemManager.main.currentAppLanguageCode);

            string alertText = string.Empty, buttonText = string.Empty;

            switch (elementLang)
            {
                case "KO":
                    alertText = "앱의 언어 설정을 변경합니다. 언어 변경 시 앱이 재시작됩니다.";
                    buttonText = "변경하기";
                    break;
                case "EN":
                default:
                    alertText = "Change the language settings of the app. The app will restart when you change the language.";
                    buttonText = "Change it";
                    break;
                case "JA":
                    alertText = "アプリの言語設定を変更します。 言語を変更すると、アプリが再起動します。";
                    buttonText = "変更する";
                    break;
                case "ZH":
                    alertText = "更改应用程序的语言设置。 更改语言时，应用程序将重新启动。";
                    buttonText = "更改";
                    break;
                case "TC":
                    alertText = "更改應用程序的語言設置。 更改語言時，應用程序將重新啓動。";
                    buttonText = "更改";
                    break;
            }

            ViewLanguage.OnChangeLanguage?.Invoke(alertText, buttonText, fontAsset, nowLang);
        }

        public void ToggleOff()
        {
            languageText.color = toggleOffColor;
        }
    }
}