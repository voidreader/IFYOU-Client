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
        [SerializeField] string projectID = string.Empty;
        [SerializeField] string imageURL = string.Empty;
        [SerializeField] string imageKey = string.Empty;
        [SerializeField] string colorCode = string.Empty;
        [SerializeField] Color mainColor;
        
        JsonData storyJSON = null; // 작품 정보
        
        /// <summary>
        /// 초기화 하기. 
        /// </summary>
        /// <param name="__j"></param>
        public void InitStoryElement(JsonData __j) {
            storyJSON = __j;
            
            imageURL = storyJSON[LobbyConst.IFYOU_PROJECT_BANNER_URL].ToString();
            imageKey = storyJSON[LobbyConst.IFYOU_PROJECT_BANNER_KEY].ToString();
            
            colorCode = storyJSON[LobbyConst.IFYOU_PROJECT_MAIN_COLOR].ToString();
            ColorUtility.TryParseHtmlString("#" + colorCode, out mainColor);
            
            // 메인 컬러 처리 
            colorShadow.color = mainColor;
            
            // 배너 이미지 처리 
            bannerImage.SetDownloadURL(imageURL, imageKey);
        }
    }
}