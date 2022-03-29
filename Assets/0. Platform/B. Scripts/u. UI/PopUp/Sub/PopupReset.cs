using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Doozy.Runtime.Signals;


namespace PIERStory {
    
    /// <summary>
    /// 구 Reset 뷰 (이름은 팝업인데 뷰..)
    /// </summary>
    public class PopupReset : CommonView
    {
        [SerializeField] TextMeshProUGUI textResetExplain; // 설명
        [SerializeField] EpisodeData targetEpisode; // 리셋을 해서 돌아갈 에피소드 데이터 
        
        [SerializeField] int currentResetPrice = 0;
        [SerializeField] int currentResetCount = 0;

        [SerializeField] TextMeshProUGUI textResetCoinPrice; // 리셋 코인 가격
        [SerializeField] TextMeshProUGUI textStoryResetCount; // 작품 리셋 횟수
        
        
        void Awake() {

        }
        
        
        public override void OnView() {
            base.OnView();
            
            
        }
        
        public override void OnStartView() {
            base.OnStartView();
            targetEpisode = SystemListener.main.resetTargetEpisode;
            
            // 현재 작품의 리셋 가격과 리셋 횟수 가져오기. 
            // currentResetCount = UserManager.main.GetProjectResetCount();
            // currentResetPrice = UserManager.main.GetProjectResetPrice();
            
            // 소모가격 세팅 
            textResetCoinPrice.text = currentResetPrice.ToString();
            
            
        }
        
        public void OnClickReset() {
            
            if(targetEpisode == null || !targetEpisode.isValidData) {
                Debug.LogError("No target data OnClickReset");
                return;
            }
            
            // 잔고 체크
            if(!UserManager.main.CheckCoinProperty(currentResetPrice)) {
                SystemManager.ShowMessageWithLocalize("80013");
                return;
            }
            
            
            
            // 리셋 
            NetworkLoader.main.ResetEpisodeProgress(targetEpisode.episodeID, currentResetPrice, true);
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
        
        /// <summary>
        /// 값이 필요한 설명글 처리 
        /// </summary>
        public void SetExplain() {
            textResetExplain.text = string.Format(SystemManager.GetLocalizedText("6000"), targetEpisode.episodeNO);
            textStoryResetCount.text = string.Format(SystemManager.GetLocalizedText("6103"), currentResetCount);
        }
    }
}