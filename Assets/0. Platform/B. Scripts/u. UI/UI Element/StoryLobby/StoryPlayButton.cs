using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Reactor;


namespace PIERStory {
    
    public enum StatePlayButton {
        inactive,
        active,
        premium,
        
        End,
        Serial
    }
    
    public class StoryPlayButton : MonoBehaviour
    {
        public int openPrice = 0;
        
        public TextMeshProUGUI textPlay;
        // public TextMeshProUGUI textPrice; // 기다리면 무료에서의 코인 가격 
        
        public Image premiumAura;
        public Image foregroundProgressor;
        public Image backgroundProgressor;
        public Image icon; // 아이콘 
        
        
        
        public StatePlayButton stateButton = StatePlayButton.active;
        public EpisodeData currentEpisode;
        bool isResumePlay = false;
        
        public bool hasPremium = false;
        
        public Progressor progressor;
        
        public GameObject groupPlay; // 활성 상태일때의 그룹 
        public GameObject groupOpen; // 비활성화 상태일때 가격 그룹
        public GameObject groupReset; // 스토리 종료 상태일때의 그룹 
        
        
        [Space]
        [Header("Sprites")]
        public Sprite spriteActiveBackgroundProgressor; // 프로그레서 백그라운드
        public Sprite spritePremiumBackgroundProgressor;
        public Sprite spriteActiveForegroundProgressor; // 프로그레서 포어그라운드
        public Sprite spritePremiumForegroundProgressor;
        
        public Sprite spriteActiveIcon;
        public Sprite spritePremuimIcon;
        public Sprite spriteCoinIcon;
        public Sprite spriteInactiveBorder;
        

        
        
        /// <summary>
        /// 이어하기 플레이 
        /// </summary>
        /// <param name="__state"></param>
        /// <param name="__progressorValue"></param>
        /// <param name="__resumePlay"></param>
        public void SetPlayButton(StatePlayButton __state, float __progressorValue, bool __resumePlay) {
            
            if(__state != StatePlayButton.Serial) {
                this.gameObject.SetActive(true);
            }
            
            stateButton = __state;
            isResumePlay = __resumePlay;
            
            SetButtonState();
            
            
            // 씬 진행도.
            progressor.SetProgressAt(__progressorValue);
        }
        

        
        /// <summary>
        /// 상태에 따라 컨트롤 처리 
        /// </summary>
        void SetButtonState() {
            
            foregroundProgressor.gameObject.SetActive(true);
            premiumAura.gameObject.SetActive(false);
            
            SystemManager.SetText(textPlay, "PLAY");
            
            // 그룹으로 나눴다..!
            groupOpen.SetActive(false);
            groupPlay.SetActive(false);
            groupReset.SetActive(false);
            
            switch(stateButton) {
                case StatePlayButton.inactive:
               
                backgroundProgressor.sprite = spriteInactiveBorder; // 여긴 게이지 안씀 
                foregroundProgressor.gameObject.SetActive(false);
                
                groupOpen.SetActive(true);
                // textPrice.text = string.Empty;
                
                
                break;
                
                case StatePlayButton.End:
                backgroundProgressor.sprite = spriteInactiveBorder; // 게이지 안씀 
                foregroundProgressor.gameObject.SetActive(false);
                
                // 로비에서 사용할때랑 에피소드 종료에서 사용할때랑 다름 
                if (StoryLobbyManager.main != null) {
                    groupReset.SetActive(true);
                }
                else {
                    groupPlay.SetActive(true);
                    SystemManager.SetText(textPlay, SystemManager.GetLocalizedText("5190"));
                }
                
                
                
                // textPrice.text = string.Empty;
                
                break;
                
                
                case StatePlayButton.active: // 활성 상태 
                
                groupPlay.SetActive(true);
                
                SystemManager.SetText(textPlay, isResumePlay?SystemManager.GetLocalizedText("5005"):SystemManager.GetLocalizedText("5169"));

                icon.sprite = spriteActiveIcon;
                backgroundProgressor.sprite = spriteActiveBackgroundProgressor;
                foregroundProgressor.sprite = spriteActiveForegroundProgressor;
                
                break;
                
                case StatePlayButton.premium: // 프리미엄 패스 유저 
                
                groupPlay.SetActive(true);
                
                SystemManager.SetText(textPlay, isResumePlay?SystemManager.GetLocalizedText("5005"):SystemManager.GetLocalizedText("5169"));
                
                icon.sprite = spritePremuimIcon;
                backgroundProgressor.sprite = spritePremiumBackgroundProgressor;
                foregroundProgressor.sprite = spritePremiumForegroundProgressor;
                premiumAura.gameObject.SetActive(true);
                
                break;
            }

            textPlay.rectTransform.anchoredPosition = isResumePlay ? new Vector2(-40, 4) : new Vector2(-50, 4);
            icon.rectTransform.anchoredPosition = isResumePlay ? new Vector2(115, 0) : new Vector2(105, 0);

            icon.SetNativeSize();
        }
    }
}