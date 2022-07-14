using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class SystemListener : MonoBehaviour
    {
        public static SystemListener main = null;
        
        #region 소개 페이지
        public StoryData introduceStory; //  소개 페이지의 작품 
        public bool isIntroduceRecommended = false; // 소개페이지에서 추천받음 
        
        SignalReceiver signalReceiverIntroduceStory;
        SignalStream signalStreamIntroduceStory;
        
        #endregion
        
        
        #region 리셋 관련
        public EpisodeData resetTargetEpisode; // 리셋을 해서 돌아갈 에피소드 데이터 
        SignalStream streamReceiveReset;
        SignalReceiver receiverResetTarget;
        #endregion
        
        void Awake() {
                
            main = this;

            signalStreamIntroduceStory = SignalStream.Get(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE);
            signalReceiverIntroduceStory = new SignalReceiver().SetOnSignalCallback(OnIntroduceStorySignal);

            // 리셋 팝업
            streamReceiveReset = SignalStream.Get(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_EPISODE_RESET);
            receiverResetTarget = new SignalReceiver().SetOnSignalCallback(OnResetSignal);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            signalStreamIntroduceStory.ConnectReceiver(signalReceiverIntroduceStory);
            streamReceiveReset.ConnectReceiver(receiverResetTarget);
        }
        
        void OnDisable() {
            signalStreamIntroduceStory.DisconnectReceiver(signalReceiverIntroduceStory);
            
            streamReceiveReset.DisconnectReceiver(receiverResetTarget);
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
        
        void OnIntroduceStorySignal (Signal signal) {
            if(!signal.hasValue) {
                Debug.LogError("No Story data!!! in OnIntroduceStorySignal");
                return;
            }
            
            introduceStory = signal.GetValueUnsafe<StoryData>();
            isIntroduceRecommended = false;   
            
            if(!string.IsNullOrEmpty(signal.message)) {
                isIntroduceRecommended = true; 
            }
            
        }
    }
}