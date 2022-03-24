using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PIERStory {
    public class MainLibrary : MonoBehaviour
    {
        
        public static Action<string> OnCategoryList = null;
        
        // Start is called before the first frame update
        public List<GenreToggle> ListCategoryToggle; // 토글들
        public List<LobbyStoryElement> ListCategoryStory; // 카테고리에 생성된 스토리 개체 
        public GameObject prefabCategoryStoryElement; // 프리팹 
        public Transform categoryParent; 
        
        
        void Start() {
            OnCategoryList = RequestFilteredStory;
        }
        
        public void InitLibrary() {
            
            // 상단 장르 설정
            InitCategory();
        }
        
        
        /// <summary>
        /// 상단 장르 설정 
        /// </summary>        
        void InitCategory() {
            
            //
            for(int i=0; i<ListCategoryToggle.Count;i++) {
                ListCategoryToggle[i].gameObject.SetActive(false);
            }
            
            for(int i=0; i<SystemManager.main.storyGenreData.Count;i++) {
                
                if(ListCategoryToggle.Count <= i)  {
                    Debug.LogError("Too many genre data");
                    break;
                }
                
                ListCategoryToggle[i].SetGenre(SystemManager.main.storyGenreData[i]);
            }            
            
        }   
        
        List<StoryData> GetGenreFilteredStoryList(string __genre) {
            return StoryManager.main.listTotalStory.Where( item => item.genre.Contains(__genre)).ToList<StoryData>();
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
            
            if(__genre == "All") {
                filteredList = StoryManager.main.listTotalStory;
            }
            else {
                filteredList = GetGenreFilteredStoryList(__genre);

            }
         
                
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
}