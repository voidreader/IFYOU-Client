using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;
using BestHTTP;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewStoryLobby : CommonView
    {

        public ImageRequireDownload background;
        string bgCurrencyName = string.Empty;
        public Transform characterParent;
        public Transform stickerParent;
        public GameObject stickerObjectPrefab;
        List<GameObject> createObjects = new List<GameObject>();
        public List<UIToggle> typeToggles;
        public UIContainer decoListContainer;

        public UIContainer mainContainer;
        public UIContainer decoContainer;
        JsonData storyProfile;              // 작품별 꾸미기 정보
        JsonData currencyLIst;              // 작품별 꾸미기 재화 리스트

        [Space(20)][Header("작품별 꾸미기 모드")]
        public GameObject bgListPrefab;
        public Transform bgListContent;
        public GameObject badgeListPrefab;
        public Transform badgeListContent;
        public GameObject stickerListPrefab;
        public Transform stickerListContent;
        public GameObject standingListPrefab;
        public Transform standingListContent;


        public override void OnStartView()
        {
            base.OnStartView();

            // 작품 진입시 저장한 작품별 꾸미기 정보 받기
            storyProfile = SystemManager.GetJsonNode(UserManager.main.currentStoryJson, "storyProfile");

            for (int i = 0; i < storyProfile.Count; i++)
            {
                switch (SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY_TYPE))
                {
                    case LobbyConst.NODE_WALLPAPER:
                        background.SetDownloadURL(SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY_KEY));
                        background.GetComponent<RectTransform>().sizeDelta = new Vector2(SystemManager.GetJsonNodeFloat(storyProfile[i], LobbyConst.NODE_WIDTH), SystemManager.GetJsonNodeFloat(storyProfile[i], LobbyConst.NODE_HEIGHT));
                        background.GetComponent<RectTransform>().anchoredPosition = new Vector2(SystemManager.GetJsonNodeFloat(storyProfile[i], LobbyConst.NODE_POS_X), SystemManager.GetJsonNodeFloat(storyProfile[i], LobbyConst.NODE_POS_Y));
                        bgCurrencyName = SystemManager.GetJsonNodeString(storyProfile[i], LobbyConst.NODE_CURRENCY);
                        break;

                    case LobbyConst.NODE_BADGE:
                    case LobbyConst.NODE_STICKER:
                        break;
                    case LobbyConst.NODE_STANDING:
                        break;
                }
            }

            // 캐릭터를 일단 Layer를 Model-C로 해서 넣자

        }

        public override void OnView()
        {
            base.OnView();

            SystemManager.HideNetworkLoading();
        }

        public override void OnHideView()
        {
            base.OnHideView();



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

            CreateListObject(LobbyConst.NODE_WALLPAPER, bgListPrefab, bgListContent);
            CreateListObject(LobbyConst.NODE_STANDING, standingListPrefab, standingListContent);
            CreateListObject(LobbyConst.NODE_BADGE, badgeListPrefab, badgeListContent);
            CreateListObject(LobbyConst.NODE_STICKER, stickerListPrefab, stickerListContent);

            mainContainer.Hide();
            decoContainer.Show();
        }


        #endregion



        #region 작품 꾸미기 모드 

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
        /// 하단의 버튼을 통해 스크롤 영역 보이게 하기
        /// </summary>
        public void OnClickShowDecoListContainer()
        {
            if (ToggleOnCheck() && decoListContainer.isHidden)
                decoListContainer.Show();
            else if (!ToggleOnCheck() && decoListContainer.isVisible)
                decoListContainer.Hide();
        }


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