using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;
using DG.Tweening;

namespace PIERStory
{
    public class ViewGame : CommonView, IPointerClickHandler
    {
        public static ViewGame main = null;     // UI singleton

        [Header("��ǳ��")]
        public RectTransform bubbleParent;          // ��ǳ�� �θ�
        public List<GameBubbleCtrl> ListBubbles;    // ��ǳ����!
        public GameBubbleCtrl partnerBubble;        // �߽��� ��ǳ��(��ȭ��)
        public GameBubbleCtrl selfBubble;           // ������ ��ǳ��(��ȭ��)
        int bubbleIndex = 0;                        // ��ǳ�� pooling �ε���

        [Header("�����̼�")]
        Action OnNarraion = delegate { };
        public Sprite typeBlackSprite;
        public Sprite typeWhiteSprite;

        public Image boxImage;
        public TextMeshProUGUI textNarration;
        string narrationText = string.Empty;

        [Space] [Header("**���ĵ� ĳ���Ϳ� ����**")]
        public Transform[] modelRenderParents;  // 0 : �̻�� or �߾� �����, 1 : 2�ν��ĵ��� �� ���ϴ� ���, 2 : 2�ν��ĵ��� ���ϴ� ���
        public RawImage[] modelRenders;         // 0 : L, 1 : C, 2: R

        [Space]
        public Image selectionTutorialText;     // ������ Ʃ�丮�� �ȳ�����

        [Space][Header("Flow time")]
        public Image fadeImage;
        public TextMeshProUGUI flowTimeText;

        [Header("**��� ���Կ� Label**")]
        public Image placeTextBG;
        public TextMeshProUGUI episodeNum;
        public Image placeTagContour;
        public TextMeshProUGUI placeNameText;

        [Space][Space][Header("�޴��� ���")]
        [Tooltip("��ȭ,�޽��� ���� �� �̹���")]
        public Image phoneImage;

        [Header("***��ȭ***")]
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


        [Header("***�޽���***")]
        public GameObject messenger;
        public Image messengerOverlay;
        public Image messengerIcon;
        public TextMeshProUGUI messageSender;
        public TextMeshProUGUI messageAlert;

        [Tooltip("�޽��� ScrollView �ȿ� ����ִ� content")]
        public GameObject messengerContent;

        [Header("Messenger Prefab")]
        public GameObject messenger_call;
        public GameObject messenger_image;
        public GameObject messenger_partner;
        public GameObject messenger_self;

        public GameObject senderBubble;
        public GameObject receiverBubble;

        GameObject speakerPrefab;
        GameObject messengerObject;         // �޽������� ���� ������Ʈ��
        Transform msgBubbleParent;          // �� ��ǳ���� �θ� transform
        TextMeshProUGUI messengerText;      // �޽������� ���Ǵ� �ؽ�Ʈ���� ��� ����
        List<GameObject> messengerData = new List<GameObject>();
        ScrollRect sr;
        string msgPrevSpeaker = string.Empty;       // �޽������� ���� ���ߴ� ���
        string msgCurrSpeaker = string.Empty;       // �޽������� ���� ���ϰ� �ִ� ���

        [Header("���ӷα�")]
        public GameObject logPanel;
        [SerializeField] ScrollRect logScrollRect;
        public TextMeshProUGUI logText;
        string prevSpeaker = string.Empty;

        public GameObject inGameMenuBtn;        // �ΰ��Ӹ޴� ��ư
        public GameObject closeLogBtn;          // �α� �г� ���� ���� ��ư

        StringBuilder logData = new StringBuilder();
        List<string> selections = new List<string>();
        string selected = string.Empty;

        [Space]
        [Header("���� ǥ��")]
        public GameObject microphoneIcon;
        Animator microphoneAnimator;

        private void Awake()
        {
            main = this;
        }

        public override void OnView()
        {
            base.OnView();
            StoryManager.enterGameScene = true;

            // ȭ�� ũ�⿡ ���缭 ���ĵ�ĳ���� renderer ������ ����
            float rawImageSize = (float)Screen.height / (float)Screen.width * 900f;
            rawImageSize = Mathf.Clamp(rawImageSize, 1600f, 2000f);

            for (int i = 0; i < modelRenders.Length; i++)
                modelRenders[i].GetComponent<RectTransform>().sizeDelta = new Vector2(rawImageSize, rawImageSize);
        }

