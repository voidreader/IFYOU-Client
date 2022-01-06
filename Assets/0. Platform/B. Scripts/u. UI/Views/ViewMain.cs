using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using LitJson;
using TMPro;
using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;
using System.Linq;

namespace PIERStory {
    public class ViewMain : CommonView
    {
        public static Action OnProfileSetting = null;
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
        [SerializeField] List<GenreToggle> ListCategoryToggle; // 토글들
        [SerializeField] List<NewStoryElement> ListCategoryStory = new List<NewStoryElement>(); // 카테고리에 생성된 스토리 개체들 
        [SerializeField] GameObject prefabStoryElement; // 프리팹
        [SerializeField] GameObject NoInterestStory; // 관심작품 없음
        [SerializeField] Transform categoryParent;
                
        

        [Header("프로필")]
        public ImageRequireDownload background;
        public Transform decoObjectParent;
        public GameObject decoObjectPrefab;
        public GameObject standingObjectPrefab;
        public Transform textObjectParent;
        public GameObject textObjectPrefab;
        List<GameObject> createObject = new List<GameObject>();

        [Header("더보기")]
        public TextMeshProUGUI userPincode;
        
        float mainScrollRectY = 0;
        
        void Start() {
            OnCategoryList = CallCategoryList;
        }
        
        public override void OnView()
        {
            base.OnView();
            
            LobbyManager.main.RequestPlatformLoadingImages(); // 플랫폼 로딩 이미지 다운로드 처리 
        }
        
