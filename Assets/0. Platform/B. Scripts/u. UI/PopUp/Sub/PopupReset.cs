using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Doozy.Runtime.Signals;


namespace PIERStory {

    public class PopupReset : CommonView
    {
        [SerializeField] TextMeshProUGUI textResetExplain; // 설명
        [SerializeField] EpisodeData targetEpisode; // 리셋을 해서 돌아갈 에피소드 데이터 
        
        

        
        
        void Awake() {

        }
        
        
        public override void OnView() {
            base.OnView();
        }
        
        public override void OnStartView() {
            base.OnStartView();
            targetEpisode = SystemListener.main.resetTargetEpisode;
            SetExplain();
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