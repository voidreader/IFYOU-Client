using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


namespace PIERStory {
    public class ViewIntroduce : CommonView
    {
        
        [SerializeField] ImageRequireDownload mainThumbnail;    // 썸네일 
        
        [SerializeField] TextMeshProUGUI textTitle;             // 타이틀
        [SerializeField] TextMeshProUGUI textAuthor;            // 원작자
        [SerializeField] TextMeshProUGUI textProducer;          // 제작사
        [SerializeField] TextMeshProUGUI textGenre;             // 장르 
        [SerializeField] TextMeshProUGUI textSummary;           // 요약
        
        public StoryData introduceStory;
        
        public override void OnView() {
            base.OnView();
        }
        
        public override void OnStartView() {
            base.OnStartView();
            
            SetInfo();            
            
        }
        
        void SetInfo() {
            
            if(string.IsNullOrEmpty(SystemListener.main.introduceStory.projectID))
                return;
            
            introduceStory = SystemListener.main.introduceStory;
             

            mainThumbnail.SetDownloadURL(introduceStory.thumbnailURL, introduceStory.thumbnailKey); // 썸네일 
            
            
            textTitle.text = introduceStory.title;
            textAuthor.text = SystemManager.GetLocalizedText("6179") + " / " + introduceStory.original; // 원작
            textProducer.text = SystemManager.GetLocalizedText("6180") + " / " + introduceStory.writer;
            textSummary.text = introduceStory.summary; // 요약 
            textGenre.text = SystemManager.GetLocalizedText("6181") + " / " + introduceStory.genre;
        }
        
        
        public void OnClickStart() {
            if(introduceStory.isLock) {
                SystemManager.ShowMessageWithLocalize("6061", true);
                return;
            }
            
            // 스토리매니저에게 작품 상세정보 요청 
            StoryManager.main.RequestStoryInfo(introduceStory);
            
        }
    }
}