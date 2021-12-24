using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Doozy.Runtime.Signals;


namespace PIERStory {

    public class PopupReset : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textResetExplain; // 설명
        [SerializeField] EpisodeData targetEpisode; // 리셋을 해서 돌아갈 에피소드 데이터 
        
        
        SignalStream streamReceive;
        SignalReceiver receiverTarget;
        
        
        void Awake() {
            streamReceive = SignalStream.Get(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_EPISODE_RESET);
            receiverTarget = new SignalReceiver().SetOnSignalCallback(OnReceiveSignal);
        }
        
        void Start() {
            streamReceive.ConnectReceiver(receiverTarget);
        }
        
        private void OnEnable() {
            streamReceive.ConnectReceiver(receiverTarget);
        }
        
        void OnDisable() {
            streamReceive.DisconnectReceiver(receiverTarget);
        }
        
        
        
        public void OnClickReset() {
            
            if(targetEpisode == null || !targetEpisode.isValidData) {
                Debug.LogError("No target data OnClickReset");
                return;
            }
            
            // 리셋 
            NetworkLoader.main.ResetEpisodeProgress(targetEpisode.episodeID, true);
        }
        
        
        void OnReceiveSignal(Signal signal) {
            if(!signal.hasValue) {
                Debug.LogError("No Signal in PopupReset");
                return;
            }
            
            Debug.Log("OnReceiveSignal PopupReset");
            targetEpisode = signal.GetValueUnsafe<EpisodeData>();
            
            SetExplain();    
            
        }
        
        void SetExplain() {
            textResetExplain.text = string.Format(SystemManager.GetLocalizedText("6000"), targetEpisode.episodeNO);
        }
    }
}