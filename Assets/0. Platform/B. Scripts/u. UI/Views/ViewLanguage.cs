using System;
using UnityEngine;

using TMPro;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewLanguage : CommonView
    {
        public static Action<string, string, TMP_FontAsset, bool> OnChangeLanguage = null;
        public TextMeshProUGUI appLanguageAlert;
        public LanguageElement[] langElements;

        public GameObject changeButton;
        public TextMeshProUGUI buttonLabel;

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_VIEW_NAME, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);

            OnChangeLanguage = ChangeLanguageAlert;

            foreach (LanguageElement le in langElements)
                le.InitElement();
        }

        void ChangeLanguageAlert(string changeText, string buttonText, TMP_FontAsset fontAsset, bool active)
        {
            appLanguageAlert.text = changeText;
            appLanguageAlert.font = fontAsset;
            buttonLabel.text = buttonText;
            buttonLabel.font = fontAsset;
            // 현재 앱 설정 언어와 다르면 버튼을 활성화해준다
            changeButton.SetActive(!active);
        }

        public void OnClickChangeAppLanguage()
        {
            // 현재 toggle이 isOn == true인 element를 찾아서
            foreach(LanguageElement le in langElements)
            {
                if (le.nowLang)
                {
                    // 유저의 사용 언어 코드를 변경해주고
                    ES3.Save<string>(SystemConst.KEY_LANG, le.elementLang);
                    SystemManager.main.currentAppLanguageCode = le.elementLang;
                    break;
                }
            }

            // 타이틀로 보내버리기
            // 이후 타이틀에서 언어 정보에 따른 폰트 다운 뭐 그런거도 해줄 거임
            Signal.Send(LobbyConst.STREAM_IFYOU, "moveTitle", "Return to title");
        }
    }
}