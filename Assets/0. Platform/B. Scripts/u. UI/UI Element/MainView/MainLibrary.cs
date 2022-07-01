using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    
        public Toggle allToggle; // 상단 모두 선택 토글 
        public string currentGenre = "All"; // 현재 선택된 장르
        public bool isPlayingSelected = true; //
        
        // 아무것도 없을때 표시되는 친구들 
        public GameObject NoLikeIcon;
        public GameObject NoPlayingIcon;
        
        
        public List<CustomGenreCheckBox> listGenreCheckBox; // 장르 체크박스 리스트 
        public List<string> listSelectedGenre = new List<string>();
        
        
        void Start() {
            OnCategoryList = RequestFilteredStory;
        }

        
        
        /// <summary>
        /// 라이브러리 초기화 
        /// </summary>
        public void InitLibrary() {
            
            Debug.Log("### InitLibrary ###");
            
            int checkboxIndex = 1;
            
            for(int i=0; i<listGenreCheckBox.Count;i++) {
                listGenreCheckBox[i].gameObject.SetActive(false);
                listGenreCheckBox[i].OnSelectedCheckBox = FilterGenre;
            }
            
            if(SystemManager.main.storyGenreData == null) {
                return;
            }
            
            // 마스터 체크박스는 5137로 (ALL)
            listGenreCheckBox[0].Init(SystemManager.GetLocalizedText("5137"));
            
            Debug.Log("InitLibrary : "  + SystemManager.main.storyGenreData.Count);
            
            // 세팅하고 체크박스 설정한다. 
            for(int i=0; i<SystemManager.main.storyGenreData.Count;i++) {
               // 장르 세팅  
               listGenreCheckBox[checkboxIndex++].Init(SystemManager.main.storyGenreData[i]);
               if(checkboxIndex > SystemManager.main.storyGenreData.Count)
                    break;
            }
            
            // 메인에서 선택받은 장르가 있는 경우와 아닌 경우로 분리시킨다. 
            if(!string.IsNullOrEmpty(SELECTED_GENRE)) {
                
            }
            else {
                
            }

        }

        
        List<StoryData> GetGenreFilteredStoryList(string __genre) {
            return StoryManager.main.listTotalStory.Where( item => item.genre.Contains(__genre)).ToList<StoryData>();
        }        
        
        
        /// <summary>
        /// 장르 필터 처리 
        /// </summary>
        /// <param name="__genre"></param>
        void FilterGenre(string __genre) {
            // 전체 장르에 대한 처리와 그 외에로 분리 
            if(__genre == SystemManager.GetLocalizedText("5137")) {
                Debug.Log("Master checkbox Selected");
                
                // 본인외 모두 비선택 처리 
                for(int i=1; i<listGenreCheckBox.Count;i++) {
                    listGenreCheckBox[i].Unselect(); 
                }
                
                // 모든장르를 가져오도록 변경 
            }
            else { // 그외 
                
                listGenreCheckBox[0].Unselect(); // 마스터 체크박스 비활성화 
                
                // 선택한 장르들만 가져오도록 변경 
                
            }
        }
        
        
        /// <summary>
        /// 필터링 된 스토리 리스트 
        /// </summary>
        /// <param name="__genre"></param>
        void RequestFilteredStory(string __genre) {
            
            // 삭제 예정 
            
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
        
                
                //ns.Init(filteredList[i], StoryElementType.category);
                ListCategoryStory.Add(ns); // 리스트에 추가 
            
            }                              
            */
        }
        
        void SetFilteredStory() {
            
        } 
    }
}