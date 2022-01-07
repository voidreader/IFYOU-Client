using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace PIERStory {

    public class PopupLevelUp : PopupBase
    {
        
        [SerializeField] TextMeshProUGUI textLevel;
        [SerializeField] TextMeshProUGUI textSpecialGiftName;
        [SerializeField] TextMeshProUGUI textNormalGiftQuantity;
        
        [SerializeField] ImageRequireDownload specialGiftIcon;
        [SerializeField] ImageRequireDownload normalGiftIcon;
        
        [SerializeField] Image aura; // 
        [SerializeField] bool isCompleteLoad = false;
        
        
        
        public override void Show()
        {
            base.Show();
            isCompleteLoad = false;
            textLevel.text = string.Empty;
            textSpecialGiftName.text = string.Empty;
            textNormalGiftQuantity.text = string.Empty;
            
            normalGiftIcon.OnDownloadImage = OnLoadNormalGift;
            specialGiftIcon.OnDownloadImage = OnLoadSpecialGift;
            
            
            aura.DOKill();
            aura.color = new Color(1,1,1,0);
            
            specialGiftIcon.gameObject.SetActive(false);
            normalGiftIcon.gameObject.SetActive(false);
            
        }
        
        public void OnShow() {
            aura.DOFade(1, 1).SetLoops(-1, LoopType.Yoyo);
            PopupManager.main.PlayConfetti();
        }
        
        public void OnStartHide() {
            PopupManager.main.HideConfetti();
        }
        
        
        
        void OnLoadNormalGift() {
            isCompleteLoad = true;
        }
        
        void OnLoadSpecialGift() {
            isCompleteLoad = true;
        }
        

    }
}