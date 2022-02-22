using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using Doozy.Runtime.Reactor;


namespace PIERStory {
    
    public enum StatePlayButton {
        inactive,
        active,
        premium
    }
    
    public class StoryPlayButton : MonoBehaviour
    {
        public TextMeshProUGUI textPlay;
        public TextMeshProUGUI textPrice; // 기다리면 무료에서의 코인 가격 
        
        public Image premiumAura;
        public Image foregroundProgressor;
        public Image backgroundProgressor;
        public Image icon; // 아이콘 
        
        
        
        public StatePlayButton stateButton = StatePlayButton.active;
        public EpisodeData currentEpisode;
        JsonData projectCurrentJSON = null;
        
        public bool hasPremium = false;
        
        public Progressor progressor;
        
        
        [Space]
        [Header("Sprites")]
        public Sprite spriteActiveBackgroundProgressor; // 프로그레서 백그라운드
        public Sprite spritePremiumBackgroundProgressor;
        public Sprite spriteActiveForegroundProgressor; // 프로그레서 포어그라운드
        public Sprite spritePremiumForegroundProgressor;
        
        public Sprite spriteActiveIcon;
        public Sprite spritePremuimIcon;
        
        
        
        
        /// <summary>
        /// 플레이 버튼 세팅 i
        /// </summary>
        public void SetPlayButton(EpisodeData __episode, JsonData __projectCurrent, bool __hasPremium) {
            projectCurrentJSON = __projectCurrent;
            currentEpisode = __episode;
            

            
            
            hasPremium = __hasPremium;
            
            
            stateButton = hasPremium?StatePlayButton.premium:StatePlayButton.active;
            
            SetButtonState();
            
            
            // 씬 진행도.
            progressor.SetValueAt(currentEpisode.sceneProgressorValue);
            
        }
        
        
        /// <summary>
        /// 상태에 따라 컨트롤 처리 
        /// </summary>
        void SetButtonState() {
            
            
            switch(stateButton) {
                case StatePlayButton.active:
                icon.sprite = spriteActiveIcon;
                backgroundProgressor.sprite = spriteActiveBackgroundProgressor;
                foregroundProgressor.sprite = spriteActiveForegroundProgressor;
                premiumAura.gameObject.SetActive(false);
                
                break;
                
                case StatePlayButton.premium:
                icon.sprite = spritePremuimIcon;
                backgroundProgressor.sprite = spritePremiumBackgroundProgressor;
                foregroundProgressor.sprite = spritePremiumForegroundProgressor;
                premiumAura.gameObject.SetActive(true);
                
                break;
            }
            
        }
    }
}