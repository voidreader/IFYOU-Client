using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

namespace PIERStory {
    public class NewStoryElement : MonoBehaviour
    {
        [SerializeField] Image colorShadow; // 컬러 그림자
        [SerializeField] ImageRequireDownload bannerImage; // 배너 이미지 
        [SerializeField] TextMeshProUGUI textTitle; // 타이틀 
        
        [SerializeField] bool isLock = false;

        [SerializeField] string colorCode = string.Empty;
        [SerializeField] Color mainColor;
        
        StoryData storyData = null; // 작품 정보
        
        
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="__j"></param>
        public NewStoryElement(StoryData __j) {
            InitStoryElement(__j);
        }
        
        /// <summary>
        /// 초기화 하기. 
        /// </summary>
        /// <param name="__j"></param>
        public void InitStoryElement(StoryData __j) {
            
            this.gameObject.SetActive(true);
            
            storyData = __j;
            
            textTitle.text = storyData.title;
            
            colorCode = storyData.colorCode;
            ColorUtility.TryParseHtmlString("#" + colorCode, out mainColor);
            
            // 메인 컬러 처리 
            colorShadow.color = mainColor;
            
            // 배너 이미지 처리 
            bannerImage.SetDownloadURL(storyData.bannerURL, storyData.bannerKey);
        }
        
        public void OnClickElement() {
            if(isLock) {
                SystemManager.ShowMessageWithLocalize("6061", true);
                return;
            }
            
            // 스토리매니저에게 작품 상세정보 요청 
            StoryManager.main.RequestStoryInfo(storyData);
            
        }
    }
}