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

        // ���� json �÷���.. 
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

        // ����� transform
        public RectTransform rtransform;


        public Image imageBubble;           // ��ü 
        public Image imageOutline;          // �ܰ�
        public Image nametag;               // �̸�ǥ �̹���
        public TextMeshProUGUI textName;    // �̸�ǥ �ؽ�Ʈ 


        public TextMeshProUGUI textContents;    // ��
        public Image imageTail;                 // ����
        public Image imageTailOutline;          // ���� �ܰ��� 
        public Image emoticon;                  // �̸�Ƽ��
        ScriptImageMount emoticonMount;         // �̸�Ƽ�� �������� ImageMount
        public GameObject fakeReverseTail;
        public bool isFakeBubble = false;

        string template = string.Empty;
        string message = string.Empty;
        int bubbleSize = 0;
        int bubblePos = 0;
        int bubbleReverse = 0;                  // ������ ���� ���
        int holdingCount = 0;
        ScriptRow row;
        string emoticon_expression = string.Empty;
        string in_effect = string.Empty;
        string out_effect = string.Empty;
        string speaker = string.Empty;
        string alternativeName = string.Empty; // ��ü �̸� 

        Vector3 tweenLocalScale = Vector3.zero; // scaleX,Y�� ��ȭ�� �Ͼ���� ��� ����� Vector3

        int speakerTall = 0;                    // ȭ��-ĳ���� ���� Ű
        bool isStandingSpeak = false;           // ���ִ� ĳ���Ͱ� ���ϴ°Ŵ�?
        bool isEmoticonAvailable = false;       // �̸�Ƽ�� expression�� �־, �� ������ ���ؼ� ��뿩�ΰ� �����ǵ��� �Ѵ�.


        [Space][Header("�ΰ� ���")]
        string options = string.Empty; // �ΰ� ���!

        string variation = string.Empty;        // variation 
        JsonData currentBubbleGroup = null;     // �踮���̼ǿ� ���� ��ǳ�� �������� 
        JsonData targetBubbleJson = null;       // size, pos�� �´� Ÿ�� JSON Object �� 
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


        [Header("Ŀ���� ũ��")]
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
        int emoticonWidth = 0;      // �̸�Ƽ�� �ʺ� 
        int emoticonHeight = 0;     // �̸�Ƽ�� ����


        [Header("Nametag")]
        int tagSpriteID = -1;
        float tagPosX = 0; // �����±� ��ġ x
        float tagPosY = 0; // �����±� ��ġ y
        string tagMainColor = "000000FF";
        string tagSubColor = "FFFFFFFF";
        string tagText = string.Empty;


        #region �ʱ�ȭ ó��

        /// <summary>
        /// �ʱ�ȭ (Pooling���� ���ϱ� �ʱ�ȭ ����)
        /// </summary>
        void InitTransform()
        {
            // transform �ʱ�ȭ 
            isEmoticonAvailable = false;
            rtransform.localScale = Vector3.one;
            rtransform.localEulerAngles = Vector3.zero;
            textContents.transform.localScale = Vector3.one; // textContent localScale�� �߿�������. 
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
        /// ���� �ʱ�ȭ 
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
        /// �ؽ�Ʈ�� ������ ������ ����� �ʰ� ���ؼ� ���� �Ѵ�. 
        /// </summary>
        void SetTransparentTextColor()
        {
            textContents.color = new Color(textContents.color.r, textContents.color.g, textContents.color.b, 0);
            imageOutline.color = new Color(ColorOutline.r, ColorOutline.g, ColorOutline.b, 0);
            nametag.color = new Color(nametag.color.r, nametag.color.g, nametag.color.b, 0);
            textName.color = new Color(textName.color.r, textName.color.g, textName.color.b, 0);
        }

        /// <summary>
        /// �Ķ���� ����
        /// </summary>
        /// <param name="pos">��ȭ ���ø� ��ǳ�� ��ġ ������</param>
        void SetParams(ScriptRow __row, Action __cb, int pos = -1)
        {
            row = __row;
            message = __row.script_data;

            if (string.IsNullOrEmpty(message))
            {
                message = " ";
                Debug.Log(">> Message is empty << ");
                __cb?.Invoke();

                return;
            }

            message = message.Replace(@"\", "\n");
            
            template = __row.template;          // ���ø� 
            holdingCount = __row.bubble_hold;   // ���� Ƚ��
            bubbleSize = row.bubble_size;       // ��ǳ�� ������

            if (pos < 0)
                bubblePos = row.bubble_pos;         // ��ǳ�� ��ġ
            else
                bubblePos = pos;

            bubbleReverse = row.bubble_reverse; // ��ǳ�� ����

            emoticon_expression = row.emoticon_expression; // �̸�Ƽ�� ǥ��
            in_effect = row.in_effect; // ���� ����
            out_effect = row.out_effect; // ���� ����

            speaker = row.speaker; // ȭ�� �߰� 
            alternativeName = row.controlAlternativeName; // ��ü�̸� 

            // �ؽ�Ʈ ���� 
            textContents.SetText(message);
        }

        #endregion

        #region ��ǳ�� ũ�� üũ �뵵 ��¥ ��ǳ�� ���� 


        /// <summary>
        /// ��ǳ�� ũ�� üũ�� ��¥ ��ǳ�� �����ϱ� 
        /// </summary>
        public void SetFakeBubble(ScriptRow __row, int __size)
        {
            isFakeBubble = true;

            SetParams(__row, null);

            bubbleSize = __size; // fake �� ������ size�� ���� ���� 
            if (bubblePos == 0)
                bubblePos = BubbleManager.GetDefaultBubbleTalkPosIndex();

            // ���� �����͸� ������� ��ǳ�� ���������� �޾ƿ´�. 
            SetBubbleGroup();

            // ���������Ϳ� ���߱� 
            SetBubbleSync();

            SetTextFontStyle();

            // ������ �߰� ����
            SetTail();

            SetBubbleEmoticon();

            // force update
            textContents.ForceMeshUpdate(true, true);
        }

        #endregion

        #region ��ǳ�� �ű� �۾� 


        /// <summary>
        /// ������ �߰� ����
        /// </summary>
        void SetTail()
        {
            if (GameManager.main == null)
                return;

            if (GameManager.main.CompareWithCurrentSpeaker(row.speaker))
                return;

            // ȭ�鿡 ĳ���� ���� ����, �̸�Ƽ�� ǥ���� ������ ������ ���� 
            if (!GameManager.main.CompareWithCurrentSpeaker(row.speaker) && string.IsNullOrEmpty(emoticon_expression))
            {
                imageTail.rectTransform.localScale = Vector3.zero;
                return;
            }
        }

        /// <summary>
        /// �⺻ ��Ʈ ��Ÿ�� ���� 
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
                    textContents.fontStyle = FontStyles.Bold;
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
        /// ����Ʈ ���鿡 ���� ó�� 
        /// </summary>
        void SetBubbleDefaults()
        {
            if (GameManager.main == null)
                return;

            #region pos���� �������� ���� ��쿡 ���� ó��

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

            #region size ���� �������� ���� ��� ó�� 

            if (bubbleSize < 1)
                bubbleSize = BubbleManager.main.GetCurrentAdjustmentSize();

            #endregion

        }


        /// <summary>
        /// ���� �������� ���� ��ǳ�� ���� 
        /// </summary>
        void SetBubbleSync()
        {

            #region TEXTAREA ó�� 
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_TOP), out textAreaTop);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_BOTTOM), out textAreaBottom);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_LEFT), out textAreaLeft);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_RIGHT), out textAreaRight);
            // right, top�� - ó���Ѵ�.
            textAreaTop *= -1;
            textAreaRight *= -1;


            textContents.rectTransform.offsetMin = new Vector2(textAreaLeft, textAreaBottom);
            textContents.rectTransform.offsetMax = new Vector2(textAreaRight, textAreaTop);
            #endregion

            #region SPRITE ó�� 

            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_CUSTOM_SIZE_X), out customSizeX);
            float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_CUSTOM_SIZE_Y), out customSizeY);

            imageBubble.type = Image.Type.Sliced;
            imageOutline.type = Image.Type.Sliced;

            imageBubble.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_BUBBLE_SPRITE_ID)); 
            imageOutline.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_OUTLINE_SPRITE_ID));

            if (bubbleReverse > 0) // ���� 
            {
                imageTail.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_REVERSE_TAIL_SPRITE_ID));
                imageTailOutline.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_REVERSE_TAIL_OUTLINE_SPRITE_ID));

            }
            else // �� ���� 
            {
                imageTail.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAIL_SPRITE_ID));
                imageTailOutline.sprite = BubbleManager.main.GetBubbleSprite(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAIL_OUTLINE_SPRITE_ID));
            }


            // �̹��� ������ �Ⱥ��̰� ó�� 
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

                // 2021.04.21 custom Ÿ�Կ��� size�� 1�� ��� ���ڼ� 9�����϶� ���� ������ ����ó��
                // ���������� ó������ ����. 
                // ��ȭ������ ó��. 
                if (bubbleSize == 1 && textContents.text.Length <= 9 && bubbleReverse < 1 && template == GameConst.TEMPLATE_TALK)
                    rtransform.sizeDelta = new Vector2(customSizeX - 66, customSizeY);
                else // �������� ���Ե� ��ġ��� �Ѵ�
                    rtransform.sizeDelta = new Vector2(customSizeX, customSizeY);
            }


            #endregion


            #region Color ó�� 

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

            #region position, scale ó�� 
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

            if (bubbleReverse > 0) // ���� ó�� 
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

            #region �����±� Ȱ��ȭ, ��������Ʈ ó�� 

            // ������ ���� ����� �����Ϳ��� �÷��� ���� ���� �ִ�.
            if (targetBubbleJson.ContainsKey(COL_TAG_SPRITE_ID))
            {
                // �����±� ��������Ʈ �� ��ġ ���� �ҷ����� 
                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAG_SPRITE_ID), out tagSpriteID);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAG_POS_X), out tagPosX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TAG_POS_Y), out tagPosY);


                if (tagSpriteID < 0) // �����±� ������ ������� �ʴ� ������ ó�� 
                    nametag.gameObject.SetActive(false);
                else
                {
                    nametag.gameObject.SetActive(true); // 
                    nametag.sprite = BubbleManager.main.GetBubbleSprite(tagSpriteID.ToString());
                    if (nametag.sprite != null)
                        nametag.SetNativeSize();

                    // ��ġ �������ֱ�. 
                    nametag.transform.localPosition = new Vector3(tagPosX, tagPosY, 0);
                    nametag.color = Color.cyan; // �ӽ� �÷�
                }
            }
            else
                nametag.gameObject.SetActive(false);


            #endregion

            #region �̸�Ƽ�� ó�� 

            if (!string.IsNullOrEmpty(emoticon_expression))
            {
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_POS_X), out emoticonPosX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_POS_Y), out emoticonPosY);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_SCALE_X), out emoticonScaleX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_SCALE_Y), out emoticonScaleY);
                // �̸�Ƽ�� �ʺ�, ���� �߰� 
                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_WIDTH), out emoticonWidth);
                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_HEIGHT), out emoticonHeight);

                emoticon.rectTransform.anchoredPosition = new Vector3(emoticonPosX, emoticonPosY, 0);
                emoticon.rectTransform.localScale = new Vector3(emoticonScaleX, emoticonScaleY, 1);

                // �̸�Ƽ�� �ʺ�, ���� ����
                emoticon.rectTransform.sizeDelta = new Vector2(emoticonWidth, emoticonHeight);

                // ��ǳ�� ��Ʈ���� �̸�Ƽ�� ũ�Ⱑ �Ѵ� 0���� �Ǿ������� ������� �ʴ� ������ �����Ѵ�.
                if (emoticonWidth <= 0 && emoticonHeight <= 0)
                    isEmoticonAvailable = false;
                else
                    isEmoticonAvailable = true;
            }

            #endregion

            #region ��ǳ�� ScaleX, ScaleY�� ���� �߰� ó�� 

            // ��ǳ���� ������ �и��� ��Ʈ������ scaleX�� scaleY�� ���̳ʽ��� ������ ���� ����. 
            // �پ��ִ� ģ������ �����ε�...

            // scaleX�� ������Ű�� ���
            // 1. Text�� ������Ų��.
            // 2. �����±׵� ������Ų��. 
            // 3. �̸�Ƽ����..? 

            if (scaleX < 0)
            {
                // �ؽ�Ʈ ������Ų��.
                textContents.rectTransform.localScale = reverseScaleX;

                // �����±׵� ������Ų��. (�����±� ����߿���)
                if (nametag.gameObject.activeSelf)
                {
                    nametag.rectTransform.localScale = reverseScaleX;
                    // position X�� -1 �����ش�. 
                    nametag.rectTransform.localPosition = new Vector3(nametag.rectTransform.localPosition.x * -1, nametag.rectTransform.localPosition.y, 0);
                }

                tweenLocalScale = reverseScaleX;
                emoticon.rectTransform.localScale = reverseScaleX;
            }


            #endregion

        }

        /// <summary>
        /// ��ǳ�� �踮���̼� �޾ƿ��� 
        /// </summary>
        string GetBubbleVariation()
        {

            // speaker �Է¾ȵ������..
            if (string.IsNullOrEmpty(row.speaker))
            {
                Debug.LogWarning("speaker is empty");
                return StoryManager.BUBBLE_VARIATION_NORMAL;
            }


            // 21.05.19 ���� variation
            // normal (1�� ���ĵ�, ȭ�ڿ� ���ĵ� ĳ���Ͱ� ����)
            // emoticon (ȭ�ڿ� ���ĵ� ĳ���Ͱ� �ٸ���, emoticon_expression ���)
            // reverse_emoticon (ȭ�ڿ� ���ĵ� ĳ���Ͱ� �ٸ���, emoticon_expression ���, ��ǳ�� ���� ���)
            // double (2�� ���ĵ�, ȭ�ڿ� ���ĵ� ĳ���Ͱ� ����)

            // ���ִ� ģ���� ���ϴ°��� ���� üũ�Ѵ�. 
            isStandingSpeak = GameManager.main.CompareWithCurrentSpeaker(row.speaker);

            if (isStandingSpeak) // ���ִ� ĳ���Ͱ� ���ϴ� ���϶� => double or normal 
            {

                // ���ĵ��� ��������� ���� �и� 
                if (GameManager.main.CheckModelStanding() > 1)
                    return StoryManager.BUBBLE_VARIATION_DOUBLE;
                else
                    return StoryManager.BUBBLE_VARIATION_NORMAL;

            }
            else // ���ִ� ĳ���Ͱ� ���ϴ°� �ƴϴ� => emoticon or reverse_emoticon
            {
                // �̸�Ƽ�� ǥ�� ���� ���� ��� üũ 
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
        /// ��ǳ�� Variation üũ �� �������� �������� 
        /// </summary>
        void SetBubbleGroup()
        {
            // �踮���̼� �����ϱ� 
            variation = GetBubbleVariation();


            currentBubbleGroup = StoryManager.main.GetBubbleGroupJSON(template, variation);
            if (currentBubbleGroup == null)
                Debug.LogError("Can't take bubble group json");

            // ��ǳ�� ������������ size,pos�� ��ġ�ϴ� ģ�� �������� 
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
        /// �̸�Ƽ�� ����
        /// </summary>
        void SetBubbleEmoticon()
        {
            emoticon.gameObject.SetActive(false);

            // �̸�Ƽ�� ������ ���� 
            if (string.IsNullOrEmpty(emoticon_expression))
                return;

            // üũ �߰� 
            if (!isEmoticonAvailable)
                return;

            // ���� ȯ�� 
            emoticonMount = GameManager.main.GetEmoticonSprite(emoticon_expression);
            if (emoticonMount != null)
                emoticon.sprite = emoticonMount.sprite;

            if (emoticon.sprite == null)
                return;

            emoticon.gameObject.SetActive(true);
        }

        /// <summary>
        /// �����±� ���� 
        /// </summary>
        void SetNametag()
        {
            // �����±� �Ⱦ��� ���ʿ� ����.
            if (!nametag.gameObject.activeSelf)
                return;


            // alternativeName�� �ִ� ���� alternative�� �켱�õȴ�. 
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


            // ������� �Դµ�, tagText�� ���� ���� ���.
            // default�� ó�����ش�. 
            if (string.IsNullOrEmpty(tagText))
            {
                tagMainColor = StoryManager.main.GetNametagColor(GameConst.COL_DEFAULT);
                tagSubColor = StoryManager.main.GetNametagColor(GameConst.COL_DEFAULT, false);
                tagText = "�����±׾���";
            }

            nametag.color = HexCodeChanger.HexToColor(tagMainColor);
            textName.color = HexCodeChanger.HexToColor(tagSubColor);

            textName.text = tagText;
        }

        #endregion

        /// <summary>
        /// ��ǳ�� �����ֱ� 
        /// </summary>
        public void ShowBubble(ScriptRow __row, Action __cb)
        {

            InitTransform();
            SetParams(__row, __cb);

            // ���� �������� ���� column �鿡 ���� ó�� 
            SetBubbleDefaults();

            // ���� �����͸� ������� ��ǳ�� ���������� �޾ƿ´�. 
            SetBubbleGroup();

            // ���������Ϳ� ���߱� 
            SetBubbleSync();

            // ��Ʈ ��Ÿ�� ó�� 
            SetTextFontStyle();

            // ������ �߰� ����
            SetTail();

            // �̸�Ƽ�� ó�� 
            SetBubbleEmoticon();
            if (emoticonMount != null)
                emoticonMount.DecreaseUseCount();

            // ĳ���� Ű ���� ��ġ ���� �߰� 
            SetCharacterTallAdjustment();

            // �Ʒ��� ���� ��ǳ�� �ػ� ���� 
            SetBottomLinePositionAdjustment();

            // �����±� ����
            SetNametag();

            OnBubble();

            if (!string.IsNullOrEmpty(__row.voice))
                GameManager.main.SoundGroup[1].PlayVoice(__row.voice);          // 21.09.27 ��ȭ ���� ���ø� ������ �̰����� ������ ����ϵ��� �Ѵ�

            __cb?.Invoke();
        }


        /// <summary>
        /// ��ȭ ��ȭ ��ǳ��
        /// </summary>
        /// <param name="pos">��ǳ�� ��ġ</param>
        /// <param name="isSelf">true = ��ȭ����, false = ��ȭ���</param>
        public void ForPhoneBubble(ScriptRow __row, Action __cb, int pos, bool isSelf)
        {
            InitTransform();
            SetParams(__row, __cb, pos);

            // bubble size, pos ���� ���ص� �͵� �޾ƿ�
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
                #region Sync ó��

                #region TEXTAREA ó�� 
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_TOP), out textAreaTop);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_BOTTOM), out textAreaBottom);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_LEFT), out textAreaLeft);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_TEXTAREA_RIGHT), out textAreaRight);

                // 21.08.03 ���Ƿ� textArea �� �߰� ����(��ȭ��� ���ø���)
                textAreaTop += 25f;
                textAreaBottom -= 15f;
                textAreaLeft += 40f;
                textAreaRight += 30f;

                // right, top�� - ó���Ѵ�.
                textAreaTop *= -1;
                textAreaRight *= -1;

                textContents.rectTransform.offsetMin = new Vector2(textAreaLeft, textAreaBottom);
                textContents.rectTransform.offsetMax = new Vector2(textAreaRight, textAreaTop);
                #endregion

                #region SPRITE ó�� 

                imageBubble.sprite = BubbleManager.main.GetForPhoneSprite(bubbleSize);
                imageBubble.SetNativeSize();
                
                #endregion

                #region position, scale ó�� 
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_POS_X), out posX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_POS_Y), out posY);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_SCALE_X), out scaleX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_SCALE_Y), out scaleY);


                if (!isFakeBubble)
                {
                    // 21.08.03 ��ǳ�� X�� ��ġ ����(��ȭ��� ��ǳ�� ����)
                    posX += 70f;

                    rtransform.anchoredPosition = new Vector2(posX, posY);
                    rtransform.localScale = new Vector3(scaleX, scaleY, 1);
                }

                #endregion

                #region �̸�Ƽ�� ó�� 

                if (string.IsNullOrEmpty(emoticon_expression))
                    return;

                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_POS_X), out emoticonPosX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_POS_Y), out emoticonPosY);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_SCALE_X), out emoticonScaleX);
                float.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_SCALE_Y), out emoticonScaleY);

                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_WIDTH), out emoticonWidth);
                int.TryParse(SystemManager.GetJsonNodeString(targetBubbleJson, COL_EMOTICON_HEIGHT), out emoticonHeight);

                // 21.08.03 ��ǳ�� �̸�Ƽ�� ��ġ ����(��ȭ��� ��ǳ�� ����)
                emoticonPosX -= 150f;
                emoticonPosY = -220f;

                // ��ǳ�� ����� �Ȱ��� �� ������
                emoticon.rectTransform.anchoredPosition = new Vector3(emoticonPosX, emoticonPosY, 0);
                emoticon.rectTransform.localScale = new Vector3(emoticonScaleX, emoticonScaleY, 1);
                emoticon.rectTransform.sizeDelta = new Vector2(emoticonWidth, emoticonHeight);

                if (emoticonWidth <= 0 && emoticonHeight <= 0)
                    isEmoticonAvailable = false;
                else
                    isEmoticonAvailable = true;

                #endregion

                // ��ǳ�� ������ �༭ ���ڵ� ������
                textContents.rectTransform.localScale = new Vector3(textContents.rectTransform.localScale.x, textContents.rectTransform.localScale.y, textContents.rectTransform.localScale.z);

                #endregion

                SetTextFontStyle();
            }

            SetBubbleEmoticon();

            SetNametag();

            PhoneBubbleOn(isSelf);

            __cb?.Invoke();

        }


        #region ��ǳ�� On/Off

        /// <summary>
        /// ��ǳ�� ���� ó�� 
        /// </summary>
        public void OnBubble()
        {
            ActiveInEffect();
        }

        void ActiveInEffect()
        {
            rtransform.DOKill();

            // default ó��
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
            if (in_effect == GameConst.INOUT_EFFECT_FADEIN) // ���̵��� 
            {
                InitColor();
                SetTransparentTextColor();
                SetFadeInImages(0.4f);
            }
            else if (in_effect == GameConst.INOUT_EFFECT_SHAKE) // ������
            {
                this.gameObject.SetActive(true);
                rtransform.DOPunchRotation(new Vector3(0, 0, 20), 0.2f, 40);
            }
            else   // ������ ��
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
        /// ��ǳ�� ���� ó��
        /// </summary>
        public void OffBubble(bool immediate = false)
        {
            // ��� ó�� 
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
        /// ���̵�ƿ� ó��
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


        #region ��ȭ ���� ��ǳ�� On/Off

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
        /// ����̽� ��ũ�� ������ ���� �ϴ� ���� ��ǳ�� ��ġ ����
        /// </summary>
        void SetBottomLinePositionAdjustment()
        {
            if (bubblePos < 7)
                return;


            // ! 7,8,9 ��ġ�� �����Ѵ�. 
            // ! ���ΰ� ������� ����̽��� 7,8,9 ��ġ�� ���� �� �Ʒ������� ó�� 
            if (SystemManager.screenRatio > 0.56f)
                return;

            // 100��ŭ �Ʒ��� �̵� 
            rtransform.anchoredPosition = new Vector2(posX, posY - 100);

        }

        /// <summary>
        /// ȭ���� 'Ű'�� ���� ��ǳ�� ��ġ ���� 
        /// </summary>
        void SetCharacterTallAdjustment()
        {
            // ĳ���� Ű�� ��ǳ�� ��ġ �����ϱ� .
            if (string.IsNullOrEmpty(row.speaker))
                return;

            // �ι�° �������� ���� 
            if (bubblePos > 6)
                return;


            // ���ĵ��� �������� Ű üũ�� �Ѵ�.
            if (GameManager.main.CheckModelStanding() < 1)
                return;

            // Ű �������� 
            speakerTall = GameManager.main.GetCurrentModelTall(row.speaker);

            // Ű ���̿� ���� ��ǳ�� ��ġ ����
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
                // ĳ���� Ű ����� 5�̻��̸� �������� ���� 
                case 5:
                default:
                    break;
            }
        }
    }
}