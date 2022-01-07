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
        public static Action<JsonData, ProfileItemElement> OnStandingSetting = null;
        public static Action<StandingElement> OnControlStanding = null;

        [Header("배경 Tab")]
        public ImageRequireDownload background;     // 배경 이미지
        public MoveBackground moveBg;               
        public GameObject bgPrefab;                 // 배경 리스트에 들어가는 element
        public Transform bgElementListContent;      // 배경element가 들어갈 parent
        public GameObject profileBgScroll;          // 배경 리스트 scroll object
        public GameObject bgScrolling;              // 배경 선택 뒤, 배경 움직이라고 알려주는 새(?)페이지
        public GameObject noneBGItem;               // 보유 배경 재화가 없는 경우 표시

        [Space][Header("데코 Obect")]
        public Transform decoObjects;               // 스탠딩, 뱃지, 말풍선 등이 생성될 Parent(root)

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
        StandingElement[] screenStand = new StandingElement[3];        // 화면에 서 있는 스탠딩
        public UIToggle[] bottomToggles;
        public UIContainer bottomContainer;

        [Header("뱃지 Tab")]
        public GameObject badgeObjectPrefab;
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

            ViewCommonTop.OnClickButtonAction = OnCllickSaveDeco;

            profileCurrencyList = UserManager.main.userProfileCurrency;

            OnDisableAllOptionals = OnClickAllDisable;
            OnBackgroundSetting = ProfileBackgrounSetting;
            OnBadgeSetting = ProfileBadgeSetting;
            OnStandingSetting = ProfileStandingSetting;
            OnControlStanding = SelectControlStanding;

            #region 프로필 꾸미기모드 사용자 세팅

            // 사용자가 보유한 재화로 프로필 페이지 꾸며둔 정보("currency")
            JsonData profileCurrency = SystemManager.GetJsonNode(UserManager.main.userProfile, LobbyConst.NODE_CURRENCY);

            if (profileCurrency.Count > 0)
            {
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
                            StandingElement standingElement = Instantiate(standingPrefab, decoObjects).GetComponent<StandingElement>();
                            standingElement.SetProfileStanding(profileCurrency[j]);

                            // 위치에 따라서 배열에 넣는 위치도 다름
                            if (standingElement.standingRect.anchoredPosition.x < 0)
                                screenStand[0] = standingElement;
                            else if (standingElement.standingRect.anchoredPosition.x == 0)
                                screenStand[1] = standingElement;
                            else
                                screenStand[2] = standingElement;

                            createObject.Add(standingElement.gameObject);
                            break;
                    }
                }
            }
            

            // 텍스트
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

            #endregion

            #region 사용자 보유 재화 리스트 세팅

            LoadBackgroundData();

            // 캐릭터 스탠딩
            LoadStandingData();

            // 뱃지


            // 스티커
            LoadStickerData();

            // 말풍선


            #endregion

            #region 사용자 세팅 element와 재화 element 연결

            
            for (int i = 0; i < createObject.Count; i++)
            {
                if (createObject[i].GetComponent<ProfileItemElement>() == null)
                    continue;

                for (int j = 0; j < decoObjects.childCount; j++)
                {
                    if (decoObjects.GetChild(j).GetComponent<ItemElement>() != null && createObject[i].GetComponent<ProfileItemElement>().currencyName == decoObjects.GetChild(j).GetComponent<ItemElement>().currencyName)
                    {
                        decoObjects.GetChild(j).GetComponent<ItemElement>().profileDecoElement = createObject[i].GetComponent<ProfileItemElement>();
                        break;
                    }
                    else if (decoObjects.GetChild(j).GetComponent<StandingElement>() != null && createObject[i].GetComponent<ProfileItemElement>().currencyName == decoObjects.GetChild(j).GetComponent<StandingElement>().currencyName)
                    {
                        decoObjects.GetChild(j).GetComponent<StandingElement>().profileItemElement = createObject[i].GetComponent<ProfileItemElement>();
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
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_MULTIPLE_BUTTON_LABEL, SystemManager.GetLocalizedText("6097"), string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5124"), string.Empty);

        }


        public override void OnHideView()
        {
            base.OnHideView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);

            for (int i = 0; i < decoObjects.childCount; i++)
                Destroy(decoObjects.GetChild(i).gameObject);

            for (int i = 0; i < textObjects.childCount; i++)
                Destroy(textObjects.GetChild(i).gameObject);

            createObject.Clear();
            
            // 배경 제어 끄고
            profileBgScroll.SetActive(true);
            bgScrolling.SetActive(false);

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
            if (!profileCurrencyList.ContainsKey(LobbyConst.NODE_WALLPAPER) || profileCurrencyList[LobbyConst.NODE_WALLPAPER] == null)
            {
                profileBgScroll.SetActive(false);
                noneBGItem.SetActive(true);
                return;
            }

            profileBgScroll.SetActive(true);
            noneBGItem.SetActive(false);

            ProfileItemElement listElement = null;

            for (int i = 0; i < profileCurrencyList[LobbyConst.NODE_WALLPAPER].Count; i++)
            {
                listElement = Instantiate(bgPrefab, bgElementListContent).GetComponent<ProfileItemElement>();
                listElement.InitCurrencyListElement(profileCurrencyList[LobbyConst.NODE_WALLPAPER][i]);

                createObject.Add(listElement.gameObject);
            }
        }


        public void OnClickDeleteBackground()
        {
            background.SetDownloadURL("", "");
            moveBg.currencyName = string.Empty;
        }


        void ProfileBackgrounSetting(JsonData __j, ProfileItemElement profileDeco)
        {
            background.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY));
            background.transform.localPosition = Vector3.zero;
            moveBg.enabled = true;
            moveBg.currencyName = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            profileBgScroll.SetActive(false);
            bgScrolling.SetActive(true);
        }

        public void OnClickFixBackground()
        {
            profileBgScroll.SetActive(true);
            bgScrolling.SetActive(false);
            moveBg.enabled = false;
        }

        #endregion

        #region 캐릭터 스탠딩 관련

        void LoadStandingData()
        {
            if(!profileCurrencyList.ContainsKey(LobbyConst.NODE_STANDING) || profileCurrencyList[LobbyConst.NODE_STANDING] == null)
            {
                profileStandingScroll.SetActive(false);
                noneStandingItem.SetActive(true);
                return;
            }

            profileStandingScroll.SetActive(true);
            noneStandingItem.SetActive(false);

            ProfileItemElement listElement = null;

            for(int i=0;i<profileCurrencyList[LobbyConst.NODE_STANDING].Count;i++)
            {
                listElement = Instantiate(standingListPrefab, standingElementListContent).GetComponent<ProfileItemElement>();
                listElement.InitCurrencyListElement(profileCurrencyList[LobbyConst.NODE_STANDING][i]);
                createObject.Add(listElement.gameObject);
            }
        }

        void ProfileStandingSetting(JsonData __j, ProfileItemElement profileStanding)
        {
            profileStandingScroll.SetActive(false);
            standingController.SetActive(true);

            controlStandingData = __j;
            controlStandingItemElement = profileStanding;
        }

        /// <summary>
        /// 스탠딩 캐릭터 선택시의 액션
        /// </summary>
        void SelectControlStanding(StandingElement standing)
        {
            controlStanding = standing;

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
        }

        void CreateStandingCharacter()
        {
            if (controlStanding != null)
                return;

            controlStanding = Instantiate(standingPrefab, decoObjects).GetComponent<StandingElement>();
            controlStanding.NewStanding(controlStandingData, controlStandingItemElement);
        }

        /// <summary>
        /// 선택된 곳은 비활성화
        /// </summary>
        void DisableScreenStand(int index)
        {
            for(int i=0;i<screenStand.Length;i++)
            {
                if (screenStand[i] == null)
                    continue;

                if (i == index)
                {
                    if (controlStanding == screenStand[i])
                        break;
                    else
                        screenStand[i].gameObject.SetActive(false);
                }
                else
                    screenStand[i].gameObject.SetActive(true);
            }
        }

        public void OnClickPositionLeft()
        {
            CreateStandingCharacter();
            controlStanding.standingRect.anchoredPosition = new Vector2(-controlStanding.standingRect.sizeDelta.x * 0.25f, 0f);
            DisableScreenStand(0);
        }

        public void OnClickPositionCenter()
        {
            CreateStandingCharacter();
            controlStanding.standingRect.anchoredPosition = Vector2.zero;
            DisableScreenStand(1);
        }

        public void OnClickPositioneRight()
        {
            CreateStandingCharacter();
            controlStanding.standingRect.anchoredPosition = new Vector2(controlStanding.standingRect.sizeDelta.x * 0.25f, 0f);
            DisableScreenStand(2);
        }

        public void OnClickPositionFlip()
        {
            if (controlStanding != null)
            {
                if (controlStanding.standingRect.eulerAngles.y == 0)
                    controlStanding.standingRect.eulerAngles = new Vector3(0f, 180f, 0f);
                else
                    controlStanding.standingRect.eulerAngles = new Vector3(0f, 0f, 0f);
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

            for (int i = 0; i < screenStand.Length; i++)
            {
                if (screenStand[i] == controlStanding)
                {
                    screenStand[i] = null;
                    break;
                }
            }

            controlStanding.RemoveFromScreen();
            Destroy(controlStanding.gameObject);
            profileStandingScroll.SetActive(true);
            standingController.SetActive(false);
        }

        public void OnClickCancleControl()
        {
            // 기존의 화면의 것을 제어해준 것이라면 다시 취소해주고
            if(IsScreenStading())
            {
                for (int i = 0; i < screenStand.Length; i++)
                {
                    if (screenStand[i] == null)
                        continue;

                    screenStand[i].RollbackTransform();
                    screenStand[i].gameObject.SetActive(true);
                }
            }
            else
            {
                if (controlStanding != null)
                    Destroy(controlStanding.gameObject);
                else
                    controlStandingItemElement.currentCount--;
            }

            controlStanding = null;

            profileStandingScroll.SetActive(true);
            standingController.SetActive(false);
        }

        public void OnClickStandingFix()
        {
            if(IsScreenStading())
            {
                for(int i=0;i<screenStand.Length;i++)
                {
                    if (screenStand[i] == null)
                        continue;

                    // 기존에 있던 스탠딩을 제어해준 것이면 있던 위치에서 지워준다
                    if (screenStand[i] == controlStanding)
                        screenStand[i] = null;
                }
            }
            else
            {
                // 신규 스탠딩의 경우에는 생성List에 추가해준다
                createObject.Add(controlStanding.gameObject);
            }

            // 이미 그 자리에 누가 있었으면 없애버려
            if (controlStanding.standingRect.anchoredPosition.x < 0 && screenStand[0] != null)
                RemoveSwapObject(0);
            else if (controlStanding.standingRect.anchoredPosition.x == 0 && screenStand[1] != null)
                RemoveSwapObject(1);
            else if (controlStanding.standingRect.anchoredPosition.x > 0 && screenStand[2] != null)
                RemoveSwapObject(2);

            // 0보다 작으면 좌측, 0이면 중앙, 0보다 크면 우측
            if (controlStanding.standingRect.anchoredPosition.x < 0)
                screenStand[0] = controlStanding;
            else if (controlStanding.standingRect.anchoredPosition.x == 0)
                screenStand[1] = controlStanding;
            else
                screenStand[2] = controlStanding;

            controlStanding.SetRectTransInfo();

            controlStanding = null;

            profileStandingScroll.SetActive(true);
            standingController.SetActive(false);
        }

        /// <summary>
        /// 화면에 서 있던 캐릭터 제어한건지 체크
        /// </summary>
        /// <returns></returns>
        bool IsScreenStading()
        {
            for (int i = 0; i < screenStand.Length; i++)
            {
                if (screenStand[i] == null)
                    continue;

                if (screenStand[i] == controlStanding)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 해당 위치에서 변경하는 경우 삭제
        /// </summary>
        /// <param name="index"></param>
        void RemoveSwapObject(int index)
        {
            for (int i = 0; i < createObject.Count; i++)
            {
                // List에서 삭제
                if (createObject[i] == screenStand[index].gameObject)
                {
                    createObject.Remove(createObject[i]);
                    break;
                }
            }

            // object 파괴
            screenStand[index].RemoveFromScreen();
            Destroy(screenStand[index].gameObject);
        }

        #endregion

        #region 뱃지


        #endregion

        #region 스티커 관련

        void LoadStickerData()
        {
            if (!profileCurrencyList.ContainsKey(LobbyConst.NODE_STICKER) || profileCurrencyList[LobbyConst.NODE_STICKER] == null)
            {
                stickerScroll.SetActive(false);
                noneStickerItem.SetActive(true);
                return;
            }

            stickerScroll.SetActive(true);
            noneStickerItem.SetActive(false);

            ProfileItemElement listElement = null;

            for (int i = 0; i < profileCurrencyList[LobbyConst.NODE_STICKER].Count; i++)
            {
                listElement = Instantiate(itemListPrefab, stickerElementListContent).GetComponent<ProfileItemElement>();
                listElement.InitCurrencyListElement(profileCurrencyList[LobbyConst.NODE_STICKER][i]);
                createObject.Add(listElement.gameObject);
            }
        }

        void ProfileBadgeSetting(JsonData __j, ProfileItemElement profileDeco)
        {
            ItemElement badgeElement = Instantiate(stickerObjectPrefab, decoObjects).GetComponent<ItemElement>();
            badgeElement.NewProfileItem(__j, profileDeco);
            createObject.Add(badgeElement.gameObject);
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

        /// <summary>
        /// 꾸미기 한거 저장
        /// </summary>
        void OnCllickSaveDeco()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = LobbyConst.FUNC_USER_PROFILE_SAVE;
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[LobbyConst.NODE_CURRENCY_LIST] = JsonMapper.ToObject("[]");

            int sortingOrder = 0;

            if(!string.IsNullOrEmpty(moveBg.currencyName))
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

            // 이제 화면에 들어간 스탠딩(미구현), 뱃지, 말풍선(미구현)을 넣어준다
            for (int i = 0; i < decoObjects.childCount; i++)
            {
                sortingOrder++;

                if (decoObjects.GetChild(i).GetComponent<ItemElement>() != null)
                    sending[LobbyConst.NODE_CURRENCY_LIST].Add(decoObjects.GetChild(i).GetComponent<ItemElement>().SaveJsonData(sortingOrder));
                else if (decoObjects.GetChild(i).GetComponent<StandingElement>() != null)
                    sending[LobbyConst.NODE_CURRENCY_LIST].Add(decoObjects.GetChild(i).GetComponent<StandingElement>().SaveStandingData(sortingOrder));
            }

            // text list를 만들어서 생성한 텍스트 값을 Json화해서 넣는다.
            sending[LobbyConst.NODE_TEXT_LIST] = JsonMapper.ToObject("[]");

            for (int i = 0; i < textObjects.childCount; i++)
                sending[LobbyConst.NODE_TEXT_LIST].Add(textObjects.GetChild(i).GetComponent<DecoTextElement>().SaveJsonData(i));

            NetworkLoader.main.SendPost(CallbackSaveDeco, sending, true);
        }

        void CallbackSaveDeco(HTTPRequest req, HTTPResponse res)
        {
            if(!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackSaveDeco");
                return;
            }

            // 저장이 완료되면 다시 ViewMain으로 간다
            UserManager.main.userProfile = JsonMapper.ToObject(res.DataAsText);
            ViewMain.OnProfileSetting?.Invoke();
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SAVE_PROFILE_DECO);
        }
    }
}