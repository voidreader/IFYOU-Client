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
        
        
        
        
        public override void OnView()
        {
            base.OnView();
        }
        
        public override void OnStartView() {
            base.OnStartView();
        }
    }
}