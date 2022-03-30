using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using Toast.Gamebase;

using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewStoryLobby : CommonView
    {
        public static Action OnDecorateSet = null;
        public static Action OnDisableAllOptionals = null;
        public static Action<bool> OnInActiveInteractable = null;
        public static Action<JsonData> OnSelectBackground = null;
        public static Action<JsonData, ProfileItemElement> OnSelectStanding = null;
        public static Action<JsonData, ProfileItemElement> OnStickerSetting = null;

        public static bool loadComplete = false;

        [Header("메인 관련 제어")]
        public GameObject backButton;               // 뒤로가기 버튼
        public GameObject premiumpassButton;        // 프리미엄 패스 버튼
        public GameObject premiumpassBadge;         // 프리미엄 패스 뱃지
        public GameObject showDetailButton;         // 꾸미기 자세히 보기 버튼

        public Image showDetailIcon;
        public Sprite spriteIconEyeOpen;
        public Sprite spriteIconEyeClose;


        [Space(15)][Header("꾸미기 편집 관련")]
        ScriptImageMount bg;
        string bgCurrency = string.Empty;
        public Transform characterParent;
        public Transform stickerParent;
        public List<GameObject> decoObjects = new List<GameObject>();          // 화면에 꾸며놓은 Object들
        public List<GameObject> currencyElements = new List<GameObject>();     // 꾸미기 각 element들

        public GameObject stickerObjectPrefab;
        public List<UIToggle> typeToggles;
        public UIContainer decoListContainer;
        public GameObject coinShopButton;
        public List<GameModelCtrl> liveModels = new List<GameModelCtrl>();
        public List<ScriptModelMount> listModelMounts = new List<ScriptModelMount>();

        bool moveBg = false;
        bool moveCharacter = false;

        Vector2 cursor = Vector2.one;
        float originX = 0, startX = 0f, dragX = 0f, moveX = 0f;

        float movableWidth = 1f;
        float camWidth = 1f;

        public UIContainer mainContainer;
        public UIContainer decoContainer;
        JsonData storyProfile;              // 작품별 꾸미기 정보
        JsonData currencyList;              // 작품별 꾸미기 재화 리스트


        [Space(20)][Header("스탠딩 캐릭터 관련")]
        public GameObject standingListPrefab;
        public Transform standingListContent;
        GameModelCtrl controlModel;
        public GameObject standingListScroll;
        public GameObject standingController;
        public GameObject usageStandingControl;
        public GameObject noneStandingCurrency;

        [Space][Header("배경 관련")]
        public GameObject bgListPrefab;
        public Transform bgListContent;
        public GameObject bgListScroll;
        public GameObject bgScrolling;
        public GameObject noneBackgroundCurrency;

        
        [Space][Header("스티커 관련")]
        public GameObject stickerListPrefab;
        public Transform stickerListContent;
        public GameObject stickerScroll;
        public GameObject noneStickerCurrency;

        int totalDecoLoad = 0;

        private void Awake()
        {
            OnDecorateSet = DecorateSetting;
            OnSelectBackground = SelectBackground;
            OnSelectStanding = SelectLiveCharacter;
            OnStickerSetting = CreateStickerElement;
            OnInActiveInteractable = ActiveInteractable;
        }

        private void Start()
        {
            camWidth = (2 * Camera.main.orthographicSize) * Camera.main.aspect;
        }

        public override void OnStartView()
        {
            base.OnStartView();

            // Action 세팅
            OnDisableAllOptionals = DisableAllStickerOptionals;
            
            // 진입시에 백버튼 시그널 추가 
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
            
        }


        public override void OnView()
        {
            base.OnView();

            if (UserManager.main.tutorialStep == 1 && !UserManager.main.tutorialClear)
                UserManager.main.UpdateTutorialStep(1, 0, CallbackStartTutorial);


            if(UserManager.main.gameComplete)
            {
                RateGame.Instance.CanShowRate();
                UserManager.main.gameComplete = false;
            }
        }

        public override void OnHideView()
        {
            base.OnHideView();

            foreach (GameObject g in decoObjects)
                Destroy(g);

            decoObjects.Clear();

            usageStandingControl.SetActive(false);

            LobbyManager.main.lobbyBackground.sprite = null;
            loadComplete = false;
        }
        
        
        /// <summary>
        /// 모델 생성 후 일정의 시간차가 필요하다. (특히 에셋번들로 생성된 경우)
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator DelayLiveModelAnimation() {
            
            // 0.1초 간격준다. 
            yield return new WaitForSeconds(0.1f);
            
            // 스탠딩 캐릭터 기본 모션 세팅
            foreach (GameModelCtrl gm in liveModels)
            {
                yield return new WaitUntil(() => gm.motionLists.Count == gm.DictMotion.Count && gm.motionLists.Count > 0);
                // 임시로 랜덤하게 재생한다. 
                gm.PlayLobbyAnimation(gm.motionLists[UnityEngine.Random.Range(0, gm.motionLists.Count)]);

                for (int i = 0; i < gm.motionLists.Count; i++)
                {
                    if (gm.motionLists[i].Contains("기본") && !gm.motionLists[i].Contains("M"))
                    {
                        // 번들용과 다운로드에서 애니메이션 재생 구분처리 해주어야한다. 
                        gm.PlayLobbyAnimation(gm.motionLists[i]);
                        break;
                    }
                }
            }

            if (liveModels.Count > 1)
                liveModels[1].model.GetComponent<Live2D.Cubism.Rendering.CubismRenderController>().SortingOrder += 1200;
        }
        

        #region 통상적(Main)

        /// <summary>
        /// 작품 로비 꾸미기 진입
        /// </summary>
        public void OnClickDecoMode()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "getUserStoryProfileCurrencyList";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;

            NetworkLoader.main.SendPost(CallbackDecoMode, sending, true);
        }

        void CallbackDecoMode(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackDecoMode");
                return;
            }

            // 데이터 불러오고 데코 모드 준비하기
            SetDecoMode(JsonMapper.ToObject(res.DataAsText));
            coinShopButton.SetActive(true);
        }
        
        /// <summary>
        /// 데코 모드 준비하기 
        /// </summary>
        /// <param name="__decoCurrencyList"></param>
        void SetDecoMode(JsonData __decoCurrencyList) {
            currencyList = __decoCurrencyList;

            // 생성 전 리스트 초기화
            foreach (GameObject g in currencyElements)
                Destroy(g);

            currencyElements.Clear();

            // 리스트 재화 항목 생성
            CreateListObject(LobbyConst.NODE_WALLPAPER, bgListPrefab, bgListContent);
            CreateListObject(LobbyConst.NODE_STANDING, standingListPrefab, standingListContent);
            CreateListObject(LobbyConst.NODE_STICKER, stickerListPrefab, stickerListContent);

            SortingList(bgListContent);
            SortingList(stickerListContent);
            StandingListSort();

            // 진행전에 decoObjects 정리 
            // Destroy된 개체들이 정리되지 않음 
            for (int i = decoObjects.Count - 1; i >= 0; i--)
            {
                if (decoObjects[i] == null)
                    decoObjects.RemoveAt(i);
            }


            // 화면에 생성된 object와 재화 listElement 연결
            for (int i = 0; i < decoObjects.Count; i++)
            {
                // 스티커가 아니면 건너뛰기
                if (decoObjects[i].GetComponent<StickerElement>() == null)
                    continue;

                for (int j = 0; j < currencyElements.Count; j++)
                {
                    // 재화명이 같은 것을 찾아서 연결
                    if(decoObjects[i].GetComponent<StickerElement>().currencyName == currencyElements[j].GetComponent<ProfileItemElement>().currencyName)
                    {
                        decoObjects[i].GetComponent<StickerElement>().currencyElement = currencyElements[j].GetComponent<ProfileItemElement>();
                        break;
                    }
                }
            }

            // 배경
            bgListScroll.SetActive(bgListContent.childCount > 0);
            noneBackgroundCurrency.SetActive(bgListContent.childCount < 1);

            // 스탠딩
            standingListScroll.SetActive(standingListContent.childCount > 0);
            noneStandingCurrency.SetActive(standingListContent.childCount < 1);

            // 스티커
            stickerScroll.SetActive(stickerListContent.childCount > 0);
            noneStickerCurrency.SetActive(stickerListContent.childCount < 1);


            // 상단의 프리미엄 패스, 버튼 두개 비활성화
            premiumpassButton.SetActive(false);
            premiumpassBadge.SetActive(false);
            showDetailButton.SetActive(false);

            ActiveInteractable(true);

            mainContainer.Hide();
            decoContainer.Show();

            foreach (UIToggle toggle in typeToggles)
                toggle.GetComponentInChildren<CanvasGroup>().alpha = 1f;

            Signal.Send(LobbyConst.STREAM_IFYOU, "showStoryLobbyDeco", string.Empty);
        }
        

        /// <summary>
        /// 로비 꾸며놓은거 자세히 보기
        /// </summary>
        public void ShowLobbyDetail()
        {
            if(mainContainer.isVisible)
            {
                backButton.SetActive(false);
                premiumpassButton.SetActive(false);
                premiumpassBadge.SetActive(false);
                showDetailIcon.sprite = spriteIconEyeClose;
                mainContainer.Hide();
            }
            else
            {
                backButton.SetActive(true);
                premiumpassButton.SetActive(!UserManager.main.HasProjectFreepass());
                premiumpassBadge.SetActive(UserManager.main.HasProjectFreepass());
                showDetailIcon.sprite = spriteIconEyeOpen;
                mainContainer.Show();
            }
        }

        void CallbackStartTutorial(HTTPRequest req, HTTPResponse res)
        {
            if(!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackStartTutorial");
                return;
            }

            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_TUTORIAL_MISSION_1);
            PopupManager.main.ShowPopup(p, false);
        }


        #endregion


        #region 작품 꾸미기 모드 
        

        /// <summary>
        /// 작품 꾸미기 한거 세팅하기
        /// </summary>
        void DecorateSetting()
        {
            liveModels.Clear();
            listModelMounts.Clear();

            // 이 작업이 StoryLoading에서 이뤄져야해
            storyProfile = SystemManager.GetJsonNode(UserManager.main.currentStoryJson, "storyProfile");
            totalDecoLoad = storyProfile.Count;

            if (totalDecoLoad == 0)
            {
                loadComplete = true;
                return;
            }

            for (int i = 0; i < storyProfile.Count; i++)
            {
                switch (SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY_TYPE))
                {
                    case LobbyConst.NODE_WALLPAPER:     // 배경
                        bg = new ScriptImageMount(GameConst.TEMPLATE_BACKGROUND, storyProfile[i], BGLoadComplete);
                        bgCurrency = SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY);
                        LobbyManager.main.lobbyBackground.transform.localPosition = new Vector3(SystemManager.GetJsonNodeInt(storyProfile[i], LobbyConst.NODE_POS_X), 0, 0);
                        break;

                    case GameConst.NODE_SCRIPT:         // 대사


                        break;
                    case LobbyConst.NODE_STICKER:       // 스티커

                        StickerElement sticker = Instantiate(stickerObjectPrefab, stickerParent).GetComponent<StickerElement>();
                        sticker.SetStickerElement(storyProfile[i], StickerLoadComplete);
                        decoObjects.Add(sticker.gameObject);

                        break;
                    case LobbyConst.NODE_STANDING:      // 스탠딩 캐릭터

                        ScriptModelMount character = new ScriptModelMount(SystemManager.GetJsonNodeString(storyProfile[i], GameConst.COL_MODEL_NAME), CharacterLoadComplete, LobbyManager.main);
                        character.SetModelDataFromStoryManager(true);

                        if (SystemManager.main.hasSafeArea)
                            character.modelController.transform.localPosition = new Vector3(SystemManager.GetJsonNodeFloat(storyProfile[i], LobbyConst.NODE_POS_X), GameConst.MODEL_PARENT_SAFEAREA_POS_Y, 0);
                        else
                            character.modelController.transform.localPosition = new Vector3(SystemManager.GetJsonNodeFloat(storyProfile[i], LobbyConst.NODE_POS_X), GameConst.MODEL_PARENT_ORIGIN_POS_Y, 0);

                        character.modelController.currencyName = SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY);

                        if (SystemManager.GetJsonNodeFloat(storyProfile[i], LobbyConst.NODE_ANGLE) > 0)
                            character.modelController.transform.localScale = new Vector3(-1, 1, 1);

                        liveModels.Add(character.modelController);
                        decoObjects.Add(character.modelController.gameObject);
                        listModelMounts.Add(character);

                        break;
                }
            }
        }

        /// <summary>
        /// 스크롤의 리스트에 생성하기
        /// </summary>
        /// <param name="key">json key값</param>
        /// <param name="listObject">list prefab</param>
        /// <param name="parent">list가 생성될 위치의 부모(transform)</param>
        void CreateListObject(string key, GameObject listObject, Transform parent)
        {
            // 만약 해당 key값을 포함하지 않으면 실행하지 않음
            if (!currencyList.ContainsKey(key))
                return;

            ProfileItemElement listElement = null;

            for (int i = 0; i < currencyList[key].Count; i++)
            {
                listElement = Instantiate(listObject, parent).GetComponent<ProfileItemElement>();
                listElement.InitCurrencyListElement(currencyList[key][i]);

                currencyElements.Add(listElement.gameObject);
            }
        }

        /// <summary>
        /// Toggle이 한개라도 On인지 체크
        /// </summary>
        bool ToggleOnCheck()
        {
            for (int i = 0; i < typeToggles.Count; i++)
            {
                if (typeToggles[i].isOn)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 모든 스티커,뱃지 옵셔널 박스 비활성화
        /// </summary>
        public void DisableAllStickerOptionals()
        {
            for (int i = 0; i < stickerParent.childCount; i++)
                stickerParent.GetChild(i).GetComponent<StickerElement>().DisableControlBox();
        }


        /// <summary>
        /// 하단의 버튼을 통해 스크롤 영역 보이게 하기
        /// </summary>
        public void OnClickShowDecoListContainer()
        {
            if (ToggleOnCheck() && decoListContainer.isHidden)
            {
                decoListContainer.Show();
                coinShopButton.SetActive(false);

                foreach (UIToggle toggle in typeToggles)
                {
                    if (!toggle.isOn)
                        toggle.GetComponentInChildren<CanvasGroup>().alpha = 0.3f;
                }
            }
            else if (!ToggleOnCheck() && decoListContainer.isVisible)
            {
                decoListContainer.Hide();
                coinShopButton.SetActive(true);

                foreach (UIToggle toggle in typeToggles)
                    toggle.GetComponentInChildren<CanvasGroup>().alpha = 1f;
            }
        }

        /// <summary>
        /// 게임형 로비 꾸미기모드 빠져나옴
        /// </summary>
        public void HideDecoContainer()
        {
            foreach (GameObject g in currencyElements)
                Destroy(g);

            currencyElements.Clear();

            premiumpassButton.SetActive(!UserManager.main.HasProjectFreepass());
            premiumpassBadge.SetActive(UserManager.main.HasProjectFreepass());
            showDetailButton.SetActive(true);

            foreach(GameModelCtrl models in liveModels)
                models.ChangeLayerRecursively(models.transform, GameConst.LAYER_MODEL_C);

            usageStandingControl.SetActive(false);

            moveBg = false;
            moveCharacter = false;
            loadComplete = false;
        }



        /// <summary>
        /// 불러오기 완료
        /// </summary>
        void CheckLoadComplete()
        {
            if (totalDecoLoad <= 0)
                loadComplete = true;

            if(loadComplete)
            {
                StartCoroutine(DelayLiveModelAnimation());
                ActiveInteractable(false);
            }
        }

        public void OnDragScreen(InputAction.CallbackContext context)
        {
            cursor = context.ReadValue<Vector2>();

            // 배경 드래그
            if(moveBg)
            {
                if (context.started)
                {
                    startX = cursor.x;
                    originX = LobbyManager.main.lobbyBackground.transform.localPosition.x;
                }
                else if (context.performed)
                {
                    dragX = cursor.x;

                    moveX = originX + ((dragX - startX) * (Time.deltaTime * 1f));

                    if (moveX > -movableWidth && moveX < movableWidth)
                        LobbyManager.main.lobbyBackground.transform.localPosition = new Vector3(moveX, 0f, 0f);
                }
            }

            // 스탠딩 캐릭터 드래그
            if(moveCharacter)
            {
                if (context.started)
                {
                    startX = cursor.x;
                    originX = controlModel.transform.localPosition.x;
                }
                else if (context.performed)
                {
                    dragX = cursor.x;
                    moveX = originX + ((dragX - startX) * (Time.deltaTime * 1f));

                    // 선택된 캐릭터 이동되게 해야됨
                    if (moveX > (-camWidth * 0.25f) && moveX < (camWidth * 0.25f))
                        controlModel.transform.localPosition = new Vector3(moveX, controlModel.transform.localPosition.y, 0f);
                }
            }
        }

        public void OnEscapeDeocoMode(InputAction.CallbackContext context)
        {
            EscapeDecoMode();
        }

        public void OnClickBackDecoMode()
        {
            EscapeDecoMode();
        }

        void EscapeDecoMode()
        {
            // 꾸미기 모드(편집 모드) 중인데 back버튼 입력을 받으면
            if (!mainContainer.isActiveAndEnabled && decoContainer.isActiveAndEnabled)
            {
                decoContainer.Hide();

                // 수정한게 있든 없든 일단 다 뿌셔!
                foreach (GameObject g in decoObjects)
                    Destroy(g);

                decoObjects.Clear();
                LobbyManager.main.lobbyBackground.sprite = null;

                // 서버에 저장되어 있는 기반으로 다시 만들어!
                DecorateSetting();
                ActiveInteractable(false);

                mainContainer.Show();
            }
        }

        /// <summary>
        /// 스탠딩 캐릭터 리스트 정렬
        /// </summary>
        void StandingListSort()
        {
            ProfileItemElement[] items = standingListContent.GetComponentsInChildren<ProfileItemElement>();

            int sortIndex = 0;

            // currentCount가 1이상인 애들을 우선적으로 두고
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].currentCount >= 1)
                {
                    items[i].transform.SetSiblingIndex(sortIndex);
                    items[i].useCheckIcon.SetActive(true);
                    sortIndex++;
                }
            }

            // 다음은 구매한 애들을 두번째
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].currentCount < 1 && items[i].totalCount > 0)
                {
                    items[i].transform.SetSiblingIndex(sortIndex);
                    items[i].useCheckIcon.SetActive(false);
                    sortIndex++;
                }
            }

            // 미구매 상품
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].currentCount < 1 && items[i].totalCount == 0)
                {
                    items[i].transform.SetSiblingIndex(sortIndex);
                    items[i].useCheckIcon.SetActive(false);
                    sortIndex++;
                }
            }
        }

        /// <summary>
        ///  선택된 항목 배열 재정리
        /// </summary>
        void SortingList(Transform content)
        {
            ProfileItemElement[] items = content.GetComponentsInChildren<ProfileItemElement>();

            int sortIndex = 0;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].totalCount > 0)
                {
                    items[i].transform.SetSiblingIndex(sortIndex);
                    sortIndex++;
                }
            }

            // 미구매 상품
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].currentCount < 1 && items[i].totalCount == 0)
                {
                    items[i].transform.SetSiblingIndex(sortIndex);
                    sortIndex++;
                }
            }
        }


        #region 배경 관련


        void BGLoadComplete()
        {
            if (totalDecoLoad > 0)
                totalDecoLoad--;

            LobbyManager.main.lobbyBackground.sprite = null;

            // 어드레서블에 없는 경우 이전 방식을 사용해서 다운만 받기 떄문에 LoadImage를 해서 생성도 해줘야함
            if (bg.sprite == null)
                bg.LoadImage();
            
            LobbyManager.main.lobbyBackground.sprite = bg.sprite;
            LobbyManager.main.lobbyBackground.transform.localScale = new Vector3(bg.gameScale, bg.gameScale, 1f);

            movableWidth = Mathf.Abs(LobbyManager.main.lobbyBackground.size.x * LobbyManager.main.lobbyBackground.transform.localScale.x - camWidth) * 0.5f;
            CheckLoadComplete();
        }

        void SelectBackground(JsonData bgData)
        {
            bg = new ScriptImageMount(GameConst.TEMPLATE_BACKGROUND, bgData, BGLoadComplete);
            bgCurrency = SystemManager.GetJsonNodeString(bgData, LobbyConst.NODE_CURRENCY);
            LobbyManager.main.lobbyBackground.transform.localPosition = new Vector3(0f, 0f, 0f);

            for (int i = 0; i < bgListContent.childCount; i++)
                bgListContent.GetChild(i).GetComponent<ProfileItemElement>().currentCount = 0;


            ActiveInteractable(false);

            bgListScroll.SetActive(false);
            bgScrolling.SetActive(true);
            moveBg = true;
        }


        /// <summary>
        /// 배경 스크롤링 후 위치 고정
        /// </summary>
        public void OnClickFixBackground()
        {
            bgListScroll.SetActive(true);
            bgScrolling.SetActive(false);
            moveBg = false;

            // 배경 설정 끝냈으니 raycastTarget을 켜준다
            ActiveInteractable(true);
        }

        #endregion

        #region 스탠딩 캐릭터 세팅

        /// <summary>
        /// 로비 진입시 스탠딩 캐릭터 생성 완료
        /// </summary>
        void CharacterLoadComplete()
        {
            totalDecoLoad--;

            CheckLoadComplete();
        }


        /// <summary>
        /// 캐릭터 생성 완료
        /// </summary>
        void CharacterInstantComplete()
        {
            if(controlModel != null)
            {
                controlModel.PlayLobbyAnimation(controlModel.motionLists[0]);

                for (int i = 0; i < controlModel.motionLists.Count; i++)
                {
                    // 생성되고 난 후, 기본 모션을 세팅한다
                    if (controlModel.motionLists[i].Contains("기본") && !controlModel.motionLists[i].Contains("M"))
                    {
                        controlModel.PlayLobbyAnimation(controlModel.motionLists[i]);
                        break;
                    }
                }
            }

            // 화면에 이미 스탠딩이 있다면 SortingOrder 값을 증가시켜준다.
            if (liveModels.Count > 1)
                controlModel.model.GetComponent<Live2D.Cubism.Rendering.CubismRenderController>().SortingOrder += 1200;

            controlModel.ChangeLayerRecursively(controlModel.transform, GameConst.LAYER_MODEL_L);
        }

        /// <summary>
        /// 스탠딩 캐릭터 선택
        /// </summary>
        void SelectLiveCharacter(JsonData __j, ProfileItemElement standingElement)
        {
            // 신규 생성인지, 기존 선택인지 분별해주자
            // 신규 생성인 경우
            if(standingElement.currentCount < 1)
            {
                // 화면에는 최대 2명의 캐릭터만 세울 수 있다
                if (liveModels.Count >= 2)
                {
                    SystemManager.ShowMessageWithLocalize("6257");
                    return;
                }

                standingElement.currentCount++;

                // 캐릭터 생성
                ScriptModelMount character = new ScriptModelMount(SystemManager.GetJsonNodeString(__j, GameConst.COL_MODEL_NAME), CharacterInstantComplete, LobbyManager.main);
                character.SetModelDataFromStoryManager(true);

                if (SystemManager.main.hasSafeArea)
                    character.modelController.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_SAFEAREA_POS_Y + 1, 0);
                else
                    character.modelController.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_ORIGIN_POS_Y + 1, 0);

                character.modelController.currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
                liveModels.Add(character.modelController);
                decoObjects.Add(character.modelController.gameObject);

                controlModel = character.modelController;
            }
            else
            {
                // 생성이 아니면 찾기
                foreach (GameModelCtrl model in liveModels)
                {
                    if (model.originModelName == SystemManager.GetJsonNodeString(__j, GameConst.COL_MODEL_NAME))
                    {
                        controlModel = model;
                        break;
                    }
                }

                controlModel.ChangeLayerRecursively(controlModel.transform, GameConst.LAYER_MODEL_L);
            }

            usageStandingControl.SetActive(true);

            standingListScroll.SetActive(false);
            standingController.SetActive(true);

            moveCharacter = true;

            ActiveInteractable(false);
        }


        /// <summary>
        /// 제어중인 스탠딩 뒤집기
        /// </summary>
        public void OnClickFlipStanding()
        {
            if (controlModel.transform.localScale.x < 0)
                controlModel.transform.localScale = Vector3.one;
            else
                controlModel.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        /// <summary>
        /// 제어중인 스탠딩 삭제(파괴)
        /// </summary>
        public void OnClickDeleteStanding()
        {
            for (int i = 0; i < standingListContent.childCount; i++)
            {
                if (standingListContent.GetChild(i).GetComponent<ProfileItemElement>().modelName == controlModel.originModelName)
                {
                    standingListContent.GetChild(i).GetComponent<ProfileItemElement>().currentCount = 0;
                    break;
                }
            }

            // 리스트에서 해당 오브젝트 삭제
            decoObjects.Remove(controlModel.gameObject);
            liveModels.Remove(controlModel);

            // 화면에 남은 모델의 SortingOrder를 0으로 만들어준다
            foreach(GameModelCtrl model in liveModels)
                model.model.GetComponent<Live2D.Cubism.Rendering.CubismRenderController>().SortingOrder = 0;

            // 파괴
            Destroy(controlModel.gameObject);
            ReturnCharacterList();
        }


        public void OnClickFixPosition()
        {
            if (controlModel != null)
                controlModel.ChangeLayerRecursively(controlModel.transform, GameConst.LAYER_MODEL_C);

            ReturnCharacterList();
        }

        /// <summary>
        /// 스탠딩 선택으로 돌아오기
        /// </summary>
        void ReturnCharacterList()
        {
            moveCharacter = false;

            StandingListSort();
            ActiveInteractable(true);

            usageStandingControl.SetActive(false);
            standingListScroll.SetActive(true);
            standingController.SetActive(false);

            controlModel = null;
        }


        #endregion

        #region 스티커 관련

        void StickerLoadComplete()
        {
            if (totalDecoLoad > 0)
                totalDecoLoad--;

            CheckLoadComplete();
        }

        void CreateStickerElement(JsonData __j, ProfileItemElement stickerListElement)
        {
            StickerElement sticker = Instantiate(stickerObjectPrefab, stickerParent).GetComponent<StickerElement>();
            sticker.CreateSticker(__j, stickerListElement);
            decoObjects.Add(sticker.gameObject);
        }

        /// <summary>
        /// 스티커가 움직일 수 있게 raycaster 활성화/비활성화
        /// </summary>
        /// <param name="__interactable"></param>
        void ActiveInteractable(bool __interactable)
        {
            for (int i = 0; i < stickerParent.childCount; i++)
                stickerParent.GetChild(i).GetComponent<Image>().raycastTarget = __interactable;
        }


        #endregion

        /// <summary>
        /// 작품 꾸미기한거 저장하기
        /// </summary>
        public void OnClickSaveDeco()
        {
            NetworkLoader.main.SendPost(CallbackSaveDeco, SaveCurrentDeco(), true);
        }

        void CallbackSaveDeco(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackSaveDeco");
                return;
            }

            storyProfile = JsonMapper.ToObject(res.DataAsText);
            Debug.Log(JsonMapper.ToStringUnicode(storyProfile));
            UserManager.main.currentStoryJson["storyProfile"] = storyProfile;
            
            ActiveInteractable(false);
            DisableAllStickerOptionals();

            decoContainer.Hide();
            mainContainer.Show();

            Signal.Send(LobbyConst.STREAM_IFYOU, "decoSaveComplete", string.Empty);
        }

        #endregion


        /// <summary>
        /// 프로젝트 코인샵 터치 
        /// </summary>
        public void OnClickProjectCoinShop()
        {
            SystemManager.ShowSystemPopupLocalize("6260", OpenStoryCoinShop, null);
        }

        /// <summary>
        /// 해당 작품 코인샵 오픈
        /// </summary>
        void OpenStoryCoinShop()
        {
            NetworkLoader.main.SendPost(CallbackOpenCoinShop, SaveCurrentDeco());
        }

        void CallbackOpenCoinShop(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackOpenCoinShop");
                return;
            }

            if (string.IsNullOrEmpty(SystemManager.main.coinShopURL))
            {
                Debug.LogError("No Coinshop url");
                return;
            }

            UserManager.main.SaveCurrentAbilityDictionary(); // 코인샵 열기전에 현재 능력치 정보 저장해놓고 시작 

            if (Application.isEditor)
            {
                RefreshAfterProjectCoinShop();
                return;
            }


            string uidParam = string.Format("?uid={0}", UserManager.main.GetUserPinCode());
            string langParam = string.Format("&lang={0}", SystemManager.main.currentAppLanguageCode);
            string projectParam = string.Format("&project_id={0}", StoryManager.main.CurrentProjectID);

            string finalURL = SystemManager.main.coinShopURL + uidParam + langParam + projectParam;
            Debug.Log("Coinshop : " + finalURL);

            GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration = new GamebaseRequest.Webview.GamebaseWebViewConfiguration();
            configuration.title = SystemManager.GetLocalizedText("6186");
            configuration.orientation = GamebaseScreenOrientation.PORTRAIT;
            configuration.colorR = 0;
            configuration.colorG = 0;
            configuration.colorB = 0;
            configuration.colorA = 255;
            configuration.barHeight = 30;
            configuration.isBackButtonVisible = false;
            configuration.contentMode = GamebaseWebViewContentMode.MOBILE;


            Gamebase.Webview.ShowWebView(finalURL, configuration, (error) => {
                Debug.Log("Webview Closed");

                // 능력치 및 프로필 재화, 상단 갱신                
                RefreshAfterProjectCoinShop();

            }, null, null);
        }

        
        /// <summary>
        /// 코인샵 웹뷰 다녀와서 리프레시
        /// </summary>
        void RefreshAfterProjectCoinShop() {
           
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "getUserStoryProfileAndAbility"; // 통신에서 다 받아오기. 
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;

            NetworkLoader.main.SendPost(RefreshAfterProjectCoinShop, sending, true);            
        }
        
        /// <summary>
        /// RefreshAfterProjectCoinShop 콜백
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void RefreshAfterProjectCoinShop(HTTPRequest request, HTTPResponse response) {
            if (!NetworkLoader.CheckResponseValidation(request, response))
            {
                Debug.LogError("Failed RefreshAfterProjectCoinShop");
                return;
            }
            
            // 노드별로 처리
            JsonData result = JsonMapper.ToObject(response.DataAsText);

            SetDecoMode(result["profileCurrency"]); // 데코 모드 갱신
            
            UserManager.main.SetStoryAbilityDictionary(result["ability"]); // 능력치 갱신
            UserManager.main.SetBankInfo(result); // 상단 정보 갱신
            
            // * 갱신 다했으면 능력치 차이를 구해서 메세지 띄운다.
            UserManager.main.NotifyUpdatedAbility(); 
        }

        JsonData SaveCurrentDeco()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = LobbyConst.FUNC_SAVE_USER_STORY_PROFILE;
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            sending[LobbyConst.NODE_CURRENCY_LIST] = JsonMapper.ToObject("[]");

            int sortingOrder = 0;

            // 배경 추가
            JsonData bgData = new JsonData();
            bgData[LobbyConst.NODE_CURRENCY] = bgCurrency;
            bgData[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
            bgData[LobbyConst.NODE_POS_X] = LobbyManager.main.lobbyBackground.transform.localPosition.x;
            bgData[LobbyConst.NODE_POS_Y] = 0f;
            bgData[LobbyConst.NODE_WIDTH] = LobbyManager.main.lobbyBackground.transform.localScale.x;
            bgData[LobbyConst.NODE_HEIGHT] = LobbyManager.main.lobbyBackground.transform.localScale.x;
            bgData[LobbyConst.NODE_ANGLE] = 0f;

            sending[LobbyConst.NODE_CURRENCY_LIST].Add(bgData);

            sortingOrder++;

            // 캐릭터 추가
            foreach (GameModelCtrl character in liveModels)
            {
                JsonData modelData = new JsonData();
                modelData[LobbyConst.NODE_CURRENCY] = character.currencyName;
                modelData[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
                modelData[LobbyConst.NODE_POS_X] = character.transform.localPosition.x;
                modelData[LobbyConst.NODE_POS_Y] = 0f;
                modelData[LobbyConst.NODE_WIDTH] = 0f;
                modelData[LobbyConst.NODE_HEIGHT] = 0f;

                if (character.transform.localScale.x < 0)
                    modelData[LobbyConst.NODE_ANGLE] = 180f;
                else
                    modelData[LobbyConst.NODE_ANGLE] = 0f;

                sending[LobbyConst.NODE_CURRENCY_LIST].Add(modelData);

                sortingOrder++;
            }

            // 22.03.08 스티커만 추가
            for (int i = 0; i < stickerParent.childCount; i++)
            {
                sending[LobbyConst.NODE_CURRENCY_LIST].Add(stickerParent.GetChild(i).GetComponent<StickerElement>().StickerJsonData(sortingOrder));
                sortingOrder++;
            }

            return sending;
        }
    }
}