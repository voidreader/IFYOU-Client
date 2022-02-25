using System;
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
        public int openPrice = 0;
        
        public TextMeshProUGUI textPlay;
        public TextMeshProUGUI textPrice; // 기다리면 무료에서의 코인 가격 
        
        public Image premiumAura;
        public Image foregroundProgressor;
        public Image backgroundProgressor;
        public Image icon; // 아이콘 
        
        
        
        public StatePlayButton stateButton = StatePlayButton.active;
        public EpisodeData currentEpisode;
        bool isResumePlay = false;
        JsonData projectCurrentJSON = null;
        
        public bool hasPremium = false;
        
        public Progressor progressor;
        
        public GameObject groupPlay; // 활성 상태일때의 그룹 
        public GameObject groupOpen; // 비활성화 상태일때 가격 그룹
        
        
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
                    
            stateButton = __state;
            isResumePlay = __resumePlay;
            
            SetButtonState();
            
            
            // 씬 진행도.
            progressor.SetProgressAt(__progressorValue);
            
        }
        
        public void SetTimeOpenPrice(int __price) {
            openPrice = __price;
            
            if(openPrice < 0)
                openPrice = 0;
            
            textPrice.text = openPrice.ToString();
        }
        
        
        /// <summary>
        /// 상태에 따라 컨트롤 처리 
        /// </summary>
        void SetButtonState() {
            
            foregroundProgressor.gameObject.SetActive(true);
            premiumAura.gameObject.SetActive(false);
            
            // 그룹으로 나눴다..!
            groupOpen.SetActive(false);
            groupPlay.SetActive(false);
            
            switch(stateButton) {
                case StatePlayButton.inactive:
               
                backgroundProgressor.sprite = spriteInactiveBorder; // 여긴 게이지 안씀 
                foregroundProgressor.gameObject.SetActive(false);
                
                groupOpen.SetActive(true);
                textPrice.text = string.Empty;
                
                
                break;
                
                case StatePlayButton.active:
                
                groupPlay.SetActive(true);
                
                textPlay.text = isResumePlay?"CONTINUE":"PLAY";
                
                icon.sprite = spriteActiveIcon;
                backgroundProgressor.sprite = spriteActiveBackgroundProgressor;
                foregroundProgressor.sprite = spriteActiveForegroundProgressor;
                
                break;
                
                case StatePlayButton.premium:
                
                groupPlay.SetActive(true);
                
                textPlay.text = isResumePlay?"CONTINUE":"PLAY";
                
                icon.sprite = spritePremuimIcon;
                backgroundProgressor.sprite = spritePremiumBackgroundProgressor;
                foregroundProgressor.sprite = spritePremiumForegroundProgressor;
                premiumAura.gameObject.SetActive(true);
                
                break;
            }
            
            icon.SetNativeSize();
            
        }
        
        public void OnClickPlayButton() {
            Debug.Log("### OnClickPlayButton ###");
        }
        
        
    }
}