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
        
        public static System.Action OnRefreshEpisodeStart = null;
        
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
        [SerializeField] Image iconOneTimePlayCurrency; // 일반 플레이 재화 아이콘 
        
        [SerializeField] bool isOneTimeUsePossible = false; // 1회권 사용 가능 상태 (티켓이 있다)
        [SerializeField] TextMeshProUGUI textOneTimePrice;   // 1회권 구매 가격
        [SerializeField] TextMeshProUGUI textPremiumPrice;  // 프리미엄 구매 가격
        [SerializeField] TextMeshProUGUI textBtnFree; // btnFree 텍스트
        [SerializeField] TextMeshProUGUI textBtnFreepass; // 프리패스 버튼 텍스트
        
        [SerializeField] EpisodeData episodeData = null; 
        
        [SerializeField] VerticalLayoutGroup buttonVerticalGroup; // 버튼들 버티컬 레이아웃 그룹 
        [SerializeField] GameObject textAdminWarn; // 어드민 유저 경고 문구
        
        JsonData projectCurrent = null; // 이어하기 체크용 Json
        
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
            
            buttonVerticalGroup.enabled = false;
        }
        
        public override void OnStartView() {
            base.OnStartView();
        }        
        
        
        void SetEpisodeInfo() {
            
            textEpisodeTitle.text = episodeData.combinedEpisodeTitle;
            textEpisodeSummary.text = episodeData.episodeSummary;
            
            popupImage.InitImage();
            popupImage.SetDownloadURL(episodeData.popupImageURL, episodeData.popupImageKey);
            
            buttonVerticalGroup.enabled = true;
            
            
            SetButtonState(); // 플레이 버튼 설정 
  
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
            textOneTimePrice.text = episodeData.priceOneTime.ToString();
            
            // 버튼 텍스트 초기화 
            textBtnFree.text = SystemManager.GetLocalizedText("5006"); 
            textBtnFreepass.text = SystemManager.GetLocalizedText("5004");

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
            
            
            // 이어하기 가능한 경우는 버튼 텍스트를 교체 
            if (LobbyManager.main != null && CheckResumePossible()) {
                // 5005: 이어하기 
                textBtnFree.text = SystemManager.GetLocalizedText("5005"); 
                textBtnFreepass.text = SystemManager.GetLocalizedText("5005");
            }
            
            // * 에피소드 플레이 상태가 future의 상태인 경우는 플레이 불가 (버튼 제거)
            if (LobbyManager.main != null && episodeData.episodeState == EpisodeState.Future)
            {
                textEpisodeSummary.text = SystemManager.GetLocalizedText("80072");
                btnLock.SetActive(true);
                // thumbnailLock.gameObject.SetActive(true);
                
                return;
            }
            
            // * current & next (플레이 가능한 상태)
            
            // * 프리패스 유저 체크, 프리패스 유저이면 프리패스 버튼 하나만 활성화 
            if(UserManager.main.HasProjectFreepass()) {
                btnFreepass.SetActive(true);
                return;
            }
            
            // * 프리패스 아님, 구매상태 체크해서 버튼 설정
            // * 무료 작품인지 먼저 체크
            if(episodeData.pricePremiumSale <= 0) {
                btnPlay.SetActive(true);
                return; 
            } // 무료면 무료버튼만 나오고 끝!
            
            // 구매 기록 체크 
            if((episodeData.purchaseState == PurchaseState.OneTime && episodeData.OneTimePlayable)
                || episodeData.purchaseState == PurchaseState.Permanent) {
                
                btnPlay.SetActive(true);        
                return;
            }
            
            // 작품의 1회 플레이 티켓이 있는 경우 재화 아이콘과 가격 레이블 변경 
            ChangeOnetimePlayCurrencyIcon(); 
            
            
            // * 여기서부터 일반 미구매 유정 
            if(episodeData.episodeType == EpisodeType.Chapter)  // 정규 챕터만 1회 플레이 가능 
                btnOneTime.SetActive(true);
                
            btnPremium.SetActive(true);
            
            // 종료 
            
        } // ? end of SetButtonState
        
        
        /// <summary>
        /// 이어하기 가능한지 체크 
        /// </summary>
        /// <returns></returns>
        bool CheckResumePossible() {
            projectCurrent = UserManager.main.GetUserProjectCurrent(episodeData.episodeID);
            
            if(projectCurrent == null || string.IsNullOrEmpty(projectCurrent["scene_id"].ToString()) || projectCurrent["is_final"].ToString().Equals("1"))
                return false;
                
            return true;
        }
        
        /// <summary>
        /// 1회 플레이 재화 아이콘 수정
        /// </summary>
        void ChangeOnetimePlayCurrencyIcon()
        {
            if (UserManager.main.GetOneTimeProjectTicket(StoryManager.main.CurrentProjectID) > 0)
            {
                textOneTimePrice.text = "1";
                iconOneTimePlayCurrency.sprite = LobbyManager.main.spriteOneTimeIcon; // 티켓 이미지로 변경한다. 
                iconOneTimePlayCurrency.SetNativeSize();

                isOneTimeUsePossible = true;
            }
            else
            {
                iconOneTimePlayCurrency.sprite = LobbyManager.main.spriteGemIcon; // 1회권 없으면 보석 처리 
            }
        }
        
        
        #region 플레이 버튼 클릭 이벤트
        
        public void OnClickPlayButton() {
            
        }
        
        #endregion
                
    }
}