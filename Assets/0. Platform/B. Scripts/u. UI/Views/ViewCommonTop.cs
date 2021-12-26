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
        
        
        [SerializeField] GameObject backButton;
        [SerializeField] TextMeshProUGUI textViewName;
        [SerializeField] Image imageBackground;
        [SerializeField] GameObject groupProperty; // 프로퍼티 그룹 (재화, 메일, 등등)
        
        // * 현재 탑을 제어하는 owner를 설정하려고 했는데, 잠시 보류... 2021.12.07
        [SerializeField] string topOwner = string.Empty;
        
        
        bool backgroundSignalValue = true;
        
        
        // Stream, Signal
        
        SignalStream signalSteamTopBackground;
        SignalStream signalSteamTopViewName;
        SignalStream signalSteamTopViewNameExist;
        SignalStream signalSteamTopPropertyGroup;
        SignalStream signalSteamTopChangeOwner;
        SignalStream signalSteamTopBackButton;
        
        
        SignalReceiver signalReceiverTopBackground;
        SignalReceiver signalReceiverTopViewName;
        SignalReceiver signalReceiverTopViewNameExist;
        SignalReceiver signalReceiverTopPropertyGroup;
        SignalReceiver signalReceiverTopChangeOwner;
        SignalReceiver signalReceiverTopBackButton;
        
        private void Awake() {
            // Background 
            signalSteamTopBackground = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND);
            signalReceiverTopBackground = new SignalReceiver().SetOnSignalCallback(OnTopBackgroundSignal);

            signalSteamTopViewNameExist = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST);
            signalReceiverTopViewNameExist = new SignalReceiver().SetOnSignalCallback(OnTopViewNameExistSignal);

            signalSteamTopViewName = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME);
            signalReceiverTopViewName = new SignalReceiver().SetOnSignalCallback(OnTopViewNameSignal);

            signalSteamTopPropertyGroup = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP);
            signalReceiverTopPropertyGroup = new SignalReceiver().SetOnSignalCallback(OnTopPropertySignal);
            
            signalSteamTopChangeOwner = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_CHANGE_OWNER);
            signalReceiverTopChangeOwner = new SignalReceiver().SetOnSignalCallback(OnTopChangeOwner);
            
            signalSteamTopBackButton = SignalStream.Get(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON);
            signalReceiverTopBackButton = new SignalReceiver().SetOnSignalCallback(OnTopBackButtonSignal);
        }
        
        private void Start() {
            signalSteamTopBackground.ConnectReceiver(signalReceiverTopBackground);
            signalSteamTopViewNameExist.ConnectReceiver(signalReceiverTopViewNameExist);
            signalSteamTopViewName.ConnectReceiver(signalReceiverTopViewName);
            signalSteamTopPropertyGroup.ConnectReceiver(signalReceiverTopPropertyGroup);
            signalSteamTopChangeOwner.ConnectReceiver(signalReceiverTopChangeOwner);
            signalSteamTopBackButton.ConnectReceiver(signalReceiverTopBackButton);
        }
        
        void OnDisable() {
            signalSteamTopBackground.DisconnectReceiver(signalReceiverTopBackground);
            signalSteamTopViewNameExist.DisconnectReceiver(signalReceiverTopViewNameExist);
            signalSteamTopViewName.DisconnectReceiver(signalReceiverTopViewName);
            signalSteamTopPropertyGroup.DisconnectReceiver(signalReceiverTopPropertyGroup);
            signalSteamTopChangeOwner.DisconnectReceiver(signalReceiverTopChangeOwner);
            signalSteamTopBackButton.DisconnectReceiver(signalReceiverTopBackButton);
        }
        
        public override void OnView()
        {
            base.OnView();
        }
        
        public void OnSignalControlBackButton(bool __flag) {
            backButton.SetActive(__flag);
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
        
    }
}