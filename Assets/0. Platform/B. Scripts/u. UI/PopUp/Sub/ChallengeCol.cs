using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using BestHTTP;

namespace PIERStory {
    public class ChallengeCol : MonoBehaviour
    {
        public ChallengeData challengeData;
        public EpisodeData episodeData;
        
        public bool isPremium = false;
        
        public int quantity = 0;
        public string currency = string.Empty;
        public bool isReceived = false; //  보상 수신 여부
        public bool isRewardable = false;  // 보상을 받을 수 있는 조건에 도달했는지?
        
        
        [Header("Sprites")]
        public Sprite spriteStar;
        public Sprite spriteCoin;
        
        [Space]
        public GameObject lockFrame;
        public Image clearCover;
        public Transform clearCheck; 
        
        public Image currencyIcon;
        public TextMeshProUGUI textCurrencyQuantity;
        
        /// <summary>
        /// 데이터 세팅 
        /// </summary>
        /// <param name="__data"></param>
        public void SetChallenge(ChallengeData __data, EpisodeData __episode, bool __isPremium = false) {
            isPremium = __isPremium;
            
            challengeData = __data;
            episodeData = __episode;
            Refresh();
        }
        
        /// <summary>
        /// 리프레시
        /// </summary>
        public void Refresh() {
            
            isRewardable = false; 
            
            // 종류에 따라 다른 정보 받아오고 
            if(isPremium) {
                currency = challengeData.premiumCurrency;
                quantity = challengeData.premiumQuantity;
                isReceived = challengeData.isPremiumReceived;
            }
            else {
                currency = challengeData.freeCurrency;
                quantity = challengeData.freeQuantity;
                isReceived = challengeData.isFreeReceived;
            }
            
            
            // 아이콘 및 수량 설정 
            if(currency == LobbyConst.GEM) 
                currencyIcon.sprite = spriteStar;
            else if(currency == LobbyConst.COIN)    
                currencyIcon.sprite = spriteCoin;
            else 
                currencyIcon.sprite = null;
                
            textCurrencyQuantity.text = quantity.ToString();
            
            // 보상 수령 여부를 체크 
            if(isReceived) {
                // 받은 경우에 대한 처리
                lockFrame.SetActive(false);
                clearCover.gameObject.SetActive(true);
            }
            else {
                clearCover.gameObject.SetActive(false);
                
                // 아직 보상을 받지 않은 경우에 대한 처리
                if(episodeData.isClear) { // 에피소드 플레이 기록 있음 
                    lockFrame.SetActive(false);
                    
                    // 프리미엄 컬럼은 프리미엄 패스 보유까지 체크한다.
                    if(isPremium ) {
                        
                        isRewardable = UserManager.main.HasProjectPremiumPassOnly(StoryManager.main.CurrentProjectID);
                        
                    }
                    else {
                        isRewardable = true;
                    }
                    
                    
                    
                }
                else { // 플레이 기록 없음 
                    lockFrame.SetActive(true);
                }
                
            }
        } // end of refresh
        
        public void OnClickCol() {
            // 보상 받을 수 없는 상태 
            if(!isRewardable) {
                
                if(isPremium) {
                    
                }
                else {
                    
                }
                
                return;
            }
            
            
            // 실제 보상 수신 처리 
        }
        
        
    } // end of class
}