        public override void OnStartView()
        {
            base.OnStartView();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // ĳ���� ���� �߿� �Է� ���� ���� 
            /*
            if (!GameManager.main.CheckModelMoveComlete())
            {
                Debug.Log(">> model is moving << ");
                return;
            }
            */

            // �α� �г� Ȱ��ȭ �߿� �Է� ���� ����.   
            if (logPanel.activeSelf)
                return;

            // threadHold �߿� �Է� ���� ����
            if (GameManager.main.isThreadHold)
            {
                Debug.Log(">> holding thread now <<");
                return;
            }

            // touch waiting �ƴ� ��� �Է� ���� ����.
            if (!GameManager.main.isWaitingScreenTouch)
                return;

            // �Է� �޾Ҿ��!
            GameManager.main.isWaitingScreenTouch = false;
        }

        #region ��ǳ�� ����

        /// <summary>
        /// ȭ����� ��� ��ǳ��(��ȭ ��ǳ�� ����), �����̼� ���� 
        /// </summary>
        public void HideBubbles()
        {
            HideNarration();

            for (int i = 0; i < ListBubbles.Count; i++)
            {
                if (ListBubbles[i].gameObject.activeSelf)
                    ListBubbles[i].OffBubble(GameManager.main.useSkip);
            }

            if (selfBubble.gameObject.activeSelf)
                selfBubble.gameObject.SetActive(false);

            if (partnerBubble.gameObject.activeSelf)
                partnerBubble.gameObject.SetActive(false);
        }

        /// <summary>
        /// ��ȭ �� ��ǳ�� ��ġ ����
        /// </summary>
        /// <param name="isSelf">true�� ��� ��ȭ����, false�� ��� ��ȭ���</param>
        public void SetPhoneProcess(ScriptRow __row, Action __cb, bool isSelf)
        {
            // �Էµ� ������ ��ǳ�� ������ üũ�� ���� üũ
            if (__row.bubble_size < 1)
                BubbleManager.main.SetFakeBubbles(__row);

            if (isSelf)
                selfBubble.ForPhoneBubble(__row, __cb, 9, isSelf);
            else
                partnerBubble.ForPhoneBubble(__row, __cb, 4, isSelf);
        }

        /// <summary>
        /// ��ȭ ���ø��� ��ǳ�� �����
        /// </summary>
        /// <param name="isSelf">true�� ������, false�� �߽���</param>
        public void HidePhoneBubble(bool isSelf)
        {
            if (isSelf)
                selfBubble.PhoneBubbleOff(isSelf);
            else
                partnerBubble.PhoneBubbleOff(isSelf);
        }

        #endregion

        #region �����̼� ����

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

            // color �� ����
            boxImage.color = CommonConst.COLOR_IMAGE_TRANSPARENT;
            textNarration.color = new Color(textNarration.color.r, textNarration.color.b, textNarration.color.g, 0); // �����ϰ� ������ش�.

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

        #region �ð��帧 ����

        /// <summary>
        /// �ð� �帧 ���� �ؽ�Ʈ ����
        /// </summary>
        /// <param name="labelText">�󺧿� �� string</param>
        /// <param name="voice">���� �÷���</param>
        /// <param name="reversal">������ ����ϴ°�?</param>
        /// <param name="isBackground">���� ���� ���, �������, �Ϸ���Ʈ�ΰ�?</param>
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

