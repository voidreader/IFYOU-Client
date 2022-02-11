using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class SystemListener : MonoBehaviour
    {
        public static SystemListener main = null;
        
        // * 엔딩플레이 (엔딩에서 바로 플레이)
        SignalStream streamReceiveEndingPlay;
        SignalReceiver receiverEndingPlay;
        public bool isEndingPlay = false; // 엔딩플레이 여부 
        
        #region 소개 페이지
        public StoryData introduceStory; //  소개 페이지의 작품 
        
        SignalReceiver signalReceiverIntroduceStory;
        SignalStream signalStreamIntroduceStory;
        
        #endregion
        
        
        
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
        
        #region 에피소드 종료
        
        SignalReceiver signalReceiverNextData;
        SignalReceiver signalReceiverEpisodeEnd;
       
        SignalStream signalStreamNextData;
        SignalStream signalStreamEpisodeEnd;
        
        public EpisodeData episodeEndNextData = null; // ViewEpisodeEnd 에서 사용하는 다음화 데이터 
        public EpisodeData episodeEndCurrentData = null; // ViewEpisodeEnd 에서 사용하는 현재 데이터 
        
        #endregion
        
        void Awake() {
            // * 씬마다 한개씩 둘것.
                
            main = this;
            
            // 리셋 팝업
            streamReceiveReset = SignalStream.Get(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_EPISODE_RESET);
            receiverResetTarget = new SignalReceiver().SetOnSignalCallback(OnResetSignal);
            
            signalStreamEpisodeStart = SignalStream.Get(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START);
            signalReceiverEpisodeStart = new SignalReceiver().SetOnSignalCallback(OnEpisodeStartSignal);
            
            // * 다음 플레이 에피소드 데이터 수신 
            signalStreamNextData = SignalStream.Get(LobbyConst.STREAM_GAME, GameConst.SIGNAL_NEXT_DATA);
            signalReceiverNextData = new SignalReceiver().SetOnSignalCallback(OnEpisodeEndNextSignal);
            
            // * 방금 플레이했던 에피소드의 데이터 수신 
            signalStreamEpisodeEnd = SignalStream.Get(LobbyConst.STREAM_GAME, GameConst.SIGNAL_EPISODE_END);
            signalReceiverEpisodeEnd = new SignalReceiver().SetOnSignalCallback(OnEpisodeEndCurrentSignal);
            
            
            // * 엔딩 플레이 
            streamReceiveEndingPlay = SignalStream.Get(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_ENDING_PLAY);
            receiverEndingPlay = new SignalReceiver().SetOnSignalCallback(OnEndingPlaySignal);
            
            signalStreamIntroduceStory = SignalStream.Get(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE);
            signalReceiverIntroduceStory = new SignalReceiver().SetOnSignalCallback(OnIntroduceStorySignal);
            
        }
        
        // Start is called before the first frame update
        void Start()
        {
            streamReceiveReset.ConnectReceiver(receiverResetTarget);
            signalStreamEpisodeStart.ConnectReceiver(signalReceiverEpisodeStart);
            
            signalStreamNextData.ConnectReceiver(signalReceiverNextData);
            signalStreamEpisodeEnd.ConnectReceiver(signalReceiverEpisodeEnd);
            
            signalStreamIntroduceStory.ConnectReceiver(signalReceiverIntroduceStory);
        }
        
        void OnDisable() {
            streamReceiveReset.DisconnectReceiver(receiverResetTarget);
            signalStreamEpisodeStart.DisconnectReceiver(signalReceiverEpisodeStart);
            
            signalStreamNextData.DisconnectReceiver(signalReceiverNextData);
            signalStreamEpisodeEnd.DisconnectReceiver(signalReceiverEpisodeEnd);
            
            
            signalStreamIntroduceStory.DisconnectReceiver(signalReceiverIntroduceStory);
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
        
        
        
        void OnEpisodeEndNextSignal(Signal s)
        {
            
            
            // 버튼 세팅(다시하기(엔딩, 사이드), 다음 에피소드 결정)
            if (s.hasValue)
            {
                Debug.Log("ViewGameEnd SIGNAL_NEXT_DATA received");
                episodeEndNextData = s.GetValueUnsafe<EpisodeData>();
            }
        }

        void OnEpisodeEndCurrentSignal(Signal signal)
        {
            
            
            if(signal.hasValue)
            {
                Debug.Log("ViewGameEnd SIGNAL_EPISODE_END received");
                episodeEndCurrentData = signal.GetValueUnsafe<EpisodeData>();
                // SetCurrent... 
                
                
                Signal.Send(LobbyConst.STREAM_GAME, "showEpisodeEnd", string.Empty);

            }
        }
        
        
        void OnEndingPlaySignal(Signal signal)
        {
            Debug.Log("### OnEndingPlaySignal ###");
            isEndingPlay = true;
        }
        
        void OnIntroduceStorySignal (Signal signal) {
            if(!signal.hasValue) {
                Debug.LogError("No Story data!!! in OnIntroduceStorySignal");
                return;
            }
            
            introduceStory = signal.GetValueUnsafe<StoryData>();
            
        }
    }
}