using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DanielLochner.Assets.SimpleScrollSnap;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using BestHTTP;
using System.Linq;


namespace PIERStory {

    public class IFYouLobby : MonoBehaviour
    {
        public static Action<string> OnCategoryList = null;
        
        JsonData promotionList = null;
        
        public GameObject mainTab; // 메인 탭 
        public GameObject categoryTab; // 카테고리 탭
        
        
        public GameObject readyArea; // 튜토리얼 0단계 적용 후 
        public GameObject allArea; // 임시 구역 
        
        
        
        
        [Header("메인 스토리 리스트")]
        public List<LobbyStoryElement> ListMainSmallStory; // 레디 등장하기 전의 모든 스토리 
        public List<LobbyStoryElement> ListReadyTwoSmallStory; // 레디에 등장하는 2개의 작은 스토리 
        
        public StoryData latestPlayStory = null; // 마지막 플레이한 스토리 
        public ImageRequireDownload latestStoryBanner;
        
        
    
        [Header("프로모션")]    
        public SimpleScrollSnap promotionScroll;
        public Transform promotionContent;
        public GameObject promotionPrefab;
        public Transform promotionPagenation;
        public GameObject pageTogglePrefab;
        
        [Space]
        [Header("카테고리")]
        public List<GenreToggle> ListCategoryToggle; // 토글들
        public List<LobbyStoryElement> ListCategoryStory; // 카테고리에 생성된 스토리 개체 
        public GameObject prefabCategoryStoryElement; // 프리팹 
        public Transform categoryParent; 
        
        
        void Start() {
            OnCategoryList = RequestFilteredStory;
        }
        
        /// <summary>
        /// 초기화..
        /// </summary>
        public void InitLobby() {
            
            Debug.Log("IFYouLobby ### InitLobby");
            
            mainTab.SetActive(true);
            categoryTab.SetActive(false);
            
            readyArea.SetActive(false);
            allArea.SetActive(true);
            
            InitPromotionList();
            InitMainSmallStory();
            
            InitCategory(); // 상단 장르 카테고리 설정 
            
        }
        
        #region 카테고리 
        
        /// <summary>
        /// 상단 카테고리 설정 
        /// </summary>
        void InitCategory() {
            
            //
            for(int i=0; i<ListCategoryToggle.Count;i++) {
                ListCategoryToggle[i].gameObject.SetActive(false);
            }
            
            if(SystemManager.main.storyGenreData == null)
                return;
            
            for(int i=0; i<SystemManager.main.storyGenreData.Count;i++) {
                
                if(ListCategoryToggle.Count <= i)  {
                    Debug.LogError("Too many genre data");
                    break;
                }
                
                ListCategoryToggle[i].SetGenre(SystemManager.main.storyGenreData[i]);
            }            
            
        }

        
        
        /// <summary>
        /// 필터링 된 스토리 리스트 
        /// </summary>
        /// <param name="__genre"></param>
        void RequestFilteredStory(string __genre) {
            // 기존에 생성된 게임오브젝트 제거 후 클리어             
            for(int i=0; i<ListCategoryStory.Count;i++) {
                Destroy(ListCategoryStory[i].gameObject);
            }
            ListCategoryStory.Clear();
            
            
            // 조건에 맞는 작품 검색 
            List<StoryData> filteredList = null;
            
            if(__genre == "Main" || __genre == SystemManager.GetLocalizedText("5131")) {
                // filteredList = StoryManager.main.listTotalStory;
                // 메인 탭으로 이동 
                categoryTab.SetActive(false);
                mainTab.SetActive(true);
                return;
                
            }
            else {
                filteredList = GetGenreFilteredStoryList(__genre);
                
                categoryTab.SetActive(true);
                mainTab.SetActive(false);
                
                if(filteredList == null) {
                    return;
                }
                
                Debug.Log("CallCategory Filter Count: " + filteredList.Count);
                for(int i=0; i<filteredList.Count; i++) {
                    LobbyStoryElement ns = Instantiate(prefabCategoryStoryElement, Vector3.zero, Quaternion.identity).GetComponent<LobbyStoryElement>();
                    ns.transform.SetParent(categoryParent);
                    ns.transform.localScale = Vector3.one;
            
                    
                    ns.Init(filteredList[i], StoryElementType.category);
                    ListCategoryStory.Add(ns); // 리스트에 추가 
                
                }                     
            }
            
       
        }
        
