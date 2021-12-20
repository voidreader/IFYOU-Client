using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using Doozy.Runtime.Signals;
using UnityEngine.SceneManagement;

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
        
        [SerializeField] float illustProgress = 0;
        [SerializeField] float sceneProgress = 0;
        [SerializeField] EpisodeContentProgress illustProgressBar;
        [SerializeField] EpisodeContentProgress sceneProgressBar;
        [SerializeField] GameObject contentsMiddleVerticalLine; // 일러스트, 경험한 사건 사이에 선 
        
        SignalReceiver signalReceiverEpisodeStart;
        SignalStream signalStreamEpisodeStart;
        
        #region Meta Signal 수신 관련 처리 
        
        void Awake() {
            signalStreamEpisodeStart = SignalStream.Get(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START);
            signalReceiverEpisodeStart = new SignalReceiver().SetOnSignalCallback(OnSignal);
        }
        
        private void OnEnable()
        {
            //add the receiver to react to signals sent through the stream
            signalStreamEpisodeStart.ConnectReceiver(signalReceiverEpisodeStart);
        }

        private void OnDisable()
        {
            //remove the receiver from reacting to signals sent through the stream
            signalStreamEpisodeStart.DisconnectReceiver(signalReceiverEpisodeStart);
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
            
            // 진행도 처리 
            illustProgress = episodeData.episodeGalleryImageProgressValue;
            sceneProgress = episodeData.sceneProgressorValue;
            
            // 갤러리 프로그레스. -1이면 없다. 
            if(illustProgress > -1) {
                contentsMiddleVerticalLine.SetActive(true);
                illustProgressBar.gameObject.SetActive(true);
                illustProgressBar.SetProgress(illustProgress);
            }
            else {
                illustProgressBar.gameObject.SetActive(false);
                contentsMiddleVerticalLine.SetActive(false);
            }
            
            sceneProgressBar.SetProgress(sceneProgress);
            
            
            
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
            
            ResetButton();
            
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
        
        
        
        
        /// <summary>
        /// 플레이 버튼 클릭!
        /// </summary>
        public void OnClickPlayButton() {
            SystemManager.main.givenEpisodeData = episodeData;
            SystemManager.ShowNetworkLoading(); 
            
            // * 구매기록이 없으면, 구매처리를 한다. (0원)
            if(!episodeData.CheckExistsPurchaseData()) {
                UserManager.OnRequestEpisodePurchase = PurchasePostProcess;
                NetworkLoader.main.PurchaseEpisode(episodeData.episodeID, PurchaseState.Permanent, episodeData.currencyPremuim, "0");
            }
            else {
                PurchasePostProcess(true);
            }
            
        }
        
        /// <summary>
        /// 1회권 플레이 
        /// </summary>
        public void OnClickOneTimePlay() {
            
            // 1회권 사용이 강제되는 상태 
            if(isOneTimeUsePossible) {
                string oneTimeCurrency = StoryManager.main.GetProjectCurrencyCode(CurrencyType.OneTime);
                
                // 올바른 화폐코드가 없는 경우에 오류 처리 
                if(string.IsNullOrEmpty(oneTimeCurrency)) {
                    SystemManager.ShowAlertWithLocalize("80073");
                    return;
                }
                
                PurchaseEpisode(PurchaseState.OneTime, oneTimeCurrency, 1);
                return;
            }
            
            // 1회권 보유하지 않음, 일반 진행 
            if(episodeData.currencyOneTime == "coin" && !UserManager.main.CheckCoinProperty(episodeData.priceOneTime))
            {
                SystemManager.ShowSimpleMessagePopUp(SystemManager.GetLocalizedText("80013"));
                return;
            }
                
            if(episodeData.currencyOneTime == "gem" && !UserManager.main.CheckGemProperty(episodeData.priceOneTime))
            {
                SystemManager.ShowSimpleMessagePopUp(SystemManager.GetLocalizedText("80014"));
                return;
            }
            
           
            // 에피소드 구매 고고 
            PurchaseEpisode(PurchaseState.OneTime, episodeData.currencyOneTime, episodeData.priceOneTime);            
            
        } // ? 1회 플레이 종료
        
        
        /// <summary>
        /// 프리미엄, 프리패스 플레이 
        /// </summary>
        public void OnClickPremiumPlay() {
            
            // 프리패스가 없으면 재화 체크하기
            if(!UserManager.main.HasProjectFreepass())
            {
                // 돈없을때 처리 
                if (episodeData.currencyPremuim == "coin" && !UserManager.main.CheckCoinProperty(episodeData.pricePremiumSale))
                {
                    SystemManager.ShowSimpleMessagePopUp(SystemManager.GetLocalizedText("80013"));
                    return;
                }

                if (episodeData.currencyPremuim == "gem" && !UserManager.main.CheckGemProperty(episodeData.pricePremiumSale))
                {
                    SystemManager.ShowSimpleMessagePopUp(SystemManager.GetLocalizedText("80014"));
                    return;
                }
            }

            // 진행             
            PurchaseEpisode(PurchaseState.Permanent, episodeData.currencyPremuim, episodeData.pricePremiumSale);
        }
        
        
        public void OnClickLock() {
            if(!UserManager.main.CheckAdminUser())
                return;

            // 어드민 유저만                 
            SystemManager.main.givenEpisodeData = episodeData;
            SystemManager.ShowNetworkLoading();
            StartGame();
        }
         
        
        #endregion
        
        
        /// <summary>
        /// 에피소드 구매처리 (통신 시작점)
        /// </summary>
        /// <param name="__state">구매 타입</param>
        /// <param name="__currency">재화코드</param>
        /// <param name="__price">소모 재화 개수</param>
        void PurchaseEpisode(PurchaseState __state, string __currency, int __price) {
            
            Debug.Log(string.Format("PurchaseEpisode {0}/{1}/{2}", __state.ToString(), __currency, __price));
            
            SystemManager.main.givenEpisodeData = episodeData;
            SystemManager.ShowNetworkLoading();
            UserManager.OnRequestEpisodePurchase = PurchasePostProcess;
            
            // 서버에서 알아서 처리해주기!
            NetworkLoader.main.PurchaseEpisode(episodeData.episodeID, __state, __currency, __price.ToString());
            
        }
        
        /// <summary>
        /// 구매 후 처리. 
        /// </summary>
        /// <param name="__isPurchaseSuccess"></param>
        void PurchasePostProcess(bool __isPurchaseSuccess)
        {
            Debug.Log(">> PurchasePostProcess : + " + __isPurchaseSuccess);
            
            if (!__isPurchaseSuccess)
            {
                Debug.LogError("Error in purchase");
                // 추가 메세지 처리 
                // ! NetworkLoader에서 메세지 띄우니까 안해도 된다. 
                return;
            }
            
            // * 구매 성공시,  현재 에피소드의 구매 정보를 갱신한다. 
            // ! 구매 정보는 갱신하고, 플레이 버튼들은 변경하지 않는다. 
            episodeData.SetPurchaseState();
            
            
            
            // 에디터 환경에서는 그냥 실행
            if(Application.isEditor) {
                StartGame();
                return;
            }
            
            // * 에디터 환경이 아닌 경우는 네트워크 상태를 체크한다. 
            if(PlayerPrefs.GetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 0) > 1) { // * 와이파이 접속이 아닌 환경에서도 게임을 플레이 하겠다고 했음. 
                StartGame(); 
            }
            else { // 동의 받지 않은 상태 
                
                // 동의받지 않았어도 wifi 상태면, 게임 시작. 
                if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
                    StartGame();
                    return;
                }
                
                // wifi 아닌경우. 
                if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {
                    SystemManager.ShowConfirmPopUp(SystemManager.GetLocalizedText("80075"), ChangeNetworkSetting, OnRejectPlayWihoutWifi);
                }
            }

        } // ? End of purchasePostProcess
        
        /// <summary>
        /// 데이터 환경 다운로드 허용하면서 시작
        /// </summary>
        void ChangeNetworkSetting()
        {
            PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 1);
            StartGame();
        }
        
        /// <summary>
        /// 3G, LTE 환경에서 게임 플레이 하지 않음 => 리프레시 처리만 해주도록 한다. 
        /// </summary>
        void OnRejectPlayWihoutWifi() {
            
            // * 무조건 리프레시를 하면 로비에서 진입할때 어색함. 
            // !  3G, LTE 환경에서 플레이 하지 않는다고 선택하면, 이미 구매는 된 상태기 때문에 
            // ! 플레이 버튼 재설정해준다. 
            
            SetButtonState();
        }        
        
        /// <summary>
        /// 찐 게임 start
        /// </summary>
        void StartGame()
        {
            Debug.Log("Game Start!!!!!");
            IntermissionManager.isMovingLobby = false; // 게임으로 진입하도록 요청
            
            // 다음 에피소드 진행 
            // * 2021.09.23 iOS 메모리 이슈를 해결하기 위해 중간 Scene을 거쳐서 실행하도록 처리 
            // * GameScene에서 게임이 시작되는 경우만!
            if(GameManager.main != null) {
                
                SceneManager.LoadSceneAsync("Intermission", LoadSceneMode.Single).allowSceneActivation = true;
            }
            else {
                SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single).allowSceneActivation = true;
            }
            
            // ! 이어하기 체크 
            string lastPlaySceneID = null;
            long lastPlayScriptNO = 0;

            // 플레이 지점 저장 정보를 가져오자. 
            if (CheckResumePossible())
            { // 이어하기 가능한 상태.
                lastPlaySceneID = projectCurrent["scene_id"].ToString();
                lastPlayScriptNO = long.Parse(projectCurrent["script_no"].ToString());

                // ! 어드민 유저는 이어하기를 사용할 수 없다.                 
                // if(!UserManager.main.CheckAdminUser())
                GameManager.SetResumeGame(lastPlaySceneID, lastPlayScriptNO);
            }
            else
            {
                GameManager.SetNewGame(); // 새로운 게임
            }

            // 통신 
            NetworkLoader.main.UpdateUserProjectCurrent(episodeData.episodeID, lastPlaySceneID, lastPlayScriptNO);
        }        
        
                
    }
}