        public override void OnStartView() {
            
            base.OnStartView();
            
            InitLobby();

            OnProfileSetting = InitProfile;

            InitProfile();
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
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON, false, string.Empty);
        }

        
        public void OnClickTabNavigation(int index) {
            switch(index) {
                case 0:  // 로비
                OnLobbyTab();
                break;
                
                case 1:  // 카테고리
                OnCategoryTab();
                break;
                
                case 2: // 상점
                OnShopTab();
                break;

                case 3:
                    break;

                case 4:     // 프로필
                    OnProfile();
                    break;
                case 5:     // 더보기
                    OnAddMore();
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
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON, false, string.Empty);
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
            
            Debug.Log("CallCategoryList : " + __genre);
            
            if(NoInterestStory == null)
                return;
            
            NoInterestStory.SetActive(false);
            
            
            // 기존에 생성된 게임오브젝트 제거 후 클리어             
            for(int i=0; i<ListCategoryStory.Count;i++) {
                Destroy(ListCategoryStory[i].gameObject);
            }
            ListCategoryStory.Clear();
            
            // 조건에 맞는 작품 검색 
            List<StoryData> filteredList = null;
            
            if(__genre == "전체") {
                filteredList = StoryManager.main.listTotalStory;
            }
            else if(__genre.Contains("관심작품")) {
                NoInterestStory.SetActive(true);    
                return;
            }
            else {
                filteredList = GetGenreFilteredStoryList(__genre);
            }
            
            if(filteredList != null)
                Debug.Log("CallCategory Filter Count: " + filteredList.Count);
            
            
            for(int i=0; i<filteredList.Count; i++) {
                NewStoryElement ns = Instantiate(prefabStoryElement, Vector3.zero, Quaternion.identity).GetComponent<NewStoryElement>();
                ns.transform.SetParent(categoryParent);
                ns.transform.localScale = Vector3.one;
                
                ns.InitStoryElement(filteredList[i]);
                ListCategoryStory.Add(ns); // 리스트에 추가 
               
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

        #region 상점
        
        /// <summary>
        /// 상점 탭 활성화
        /// </summary>
        public void OnShopTab() {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
        }
        
        #endregion

        #region 프로필

        void OnProfile()
        {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MAIL_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON, false, string.Empty);
        }


        void InitProfile()
        {
            // 남아있는게 있으면 일단 다 뿌셔뿌셔
            foreach (GameObject g in createObject)
                Destroy(g);

            createObject.Clear();

            JsonData profileCurrency = SystemManager.GetJsonNode(UserManager.main.userProfile, LobbyConst.NODE_CURRENCY);
            JsonData profileText = SystemManager.GetJsonNode(UserManager.main.userProfile, LobbyConst.NODE_TEXT);

            if (profileCurrency.Count > 0)
            {
                int sortingOrder = 0;

                // 배경을 먼저 넣어준다. 배경이 존재한다면 sortingOrder 0값이 존재한다.
                if (SystemManager.GetJsonNodeString(profileCurrency[sortingOrder], LobbyConst.NODE_SORTING_ORDER) == "0")
                {
                    background.SetDownloadURL(SystemManager.GetJsonNodeString(profileCurrency[sortingOrder], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(profileCurrency[sortingOrder], LobbyConst.NODE_CURRENCY_KEY));
                    background.GetComponent<RectTransform>().anchoredPosition = new Vector2(float.Parse(SystemManager.GetJsonNodeString(profileCurrency[sortingOrder], LobbyConst.NODE_POS_X)), 0f);
                    background.GetComponent<RectTransform>().sizeDelta = new Vector2(float.Parse(SystemManager.GetJsonNodeString(profileCurrency[sortingOrder], LobbyConst.NODE_WIDTH)), float.Parse(SystemManager.GetJsonNodeString(profileCurrency[sortingOrder], LobbyConst.NODE_HEIGHT)));
                    sortingOrder++;
                }

                // 데코 아이템, 스탠딩 화면에 뿌리기
                for (int i = sortingOrder; i < profileCurrency.Count; i++)
                {
                    switch (SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_CURRENCY_TYPE))
                    {
                        case LobbyConst.NODE_BADGE:
                        case LobbyConst.NODE_STICKER:
                            ItemElement deco = Instantiate(decoObjectPrefab, decoObjectParent).GetComponent<ItemElement>();
                            deco.SetProfileItem(profileCurrency[i]);
                            deco.GetComponent<Image>().raycastTarget = false;       // 프로필 페이지에서 선택되면 안돼!
                            createObject.Add(deco.gameObject);
                            break;
                        case LobbyConst.NODE_STANDING:
                            StandingElement standingElement = Instantiate(standingObjectPrefab, decoObjectParent).GetComponent<StandingElement>();
                            standingElement.SetProfileStanding(profileCurrency[i]);
                            standingElement.GetComponent<Image>().raycastTarget = false;
                            createObject.Add(standingElement.gameObject);
                            break;
                    }
                    
                }
            }

            if (profileText.Count > 0)
            {
                // 텍스트
                for (int i = 0; i < profileText.Count; i++)
                {
                    DecoTextElement textElement = Instantiate(textObjectPrefab, textObjectParent).GetComponent<DecoTextElement>();
                    textElement.SetProfileText(profileText[i]);
                    textElement.inputField.interactable = false;     // 프로필 페이지에서 텍스트가 편집되면 안돼!
                    textElement.GetComponent<Image>().raycastTarget = false;
                    createObject.Add(textElement.gameObject);
                }
            }
        }

        public void OnClickDecoMode()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = LobbyConst.FUNC_GET_PROFILE_CURRENCY_OWN_LIST;
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;

            NetworkLoader.main.SendPost(CallbackGetProfileCurrencyList, sending, true);
        }

        void CallbackGetProfileCurrencyList(HTTPRequest req, HTTPResponse res)
        {
            if(!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackGetProfileCurrencyList");
                return;
            }

            // 통신이 완료된 후, 사용자가 소지하고 있는 프로필 재화 정보를 받은 뒤, 페이지를 넘긴다.
            UserManager.main.userProfileCurrency = JsonMapper.ToObject(res.DataAsText);
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_DECO_MODE, string.Empty);
        }

        #endregion


        #region 더보기

        void OnAddMore()
        {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, "더보기", string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON, false, string.Empty);
        }


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