        /// <summary>
        /// 장르로 필터 걸어서 리스트 가져오기 
        /// </summary>
        /// <param name="__genre"></param>
        /// <returns></returns>
        List<StoryData> GetGenreFilteredStoryList(string __genre) {
            return StoryManager.main.listTotalStory.Where( item => item.genre.Contains(__genre)).ToList<StoryData>();
        }
        
        
        
        #endregion
        
        
        
        /// <summary>
        /// 프로모션 초기화
        /// </summary>
        public void InitPromotionList() {
            promotionList = SystemManager.main.promotionData;
            
            if(promotionList == null)
                return;
            
            // 이미 생성된거 있으면 다시 생성.. 
            if(promotionScroll.NumberOfPanels > 0) {
                /*
                Toggle[] toggles = promotionScroll.pagination.GetComponentsInChildren<Toggle>();
                
                for(int i=0; i<promotionScroll.NumberOfPanels; i++) {
                    Destroy(toggles[i]);
                    promotionScroll.RemoveFromBack();
                }
                
                promotionScroll.Setup();
                */
                return;
            }
            
            
            
            for(int i=0; i<promotionList.Count;i++) {
                
                // 생성 
                IFYouPromotionElement promotion = Instantiate(promotionPrefab, promotionContent).GetComponent<IFYouPromotionElement>();
                promotion.SetPromotion(promotionList[i], promotionList[i]["detail"]); // 초기화 
                
                Instantiate(pageTogglePrefab, promotionPagenation); // 페이지네이션 관련 처리 
            }
            
            promotionScroll.Setup();
        }
        
        /// <summary>
        /// 메인 화면의 작은 스토리 네모들 초기화 하기 
        /// </summary>
        public void InitMainSmallStory() {
            ResetMainSmallStory();
           
            
            // 최근에 플레이한 작품 있음 
            if(StoryManager.main.latestPlayProjectID > 0) {
                readyArea.SetActive(true);
               
               // readyArea에 대한 처리 
               // 최근 프로젝트 설정 
               latestPlayStory = StoryManager.main.FindProject(StoryManager.main.latestPlayProjectID.ToString()); 
               latestStoryBanner.SetDownloadURL(latestPlayStory.thumbnailURL, latestPlayStory.thumbnailKey);
              
                for(int i=0; i<2; i++) {
                    if(i >= StoryManager.main.ListRecommendStoryID.Count)
                        break;
                        
                    ListReadyTwoSmallStory[i].Init(StoryManager.main.FindProject(StoryManager.main.ListRecommendStoryID[i]), StoryElementType.general);
                }
            }
            else { // 최근에 플레이한 작품 없음 
            
                // 모든 작품이 보이게 처리 
                allArea.SetActive(true);
                
            
                int  index = 0;
                
                for(int i=0;i<StoryManager.main.listTotalStory.Count;i++) {
                    
                    if(index >= ListMainSmallStory.Count)
                        return;
                    
                    ListMainSmallStory[index].Init(StoryManager.main.listTotalStory[i], StoryElementType.general);
                    index++;
                }                
            }
            
        }
        
        
        /// <summary>
        /// 메인의 스몰 작품 리스트 리셋 
        /// </summary>
        void ResetMainSmallStory() {
            
            readyArea.SetActive(false);
            allArea.SetActive(false);   
            
            // All 영역에 있는 모든 작품들. 
            for(int i=0; i<ListMainSmallStory.Count; i++) {
                ListMainSmallStory[i].gameObject.SetActive(false);
            }
            
            
            // Ready 영역에 있는 오른쪽의 작은 2개 
            for(int i=0; i<ListReadyTwoSmallStory.Count; i++) {
                ListReadyTwoSmallStory[i].gameObject.SetActive(false);
            }
        }
        
        
        /// <summary>
        /// 가장 최근에 플레이 작품 Ready 버튼 클릭 
        /// </summary>
        public void OnClickReady() {
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE, latestPlayStory);
        }
        
        
        
    }
}