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
        
        [SerializeField] bool isEpisodeContinuePlay = false; // 에피소드 이어하기 상태? 
        
        [SerializeField] ImageRequireDownload popupImage; // 이미지 
        [SerializeField] TextMeshProUGUI textEpisodeTitle;
        [SerializeField] TextMeshProUGUI textEpisodeSummary;
        
        
        [Space]
        [Header("== 버튼 ==")]
        [SerializeField] GameObject btnPlay; // 플레이 (엔딩에서 진입하거나, 구매만 한 상태의 경우)
        [SerializeField] GameObject btnLock; // 잠금
        [SerializeField] GameObject btnConinue; // 이어서 플레이
        [SerializeField] GameObject btnStarPlay; // 스타플레이
        [SerializeField] GameObject btnAdPlay; // 무료 플레이
        [SerializeField] GameObject btnPremiumPass; // 프리미엄 패스

        
        
        [SerializeField] GameObject continueButtonIcon; // 이어서 플레이 버튼에 붙는 아이콘
        [SerializeField] GameObject continueButtonAD; // 이어서 플레이 버튼에 붙는 광고사인
        [SerializeField] TextMeshProUGUI textStarPlayPrice;  // 스타플레이 구매 가격
        
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
        
        public override void OnView()
        {
            base.OnView();
            
            buttonVerticalGroup.enabled = false;
        }
        
        public override void OnStartView() {
            base.OnStartView();
            SetEpisodeInfo();
        }        
        
        
        void SetEpisodeInfo() {
            
            episodeData = SystemListener.main.startEpisode;
            if(episodeData == null || !episodeData.isValidData) {
                Debug.LogError("Wrong Episode data");
            }
            
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
            btnAdPlay.gameObject.SetActive(false);
            btnStarPlay.gameObject.SetActive(false);
            btnPremiumPass.gameObject.SetActive(false);
            
            // 이어서 플레이 버튼 
            btnConinue.SetActive(false);
            continueButtonAD.SetActive(false);
            continueButtonIcon.SetActive(true); 
            isEpisodeContinuePlay = false;

            textStarPlayPrice.text = episodeData.priceStarPlaySale.ToString();
            
            // 버튼 텍스트 초기화 
            // textBtnFree.text = SystemManager.GetLocalizedText("5006"); 
            textBtnFreepass.text = SystemManager.GetLocalizedText("5004");

            // isOneTimeUsePossible = false;
        }
        
        /// <summary>
        /// 에피소드 플레이, 구매 상태에 따른 버튼 설정 
        /// </summary>
        void SetButtonState() {
            
            ResetButton();
            
            // 엔딩 수집화면에서 진입하는 경우는 플레이 버튼만 노출한다.
            if(LobbyManager.main != null && !UserManager.main.useRecord) {
                btnPlay.gameObject.SetActive(true);
                return;
            }
           
            // * 에피소드 플레이 상태가 future의 상태인 경우는 플레이 불가 => Lock 버튼 노출
            if (LobbyManager.main != null && episodeData.episodeState == EpisodeState.Future)
            {
                textEpisodeSummary.text = SystemManager.GetLocalizedText("80072");
                btnLock.SetActive(true);
                return;
            }
            
            // * current & next (플레이 가능한 상태)
            
            // 이어하기가 가능한 경우와 아닌 경우를 분류한다. 
            // 이어하기 처리 (로비씬에서만 가능하다.)
            if (LobbyManager.main != null && CheckResumePossible()) {
                Debug.Log("continue play possible");
                isEpisodeContinuePlay = true;
                // 5005: 프리미엄 패스에 이어하기 텍스트 처리 해놓기. 
                textBtnFreepass.text = SystemManager.GetLocalizedText("5005");
            }
            
            // * 프리패스 유저 체크, 프리패스 유저이면 프리패스 버튼 하나만 활성화 
            if(UserManager.main.HasProjectFreepass()) {
                Debug.Log("Premium Pass USER!!!");
                btnPremiumPass.SetActive(true);
                return;
            }
            
            // 구매기록 체크... 구매기록이 있는 경우와 아닌 경우 
            
            
            // * 프리패스 아님, 구매상태 체크해서 버튼 설정
            // * 이어하기와 아닌 경우에 대한 분리 
            if(isEpisodeContinuePlay) {
                
                // 스타플레이 구매기록 있음 
                if(episodeData.purchaseState == PurchaseState.Permanent) {
                    btnConinue.SetActive(true);
                    return;
                }
                
                // 스타플레이 구매기록 없으면.. 
                btnConinue.SetActive(true);
                continueButtonIcon.SetActive(false);
                continueButtonAD.SetActive(true); // 광고 사인 붙여주기 
                
                // 하단에 스타플레이 버튼 보여준다.
                btnStarPlay.SetActive(true);
                
                
            }
            else { // 이어서 플레이 아님. 
            
                // 무료인지 아닌지에 대한 체크. 
                if(episodeData.priceStarPlaySale <= 0) {
                    // 스타플레이 가격이 0이면, 스타플레이 버튼만 노출하고 끝. 
                    btnStarPlay.SetActive(true);
                    return; 
                } 
                
                // 유료 구매이력이 있는 경우. 
                if(episodeData.purchaseState == PurchaseState.Permanent) {
                    btnPlay.SetActive(true);
                    return;
                }
                else {
                    // * 구매 이력이 없음. 광고보기와 스타 플레이 버튼 노출
                    btnAdPlay.SetActive(true);    
                    btnStarPlay.SetActive(true);
                }
            
            }
            
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
                NetworkLoader.main.PurchaseEpisode(episodeData.episodeID, PurchaseState.Permanent, episodeData.currencyStarPlay, "0");
            }
            else {
                PurchasePostProcess(true);
            }
        }
        
        /// <summary>
        /// 1회권 플레이 
        /// </summary>
        void OnClickOneTimePlay() {
            
            // 1회권 사용이 강제되는 상태 
            /*
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
            */
        } // ? 1회 플레이 종료
        
        
        /// <summary>
        /// 스타플레이, 프리미엄 패스 플레이 
        /// </summary>
        public void OnClickPremiumPlay() {
            
            // 프리패스가 없으면 재화 체크하기
            if(!UserManager.main.HasProjectFreepass())
            {
                // 돈없을때 처리 
                if (episodeData.currencyStarPlay == "coin" && !UserManager.main.CheckCoinProperty(episodeData.priceStarPlaySale))
                {
                    SystemManager.ShowSimpleMessagePopUp(SystemManager.GetLocalizedText("80013"));
                    return;
                }

                if (episodeData.currencyStarPlay == "gem" && !UserManager.main.CheckGemProperty(episodeData.priceStarPlaySale))
                {
                    SystemManager.ShowSimpleMessagePopUp(SystemManager.GetLocalizedText("80014"));
                    return;
                }
            }

            // 진행             
            PurchaseEpisode(PurchaseState.Permanent, episodeData.currencyStarPlay, episodeData.priceStarPlaySale);
        }
        
        
        /// <summary>
        /// 잠금 버튼 
        /// </summary>
        public void OnClickLock() {
            if(!UserManager.main.CheckAdminUser())
                return;

            // 어드민 유저만                 
            SystemManager.main.givenEpisodeData = episodeData;
            SystemManager.ShowNetworkLoading();
            StartGame();
        }
        
        /// <summary>
        /// 이어하기 버튼 클릭
        /// </summary>
        public void OnClickContinue() {
            SystemManager.main.givenEpisodeData = episodeData;
            PurchasePostProcess(true);
        }
        
        
        /// <summary>
        /// 광고보고 무료로 하기 플레이 버튼 클릭
        /// </summary>
        public void OnClickAdPlay() {
            PurchaseEpisode(PurchaseState.AD, "none", 0);
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
            
            
            StartGame();
            
            return;
            // ! 추후 작성
            // 에디터 환경에서는 그냥 실행
            if(Application.isEditor) {
                
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
            if (isEpisodeContinuePlay)
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