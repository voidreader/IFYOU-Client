﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using LitJson;
using BestHTTP;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewStoryLobby : CommonView
    {
        public static Action OnDecorateSet = null;
        public static Action OnDisableAllOptionals = null;
        public static Action<JsonData> OnSelectBackground = null;
        public static Action<JsonData> OnSelectCharacter = null;


        public static bool loadComplete = false;

        ScriptImageMount bg;
        string bgCurrency = string.Empty;
        public Transform characterParent;
        public Transform stickerParent;
        public GameObject stickerObjectPrefab;
        List<GameObject> createObjects = new List<GameObject>();
        public List<UIToggle> typeToggles;
        public UIContainer decoListContainer;
        List<GameModelCtrl> liveModels = new List<GameModelCtrl>();

        bool moveBg = false;
        bool moveCharacter = false;

        Vector2 cursor = Vector2.one;
        float originX = 0, startX = 0f, dragX = 0f, moveX = 0f;

        float movableWidth = 1f;
        float camWidth = 1f;

        public UIContainer mainContainer;
        public UIContainer decoContainer;
        JsonData storyProfile;              // 작품별 꾸미기 정보
        JsonData currencyLIst;              // 작품별 꾸미기 재화 리스트


        [Space(20)][Header("스탠딩 캐릭터 관련")]
        public GameObject standingListPrefab;
        public Transform standingListContent;
        GameModelCtrl controlModel;

        [Space][Header("배경 관련")]
        public GameObject bgListPrefab;
        public Transform bgListContent;
        public GameObject bgListScroll;
        public GameObject bgScrolling;

        
        [Space][Header("스티커 관련")]
        public GameObject stickerListPrefab;
        public Transform stickerListContent;
        

        


        int totalDecoLoad = 0;

        private void Awake()
        {
            OnDecorateSet = DecorateSetting;
            OnSelectBackground = SelectBackground;
            OnSelectCharacter = SelectLiveCharacter;
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

            // 스탠딩 캐릭터 기본 모션 세팅
            foreach (GameModelCtrl gm in liveModels)
            {
                gm.modelAnim.Play(gm.motionLists[0]);

                for(int i=0;i<gm.motionLists.Count;i++)
                {
                    if(gm.motionLists[i].Contains("기본") && !gm.motionLists[i].Contains("M"))
                    {
                        gm.modelAnim.CrossFade(gm.motionLists[i]);
                        break;
                    }
                }
            }

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty); 
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
        }

        public override void OnView()
        {
            base.OnView();

            
        }

        public override void OnHideView()
        {
            base.OnHideView();

            foreach (GameObject g in createObjects)
                Destroy(g);

            createObjects.Clear();
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

            // 작품에 포함된 재화정보를 넣어준다
            currencyLIst = JsonMapper.ToObject(res.DataAsText);

            // 리스트 재화 항목 생성
            CreateListObject(LobbyConst.NODE_WALLPAPER, bgListPrefab, bgListContent);
            CreateListObject(LobbyConst.NODE_STANDING, standingListPrefab, standingListContent);
            CreateListObject(LobbyConst.NODE_STICKER, stickerListPrefab, stickerListContent);

            // 화면의 활성화된 것과 리스트 재화와 연결




            mainContainer.Hide();
            decoContainer.Show();
        }


        #endregion



        #region 작품 꾸미기 모드 

        /// <summary>
        /// 작품 꾸미기 한거 세팅하기
        /// </summary>
        void DecorateSetting()
        {
            // 이 작업이 StoryLoading에서 이뤄져야해
            storyProfile = SystemManager.GetJsonNode(UserManager.main.currentStoryJson, "storyProfile");
            totalDecoLoad = storyProfile.Count;

            for (int i = 0; i < storyProfile.Count; i++)
            {
                switch (SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY_TYPE))
                {
                    case LobbyConst.NODE_WALLPAPER:
                        bg = new ScriptImageMount(GameConst.TEMPLATE_BACKGROUND, storyProfile[i], BGLoadComplete);
                        bgCurrency = SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY);
                        LobbyManager.main.lobbyBackground.transform.localPosition = new Vector3(SystemManager.GetJsonNodeInt(storyProfile[i], LobbyConst.NODE_POS_X), 0, 0);
                        break;

                    case LobbyConst.NODE_BADGE:
                    case LobbyConst.NODE_STICKER:
                        break;
                    case LobbyConst.NODE_STANDING:

                        // live2D는 LobbyManager를 부모로해서 만들고, 그냥 이미지는 StandingElement를 생성하자
                        if (SystemManager.GetJsonNodeInt(storyProfile[i], "model_id") < 0)
                        {

                            break;
                        }

                        ScriptModelMount character = new ScriptModelMount(SystemManager.GetJsonNodeString(storyProfile[i], GameConst.COL_MODEL_NAME), CharacterInstantComplete, LobbyManager.main);
                        character.SetModelDataFromStoryManager();

                        if (SystemManager.main.hasSafeArea)
                            character.modelController.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_SAFEAREA_POS_Y + 1, 0);
                        else
                            character.modelController.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_ORIGIN_POS_Y + 1, 0);

                        liveModels.Add(character.modelController);
                        createObjects.Add(character.modelController.gameObject);

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
            ProfileItemElement listElement = null;

            for (int i = 0; i < currencyLIst[key].Count; i++)
            {
                listElement = Instantiate(listObject, parent).GetComponent<ProfileItemElement>();
                listElement.InitCurrencyListElement(currencyLIst[key][i]);

                createObjects.Add(listElement.gameObject);
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
        void DisableAllStickerOptionals()
        {
            for (int i = 0; i < stickerParent.childCount; i++)
                stickerParent.GetChild(i).GetComponent<ItemElement>().DisableOptionals();
        }

        /// <summary>
        /// 하단의 버튼을 통해 스크롤 영역 보이게 하기
        /// </summary>
        public void OnClickShowDecoListContainer()
        {
            if (ToggleOnCheck() && decoListContainer.isHidden)
                decoListContainer.Show();
            else if (!ToggleOnCheck() && decoListContainer.isVisible)
                decoListContainer.Hide();
        }

        void CheckLoadComplete()
        {
            if (totalDecoLoad == 0)
                loadComplete = true;
        }

        public void OnDragScreen(InputAction.CallbackContext context)
        {
            cursor = context.ReadValue<Vector2>();

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


        #region 배경 관련


        void BGLoadComplete()
        {
            if (totalDecoLoad > 0)
                totalDecoLoad--;

            LobbyManager.main.lobbyBackground.sprite = null;
            LobbyManager.main.lobbyBackground.sprite = bg.sprite;

            movableWidth = Mathf.Abs(LobbyManager.main.lobbyBackground.size.x * LobbyManager.main.lobbyBackground.transform.localScale.x - camWidth) * 0.5f;

            CheckLoadComplete();
        }

        void SelectBackground(JsonData bgData)
        {
            bg = new ScriptImageMount(GameConst.TEMPLATE_BACKGROUND, bgData, BGLoadComplete);
            bgCurrency = SystemManager.GetJsonNodeString(bgData, LobbyConst.NODE_CURRENCY);
            LobbyManager.main.lobbyBackground.transform.localPosition = new Vector3(0f, 0f, 0f);

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
            for (int i = 0; i < stickerParent.childCount; i++)
                stickerParent.GetChild(i).GetComponent<Image>().raycastTarget = true;
        }

        #endregion

        #region 스탠딩 캐릭터 세팅

        /// <summary>
        /// 캐릭터 생성 완료
        /// </summary>
        void CharacterInstantComplete()
        {
            if (totalDecoLoad > 0)
                totalDecoLoad--;

            if(controlModel != null)
            {
                controlModel.modelAnim.Play(controlModel.motionLists[0]);

                for(int i=0;i<controlModel.motionLists.Count;i++)
                {
                    if(controlModel.motionLists[i].Contains("기본") && !controlModel.motionLists[i].Contains("M"))
                    {
                        controlModel.modelAnim.Play(controlModel.motionLists[i]);
                        break;
                    }
                }
            }

            CheckLoadComplete();
        }

        void SelectLiveCharacter(JsonData standingData)
        {
            // 화면에는 최대 2명의 캐릭터만 세울 수 있다
            if (liveModels.Count > 2)
                return;

            ScriptModelMount character = new ScriptModelMount(SystemManager.GetJsonNodeString(standingData, GameConst.COL_MODEL_NAME), CharacterInstantComplete, LobbyManager.main);
            character.SetModelDataFromStoryManager();

            if (SystemManager.main.hasSafeArea)
                character.modelController.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_SAFEAREA_POS_Y + 1, 0);
            else
                character.modelController.transform.localPosition = new Vector3(0, GameConst.MODEL_PARENT_ORIGIN_POS_Y + 1, 0);

            liveModels.Add(character.modelController);
            createObjects.Add(character.modelController.gameObject);

            controlModel = character.modelController;
            moveCharacter = true;
        }




        #endregion

        /// <summary>
        /// 작품 꾸미기한거 저장하기
        /// </summary>
        public void OnClickSaveDeco()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = LobbyConst.FUNC_SAVE_USER_STORY_PROFILE;
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            sending[LobbyConst.NODE_CURRENCY_LIST] = JsonMapper.ToObject("[]");

            NetworkLoader.main.SendPost(CallbackSaveDeco, sending, true);
        }

        void CallbackSaveDeco(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackSaveDeco");
                return;
            }

            storyProfile = JsonMapper.ToObject(res.DataAsText);

            decoContainer.Hide();
            mainContainer.Show();
        }

        #endregion
    }
}