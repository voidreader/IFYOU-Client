using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



namespace PIERStory {
    public class PopupFlowReset : PopupBase
    {
        public GameObject groupDoubleButton; // 프리미엄 패스 미소유 유저 버튼 2개
        public GameObject groupSingleButton; // 프리미엄 패스 소유 유저용 단일 버튼 
        
        public int resetPrice = 0;
        public bool hasPremiumPass = false;
        
        [SerializeField] EpisodeData targetEpisode; // 리셋을 해서 돌아갈 에피소드 데이터 
        [SerializeField] TextMeshProUGUI textResetCoinPrice; // 리셋 코인 가격
        [SerializeField] TextMeshProUGUI textResetEpisode; // 리셋 에피소드 안내 
        
        public override void Show()
        {
            base.Show();
            
            targetEpisode = SystemListener.main.resetTargetEpisode; // 시스템 리스너에서 대상 에피소드 보유 중. 
            resetPrice = SystemManager.main.firsetResetPrice; // 리셋 가격 시스탬 매니저에서 가져오기 
            
            hasPremiumPass = UserManager.main.HasProjectFreepass();
            
            // 프리미엄 패스 보유여부에 따라서 버튼 뜨는게 다르다. 
            groupDoubleButton.SetActive(!hasPremiumPass);
            groupSingleButton.SetActive(hasPremiumPass);
            
            // 엔딩, 엔딩 플레이 완에 도달하면 반값으로 처리 
            if(UserManager.main.CheckReachEnding()) {
                resetPrice = Mathf.RoundToInt(resetPrice * 0.5f);

                // 엔딩에서 1화로 돌아가는거면 true로 변환해주고
                if (targetEpisode.episodeNumber == 1)
                    NetworkLoader.main.isFirstEpisode = true;
            }
            
            // 가격
            textResetCoinPrice.text = resetPrice.ToString();
            
            // 스토리 리셋인 경우 true로 전달됨
            if(Data.isPositive)
                return;
            
            // 에피소드 {0}으로 돌아가 새로운 이야기를 시작한다네 
            // FlowReset에서만 사용 
            textResetEpisode.text = string.Format(SystemManager.GetLocalizedText("6219"), targetEpisode.episodeNO);
            
        }

        public override void Hide()
        {
            base.Hide();
        }


        public void OnClickClosePopup()
        {
            NetworkLoader.main.isFirstEpisode = false;
        }


        /// <summary>
        /// 코인주고 리셋 
        /// </summary>
        public void OnClickReset() {
            if(targetEpisode == null || !targetEpisode.isValidData) {
                Debug.LogError("No target data OnClickReset");
                return;
            }
            
            // 잔고 체크
            if(!UserManager.main.CheckCoinProperty(resetPrice)) {
                SystemManager.ShowMessageWithLocalize("80013");
                return;
            }
            
            
            base.Hide();
            
            // 유료 리셋 
            NetworkLoader.main.ResetEpisodeProgress(targetEpisode.episodeID, resetPrice, false);
        }
        
        /// <summary>
        /// 프리미엄 패스 화면 오픈 
        /// </summary>
        public void OnClickPremiumPass() {
            
            // 프리미엄 팝업 오픈 
            PopupBase p = PopupManager.main.GetPopup("PremiumPass");
                
            PopupManager.main.ShowPopup(p, false, false);
            
            
            base.Hide();
        }
        
        
        /// <summary>
        /// 프리미엄 패스 리셋 
        /// </summary>
        public void OnClickPremiumPassReset() {
            
            base.Hide();
            
            // 공짜 리셋 
            NetworkLoader.main.ResetEpisodeProgress(targetEpisode.episodeID, 0, true);
        }
        
    }
}