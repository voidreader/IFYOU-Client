using System;
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
        
        [SerializeField] UIView viewGameMenu; // 게임메뉴. 
        

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
        public Transform[] modelRenderParents;  // 0 : 미사용 or 중앙 사용중, 1 : 2인스탠딩중 말 안하는 사람, 2 : 2인스탠딩중 말하는 사람
        public RawImage[] modelRenders;         // 0 : L, 1 : C, 2: R

        
        [Space][Space][Header("**선택지**")]
        public Image selectionTutorialText;     // 선택지 튜토리얼 안내문구
        public List<ScriptRow> ListSelectionRows = new List<ScriptRow>(); // 현재보여지는 선택지 정보의 스크립트 데이터 
        public List<IFYouGameSelectionCtrl> ListGameSelection = new List<IFYouGameSelectionCtrl>(); // UI에 저장된 선택지들 
        public List<IFYouGameSelectionCtrl> ListAppearSelection = new List<IFYouGameSelectionCtrl>(); // 활성화된 선택지 
        
        
        
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

        public Image callBackground;
        public Image callIcon;
        public TextMeshProUGUI calledName;
        public TextMeshProUGUI callTime;
        public Image phoneCallCircle1;
        public Image phoneCallCircle2;
        public Image phoneCallCircle3;
        bool timeEnd = false;


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
        }


        /// <summary>
        /// 화면 터치 처리 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
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
            
            // Debug.Log("HideBubble Called");
                        
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

            if(countNewLine == 0)
            {
                while (countNewLine * 22 < narrationText.Length)
                    countNewLine++;

                countNewLine--;
            }

            if(countNewLine < 2)
                boxImage.GetComponent<RectTransform>().sizeDelta = new Vector2(boxImage.GetComponent<RectTransform>().sizeDelta.x, 160);
            else
                boxImage.GetComponent<RectTransform>().sizeDelta = new Vector2(boxImage.GetComponent<RectTransform>().sizeDelta.x, 100 + (countNewLine * 60));

            boxImage.gameObject.SetActive(true);
            textNarration.text = narrationText;

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
            
            // Appear 리스트 클리어 
            ListAppearSelection.Clear();
            IFYouGameSelectionCtrl.ListStacks.Clear();

            // 튜토리얼을 보지 않고 스킵해서 선택지 어떻게 누르는지 모르는 바보들을 위해 선택지 꾸욱 눌러야 한다고 문구를 띄워준다.
            /*
            if (UserManager.main.tutorialStep.Equals(2))
                selectionTutorialText.gameObject.SetActive(true);
            else
                selectionTutorialText.gameObject.SetActive(false);
            */

            for (int i = 0; i < ListSelectionRows.Count; i++)
            {
                if(i >= ListGameSelection.Count) {
                    SystemManager.ShowMessageAlert("너무 많은 선택지!(최대 6개)", false);
                    break;
                }
                
                ListGameSelection[i].SetSelection(ListSelectionRows[i], i);
                ListAppearSelection.Add(ListGameSelection[i]); // appear에 추가. 
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
            if (ListAppearSelection.Count == 0) {
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
            if (messenger.activeSelf)
                messenger.SetActive(false);

            
            // * 광고처리 추가 
            AdManager.main.PlaySelectionAD();

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
                    if (SystemManager.main.currentAppLanguageCode == "EN")
                        yield return new WaitForSeconds(0.03f);
                    else
                        yield return new WaitForSeconds(0.1f);
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
            phoneOverlay.SetActive(false);
            phoneImage.SetActive(false);
            phoneCall.SetActive(false);
            messengerOverlay.gameObject.SetActive(false);

            callBackground.gameObject.SetActive(false);
        }

        #region 전화

        /// <summary>
        /// 핸드폰 화면 실행 코루틴. 버튼에 연결해서 사용
        /// </summary>
        public void PhoneCallButton()
        {
            timeEnd = false;

            // fade 전 투명도 0
            callBackground.color = new Color(callBackground.color.r, callBackground.color.g, callBackground.color.b, 0f);
            callIcon.color = new Color(callIcon.color.r, callIcon.color.g, callIcon.color.b, 0f);
            calledName.color = new Color(calledName.color.r, calledName.color.g, calledName.color.b, 0f);
            callTime.color = new Color(callTime.color.r, callTime.color.g, callTime.color.b, 0f);
            callBackground.gameObject.SetActive(true);
            callTime.text = "0:00";

            const float animTime = 0.7f;

            Sequence hangUp = DOTween.Sequence();
            hangUp.Append(phoneImage.GetComponent<RectTransform>().DOAnchorPosY(-1350f, 0.5f));
            hangUp.Append(callBackground.DOFade(1f, animTime));
            hangUp.Join(callIcon.DOFade(1f, animTime));
            hangUp.Join(calledName.DOFade(1f, animTime));
            hangUp.Join(callTime.DOFade(1f, animTime));
            hangUp.OnComplete(() =>
            {

                GameManager.main.isThreadHold = false;
                GameManager.main.isWaitingScreenTouch = false;

                //통화시간 진행하기
                StartCoroutine(PhoneTimer());
            });
        }


        /// <summary>
        /// 전화기 활성화 혹은 전화 화면 활성화
        /// </summary>
        public void ActivePhoneImage(string template)
        {
            if (template.Equals(GameConst.TEMPLATE_PHONECALL))
            {
                phoneOverlay.SetActive(true);
                phoneImage.SetActive(true);
                phoneCall.SetActive(true);
                messengerOverlay.gameObject.SetActive(false);

                timeEnd = true;

                // 전화 걸려왔을 때 연출
                if (!GameManager.main.useSkip)
                    StartCoroutine(RoutinePhoneEnter());
            }
            else
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
            // 위치 초기화
            phoneImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1150);
            phoneImage.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.7f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.7f);
            Handheld.Vibrate();

            StartCoroutine(PhoneCallCircleMove());
        }

        /// <summary>
        /// 통화 버튼쪽 물결 처리
        /// </summary>
        IEnumerator PhoneCallCircleMove()
        {
            const float animTime = 0.12f;
            int circleCount = 0;
            bool isFirstTime = true;

            // 전화 받을 때까지 계속 돈다
            while (phoneCall.activeSelf)
            {
                switch (circleCount)
                {
                    case 0:
                        phoneCallCircle3.DOFade(0f, animTime);
                        if (!isFirstTime)
                            yield return new WaitForSeconds(1f);
                        break;

                    case 1:
                        phoneCallCircle1.DOFade(1f, animTime);
                        break;

                    case 2:
                        phoneCallCircle2.DOFade(1f, animTime);
                        break;

                    case 3:
                        phoneCallCircle3.DOFade(1f, animTime);
                        break;

                    case 4:
                        phoneCallCircle1.DOFade(0f, animTime);
                        break;

                    case 5:
                        phoneCallCircle2.DOFade(0f, animTime);
                        break;

                }

                circleCount++;

                if (circleCount > 5)
                {
                    circleCount = 0;
                    Handheld.Vibrate();

                    if (isFirstTime)
                        isFirstTime = false;
                }

                yield return new WaitForSeconds(animTime);
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
                    messengerText.text = row.script_data;
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
            string enterAcess = row.script_data.Replace("\\", " ");
            string[] lineStr = enterAcess.Split(' ');
            string tmp = string.Empty;
            int lineStringLength = 0;

            messengerText.text = string.Empty;

            foreach (string s in lineStr)
            {
                if (lineStringLength + s.Length > 24)
                {
                    messengerText.text += "\n";
                    lineStringLength = 0;
                }

                lineStringLength += s.Length + 1;
                messengerText.text += s + " ";

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

            placeTag.Join(placeTextBG.DOFade(1f, 1f).SetDelay(1f).OnStart(() =>
            {
                // 스킵이 아닌 경우에만 활성화 한다
                if (!GameManager.main.useSkip)
                    placeTextBG.gameObject.SetActive(true);
            }));

            // 동일한 속도로 글자 타이핑 애니메이션
            float animTime = 0.1f;

            // 영어권에서는 2배 더 빠르게!
            if (SystemManager.main.currentAppLanguageCode == "EN")
                animTime = 0.05f;

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
                    logData.Append("<b><color=#FF73A5>[" + s + "]</color></b>");
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
        }

        #endregion
        
        
        /// <summary>
        /// 상단 터치할때. 
        /// </summary>
        public void OnClickCallGameMenu() {
            
            // * Nody로 연결하지 않음
            // * 종종 Flow가 매끄럽지 않은 경우가 있다.
            
            viewGameMenu.Show();
        }
        
    }
}