            // script_data�� null���� �ƴ϶��
            if (!string.IsNullOrEmpty(labelText))
            {
                if (labelText.Contains("\\"))
                    labelText = labelText.Replace("\\", "\n");


                if (!string.IsNullOrEmpty(voice))
                    GameManager.main.SoundGroup[1].PlayVoice(voice);

                for (int i = 0; i < labelText.Length; i++)
                {
                    string tmpStr = string.Empty;

                    // ���� ���ڰ� Ư�������� ��� �� ���ڷ� ����Ѵ�
                    if (i + 1 < labelText.Length && System.Text.RegularExpressions.Regex.IsMatch(labelText[i + 1].ToString(), @"[~!@\#$%^&*\()\=+|\\/:;,?""<>']"))
                        tmpStr = labelText[i + 1].ToString();

                    flowTimeText.text += (labelText[i] + tmpStr);

                    if (!string.IsNullOrEmpty(tmpStr))
                        i++;

                    yield return new WaitForSeconds(0.1f);
                }

                // 21.10.22
                // ������ ������̶�� ���� ����� ������ ��ٷ�!
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
        /// �ð��帧, Animation callback
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

        #region �ڵ��� ��� ����

        public void HIdePhoneImage()
        {
            phoneImage.gameObject.SetActive(false);
            phoneCall.SetActive(false);
            messengerOverlay.gameObject.SetActive(false);

            callBackground.gameObject.SetActive(false);
        }

        #region ��ȭ

        /// <summary>
        /// �ڵ��� ȭ�� ���� �ڷ�ƾ. ��ư�� �����ؼ� ���
        /// </summary>
        public void PhoneCallButton()
        {
            timeEnd = false;

            // fade �� ���� 0
            callBackground.color = new Color(callBackground.color.r, callBackground.color.g, callBackground.color.b, 0f);
            callIcon.color = new Color(callIcon.color.r, callIcon.color.g, callIcon.color.b, 0f);
            calledName.color = new Color(calledName.color.r, calledName.color.g, calledName.color.b, 0f);
            callTime.color = new Color(callTime.color.r, callTime.color.g, callTime.color.b, 0f);
            callBackground.gameObject.SetActive(true);

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

                //��ȭ�ð� �����ϱ�
                StartCoroutine(PhoneTimer());
            });
        }


        /// <summary>
        /// ��ȭ�� Ȱ��ȭ Ȥ�� ��ȭ ȭ�� Ȱ��ȭ
        /// </summary>
        public void ActivePhoneImage(string template)
        {
            if (template.Equals(GameConst.TEMPLATE_PHONECALL))
            {
                phoneImage.gameObject.SetActive(true);
                phoneCall.SetActive(true);

                timeEnd = true;

                // ��ȭ �ɷ����� �� ����
                if (!GameManager.main.useSkip)
                    StartCoroutine(RoutinePhoneEnter());
            }
            else
            {
                if (phoneCall.activeSelf)
                {
                    phoneImage.gameObject.SetActive(false);
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

                // ȭ�鿡 Ȱ��ȭ ���� ���� ����
                // �޸� ��ȭ�� �ƿ� ������� ���� ������ timeEnd�� ������ ���� ���� ���� �� �𸣰���
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
            // ��ġ �ʱ�ȭ
            phoneImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1150);
            phoneImage.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.7f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.7f);
            Handheld.Vibrate();

            StartCoroutine(PhoneCallCircleMove());
        }

