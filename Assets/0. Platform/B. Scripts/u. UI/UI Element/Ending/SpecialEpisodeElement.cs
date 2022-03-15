using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Doozy.Runtime.Signals;
using UnityEngine.SceneManagement;

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
        
        
        public GameObject objLock; // OPEN 열쇠 표시 
        
        public Sprite spriteUnlock;
        public Sprite spriteLock;
        

        public void InitSpecialEpisode(EpisodeData epiData)
        {
            specialEpisode = epiData;
            episodeBanner.SetDownloadURL(epiData.popupImageURL, epiData.popupImageKey);


            if(specialEpisode.isUnlock) { // 잠금해제된 상태 
                textTitle.gameObject.SetActive(true);
                textTitle.text = epiData.episodeTitle;
                
                objLock.SetActive(false);
                
                imageInfo.sprite = spriteUnlock;
                
            }
            else { // 잠김 상태
                textTitle.gameObject.SetActive(false);
                objLock.SetActive(true);
                
                imageInfo.sprite = spriteLock;
            }
            gameObject.SetActive(true);
        }


        #region OnClick Button Event

        /// <summary>
        /// 엔딩 플레이!
        /// </summary>
        public void OnClickStartSpecialEpisode()
        {
            SystemManager.main.givenEpisodeData = specialEpisode;
            SystemManager.ShowNetworkLoading(); 
            
            // 0원으로 구매 처리 
            if(specialEpisode.purchaseState != PurchaseState.Permanent) {
                UserManager.OnRequestEpisodePurchase = PurchasePostProcess;
                NetworkLoader.main.PurchaseEpisode(specialEpisode.episodeID, PurchaseState.Permanent, specialEpisode.currencyStarPlay, "0");
            }
            else {
                
                // 이미 구매기록 있다면, 그냥 진행 
                PurchasePostProcess(true);
            }
        }
        
        
        void PurchasePostProcess(bool __isPurchaseSuccess) {
            if (!__isPurchaseSuccess)
            {
                Debug.LogError("Error in purchase");
                SystemManager.HideNetworkLoading();
                return;
            }
            
            specialEpisode.SetPurchaseState();
            StartSpecialEpisode();
            
        }
        
        
        /// <summary>
        /// 스페셜 에피소드 플레이 
        /// </summary>
        void StartSpecialEpisode() {
            
            if(!specialEpisode.isUnlock)
                return;
            
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_GAME_BEGIN, string.Empty);
            IntermissionManager.isMovingLobby = false; // 게임으로 진입하도록 요청
            
            if(GameManager.main != null) 
                SceneManager.LoadSceneAsync(CommonConst.SCENE_INTERMISSION, LoadSceneMode.Single).allowSceneActivation = true;
            else 
                SceneManager.LoadSceneAsync(CommonConst.SCENE_GAME, LoadSceneMode.Single).allowSceneActivation = true;
            
            
            GameManager.SetNewGame();
            // 통신 
            NetworkLoader.main.UpdateUserProjectCurrent(specialEpisode.episodeID, null, 0);
            
            Firebase.Analytics.FirebaseAnalytics.LogEvent("SpecialEpisodeStart", "episode_id", specialEpisode.episodeID);
        }

        #endregion
    }
}
