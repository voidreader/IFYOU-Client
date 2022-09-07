using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TMPro;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    /// <summary>
    /// 엔딩 모음 페이지에서 사용되는 엔딩요소를 이룬다
    /// </summary>
    public class SpecialEpisodeElement : MonoBehaviour
    {
        public ImageRequireDownload episodeBanner;
        public TextMeshProUGUI textTitle;


        public EpisodeData specialEpisode; // 스페셜 에피소드 
        public Image imageInfo; // 하단 인포 
        

        public Sprite spriteUnlock;
        public Sprite spriteLock;
        
        public GameObject groupPrice; // 가격 정보 
        public TextMeshProUGUI textPrice;  
        public GameObject objNew; // 신규 마크 
        public int playPrice = 0; 
        
        public GameObject buttonHint; // 힌트 버튼 

        public void InitSpecialEpisode(EpisodeData epiData)
        {
            specialEpisode = epiData;
            episodeBanner.SetDownloadURL(epiData.popupImageURL, epiData.popupImageKey);
            groupPrice.SetActive(false);
            objNew.SetActive(false);
            buttonHint.SetActive(false);

            if(specialEpisode.isUnlock) { // 잠금해제된 상태 
                imageInfo.sprite = spriteUnlock;
            }
            else { // 잠김 상태
                imageInfo.sprite = spriteLock;
            }

            SystemManager.SetText(textTitle, string.Format("{0}\n<size=22>{1}</size>", epiData.episodeTitle, epiData.episodeSummary));


            // 구매 상태에 따른 가격 표시 추가
            if (specialEpisode.purchaseState == PurchaseState.None) {
                groupPrice.SetActive(true);
                textPrice.text = specialEpisode.priceStarPlaySale.ToString();
                playPrice = specialEpisode.priceStarPlaySale;
                
                if(specialEpisode.isUnlock)
                    objNew.SetActive(true);
            }
            
            // 언락스타일 있으면, 버튼 보여준다.
            if(specialEpisode.unlockStyle != "none") {
                buttonHint.SetActive(true); 
            }
            
            gameObject.SetActive(true);
        }


        #region OnClick Button Event

        /// <summary>
        /// 스페셜 에피소드 플레이
        /// </summary>
        public void OnClickStartSpecialEpisode()
        {
            
            // 잠긴 상태는 리턴. 
            if(!specialEpisode.isUnlock)
                return;          
                
            Debug.Log("## OnClickStartSpecialEpisode");
                
            // 구매되지 않은 경우에만 띄운다. 
            if(specialEpisode.purchaseState != PurchaseState.Permanent) {
                
                Debug.Log("## OnClickStartSpecialEpisode Not Permanent");
                
                
                // 패스 유저는 그냥 0원 구매후 진행 
                if(UserManager.main.HasProjectFreepass()) {
                    UserManager.OnRequestEpisodePurchase = PurchasePostProcess;
                    NetworkLoader.main.PurchaseEpisode(specialEpisode.episodeID, PurchaseState.Permanent, specialEpisode.currencyStarPlay, "0");
                    
                    SystemManager.main.givenEpisodeData = specialEpisode;
                    SystemManager.ShowNetworkLoading(); 
                    return;                    
                }
                
                // 일반 유저 
                // 팝업 오픈 
                PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_SPECIAL_EPISODE_BUY);
                if(p == null) {
                    Debug.LogError("SpecialEpisodeBuy Popup is null");
                    return;
                }
                
                p.Data.contentEpisode = specialEpisode; // 현재 에피소드 설정 
                p.Data.positiveButtonCallback = StartSpecialEpisode; // 콜백 설정. 
                PopupManager.main.ShowPopup(p, false, false);
                
                // Debug.Log("## OnClickStartSpecialEpisode Show Popup");
                
                return;
                
            } // 끝
            
            
            
            
            
            // 구매된 경우는 플레이 처리 
            SystemManager.main.givenEpisodeData = specialEpisode;
            SystemManager.ShowNetworkLoading(); 
            StartSpecialEpisode();
  
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void OnClickHint() {
            PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_SPECIAL_HINT);
            p.Data.contentEpisode = specialEpisode;
            PopupManager.main.ShowPopup(p, true);
        }
        
        

        /// <summary>
        /// 구매 후 처리 
        /// </summary>
        /// <param name="__isPurchaseSuccess"></param>
        void PurchasePostProcess(bool __isPurchaseSuccess) {
            if (!__isPurchaseSuccess)
            {
                Debug.LogError("Error in purchase");
                SystemManager.HideNetworkLoading();
                return;
            }
            
            StartSpecialEpisode();
        }        
        
        /// <summary>
        /// 스페셜 에피소드 플레이 
        /// </summary>
        void StartSpecialEpisode() {
            SystemManager.main.givenEpisodeData = specialEpisode;
            SystemManager.ShowNetworkLoading(); 
            
            
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_GAME_BEGIN, string.Empty);
            //IntermissionManager.isMovingLobby = false; // 게임으로 진입하도록 요청
            //
            //if(GameManager.main != null) 
            //    SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
            //else 
            //    SceneManager.LoadSceneAsync(CommonConst.SCENE_GAME, LoadSceneMode.Single).allowSceneActivation = true;
            
            
            GameManager.SetNewGame();
            // 통신 
            NetworkLoader.main.UpdateUserProjectCurrent(specialEpisode.episodeID, null, 0, false, "StartSpecialEpisode");
            
        }

        #endregion
    }
}
