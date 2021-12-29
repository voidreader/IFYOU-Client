using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {
    public class PopupPass : PopupBase
    {
        [SerializeField] PassBanner passBanner;
        [SerializeField] TextMeshProUGUI textOriginPrice; // 원 가격 
        [SerializeField] TextMeshProUGUI textSalePrice; // 할인 가격
        
        
        public override void Show()
        {
            base.Show();
        }
        
        /// <summary>
        /// 패스배너 초기화 
        /// </summary>
        public void InitPassBanner() {
            passBanner.SetPremiumPass(true);
            
            textOriginPrice.text = passBanner.originFreepassPrice.ToString();
            textSalePrice.text = passBanner.saleFreepassPrice.ToString();
        }
    }
}