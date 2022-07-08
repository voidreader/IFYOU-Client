using UnityEngine;

using TMPro;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class LanguageElement : MonoBehaviour
    {
        UIToggle toggle;
        public bool nowLang;
        public string elementLang; // 타겟언어
        public TextMeshProUGUI languageText;
        public TMP_FontAsset fontAsset;

        Color toggleOnColor = new Color32(243, 140, 161, 255);
        Color toggleOffColor = new Color32(153, 153, 153, 255);
        
        void Start() {
            
            // 일본어일때 
            if(elementLang == "JA") {
                fontAsset = SystemManager.main.jaFont; // 일본어 폰트로 자동변경 
                languageText.font = fontAsset;
            }
        }


        public void InitElement()
        {
            if(toggle == null)
                toggle = GetComponent<UIToggle>();

            if (elementLang.Equals(ES3.Load<string>(SystemConst.KEY_LANG)))
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
            nowLang = elementLang.Equals(ES3.Load<string>(SystemConst.KEY_LANG));

            // 선택되면 잠시 언어코드를 변경한다
            SystemManager.main.currentAppLanguageCode = elementLang.ToUpper();

            string alertText = SystemManager.GetLocalizedText("6124");
            string buttonText = SystemManager.GetLocalizedText("6125");

            PopupLanguage.OnChangeLanguage?.Invoke(alertText, buttonText, fontAsset, nowLang);
        }

        public void ToggleOff()
        {
            languageText.color = toggleOffColor;
        }
    }
}