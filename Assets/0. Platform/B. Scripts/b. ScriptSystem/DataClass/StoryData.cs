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
        
        public string original = string.Empty; // 원작 
        
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
        
        // * 에피소드 종료 이미지 
        public string episodeFinishImageURL = string.Empty; 
        public string episodeFinishImageKey = string.Empty;
        
        //* 프리미엄 패스 및 소개 이미지 
        public string premiumPassURL = string.Empty;
        public string premiumPassKey = string.Empty;
        
        //* 작은 카테고리 이미지
        public string categoryImageURL = string.Empty;
        public string categoryImageKey = string.Empty;

        // 코인샵 배너에 사용되는 이미지(메인 카테고리에서 가로형 타입에 사용될 예정)
        public string coinBannerUrl = string.Empty;
        public string coinBannerKey = string.Empty;

        // 메인 화면의 빠른 플레이 배너 이미지
        public string fastPlayBannerUrl = string.Empty;
        public string fastPlayBannerKey = string.Empty;
        
        public bool isLock = false; // 잠금
        public string colorCode = "000000"; // 메인 칼라 코드 
        
        public float projectProgress = 0;
        public bool isPlaying = false; // 플레이중? 
        
        public string represenativeGenre = string.Empty; // 대표 장르         
        public string genre = string.Empty; // 장르
        public string serialDay = string.Empty; // 연재일 추가 
        
        public bool isSerial = false; // 연재작 체크 
        public List<string> listSerialDays = new List<string>();
        
        
        public int passPrice = 0; // 프리미엄 패스 프라이스 
        public float passDiscount = 0.1f; // 프리이엄 패스 진행도에 따른 할인율 

        public bool isNotify = false;       // 작품 푸쉬 알림 설정
        
        public int hitCount = 0;
        public int likeCount = 0;
        public string[] arrHashtag; // 해시태그 string array
        
        public bool isValidData {
            get {
                return originData != null && !string.IsNullOrEmpty(projectID);
            }
        }
        
        public StoryData() {
            originData = null;
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
            original = SystemManager.GetJsonNodeString(originData, LobbyConst.ORIGINAL); 
            
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
            
            
            // 에피소드 종료 이미지 
            episodeFinishImageURL = SystemManager.GetJsonNodeString(originData, "episode_finish_url");
            episodeFinishImageKey = SystemManager.GetJsonNodeString(originData, "episode_finish_key");
            
            // 프리미엄 패스 이미지
            premiumPassURL = SystemManager.GetJsonNodeString(originData, "premium_pass_url");
            premiumPassKey = SystemManager.GetJsonNodeString(originData, "premium_pass_key");
            
            // 카테고리 이미지
            categoryImageURL = SystemManager.GetJsonNodeString(originData, "category_thumbnail_url");
            categoryImageKey = SystemManager.GetJsonNodeString(originData, "category_thumbnail_key");

            coinBannerUrl = SystemManager.GetJsonNodeString(originData, "coin_banner_url");
            coinBannerKey = SystemManager.GetJsonNodeString(originData, "coin_banner_key");

            fastPlayBannerUrl = SystemManager.GetJsonNodeString(originData, "fastplay_banner_url");
            fastPlayBannerKey = SystemManager.GetJsonNodeString(originData, "fastplay_banner_key");

            // 프리미엄 패스 가격정보
            passPrice = SystemManager.GetJsonNodeInt(originData, "pass_price");
            passDiscount = SystemManager.GetJsonNodeFloat(originData, "pass_discount");

            isNotify = SystemManager.GetJsonNodeBool(originData, "is_notify");

            // 프로젝트 진행율 
            projectProgress = SystemManager.GetJsonNodeFloat(originData, LobbyConst.STORY_PROJECT_PROGRESS);
            
            // 
            isPlaying = SystemManager.GetJsonNodeBool(originData, LobbyConst.STORY_IS_PLAYING); 
            
            // 대표 장르 설정 
            JsonData genreData = originData["genre"];
            if(genreData.Count > 0) {
                represenativeGenre = genreData[0].ToString();
            }
            
            
            // 전체 장르 설정 
            genre = SystemManager.GetJsonNodeString(originData, "genre");
            genre = string.Empty;
            
            for(int i=0; i<originData["genre"].Count;i++) {
                genre += originData["genre"][i].ToString();
                
                // 마지막에 콤마 붙이지 않는다.
                if(i < originData["genre"].Count-1)
                    genre += ",";
            }
            
            hitCount  = SystemManager.GetJsonNodeInt(originData, "hit_count"); // 조회수 카운트 
            likeCount  = SystemManager.GetJsonNodeInt(originData, "like_count"); // 선호작 카운트
            
            arrHashtag = SystemManager.GetJsonNodeString(originData, "hashtags").Split(',');
            
            
            // 연재일 
            serialDay = SystemManager.GetJsonNodeString(originData, "serial_day");
            if(string.IsNullOrEmpty(serialDay) || serialDay == "-1") {
                isSerial = false;
            }
            else {
                isSerial = true;
                listSerialDays.Clear();
                
                // 요일 처리 
                string[] arrSerial = serialDay.Split(',');
                
                // 요일 로컬라이징 
                for(int i=0; i<arrSerial.Length;i++) {
                    switch(arrSerial[i]) {
                        case "0": // 일
                        listSerialDays.Add(SystemManager.GetLocalizedText("5183"));
                        break;
                        case "1": // 월
                        listSerialDays.Add(SystemManager.GetLocalizedText("5177"));
                        break;
                        case "2": // 화
                        listSerialDays.Add(SystemManager.GetLocalizedText("5178"));
                        break;
                        case "3": // 수
                        listSerialDays.Add(SystemManager.GetLocalizedText("5179"));
                        break;
                        case "4": // 목
                        listSerialDays.Add(SystemManager.GetLocalizedText("5180"));
                        break;
                        case "5": // 금
                        listSerialDays.Add(SystemManager.GetLocalizedText("5181"));
                        break;
                        case "6": // 토
                        listSerialDays.Add(SystemManager.GetLocalizedText("5182"));
                        break;
                        
                    }
                }
            }
   
        } // ? END
        
        
        
        /// <summary>
        /// 연재일 정보 가져오기 
        /// </summary>
        /// <returns></returns>
        public string GetSeiralDay() {
            if(!isSerial)
                return string.Empty;
                
            string allSerailDay = string.Empty;
            for(int i=0; i<listSerialDays.Count;i++) {
                allSerailDay += listSerialDays[i] + " ";
            }
            
            return allSerailDay;
        }
    }
}