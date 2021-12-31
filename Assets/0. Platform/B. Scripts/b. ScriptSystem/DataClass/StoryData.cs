using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace PIERStory {

    /// <summary>
    /// 스토리(프로젝트) 데이터 
    /// </summary>
    [Serializable]
    public class StoryData
    {
        JsonData originData = null;
        
        public string projectID = string.Empty; // 프로젝트 ID 
        public string title = string.Empty; // 타이틀 
        public string writer = string.Empty; // 원작자 
        public string summary = string.Empty; // 요약 
        
        public int sortKey = 0; //  정렬 순서 
        public string bubbleSetID =  string.Empty; // 말풍선 세트 ID 
        
        public bool isCredit = false; // 크레딧 사용 여부        
        
        
        // 이미지 관련         
        public string bannerURL = string.Empty;
        public string bannerKey = string.Empty;
        public string thumbnailURL = string.Empty;
        public string thumbnailKey = string.Empty;
        public string circleImageURL = string.Empty;
        public string circleImageKey = string.Empty;
        
        
        public bool isLock = false; // 잠금
        public string colorCode = "000000"; // 메인 칼라 코드 
        
        public float projectProgress = 0;
        public bool isPlaying = false; // 플레이중? 
        
        public string genre = string.Empty; // 장르
        
        public StoryData() {
            
        }
        
        /// <summary>
        /// 스토리 데이터 생성 
        /// </summary>
        /// <param name="__j"></param>
        public StoryData (JsonData __j) {
            originData = __j;
            
            InitData();
        }
        
        /// <summary>
        /// 데이터 초기화 
        /// </summary>
        void InitData() {
            projectID = SystemManager.GetJsonNodeString(originData, LobbyConst.STORY_ID);
            title = SystemManager.GetJsonNodeString(originData, LobbyConst.STORY_TITLE);
            summary = SystemManager.GetJsonNodeString(originData, LobbyConst.SUMMARY);
            writer = SystemManager.GetJsonNodeString(originData, LobbyConst.WRITER); 
            
            sortKey = SystemManager.GetJsonNodeInt(originData, LobbyConst.SORTKEY); 
            bubbleSetID = SystemManager.GetJsonNodeString(originData, LobbyConst.STORY_BUBBLE_ID); 
            isCredit = SystemManager.GetJsonNodeBool(originData, LobbyConst.IS_CREDIT); 
            isLock = SystemManager.GetJsonNodeBool(originData, LobbyConst.IS_LOCK); 
            colorCode = SystemManager.GetJsonNodeString(originData, LobbyConst.IFYOU_PROJECT_MAIN_COLOR); 
            
            // 이미지 친구1
            bannerURL = SystemManager.GetJsonNodeString(originData, LobbyConst.IFYOU_PROJECT_BANNER_URL);
            bannerKey = SystemManager.GetJsonNodeString(originData, LobbyConst.IFYOU_PROJECT_BANNER_KEY);
            
            // 이미지 친구2
            thumbnailURL = SystemManager.GetJsonNodeString(originData, LobbyConst.IFYOU_PROJECT_THUMBNAIL_URL);
            thumbnailKey = SystemManager.GetJsonNodeString(originData, LobbyConst.IFYOU_PROJECT_THUMBNAIL_KEY);
            
            // 원형 이미지
            circleImageURL = SystemManager.GetJsonNodeString(originData, LobbyConst.IFYOU_PROJECT_CIRCLE_URL);
            circleImageKey = SystemManager.GetJsonNodeString(originData, LobbyConst.IFYOU_PROJECT_CIRCLE_KEY);
            
            // 프로젝트 진행율 
            projectProgress = SystemManager.GetJsonNodeFloat(originData, LobbyConst.STORY_PROJECT_PROGRESS)   ;
            
            // 
            isPlaying = isLock = SystemManager.GetJsonNodeBool(originData, LobbyConst.STORY_IS_PLAYING); 
            
            //
            genre = SystemManager.GetJsonNodeString(originData, "genre");
            genre = string.Empty;
            
            for(int i=0; i<originData["genre"].Count;i++) {
                genre += originData["genre"][i].ToString();
                genre += ",";
            }
            
        }
    }
}