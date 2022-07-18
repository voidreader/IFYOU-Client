using System;
using UnityEngine;

using TMPro;
using BestHTTP;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class ViewCommonTop : CommonView
    {
        public static bool isBackgroundShow = true;                 // 배경 보여지고 있는지 
        public static string staticCurrentTopOwner = string.Empty;  // static owner.
     
        
        #region Actions 
        
        public static Action OnRefreshSuperUser = null; // 슈퍼유저 표기용도 
        public static Action OnBackAction = null;       // 백 버튼 터치 추가 액션 필요시 사용

        public static Action<int> OnForShowCoin = null; // 보여주기용 코인
        
        public static Action OnRefreshAccountLink = null; // 계정연동 리프레시
        
        #endregion

        [SerializeField] GameObject backButton; // 뒤로가기 버튼 
        [SerializeField] TextMeshProUGUI textViewName; // 뷰 이름  
        [SerializeField] GameObject groupProperty; // 프로퍼티 그룹 (재화, 메일, 등등)
        public GameObject mailButton;           // 프로퍼티 그룹의 메일 버튼
        public GameObject settingButton; // 세팅 버튼 
        [SerializeField] GameObject mailNotify; // 메일 알림 표시 
        [SerializeField] GameObject moreNotify; // 설정 알림 표시 

        public GameObject attendanceButton;     // 출석 이벤트 버튼

        public CoinIndicator topCoin;           // 상단바에 존재하는 코인

        
        [SerializeField] GameObject logo; // 로고
        [SerializeField] GameObject bottomLine; // 아래 라인 
        
        
        
        // 바로 이전 상태 저장 변수
        bool previousBackButtonShow = false; // 백버튼의 이전 상태
        bool previousTextViewNameShow = false; // 이전 뷰 이름 상태
        string previousViewName = string.Empty; // 이전 뷰 텍스트
        bool previousGroupPropertyShow = false; // 이전 그룹 프로퍼티 상태 
        bool previousMailShow = true; // 이전 메일함 버튼 
        
        bool previousLogoShow = false; // 로고 보여주기 
        
        bool previousAttendanceButtonShow = false; 
        
        
        public GameObject objParent; // 최상위 개체 
        [SerializeField] GameObject superUserSign; // 슈퍼유저 표시 
        
        
        // Stream, Signal
        SignalStream signalStreamTopViewName;
        SignalStream signalStreamTopViewNameExist;
        SignalStream signalStreamTopPropertyGroup;
        SignalStream signalStreamTopMail;
        
        SignalStream signalStreamTopBackButton;
        SignalStream signalStreamRecover;
        SignalStream signalStreamSaveState;
        SignalStream signalStreamParent;
        
        
        
        SignalReceiver signalReceiverTopViewName;
        SignalReceiver signalReceiverTopViewNameExist;
        SignalReceiver signalReceiverTopPropertyGroup;
        SignalReceiver signalReceiverTopMail;
        
        SignalReceiver signalReceiverTopBackButton;
        SignalReceiver signalReceiverRecover;
        SignalReceiver signalReceiverSaveState;

        SignalReceiver signalReceiverParent;
        
        private void Awake() {
            OnRefreshAccountLink = RefreshAccountLink;            

            signalStreamTopViewNameExist = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST);
            signalReceiverTopViewNameExist = new SignalReceiver().SetOnSignalCallback(OnTopViewNameExistSignal);

            signalStreamTopViewName = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME);
            signalReceiverTopViewName = new SignalReceiver().SetOnSignalCallback(OnTopViewNameSignal);

            signalStreamTopPropertyGroup = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP);
            signalReceiverTopPropertyGroup = new SignalReceiver().SetOnSignalCallback(OnTopPropertySignal);

            signalStreamTopMail = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON);
            signalReceiverTopMail = new SignalReceiver().SetOnSignalCallback(OnTopMailSignal);
            
            signalStreamTopBackButton = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON);
            signalReceiverTopBackButton = new SignalReceiver().SetOnSignalCallback(OnTopBackButtonSignal);


            // 복원 시그널 추가
            signalStreamRecover = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER);
            signalReceiverRecover = new SignalReceiver().SetOnSignalCallback(OnTopRecoverSignal);
            // 상태 저장 시그널 추가
            signalStreamSaveState = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE);
            signalReceiverSaveState = new SignalReceiver().SetOnSignalCallback(OnTopSaveState);

            
            signalStreamParent = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_PARENT);
            signalReceiverParent = new SignalReceiver().SetOnSignalCallback(OnShowParent);
            
            
            mailNotify.gameObject.SetActive(false);
            moreNotify.gameObject.SetActive(false);
        }
        
        private void Start() {
            
            // 메일 알림 표시 추가
            UserManager.OnRefreshUnreadMailCount = RefreshMailNotification;
            
            signalStreamTopViewNameExist.ConnectReceiver(signalReceiverTopViewNameExist);
            signalStreamTopViewName.ConnectReceiver(signalReceiverTopViewName);
            signalStreamTopPropertyGroup.ConnectReceiver(signalReceiverTopPropertyGroup);
            signalStreamTopMail.ConnectReceiver(signalReceiverTopMail);
            
            signalStreamTopBackButton.ConnectReceiver(signalReceiverTopBackButton);
            signalStreamRecover.ConnectReceiver(signalReceiverRecover);
            signalStreamSaveState.ConnectReceiver(signalReceiverSaveState);
            
            OnRefreshSuperUser = SetSuperUser;
            OnForShowCoin = RefreshCoin;

            signalStreamParent.ConnectReceiver(signalReceiverParent);
            
            if(UserManager.main != null && UserManager.main.unreadMailCount > 0) {
                mailNotify.SetActive(true);
            }
            
        }
        
        void OnDisable() {
            signalStreamTopViewNameExist.DisconnectReceiver(signalReceiverTopViewNameExist);
            signalStreamTopViewName.DisconnectReceiver(signalReceiverTopViewName);
            signalStreamTopPropertyGroup.DisconnectReceiver(signalReceiverTopPropertyGroup);
            signalStreamTopMail.DisconnectReceiver(signalReceiverTopMail);
            
            signalStreamTopBackButton.DisconnectReceiver(signalReceiverTopBackButton);
            signalStreamRecover.DisconnectReceiver(signalReceiverRecover);
            signalStreamSaveState.DisconnectReceiver(signalReceiverSaveState);

            signalStreamParent.DisconnectReceiver(signalReceiverParent);
        }
        
        public override void OnView()
        {
            base.OnView();

            
            SetSuperUser();
        }
        
        
        /// <summary>
        /// 슈퍼유저 표기 
        /// </summary>
        void SetSuperUser() {
            
            if(UserManager.main == null || string.IsNullOrEmpty(UserManager.main.userKey)) {
                superUserSign.SetActive(false);
                return;
            }
            
            Debug.Log("### SetSuperUser ###");
            superUserSign.SetActive(UserManager.main.CheckAdminUser());
        }

        
        /// <summary>
        /// 코인 리프레시 
        /// </summary>
        /// <param name="__newValue"></param>
        void RefreshCoin(int __newValue)
        {
            topCoin.RefreshCoin(__newValue);
        }
        
        /// <summary>
        /// 계정연동 refresh 
        /// </summary>        
        void RefreshAccountLink() {
            if(UserManager.main == null) {
                moreNotify.SetActive(false);
                return;
            }
            
            moreNotify.SetActive(!UserManager.main.CheckAccountLink());
        }



        
        public void OnSignalControlBackButton(bool __flag) {
            backButton.SetActive(__flag);
        }

        public void OnClickMail()
        {
            // 22.01.20 Doozy Nody global로 변경하면서 Popup으로 변경
            NetworkLoader.main.RequestUnreadMailList(CallbackOpenMail);
        }

        void CallbackOpenMail(HTTPRequest req, HTTPResponse res)
        {
            if(!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackOpenMail");
                return;
            }

            PopupBase p = PopupManager.main.GetPopup("Mail");

            if(p == null)
            {
                Debug.LogError("No Popup");
                return;
            }

            PopupManager.main.ShowPopup(p, false);
        }
        
        /// <summary>
        /// 상태 정보 저장. 
        /// </summary>
        void SavePreviousState() {
            
            Debug.Log("SavePreviousState");
            
            previousBackButtonShow = backButton.activeSelf;
            previousGroupPropertyShow = groupProperty.activeSelf;
            previousTextViewNameShow = textViewName.gameObject.activeSelf;
            previousViewName = textViewName.text;
            previousMailShow = mailButton.activeSelf;
            previousLogoShow = !previousBackButtonShow;
            
            
            // 플로팅 버튼 친구들
            previousAttendanceButtonShow = attendanceButton.activeSelf;
        }
        
        /// <summary>
        /// 상태정보 복원
        /// 팝업이나 일부 뷰가 닫힐때 다시 실행한다. 
        /// </summary>
        void RecoverState() {
            
            groupProperty.SetActive(previousGroupPropertyShow);
            textViewName.gameObject.SetActive(previousTextViewNameShow);

            textViewName.text = previousViewName;
            
            
            mailButton.SetActive(previousMailShow);
            
            // 백버튼과 로고&세팅버튼은 서로 같이 활성화 할 수 없음. 
            backButton.SetActive(previousBackButtonShow);
            logo.SetActive(!previousBackButtonShow);
            settingButton.SetActive(!previousBackButtonShow);
            
            
            // 출석버튼, 하우투플레이 버튼             
            attendanceButton.SetActive(previousAttendanceButtonShow);
        }

        void OnTopSaveState(Signal signal) {
            SavePreviousState();
        }
        
        void OnTopRecoverSignal(Signal signal) {
            
            // Debug.Log("Top Recover Signal Received");
            RecoverState();
        }
        
        /// <summary>
        /// 백버튼 
        /// </summary>
        /// <param name="signal"></param>
        void OnTopBackButtonSignal(Signal signal) {
            if(!signal.hasValue) {
                Debug.LogError("OnTopBackButtonSignal has no value");
                return;
            }
            
            bool isShow = signal.GetValueUnsafe<bool>();
            backButton.SetActive(isShow);
            
            // * 로고랑 세팅버튼은 묶음이다. 
            logo.SetActive(!isShow);
            settingButton.SetActive(!isShow);
            
            
        }
        

        

        
        
        /// <summary>
        /// 뷰 네임 표기 시그널 처리 
        /// </summary>
        /// <param name="signal"></param>
        void OnTopViewNameExistSignal(Signal signal) {
            if(!signal.hasValue) {
                Debug.LogError("OnTopViewNameSignal has no value");
                return;
            }
            
            bool isShow = signal.GetValueUnsafe<bool>();
            textViewName.gameObject.SetActive(isShow);
        }

        /// <summary>
        /// 뷰 네임명
        /// </summary>
        /// <param name="s"></param>
        void OnTopViewNameSignal(Signal s)
        {
            if(s.hasValue)
            {
                string viewName = s.GetValueUnsafe<string>();
                
                SystemManager.SetText(textViewName, viewName);
            }
        }

        
        void OnTopPropertySignal(Signal signal) {
            if(!signal.hasValue) {
                Debug.LogError("OnTopPropertySignal has no value");
                return;
            }
            
            bool isShow = signal.GetValueUnsafe<bool>();
            groupProperty.SetActive(isShow);
            
        }

        void OnTopMailSignal(Signal s)
        {
            if(s.hasValue)
            {
                bool isShow = s.GetValueUnsafe<bool>();
                mailButton.SetActive(isShow);
            }
        }
        
        void OnShowParent(Signal s) {
            if(!s.hasValue) {
                return;
            }
            
            bool isShow = s.GetValueUnsafe<bool>();
            objParent.SetActive(isShow);
            
        }


        /// <summary>
        /// 메일 알림 표시 
        /// </summary>
        /// <param name="__cnt"></param>
        void RefreshMailNotification(int __cnt) {
            mailNotify.SetActive(__cnt > 0);
        }
        
        
        /// <summary>
        /// 코인 클릭 이벤트 
        /// </summary>
        public void OnClickCoin() {
            
            Debug.Log("### OnClickCoin ###");
            
            if(ViewCommonStarShop.isCommonShopOpen)
                return;
                
            if(MainShop.isMainNavigationShop)
                return;
            
            Signal.Send(LobbyConst.STREAM_COMMON, "Shop", string.Empty);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("main_coinshop");
            
        }
        
        public void OnClickShop() {
            
            if(ViewCommonStarShop.isCommonShopOpen)
                return;
                
            if(MainShop.isMainNavigationShop)
                return;            
            
            Signal.Send(LobbyConst.STREAM_COMMON, "Shop", string.Empty);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("main_starshop");
        }


        /// <summary>
        /// 백버튼 터치 추가 액션 
        /// </summary>
        public void OnClickBack() {
            
            Debug.Log("### TOP OnClickBack");
            
            OnBackAction?.Invoke();
        }


    }
}