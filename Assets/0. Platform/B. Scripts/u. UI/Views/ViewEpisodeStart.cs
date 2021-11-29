using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class ViewEpisodeStart : CommonView
    {
        
        [SerializeField] ImageRequireDownload popupImage; // 이미지 
        [SerializeField] TextMeshProUGUI textEpisodeTitle;
        [SerializeField] TextMeshProUGUI textEpisodeSummary;
        
        
        [Space]
        [Header("== 버튼 ==")]
        [SerializeField] GameObject btnPlay; // 플레이 
        [SerializeField] GameObject btnLock; // 잠금
        [SerializeField] GameObject btnConinue; // 이어서 플레이
        [SerializeField] GameObject btnPremium; // 프리미엄
        [SerializeField] GameObject btnOneTime; // 1회 플레이
        [SerializeField] GameObject btnFreepass; // 프리패스
        
        [SerializeField] bool isOneTimeUsePossible = false; // 1회권 사용 가능 상태 
        [SerializeField] TextMeshProUGUI textNormalPrice;   // 1회권 구매 가격
        [SerializeField] TextMeshProUGUI textPremiumPrice;  // 프리미엄 구매 가격
        
        [SerializeField] EpisodeData episodeData = null; 
        
        [SerializeField] VerticalLayoutGroup buttonVerticalGroup; // 버튼들 버티컬 레이아웃 그룹 
        [SerializeField] GameObject textAdminWarn; // 어드민 유저 경고 문구
        
        SignalReceiver signalReceiver;
        SignalStream signalStream;
        
        #region Meta Signal 수신 관련 처리 
        
        void Awake() {
            signalStream = SignalStream.Get(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START);
            signalReceiver = new SignalReceiver().SetOnSignalCallback(OnSignal);
        }
        
        private void OnEnable()
        {
            //add the receiver to react to signals sent through the stream
            signalStream.ConnectReceiver(signalReceiver);
        }

        private void OnDisable()
        {
            //remove the receiver from reacting to signals sent through the stream
            signalStream.DisconnectReceiver(signalReceiver);
        }        
        
        private void OnSignal(Signal signal)
        {
           //check if signal is MetaSignal
           if (signal.hasValue)
           {
               
               // Type valueType = signal.valueType; //get the payload value type
               Debug.Log("ViewEpisodeStart OnSingal");
               episodeData = signal.GetValueUnsafe<EpisodeData>();
               SetEpisodeInfo();
           }
        }
        
        #endregion
        
        public override void OnView()
        {
            base.OnView();
        }
        
        public override void OnStartView() {
            base.OnStartView();
        }        
        
        
        void SetEpisodeInfo() {
            
            textEpisodeTitle.text = episodeData.combinedEpisodeTitle;
            textEpisodeSummary.text = episodeData.episodeSummary;
  
        }
        
        
        /// <summary>
        /// 플레이 버튼들 리셋 
        /// </summary>
        void ResetButton()
        {
            btnPlay.gameObject.SetActive(false);
            btnLock.gameObject.SetActive(false);
            btnOneTime.gameObject.SetActive(false);
            btnPremium.gameObject.SetActive(false);
            btnFreepass.gameObject.SetActive(false);

            textPremiumPrice.text = episodeData.pricePremiumSale.ToString();
            textNormalPrice.text = episodeData.priceOneTime.ToString();

            isOneTimeUsePossible = false;
        }
        
        /// <summary>
        /// 에피소드 플레이, 구매 상태에 따른 버튼 설정 
        /// </summary>
        void SetButtonState() {
            
            // 엔딩 수집화면에서 진입하는 경우는 그냥 FREE
            if(LobbyManager.main != null && !UserManager.main.useRecord) {
                btnPlay.gameObject.SetActive(true);
                return;
            }
        }
    }
}