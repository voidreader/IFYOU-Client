using System;
using System.Collections;
using UnityEngine;


using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory {
    public class ViewMain : CommonView
    {
        public static Action OnMoveStarShop = null;
        public static Action OnRefreshProfileNewSign = null;
        
        [Header("로비")]
        public IFYouLobby lobby;
        
        [SerializeField] UIToggle mainToggle;
        public UIContainer lobbyContainer;

        public UIToggle shopToggle;
        
        [Header("내서재")]
        public MainLibrary library;


        [Header("프로필(등급)")]
        public MainProfile ifyouProfile;
        public GameObject achievementNewSign;

        /*
                [Header("더보기")]
                public TextMeshProUGUI userPincode;
                public TextMeshProUGUI mLevelText;      // 더보기 페이지 레벨
                public TextMeshProUGUI mExpText;        // 더보기 페이지 경험치
                public Image mExpGauge;                 // 더보기 페이지 경험치바
        */

        private void Awake()
        {
            OnRefreshProfileNewSign = EnableNewAchievementSign;
        }

        void Update() {
            
            // ViewMain에서 종료 띄우기.
            if(Input.GetKeyDown(KeyCode.Escape)) {
                
                CommonView.DeleteDumpViews();
                
                if(PopupManager.main.GetFrontActivePopup() == null 
                && ((CommonView.ListActiveViews.Count == 1 && CommonView.ListActiveViews.Contains(this)) || CommonView.ListActiveViews.Count < 1)) {
                    SystemManager.ShowSystemPopup(SystemManager.GetLocalizedText("6064"), Application.Quit, null, true);    
                }
            }
        }
        
        public override void OnView()
        {
            if(UserManager.main == null || !UserManager.main.completeReadUserData)
                return;            
            
            base.OnView();

            // 출석보상 안 먹었으면 무조건 또 띄워!
            if (!UserManager.main.TodayAttendanceCheck())
            {
                PopupBase p = PopupManager.main.GetPopup("Attendance");
                PopupManager.main.ShowPopup(p, true);
            }

            // 앱 첫실행 시에만 출석보상 체크하고 띄워!
            if (!StoryManager.enterGameScene && !PlayerPrefs.HasKey("noticeOneday") && SystemManager.main.noticeData.Count > 0)
            {   
                Debug.Log("<color=yellow> Notice Call </color>");
                
                // 실행당 한번만 오픈                 
                if(!SystemManager.noticePopupExcuted) {
                    PopupBase p = PopupManager.main.GetPopup("Notice");
                    PopupManager.main.ShowPopup(p, true);
                    SystemManager.noticePopupExcuted = true; // true 로 설정. 이번 실행헤서는 또 뜨지 않게. 
                }
                
            }

            LobbyManager.main.RequestPlatformLoadingImages(); // 플랫폼 로딩 이미지 다운로드 처리 
            
            UserManager.main.SetNewNickname(UserManager.main.nickname);
            
            
            mainToggle.SetIsOn(true); // '메인' 네이게이션이 언제나 선택된 상태 
            
            
            Firebase.Analytics.FirebaseAnalytics.LogEvent("MainEnter");
            
        }

        public override void OnStartView()
        {

            base.OnStartView();

            InitLobby();

            // 라이브러리 컨테이너 초기화 
            library.InitLibrary();
            
            // 신규 업적이 있을때 표시
            EnableNewAchievementSign();

        }

        /// <summary>
        /// 로비 컨테이너 초기화 
        /// </summary>
        void InitLobby()
        {
            Signal.Send(LobbyConst.STREAM_IFYOU, "initNavigation", string.Empty);
            lobby.InitLobby();

        }
        
        public void OnLobbyTab() {
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, true, string.Empty);
            
            MainShop.isMainNavigationShop = false;
        }

        
        public void OnClickTabNavigation(int index) {
            switch(index) {
                case 0:  // 로비
                    OnLobbyTab();
                break;
                
                case 1:  // 내서재 (라이브러리)
                    OnLibraryTab();
                break;
                
                case 2: // 상점
                    OnShopTab();
                break;

                case 3:
                    break;

                case 4:     // 프로필
                    OnProfile();
                    break;
                case 5:     // 더보기
                    OnAddMore();
                    break;
            }

            if (index != 0)
                StartCoroutine(RemoveTopBackground());
        }
        
        
        #region 메인 로비 




        void MoveToStarShop()
        {
            mainToggle.OnToggleOffCallback.Execute();
            shopToggle.OnToggleOnCallback.Execute();
        }



        IEnumerator RemoveTopBackground()
        {
            if (lobbyContainer.gameObject.activeSelf)
            {
                yield return new WaitUntil(() => !lobbyContainer.gameObject.activeSelf);

                if (!lobbyContainer.gameObject.activeSelf)
                    Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            }

            yield break;
        }

        #endregion
        

        #region 카테고리
        
        /// <summary>
        /// 카테고리 탭 활성화 
        /// </summary>
        public void OnLibraryTab() {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
            
            MainShop.isMainNavigationShop = false;
        }
        

                
        #endregion

        #region 상점
        
        /// <summary>
        /// 상점 탭 활성화
        /// </summary>
        public void OnShopTab() {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
            
            // MainShop.OnRefreshTopShop?.Invoke();
            
            MainShop.isMainNavigationShop = true;
        }
        
        #endregion

        #region 프로필

        void OnProfile()
        {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
        }

        void EnableNewAchievementSign()
        {
            achievementNewSign.SetActive(UserManager.main.CountClearAchievement() > 0);
        }



        #endregion


        #region 더보기

        void OnAddMore()
        {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            // Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, "더보기", string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
        }




        /// <summary>
        /// pin code 복사
        /// </summary>
        public void OnClickCopyUID()
        {
            // UniClipboard.SetText(userPincode.text);
            // SystemManager.ShowSimpleAlertLocalize("6017");
        }

        
        #endregion


    }
}