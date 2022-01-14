using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class GameBubbleCtrl : MonoBehaviour
    {
        static float smallSizeFactor = 0.8f;

        #region statics 
        static Vector2 centerPivot = new Vector2(0.5f, 0.5f);
        static Vector3 reverseScaleX = new Vector3(-1, 1, 1);

        // 서버 json 컬럼들.. 
        const string COL_TEXTAREA_LEFT = "textarea_left";
        const string COL_TEXTAREA_RIGHT = "textarea_right";
        const string COL_TEXTAREA_TOP = "textarea_top";
        const string COL_TEXTAREA_BOTTOM = "textarea_bottom";

        const string COL_POS_X = "pos_x";
        const string COL_POS_Y = "pos_y";
        const string COL_BUBBLE_SPRITE_ID = "bubble_sprite_id";
        const string COL_OUTLINE_SPRITE_ID = "outline_sprite_id";
        const string COL_SCALE_X = "scale_x";
        const string COL_SCALE_Y = "scale_y";
        const string COL_TAIL_SPRITE_ID = "tail_sprite_id";
        const string COL_TAIL_OUTLINE_SPRITE_ID = "tail_outline_sprite_id";
        const string COL_TAIL_POS_X = "tail_pos_x";
        const string COL_TAIL_POS_Y = "tail_pos_y";
        const string COL_TAIL_SCALE_X = "tail_scale_x";
        const string COL_TAIL_SCALE_Y = "tail_scale_y";
        const string COL_REVERSE_TAIL_SPRITE_ID = "reverse_tail_sprite_id";
        const string COL_REVERSE_TAIL_OUTLINE_SPRITE_ID = "reverse_tail_outline_sprite_id";
        const string COL_REVERSE_TAIL_POS_X = "reverse_tail_pos_x";
        const string COL_REVERSE_TAIL_POS_Y = "reverse_tail_pos_y";
        const string COL_REVERSE_TAIL_SCALE_X = "reverse_tail_scale_x";
        const string COL_REVERSE_TAIL_SCALE_Y = "reverse_tail_scale_y";
        const string COL_EMOTICON_POS_X = "emoticon_pos_x";
        const string COL_EMOTICON_POS_Y = "emoticon_pos_y";
        const string COL_EMOTICON_SCALE_X = "emoticon_scale_x";
        const string COL_EMOTICON_SCALE_Y = "emoticon_scale_y";
        const string COL_EMOTICON_WIDTH = "emoticon_width";
        const string COL_EMOTICON_HEIGHT = "emoticon_height";

        const string COL_FONT_COLOR = "font_color";
        const string COL_FILL_COLOR = "fill_color";
        const string COL_OUTLINE_COLOR = "outline_color";
        const string COL_CUSTOM_SIZE_X = "custom_size_x";
        const string COL_CUSTOM_SIZE_Y = "custom_size_y";

        const string COL_TAG_SPRITE_ID = "tag_sprite_id";
        const string COL_TAG_POS_X = "tag_pos_x";
        const string COL_TAG_POS_Y = "tag_pos_y";

        #endregion

        // 제어용 transform
        public RectTransform rtransform;


        public Image imageBubble;           // 몸체 
        public Image imageOutline;          // 외곽
        public Image nametag;               // 이름표 이미지
        public TextMeshProUGUI textName;    // 이름표 텍스트 


        public TextMeshProUGUI textContents;    // 글
        public Image imageTail;                 // 꼬리
        public Image imageTailOutline;          // 꼬리 외곽선 
        public Image emoticon;                  // 이모티콘
        ScriptImageMount emoticonMount;         // 이모티콘 가져오는 ImageMount
        public GameObject fakeReverseTail;
        public bool isFakeBubble = false;
        
        public bool needDelayShow = false;

        string template = string.Empty;
        string message = string.Empty;
        [SerializeField] int bubbleSize = 0;
        [SerializeField] int bubblePos = 0;
        int bubbleReverse = 0;                  // 말꼬리 반전 사용
        int holdingCount = 0;
        ScriptRow row;
        string emoticon_expression = string.Empty;
        string in_effect = string.Empty;
        string out_effect = string.Empty;
        [SerializeField] string speaker = string.Empty;
        string alternativeName = string.Empty; // 대체 이름 

        Vector3 tweenLocalScale = Vector3.zero; // scaleX,Y에 변화가 일어났을때 대신 사용할 Vector3

        int speakerTall = 0;                    // 화자-캐릭터 모델의 키
        bool isStandingSpeak = false;           // 서있는 캐릭터가 말하는거니?
        bool isEmoticonAvailable = false;       // 이모티콘 expression이 있어도, 이 변수에 의해서 사용여부가 결정되도록 한다.
        
        int adjustmentPosY = 0;


        [Space][Header("부가 기능")]
        string options = string.Empty; // 부가 기능!

        [SerializeField] string variation = string.Empty;        // variation 
        JsonData currentBubbleGroup = null;     // 배리에이션에 따른 말풍선 기준정보 
        JsonData targetBubbleJson = null;       // size, pos에 맞는 타겟 JSON Object 행 
        string targetBubbleDataString = string.Empty;



        [Header("Position Scale")]
        float posX = 0;
        float posY = 0;
        float scaleX = 0;
        float scaleY = 0;

        [Header("Tail")]
        float tailPosX = 0;
        float tailPosY = 0;
        float tailScaleX = 0;
        float tailScaleY = 0;
        float reverseTailPosX = 0;
        float reverseTailPosY = 0;
        float reverseTailScaleX = 0;
        float reverseTailScaleY = 0;


        [Header("TextArea")]
        float textAreaTop = 0;
        float textAreaBottom = 0;
        float textAreaLeft = 0;
        float textAreaRight = 0;


        [Header("커스텀 크기")]
        float customSizeX = 0;
        float customSizeY = 0;

        [Header("Colors")]
        string fontColor = string.Empty;
        string fillColor = string.Empty;
        string outlineColor = string.Empty;
        Color ColorFont;
        Color ColorFill;
        Color ColorOutline;

        [Header("Emoticon")]
        float emoticonPosX = 0;
        float emoticonPosY = 0;
        float emoticonScaleX = 0;
        float emoticonScaleY = 0;
        int emoticonWidth = 0;      // 이모티콘 너비 
        int emoticonHeight = 0;     // 이모티콘 높이


        [Header("Nametag")]
        int tagSpriteID = -1;
        float tagPosX = 0; // 네임태그 위치 x
        float tagPosY = 0; // 네임태그 위치 y
        string tagMainColor = "000000FF";
        string tagSubColor = "FFFFFFFF";
        string tagText = string.Empty;


        #region 초기화 처리

        /// <summary>
        /// 초기화 (Pooling으로 쓰니까 초기화 주의)
        /// </summary>
        void InitTransform()
        {
            // transform 초기화 
            isEmoticonAvailable = false;
            rtransform.localScale = Vector3.one;
            rtransform.localEulerAngles = Vector3.zero;
            textContents.transform.localScale = Vector3.one; // textContent localScale은 중요해졌다. 
            nametag.transform.localScale = Vector3.one;

            imageBubble.transform.localScale = Vector3.one;
            emoticon.transform.localScale = Vector3.one;

            fakeReverseTail.SetActive(false);

            ColorFont = Color.black;
            ColorFill = Color.white;
            ColorOutline = Color.black;

            textContents.color = ColorFont;
            textContents.fontStyle = TMPro.FontStyles.Normal;

            InitColor();

            tweenLocalScale = Vector3.one;
        }

        /// <summary>
        /// 색상 초기화 
        /// </summary>
        void InitColor()
        {
            nametag.color = new Color(nametag.color.r, nametag.color.g, nametag.color.b, 1);
            textName.color = new Color(textName.color.r, textName.color.g, textName.color.b, 1);

            imageBubble.color = ColorFill;
            imageTail.color = ColorFill;
            imageOutline.color = ColorOutline;
            imageTailOutline.color = ColorOutline;

            emoticon.color = new Color(emoticon.color.r, emoticon.color.g, emoticon.color.b, 1);
            textContents.color = ColorFont;
        }

        /// <summary>
        /// 텍스트는 지정한 색에서 벗어나지 않게 위해서 따로 한다. 
        /// </summary>
        void SetTransparentTextColor()
        {
            textContents.color = new Color(textContents.color.r, textContents.color.g, textContents.color.b, 0);
            imageOutline.color = new Color(ColorOutline.r, ColorOutline.g, ColorOutline.b, 0);
            nametag.color = new Color(nametag.color.r, nametag.color.g, nametag.color.b, 0);
            textName.color = new Color(textName.color.r, textName.color.g, textName.color.b, 0);
        }

        /// <summary>
        /// 파라매터 세팅
        /// </summary>
        /// <param name="pos">전화 템플릿 말풍선 위치 고정용</param>
        void SetParams(ScriptRow __row, Action __cb, int pos = -1)
        {
            row = __row;
            message = __row.script_data;
            
            needDelayShow = false; 

            if (string.IsNullOrEmpty(message))
            {
                message = " ";
                Debug.Log(">> Message is empty << ");
                __cb?.Invoke();

                return;
            }

            message = message.Replace(@"\", "\n");
            
            template = __row.template;          // 템플릿 
            holdingCount = __row.bubble_hold;   // 유지 횟수
            bubbleSize = row.bubble_size;       // 말풍선 사이즈

            if (pos < 0)
                bubblePos = row.bubble_pos;         // 말풍선 위치
            else
                bubblePos = pos;

            bubbleReverse = row.bubble_reverse; // 말풍선 반전

            emoticon_expression = row.emoticon_expression; // 이모티콘 표현
            in_effect = row.in_effect; // 등장 연출
            out_effect = row.out_effect; // 퇴장 연출

            speaker = row.speaker; // 화자 추가 
            alternativeName = row.controlAlternativeName; // 대체이름 

            // 텍스트 설정 
            textContents.SetText(message);
        }

        #endregion

        #region 말풍선 크기 체크 용도 가짜 말풍선 설정 


        /// <summary>
        /// 말풍선 크기 체크용 가짜 말풍선 세팅하기 
        /// </summary>
        public void SetFakeBubble(ScriptRow __row, int __size)
        {
            isFakeBubble = true;

            SetParams(__row, null);

            bubbleSize = __size; // fake 는 지정한 size로 수동 변경 
            if (bubblePos == 0)
                bubblePos = BubbleManager.GetDefaultBubbleTalkPosIndex();

            // 받은 데이터를 기반으로 말풍선 기준정보를 받아온다. 
            SetBubbleGroup();

            // 서버데이터와 맞추기 
            SetBubbleSync();

            SetTextFontStyle();

            // 말꼬리 추가 설정
            SetTail();

            SetBubbleEmoticon();

            // force update
            textContents.ForceMeshUpdate(true, true);
        }

        #endregion

        #region 말풍선 신규 작업 


        /// <summary>
        /// 말꼬리 추가 설정
        /// </summary>
        void SetTail()
        {
            if (GameManager.main == null)
                return;

            if (GameManager.main.CompareWithCurrentSpeaker(row.speaker))
                return;

            // 화면에 캐릭터 모델이 없고, 이모티콘 표현이 없으면 말꼬리 제거 
            if (!GameManager.main.CompareWithCurrentSpeaker(row.speaker) && string.IsNullOrEmpty(emoticon_expression))
            {
                imageTail.rectTransform.localScale = Vector3.zero;
                return;
            }
        }

        /// <summary>
        /// 기본 폰트 스타일 설정 
        /// </summary>
        void SetTextFontStyle()
        {
            textContents.characterSpacing = 0;

            if (BubbleManager.main == null)
            {
                textContents.fontSize = 28;
                return;
            }

            switch (template)
            {
                case GameConst.TEMPLATE_TALK:
                case GameConst.TEMPLATE_MONOLOGUE:
                case GameConst.TEMPLATE_SPEECH:
                case GameConst.TEMPLATE_FEELING:
                    textContents.fontSize = BubbleManager.main.normalFontSize;
                    break;


                case GameConst.TEMPLATE_YELL:
                    textContents.fontSize = BubbleManager.main.BigFontSize;
                    // textContents.fontStyle = FontStyles.Bold;
                    textContents.characterSpacing = -10;
                    break;

                case GameConst.TEMPLATE_WHISPER:
                    textContents.fontSize = BubbleManager.main.normalFontSize;
                    break;

                case GameConst.TEMPLATE_PHONE_PARTNER:
                case GameConst.TEMPLATE_PHONE_SELF:
                    textContents.fontSize = BubbleManager.main.normalFontSize;
                    break;
            }
        }


        /// <summary>
        /// 디폴트 값들에 대한 처리 
        /// </summary>
        void SetBubbleDefaults()
        {
            if (GameManager.main == null)
                return;

            #region pos값이 정해지지 않은 경우에 대한 처리

            if (bubblePos < 1)
            {
                if (GameManager.main.GetSpeakerModelExists(row.speaker))
                {
                    bubblePos = BubbleManager.GetDefaultBubbleTalkPosIndex();
                    BubbleManager.IncreaseBubbleTalkPosIndex();
                }
                else
                {
                    bubblePos = BubbleManager.GetDefaultBubbleCenterPosIndex();
                    BubbleManager.IncreaseBubbleCenterPosIndex();
                }
            }
            #endregion

            #region size 값이 정해지지 않은 경우 처리 

            if (bubbleSize < 1)
                bubbleSize = BubbleManager.main.GetCurrentAdjustmentSize();

            #endregion

        }


        /// <summary>
        /// 서버 기준정보 따라서 말풍선 설정 
        /// </summary>
        void SetBubbleSync()
        {

            #region TEXTAREA 처리 
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_TOP), out textAreaTop);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_BOTTOM), out textAreaBottom);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_LEFT), out textAreaLeft);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_RIGHT), out textAreaRight);
            // right, top은 - 처리한다.
            textAreaTop *= -1;
            textAreaRight *= -1;


            textContents.rectTransform.offsetMin = new Vector2(textAreaLeft, textAreaBottom);
            textContents.rectTransform.offsetMax = new Vector2(textAreaRight, textAreaTop);
            #endregion

            #region SPRITE 처리 

            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_CUSTOM_SIZE_X), out customSizeX);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_CUSTOM_SIZE_Y), out customSizeY);

            imageBubble.type = Image.Type.Sliced;
            imageOutline.type = Image.Type.Sliced;

            imageBubble.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_BUBBLE_SPRITE_ID)); 
            imageOutline.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_OUTLINE_SPRITE_ID));

            if (bubbleReverse > 0) // 반전 
            {
                imageTail.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_REVERSE_TAIL_SPRITE_ID));
                imageTailOutline.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_REVERSE_TAIL_OUTLINE_SPRITE_ID));

            }
            else // 안 반전 
            {
                imageTail.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAIL_SPRITE_ID));
                imageTailOutline.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAIL_OUTLINE_SPRITE_ID));
            }


            // 이미지 없으면 안보이게 처리 
            if (imageTail.sprite != null)
                imageTail.SetNativeSize();

            imageOutline.gameObject.SetActive(imageOutline.sprite != null);
            imageTail.gameObject.SetActive(imageTail.sprite != null);
            imageTailOutline.gameObject.SetActive(imageTailOutline.sprite != null);


            if (imageBubble.sprite != null && customSizeX <= 0 && customSizeY <= 0)
            {
                imageBubble.type = Image.Type.Simple;
                imageBubble.SetNativeSize();
            }

            if (imageOutline.sprite != null && customSizeX <= 0 && customSizeY <= 0)
            {
                imageOutline.type = Image.Type.Simple;
                imageOutline.rectTransform.anchorMax = centerPivot;
                imageOutline.rectTransform.anchorMin = centerPivot;
                imageOutline.rectTransform.localPosition = Vector3.zero;

                imageOutline.SetNativeSize();
            }

            if (customSizeX > 0 || customSizeY > 0)
            {
                rtransform.sizeDelta = new Vector2(customSizeX, customSizeY);

                // 2021.04.21 custom 타입에서 size가 1인 경우 글자수 9이하일때 가로 사이즈 감소처리
                // 반전에서는 처리하지 않음. 
                // 대화에서만 처리. 
                if (bubbleSize == 1 && textContents.text.Length <= 9 && bubbleReverse < 1 && template == GameConst.TEMPLATE_TALK)
                    rtransform.sizeDelta = new Vector2(customSizeX - 66, customSizeY);
                else // 나머지는 기입된 수치대로 한다
                    rtransform.sizeDelta = new Vector2(customSizeX, customSizeY);
            }


            #endregion


            #region Color 처리 

            fontColor = SystemManager.GetJsonNodeString(targetBubbleJson, COL_FONT_COLOR);
            fillColor = SystemManager.GetJsonNodeString(targetBubbleJson, COL_FILL_COLOR);
            outlineColor = SystemManager.GetJsonNodeString(targetBubbleJson, COL_OUTLINE_COLOR);

            textContents.color = HexCodeChanger.HexToColor(fontColor);
            ColorFont = textContents.color;

            imageBubble.color = HexCodeChanger.HexToColor(fillColor);
            imageTail.color = HexCodeChanger.HexToColor(fillColor);
            ColorFill = imageBubble.color;

            imageOutline.color = HexCodeChanger.HexToColor(outlineColor);
            imageTailOutline.color = HexCodeChanger.HexToColor(outlineColor);
            ColorOutline = imageOutline.color;

            #endregion

            #region position, scale 처리 
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_POS_X), out posX);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_POS_Y), out posY);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_SCALE_X), out scaleX);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_SCALE_Y), out scaleY);

            if (!isFakeBubble)
            {
                rtransform.anchoredPosition = new Vector2(posX, posY);
                rtransform.localScale = new Vector3(scaleX, scaleY, 1);
            }

            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAIL_POS_X), out tailPosX);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAIL_POS_Y), out tailPosY);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAIL_SCALE_X), out tailScaleX);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAIL_SCALE_Y), out tailScaleY);

            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_REVERSE_TAIL_POS_X), out reverseTailPosX);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_REVERSE_TAIL_POS_Y), out reverseTailPosY);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_REVERSE_TAIL_SCALE_X), out reverseTailScaleX);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_REVERSE_TAIL_SCALE_Y), out reverseTailScaleY);

            if (bubbleReverse > 0) // 반전 처리 
            {
                imageTail.rectTransform.anchoredPosition = new Vector3(reverseTailPosX, reverseTailPosY, 0);
                imageTail.rectTransform.localScale = new Vector3(reverseTailScaleX, reverseTailScaleY, 1);
            }
            else
            {
                imageTail.rectTransform.anchoredPosition = new Vector3(tailPosX, tailPosY, 0);
                imageTail.rectTransform.localScale = new Vector3(tailScaleX, tailScaleY, 1);
            }


            #endregion

            #region 네임태그 활성화, 스프라이트 처리 

            // 이전에 로컬 저장된 데이터에는 컬럼이 없을 수도 있다.
            if (targetBubbleJson.ContainsKey(COL_TAG_SPRITE_ID))
            {
                // 네임태그 스프라이트 및 위치 정보 불러오기 
                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAG_SPRITE_ID), out tagSpriteID);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAG_POS_X), out tagPosX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAG_POS_Y), out tagPosY);


                if (tagSpriteID < 0) // 네임태그 없으면 사용하지 않는 것으로 처리 
                    nametag.gameObject.SetActive(false);
                else
                {
                    nametag.gameObject.SetActive(true); // 
                    nametag.sprite = BubbleManager.main.GetBubbleSprite(tagSpriteID.ToString());
                    if (nametag.sprite != null)
                        nametag.SetNativeSize();

                    // 위치 설정해주기. 
                    nametag.transform.localPosition = new Vector3(tagPosX, tagPosY, 0);
                    nametag.color = Color.cyan; // 임시 컬러
                }
            }
            else
                nametag.gameObject.SetActive(false);


            #endregion

            #region 이모티콘 처리 

            if (!string.IsNullOrEmpty(emoticon_expression))
            {
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_POS_X), out emoticonPosX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_POS_Y), out emoticonPosY);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_SCALE_X), out emoticonScaleX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_SCALE_Y), out emoticonScaleY);
                // 이모티콘 너비, 높이 추가 
                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_WIDTH), out emoticonWidth);
                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_HEIGHT), out emoticonHeight);

                emoticon.rectTransform.anchoredPosition = new Vector3(emoticonPosX, emoticonPosY, 0);
                emoticon.rectTransform.localScale = new Vector3(emoticonScaleX, emoticonScaleY, 1);

                // 이모티콘 너비, 높이 적용
                emoticon.rectTransform.sizeDelta = new Vector2(emoticonWidth, emoticonHeight);

                // 말풍선 세트에서 이모티콘 크기가 둘다 0으로 되어있으면 사용하지 않는 것으로 간주한다.
                if (emoticonWidth <= 0 && emoticonHeight <= 0)
                    isEmoticonAvailable = false;
                else
                    isEmoticonAvailable = true;
            }

            #endregion

            #region 말풍선 ScaleX, ScaleY에 대한 추가 처리 

            // 말풍선과 꼬리가 분리된 세트에서는 scaleX와 scaleY를 마이너스로 설정할 일이 없다. 
            // 붙어있는 친구들이 문제인데...

            // scaleX를 반전시키는 경우
            // 1. Text도 반전시킨다.
            // 2. 네임태그도 반전시킨다. 
            // 3. 이모티콘은..? 

            if (scaleX < 0)
            {
                // 텍스트 반전시킨다.
                textContents.rectTransform.localScale = reverseScaleX;

                // 네임태그도 반전시킨다. (네임태그 사용중에만)
                if (nametag.gameObject.activeSelf)
                {
                    nametag.rectTransform.localScale = reverseScaleX;
                    // position X를 -1 곱해준다. 
                    nametag.rectTransform.localPosition = new Vector3(nametag.rectTransform.localPosition.x * -1, nametag.rectTransform.localPosition.y, 0);
                }

                tweenLocalScale = reverseScaleX;
                emoticon.rectTransform.localScale = reverseScaleX;
            }


            #endregion

        }

        /// <summary>
        /// 말풍선 배리에이션 받아오기 
        /// </summary>
        string GetBubbleVariation()
        {

            // speaker 입력안됐을까봐..
            if (string.IsNullOrEmpty(row.speaker))
            {
                Debug.LogWarning("speaker is empty");
                return StoryManager.BUBBLE_VARIATION_NORMAL;
            }


            // 21.05.19 기준 variation
            // normal (1인 스탠딩, 화자와 스탠딩 캐릭터가 같음)
            // emoticon (화자와 스탠딩 캐릭터가 다르고, emoticon_expression 사용)
            // reverse_emoticon (화자와 스탠딩 캐릭터가 다르고, emoticon_expression 사용, 말풍선 반전 사용)
            // double (2인 스탠딩, 화자와 스탠딩 캐릭터가 같음)

            // 서있는 친구가 말하는건지 먼저 체크한다. 
            isStandingSpeak = GameManager.main.CompareWithCurrentSpeaker(row.speaker);

            if (isStandingSpeak) // 서있는 캐릭터가 말하는 중일때 => double or normal 
            {

                // 스탠딩이 몇명인지에 따른 분리 
                if (GameManager.main.CheckModelStanding() > 1)
                    return StoryManager.BUBBLE_VARIATION_DOUBLE;
                else
                    return StoryManager.BUBBLE_VARIATION_NORMAL;

            }
            else // 서있는 캐릭터가 말하는게 아니다 => emoticon or reverse_emoticon
            {
                // 이모티콘 표현 값이 없을 경우 체크 
                if (string.IsNullOrEmpty(emoticon_expression))
                {
                    // Debug.LogWarning("emoticon_expression is empty");
                    return StoryManager.BUBBLE_VARIATION_NORMAL;
                }

                if (bubbleReverse <= 0)
                    return StoryManager.BUBBLE_VARIATION_EMOTICON;
                else
                    return StoryManager.BUBBLE_VARIATION_REVERSE_EMOTICON;

            }
        }

        /// <summary>
        /// 말풍선 Variation 체크 및 기준정보 가져오기 
        /// </summary>
        void SetBubbleGroup()
        {
            // 배리에이션 설정하기 
            variation = GetBubbleVariation();


            currentBubbleGroup = StoryManager.main.GetBubbleGroupJSON(template, variation);
            if (currentBubbleGroup == null)
                Debug.LogError("Can't take bubble group json");

            // 말풍선 기준정보에서 size,pos가 일치하는 친구 가져오기 
            int groupSize = 0;
            int groupPos = 0;
            targetBubbleDataString = string.Empty;

            for (int i = 0; i < currentBubbleGroup.Count; i++)
            {
                int.TryParse(SystemManager.GetJsonNodeString(currentBubbleGroup[i], "size"), out groupSize);    // size
                int.TryParse(SystemManager.GetJsonNodeString(currentBubbleGroup[i], "pos"), out groupPos);      // pos

                if (groupSize == bubbleSize && groupPos == bubblePos)
                {
                    targetBubbleJson = currentBubbleGroup[i];
                    targetBubbleDataString = JsonMapper.ToJson(targetBubbleJson);
                    return;
                }
            }
        }

        /// <summary>
        /// 이모티콘 설정
        /// </summary>
        void SetBubbleEmoticon()
        {
            emoticon.gameObject.SetActive(false);

            // 이모티콘 없으면 리턴 
            if (string.IsNullOrEmpty(emoticon_expression))
                return;

            // 체크 추가 
            if (!isEmoticonAvailable)
                return;

            // 정상 환경 
            emoticonMount = GameManager.main.GetEmoticonSprite(emoticon_expression);
            if (emoticonMount != null)
                emoticon.sprite = emoticonMount.sprite;

            if (emoticon.sprite == null)
                return;

            emoticon.gameObject.SetActive(true);
        }

        /// <summary>
        /// 네임태그 설정 
        /// </summary>
        void SetNametag()
        {
            // 네임태그 안쓰면 할필요 없다.
            if (!nametag.gameObject.activeSelf)
                return;


            // alternativeName이 있는 경우는 alternative가 우선시된다. 
            if (!string.IsNullOrEmpty(alternativeName))
            {
                tagMainColor = StoryManager.main.GetNametagColor(alternativeName);
                tagSubColor = StoryManager.main.GetNametagColor(alternativeName, false);
                tagText = StoryManager.main.GetNametagName(alternativeName);
            }
            else
            {
                tagText = StoryManager.main.GetNametagName(speaker);
                tagMainColor = StoryManager.main.GetNametagColor(speaker);
                tagSubColor = StoryManager.main.GetNametagColor(speaker, false);
            }


            // 여기까지 왔는데, tagText가 값이 없는 경우.
            // default로 처리해준다. 
            if (string.IsNullOrEmpty(tagText))
            {
                tagMainColor = StoryManager.main.GetNametagColor(GameConst.COL_DEFAULT);
                tagSubColor = StoryManager.main.GetNametagColor(GameConst.COL_DEFAULT, false);
                tagText = "네임태그없음";
            }

            nametag.color = HexCodeChanger.HexToColor(tagMainColor);
            textName.color = HexCodeChanger.HexToColor(tagSubColor);

            textName.text = tagText;
        }

        #endregion

        /// <summary>
        /// 말풍선 보여주기 
        /// </summary>
        public void ShowBubble(ScriptRow __row, Action __cb)
        {

            InitTransform();
            SetParams(__row, __cb);

            // 값이 지정되지 않은 column 들에 대한 처리 
            SetBubbleDefaults();

            // 받은 데이터를 기반으로 말풍선 기준정보를 받아온다. 
            SetBubbleGroup();

            // 서버데이터와 맞추기 
            SetBubbleSync();

            // 폰트 스타일 처리 
            SetTextFontStyle();

            // 말꼬리 추가 설정
            SetTail();

            // 이모티콘 처리 
            SetBubbleEmoticon();
            if (emoticonMount != null)
                emoticonMount.DecreaseUseCount();

            // 캐릭터 키 따라서 위치 조정 추가 
            SetCharacterTallAdjustment();

            // 아래쪽 라인 말풍선 해상도 대응 
            SetBottomLinePositionAdjustment();

            // 네임태그 설정
            SetNametag();

            OnBubble();
            
            //textContents.SetText(message);
            //textContents.text = message;

            if (!string.IsNullOrEmpty(__row.voice))
                GameManager.main.SoundGroup[1].PlayVoice(__row.voice);          // 21.09.27 대화 관련 템플릿 때에는 이곳에서 음성을 재생하도록 한다

            __cb?.Invoke();
        }


        /// <summary>
        /// 전화 통화 말풍선
        /// </summary>
        /// <param name="pos">말풍선 위치</param>
        /// <param name="isSelf">true = 전화본인, false = 전화상대</param>
        public void ForPhoneBubble(ScriptRow __row, Action __cb, int pos, bool isSelf)
        {
            InitTransform();
            SetParams(__row, __cb, pos);

            // bubble size, pos 설정 안해둔 것들 받아옴
            SetBubbleDefaults();

            SetBubbleGroup();

            if (isSelf)
            {
                SetBubbleSync();

                SetTextFontStyle();

                SetTail();
            }
            else
            {
                #region Sync 처리

                #region TEXTAREA 처리 
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_TOP), out textAreaTop);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_BOTTOM), out textAreaBottom);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_LEFT), out textAreaLeft);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_RIGHT), out textAreaRight);

                // 21.08.03 임의로 textArea 값 추가 조정(전화상대 템플릿만)
                textAreaTop += 25f;
                textAreaBottom -= 15f;
                textAreaLeft += 40f;
                textAreaRight += 30f;

                // right, top은 - 처리한다.
                textAreaTop *= -1;
                textAreaRight *= -1;

                textContents.rectTransform.offsetMin = new Vector2(textAreaLeft, textAreaBottom);
                textContents.rectTransform.offsetMax = new Vector2(textAreaRight, textAreaTop);
                #endregion

                #region SPRITE 처리 

                imageBubble.sprite = BubbleManager.main.GetForPhoneSprite(bubbleSize);
                imageBubble.SetNativeSize();
                
                #endregion

                #region position, scale 처리 
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_POS_X), out posX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_POS_Y), out posY);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_SCALE_X), out scaleX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_SCALE_Y), out scaleY);


                if (!isFakeBubble)
                {
                    // 21.08.03 말풍선 X축 위치 변경(전화상대 말풍선 한정)
                    posX += 70f;

                    rtransform.anchoredPosition = new Vector2(posX, posY);
                    rtransform.localScale = new Vector3(scaleX, scaleY, 1);
                }

                #endregion

                #region 이모티콘 처리 

                if (string.IsNullOrEmpty(emoticon_expression))
                    return;

                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_POS_X), out emoticonPosX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_POS_Y), out emoticonPosY);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_SCALE_X), out emoticonScaleX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_SCALE_Y), out emoticonScaleY);

                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_WIDTH), out emoticonWidth);
                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_HEIGHT), out emoticonHeight);

                // 21.08.03 말풍선 이모티콘 위치 변경(전화상대 말풍선 한정)
                emoticonPosX -= 150f;
                emoticonPosY = -220f;

                // 말풍선 뒤집어서 똑같이 또 뒤집음
                emoticon.rectTransform.anchoredPosition = new Vector3(emoticonPosX, emoticonPosY, 0);
                emoticon.rectTransform.localScale = new Vector3(emoticonScaleX, emoticonScaleY, 1);
                emoticon.rectTransform.sizeDelta = new Vector2(emoticonWidth, emoticonHeight);

                if (emoticonWidth <= 0 && emoticonHeight <= 0)
                    isEmoticonAvailable = false;
                else
                    isEmoticonAvailable = true;

                #endregion

                // 말풍선 뒤집어 줘서 글자도 뒤집음
                textContents.rectTransform.localScale = new Vector3(textContents.rectTransform.localScale.x, textContents.rectTransform.localScale.y, textContents.rectTransform.localScale.z);

                #endregion

                SetTextFontStyle();
            }

            SetBubbleEmoticon();

            SetNametag();

            PhoneBubbleOn(isSelf);

            __cb?.Invoke();

        }


        #region 말풍선 On/Off

        /// <summary>
        /// 말풍선 등장 처리 
        /// </summary>
        public void OnBubble()
        {
            if(needDelayShow && speakerTall < 0) {
                Debug.Log("<color=white>Bubble Delay Show</color>");
                // needDelayShow = false;
                
                Invoke("DelayOnBubble", 0.1f);
            }
            
            ActiveInEffect();
        }
        
        void DelayOnBubble() {
            
            Debug.Log("DelayOnBubble : " + this.gameObject.name);
            
            // delay 호출되었을때 needDelayShow가 false이면 실행하지 않는다. 
            if(!needDelayShow)
                return;
            
            SetCharacterTallAdjustment(); // 키 측정 다시한다. 
            OnBubble();
        }
        
        
        
        /// <summary>
        /// 실 등장!
        /// </summary>
        void ActiveInEffect()
        {
            
            needDelayShow = false;
            
            rtransform.DOKill();

            // default 처리
            if (string.IsNullOrEmpty(in_effect) || in_effect == CommonConst.NONE)
            {
                rtransform.localScale = Vector3.zero;
                this.gameObject.SetActive(true);
                rtransform.pivot = centerPivot;

                if (tweenLocalScale != Vector3.one)
                    rtransform.DOScale(tweenLocalScale, 0.2f).SetEase(Ease.OutBack);
                else
                    rtransform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
            }
            if (in_effect == GameConst.INOUT_EFFECT_FADEIN) // 페이드인 
            {
                InitColor();
                SetTransparentTextColor();
                SetFadeInImages(0.4f);
            }
            else if (in_effect == GameConst.INOUT_EFFECT_SHAKE) // 흔들흔들
            {
                this.gameObject.SetActive(true);
                rtransform.DOPunchRotation(new Vector3(0, 0, 20), 0.2f, 40);
            }
            else   // 스케일 업
            {
                rtransform.localScale = Vector3.zero;
                this.gameObject.SetActive(true);

                if (tweenLocalScale != Vector3.one)
                    rtransform.DOScale(tweenLocalScale, 0.2f).SetEase(Ease.OutBack);
                else
                    rtransform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
            }
        }


        /// <summary>
        /// 말풍선 종료 처리
        /// </summary>
        public void OffBubble(bool immediate = false)
        {
            // delayShow가 걸려있는 상태에서 OffBubble이 호출받은 경우.
            if(needDelayShow) {
                needDelayShow = false;
                return;
            }
            
            // 즉시 처리 
            if (immediate)
            {
                OnCompletedOffTween();
                return;
            }

            if (holdingCount > 0)
            {
                holdingCount--;
                return;
            }

            ActiveOutEffect();
        }

        void OnCompletedOffTween()
        {
            gameObject.SetActive(false);

            if (emoticonMount != null)
                emoticonMount.EndImage();
        }


        /// <summary>
        /// 페이드아웃 처리
        /// </summary>
        /// <param name="__time"></param>
        void SetFadeOutImages(float __time)
        {
            imageBubble.DOFade(0, __time).OnComplete(OnCompletedOffTween);
            imageOutline.DOFade(0, __time);
            nametag.DOFade(0f, __time);
            textName.DOFade(0f, __time);
            imageTail.DOFade(0, __time);
            emoticon.DOFade(0, __time);
            textContents.DOFade(0, __time);
        }

        void SetFadeInImages(float __time)
        {
            this.gameObject.SetActive(true);

            imageBubble.DOFade(ColorFill.a, __time);
            imageOutline.DOFade(ColorOutline.a, __time);
            nametag.DOFade(1f, __time);
            textName.DOFade(1f, __time);
            imageTail.DOFade(1, __time);
            emoticon.DOFade(1, __time);
            textContents.DOFade(1, __time);
        }

        void ActiveOutEffect()
        {
            if (string.IsNullOrEmpty(out_effect) || out_effect == CommonConst.NONE)
                OnCompletedOffTween();
            else if (out_effect == GameConst.INOUT_EFFECT_FADEOUT)
            {
                InitColor();
                SetFadeOutImages(0.4f);
            }
            else if (out_effect == GameConst.INOUT_EFFECT_SCALEDOWN)
            {
                transform.DOScale(0, 0.3f).OnComplete(OnCompletedOffTween);
            }
            else
                OnCompletedOffTween();
        }

        #endregion


        #region 전화 관련 말풍선 On/Off

        void PhoneBubbleOn(bool isSelf)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            imageBubble.enabled = true;
            textContents.gameObject.SetActive(true);

            if (isSelf)
            {
                imageOutline.enabled = true;
                imageTail.enabled = true;
                imageTailOutline.enabled = true;
            }

            ActiveInEffect();
        }

        public void PhoneBubbleOff(bool isSelf)
        {
            imageBubble.enabled = false;
            textContents.gameObject.SetActive(false);

            if (isSelf)
            {
                imageOutline.enabled = false;
                imageTail.enabled = false;
                imageTailOutline.enabled = false;
            }
        }

        #endregion

        

        /// <summary>
        /// 디바이스 스크린 비율에 따른 하단 라인 말풍선 위치 조정
        /// </summary>
        void SetBottomLinePositionAdjustment()
        {
            if (bubblePos < 7)
                return;
                
            adjustmentPosY = 0;


            // ! 7,8,9 위치만 조정한다. 
            // ! 세로가 길어지는 디바이스는 7,8,9 위치를 조금 더 아래쪽으로 처리 
            if (SystemManager.screenRatio < 0.56f)
                adjustmentPosY = -100; // 길어지면 -100
                    
            
            
            // banner 관련된 내용 추가. 
            // 배너 사용하면 살짝 올려야한다.
            if(AdManager.main.isIronSourceBannerLoad)
                adjustmentPosY += 100; // 다시 100 플러스.
            

            // 100만큼 아래로 이동 
            rtransform.anchoredPosition = new Vector2(posX, posY + adjustmentPosY);

        }

        /// <summary>
        /// 화자의 '키'에 따른 말풍선 위치 조정 
        /// </summary>
        void SetCharacterTallAdjustment()
        {
            // 캐릭터 키로 말풍선 위치 조정하기 .
            if (string.IsNullOrEmpty(row.speaker))
                return;

            // 두번째 열까지만 조정 
            if (bubblePos > 6)
                return;


            // 스탠딩이 있을때만 키 체크를 한다.
            if (GameManager.main.CheckModelStanding() < 1)
                return;

            // 키 가져오기 
            speakerTall = GameManager.main.GetCurrentModelTall(row.speaker);
            
            // * 화자 키가 0보다 작으면 아직 키 체크가 완료되지 않은상태다. 
            if(speakerTall < 0) {
                needDelayShow = true; // 약간 딜레이가 필요하다. (키체크가 필요해...)
                return; 
            }

            // 키 높이에 의한 말풍선 위치 조절
            switch (speakerTall)
            {
                case 0:
                    rtransform.anchoredPosition = new Vector2(posX, posY - 300);
                    break;
                case 1:
                    rtransform.anchoredPosition = new Vector2(posX, posY - 240);
                    break;
                case 2:
                    rtransform.anchoredPosition = new Vector2(posX, posY - 180);
                    break;
                case 3:
                    rtransform.anchoredPosition = new Vector2(posX, posY - 90);
                    break;
                case 4:
                    rtransform.anchoredPosition = new Vector2(posX, posY - 30);
                    break;
                // 캐릭터 키 등급이 5이상이면 조정하지 않음 
                case 5:
                default:
                    break;
            }
        }
    }
}