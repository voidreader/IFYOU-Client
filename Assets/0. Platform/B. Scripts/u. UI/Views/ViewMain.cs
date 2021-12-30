using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using LitJson;
using TMPro;
using Doozy.Runtime.Signals;
using System.Linq;

namespace PIERStory {
    public class ViewMain : CommonView
    {
        public static Action<string> OnCategoryList;
        
        
        [Header("로비")]
        [SerializeField] ScrollRect mainScrollRect;
        
        
        // 진행중인 이야기 타이틀과 ScrollRect
        [SerializeField] GameObject playingAreaTitle;
        [SerializeField] GameObject playingAreaScrollRect;
        
        [SerializeField] List<PlayingStoryElement> ListPlayingStoryElements; // 진행중 이야기 
        [SerializeField] List<MainStoryRow> ListRecommendStoryRow; // 추천 스토리의 2열짜리 행 
        [SerializeField] List<NewStoryElement> ListNewStoryElement; // 새로운 이야기 개별 개체 
        
        [Header("카테고리")] 
        JsonData genreData = null;
        [SerializeField] List<GenreToggle> ListCategoryToggle;
        [SerializeField] GameObject prefabStoryElement; // 프리팹
        [SerializeField] GameObject NoInterestStory; // 관심작품 없음
        [SerializeField] Transform categoryParent;
                
        

        [Header("더보기")]
        public TextMeshProUGUI userPincode;
        
        float mainScrollRectY = 0;
        
        public override void OnView()
        {
            base.OnView();
            
        }
        
        public override void OnStartView() {
            
            base.OnStartView();
            
            // 로비 
            InitLobby();
            
            // 더보기 
            InitAddMore();
            
            // 카테고리 
            InitCategory();
        }
        
        /// <summary>
        /// 로비 컨테이너 초기화 
        /// </summary>
        void InitLobby() {
            Signal.Send(LobbyConst.STREAM_IFYOU, "initNavigation", string.Empty);
            
            InitPlayingStoryElements(); // 진행중인 이야기 Area 초기화 
            InitRecommendStory(); // 추천스토리 Area 초기화
            InitNewStoryElements(); // 새로운 이야기 Area 초기화
            
        }
        
        public void OnLobbyTab() {
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);            
        }

        
        public void OnClickTabNavigation(int index) {
            switch(index) {
                case 0: 
                OnLobbyTab();
                break;
                
                case 1:
                OnCategoryTab();
                break;
            }
        }
        
        
        #region 메인 로비 
        
        /// <summary>
        /// 진행중인 이야기 초기화
        /// </summary>
        void InitPlayingStoryElements() {
            ResetPlayingStoryElements();
            
            int elementIndex = 0;
            
            for(int i=0;i<StoryManager.main.listTotalStory.Count;i++) {
                
                if(!StoryManager.main.listTotalStory[i].isPlaying)
                    continue;
                
                
                // 진행기록이 있는 작품만 가져온다.                 
                ListPlayingStoryElements[elementIndex++].InitElement(StoryManager.main.listTotalStory[i]);
                
                if(elementIndex == 1)  {
                    playingAreaTitle.SetActive(true);
                    playingAreaScrollRect.SetActive(true);
                }
            }
                
        }
        
        /// <summary>
        /// 진행중인 이야기 Reset
        /// </summary>
        void ResetPlayingStoryElements() {
            
            for(int i=0; i<ListPlayingStoryElements.Count;i++) {
                ListPlayingStoryElements[i].gameObject.SetActive(false);
            }
            
            playingAreaTitle.SetActive(false);
            playingAreaScrollRect.SetActive(false);
        }
        
        /// <summary>
        /// 새로운 이야기 Area 초기화 
        /// </summary>
        void InitRecommendStory() {
            ResetRecommendStory();
            
            // 작품개수를 2로 나눈다. 
            int dividedIntoTwo = Mathf.FloorToInt((float)StoryManager.main.listRecommendStory.Count / 2f );
            
            // 2배수로 나눈 수만큼 초기화 시작.
            for(int i=0; i<dividedIntoTwo; i++) {
                ListRecommendStoryRow[i].InitRow(i);
            }
            
        }
        
