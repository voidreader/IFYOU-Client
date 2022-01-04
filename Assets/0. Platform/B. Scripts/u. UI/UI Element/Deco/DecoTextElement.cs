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

        public TMP_InputField inputField;
        public TMP_Text textComponent;

        public GameObject selectedBox;
        public GameObject deleteButton;

        INPUT_STATE state = INPUT_STATE.None;

        public void NewTextProfile(Color c, int fontSize)
        {
            inputField.text = "New Text";
            textComponent.color = c;

            inputField.pointSize = fontSize;
            state = INPUT_STATE.None;
        }

        /// <summary>
        /// 저장해두었던 프로필 텍스트 object 세팅
        /// </summary>
        public void SetProfileText(JsonData __j)
        {
            inputField.text = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_INPUT_TEXT);
            inputField.pointSize = int.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_FONT_SIZE));
            textComponent.fontSize = inputField.pointSize;
            textComponent.color = HexCodeChanger.HexToColor(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_COLOR_RGB));
            inputField.GetComponent<RectTransform>().anchoredPosition = new Vector2(float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_POS_X)), float.Parse(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_POS_Y)));
            textComponent.GetComponent<RectTransform>().sizeDelta = textComponent.GetPreferredValues(textComponent.text) * Vector2.one;
            inputField.GetComponent<RectTransform>().sizeDelta = new Vector2(textComponent.GetComponent<RectTransform>().sizeDelta.x + 30, textComponent.GetComponent<RectTransform>().sizeDelta.y + 30);
            state = INPUT_STATE.None;
        }

        private void Update()
        {
            if (state == INPUT_STATE.None)
                return;

            textComponent.GetComponent<RectTransform>().sizeDelta = textComponent.GetPreferredValues(textComponent.text) * Vector2.one;
            inputField.GetComponent<RectTransform>().sizeDelta = new Vector2(textComponent.GetComponent<RectTransform>().sizeDelta.x + 30, textComponent.GetComponent<RectTransform>().sizeDelta.y + 30);
        }

        public void DeleteTextObject()
        {
            Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 프로필(메인) 화면에서 클릭 되지 않도록(Caret 때문에 선택이 됨)
            if (!inputField.interactable)
                return;

            if(!selectedBox.activeSelf)
                ViewProfileDeco.OnDisableAllOptionals?.Invoke();

            switch (state)
            {
                case INPUT_STATE.None:
                    selectedBox.SetActive(true);
                    state = INPUT_STATE.Move;
                    break;
                case INPUT_STATE.Move:
                    inputField.enabled = true;
                    state = INPUT_STATE.Write;
                    break;
                case INPUT_STATE.Write:
                    break;
            }
        }

        public void DisableOptional()
        {
            inputField.enabled = false;
            selectedBox.SetActive(false);
            state = INPUT_STATE.None;
        }

        #region Drag Action

        public void OnBeginDrag(PointerEventData eventData)
        {
            deleteButton.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!inputField.interactable)
                return;

            transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            deleteButton.SetActive(true);
        }

        #endregion

        public JsonData SaveJsonData(int sortingOrder)
        {
            JsonData data = new JsonData();

            data[LobbyConst.NODE_INPUT_TEXT] = inputField.text;
            data[LobbyConst.NODE_FONT_SIZE] = inputField.pointSize;
            data[LobbyConst.NODE_COLOR_RGB] = ColorUtility.ToHtmlStringRGB(textComponent.color);
            data[LobbyConst.NODE_SORTING_ORDER] = sortingOrder;
            data[LobbyConst.NODE_POS_X] = inputField.GetComponent<RectTransform>().anchoredPosition.x;
            data[LobbyConst.NODE_POS_Y] = inputField.GetComponent<RectTransform>().anchoredPosition.y;
            data[LobbyConst.NODE_ANGLE] = 0f;

            return data;
        }
    }
}