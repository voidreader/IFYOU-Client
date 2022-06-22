﻿using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory
{
    public class ViewGame : CommonView, IPointerClickHandler
    {
        public static ViewGame main = null;     // UI singleton
        
        public CommonView gameMenu;

        [Header("말풍선")]
        public RectTransform bubbleParent;          // 말풍선 부모
        public List<GameBubbleCtrl> ListBubbles;    // 말풍선들!
        public GameBubbleCtrl partnerBubble;        // 발신자 말풍선(전화용)
        public GameBubbleCtrl selfBubble;           // 수신자 말풍선(전화용)
        int bubbleIndex = 0;                        // 말풍선 pooling 인덱스

        [Header("나레이션")]
        Action OnNarraion = delegate { };
        public Sprite typeBlackSprite;
        public Sprite typeWhiteSprite;

        public Image boxImage;
        public TextMeshProUGUI textNarration;
        string narrationText = string.Empty;

        [Space] [Header("**스탠딩 캐릭터용 변수**")]
        public Transform[] modelRenderParents;  // 0 : 미사용, 1 : 2인스탠딩중 말 안하는 사람, 2 : 2인스탠딩중 말하는 사람 or 중앙 사용중
        public RawImage[] modelRenders;         // 0 : L, 1 : C, 2: R

        
        [Space][Space][Header("**선택지**")]
        public CanvasGroup selectionInfo;
        public TextMeshProUGUI selectionInfoText;       // 선택지 안내 텍스트
        public Image selectionTutorialText;             // 선택지 튜토리얼 안내문구
        public Image selectionBackground;               // 선택지 나올때 음영처리를 위한 백그라운드 이미지
        public List<ScriptRow> ListSelectionRows = new List<ScriptRow>(); // 현재보여지는 선택지 정보의 스크립트 데이터 
        public List<IFYouGameSelectionCtrl> ListGameSelection = new List<IFYouGameSelectionCtrl>(); // UI에 저장된 선택지들 
        public List<IFYouGameSelectionCtrl> ListAppearSelection = new List<IFYouGameSelectionCtrl>(); // 활성화된 선택지 
        public UIView commonTop;
        
        
        public GameObject screenInputBlocker = null;            // 연출중 입력막고싶다..!

        [Space][Header("Flow time")]
        public Image fadeImage;
        public TextMeshProUGUI flowTimeText;

        [Header("**장소 진입용 Label**")]
        public Image placeTextBG;
        public TextMeshProUGUI episodeNum;
        public TextMeshProUGUI placeNameText;

        [Space][Space][Header("휴대폰 사용")]
        [Tooltip("전화,메시지 오면 뜰 이미지")]
        public GameObject phoneOverlay;
        public GameObject phoneImage;

        [Header("***전화***")]
        public GameObject phoneCall;
        public TextMeshProUGUI callName;

        public CanvasGroup callBG;
        public Image callBackground;            // 전화중 배경에 깔리는 배경(?), overlay같은거
        public Image callIcon;
        public GameObject connectMark;          // 전화연결중 표기
        public TextMeshProUGUI calledName;
        public TextMeshProUGUI callTime;
        public GameObject callButtons;          // 전화버튼 2개를 담고 있는 Object
        public GameObject hangUpButton;         // 전화끊기 버튼
        public GameObject answerButton;         // 전화받기 버튼

        string hangUpSceneId = string.Empty;
        string answerSceneId = string.Empty;
        bool timeEnd = false;
        public bool userCall = false;                  // 전화를 걸었는가?
        public bool isVibrate = false;                 // 현재 핸드폰이 진동중인가요?


        [Header("***메신저***")]
        public GameObject messenger;
        public Image messengerOverlay;
        public Image messengerIcon;
        public TextMeshProUGUI messageSender;
        public TextMeshProUGUI messageAlert;

        [Tooltip("메신저 ScrollView 안에 들어있는 content")]
        public GameObject messengerContent;

        [Header("Messenger Prefab")]
        public GameObject messenger_call;
        public GameObject messenger_image;
        public GameObject messenger_partner;
        public GameObject messenger_self;

        public GameObject senderBubble;
        public GameObject receiverBubble;

        GameObject speakerPrefab;
        GameObject messengerObject;         // 메신저에서 사용될 오브젝트들
        Transform msgBubbleParent;          // 톡 말풍선의 부모 transform
        TextMeshProUGUI messengerText;      // 메신저에서 사용되는 텍스트들이 담길 변수
        List<GameObject> messengerData = new List<GameObject>();
        ScrollRect sr;
        string msgPrevSpeaker = string.Empty;       // 메신저에서 전에 말했던 사람
        string msgCurrSpeaker = string.Empty;       // 메신저에서 현재 말하고 있는 사람


        [Header("게임로그")]
        public GameObject logPanel;
        public ScrollRect logScrollRect;
        public TextMeshProUGUI logText;
        private string prevSpeaker = string.Empty;

        public GameObject inGameMenuBtn;        // 인게임메뉴 버튼
        public GameObject closeLogBtn;          // 로그 패널 외의 영역 버튼

        StringBuilder logData = new StringBuilder();
        List<string> selections = new List<string>();
        string selected = string.Empty;

        [Space][Header("음성 표출")]
        public GameObject microphoneIcon;
        Animator microphoneAnimator;


        private void Awake()
        {
            main = this;
            selectionInfoText.text = string.Empty;
        }

        void Update()
        {
            if (!GameManager.main.SoundGroup[1].GetIsPlaying)
                InactiveMicrophoneIcon();
                
            if(Input.GetKeyDown(KeyCode.Escape)) {
                CommonView.DeleteDumpViews(); // null 인 리스트 정리
                
                // 백버튼 눌렀을때, 팝업창 없고, 살아있는 뷰가 Gameview 하나고, 로그 패널 없을때.
                if(PopupManager.main.GetFrontActivePopup() == null 
                    && !logPanel.activeSelf
                    
                    && ( (CommonView.ListActiveViews.Count == 1 && CommonView.ListActiveViews.Contains(this))  // 1개 활성화, 본인
                        || (CommonView.ListActiveViews.Count == 2 && CommonView.ListActiveViews.Contains(this) && CommonView.ListActiveViews.Contains(gameMenu) )) // 2개 활성화 본인과 메뉴 
                    ) {
                        
                    // 
                    SystemManager.ShowSystemPopupLocalize("6037", GameManager.main.QuitGame, null, true);
                        
                }
            }
        }

        public override void OnStartView()
        {
            base.OnStartView();
            
            // ViewGame 초기화 
            StoryManager.enterGameScene = true;
            SetBlockScreenActiveFlag(false); 

            // 화면 크기에 맞춰서 스탠딩캐릭터 renderer 사이즈 조절
            float rawImageSize = (float)Screen.height / (float)Screen.width * 900f;
            rawImageSize = Mathf.Clamp(rawImageSize, 1600f, 2000f);

            for (int i = 0; i < modelRenders.Length; i++)
                modelRenders[i].GetComponent<RectTransform>().sizeDelta = new Vector2(rawImageSize, rawImageSize);
                
            selectionTutorialText.gameObject.SetActive(false);
            selectionBackground.gameObject.SetActive(false);
        }


        /// <summary>
        /// 화면 터치 처리 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {

            // 광고 볼때 터치 안되도록 처리 
            if (AdManager.main.isAdShowing)
            {
                Debug.Log(">> advertisement is showing now <<");
                return;
            }
            
            // 로그 패널 활성화 중엔 입력 받지 않음.   
            if (logPanel.activeSelf)
                return;

            // threadHold 중에 입력 받지 않음
            if (GameManager.main.isThreadHold)
            {
                Debug.Log(">> holding thread now <<");
                return;
            }

            // touch waiting 아닌 경우 입력 받지 않음.
            if (!GameManager.main.isWaitingScreenTouch)
                return;

            // 입력 받았어요!
            GameManager.main.isWaitingScreenTouch = false;
        }


        #region 말풍선 관련

        public void MakeTalkBubble(ScriptRow __row, Action __cb, int index = -1)
        {
            // StartCoroutine(MakingTalkBubble(__row, __cb, index));
            if (bubbleIndex >= ListBubbles.Count)
                bubbleIndex = 0;

            // 아무말도 안넣으면 그냥 종료 
            if (string.IsNullOrEmpty(__row.script_data))
            {
                __cb?.Invoke();
                // yield break;
                return;
            }

            // 말풍선 크기를 판단하기 위해 호출한다.
            // 크기 지정을 수동으로 해놓은 경우만!
            // * 추가 설명 : TextMesh 는 실제 화면에서 render 되어야지, TextArea를 오버했는지 안했는지를 알 수 있다.
            if (__row.bubble_size == 0)
                BubbleManager.main.SetFakeBubbles(__row);

            // 말풍선 풀에서 하나를 골라 세팅합니다.
            // skip을 사용했거나, 캐릭터 스탠딩이 아닐때
            if (GameManager.main.useSkip || index < 0)
                ListBubbles[bubbleIndex++].ShowBubble(__row, __cb);
            else
                StartCoroutine(RoutineMoveWait(__row, __cb));
        }
        
        IEnumerator MakingTalkBubble(ScriptRow __row, Action __cb, int index = -1) {
            
            yield return new WaitForFixedUpdate(); // physics 대기 
            
            if (bubbleIndex >= ListBubbles.Count)
                bubbleIndex = 0;

            // 아무말도 안넣으면 그냥 종료 
            if (string.IsNullOrEmpty(__row.script_data))
            {
                __cb?.Invoke();
                yield break;
            }

            // 말풍선 크기를 판단하기 위해 호출한다.
            // 크기 지정을 수동으로 해놓은 경우만!
            // * 추가 설명 : TextMesh 는 실제 화면에서 render 되어야지, TextArea를 오버했는지 안했는지를 알 수 있다.
            if (__row.bubble_size == 0)
                BubbleManager.main.SetFakeBubbles(__row);

            // 말풍선 풀에서 하나를 골라 세팅합니다.
            // skip을 사용했거나, 캐릭터 스탠딩이 아닐때
            if (GameManager.main.useSkip || index < 0)
                ListBubbles[bubbleIndex++].ShowBubble(__row, __cb);
            else
                StartCoroutine(RoutineMoveWait(__row, __cb));
        }
        

        /// <summary>
        /// 캐릭터가 무빙 중인 경우는 무빙이 완료될때까지 기다린다. 
        /// </summary>
        IEnumerator RoutineMoveWait(ScriptRow __row, Action __cb)
        {
            yield return new WaitUntil(() => GameManager.main.CheckModelMoveComlete());
            ListBubbles[bubbleIndex++].ShowBubble(__row, __cb);
        }

        /// <summary>
        /// 화면상의 모든 말풍선(전화 말풍선 포함), 나레이션 제거 
        /// </summary>
        public void HideBubbles()
        {
            HideNarration();

            for (int i = 0; i < ListBubbles.Count; i++)
            {
                if (ListBubbles[i].gameObject.activeSelf || ListBubbles[i].needDelayShow)
                    ListBubbles[i].OffBubble(GameManager.main.useSkip);
            }

            if (selfBubble.gameObject.activeSelf)
                selfBubble.gameObject.SetActive(false);

            if (partnerBubble.gameObject.activeSelf)
                partnerBubble.gameObject.SetActive(false);
        }

        /// <summary>
        /// 전화 중 말풍선 위치 설정
        /// </summary>
        /// <param name="isSelf">true일 경우 전화본인, false일 경우 전화상대</param>
        public void SetPhoneProcess(ScriptRow __row, Action __cb, bool isSelf)
        {
            // 입력된 데이터 말풍선 사이즈 체크를 위한 체크
            if (__row.bubble_size < 1)
                BubbleManager.main.SetFakeBubbles(__row);

            if (isSelf)
                selfBubble.ForPhoneBubble(__row, __cb, 9, isSelf);
            else
                partnerBubble.ForPhoneBubble(__row, __cb, 4, isSelf);
        }

        /// <summary>
        /// 전화 템플릿용 말풍선 숨기기
        /// </summary>
        /// <param name="isSelf">true면 수신자, false면 발신자</param>
        public void HidePhoneBubble(bool isSelf)
        {
            if (isSelf)
                selfBubble.PhoneBubbleOff(isSelf);
            else
                partnerBubble.PhoneBubbleOff(isSelf);
        }

        #endregion

        #region 나레이션 관련

        public void ShowNarration(string __narration, bool typeWhite, Action __cb)
        {
            OnNarraion = __cb;

            if(string.IsNullOrEmpty(__narration))
            {
                GameManager.main.isWaitingScreenTouch = false;
                OnNarraion();
                return;
            }

            Color textColor = Color.black;

            if (typeWhite)
            {
                boxImage.sprite = typeWhiteSprite;
                ColorUtility.TryParseHtmlString("#3D3D3DFF", out textColor);
            }
            else
            {
                boxImage.sprite = typeBlackSprite;
                textColor = Color.white;
            }

            textNarration.color = textColor;

            // color 값 조정
            boxImage.color = CommonConst.COLOR_IMAGE_TRANSPARENT;
            textNarration.color = new Color(textNarration.color.r, textNarration.color.b, textNarration.color.g, 0); // 투명하게 만들어준다.

            narrationText = __narration.Replace(@"\", "\n");

            int countNewLine = 0;

            if(narrationText.Contains(@"\"))
            {
                foreach(char c in narrationText)
                {
                    if (c == '\\')
                        countNewLine++;
                }
            }

            int charCount = SystemManager.main.currentAppLanguageCode != "EN" ? 22 : 30;

            if(countNewLine == 0)
            {
                while (countNewLine * charCount < narrationText.Length)
                    countNewLine++;

                countNewLine--;
            }

            if(countNewLine < 2)
                boxImage.GetComponent<RectTransform>().sizeDelta = new Vector2(boxImage.GetComponent<RectTransform>().sizeDelta.x, 160);
            else
                boxImage.GetComponent<RectTransform>().sizeDelta = new Vector2(boxImage.GetComponent<RectTransform>().sizeDelta.x, 80 + (countNewLine * 60));

            boxImage.gameObject.SetActive(true);
            
            SystemManager.SetText(textNarration, narrationText);

            boxImage.DOFade(0.8f, 0.2f);
            textNarration.DOFade(1, 0.2f);

            OnNarraion();
        }

        public void HideNarration()
        {
            boxImage.DOKill();
            textNarration.DOKill();

            boxImage.gameObject.SetActive(false);
        }

        #endregion

        #region 선택지 처리


        /// <summary>
        /// 선택지 쌓기 처리 
        /// </summary>
        /// <param name="__row"></param>
        /// <param name="__openUI"></param>
        public void StackSelection(ScriptRow __row, bool __openUI)
        {
            ListSelectionRows.Add(__row); // 쌓기!

            // 마지막 선택지가 쌓였으면, UI 활성화 
            if (__openUI)
                OpenSelectionUI();
        }

        /// <summary>
        /// 선택지 UI 활성화 
        /// </summary>
        void OpenSelectionUI()
        {
            GameManager.main.isSelectionInputWait = true; // 선택지 입력을 기다려야 한다. 
            IFYouGameSelectionCtrl.isChooseCompleted = false; // 선택지 입력 초기화 

            // Appear 리스트 클리어 
            ListAppearSelection.Clear();
            IFYouGameSelectionCtrl.ListStacks.Clear();

            // 선택지 뒷배경에 대한 처리 
            selectionBackground.color = CommonConst.COLOR_BLACK_TRANSPARENT;
            selectionBackground.gameObject.SetActive(true);
            selectionBackground.DOFade(0.7f, 1);

            // 선택지 안내 표출
            if (!string.IsNullOrEmpty(selectionInfoText.text))
                selectionInfo.DOFade(1f, 1f);

            if (!UserManager.main.isSelectionTutorialClear)
            {
                selectionTutorialText.color = CommonConst.COLOR_BLACK_TRANSPARENT;
                selectionTutorialText.gameObject.SetActive(true);
                selectionTutorialText.DOFade(1, 1);
            }


            // 선택지 셋팅
            /*
            for (int i = 0; i < ListSelectionRows.Count; i++)
            {
                if(i >= ListGameSelection.Count) {
                    SystemManager.ShowMessageAlert("너무 많은 선택지!(최대 6개)", false);
                    break;
                }
                
                ListGameSelection[i].SetSelection(ListSelectionRows[i], i);
                ListAppearSelection.Add(ListGameSelection[i]); // appear에 추가. 
            }
            */


            // 마지막 선택지부터 stack처럼 쌓기
            for (int i = ListSelectionRows.Count - 1; i >= 0; i--)
            {
                ListGameSelection[i].isPurchaseSelection = false;
                ListGameSelection[i].SetSelection(ListSelectionRows[i], ListSelectionRows.Count - 1 - i);
                ListAppearSelection.Add(ListGameSelection[i]); // appear에 추가. 
            }

            // 선택지 리스트 중 힌트 포함하고 있는지 체크
            bool hasSelectionHint = false;
            for (int i = 0; i < ListAppearSelection.Count; i++)
            {
                if (ListAppearSelection[i].hasSelectionHint)
                {
                    hasSelectionHint = true;
                    break;
                }
            }

            if(hasSelectionHint)
            {
                for (int i = 0; i < ListAppearSelection.Count; i++)
                    ListAppearSelection[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(ListAppearSelection[i].GetComponent<RectTransform>().anchoredPosition.x - 20, ListAppearSelection[i].GetComponent<RectTransform>().anchoredPosition.y);
            }


            // 구매할 것이 있는지 체크
            bool hasPurchaseSelection = false;

            for (int i = 0; i < ListAppearSelection.Count; i++)
            {
                if (ListAppearSelection[i].isPurchaseSelection || !ListAppearSelection[i].isPurchasedHint)
                {
                    hasPurchaseSelection = true;
                    break;
                }
            }

            // 구매해야하는 선택지 혹은 선택지 힌트가 있다면
            if (hasPurchaseSelection)
            {
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
                Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);

                commonTop.Show();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="__flag"></param>
        void SetBlockScreenActiveFlag(bool __flag)
        {
            screenInputBlocker.SetActive(__flag);
        }


        /// <summary>
        /// 선택지 선택 완료에 대한 처리 
        /// </summary>
        /// <param name="__targetSceneID">이동할 사건 ID</param>
        /// <param name="__selectionIndex">선택지 버튼 인덱스</param>
        public void ChooseSelection(string __targetSceneID, int __selectionIndex)
        {
            SetBlockScreenActiveFlag(true); // 입력 막기.(안전빵 )
            
            // 백그라운드 제거 
            selectionBackground.DOFade(0, 0.5f).OnComplete(()=>{ selectionBackground.gameObject.SetActive(false);});
            
            // 튜토리얼 선택지 문구 제거 
            if(selectionTutorialText.gameObject.activeSelf) {
                selectionTutorialText.DOFade(0, 0.5f).OnComplete(()=>{ selectionTutorialText.gameObject.SetActive(false);});
            }
            
            // 사건 ID 미리할당. 
            GameManager.main.targetSelectionSceneID = __targetSceneID;
        }
        
        /// <summary>
        /// 선택지 모두 퇴장완료
        /// </summary>
        /// <param name="__selection"></param>
        public void RemoveListAppearSelection(IFYouGameSelectionCtrl __selection)
        {
            // IFYouGameSelectionCtrl에서 상태가 None으로 변경되면서 호출된다. 
            ListAppearSelection.Remove(__selection);
            
            Debug.Log(">> RemoveListAppearSelection, count : " + ListAppearSelection.Count);

            // * 모든 선택지가 퇴장했다. 
            if (ListAppearSelection.Count == 0)
            {
                SetBlockScreenActiveFlag(false); // 입력막기 풀기.
                HideSelection(); // Selection 처리 완료
            }
        }

        /// <summary>
        /// 즉각적인 선택지 비활성화 처리 
        /// </summary>
        public void HideSelection()
        {
            Debug.Log(">> HideSelection");
        
            GameManager.main.isSelectionInputWait = false;
        
            ListSelectionRows.Clear(); // 선택지 스크립트 stack 클리어 
            
            // 이동!
            GameManager.main.currentPage.SetCurrentRowBySceneID(GameManager.main.targetSelectionSceneID);

            // 메신저 중이었다면 비활성화
            // 22.04.04 이유는 기억이 나지 않지만 메신저를 비활성화 해주고 있다. 왜지...?
            //if (messenger.activeSelf)
            //messenger.SetActive(false);
            
            // * 광고처리 추가 
            AdManager.main.PlaySelectionAD();
        }

        /// <summary>
        /// 선택지 안내 문구 세팅
        /// </summary>
        /// <param name="text"></param>
        public void SetSelectionInfoText(string __info)
        {
            SystemManager.SetText(selectionInfoText, __info);
            selectionInfo.alpha = 0f;
        }




        #endregion

        #region 시간흐름 관련

        /// <summary>
        /// 시간 흐름 관련 텍스트 연출
        /// </summary>
        /// <param name="labelText">라벨에 들어갈 string</param>
        /// <param name="voice">음성 컬럼값</param>
        /// <param name="reversal">반전을 사용하는가?</param>
        /// <param name="isBackground">다음 행이 배경, 장소진입, 일러스트인가?</param>
        public void FlowTimeAnim(string labelText, string voice, bool reversal, bool isBackground)
        {
            flowTimeText.text = string.Empty;

            if (reversal)
            {
                fadeImage.color = new Color(1, 1, 1, 0);
                flowTimeText.color = Color.black;
            }
            else
            {
                fadeImage.color = new Color(0, 0, 0, 0);
                flowTimeText.color = Color.white;
            }

            fadeImage.gameObject.SetActive(true);

            StartCoroutine(RoutineFlowTime(labelText, voice, isBackground));
        }

        IEnumerator RoutineFlowTime(string labelText, string voice, bool isBackground)
        {
            Tween timeTween = fadeImage.DOFade(1f, 1f);
            yield return timeTween.WaitForCompletion();
            GameManager.main.HideCharacters();

            // script_data가 null값이 아니라면
            if (!string.IsNullOrEmpty(labelText))
            {
                if (labelText.Contains("\\"))
                    labelText = labelText.Replace("\\", "\n");


                if (!string.IsNullOrEmpty(voice))
                    GameManager.main.SoundGroup[1].PlayVoice(voice);
                    
                if(SystemManager.main.currentAppLanguageCode == CommonConst.COL_AR) { 
                    // 아랍어는 타이핑 안한다. 
                    flowTimeText.color = new Color(flowTimeText.color.r, flowTimeText.color.g, flowTimeText.color.b, 0);
                    flowTimeText.text = labelText;
                    flowTimeText.DOFade(1, 2); // 2초 페이드인 
                    yield return new WaitForSeconds(3); // 3초 대기 
                }
                else {
                    // * 일반언어. 타이핑 된다. 
                    for (int i = 0; i < labelText.Length; i++)
                    {
                        string tmpStr = string.Empty;

                        // 다음 문자가 특수문자인 경우 한 글자로 취급한다
                        if (i + 1 < labelText.Length && System.Text.RegularExpressions.Regex.IsMatch(labelText[i + 1].ToString(), @"[~!@\#$%^&*\()\=+|\\/:;,?""<>']"))
                            tmpStr = labelText[i + 1].ToString();

                        flowTimeText.text += (labelText[i] + tmpStr);

                        if (!string.IsNullOrEmpty(tmpStr))
                            i++;

                        // 영어 텍스트는 길어서 너무 속도가 느리다
                        if (SystemManager.main.currentAppLanguageCode == CommonConst.COL_EN)
                            yield return new WaitForSeconds(0.04f);
                        else
                            yield return new WaitForSeconds(0.1f);
                    }                    
                }

                // 21.10.22
                // 음성이 재생중이라면 전부 재생될 때까지 기다려!
                if (GameManager.main.SoundGroup[1].GetIsPlaying)
                    yield return new WaitUntil(() => !GameManager.main.SoundGroup[1].GetIsPlaying);

                flowTimeText.DOFade(0f, 1f).SetDelay(1f);
                yield return new WaitForSeconds(1f);
            }

            if (isBackground)
            {
                Debug.Log("Wait Off [RoutineFlowTime]");
                GameManager.main.isThreadHold = false;
                yield break;
            }

            timeTween = fadeImage.DOFade(0f, 1f);

            yield return timeTween.WaitForCompletion();

            OnCompleteTimeFlowAnim();
        }


        /// <summary>
        /// 시간흐름, Animation callback
        /// </summary>
        void OnCompleteTimeFlowAnim()
        {
            GameManager.main.isThreadHold = false;
            fadeImage.gameObject.SetActive(false);
        }

        public void FadeOutTimeFlow()
        {
            if (!fadeImage.gameObject.activeSelf)
                return;

            fadeImage.DOFade(0f, 1.3f).OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false);
            });
        }

        #endregion

        #region 핸드폰 사용 관련

        public void HIdePhoneImage()
        {
            // 전화를 걸었던 상황이면 폰을 지우지 않는다
            if (userCall)
                return;

            phoneOverlay.SetActive(false);
            phoneImage.SetActive(false);
            phoneCall.SetActive(false);
            messengerOverlay.gameObject.SetActive(false);

            callBackground.gameObject.SetActive(false);
        }

        #region 전화


        /// <summary>
        /// 전화기 보여주기
        /// </summary>
        /// <param name="phoneRing">true = 전화가 옴, false = 전화를 검</param>
        /// <param name="isAnswer">true = 전화 받기만 가능, false = 전화 끊기만 가능</param>
        /// <param name="__hangUpId">끊기 눌렀을 때의 sceneId</param>
        /// <param name="__answerId">받기 눌렀을 떄의 sceneId</param>
        public void ShowPhoneImage(bool phoneRing, bool isAnswer, string __hangUpId = "", string __answerId = "")
        {
            phoneImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1350);

            phoneOverlay.SetActive(true);
            phoneImage.SetActive(true);
            phoneCall.SetActive(true);
            connectMark.SetActive(false);
            callButtons.SetActive(true);
            hangUpButton.SetActive(true);
            answerButton.SetActive(true);
            messengerOverlay.gameObject.SetActive(false);

            // 전화 선택 템플릿으로 scene Id를 전달 받은 경우
            if (!string.IsNullOrEmpty(__hangUpId) && !string.IsNullOrEmpty(__answerId))
            {
                hangUpSceneId = __hangUpId;
                answerSceneId = __answerId;

                StartCoroutine(RoutinePhoneEnter());
                return;
            }

            // 전화를 걸은 경우
            if(!phoneRing)
            {
                userCall = true;
                connectMark.SetActive(true);
                callButtons.SetActive(false);
                GameManager.main.isThreadHold = false;
            }

            // 전화 받기만 가능한 경우
            if (isAnswer)
                hangUpButton.SetActive(false);
            else
                answerButton.SetActive(false);

            StartCoroutine(RoutinePhoneEnter());
        }


        /// <summary>
        /// 전화 받기 버튼 누를시
        /// </summary>
        public void AnswerPhoneButton()
        {
            isVibrate = false;
            timeEnd = true;
            phoneImage.GetComponent<RectTransform>().DOAnchorPosY(-1350f, 0.5f);

            // fade 전 투명도 0
            callBG.alpha = 0f;
            callTime.text = "0:00";
            const float animTime = 0.7f;

            callBackground.gameObject.SetActive(true);
            callBG.DOFade(1f, animTime).SetDelay(0.5f).OnComplete(() =>
            {
                GameManager.main.isThreadHold = false;
                GameManager.main.isWaitingScreenTouch = false;

                if (!string.IsNullOrEmpty(answerSceneId))
                {
                    GameManager.main.MoveToTargetSceneID(answerSceneId);
                    answerSceneId = string.Empty;
                }

                ShowCallBackgrond();
            });
        }

        // 전화 끊기 버튼 누를시
        public void HangUpPhoneButton()
        {
            CleanUpPhone();
        }

        
        /// <summary>
        /// 화면에서 폰 치우기, 전화 끊기
        /// </summary>
        public void CleanUpPhone()
        {
            // 폰을 아무튼 화면에서 치웠으니 false로 만들어준다
            userCall = false;

            phoneImage.GetComponent<RectTransform>().DOAnchorPosY(-1350f, 0.5f).OnComplete(() =>
            {
                GameManager.main.isThreadHold = false;
                GameManager.main.isWaitingScreenTouch = false;

                if(!string.IsNullOrEmpty(hangUpSceneId))
                {
                    GameManager.main.MoveToTargetSceneID(hangUpSceneId);
                    hangUpSceneId = string.Empty;
                }
            });
        }

        /// <summary>
        /// 통화중에 사용하는 전화배경 깔기
        /// </summary>
        public void ShowCallBackgrond()
        {
            if (phoneCall.activeSelf)
            {
                phoneOverlay.SetActive(false);
                phoneImage.SetActive(false);
                phoneCall.SetActive(false);
            }

            if (timeEnd)
            {
                timeEnd = false;

                int timer = GameManager.isResumePlay ? UnityEngine.Random.Range(30, 80) : 0;
                if ((timer % 60) < 10f)
                    callTime.text = string.Format("{0}:0{1}", (timer / 60 % 60), (timer % 60));
                else
                    callTime.text = string.Format("{0}:{1}", (timer / 60 % 60), (timer % 60));

                StartCoroutine(PhoneTimer(timer));
            }

            callBackground.gameObject.SetActive(true);
        }

        IEnumerator PhoneTimer(int second = 0)
        {
            float timer = second;

            while (!timeEnd)
            {
                timer += Time.deltaTime;

                // 화면에 활성화 중일 때만 띄운다
                // 달리 전화를 아예 사용하지 않은 이후의 timeEnd를 끝맺을 곳을 어디로 할지 잘 모르겠음
                if (callBackground.gameObject.activeSelf)
                {
                    if ((int)(timer % 60) < 10f)
                        callTime.text = string.Format("{0}:0{1}", ((int)timer / 60 % 60), (int)(timer % 60));
                    else
                        callTime.text = string.Format("{0}:{1}", ((int)timer / 60 % 60), (int)(timer % 60));
                }

                yield return null;
            }
        }

        IEnumerator RoutinePhoneEnter()
        {
            // 스킵을 사용중에는 전화등장 연출을 하지 않는다
            if (GameManager.main.useSkip)
                yield break;

            // 위치 초기화
            phoneImage.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.7f).SetEase(Ease.OutBack);
            timeEnd = true;

            yield return new WaitForSeconds(0.7f);

            // 전화가 오는 상황에만 true
            isVibrate = !userCall ? true : false;

            while(isVibrate)
            {
                Handheld.Vibrate();
                yield return new WaitForSeconds(1.2f);
            }    
        }

        /// <summary>
        /// 전화 온 사람 이름 정보 세팅
        /// </summary>
        public void SetPhoneCallInfo(ScriptRow __row)
        {
            callName.text = GameManager.main.GetNotationName(__row);
            calledName.text = GameManager.main.GetNotationName(__row);
        }

        #endregion

        #region 메신저

        /// <summary>
        /// 메시지 도착 연출
        /// </summary>
        /// <param name="completeCallback">GameManager.main.isThreadHold = false;</param>
        /// <param name="speaker">발신자</param>
        public void ReceiveMessage(Action completeCallback, string speaker)
        {
            float animTime = 0.7f;

            phoneOverlay.SetActive(true);
            phoneImage.SetActive(true);
            phoneCall.SetActive(false);
            messengerOverlay.gameObject.SetActive(true);

            // 핸드폰 위치 초기화
            phoneImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1350);

            // 투명도 0
            messengerOverlay.color = new Color(messengerOverlay.color.r, messengerOverlay.color.g, messengerOverlay.color.b, 0f);
            messengerIcon.color = new Color(messengerIcon.color.r, messengerIcon.color.g, messengerIcon.color.b, 0f);
            messageSender.color = new Color(messageSender.color.r, messageSender.color.g, messageSender.color.b, 0f);
            messageAlert.color = new Color(messageAlert.color.r, messageAlert.color.g, messageAlert.color.b, 0f);

            // 발신자 string 값 세팅
            messageSender.text = speaker;

            Sequence alert = DOTween.Sequence();
            alert.Append(messengerOverlay.DOFade(1f, animTime)).Join(phoneImage.GetComponent<RectTransform>().DOAnchorPosY(0f, animTime).SetEase(Ease.OutBack));
            alert.Join(messengerOverlay.rectTransform.DOAnchorPosY(100f, animTime).SetDelay(animTime * 0.25f));
            alert.Join(messengerIcon.DOFade(1f, animTime));
            alert.Join(messageSender.DOFade(1f, animTime));
            alert.Join(messageAlert.DOFade(1f, animTime));

            alert.OnComplete(() => completeCallback());
        }

        /// <summary>
        /// 메신저 관련 만듬
        /// </summary>
        /// <param name="template">템플릿 분류</param>
        /// <param name="row"></param>
        public void CreateForMeesenger(string template, ScriptRow row)
        {
            messengerObject = null;

            switch (template)
            {
                case GameConst.TEMPLATE_MESSAGE_CALL:
                    // 메신저 알람은 터치 없이 행넘김. string 값만 입력해줌
                    messengerObject = Instantiate(messenger_call, messengerContent.transform);
                    messengerText = messengerObject.GetComponentInChildren<TextMeshProUGUI>();
                    
                    SystemManager.SetText(messengerText, row.script_data);
                    GameManager.main.isWaitingScreenTouch = false;
                    break;

                case GameConst.TEMPLATE_MESSAGE_PARTNER:
                    SetMessageTalker(row, messenger_partner, senderBubble);
                    break;

                case GameConst.TEMPLATE_MESSAGE_SELF:
                    SetMessageTalker(row, messenger_self, receiverBubble);
                    break;

                case GameConst.TEMPLATE_MESSAGE_IMAGE:
                    messengerObject = Instantiate(messenger_image, messengerContent.transform);
                    Image img = messengerObject.GetComponentInChildren<Image>();
                    if (GameManager.main.DictMinicutImages.ContainsKey(row.script_data) && GameManager.main.DictMinicutImages[row.script_data] != null)
                        GameManager.main.DictMinicutImages[row.script_data].LoadImage();

                    if (GameManager.main.DictMinicutImages[row.script_data].sprite != null)
                        img.sprite = GameManager.main.DictMinicutImages[row.script_data].sprite;
                    else
                        GameManager.ShowMissingComponent(row.template, row.script_data);

                    break;
            }

            if (template.Equals(GameConst.TEMPLATE_MESSAGE_IMAGE) || template.Equals(GameConst.TEMPLATE_MESSAGE_CALL))
                messengerData.Add(messengerObject);
            else
                messengerData.Add(speakerPrefab);

            if (msgBubbleParent != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(msgBubbleParent.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(speakerPrefab.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(messengerContent.GetComponent<RectTransform>());
            }

            Canvas.ForceUpdateCanvases();
            ForceScrollDown();
        }

        /// <summary>
        /// 템플릿이 메시지상대, 메시지본인 일때만 실행되는 함수
        /// </summary>
        /// <param name="messageType">메시지 본인, 상대 일때의 prefab</param>
        /// <param name="bubbleType">메시지 본인, 상대 일 때의 말풍선 타입 prefab</param>
        void SetMessageTalker(ScriptRow row, GameObject messageType, GameObject bubbleType)
        {

            if (!string.IsNullOrEmpty(row.controlAlternativeName))
                msgCurrSpeaker = row.controlAlternativeName;
            else
            {
                if (!string.IsNullOrEmpty(row.speaker))
                    msgCurrSpeaker = row.speaker;
                else
                    msgCurrSpeaker = string.Format("{0}", "(Unknown)");
            }

            // 이전에 말한 사람과 전에 말한 화자가 같은 사람이냐?
            bool sameSpeaker = msgCurrSpeaker.Equals(msgPrevSpeaker) ? true : false;
            // 확인했으니 화자 덮어씌우기
            msgPrevSpeaker = msgCurrSpeaker;
            msgBubbleParent = null;

            if (!sameSpeaker)
            {
                // 화자의 첫 등장이거나 이전 화자와 다르면 speakerPrefab을 담아두는 변수에 새로 설정해준다.
                speakerPrefab = null;
                speakerPrefab = Instantiate(messageType, messengerContent.transform);
                MessengerUserInfo userInfo = speakerPrefab.GetComponent<MessengerUserInfo>();
                Sprite s = null;

                // 이모티콘 표현이 있고, 이모티콘이 존재하면 프로필 사진을 넣어준다
                if (!string.IsNullOrEmpty(row.emoticon_expression) && GameManager.main.GetEmoticonSprite(row.emoticon_expression) != null)
                    s = GameManager.main.GetEmoticonSprite(row.emoticon_expression).sprite;

                userInfo.SetMessengerForm(s, msgCurrSpeaker);

                msgBubbleParent = userInfo.bubblesParent;
            }
            else
            {
                // 이전 화자가 말하고 있는 것이기 때문에 말풍선만 추가해준다.
                MessengerUserInfo existUser = speakerPrefab.GetComponent<MessengerUserInfo>();
                msgBubbleParent = existUser.bubblesParent;
            }

            messengerObject = Instantiate(bubbleType, msgBubbleParent);
            messengerText = messengerObject.GetComponentInChildren<TextMeshProUGUI>();

            // 메신저 말풍선이 한 줄에 16자 이상이 들어가면 자동으로 잘라서 엔터를 해준다.
            // 22.01.15
            // 영어는 단어가 길어서 단어별로 판별해서 잘라서 엔터를 해줘야 한다
            string enterAcess = row.script_data.Replace("\\", "\n");
            string[] lineStr = enterAcess.Split(' ');
            int lineStringLength = 0;
            
            messengerText.text = string.Empty;

            foreach (string s in lineStr)
            {
                // 엔터를 만나면 엔터로 split하여 마지막 남은 단어로 글자수를 넣어줌
                if (s.Contains("\n"))
                {
                    string[] enterSplit = s.Split('\n');
                    lineStringLength = enterSplit[enterSplit.Length - 1].Length;
                }

                lineStringLength += s.Length + 1;
                messengerText.text += s + " ";

                // 해당 라인이 18자가 넘으면 엔터넣어주기
                if (lineStringLength + s.Length > 18)
                {
                    messengerText.text += "\n";
                    lineStringLength = 0;
                }

                
                /*
                if (s.Length > 16)
                {
                    for (int i = 0; i < s.Length / 16; i++)
                        tmp = s.Insert(16 * (i + 1), "\n");
                }

                if (string.IsNullOrEmpty(tmp))
                    messengerText.text = messengerText.text.Insert(messengerText.text.Length, s + "\n");
                else
                    messengerText.text = messengerText.text.Insert(messengerText.text.Length, tmp);
                */
            }
            
            SystemManager.SetText(messengerText, messengerText.text);

            // 이 또한 대화이므로 게임로그에 데이터를 기입한다.
            CreateTalkLog(row.template, GameManager.main.GetNotationName(row), row.script_data);
        }

        public void SetMessengerScrollBar()
        {
            if (sr == null)
                sr = messenger.GetComponent<ScrollRect>();
        }

        void ForceScrollDown()
        {
            StartCoroutine(RoutineScrollDown());
        }

        /// <summary>
        /// 메신저 내용이 올때마다 스크롤 제일 아래로 내린다
        /// </summary>
        IEnumerator RoutineScrollDown()
        {
            yield return null;
            sr.verticalNormalizedPosition = 0f;
        }

        /// <summary>
        /// 다시하기 등에서 내용을 전부 지운다
        /// </summary>
        public void DestoryAllContents()
        {
            if (messengerData == null)
                return;

            foreach (GameObject g in messengerData)
                Destroy(g);

            messengerData.Clear();
            messenger.SetActive(false);
            msgPrevSpeaker = string.Empty;
            msgCurrSpeaker = string.Empty;
        }

        #endregion

        #endregion

        #region 장소 진입

        /// <summary>
        /// 장소 진입 템플릿용 장소 태그
        /// </summary>
        /// <param name="placeLabel">태그 안에 들어갈 텍스트 값</param>
        public void PlaceNameAnim(Sequence placeTag, string placeLabel)
        {
            string textEpisodeNum = string.Empty; 
            
            episodeNum.text = string.Empty;
            placeNameText.text = string.Empty;
            
            // * 장소진입  태그는 에피소드 타입마다 다르다. 
            switch(GameManager.main.currentEpisodeData.episodeType) {
                case EpisodeType.Chapter:
                textEpisodeNum = string.Format("EPISODE. {0}", GameManager.main.currentEpisodeData.episodeNO);
                break;
                
                case EpisodeType.Side:
                textEpisodeNum = "SPECIAL EPISODE";
                break;
                
                case EpisodeType.Ending:
                textEpisodeNum = "ENDING";
                break;
            }
            episodeNum.text = textEpisodeNum;
            
            // 투명도 0으로 만들어서 안보이게 하기
            placeTextBG.color = new Color(placeTextBG.color.r, placeTextBG.color.g, placeTextBG.color.b, 0f);
            episodeNum.color = new Color(episodeNum.color.r, episodeNum.color.g, episodeNum.color.b, 1f);
            placeNameText.color = new Color(placeNameText.color.r, placeNameText.color.g, placeNameText.color.b, 1f);


            // 스킵인 경우 여길 아예 타지 맙시다
            if (GameManager.main.useSkip)
                return;

            placeTag.Join(placeTextBG.DOFade(1f, 1f).SetDelay(1f).OnStart(() =>
            {
                placeTextBG.gameObject.SetActive(true);
            }));

            // 동일한 속도로 글자 타이핑 애니메이션
            float animTime = 0.1f;

            // 영어권에서는 2배 더 빠르게!
            if (SystemManager.main.currentAppLanguageCode == CommonConst.COL_EN
                 || SystemManager.main.currentAppLanguageCode == CommonConst.COL_AR)
                animTime = 0.04f;

            placeTag.Append(placeNameText.DOText(placeLabel, placeLabel.Length * animTime).SetEase(Ease.Linear));
            placeTag.Append(placeTextBG.DOFade(0f, 2f).SetDelay(1f)).Join(episodeNum.DOFade(0f, 2f)).Join(placeNameText.DOFade(0f, 2f));
        }

        #endregion

        #region GameLog 관련

        /// <summary>
        /// 로그 활성화
        /// </summary>
        public void ShowLog()
        {
            // 로그 창을 오픈합니다.
            logPanel.SetActive(true);
            inGameMenuBtn.SetActive(false);
            closeLogBtn.SetActive(true);

            // 로그 스크롤을 최하단으로 내리기
            logScrollRect.verticalNormalizedPosition = 0f;
            
            SystemManager.SetText(logText, logText.text);
        }

        public void CreateNarrationLog(string __data)
        {
            if (string.IsNullOrEmpty(__data))
                return;

            prevSpeaker = string.Empty;
            __data = __data.Replace("\\", " ");
            logData.Append("\n<i>" + __data + "</i>\n");

            logText.text = logData.ToString();
        }

        /// <summary>
        /// 선택지 로그 생성하기 
        /// </summary>
        public void CreateSelectionLog(string __selected)
        {
            selected = __selected;

            logData.Append("\n");

            // 선택한 선택지와 아닌것을 구분하기 
            foreach (string s in selections)
            {
                if (s.Equals(selected))
                    logData.Append("<b><color=#FF0080>[" + s + "]</color></b>");
                else
                    logData.Append("[" + s + "]");
            }

            prevSpeaker = string.Empty;
            logData.Append("\n");
            logText.text = logData.ToString();

            // 재사용을 위해 비워준다.
            selections.Clear();
        }

        /// <summary>
        /// 선택지 정보를 미리 수집해놓습니다. (DoAction 에서)
        /// </summary>
        public void CollectSelections(string __data)
        {
            selections.Add(__data);
        }


        /// <summary>
        /// Log 만들기
        /// </summary>
        /// <param name="template"></param>
        /// <param name="speaker"></param>
        /// <param name="__data"></param>
        public void CreateTalkLog(string template, string speaker, string __data)
        {
            if (string.IsNullOrEmpty(__data))
                return;

            if (string.IsNullOrEmpty(prevSpeaker) || !prevSpeaker.Equals(speaker))
            {
                prevSpeaker = speaker;
                
                // 이름 로컬라이징.
                logData.Append("\n<size=26><color=#404040>" + StoryManager.main.GetNametagName(speaker) + "</color></size>\n");
            }

            __data = __data.Replace("\\", " ");

            switch (template)
            {
                case GameConst.TEMPLATE_TALK:
                    logData.Append(__data + "\n");
                    break;
                case GameConst.TEMPLATE_YELL:       // 외침
                    logData.Append("<b>" + __data + "</b>\n");
                    break;
                case GameConst.TEMPLATE_WHISPER:    // 속삭임
                    logData.Append("<color=#404040B3>" + __data + "</color>\n");
                    break;
                case GameConst.TEMPLATE_FEELING:    // 속마음
                    logData.Append("<color=#999999>'" + __data + "'</color>\n");
                    break;
                case GameConst.TEMPLATE_MONOLOGUE:  // 독백
                    logData.Append("<color=#999999B3>" + __data + "</color>\n");
                    break;
                case GameConst.TEMPLATE_SPEECH:     // 중요대사
                    logData.Append("<u>" + __data + "</u>\n");
                    break;

                case GameConst.TEMPLATE_PHONE_SELF:
                case GameConst.TEMPLATE_PHONE_PARTNER:
                    logData.Append("<color=#FF73A5>" + __data + "</color>\n");
                    break;
                case GameConst.TEMPLATE_MESSAGE_SELF:
                case GameConst.TEMPLATE_MESSAGE_PARTNER:
                    logData.Append("<color=#006FBF>" + __data + "</color>\n");
                    break;
            }

            logText.text = logData.ToString();
        }

        public void CleanGameLog()
        {
            logText.text = string.Empty;
            logData.Clear();
        }


        /// <summary>
        /// 게임 로그 비활성화
        /// </summary>
        public void DisableGameLog()
        {
            logPanel.SetActive(false);

            inGameMenuBtn.SetActive(true);
            closeLogBtn.SetActive(false);
            
            
            // 로그를 비활성화할때 썼던 text를 또 쓰기 때문에 다시 한번 호출한다.
            // 아랍어 역순 배열때문에 사용
            SystemManager.SetText(logText, logText.text);
        }

        #endregion
        
        #region 음성 재생 중 아이콘 연출

        public void ActiveMicrophoneIcon()
        {
            microphoneIcon.SetActive(true);

            if(microphoneAnimator == null)
                microphoneAnimator = microphoneIcon.GetComponent<Animator>();
            
            microphoneAnimator.SetTrigger("Loop");
        }

        public void InactiveMicrophoneIcon()
        {
            microphoneIcon.SetActive(false);
        }

        #endregion
        
    }
}