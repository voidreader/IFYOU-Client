using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace PIERStory {
    
    
    // * 매번 JSON에서 파싱해서 꺼내오는게 너무 귀찮아서 만들었다! 
    
    [Serializable]
    public class EpisodeData
    {
        JsonData episodeJSON; // 에피소드 정보 
        
        JsonData purchaseData; // 에피소드 구매 정보
        
        public string episodeID = string.Empty; // 에피소드 ID 
        public string episodeNO = string.Empty; // 에피소드 순번
        public string episodeTypeString = string.Empty; // 에피소드 타입
        
        public EpisodeType episodeType = EpisodeType.Chapter; // 에피소드 타입 enum
        
        public string episodeTitle = string.Empty; // 에피소드 타이틀
        public string episodeSummary = string.Empty; // 에피소드 스토리 요약 
        
        public string combinedEpisodeTitle = string.Empty; // 에피소드 순번과 타이틀의 조합 
        public string storyLobbyTitle = string.Empty; // 스토리 로비에서 사용하는 타이틀과 순번 조합 
        public string flowPrefix = string.Empty; // 플로우맵에서 사용되는 에피소드 제목 Prefix
        
        public string endingType = string.Empty;  // 엔딩 타입 
        public string dependEpisode = string.Empty;  // 의존 에피소드 
        public bool endingOpen = false; // 엔딩 오픈 여부 
        public float totalSceneCount = 0f;         // 진행률(분모)
        public float playedSceneCount = 0f;        // 플레이어 진행률(분자)
        public float sceneProgressorValue = 0; // 씬 프로그레서 값 
        
        public float episodeGalleryImageProgressValue = 0; // 일러스트 프로그레서 값 
        
        public string squareImageURL = string.Empty; // 목록 사각 썸네일 
        public string squareImageKey = string.Empty; 
        
        public string popupImageURL = string.Empty; // 팝업 이미지 
        public string popupImageKey = string.Empty; 
        
        public EpisodeState episodeState = EpisodeState.Future; // 에피소드 플레이 상태 
        public PurchaseState purchaseState = PurchaseState.None; // 구매 상태
        
        public bool OneTimePlayable = false; // 1회 플레이 가능여부  
        
        public int priceStarPlaySale = 0; // 스타플레이 세일 가격
        public int priceStarPlay = 0; // 스타플레이 가격
        
        public string currencyOneTime = string.Empty; // 1회 플레이 화폐
        public string currencyStarPlay =  string.Empty; // 스타플레이 화폐 
        public int priceOneTime = 0; // 1회 플레이 가격
        
        public bool isUnlock = true; // 언락 여부 
        
        /// <summary>
        /// 유효한 데이터인지? 
        /// </summary>
        /// <value></value>
        public bool isValidData {
            get {
                return !string.IsNullOrEmpty(episodeID);
            }
        }
        
        
        public EpisodeData(JsonData __j) {
            episodeJSON = __j;
            
            InitData();
        }
        
        public void SetEpisodeData(JsonData __j) {
            episodeJSON = __j;
            InitData();
        }
        
        /// <summary>
        /// 데이터 설정하기
        /// </summary>
        void InitData() {
            episodeID = SystemManager.GetJsonNodeString(episodeJSON, "episode_id");
            episodeNO = SystemManager.GetJsonNodeString(episodeJSON, "chapter_number");
            episodeTypeString = SystemManager.GetJsonNodeString(episodeJSON, "episode_type");
           
            episodeTitle = SystemManager.GetJsonNodeString(episodeJSON, "title");
            episodeSummary = SystemManager.GetJsonNodeString(episodeJSON, "summary");

            endingOpen = SystemManager.GetJsonNodeBool(episodeJSON, "ending_open");
            endingType = SystemManager.GetJsonNodeString(episodeJSON, "ending_type");
            dependEpisode = SystemManager.GetJsonNodeString(episodeJSON, "depend_episode");
            

            
            // 에피소드에 등장한는 갤러리 이미지
            
            episodeGalleryImageProgressValue = UserManager.main.CalcEpisodeGalleryProgress(episodeID);
            
            // * 이미지 
            squareImageURL = SystemManager.GetJsonNodeString(episodeJSON, LobbyConst.TITLE_IMAGE_URL);
            squareImageKey = SystemManager.GetJsonNodeString(episodeJSON, LobbyConst.TITLE_IMAGE_KEY);
            popupImageURL = SystemManager.GetJsonNodeString(episodeJSON, LobbyConst.POPUP_IMAGE_URL);
            popupImageKey = SystemManager.GetJsonNodeString(episodeJSON, LobbyConst.POPUP_IMAGE_KEY);
            
            
            // * 가격
            priceStarPlaySale = int.Parse(SystemManager.GetJsonNodeString(episodeJSON, LobbyConst.EPISODE_SALE_PRICE));
            priceStarPlay = int.Parse(SystemManager.GetJsonNodeString(episodeJSON, LobbyConst.EPISODE_PRICE));
            currencyStarPlay = SystemManager.GetJsonNodeString(episodeJSON, "currency");
            currencyOneTime = SystemManager.GetJsonNodeString(episodeJSON, "one_currency");
            priceOneTime = int.Parse(SystemManager.GetJsonNodeString(episodeJSON, "one_price"));
            
            
            // * 사건 진행율 
            totalSceneCount = int.Parse(episodeJSON["total_scene_count"].ToString()); // 에피소드에 등장하는 모든 사건ID 카운트
            playedSceneCount = int.Parse(episodeJSON["played_scene_count"].ToString()); // 유저가 한번이라도 플레이 했던 사건 ID 카운트 
             if(totalSceneCount > 0)        
                sceneProgressorValue = playedSceneCount / totalSceneCount;
                
            
                        
            switch(episodeTypeString) {
                case "chapter":
                episodeType = EpisodeType.Chapter;
                combinedEpisodeTitle = string.Format(SystemManager.GetLocalizedText("6090"),  episodeNO) + episodeTitle;
                storyLobbyTitle = string.Format("EP {0}. ", episodeNO) + episodeTitle;
                flowPrefix = "EP" + string.Format("{0:D2}", episodeNO);
                break;
                
                case "ending":
                episodeType = EpisodeType.Ending;
                combinedEpisodeTitle = "Ending. " + episodeTitle;
                storyLobbyTitle = combinedEpisodeTitle;
                
                flowPrefix = "Ending";
                
                break;
                
                case "side":
                episodeType = EpisodeType.Side;
                combinedEpisodeTitle = "Special. " + episodeTitle;
                storyLobbyTitle = combinedEpisodeTitle;
                break;
            }
            
            if(episodeType == EpisodeType.Side) {
                isUnlock = SystemManager.GetJsonNodeBool(episodeJSON, "is_open");
            }
            
                
            SetEpisodePlayState();
            SetPurchaseState();
        }
        
        /// <summary>
        /// 새로운 sceneProgressorValue 
        /// </summary>
        /// <param name="__newCount"></param>
        public void SetNewPlayedSceneCount(float __newCount) {
            playedSceneCount = __newCount;
            if(totalSceneCount > 0)        
                sceneProgressorValue = playedSceneCount / totalSceneCount;
        }
        
        /// <summary>
        /// 갤러리 이미지 프로그래서 밸류 리프레시 
        /// </summary>
        public void RefreshGalleryProgressValue() {
            episodeGalleryImageProgressValue = UserManager.main.CalcEpisodeGalleryProgress(episodeID); 
            
            Debug.Log(">> RefreshGalleryProgressValue : " + episodeGalleryImageProgressValue);
        }
        
        /// <summary>
        /// 에피소드의 플레이 상태 설정 
        /// </summary>
        public void SetEpisodePlayState() {
            string currentRegularEpisodeID = string.Empty;
            
            if(episodeType == EpisodeType.Side) {
                episodeState = EpisodeState.Current;
                return;
            }
            
            
            // 아직 작업중인 작품은 값이 없을 수 있다.
            if(UserManager.main.GetUserProjectRegularEpisodeCurrent() == null) {
                currentRegularEpisodeID = string.Empty;
            }
            else {
                currentRegularEpisodeID = UserManager.main.GetUserProjectRegularEpisodeCurrent()["episode_id"].ToString();
            }
            
            
            // current의 ID와 현재 에피소드 ID가 같음. 
            if(currentRegularEpisodeID == episodeID) {
                episodeState = EpisodeState.Current;
            }
            else { // 다른 경우에 과거, 미래 체크 
            
                // episode Progress 테이블에 있음 
                if (UserManager.main.CheckEpisodeProgress(episodeID)) {
                    episodeState = EpisodeState.Prev; // 과거 
                }    
                else { // 없으면 미래.
                    episodeState = EpisodeState.Future; // 미래 
                }
            }
        }
        
        /// <summary>
        /// 에피소드의 구매 상태 설정 
        /// </summary>        
        public void SetPurchaseState() {
            string episodePurchaseState = string.Empty;
            
            // 구매내역을 purchaseDate로 out 
            // * purchaseState 정한다. 
            if (UserManager.main.CheckPurchaseEpisode(episodeID, ref purchaseData))
            {
                Debug.Log(JsonMapper.ToStringUnicode(purchaseData));
                
                if(purchaseData["purchase_type"].ToString() == "Permanent")
                {
                    purchaseState = PurchaseState.Permanent; // 영구적인 구매 상태 
                }
                else if (purchaseData["purchase_type"].ToString() == "OneTime"){ // OneTime
                    purchaseState = PurchaseState.OneTime;
                    OneTimePlayable = SystemManager.GetJsonNodeBool(purchaseData, "onetime_playable");
                }
                else if (purchaseData["purchase_type"].ToString() == "Rent"){ // 
                    purchaseState = PurchaseState.Rent;
                }
                else if (purchaseData["purchase_type"].ToString() == "AD"){ // 
                    purchaseState = PurchaseState.AD;
                }
            }
            else
            {
                // 구매 내역이 없는 경우에 대한 처리 
                if(priceStarPlaySale < 1)
                    purchaseState = PurchaseState.Free; // 무료!
                else 
                    purchaseState = PurchaseState.None;
            }
        } // ? SetPurchaseState
        
        /// <summary>
        /// 구매기록이 있는지 체크 
        /// </summary>
        /// <returns></returns>
        public bool CheckExistsPurchaseData() {
            return purchaseData != null;
        }
        
    }   

}