using UnityEngine;
using UnityEngine.EventSystems;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class DecoTextElement : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        enum INPUT_STATE
        {
            None, Move, Write
        }

        public TMP_Text textComponent;

        public GameObject selectedBox;
        public GameObject deleteButton;

        TouchScreenKeyboard mobileKeyboard;
        INPUT_STATE state = INPUT_STATE.None;

        public void NewTextProfile(Color c, int fontSize)
        {
            textComponent.color = c;
            textComponent.fontSize = fontSize;
            state = INPUT_STATE.None;
            textComponent.GetComponent<RectTransform>().sizeDelta = textComponent.GetPreferredValues(textComponent.text) * Vector2.one;
            GetComponent<RectTransform>().sizeDelta = new Vector2(textComponent.GetComponent<RectTransform>().sizeDelta.x + 30, textComponent.GetComponent<RectTransform>().sizeDelta.y + 30);
        }

        /// <summary>
        /// 저장해두었던 프로필 텍스트 object 세팅
        /// </summary>
        public void SetProfileText(JsonData __j)
        {
            textComponent.text = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_INPUT_TEXT);
            textComponent.fontSize = int.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_FONT_SIZE));
            textComponent.color = HexCodeChanger.HexToColor(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_COLOR_RGB));
            GetComponent<RectTransform>().anchoredPosition = new Vector2(float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_POS_X)), float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_POS_Y)));
            textComponent.GetComponent<RectTransform>().sizeDelta = textComponent.GetPreferredValues(textComponent.text) * Vector2.one;
            GetComponent<RectTransform>().sizeDelta = new Vector2(textComponent.GetComponent<RectTransform>().sizeDelta.x + 30, textComponent.GetComponent<RectTransform>().sizeDelta.y + 30);
            state = INPUT_STATE.None;
        }

        private void Update()
        {
            if (state == INPUT_STATE.None || mobileKeyboard == null)
                return;

            textComponent.text = mobileKeyboard.text;
            textComponent.GetComponent<RectTransform>().sizeDelta = textComponent.GetPreferredValues(textComponent.text) * Vector2.one;
            GetComponent<RectTransform>().sizeDelta = new Vector2(textComponent.GetComponent<RectTransform>().sizeDelta.x + 30, textComponent.GetComponent<RectTransform>().sizeDelta.y + 30);
        }

        public void DeleteTextObject()
        {
            Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {

            switch (state)
            {
                case INPUT_STATE.None:
                    selectedBox.SetActive(true);
                    state = INPUT_STATE.Move;
                    break;
                case INPUT_STATE.Move:
                    mobileKeyboard = TouchScreenKeyboard.Open(textComponent.text, TouchScreenKeyboardType.Default, true, true, false);
                    state = INPUT_STATE.Write;
                    break;
                case INPUT_STATE.Write:
                    break;
            }
        }

        public void DisableOptional()
        {
            //inputField.enabled = false;
            selectedBox.SetActive(false);
            state = INPUT_STATE.None;
            mobileKeyboard = null;
        }

        #region Drag Action

        public void OnBeginDrag(PointerEventData eventData)
        {
            deleteButton.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            deleteButton.SetActive(true);
            state = INPUT_STATE.None;
        }

        #endregion

        public JsonData SaveJsonData(int sortingOrder)
        {
            JsonData data = new JsonData();

            data[LobbyConst.NODE_INPUT_TEXT] = textComponent.text;
            data[LobbyConst.NODE_FONT_SIZE] = textComponent.fontSize;
            data[LobbyConst.NODE_COLOR_RGB] = ColorUtility.ToHtmlStringRGB(textComponent.color);
            data[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
            data[LobbyConst.NODE_POS_X] = GetComponent<RectTransform>().anchoredPosition.x;
            data[LobbyConst.NODE_POS_Y] = GetComponent<RectTransform>().anchoredPosition.y;
            data[LobbyConst.NODE_ANGLE] = 0f;

            return data;
        }
    }
}