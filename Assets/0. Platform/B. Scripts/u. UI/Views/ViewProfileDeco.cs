using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory
{
    public class ViewProfileDeco : CommonView
    {
        public static Action OnDisableAllOptionals = null;
        public static Action<JsonData, ProfileItemElement> OnBackgroundSetting = null;
        public static Action<JsonData, ProfileItemElement> OnBadgeSetting = null;
        public static Action<JsonData, ProfileItemElement> OnStickerSetting = null;
        public static Action<JsonData, ProfileItemElement> OnStandingSetting = null;
        public static Action<StandingElement> OnControlStanding = null;
        public static Action<string, string, string> OnProfileFrameSetting = null;
        public static Action<string, string, string> OnProfilePortraitSetting = null;

        public UIContainer listContentContainer;

        [Header("간단 프로필 관련")]
        public ImageRequireDownload profileFrame;
        public ImageRequireDownload profilePortrait;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI nicknameText;
        public TextMeshProUGUI expText;
        public Image expGauge;

        string frameName = string.Empty;
        string portraitName = string.Empty;

        public GameObject profileBriefContent;
        public Image profileBriefShowButton;
        public Sprite spriteVisable;
        public Sprite spriteInvisable;

        [Header("닉네임 변경 관련")]
        public TMP_InputField nicknameInputField;

        [Space][Header("배경 Tab")]
        public ImageRequireDownload background;     // 배경 이미지
        public MoveBackground moveBg;               
        public GameObject bgPrefab;                 // 배경 리스트에 들어가는 element
        public Transform bgElementListContent;      // 배경element가 들어갈 parent
        public GameObject profileBgScroll;          // 배경 리스트 scroll object
        public GameObject bgScrolling;              // 배경 선택 뒤, 배경 움직이라고 알려주는 새(?)페이지
        public GameObject noneBGItem;               // 보유 배경 재화가 없는 경우 표시

        [Space][Header("데코 Obect")]
        public Transform standingObjects;           // 스탠딩이 생성될 Parent(root)
        public Transform decoObjects;               // 뱃지, 스티커가 생성될 Parent(root)

        [Header("스탠딩 캐릭터 Tab")]
        public Transform standingElementListContent;
        public GameObject standingPrefab;           // 화면에 보여질 캐릭터 prefab
        public GameObject standingListPrefab;       // 목록 icon prefab
        public GameObject profileStandingScroll;
        public GameObject standingController;       // 스탠딩 캐릭터 배치 및 제어창
        public GameObject noneStandingItem;         // 보유 스탠딩 재화가 없는 경우 표시
        StandingElement controlStanding;            // 제어할 스탠딩 오브젝트
        JsonData controlStandingData;
        ProfileItemElement controlStandingItemElement;
        public UIToggle[] bottomToggles;
        public UIContainer bottomContainer;

        [Header("뱃지 Tab")]
        public GameObject badgeListPrefab;
        public Transform badgeElementListContent;
        public GameObject badgeScroll;
        public GameObject noneBadgeItem;

        [Header("스티커 Tab")]
        public GameObject stickerObjectPrefab;      // 화면에 세워질 sticker object prefab
        public GameObject itemListPrefab;           // 뱃지, 스티커 icon prefab
        public Transform stickerElementListContent; // 뱃지element가 들어갈 parent
        public GameObject stickerScroll;
        public GameObject noneStickerItem;          // 보유 스티커 재화가 없는 경우 표시


        [Header("프로필 테두리 Tab")]
        public GameObject frameListElement;
        public Transform frameListContent;

        [Header("프로필 초상화 Tab")]
        public GameObject portraitListElement;
        public Transform portraitListContent;


        [Space][Header("Text Tab")]
        public TextMeshProUGUI fontSizeText;        // 폰트 사이즈 설정 숫자
        public Image[] fontColorElements;
        public Transform textObjects;               // 텍스트 생성 위치(parent)
        public GameObject textObjectPrefab;         // InputField object
        Color fontColor = Color.magenta;

        JsonData profileCurrencyList;               // 사용자 보유 재화 리스트
        List<GameObject> createObject = new List<GameObject>();

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);

            profileCurrencyList = UserManager.main.userProfileCurrency;

            OnDisableAllOptionals = OnClickAllDisable;
            OnBackgroundSetting = ProfileBackgrounSetting;
            OnBadgeSetting = ProfileBadgeSetting;
            OnStickerSetting = ProfileStickerSetting;
            OnStandingSetting = ProfileStandingSetting;
            OnControlStanding = SelectControlStanding;
            OnProfileFrameSetting = SetProfileFrame;
            OnProfilePortraitSetting = SetProfilePortrait;
            

            #region 프로필 꾸미기모드 사용자 세팅

            // 사용자가 보유한 재화로 프로필 페이지 꾸며둔 정보("currency")
            JsonData profileCurrency = SystemManager.GetJsonNode(UserManager.main.userProfile, LobbyConst.NODE_CURRENCY);

            if (profileCurrency.Count > 0)
            {
                bool hasFrame = false;
                // 배경, 스탠딩, 스티커 
                for (int j = 0; j < profileCurrency.Count; j++)
                {
                    switch (SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY_TYPE))
                    {
                        case LobbyConst.NODE_WALLPAPER:
                            background.SetDownloadURL(SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY_KEY));
                            background.GetComponent<RectTransform>().anchoredPosition = new Vector2(float.Parse(SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_POS_X)), 0f);
                            background.GetComponent<RectTransform>().sizeDelta = new Vector2(float.Parse(SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_WIDTH)), float.Parse(SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_HEIGHT)));
                            moveBg.currencyName = SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY);
                            break;

                        case LobbyConst.NODE_BADGE:
                        case LobbyConst.NODE_STICKER:
                            ItemElement itemElement = Instantiate(stickerObjectPrefab, decoObjects).GetComponent<ItemElement>();
                            itemElement.SetProfileItem(profileCurrency[j]);
                            createObject.Add(itemElement.gameObject);
                            break;
                        case LobbyConst.NODE_STANDING:
                            StandingElement standingElement = Instantiate(standingPrefab, standingObjects).GetComponent<StandingElement>();
                            standingElement.SetProfileStanding(profileCurrency[j]);
                            createObject.Add(standingElement.gameObject);
                            break;

                        case LobbyConst.NODE_PORTRAIT:
                            profilePortrait.SetDownloadURL(SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY_KEY));
                            portraitName = SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY);

                            break;

                        case LobbyConst.NODE_FRAME:
                            profileFrame.SetDownloadURL(SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY_KEY));
                            frameName = SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY);
                            hasFrame = true;
                            break;
                    }
                }

                if (!hasFrame)
                    RemoveFrame();
            }

            levelText.text = string.Format("Lv. {0}", UserManager.main.level);
            int totalExp = SystemManager.main.GetLevelMaxExp((UserManager.main.level + 1).ToString());
            expGauge.fillAmount = (float)UserManager.main.exp / (float)totalExp;
            expText.text = string.Format("{0}/{1}", UserManager.main.exp, totalExp);
            profileBriefContent.SetActive(true);

            // 텍스트
            /*
            JsonData profileText = SystemManager.GetJsonNode(UserManager.main.userProfile, LobbyConst.NODE_TEXT);

            if (profileText.Count > 0)
            {
                for (int i = 0; i < profileText.Count; i++)
                {
                    DecoTextElement textElement = Instantiate(textObjectPrefab, textObjects).GetComponent<DecoTextElement>();
                    textElement.SetProfileText(profileText[i]);
                    createObject.Add(textElement.gameObject);
                }
            }
            */

            #endregion

            #region 사용자 보유 재화 리스트 세팅

            LoadBackgroundData();

            // 캐릭터 스탠딩
            LoadStandingData();

            // 뱃지
            LoadBadgeData();

            // 스티커
            LoadStickerData();

            // 테두리
            LoadFrameData();

            // 초상화
            LoadPortraitData();

            #endregion

            #region 사용자 세팅 element와 재화 element 연결

            bool outLoop = false;

            for (int i = 0; i < createObject.Count; i++)
            {
                if (createObject[i].GetComponent<ProfileItemElement>() == null)
                    continue;

                for (int j = 0; j < decoObjects.childCount; j++)
                {
                    if (decoObjects.GetChild(j).GetComponent<ItemElement>() != null && createObject[i].GetComponent<ProfileItemElement>().currencyName == decoObjects.GetChild(j).GetComponent<ItemElement>().currencyName)
                    {
                        decoObjects.GetChild(j).GetComponent<ItemElement>().profileDecoElement = createObject[i].GetComponent<ProfileItemElement>();
                        outLoop = true;
                        break;
                    }
                }

                if (outLoop)
                    continue;

                for (int j = 0; j < standingObjects.childCount; j++)
                {
                    if (standingObjects.GetChild(j).GetComponent<StandingElement>() != null && createObject[i].GetComponent<ProfileItemElement>().currencyName == standingObjects.GetChild(j).GetComponent<StandingElement>().currencyName)
                    {
                        standingObjects.GetChild(j).GetComponent<StandingElement>().profileItemElement = createObject[i].GetComponent<ProfileItemElement>();
                        outLoop = false;
                        break;
                    }
                }
            }


            #endregion

        }


        public override void OnView()
        {
            base.OnView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);

            AdManager.main.AnalyticsEnter("deco_enter");
        }


        public override void OnHideView()
        {
            base.OnHideView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);

            foreach (GameObject g in createObject)
                Destroy(g);

            createObject.Clear();

            // 혹시 데코, 텍스트 남아있는 오브젝트 삭제
            for (int i = 0; i < decoObjects.childCount; i++)
                Destroy(decoObjects.GetChild(i).gameObject);

            for (int i = 0; i < textObjects.childCount; i++)
                Destroy(textObjects.GetChild(i).gameObject);

            // 배경 제어 끄고
            profileBgScroll.SetActive(true);
            bgScrolling.SetActive(false);
            background.OnDownloadImage = null;

            // 스탠딩 제어 끄고
            profileStandingScroll.SetActive(true);
            standingController.SetActive(false);
            
        }

        /// <summary>
        /// 화면 위의 Optionals 모두 비활성화
        /// </summary>
        public void OnClickAllDisable()
        {
            for(int i=0;i<decoObjects.childCount;i++)
            {
                if (decoObjects.GetChild(i).GetComponent<ItemElement>() != null)
                    decoObjects.GetChild(i).GetComponent<ItemElement>().DisableOptionals();
            }

            for (int i = 0; i < textObjects.childCount; i++)
                textObjects.GetChild(i).GetComponent<DecoTextElement>().DisableOptional();
        }

        #region 배경 관련

        void LoadBackgroundData()
        {
            // 배경 재화를 보유하지 못한 경우
            if (!CheckListable(LobbyConst.NODE_WALLPAPER))
            {
                profileBgScroll.SetActive(false);
                noneBGItem.SetActive(true);
                return;
            }

            profileBgScroll.SetActive(true);
            noneBGItem.SetActive(false);

            CreateListObject(LobbyConst.NODE_WALLPAPER, bgPrefab, bgElementListContent);
        }

        void ProfileBackgrounSetting(JsonData __j, ProfileItemElement profileDeco)
        {
            background.OnDownloadImage = SetBackgroundLoading;
            SystemManager.ShowNetworkLoading();
            background.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY), true);
            background.transform.localPosition = Vector3.zero;
            
            moveBg.currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

            // 배경 위에 뭔가 있으면 화면 드래그가 안되니까 rayCast를 화면 제외하고 다 꺼주자
            for (int i = 0; i < decoObjects.childCount; i++)
                decoObjects.GetChild(i).GetComponent<Image>().raycastTarget = false;

            for (int i = 0; i < textObjects.childCount; i++)
                textObjects.GetChild(i).GetComponent<Image>().raycastTarget = false;

            for (int i = 0; i < standingObjects.childCount; i++)
                standingObjects.GetChild(i).GetComponent<Image>().raycastTarget = false;

            profileBgScroll.SetActive(false);
            bgScrolling.SetActive(true);
        }

        public void OnClickFixBackground()
        {
            profileBgScroll.SetActive(true);
            bgScrolling.SetActive(false);
            moveBg.enabled = false;

            // 배경 설정 끝냈으니까 다시 raycastTargtet을 켜주자
            for (int i = 0; i < decoObjects.childCount; i++)
                decoObjects.GetChild(i).GetComponent<Image>().raycastTarget = true;

            for (int i = 0; i < textObjects.childCount; i++)
                textObjects.GetChild(i).GetComponent<Image>().raycastTarget = true;

            for (int i = 0; i < standingObjects.childCount; i++)
                standingObjects.GetChild(i).GetComponent<Image>().raycastTarget = true;
        }

        #endregion

        #region 캐릭터 스탠딩 관련

        void LoadStandingData()
        {
            if(!CheckListable(LobbyConst.NODE_STANDING))
            {
                profileStandingScroll.SetActive(false);
                noneStandingItem.SetActive(true);
                return;
            }

            profileStandingScroll.SetActive(true);
            noneStandingItem.SetActive(false);

            CreateListObject(LobbyConst.NODE_STANDING, standingListPrefab, standingElementListContent);
        }

        /// <summary>
        /// 스탠딩 캐릭터 화면에 셋팅
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="profileStanding"></param>
        void ProfileStandingSetting(JsonData __j, ProfileItemElement profileStanding)
        {
            if (standingObjects.childCount >= 3)
                return;

            profileStandingScroll.SetActive(false);
            standingController.SetActive(true);

            controlStandingData = __j;
            controlStandingItemElement = profileStanding;
            controlStanding = Instantiate(standingPrefab, standingObjects).GetComponent<StandingElement>();
            controlStanding.NewStanding(controlStandingData, controlStandingItemElement);
            controlStanding.standingRect.anchoredPosition = Vector2.zero;
            controlStanding.selected = true;
            createObject.Add(controlStanding.gameObject);
            BlockExcludeStandingControl();
        }

        /// <summary>
        /// 스탠딩 캐릭터 선택시의 액션
        /// </summary>
        void SelectControlStanding(StandingElement standing)
        {
            // 이미 제어중인 경우 return
            if (standingController.activeSelf)
                return;

            controlStanding = standing;
            controlStanding.selected = true;

            for (int i = 0; i < bottomToggles.Length; i++)
            {
                if (i == 1)
                    bottomToggles[i].isOn = true;
                else
                    bottomToggles[i].isOn = false;
            }

            if (bottomContainer.isHidden)
                bottomContainer.Show();

            profileStandingScroll.SetActive(false);
            standingController.SetActive(true);
            BlockExcludeStandingControl();
        }


        public void OnClickPositionFlip()
        {
            if (controlStanding != null)
            {
                if (controlStanding.standingRect.localScale.x == 1)
                    controlStanding.standingRect.localScale = new Vector3(-1f, 1f, 1f);
                else
                    controlStanding.standingRect.localScale = Vector3.one;
            }
        }

        public void OnClickDeleteStanding()
        {
            if (controlStanding == null)
                return;

            for (int i = 0; i < createObject.Count; i++)
            {
                if (createObject[i] == controlStanding.gameObject)
                {
                    createObject.Remove(createObject[i]);
                    break;
                }
            }

            controlStanding.RemoveFromScreen();
            Destroy(controlStanding.gameObject);
            profileStandingScroll.SetActive(true);
            standingController.SetActive(false);

            for (int i = 0; i < decoObjects.childCount; i++)
                decoObjects.GetChild(i).GetComponent<Image>().raycastTarget = true;

            for (int i = 0; i < standingObjects.childCount; i++)
                standingObjects.GetChild(i).GetComponent<Image>().raycastTarget = true;
        }

        public void OnClickStandingFix()
        {
            controlStanding.SetRectTransInfo();
            controlStanding.selected = false;
            controlStanding = null;

            profileStandingScroll.SetActive(true);
            standingController.SetActive(false);

            for (int i = 0; i < decoObjects.childCount; i++)
                decoObjects.GetChild(i).GetComponent<Image>().raycastTarget = true;

            for (int i = 0; i < standingObjects.childCount; i++)
                standingObjects.GetChild(i).GetComponent<Image>().raycastTarget = true;
        }

        /// <summary>
        /// 스탠딩 캐릭터 움직이는 동안 클릭되는 것 막기
        /// </summary>
        void BlockExcludeStandingControl()
        {
            for (int i = 0; i < decoObjects.childCount; i++)
                decoObjects.GetChild(i).GetComponent<Image>().raycastTarget = false;

            for(int i=0;i<standingObjects.childCount;i++)
            {
                if (standingObjects.GetChild(i).GetComponent<StandingElement>() != controlStanding)
                    standingObjects.GetChild(i).GetComponent<Image>().raycastTarget = false;
            }
        }

        #endregion

        #region 뱃지

        void LoadBadgeData()
        {
            if (!CheckListable(LobbyConst.NODE_BADGE))
            {
                badgeScroll.SetActive(false);
                noneBadgeItem.SetActive(true);
                return;
            }

            badgeScroll.SetActive(true);
            noneBadgeItem.SetActive(false);

            CreateListObject(LobbyConst.NODE_BADGE, badgeListPrefab, badgeElementListContent);
        }

        void ProfileBadgeSetting(JsonData __j, ProfileItemElement profileDeco)
        {
            ItemElement badgeElement = Instantiate(stickerObjectPrefab, decoObjects).GetComponent<ItemElement>();
            badgeElement.NewProfileItem(__j, profileDeco);
            createObject.Add(badgeElement.gameObject);
        }

        #endregion

        #region 스티커 관련

        void LoadStickerData()
        {
            if (!CheckListable(LobbyConst.NODE_STICKER))
            {
                stickerScroll.SetActive(false);
                noneStickerItem.SetActive(true);
                return;
            }

            stickerScroll.SetActive(true);
            noneStickerItem.SetActive(false);

            CreateListObject(LobbyConst.NODE_STICKER, itemListPrefab, stickerElementListContent);
        }

        void ProfileStickerSetting(JsonData __j, ProfileItemElement profileDeco)
        {
            ItemElement stickerElement = Instantiate(stickerObjectPrefab, decoObjects).GetComponent<ItemElement>();
            stickerElement.NewProfileItem(__j, profileDeco);
            createObject.Add(stickerElement.gameObject);
        }

        #endregion


        #region 프로필사진 테두리 관련

        void LoadFrameData()
        {
            if (!CheckListable(LobbyConst.NODE_FRAME))
                return;

            CreateListObject(LobbyConst.NODE_FRAME, frameListElement, frameListContent);
        }

        public void RemoveFrame()
        {
            for (int i = 1; i < frameListContent.childCount; i++)
                frameListContent.GetChild(i).GetComponent<ProfileItemElement>().useCheckIcon.SetActive(false);

            profileFrame.SetTexture2D(LobbyManager.main.textureNoneFrame);
            frameName = string.Empty;
        }

        void SetProfileFrame(string __url, string __key, string __currency)
        {
            for (int i = 1; i < frameListContent.childCount; i++)
                frameListContent.GetChild(i).GetComponent<ProfileItemElement>().useCheckIcon.SetActive(false);

            profileFrame.SetDownloadURL(__url, __key);
            frameName = __currency;
        }

        #endregion


        #region 프로필 초상화 관련

        void LoadPortraitData()
        {
            if (!CheckListable(LobbyConst.NODE_PORTRAIT))
                return;

            CreateListObject(LobbyConst.NODE_PORTRAIT, portraitListElement, portraitListContent);
        }

        void SetProfilePortrait(string __url, string __key, string __currency)
        {
            for (int i = 0; i < portraitListContent.childCount; i++)
                portraitListContent.GetChild(i).GetComponent<ProfileItemElement>().useCheckIcon.SetActive(false);

            profilePortrait.SetDownloadURL(__url, __key);
            portraitName = __currency;
        }

        #endregion

        #region 텍스트 관련

        /// <summary>
        /// 선택한 폰트의 색상값 반환
        /// </summary>
        /// <param name="index"></param>
        public void OnClickGetColor(int index)
        {
            fontColor = fontColorElements[index].color;
        }

        /// <summary>
        /// 폰트 사이즈 감소
        /// </summary>
        public void OnClickDecreaseSize()
        {
            int fontSize = int.Parse(fontSizeText.text);

            if (fontSize > 20)
                fontSize--;

            fontSizeText.text = fontSize.ToString();
        }

        /// <summary>
        /// 폰트 사이즈 증가
        /// </summary>
        public void OnClickIncreaseSize()
        {
            int fontSize = int.Parse(fontSizeText.text);

            if (fontSize < 36)
                fontSize++;

            fontSizeText.text = fontSize.ToString();
        }

        /// <summary>
        /// 텍스트 생성
        /// </summary>
        public void CreateTextObject()
        {
            // font color를 선택하지 않은 경우 첫번째껄 default로 설정해서 준다
            if (fontColor == Color.magenta)
                fontColor = fontColorElements[0].color;

            DecoTextElement textElement = Instantiate(textObjectPrefab, textObjects).GetComponent<DecoTextElement>();
            textElement.NewTextProfile(fontColor, int.Parse(fontSizeText.text));
            createObject.Add(textElement.gameObject);

            fontColor = Color.magenta;
        }

        #endregion


        #region OnClickEvent
        
        /// <summary>
        /// 간단 프로필(프로필 초상화, 테두리 그룹 보이기/숨기기)
        /// </summary>
        public void OnClickShowProfileBrief() 
        {
            if (profileBriefContent.activeSelf)
            {
                profileBriefContent.SetActive(false);
                profileBriefShowButton.sprite = spriteInvisable;
            }
            else
            {
                profileBriefContent.SetActive(true);
                profileBriefShowButton.sprite = spriteVisable;
            }
        }


        /// <summary>
        /// 하단의 버튼을 통해 스크롤 영역 보이게 하기
        /// </summary>
        public void OnClickShowContent()
        {
            if (ToggleOnCheck() && bottomContainer.isHidden)
                bottomContainer.Show();
            else if (!ToggleOnCheck() && bottomContainer.isVisible)
                bottomContainer.Hide();
        }


        /// <summary>
        /// 꾸미기 오브젝트(캐릭터, 뱃지, 스티커) 모두 삭제
        /// </summary>
        public void OnClickDeleteAllDecoObject()
        {
            for (int i = 0; i < decoObjects.childCount; i++)
                Destroy(decoObjects.GetChild(i).gameObject);

            for (int i = 0; i < textObjects.childCount; i++)
                Destroy(textObjects.GetChild(i).gameObject);

            for (int i = 0; i < standingObjects.childCount; i++)
                Destroy(standingObjects.GetChild(i).gameObject);

            // 스탠딩 갯수 초기화
            for (int i = 0; i < standingElementListContent.childCount; i++)
                standingElementListContent.GetChild(i).GetComponent<ProfileItemElement>().currentCount = 0;

            // 뱃지 갯수 초기화
            for (int i = 0; i < badgeElementListContent.childCount; i++)
                badgeElementListContent.GetChild(i).GetComponent<ProfileItemElement>().currentCount = 0;

            // 스티커 갯수 초기화
            for (int i = 0; i < stickerElementListContent.childCount; i++)
            {
                stickerElementListContent.GetChild(i).GetComponent<ProfileItemElement>().currentCount = 0;
                stickerElementListContent.GetChild(i).GetComponent<ProfileItemElement>().SetCountText();
            }
        }


        /// <summary>
        /// 닉네임 표기 일단 변경
        /// </summary>
        public void OnClickSubmitNIckname()
        {
            nicknameText.text = nicknameInputField.text;
        }


        /// <summary>
        /// 꾸미기 한거 저장하는데 닉네임 먼저 저장함
        /// </summary>
        public void OnCllickSave()
        {
            // 닉네임 체크를 먼저 한다
            // 닉네임이 공백인가?
            if(string.IsNullOrEmpty(nicknameText.text))
            {
                SystemManager.ShowMessageWithLocalize("6199", false);
                return;
            }

            // 닉네임 변경을 하지 않았다면 꾸미기 저장 바로~
            if(nicknameText.text == UserManager.main.nickname)
            {
                SaveDeco();
                return;
            }

            JsonData nickname = new JsonData();
            nickname[CommonConst.FUNC] = "updateUserNickname";
            nickname["nickname"] = nicknameText.text;

            NetworkLoader.main.SendPost(OnUpdateNickname, nickname);
        }
        

        void OnUpdateNickname(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                try
                {
                    Debug.Log("Failed OnUpdateNickname : " + res.DataAsText);
                }
                catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                }
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);
            UserManager.main.SetNewNickname(result["nickname"].ToString());

            // 닉네임 걸릴 것 없으니 프로필 꾸민거 저장하자!
            SaveDeco();
        }

        /// <summary>
        /// 프로필 꾸미기 저장하기
        /// </summary>
        void SaveDeco()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = LobbyConst.FUNC_USER_PROFILE_SAVE;
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending["kind"] = "deco";
            sending[LobbyConst.NODE_CURRENCY_LIST] = JsonMapper.ToObject("[]");

            int sortingOrder = 0;

            if (!string.IsNullOrEmpty(moveBg.currencyName))
            {
                // 배경 먼저 넣는다. 배경은 무조건 sortingOrder값을 0으로 가져간다
                JsonData bgData = new JsonData();
                bgData[LobbyConst.NODE_CURRENCY] = moveBg.currencyName;
                bgData[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
                bgData[LobbyConst.NODE_POS_X] = moveBg.bgObject.anchoredPosition.x;
                bgData[LobbyConst.NODE_POS_Y] = moveBg.bgObject.anchoredPosition.y;
                bgData[LobbyConst.NODE_WIDTH] = moveBg.bgObject.sizeDelta.x;
                bgData[LobbyConst.NODE_HEIGHT] = moveBg.bgObject.sizeDelta.y;
                bgData[LobbyConst.NODE_ANGLE] = 0;

                sending[LobbyConst.NODE_CURRENCY_LIST].Add(bgData);
            }

            // 스탠딩을 먼저 넣어준다
            for (int i = 0; i < standingObjects.childCount; i++)
            {
                sortingOrder++;
                sending[LobbyConst.NODE_CURRENCY_LIST].Add(standingObjects.GetChild(i).GetComponent<StandingElement>().SaveStandingData(sortingOrder));
            }


            // 이제 화면에 들어간 뱃지, 스티커를 넣어준다
            for (int i = 0; i < decoObjects.childCount; i++)
            {
                sortingOrder++;
                sending[LobbyConst.NODE_CURRENCY_LIST].Add(decoObjects.GetChild(i).GetComponent<ItemElement>().SaveJsonData(sortingOrder));
            }

            // frame 넣기
            JsonData frameData = new JsonData();
            frameData[LobbyConst.NODE_CURRENCY] = frameName;
            frameData[LobbyConst.NODE_SORTING_ORDER] = 0;
            frameData[LobbyConst.NODE_POS_X] = 0f;
            frameData[LobbyConst.NODE_POS_Y] = 0f;
            frameData[LobbyConst.NODE_WIDTH] = profileFrame.downloadedTexture.width;
            frameData[LobbyConst.NODE_HEIGHT] = profileFrame.downloadedTexture.height;
            frameData[LobbyConst.NODE_ANGLE] = 0f;

            sending[LobbyConst.NODE_CURRENCY_LIST].Add(frameData);

            JsonData portraitData = new JsonData();
            portraitData[LobbyConst.NODE_CURRENCY] = portraitName;
            portraitData[LobbyConst.NODE_SORTING_ORDER] = 0;
            portraitData[LobbyConst.NODE_POS_X] = 0f;
            portraitData[LobbyConst.NODE_POS_Y] = 0f;
            portraitData[LobbyConst.NODE_WIDTH] = profilePortrait.downloadedSprite.rect.width;
            portraitData[LobbyConst.NODE_HEIGHT] = profilePortrait.downloadedSprite.rect.height;
            portraitData[LobbyConst.NODE_ANGLE] = 0f;

            sending[LobbyConst.NODE_CURRENCY_LIST].Add(portraitData);

            // text list를 만들어서 생성한 텍스트 값을 Json화해서 넣는다.
            /*
            sending[LobbyConst.NODE_TEXT_LIST] = JsonMapper.ToObject("[]");

            for (int i = 0; i < textObjects.childCount; i++)
                sending[LobbyConst.NODE_TEXT_LIST].Add(textObjects.GetChild(i).GetComponent<DecoTextElement>().SaveJsonData(i));

            */

            NetworkLoader.main.SendPost(CallbackSaveDeco, sending, true);
        }

        void CallbackSaveDeco(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackSaveDeco");
                return;
            }

            // 저장이 완료되면 다시 ViewMain으로 간다
            UserManager.main.userProfile = JsonMapper.ToObject(res.DataAsText);
            Debug.Log(JsonMapper.ToStringUnicode(UserManager.main.userProfile));
            ViewMain.OnProfileSetting?.Invoke();
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SAVE_PROFILE_DECO);
            SystemManager.ShowSimpleAlertLocalize("6122");
        }

        #endregion

        void SetBackgroundLoading()
        {
            SystemManager.HideNetworkLoading();

            // 높이가 1200 미만이면 비율 2배수하기
            if (background.GetComponent<RectTransform>().sizeDelta.y < 1200)
                background.GetComponent<RectTransform>().sizeDelta *= 2f;

            moveBg.enabled = true;
        }

        /// <summary>
        /// 리스트를 생성할 수 있는가?
        /// </summary>
        /// <param name="key">json key값</param>
        bool CheckListable(string key)
        {
            if (!profileCurrencyList.ContainsKey(key))
                return false;

            if (profileCurrencyList[key] == null)
                return false;

            return true;
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

            for (int i = 0; i < profileCurrencyList[key].Count; i++)
            {
                listElement = Instantiate(listObject, parent).GetComponent<ProfileItemElement>();
                listElement.InitCurrencyListElement(profileCurrencyList[key][i]);

                createObject.Add(listElement.gameObject);
            }
        }

        /// <summary>
        /// Toggle이 한개라도 On인지 체크
        /// </summary>
        /// <returns></returns>
        bool ToggleOnCheck()
        {
            for (int i = 0; i < bottomToggles.Length; i++)
            {
                if (bottomToggles[i].isOn)
                    return true;
            }

            return false;
        }
    }
}