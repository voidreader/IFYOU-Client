using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewProfileDeco : CommonView
    {
        public static Action OnDisableAllOptionals = null;
        public static Action<JsonData, ProfileItemElement> OnBackgroundSetting = null;
        public static Action<JsonData, ProfileItemElement> OnBadgeSetting = null;

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
        public GameObject stickerObjectPrefab;      // 화면에 세워질 sticker object prefab
        public GameObject itemListPrefab;           // 뱃지, 스티커 icon prefab
        public Transform stickerElementListContent; // 뱃지element가 들어갈 parent
        public GameObject stickerScroll;
        public GameObject noneStickerItem;          // 보유 스티커 재화가 없는 경우 표시

        [Header("Text Tab")]
        public TextMeshProUGUI fontSizeText;        // 폰트 사이즈 설정 숫자
        public Image[] fontColorElements;
        public Transform textObjects;               // 텍스트 생성 위치(parent)
        public GameObject textObjectPrefab;         // InputField object
        Color fontColor = Color.magenta;

        JsonData profileCurrencyList;
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

            JsonData profileCurrency = SystemManager.GetJsonNode(UserManager.main.userProfile, LobbyConst.NODE_CURRENCY);


            // 사용자가 소지하고 있는 재화 리스트 불러와서 뿌려주기
            ProfileItemElement listElement = null;

            if(!profileCurrencyList.ContainsKey(LobbyConst.NODE_WALLPAPER) || profileCurrencyList[LobbyConst.NODE_WALLPAPER] == null)
            {
                profileBgScroll.SetActive(false);
                noneBGItem.SetActive(true);
            }
            else
            {
                profileBgScroll.SetActive(true);
                noneBGItem.SetActive(false);

                // 배경
                for (int i = 0; i < profileCurrencyList[LobbyConst.NODE_WALLPAPER].Count; i++)
                {
                    listElement = Instantiate(bgPrefab, bgElementListContent).GetComponent<ProfileItemElement>();
                    listElement.InitCurrencyListElement(profileCurrencyList[LobbyConst.NODE_WALLPAPER][i]);
                    
                    // ! 여기 프로필 저장정보가 없는 경우 오류가 납니다. 빈 JSON이 날아옵니다. '{}'  
                    // ! (임시로 조건 수정함)
                    // ! 그리고 0번째 데이터가 월페이퍼라고 장담할 수 없습니다. 
                    // ! 아래 스티커 처럼 for문을 돌리세요. 
                    if ( profileCurrency != null && profileCurrency.Count > 0
                        && SystemManager.GetJsonNodeString(profileCurrencyList[LobbyConst.NODE_WALLPAPER][i], LobbyConst.NODE_CURRENCY) == SystemManager.GetJsonNodeString(profileCurrency[0], LobbyConst.NODE_CURRENCY))
                    {
                        background.SetDownloadURL(SystemManager.GetJsonNodeString(profileCurrency[0], LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(profileCurrency[0], LobbyConst.NODE_CURRENCY_KEY));
                        background.GetComponent<RectTransform>().anchoredPosition = new Vector2(float.Parse(SystemManager.GetJsonNodeString(profileCurrency[0], LobbyConst.NODE_POS_X)), 0f);
                        background.GetComponent<RectTransform>().sizeDelta = new Vector2(float.Parse(SystemManager.GetJsonNodeString(profileCurrency[0], LobbyConst.NODE_WIDTH)), float.Parse(SystemManager.GetJsonNodeString(profileCurrency[0], LobbyConst.NODE_HEIGHT)));
                        moveBg.currencyName = SystemManager.GetJsonNodeString(profileCurrency[0], LobbyConst.NODE_CURRENCY);
                    }

                    createObject.Add(listElement.gameObject);
                }
            }


            // 캐릭터 스탠딩


            // 뱃지


            // 스티커
            if (!profileCurrencyList.ContainsKey(LobbyConst.NODE_BADGE) || profileCurrencyList[LobbyConst.NODE_BADGE] == null)
            {
                stickerScroll.SetActive(false);
                noneStickerItem.SetActive(true);
            }
            else
            {
                stickerScroll.SetActive(true);
                noneStickerItem.SetActive(false);

                for (int i = 0; i < profileCurrencyList[LobbyConst.NODE_BADGE].Count; i++)
                {
                    listElement = Instantiate(itemListPrefab, stickerElementListContent).GetComponent<ProfileItemElement>();
                    listElement.InitCurrencyListElement(profileCurrencyList[LobbyConst.NODE_BADGE][i]);
                    // current_cnt 1개 이상인걸 체크해서(if문) Instantiate 해줄 for문 추가.
                    // curreny 이름으로 체크(이름이 PK임)해서 찾아서 생성해주고 ProfileDecoElement 연결 해준다
                    if (int.Parse(SystemManager.GetJsonNodeString(profileCurrencyList[LobbyConst.NODE_BADGE][i], LobbyConst.NODE_CURRENT_COUNT)) > 0)
                    {
                        for (int j = 0; j < profileCurrency.Count; j++)
                        {
                            // 생성되는 listElement의 currency와 사용한 currency의 string 값이 같다면
                            if (SystemManager.GetJsonNodeString(profileCurrencyList[LobbyConst.NODE_BADGE][i], LobbyConst.NODE_CURRENCY) == SystemManager.GetJsonNodeString(profileCurrency[j], LobbyConst.NODE_CURRENCY))
                            {
                                ItemElement itemElement = Instantiate(stickerObjectPrefab, decoObjects).GetComponent<ItemElement>();
                                itemElement.SetProfileItem(profileCurrency[j], listElement);
                                createObject.Add(itemElement.gameObject);
                            }
                        }
                    }

                    createObject.Add(listElement.gameObject);
                }
            }

            // 말풍선



            // 텍스트
            JsonData profileText = SystemManager.GetJsonNode(UserManager.main.userProfile, LobbyConst.NODE_TEXT);

            if (profileText.Count > 0)
            {
                // 텍스트
                for (int i = 0; i < profileText.Count; i++)
                {
                    DecoTextElement textElement = Instantiate(textObjectPrefab, textObjects).GetComponent<DecoTextElement>();
                    textElement.SetProfileText(profileText[i]);
                    createObject.Add(textElement.gameObject);
                }
            }
        }


        public override void OnView()
        {
            base.OnView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_MULTIPLE_BUTTON_LABEL, "저장", string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, "꾸미기모드", string.Empty);

        }


        public override void OnHideView()
        {
            base.OnHideView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);

            foreach (GameObject g in createObject)
                Destroy(g);
        }

        /// <summary>
        /// 화면 위의 Optionals 모두 비활성화
        /// </summary>
        public void OnClickAllDisable()
        {
            for(int i=0;i<decoObjects.childCount;i++)
                decoObjects.GetChild(i).GetComponent<ItemElement>().DisableOptionals();

            for (int i = 0; i < textObjects.childCount; i++)
                textObjects.GetChild(i).GetComponent<DecoTextElement>().DisableOptional();
        }

        #region 배경 관련

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

        #endregion

        #region 뱃지


        #endregion

        #region 스티커 관련


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
        public void OnCllickSaveDeco()
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
                sending[LobbyConst.NODE_CURRENCY_LIST].Add(decoObjects.GetChild(i).GetComponent<ItemElement>().SaveJsonData(sortingOrder));
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