using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Components;


namespace PIERStory {
    public class MainLibrary : MonoBehaviour
    {
        
        public static Action<string> OnCategoryList = null;
        public static string SELECTED_GENRE = string.Empty; // 메인화면에서 선택된 장르 
        
        
        // Start is called before the first frame update
        // public List<GenreToggle> ListCategoryToggle; // 토글들
        // public List<LobbyStoryElement> ListCategoryStory; // 카테고리에 생성된 스토리 개체 
        public GameObject prefabCategoryStoryElement; // 프리팹 
        public Transform categoryParent; 
    
        public UIToggle firstLeftToggle; // 왼쪽 첫번째 토글(진행중 토글)
        public string currentGenre = "All"; // 현재 선택된 장르
        public bool isPlayingSelected = true; //
        
        // 아무것도 없을때 표시되는 친구들 
        public GameObject NoLikeIcon;
        public GameObject NoPlayingIcon;
        
        
        public List<CustomGenreCheckBox> listGenreCheckBox;
        
        
        void Start() {
            OnCategoryList = RequestFilteredStory;
        }
        
        
        /// <summary>
        /// 라이브러리 초기화 
        /// </summary>
        public void InitLibrary() {
            for(int i=0; i<listGenreCheckBox.Count;i++) {
                listGenreCheckBox[i].gameObject.SetActive(false);
            }
            
            if(SystemManager.main.storyGenreData == null) {
                return;
            }
            
            for(int i=0; i<SystemManager.main.storyGenreData.Count;i++) {
               // 장르 세팅  
            }
            
            // 세팅하고 체크박스 설정한다. 
        }

        
        List<StoryData> GetGenreFilteredStoryList(string __genre) {
            return StoryManager.main.listTotalStory.Where( item => item.genre.Contains(__genre)).ToList<StoryData>();
        }        
        
        /// <summary>
        /// 인위적으로 왼쪽 토클 클릭시 처리 
        /// </summary>
        public void OnClickLeftToggle() {
            
            isPlayingSelected = firstLeftToggle.isOn;
            RequestFilteredStory(currentGenre);
        }
        
        
        /// <summary>
        /// 필터링 된 스토리 리스트 
        /// </summary>
        /// <param name="__genre"></param>
        void RequestFilteredStory(string __genre) {
            
            /*
            currentGenre = __genre;
            
            // 기존에 생성된 게임오브젝트 제거 후 클리어             
            for(int i=0; i<ListCategoryStory.Count;i++) {
                Destroy(ListCategoryStory[i].gameObject);
            }
            ListCategoryStory.Clear();
            
            
            // * 클리어까지는 동일하고, 왼쪽 토글에 따라서
            // * 진행중인 작품과 관심작품으로 분류를 한다. 
            
            
            // 조건에 맞는 작품 검색 
            List<StoryData> filteredList = null;
            
            if(__genre == "All" || __genre == SystemManager.GetLocalizedText("5137")) {
                filteredList = StoryManager.main.listTotalStory;
            }
            else {
                filteredList = GetGenreFilteredStoryList(__genre);

            }
         
                
            if(filteredList == null) {
                return;
            }
            
            Debug.Log("Library #1 Filter Count: " + filteredList.Count);
            NoPlayingIcon.SetActive(false);
            NoLikeIcon.SetActive(false);
            
            
            if(firstLeftToggle.isOn) {
                // return StoryManager.main.listTotalStory.Where( item => item.genre.Contains(__genre)).ToList<StoryData>();
                filteredList = filteredList.Where(item => item.projectProgress > 0).ToList<StoryData>(); // 진행중인 작품만 필터링 
                
                NoPlayingIcon.SetActive(filteredList.Count == 0);
                
            }
            else {
                filteredList = filteredList.Where(item => StoryManager.main.CheckProjectLike(item.projectID)).ToList<StoryData>(); // 관심작품만. 
                
                NoLikeIcon.SetActive(filteredList.Count == 0);
            }
            
            
            Debug.Log("Library #2 Filter Count: " + filteredList.Count);
            
            for(int i=0; i<filteredList.Count; i++) {
                LobbyStoryElement ns = Instantiate(prefabCategoryStoryElement, Vector3.zero, Quaternion.identity).GetComponent<LobbyStoryElement>();
                ns.transform.SetParent(categoryParent);
                ns.transform.localScale = Vector3.one;
        
                
                ns.Init(filteredList[i], StoryElementType.category);
                ListCategoryStory.Add(ns); // 리스트에 추가 
            
            }                              
            */
        }
    }
}