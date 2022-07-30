using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace PIERStory {
    public class ChallengeCol : MonoBehaviour
    {
        public ChallengeData challengeData;
        public bool isPremium = false;
        
        public int quantity = 0;
        public string currency = string.Empty;
        public bool isReceived = false;
        
        
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
        public void SetChallenge(ChallengeData __data) {
            challengeData = __data;
            Refresh();
        }
        
        /// <summary>
        /// 리프레시
        /// </summary>
        public void Refresh() {
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
                
                // 보상을 받지 않은 경우 챌린지 조건이 충족되었는지 여부로 체크 
                // 클리어했던적이 있는 에피소드.. 기준으로 체크를 해야하는데..? 
            }
            
        }
        
        
    }
}