        /// <summary>
        /// ��ȭ ��ư�� ���� ó��
        /// </summary>
        IEnumerator PhoneCallCircleMove()
        {
            const float animTime = 0.12f;
            int circleCount = 0;
            bool isFirstTime = true;

            // ��ȭ ���� ������ ��� ����
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
        /// ��ȭ �� ��� �̸� ���� ����
        /// </summary>
        public void SetPhoneCallInfo(ScriptRow __row)
        {
            callName.text = GameManager.main.GetNotationName(__row);
            calledName.text = GameManager.main.GetNotationName(__row);
        }

        #endregion

        #region �޽���

        /// <summary>
        /// �޽��� ���� ����
        /// </summary>
        /// <param name="completeCallback">GameManager.main.isThreadHold = false;</param>
        /// <param name="speaker">�߽���</param>
        public void ReceiveMessage(Action completeCallback, string speaker)
        {
            float animTime = 0.7f;
            phoneImage.gameObject.SetActive(true);
            phoneCall.SetActive(false);
            messengerOverlay.gameObject.SetActive(true);

            // �ڵ��� ��ġ �ʱ�ȭ
            phoneImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1350);

            // ���� 0
            messengerOverlay.color = new Color(messengerOverlay.color.r, messengerOverlay.color.g, messengerOverlay.color.b, 0f);
            messengerIcon.color = new Color(messengerIcon.color.r, messengerIcon.color.g, messengerIcon.color.b, 0f);
            messageSender.color = new Color(messageSender.color.r, messageSender.color.g, messageSender.color.b, 0f);
            messageAlert.color = new Color(messageAlert.color.r, messageAlert.color.g, messageAlert.color.b, 0f);

            // �߽��� string �� ����
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
        /// �޽��� ���� ����
        /// </summary>
        /// <param name="template">���ø� �з�</param>
        /// <param name="row"></param>
        public void CreateForMeesenger(string template, ScriptRow row)
        {
            messengerObject = null;

            switch (template)
            {
                case GameConst.TEMPLATE_MESSAGE_CALL:
                    // �޽��� �˶��� ��ġ ���� ��ѱ�. string ���� �Է�����
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
        /// ���ø��� �޽������, �޽������� �϶��� ����Ǵ� �Լ�
        /// </summary>
        /// <param name="messageType">�޽��� ����, ��� �϶��� prefab</param>
        /// <param name="bubbleType">�޽��� ����, ��� �� ���� ��ǳ�� Ÿ�� prefab</param>
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

            // ������ ���� ����� ���� ���� ȭ�ڰ� ���� ����̳�?
            bool sameSpeaker = msgCurrSpeaker.Equals(msgPrevSpeaker) ? true : false;
            // Ȯ�������� ȭ�� ������
            msgPrevSpeaker = msgCurrSpeaker;
            msgBubbleParent = null;

            if (!sameSpeaker)
            {
                // ȭ���� ù �����̰ų� ���� ȭ�ڿ� �ٸ��� speakerPrefab�� ��Ƶδ� ������ ���� �������ش�.
                speakerPrefab = null;
                speakerPrefab = Instantiate(messageType, messengerContent.transform);
                MessengerUserInfo userInfo = speakerPrefab.GetComponent<MessengerUserInfo>();
                Sprite s = null;

                // �̸�Ƽ�� ǥ���� �ְ�, �̸�Ƽ���� �����ϸ� ������ ������ �־��ش�
                if (!string.IsNullOrEmpty(row.emoticon_expression) && GameManager.main.GetEmoticonSprite(row.emoticon_expression) != null)
                    s = GameManager.main.GetEmoticonSprite(row.emoticon_expression).sprite;

                userInfo.SetMessengerForm(s, msgCurrSpeaker);

                msgBubbleParent = userInfo.bubblesParent;
            }
            else
            {
                // ���� ȭ�ڰ� ���ϰ� �ִ� ���̱� ������ ��ǳ���� �߰����ش�.
                MessengerUserInfo existUser = speakerPrefab.GetComponent<MessengerUserInfo>();
                msgBubbleParent = existUser.bubblesParent;
            }

            messengerObject = Instantiate(bubbleType, msgBubbleParent);
            messengerText = messengerObject.GetComponentInChildren<TextMeshProUGUI>();

            // �޽��� ��ǳ���� �� �ٿ� 16�� �̻��� ���� �ڵ����� �߶� ���͸� ���ش�.
            string[] lineStr = row.script_data.Split('\\');
            messengerText.text = string.Empty;

            foreach (string s in lineStr)
            {
                string tmp = string.Empty;

                if (s.Length > 16)
                {
                    for (int i = 0; i < s.Length / 16; i++)
                        tmp = s.Insert(16 * (i + 1), "\n");
                }

                if (string.IsNullOrEmpty(tmp))
                    messengerText.text = messengerText.text.Insert(messengerText.text.Length, s + "\n");
                else
                    messengerText.text = messengerText.text.Insert(messengerText.text.Length, tmp);
            }

            // �� ���� ��ȭ�̹Ƿ� ���ӷα׿� �����͸� �����Ѵ�.
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
        /// �޽��� ������ �ö����� ��ũ�� ���� �Ʒ��� ������
        /// </summary>
        IEnumerator RoutineScrollDown()
        {
            yield return null;
            sr.verticalNormalizedPosition = 0f;
        }

        /// <summary>
        /// �ٽ��ϱ� ��� ������ ���� �����
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

        #region ��� ����

        /// <summary>
        /// ��� ���� ���ø��� ��� �±�
        /// </summary>
        /// <param name="placeLabel">�±� �ȿ� �� �ؽ�Ʈ ��</param>
        public void PlaceNameAnim(Sequence placeTag, string placeLabel)
        {
            episodeNum.text = string.Empty;
            placeNameText.text = string.Empty;

            if (SystemManager.GetJsonNodeString(GameManager.main.currentEpisodeJson, CommonConst.COL_EPISODE_TYPE).Equals(CommonConst.COL_CHAPTER))
            {
                if (!string.IsNullOrEmpty(SystemManager.GetJsonNodeString(GameManager.main.currentEpisodeJson, CommonConst.COL_EPISODE_NO)))
                    episodeNum.text = string.Format("EPISODE. {0}", SystemManager.GetJsonNodeString(GameManager.main.currentEpisodeJson, CommonConst.COL_EPISODE_NO));
            }
            else
            {
                // ���� ���Ǽҵ尡 �ƴϸ� episode type�� ������ش�
                episodeNum.text = SystemManager.GetJsonNodeString(GameManager.main.currentEpisodeJson, CommonConst.COL_EPISODE_TYPE).ToUpper();

                if (SystemManager.GetJsonNodeString(GameManager.main.currentEpisodeJson, CommonConst.COL_EPISODE_TYPE).Equals(CommonConst.COL_SIDE))
                    episodeNum.text = "SPECIAL EPISODE";
            }

            // ���� 0���� ���� �Ⱥ��̰� �ϱ�
            placeTextBG.color = new Color(placeTextBG.color.r, placeTextBG.color.g, placeTextBG.color.b, 0f);
            episodeNum.color = new Color(episodeNum.color.r, episodeNum.color.g, episodeNum.color.b, 1f);
            placeTagContour.color = new Color(placeTagContour.color.r, placeTagContour.color.g, placeTagContour.color.b, 1f);
            placeNameText.color = new Color(placeNameText.color.r, placeNameText.color.g, placeNameText.color.b, 1f);

            placeTag.Join(placeTextBG.DOFade(1f, 1f).SetDelay(1f).OnStart(() =>
            {
                // ��ŵ�� �ƴ� ��쿡�� Ȱ��ȭ �Ѵ�
                if (!GameManager.main.useSkip)
                    placeTextBG.gameObject.SetActive(true);
            }));

            // ������ �ӵ��� ���� Ÿ���� �ִϸ��̼�
            placeTag.Append(placeNameText.DOText(placeLabel, placeLabel.Length * 0.1f).SetEase(Ease.Linear));
            placeTag.Append(placeTextBG.DOFade(0f, 2f).SetDelay(1f)).Join(episodeNum.DOFade(0f, 2f)).Join(placeTagContour.DOFade(0f, 2f)).Join(placeNameText.DOFade(0f, 2f));
        }

        #endregion

        #region GameLog ����

        /// <summary>
        /// �α� Ȱ��ȭ
        /// </summary>
        public void ShowLog()
        {
            // �α� â�� �����մϴ�.
            logPanel.SetActive(true);
            inGameMenuBtn.SetActive(false);
            closeLogBtn.SetActive(true);

            // �α� ��ũ���� ���ϴ����� ������
            logScrollRect.verticalNormalizedPosition = -1f;
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
        /// ������ �α� �����ϱ� 
        /// </summary>
        public void CreateSelectionLog(string __selected)
        {
            selected = __selected;

            logData.Append("\n");

            // ������ �������� �ƴѰ��� �����ϱ� 
            foreach (string s in selections)
            {
                if (s.Equals(selected))
                    logData.Append("<b><color=#6284FF>[" + s + "]</color></b>");
                else
                    logData.Append("[" + s + "]");
            }

            prevSpeaker = string.Empty;
            logData.Append("\n");
            logText.text = logData.ToString();

            // ������ ���� ����ش�.
            selections.Clear();
        }

        /// <summary>
        /// ������ ������ �̸� �����س����ϴ�. (DoAction ����)
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
                logData.Append("\n<size=26><color=#000000>" + speaker + "</color></size>\n");
            }

            __data = __data.Replace("\\", " ");

            switch (template)
            {
                case GameConst.TEMPLATE_TALK:
                    logData.Append(__data + "\n");
                    break;
                case GameConst.TEMPLATE_YELL:
                    logData.Append("<b>" + __data + "</b>\n");
                    break;
                case GameConst.TEMPLATE_WHISPER:
                    logData.Append("<color=#3D3D3DB3>" + __data + "</color>\n");
                    break;
                case GameConst.TEMPLATE_FEELING:
                    logData.Append("<color=#767676>'" + __data + "'</color>\n");
                    break;
                case GameConst.TEMPLATE_MONOLOGUE:
                    logData.Append("<color=#767676B3>" + __data + "</color>\n");
                    break;
                case GameConst.TEMPLATE_SPEECH:
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
        /// ���� �α� ��Ȱ��ȭ
        /// </summary>
        public void DisableGameLog()
        {
            logPanel.SetActive(false);

            inGameMenuBtn.SetActive(true);
            closeLogBtn.SetActive(false);
        }

        #endregion
    }
}