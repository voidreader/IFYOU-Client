using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using Toast.Gamebase;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using DanielLochner.Assets.SimpleScrollSnap;

namespace PIERStory {
    public class ViewMain : CommonView
    {
        public static Action OnMoveStarShop = null;
        public static Action OnProfileSetting = null;
        public static Action<string> OnCategoryList;
        
        
        [Header("로비")]
        [SerializeField] ScrollRect mainScrollRect;
        [SerializeField] UIToggle mainToggle;

        // 프로모션 배너
        public SimpleScrollSnap promotionScroll;
        public Transform promotionContent;
        public GameObject promotionProjectPrefab;
        public GameObject promotionGoodsPrefab;
        public Transform promotionPagenation;
        public GameObject pageTogglePrefab;
        public UIToggle shopToggle;
        
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
        public ImageRequireDownload profilePortrait;
        public ImageRequireDownload profileFrame;
        public TextMeshProUGUI nickname;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI expText;
        public Image expGauge;
        bool decoMode = true;       // true = 꾸미기 모드, false = 프로필 꾸미기
        List<GameObject> createObject = new List<GameObject>();

        [Header("더보기")]
        public TextMeshProUGUI userPincode;
        public TextMeshProUGUI mNickname;       // 더보기 페이지 닉네임
        public TextMeshProUGUI mLevelText;      // 더보기 페이지 레벨
        public TextMeshProUGUI mExpText;        // 더보기 페이지 경험치
        public Image mExpGauge;                 // 더보기 페이지 경험치바
        public Image pushAlert;                 // 푸쉬 알림
        public Image nightPushAlert;            // 야간 푸쉬 알림
        public Image dataUseAgree;              // 데이터 사용 허용
        public UIToggle pushToggle;
        public UIToggle nightPushToggle;
        public UIToggle dataUseToggle;

        public Sprite spriteToggleOn;
        public Sprite spriteToggleOff;

        
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

            // 카테고리 
            InitCategory();

            InitProfile();
            InitAddMore();

            // (프로필) 닉네임, 레벨, 경험치
            levelText.text = string.Format("Lv. {0}", UserManager.main.level);

            int totalExp = SystemManager.main.GetLevelMaxExp((UserManager.main.level + 1).ToString());
            expGauge.fillAmount = (float)UserManager.main.exp / (float)totalExp;
            expText.text = string.Format("{0}/{1}", UserManager.main.exp, totalExp);

            // (더보기) 닉네임, 레벨, 경험치

            mLevelText.text = levelText.text;
            mExpGauge.fillAmount = expGauge.fillAmount;
            mExpText.text = expText.text;
        }