        /// <summary>
        /// 신규 스토리 세팅 
        /// </summary>
        void InitNewStoryElements() {
            ResetNewStory();
            
            
            for(int i=0; i<StoryManager.main.listTotalStory.Count;i++) {
                ListNewStoryElement[i].InitStoryElement(StoryManager.main.listTotalStory[i]);
            }   
        }
        
        
        void ResetRecommendStory() {
            for(int i=0; i<ListRecommendStoryRow.Count; i++) {
                ListRecommendStoryRow[i].gameObject.SetActive(false);
            }
        }
        
        void ResetNewStory() {
            for(int i=0; i<ListNewStoryElement.Count; i++) {
                ListNewStoryElement[i].gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 메인ScrollRect 상하 변경시.. 상단 제어 
        /// </summary>
        /// <param name="vec"></param>
        public void OnValueChangedMainScroll(Vector2 vec) {
            
            if(mainScrollRectY == vec.y)
                return;
                
            mainScrollRectY = vec.y;
            
            if(mainScrollRectY < 0.95f && !ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
                return;
            }
           
            
            if(mainScrollRectY >= 0.95f && ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
                return;
            }
        }

        #endregion

        #region 카테고리
        
        /// <summary>
        /// 카테고리 탭 활성화 
        /// </summary>
        public void OnCategoryTab() {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
        }
        
        /// <summary>
        /// 
        /// </summary>
        void InitCategory() {
            
            //
            for(int i=0; i<ListCategoryToggle.Count;i++) {
                ListCategoryToggle[i].gameObject.SetActive(false);
            }
            
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "getDistinctProjectGenre";
            NetworkLoader.main.SendPost(OnCallbackGenre, sending, false);
        }
        
        void OnCallbackGenre(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            Debug.Log("OnCallbackGenre");
            
            genreData = JsonMapper.ToObject(response.DataAsText);
            
            // 
            for(int i=0; i<genreData.Count;i++) {
                
                if(ListCategoryToggle.Count <= i)  {
                    Debug.LogError("Too many genre data");
                    break;
                }
                
                ListCategoryToggle[i].SetGenre(genreData[i]);
            }
            
            NoInterestStory.SetActive(true);
            
        }
        
        void CallCategoryList(string __genre)  {
            NoInterestStory.SetActive(false);
            
            List<StoryData> filteredList = null;
            
            if(__genre == "전체") {
                filteredList = StoryManager.main.listTotalStory;
            }
            else if(__genre.Contains("관심작품")) {
                NoInterestStory.SetActive(true);    
            }
            else {
                filteredList = GetGenreFilteredStoryList(__genre);
            }
            
            for(int i=0; i<filteredList.Count; i++) {
                // NewStoryElement ns = Instantiate(prefabStoryElement, Vector3.zero, Quaternion.)
            }
            
        }
        
        /// <summary>
        /// 장르로 필터 걸어서 리스트 가져오기 
        /// </summary>
        /// <param name="__genre"></param>
        /// <returns></returns>
        List<StoryData> GetGenreFilteredStoryList(string __genre) {
            return StoryManager.main.listTotalStory.Where( item => item.genre.Contains("__genre")).ToList<StoryData>();
        }
        
        #endregion

        #region 상점
        #endregion

        #region 더보기

        /// <summary>
        /// 더보기 페이지 설정
        /// </summary>
        void InitAddMore()
        {
            userPincode.text = string.Format("UID : {0}", UserManager.main.GetUserPinCode());
        }


        /// <summary>
        /// pin code 복사
        /// </summary>
        public void OnClickCopyUID()
        {
            UniClipboard.SetText(userPincode.text);
        }
        
        #endregion


    }
}