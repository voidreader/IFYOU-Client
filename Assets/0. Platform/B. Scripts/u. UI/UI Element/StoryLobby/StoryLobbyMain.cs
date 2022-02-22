using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LitJson;


namespace PIERStory {

    public class StoryLobbyMain : MonoBehaviour
    {
        
        public StoryData currentStoryData;
        JsonData projectCurrentJSON = null;
        public string currentEpisodeID = string.Empty; // 현재 순번의 에피소드 ID 
        public EpisodeData currentEpisodeData; // 현재 순번의 에피소드 데이터 
        public bool hasPremium = false; // 프리미엄 패스 보유 여부 
        
        
        
        
        [Space]
        [Header("Controls")]
        // 스토리 플레이 버튼
        public StoryPlayButton storyPlayButton;
        
        public RectTransform groupStoryContents; // 스토리 컨텐츠 그룹 
        
        public Image imageEpisodeTitle; // 에피소드 타이틀 배경 
        public TextMeshProUGUI textEpisodeTitle; // 에피소드 타이틀 
        
        [Space]
        [Header("Sprites")]
        public Sprite spriteActiveEpisodeTitleBG;
        public Sprite spriteInactiveEpisodeTitleBG;
        
        
        
        /// <summary>
        /// 스토리 로비 
        /// </summary>
        public void InitStoryLobbyControls() {
            currentStoryData =  StoryManager.main.CurrentProject; // 현재 작품 
            projectCurrentJSON = UserManager.main.GetUserProjectRegularEpisodeCurrent(); // 작품상에서 현재 위치 
            
            if(projectCurrentJSON == null) {
                SystemManager.ShowSimpleAlert("Invalid Episode State. Please contact to help center");
                return;
            }
            
            currentEpisodeID = SystemManager.GetJsonNodeString(projectCurrentJSON, "episode_id");
            currentEpisodeData = StoryManager.GetRegularEpisodeByID(currentEpisodeID);
            hasPremium = UserManager.main.HasProjectFreepass();
            
            
            // 스토리 플레이버튼 초기화 
            storyPlayButton.SetPlayButton(currentEpisodeData, projectCurrentJSON, hasPremium);
            
            // 컨텐츠 그룹 초기화 
            InitStoryContensGroup();
            
            
            // 에피소드 타이틀 
            InitEpisodeTitle(); 
        }
        
        
        /// <summary>
        /// 타이틀 정보 처리 
        /// </summary>
        void InitEpisodeTitle() {
            
            // TODO. active inactive 구분 필요 
            
            
            imageEpisodeTitle.sprite = spriteActiveEpisodeTitleBG;
            textEpisodeTitle.color = Color.black;
            textEpisodeTitle.text = currentEpisodeData.storyLobbyTitle;
        }
        
        void InitStoryContensGroup() {
            
            // TODO 컨트롤 초기화 
            /*
            groupStoryContents.DOKill();
            groupStoryContents.anchoredPosition = new Vector2(-355, 0);
            */
            
            // TODO 그룹... 
        }
        
    }
}