        /// <summary>
        /// 로비 컨테이너 초기화 
        /// </summary>
        void InitLobby()
        {
            Signal.Send(LobbyConst.STREAM_IFYOU, "initNavigation", string.Empty);

            InitPromotionList();
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
        /// 프로모션 리스트 세팅
        /// </summary>
        void InitPromotionList()
        {
            JsonData promotionList = SystemManager.main.promotionData;

            for (int i = 0; i < promotionList.Count; i++)
            {
                // 프로모션 타입을 체크
                if(SystemManager.GetJsonNodeString(promotionList[i], LobbyConst.NODE_PROMOTION_TYPE) == LobbyConst.COL_PROJECT)
                {
                    PromotionProject promotionProject = Instantiate(promotionProjectPrefab, promotionContent).GetComponent<PromotionProject>();
                    promotionProject.SetPromotionProject(SystemManager.GetJsonNodeString(promotionList[i], "location"), promotionList[i]["detail"]);
                    Instantiate(pageTogglePrefab, promotionPagenation);
                }
                else
                {
                    PromotionGoods promotionGoods = Instantiate(promotionGoodsPrefab, promotionContent).GetComponent<PromotionGoods>();
                    promotionGoods.SetPromotionGoods(SystemManager.GetJsonNodeString(promotionList[i], "location"), promotionList[i]["detail"]);
                    Instantiate(pageTogglePrefab, promotionPagenation);
                }
            }

            promotionScroll.Setup();
            OnMoveStarShop = MoveToStarShop;
        }

        void MoveToStarShop()
        {
            mainToggle.OnToggleOffCallback.Execute();
            shopToggle.OnToggleOnCallback.Execute();
        }

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
            
            if(!mainToggle.isOn)
                return;
            
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
                bool hasBG = false, hasPortrait = false;
                // currency List 화면에 배포
                for (int i = 0; i < profileCurrency.Count; i++)
                {
                    switch (SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_CURRENCY_TYPE))
                    {
                        case LobbyConst.NODE_WALLPAPER:
                            background.SetDownloadURL(SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_CURRENCY_KEY), true);
                            background.GetComponent<RectTransform>().anchoredPosition = new Vector2(float.Parse(SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_POS_X)), 0f);
                            background.GetComponent<RectTransform>().sizeDelta = new Vector2(float.Parse(SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_WIDTH)), float.Parse(SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_HEIGHT)));
                            hasBG = true;
                            break;

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

                        case LobbyConst.NODE_PORTRAIT:
                            profilePortrait.SetDownloadURL(SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_CURRENCY_KEY));
                            hasPortrait = true;
                            break;
                        case LobbyConst.NODE_FRAME:
                            profileFrame.SetDownloadURL(SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(profileCurrency[i], LobbyConst.NODE_CURRENCY_KEY), true);
                            break;
                    }
                }

                // 배경을 지정하지 않은 경우
                if (!hasBG)
                    background.SetTexture2D(null);

                // 프로필 사진 지정 안한 경우
                if (!hasPortrait)
                    profilePortrait.SetTexture2D(null);
            }

            if (profileText.Count > 0)
            {
                // 텍스트
                for (int i = 0; i < profileText.Count; i++)
                {
                    DecoTextElement textElement = Instantiate(textObjectPrefab, textObjectParent).GetComponent<DecoTextElement>();
                    textElement.SetProfileText(profileText[i]);
                    textElement.GetComponent<Image>().raycastTarget = false;
                    createObject.Add(textElement.gameObject);
                }
            }

        }

        public void OnClickDecoMode(bool __decoMode)
        {
            decoMode = __decoMode;

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

            if(decoMode)
                Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_DECO_MODE, string.Empty);
            else
                Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_MOVE_PROFILE_DECO, string.Empty);
        }

        #endregion


        #region 더보기

        void OnAddMore()
        {
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, "더보기", string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON, false, string.Empty);

            #region 게임베이스 push

            if (Application.isEditor)
                return;


            Gamebase.Push.QueryTokenInfo((data, error) =>
            {
                if(Gamebase.IsSuccess(error))
                {
                    pushToggle.isOn = data.agreement.adAgreement;
                    nightPushToggle.isOn = data.agreement.adAgreementNight;

                    Debug.Log(string.Format("TokenInfo = pushAlert : {0}, nightPush : {1}", data.agreement.adAgreement, data.agreement.adAgreementNight));
                }
                else
                    Debug.LogError(string.Format("QueryToken response failed. Error : {0}", error));
            });

            // 데이터 사용 허용
            if (PlayerPrefs.GetInt(SystemConst.KEY_NETWORK_DOWNLOAD) > 0)
                dataUseToggle.isOn = true;
            else
                dataUseToggle.isOn = false;

            #endregion

            if (pushToggle.isOn)
                pushToggle.OnToggleOnCallback.Execute();
            else
                pushToggle.OnToggleOffCallback.Execute();


            if(nightPushToggle.isOn)
                nightPushToggle.OnToggleOnCallback.Execute();
            else
                nightPushToggle.OnToggleOffCallback.Execute();

            if(dataUseToggle.isOn)
                dataUseToggle.OnToggleOnCallback.Execute();
            else
                dataUseToggle.OnToggleOffCallback.Execute();
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


        /// <summary>
        /// 푸쉬 알림 On
        /// </summary>
        public void ToggleOnPushAlert()
        {
            pushAlert.sprite = spriteToggleOn;
            nightPushAlert.color = Color.white;
            SystemManager.main.PushRegister(true, false);
        }

        /// <summary>
        /// 푸쉬 알림 OFf
        /// </summary>
        public void ToggleOffPushAlert()
        {
            pushAlert.sprite = spriteToggleOff;
            nightPushAlert.sprite = spriteToggleOff;
            nightPushAlert.color = Color.gray;

            SystemManager.main.PushRegister(false, false);
        }

        /// <summary>
        /// 야간 푸쉬 알림 On
        /// </summary>
        public void ToggleOnNightPushAlert()
        {
            // 푸쉬 알림이 켜져있지 않으면 잠군다
            if(!pushToggle.isOn)
            {
                SystemManager.ShowAlert(SystemManager.GetLocalizedText("6033"));
                return;
            }

            nightPushAlert.sprite = spriteToggleOn;
            SystemManager.main.PushRegister(true, true);
        }

        /// <summary>
        /// 야간 푸쉬 알림 Off
        /// </summary>
        public void ToggleOffNightPushAlert()
        {
            nightPushAlert.sprite = spriteToggleOff;
            SystemManager.main.PushRegister(true, false);
        }

        /// <summary>
        /// 데이터 사용 허용 On
        /// </summary>
        public void ToggleOnDataUse()
        {
            dataUseAgree.sprite = spriteToggleOn;
            PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 1);
        }

        /// <summary>
        /// 데이터 사용 허용 Off
        /// </summary>
        public void ToggleOffDataUse()
        {
            dataUseAgree.sprite = spriteToggleOff;
            PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 0);
        }

        
        #endregion


    }
}