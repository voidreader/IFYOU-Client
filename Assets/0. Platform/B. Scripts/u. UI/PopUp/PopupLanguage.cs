using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using Toast.Gamebase;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class PopupLanguage : PopupBase
    {
        public static Action<string, string, TMP_FontAsset, bool> OnChangeLanguage = null;

        [Space(15)]
        public TextMeshProUGUI appLanguageAlert;
        public LanguageElement[] langElements;

        public GameObject changeButton;
        public TextMeshProUGUI buttonLabel;
        bool isChangingScene = false;
        
        public string originLang = string.Empty;

        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();

            changeButton.SetActive(false);

            OnChangeLanguage = ChangeLanguageAlert;

            foreach (LanguageElement le in langElements)
                le.InitElement();

            isChangingScene = false;
            
            originLang = SystemManager.main.currentAppLanguageCode;
            
        }

        public override void Hide()
        {
            base.Hide();
            
            if(isChangingScene)
                return;
                
            // 복구. 
            SystemManager.main.currentAppLanguageCode = originLang;
        }

        void ChangeLanguageAlert(string changeText, string buttonText, TMP_FontAsset fontAsset, bool active)
        {
            appLanguageAlert.font = fontAsset;
            SystemManager.SetText(appLanguageAlert, changeText);
            
            buttonLabel.font = fontAsset;
            SystemManager.SetText(buttonLabel, buttonText);
            // 현재 앱 설정 언어와 다르면 버튼을 활성화해준다
            changeButton.SetActive(!active);
        }


        public void OnClickChangeAppLanguage()
        {
            SystemManager.ShowSystemPopupLocalize("6126", ChangeAppLanguage, null);
        }

        void ChangeAppLanguage()
        {
            // 두번 실행되는 것 같아..
            if (isChangingScene)
            {
                Debug.LogError("Already changing.. ");
                return;

            }

            SystemManager.ShowNetworkLoading();

            // 현재 toggle이 isOn == true인 element를 찾아서
            foreach (LanguageElement le in langElements)
            {
                if (le.GetComponent<UIToggle>().isOn)
                {
                    // 유저의 사용 언어 코드를 변경해주고
                    ES3.Save<string>(SystemConst.KEY_LANG, le.elementLang);
                    SystemManager.main.currentAppLanguageCode = le.elementLang;
                    Debug.Log(">> OnClickChangeAppLanguage : " + le.elementLang);
                    break;
                }
            }

            isChangingScene = true;

            UserManager.main.RequestServiceStoryList(); // 언어변경하고, 서버에서 받는 정보 refresh
            NetworkLoader.main.RequestGameProductList(); // 상품정보 갱신 언어정보가 달라졌으니까.

            StartCoroutine(OnCompleteRefreshServerInfo()); // 코루틴 콜 
        }


        IEnumerator OnCompleteRefreshServerInfo()
        {

            Debug.Log(">> OnCompleteRefreshServerInfo #1");
            yield return null;

            // 통신 완료되길 기다린다. 
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());

            Debug.Log(">> OnCompleteRefreshServerInfo #2");


            // 플랫폼 로딩 이미지 삭제 해놓는다. (타이틀에서 이전 언어의 플랫폼 로딩 이미지를 부르는 것 방지)
            ES3.DeleteKey(SystemConst.KEY_PLATFORM_LOADING);


            // 타이틀로 보내버리기
            SystemManager.main.givenStoryData = null; // 목록으로 가는것을 막기 위해 작성

            Signal.Send(LobbyConst.STREAM_COMMON, "LobbyBegin"); // 시그널 보내서 Nody를 이동시킨다. 
            IntermissionManager.isMovingLobby = true;


            switch (SystemManager.main.currentAppLanguageCode)
            {
                case "EN":
                    SystemManager.main.currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.English;
                    break;
                case "KO":
                    SystemManager.main.currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.Korean;
                    break;
                case "JA":
                    SystemManager.main.currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.Japanese;
                    break;
                default:
                    SystemManager.main.currentGamebaseLanguageCode = GamebaseDisplayLanguageCode.English;
                    break;
            }

            // 게임베이스 디스플레이 언어 변경 
            Gamebase.SetDisplayLanguageCode(SystemManager.main.currentGamebaseLanguageCode);

            Debug.Log(">> Changed gamebase language : " + SystemManager.main.currentGamebaseLanguageCode);
            Debug.Log(">> current GamebaseCODE : " + Gamebase.GetDisplayLanguageCode());
            
            SystemManager.main.SetMainFont(); // 메인폰트 재설정

            SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
        }
    }
}