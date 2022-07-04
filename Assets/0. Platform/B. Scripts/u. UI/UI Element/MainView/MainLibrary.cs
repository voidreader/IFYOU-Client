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
        
        
        public static string SELECTED_GENRE = string.Empty; // 메인화면에서 선택된 장르 
        public static Action RefreshLibrary = null;
        
        
        public List<StoryData> filteredStoryData; // 필터링된 스토리 데이터 
        public List<LobbyStoryElement> listLibraryStory = new List<LobbyStoryElement>(); // 라이브러리에 생성된 스토리 개체 
        public GameObject prefabLibraryStory; // 프리팹 
        public Transform libraryParent; 
    
        public Toggle allToggle; // 모두 선택 토글 
        public Toggle playingToggle; // 플레이중 선택 토글 
        public Toggle likeToggle; // 관심작품 선택 토글 
        public string currentGenre = "All"; // 현재 선택된 장르
        public bool isPlayingSelected = true; //
        
        // 아무것도 없을때 표시되는 친구들 
        public GameObject NoLikeIcon;
        public GameObject NoPlayingIcon;
        
        
        public List<CustomGenreCheckBox> listGenreCheckBox; // 장르 체크박스 리스트 
        public List<string> listSelectedGenre = new List<string>();
        
        
        void Start() {
            RefreshLibrary = SetFilteredStory;
        }

        
        
        /// <summary>
        /// 라이브러리 초기화 
        /// </summary>
        public void InitLibrary() {
            
            /*
            if(!this.gameObject.activeSelf)
                return;
            */
            
            Debug.Log("### InitLibrary ###");
            
            
            int checkboxIndex = 1;
            listSelectedGenre.Clear();
            
            for(int i=0; i<listGenreCheckBox.Count;i++) {
                listGenreCheckBox[i].gameObject.SetActive(false);
                listGenreCheckBox[i].OnSelectedCheckBox = FilterGenre;
            }
            
            if(SystemManager.main.storyGenreData == null) {
                return;
            }
            
            // 마스터 체크박스는 5137로 (ALL)
            listGenreCheckBox[0].Init(SystemManager.GetLocalizedText("5137"));
            
            Debug.Log("InitLibrary Genre : "  + SystemManager.main.storyGenreData.Count);
            
            // 세팅하고 체크박스 설정한다. 
            for(int i=0; i<SystemManager.main.storyGenreData.Count;i++) {
               // 장르 세팅  
               listGenreCheckBox[checkboxIndex++].Init(SystemManager.main.storyGenreData[i]);
               if(checkboxIndex > SystemManager.main.storyGenreData.Count)
                    break;
            }
            
            // 메인에서 선택받은 장르가 있는 경우와 아닌 경우로 분리시킨다. 
            if(!string.IsNullOrEmpty(SELECTED_GENRE)) {
                Debug.Log("MAIN SELECTED GENRE : " + SELECTED_GENRE);
                
                // 선택된 체크박스와 동일한 장르만 선택처리 
                for(int i=0; i<listGenreCheckBox.Count;i++) {
                    if(listGenreCheckBox[i].localizedText == SELECTED_GENRE) 
                        listGenreCheckBox[i].OnClickCheckBox();
                }
                
                
                SELECTED_GENRE = string.Empty;
            }
            else {
                listGenreCheckBox[0].OnClickCheckBox(); // '전체' 마스터 체크박스 선택처리 
            }

        }

        
        List<StoryData> GetGenreFilteredStoryList() {
            
            // 리스트가 비어있으면 전체다. 
            if(listSelectedGenre.Count == 0)
                return null;
            else if(listSelectedGenre.Count == 1 && listSelectedGenre[0] == "all") {
                return StoryManager.main.listTotalStory;
            }
            else {
                return StoryManager.main.listTotalStory.Where(item => listSelectedGenre.Contains(item.genre)).ToList<StoryData>();
            }
        }        
        
        
        /// <summary>
        /// 장르 필터 처리 
        /// </summary>
        /// <param name="__genre"></param>
        void FilterGenre(string __genre, bool __isSelected) {
            
            
            // 체크박스 선택 
            if(__isSelected) {
            
                // 전체 장르에 대한 처리와 그 외에로 분리 
                if(__genre == SystemManager.GetLocalizedText("5137")) {
                    Debug.Log("Master checkbox Selected");
                    
                    // 본인외 모두 비선택 처리 
                    for(int i=1; i<listGenreCheckBox.Count;i++) {
                        listGenreCheckBox[i].Unselect(); 
                    }
                    
                    listSelectedGenre.Clear();
                    
                    // 모든장르를 가져오도록 변경 
                    listSelectedGenre.Add("all");
    
                }
                else { // 그외 
                    
                    listGenreCheckBox[0].Unselect(); // 마스터 체크박스 비활성화 
                    listSelectedGenre.Remove("all");
                    
                    // 선택한 장르들만 가져오도록 변경 
                    if(!listSelectedGenre.Contains(__genre))
                        listSelectedGenre.Add(__genre);
                }
            }
            else { // 선택 해제 
                if(__genre == SystemManager.GetLocalizedText("5137")) {
                    listSelectedGenre.Remove("all");
                }
                else {
                    listSelectedGenre.Remove(__genre);
                }
            }
            
            // 스토리 설정 
            SetFilteredStory();
        }
        
        
        /// <summary>
        /// 필터리된 리스트 라이브러리에 세팅하기 
        /// </summary>
        public void SetFilteredStory() {
            
            int listIndex = 0;
            int filterStoryCounter = 0;
            
            NoPlayingIcon.SetActive(false);
            NoLikeIcon.SetActive(false);
            
            // 장르로 필터링 
            filteredStoryData = GetGenreFilteredStoryList();
            
            // 화면 리스트 비활성화
            for(int i=0; i<listLibraryStory.Count;i++) {
                listLibraryStory[i].gameObject.SetActive(false);
            }
            
            
            // 탭처리 
            if(filteredStoryData != null && filteredStoryData.Count > 0) {
            
                // 장르로 분류를 한번하고, 상단 탭으로 또 처리한다.
                if(likeToggle.isOn) { // 관심작품
                    filteredStoryData.Where(item => StoryManager.main.CheckProjectLike(item.projectID)).ToList<StoryData>();
                }
                else if(playingToggle.isOn) { // 진행작품
                    filteredStoryData = filteredStoryData.Where(item => item.projectProgress > 0).ToList<StoryData>(); 
                }
            }
            
            // 결과가 없으면 특정 아이콘 노출 
            if(filteredStoryData == null || filteredStoryData.Count == 0) {
                
                NoLikeIcon.SetActive(likeToggle.isOn);
                NoPlayingIcon.SetActive(playingToggle.isOn);
                
                return;
            }
            
            
            foreach(StoryData story in filteredStoryData) {
                Debug.Log(story.title +"/" + story.genre);
                
                filterStoryCounter++;
                
                // 리스트에 미리 만들어놓은게 없으면 미리 생성 
                if(listLibraryStory.Count < filterStoryCounter) {
                    LobbyStoryElement ns = Instantiate(prefabLibraryStory, Vector3.zero, Quaternion.identity).GetComponent<LobbyStoryElement>();
                    ns.transform.SetParent(libraryParent);
                    ns.transform.localScale = Vector3.one;
                    ns.Init(story, true, false, false);
                    
                    listLibraryStory.Add(ns);
                }
                else {
                    // 만들어 놓은거 있으면 재활용
                    listLibraryStory[listIndex++].Init(story, true, false,false);
                }
            } // end of foreah
        } // 끝 
        
        
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
        
    }
}