﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewLanguage : CommonView
    {
        public static Action<string, string, TMP_FontAsset, bool> OnChangeLanguage = null;
        public TextMeshProUGUI appLanguageAlert;
        public LanguageElement[] langElements;

        public GameObject changeButton;
        public TextMeshProUGUI buttonLabel;
        AsyncOperation asyncOperation;
        public bool isChangingScene = false;

        void OnEnable() {
            // 상태 저장 
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
        }

        public override void OnStartView()
        {
            base.OnStartView();
            
            changeButton.SetActive(false);

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5127"), string.Empty);

            OnChangeLanguage = ChangeLanguageAlert;

            foreach (LanguageElement le in langElements)
                le.InitElement();
                
            isChangingScene = false;
        }
        
        public override void OnHideView() {
            base.OnHideView();
            
            if(ES3.KeyExists(SystemConst.KEY_LANG))
                SystemManager.main.currentAppLanguageCode = ES3.Load<string>(SystemConst.KEY_LANG);

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);
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
            SystemManager.ShowSystemPopupLocalize("6126", ChangeAppLanguage, null);
        }

        void ChangeAppLanguage()
        {
            // 두번 실행되는 것 같아..
            if(isChangingScene)
                return;
            
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
            
            UserManager.main.RequestServiceStoryList(); // 언어변경하고, 서버에서 받는 정보 refresh
            NetworkLoader.main.RequestGameProductList(); // 상품정보 갱신 언어정보가 달라졌으니까.
            
            StartCoroutine(OnCompleteRefreshServerInfo()); // 코루틴 콜 
            
            isChangingScene = true;
        }
        
        
        IEnumerator OnCompleteRefreshServerInfo() {
            
            Debug.Log(">> OnCompleteRefreshServerInfo #1");
            
            // 통신 완료되길 기다린다. 
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            
            Debug.Log(">> OnCompleteRefreshServerInfo #2");
            // 타이틀로 보내버리기
            SystemManager.main.givenStoryData = null; // 목록으로 가는것을 막기 위해 작성
            ViewNoticeDetail.isDependent = true;           
            
            
            Signal.Send(LobbyConst.STREAM_COMMON, "LobbyBegin");
            IntermissionManager.isMovingLobby = true;
            SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
            // SceneManager.LoadSceneAsync(CommonConst.SCENE_LOBBY, LoadSceneMode.Single).allowSceneActivation = true;

        }
        
        
    }
}