using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class ViewCommonTop : CommonView
    {
        public static bool isBackgroundShow = true; // 배경 보여지고 있는지 
        public static string staticCurrentTopOwner = string.Empty; // static owner.
        public static Action OnClickButtonAction = null;    // 다용도 버튼 Action 저장용도


        [SerializeField] GameObject backButton; // 뒤로가기 버튼 
        [SerializeField] TextMeshProUGUI textViewName; // 뷰 이름 
        [SerializeField] Image imageBackground; // 뒤의 흰 배경
        [SerializeField] GameObject groupProperty; // 프로퍼티 그룹 (재화, 메일, 등등)
        [SerializeField] HorizontalLayoutGroup propertyHorizontalLayout;
        public GameObject mailButton;           // 프로퍼티 그룹의 메일 버튼
        [SerializeField] GameObject mailNotify; // 메일 알림 표시 
        
        public GameObject multipleButton;       // 여러 용도(저장, 변경 등)로 사용될 버튼
        public TextMeshProUGUI multipleButtonText;      // 다용도 버튼에 들어가는 텍스트


        
        [SerializeField] GameObject logo; // 로고
        
        // * 현재 탑을 제어하는 owner를 설정하려고 했는데, 잠시 보류... 2021.12.07
        [SerializeField] string topOwner = string.Empty;
        
        
        // 바로 이전 상태 저장 변수
        bool previousBackButtonShow = false; // 백버튼의 이전 상태
        bool previousTextViewNameShow = false; // 이전 뷰 이름 상태
        string previousViewName = string.Empty; // 이전 뷰 텍스트
        bool previousGroupPropertyShow = false; // 이전 그룹 프로퍼티 상태 
        [SerializeField] bool previousBackgroundShow = false; // 이전 백그라운드 상태 
        bool previousMailShow = true; // 이전 메일함 버튼 
        
        bool previousLogoShow = false; // 로고 보여주기 
        
        bool previousMultipleButtonShow = false; // 멀티 버튼 보여주기
        string previousMultipleLabelText = string.Empty; // 멀티 버튼 텍스트
        
        
        [SerializeField] bool backgroundSignalValue = true;
        
        
        // Stream, Signal
        
        SignalStream signalStreamTopBackground;
        SignalStream signalStreamTopViewName;
        SignalStream signalStreamTopViewNameExist;
        SignalStream signalStreamTopPropertyGroup;
        SignalStream signalStreamTopMail;
        SignalStream signalStreamTopChangeOwner;
        SignalStream signalStreamTopBackButton;
        SignalStream signalStreamTopMultipleButton;
        SignalStream signalStreamTopMultipleButtonText;
        SignalStream signalStreamRecover;
        SignalStream signalStreamSaveState;
        
        
        SignalReceiver signalReceiverTopBackground;
        SignalReceiver signalReceiverTopViewName;
        SignalReceiver signalReceiverTopViewNameExist;
        SignalReceiver signalReceiverTopPropertyGroup;
        SignalReceiver signalReceiverTopMail;
        SignalReceiver signalReceiverTopChangeOwner;
        SignalReceiver signalReceiverTopBackButton;
        SignalReceiver siganlReceiverTopMultipleButton;
        SignalReceiver signalReceiverTopMultipleButtonText;
        SignalReceiver signalReceiverRecover;
        SignalReceiver signalReceiverSaveState;
        
        private void Awake() {
            // Background 
            signalStreamTopBackground = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND);
            signalReceiverTopBackground = new SignalReceiver().SetOnSignalCallback(OnTopBackgroundSignal);

            signalStreamTopViewNameExist = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST);
            signalReceiverTopViewNameExist = new SignalReceiver().SetOnSignalCallback(OnTopViewNameExistSignal);

            signalStreamTopViewName = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME);
            signalReceiverTopViewName = new SignalReceiver().SetOnSignalCallback(OnTopViewNameSignal);

            signalStreamTopPropertyGroup = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP);
            signalReceiverTopPropertyGroup = new SignalReceiver().SetOnSignalCallback(OnTopPropertySignal);

            signalStreamTopMail = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON);
            signalReceiverTopMail = new SignalReceiver().SetOnSignalCallback(OnTopMailSignal);
            
            signalStreamTopChangeOwner = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_CHANGE_OWNER);
            signalReceiverTopChangeOwner = new SignalReceiver().SetOnSignalCallback(OnTopChangeOwner);
            
            signalStreamTopBackButton = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON);
            signalReceiverTopBackButton = new SignalReceiver().SetOnSignalCallback(OnTopBackButtonSignal);

            // 몇가지 예외적으로 다용도로 사용될 버튼 활성화 및 Action 추가
            signalStreamTopMultipleButton = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON);
            siganlReceiverTopMultipleButton = new SignalReceiver().SetOnSignalCallback(OnTopMultipleSignal);
            signalStreamTopMultipleButtonText = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_MULTIPLE_BUTTON_LABEL);
            signalReceiverTopMultipleButtonText = new SignalReceiver().SetOnSignalCallback(OnTopMultipleLabelSignal);
            

            // 복원 시그널 추가
            signalStreamRecover = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER);
            signalReceiverRecover = new SignalReceiver().SetOnSignalCallback(OnTopRecoverSignal);
            // 상태 저장 시그널 추가
            signalStreamSaveState = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE);
            signalReceiverSaveState = new SignalReceiver().SetOnSignalCallback(OnTopSaveState);
        }
        
        private void Start() {
            
            // 메일 알림 표시 추가
            UserManager.OnRefreshUnreadMailCount = RefreshMailNotification;
            
            signalStreamTopBackground.ConnectReceiver(signalReceiverTopBackground);
            signalStreamTopViewNameExist.ConnectReceiver(signalReceiverTopViewNameExist);
            signalStreamTopViewName.ConnectReceiver(signalReceiverTopViewName);
            signalStreamTopPropertyGroup.ConnectReceiver(signalReceiverTopPropertyGroup);
            signalStreamTopMail.ConnectReceiver(signalReceiverTopMail);
            signalStreamTopChangeOwner.ConnectReceiver(signalReceiverTopChangeOwner);
            signalStreamTopBackButton.ConnectReceiver(signalReceiverTopBackButton);
            signalStreamTopMultipleButton.ConnectReceiver(siganlReceiverTopMultipleButton);
            signalStreamTopMultipleButtonText.ConnectReceiver(signalReceiverTopMultipleButtonText);
            signalStreamRecover.ConnectReceiver(signalReceiverRecover);
            signalStreamSaveState.ConnectReceiver(signalReceiverSaveState);
        }
        
        void OnDisable() {
            signalStreamTopBackground.DisconnectReceiver(signalReceiverTopBackground);
            signalStreamTopViewNameExist.DisconnectReceiver(signalReceiverTopViewNameExist);
            signalStreamTopViewName.DisconnectReceiver(signalReceiverTopViewName);
            signalStreamTopPropertyGroup.DisconnectReceiver(signalReceiverTopPropertyGroup);
            signalStreamTopMail.DisconnectReceiver(signalReceiverTopMail);
            signalStreamTopChangeOwner.DisconnectReceiver(signalReceiverTopChangeOwner);
            signalStreamTopBackButton.DisconnectReceiver(signalReceiverTopBackButton);
            signalStreamTopMultipleButton.DisconnectReceiver(siganlReceiverTopMultipleButton);
            signalStreamTopMultipleButtonText.DisconnectReceiver(signalReceiverTopMultipleButtonText);
            signalStreamRecover.DisconnectReceiver(signalReceiverRecover);
            signalStreamSaveState.DisconnectReceiver(signalReceiverSaveState);
        }
        
        public override void OnView()
        {
            base.OnView();
            
            // propertyHorizontalLayout.enabled = false;
            
        }
        
        public void OnSignalControlBackButton(bool __flag) {
            backButton.SetActive(__flag);
        }
        
        /// <summary>
        /// 상태 정보 저장. 
        /// </summary>
        void SavePreviousState() {
            
            Debug.Log("SavePreviousState");
            
            previousBackButtonShow = backButton.activeSelf;
            previousBackgroundShow = backgroundSignalValue;
            previousGroupPropertyShow = groupProperty.activeSelf;
            previousTextViewNameShow = textViewName.gameObject.activeSelf;
            previousViewName = textViewName.text;
            previousMailShow = mailButton.activeSelf;
            previousLogoShow = !previousBackButtonShow;
            
            // 멀티플 버튼
            previousMultipleButtonShow = multipleButton.activeSelf;
            
            // 멀티플 버튼 레이블
            previousMultipleLabelText = multipleButtonText.text;
        }
        
        /// <summary>
        /// 상태정보 복원
        /// 팝업이나 일부 뷰가 닫힐때 다시 실행한다. 
        /// </summary>
        void RecoverState() {
            backButton.SetActive(previousBackButtonShow);
            groupProperty.SetActive(previousGroupPropertyShow);
            textViewName.gameObject.SetActive(previousTextViewNameShow);
            textViewName.text = previousViewName;
            
            if(previousBackgroundShow) {
                imageBackground.DOFade(1, 0.4f);
            }
            else {
                imageBackground.DOFade(0, 0.4f);
            }
            
            mailButton.SetActive(previousMailShow);
            logo.SetActive(!previousBackButtonShow);
            
            // 멀티플 버튼
            multipleButton.SetActive(previousMultipleButtonShow);
            
            // 멀티플 버튼 레이블
            multipleButtonText.text = previousMultipleLabelText;
            
            
        }

        void OnTopSaveState(Signal signal) {
            SavePreviousState();
        }
        
        void OnTopRecoverSignal(Signal signal) {
            
            Debug.Log("Top Recover Signal Received");
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
            logo.SetActive(!isShow);
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        void OnTopChangeOwner(Signal signal) {
            
          
            if(string.IsNullOrEmpty(signal.message)) {
                Debug.LogError("OnTopChangeOwner has no message");
                return;
            }
            
            staticCurrentTopOwner = topOwner = signal.message;
            Debug.Log(">> Current Top Owner : " + topOwner);
            
        }
        
        
        /// <summary>
        /// 백그라운드 관련 Signal 처리 
        /// </summary>
        /// <param name="signal"></param>
        void OnTopBackgroundSignal(Signal signal) {
            if(!signal.hasValue) {
                Debug.LogError("OnTopBackgroundSignal has no value");
                return;
            }
            
            
            backgroundSignalValue = signal.GetValueUnsafe<bool>();
            
            // 같으면 return.
            if(backgroundSignalValue == isBackgroundShow) {
                Debug.Log("OnTopBackgroundSignal same signal received");
                return; 
            }
                
            isBackgroundShow = backgroundSignalValue; // static 변수도 같이 갱신
            
            
            Debug.Log("OnTopBackgroundSignal : " + backgroundSignalValue);
            
            imageBackground.DOKill();
            
            if(backgroundSignalValue) {
                imageBackground.DOFade(1, 0.4f);
            }
            else {
                Debug.Log("top background out");
                imageBackground.DOFade(0, 0.4f);
            }
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
                textViewName.text = viewName;
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

        void OnTopMultipleSignal(Signal s)
        {
            if(s.hasValue)
            {
                bool isShow = s.GetValueUnsafe<bool>();
                multipleButton.SetActive(isShow);
            }
            else
                multipleButton.SetActive(false);
        }

        void OnTopMultipleLabelSignal(Signal s)
        {
            if(s.hasValue)
            {
                string label = s.GetValueUnsafe<string>();
                multipleButtonText.text = label;
            }
        }

        public void OnClickMultipleButton()
        {
            OnClickButtonAction?.Invoke();
        }
        
        /// <summary>
        /// 메일 알림 표시 
        /// </summary>
        /// <param name="__cnt"></param>
        void RefreshMailNotification(int __cnt) {
            mailNotify.SetActive(__cnt > 0);
        }
    }
}