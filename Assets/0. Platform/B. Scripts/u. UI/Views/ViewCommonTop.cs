using System.Collections;
using System.Collections.Generic;
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
        
        
        [SerializeField] GameObject backButton; // 뒤로가기 버튼 
        [SerializeField] TextMeshProUGUI textViewName; // 뷰 이름 
        [SerializeField] Image imageBackground; // 뒤의 흰 배경
        [SerializeField] GameObject groupProperty; // 프로퍼티 그룹 (재화, 메일, 등등)
        [SerializeField] HorizontalLayoutGroup propertyHorizontalLayout;
        public GameObject mailButton;           // 프로퍼티 그룹의 메일 버튼
        
        // * 현재 탑을 제어하는 owner를 설정하려고 했는데, 잠시 보류... 2021.12.07
        [SerializeField] string topOwner = string.Empty;
        
        
        // 바로 이전 상태 저장 변수
        bool previousBackButtonShow = false; // 백버튼의 이전 상태
        bool previousTextViewNameShow = false; // 이전 뷰 이름 상태
        string previousViewName = string.Empty; // 이전 뷰 텍스트
        bool previousGroupPropertyShow = false; // 이전 그룹 프로퍼티 상태 
        [SerializeField] bool previousBackgroundShow = false; // 이전 백그라운드 상태 
        bool previousMailShow = true; // 이전 메일함 버튼 
        
        
        [SerializeField] bool backgroundSignalValue = true;
        
        
        // Stream, Signal
        
        SignalStream signalStreamTopBackground;
        SignalStream signalStreamTopViewName;
        SignalStream signalStreamTopViewNameExist;
        SignalStream signalStreamTopPropertyGroup;
        SignalStream signalStreamTopMail;
        SignalStream signalStreamTopChangeOwner;
        SignalStream signalStreamTopBackButton;
        SignalStream signalStreamRecover;
        SignalStream signalStreamSaveState;
        
        
        SignalReceiver signalReceiverTopBackground;
        SignalReceiver signalReceiverTopViewName;
        SignalReceiver signalReceiverTopViewNameExist;
        SignalReceiver signalReceiverTopPropertyGroup;
        SignalReceiver signalReceiverTopMail;
        SignalReceiver signalReceiverTopChangeOwner;
        SignalReceiver signalReceiverTopBackButton;
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
            
            // 복원 시그널 추가
            signalStreamRecover = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER);
            signalReceiverRecover = new SignalReceiver().SetOnSignalCallback(OnTopRecoverSignal);
            // 상태 저장 시그널 추가
            signalStreamSaveState = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE);
            signalReceiverSaveState = new SignalReceiver().SetOnSignalCallback(OnTopSaveState);
        }
        
        private void Start() {
            signalStreamTopBackground.ConnectReceiver(signalReceiverTopBackground);
            signalStreamTopViewNameExist.ConnectReceiver(signalReceiverTopViewNameExist);
            signalStreamTopViewName.ConnectReceiver(signalReceiverTopViewName);
            signalStreamTopPropertyGroup.ConnectReceiver(signalReceiverTopPropertyGroup);
            signalStreamTopMail.ConnectReceiver(signalReceiverTopMail);
            signalStreamTopChangeOwner.ConnectReceiver(signalReceiverTopChangeOwner);
            signalStreamTopBackButton.ConnectReceiver(signalReceiverTopBackButton);
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
            previousBackButtonShow = backButton.activeSelf;
            previousBackgroundShow = backgroundSignalValue;
            previousGroupPropertyShow = groupProperty.activeSelf;
            previousTextViewNameShow = textViewName.gameObject.activeSelf;
            previousViewName = textViewName.text;
            previousMailShow = mailButton.activeSelf;
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
            
            if(backgroundSignalValue) {
                imageBackground.DOFade(1, 0.4f);
            }
            else {
                imageBackground.DOFade(0, 0.4f);
            }
            
            mailButton.SetActive(previousMailShow);
            
            
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
            if(backgroundSignalValue == isBackgroundShow)
                return; 
                
            isBackgroundShow = backgroundSignalValue; // static 변수도 같이 갱신
            
            
            // Debug.Log("OnTopBackgroundSignal : " + backgroundSignalValue);
            
            imageBackground.DOKill();
            
            
            if(backgroundSignalValue) {
                imageBackground.DOFade(1, 0.4f);
            }
            else {
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
        
    }
}