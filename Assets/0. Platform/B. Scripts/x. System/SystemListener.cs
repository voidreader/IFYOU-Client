using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class SystemListener : MonoBehaviour
    {
        public static SystemListener main = null;
        
        
        #region 리셋 관련
        public EpisodeData resetTargetEpisode; // 리셋을 해서 돌아갈 에피소드 데이터 
        SignalStream streamReceiveReset;
        SignalReceiver receiverResetTarget;
        #endregion
        
        #region 에피소드 시작 
        SignalReceiver signalReceiverEpisodeStart;
        SignalStream signalStreamEpisodeStart;
        public EpisodeData startEpisode; // 에피소드 시작화면 에피소드 데이터 
        #endregion
        
        void Awake() {
            
            if(main != null) {
                Destroy(this.gameObject);
                return;
            }
                
            main = this;
            
            // 리셋 팝업
            streamReceiveReset = SignalStream.Get(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_EPISODE_RESET);
            receiverResetTarget = new SignalReceiver().SetOnSignalCallback(OnResetSignal);
            
            signalStreamEpisodeStart = SignalStream.Get(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START);
            signalReceiverEpisodeStart = new SignalReceiver().SetOnSignalCallback(OnEpisodeStartSignal);
            
            DontDestroyOnLoad(this);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            streamReceiveReset.ConnectReceiver(receiverResetTarget);
            signalStreamEpisodeStart.ConnectReceiver(signalReceiverEpisodeStart);
        }
        
        void OnDisable() {
            streamReceiveReset.DisconnectReceiver(receiverResetTarget);
            signalStreamEpisodeStart.DisconnectReceiver(signalReceiverEpisodeStart);
        }
        
        
        /// <summary>
        /// 리셋 시그널 
        /// </summary>
        /// <param name="signal"></param>
        public void OnResetSignal(Signal signal) {
            if(!signal.hasValue) {
                Debug.LogError("No Signal in OnResetSignal");
                return;
            }
            resetTargetEpisode = signal.GetValueUnsafe<EpisodeData>();
        }
        
        private void OnEpisodeStartSignal(Signal signal)
        {
            if(!signal.hasValue) {
                Debug.LogError("No Signal in OnEpisodeStart");
                return;
            }
            
            // Type valueType = signal.valueType; //get the payload value type
            Debug.Log("ViewEpisodeStart OnSingal");
            startEpisode = signal.GetValueUnsafe<EpisodeData>();
        }
    }
}