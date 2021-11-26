using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;


namespace PIERStory {
    public class ViewEpisodeStart : CommonView
    {
        JsonData episodeData; // 에피소드 정보
        JsonData purchaseData; // 에피소드 구매 정보 
        
        [SerializeField] ImageRequireDownload popupImage; // 이미지 
        [SerializeField] TextMeshProUGUI textEpisodeTitle;
        [SerializeField] TextMeshProUGUI textEpisodeSummary;
        
        
        [Space]
        [Header("== 버튼 ==")]
        [SerializeField] GameObject btnPlay; // 플레이 
        [SerializeField] GameObject btnConinue; // 이어서 플레이
        [SerializeField] GameObject btnPremium; // 프리미엄
        [SerializeField] GameObject btnOneTime; // 1회 플레이
        
        
        
        
        
        public override void OnView()
        {
            base.OnView();
        }
        
        public override void OnStartView() {
            base.OnStartView();
        }
    }
}