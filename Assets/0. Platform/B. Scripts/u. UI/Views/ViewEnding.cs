using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using DanielLochner.Assets.SimpleScrollSnap;

namespace PIERStory
{
    public class ViewEnding : CommonView
    {
        public Image endingToggle;
        public Image choiceToggle;
        public GameObject endingListContents;
        public GameObject choiceHistoryContents;


        [Header("엔딩 관련")]
        public TextMeshProUGUI collectionText;
        public TextMeshProUGUI collectionPercentage;
        public Image collectionGauge;

        public EndingElement[] endingElements;


        [Space(20)][Header("선택지 관련")]
        public SimpleScrollSnap scrollSnap;

        public TextMeshProUGUI nonePlayText;
        public GameObject prevRoundScroll;

        public Transform currentContent;
        public Transform prevContent;

        public GameObject episodeTitlePrefab;
        public GameObject prevScriptPrefab;
        public GameObject selectionScriptPrefab;
        public GameObject emptyPrefab;
        public GameObject endingTitlePrefab;

        List<GameObject> createObject = new List<GameObject>();

        public Sprite selectBoxSprite;
        public Sprite unselectBoxSprite;

        readonly Color selectTextColor = new Color32(79, 79, 79, 255);
        readonly Color unSelectTextColor = new Color32(196, 196, 196, 255);

        float startX = 0f, dragX = 0f;
        Vector2 cursor = Vector2.zero;


        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5025"), string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);

            collectionText.text = string.Format(SystemManager.GetLocalizedText("6138"), StoryManager.main.unlockEndingCount, StoryManager.main.totalEndingCount);

            float percentage = (float)StoryManager.main.unlockEndingCount / (float)StoryManager.main.totalEndingCount;
            collectionGauge.fillAmount = percentage;
            collectionPercentage.text = string.Format("{0}%", Mathf.Round(percentage * 100f));


            // 엔딩 요소 pooling
            int endingElementIndex = 0;

            for (int i = 0; i < StoryManager.main.ListCurrentProjectEpisodes.Count; i++)
            {
                if (StoryManager.main.ListCurrentProjectEpisodes[i].episodeType == EpisodeType.Ending)
                {
                    endingElements[endingElementIndex].InitEndingInfo(StoryManager.main.ListCurrentProjectEpisodes[i]);
                    endingElementIndex++;
                }
            }


            #region 이전, 현재회차 선택지 보기

            // 선택지 데이터
            JsonData selectionData = SystemManager.GetJsonNode(UserManager.main.currentStorySelectionHistoryJson, GameConst.TEMPLATE_SELECTION);
            // 선택에 의한 엔딩 데이터
            JsonData endingData = SystemManager.GetJsonNode(UserManager.main.currentStorySelectionHistoryJson, CommonConst.COL_ENDING);


            // 이번이 첫플레이인 경우 이전 회차가 없다
            if(selectionData.Count < 2)
                prevRoundScroll.SetActive(false);

            scrollSnap.Setup();

            // 한번도 플레이한 적 없는 경우
            if (selectionData.Count < 1)
            {
                nonePlayText.gameObject.SetActive(true);
                return;
            }

            Stack<string> roundKeyStack = new Stack<string>();

            foreach (string roundKey in selectionData.Keys)
                roundKeyStack.Push(roundKey);

            while (roundKeyStack.Count > 2)
                roundKeyStack.Pop();

            while(roundKeyStack.Count > 0)
            {
                Transform parent = null;

                // 이전회차인지 현재회차인지에 따라서 생성될 위치가 다르다
                if (roundKeyStack.Count == 2)
                    parent = prevContent;
                else if (roundKeyStack.Count == 1)
                    parent = currentContent;

                string roundKey = roundKeyStack.Pop();
                int episodeNum = 1;

                foreach (string episodeTitle in selectionData[roundKey].Keys)
                {
                    // 에피소드 제목 설정
                    SelectionEpisodeTitleElement titleElement = Instantiate(episodeTitlePrefab, parent).GetComponent<SelectionEpisodeTitleElement>();
                    titleElement.SetEpisodeTitle(episodeNum, episodeTitle);
                    episodeNum++;
                    createObject.Add(titleElement.gameObject);

                    foreach (string prevScript in selectionData[roundKey][episodeTitle].Keys)
                    {
                        // 선택지 전 대사 설정
                        SelectionPrevScriptElement prevScriptElement = Instantiate(prevScriptPrefab, parent).GetComponent<SelectionPrevScriptElement>();
                        prevScriptElement.SetPrevScript(prevScript);
                        createObject.Add(prevScriptElement.gameObject);

                        // 선택지 셋팅
                        JsonData selectionGroup = selectionData[roundKey][episodeTitle][prevScript];

                        for (int i = 0; i < selectionGroup.Count; i++)
                        {
                            SelectionScriptElement selectionScript = Instantiate(selectionScriptPrefab, parent).GetComponent<SelectionScriptElement>();
                            string scriptData = SystemManager.GetJsonNodeString(selectionGroup[i], "selection_content");

                            // 선택한 선택지인가?
                            if (SystemManager.GetJsonNodeBool(selectionGroup[i], "selected"))
                                selectionScript.SetSelectionScript(selectBoxSprite, scriptData, selectTextColor);
                            else
                                selectionScript.SetSelectionScript(unselectBoxSprite, scriptData, unSelectTextColor);

                            createObject.Add(selectionScript.gameObject);
                        }
                    }

                    GameObject emptyBox = Instantiate(emptyPrefab, parent);
                    createObject.Add(emptyBox);
                }

                // 엔딩 세팅
                string endingType = SystemManager.GetJsonNodeString(endingData[roundKey][0], LobbyConst.ENDING_TYPE);

                if (string.IsNullOrEmpty(endingType))
                    return;

                if (endingType == LobbyConst.COL_HIDDEN)
                    endingType = SystemManager.GetLocalizedText("5087");
                else
                    endingType = SystemManager.GetLocalizedText("5088");

                SelectionEndingTitleElement endingTitleElement = Instantiate(endingTitlePrefab, parent).GetComponent<SelectionEndingTitleElement>();
                endingTitleElement.SetEndingTitle(string.Format("{0}. {1}", endingType, SystemManager.GetJsonNodeString(endingData[roundKey][0], "ending_title")));

                createObject.Add(endingTitleElement.gameObject);
            }

            #endregion

        }


        public override void OnHideView()
        {
            base.OnHideView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, true, string.Empty);

            for (int i = 0; i < endingElements.Length; i++)
                endingElements[i].gameObject.SetActive(false);

            foreach (GameObject g in createObject)
                Destroy(g);

            createObject.Clear();

            prevRoundScroll.SetActive(true);
        }

        public void ShowEndingList()
        {
            endingToggle.sprite = LobbyManager.main.toggleSelected;
            choiceToggle.sprite = LobbyManager.main.toggleUnselected;

            endingListContents.SetActive(true);
            choiceHistoryContents.SetActive(false);
        }


        public void ShowChoiceHistory()
        {
            endingToggle.sprite = LobbyManager.main.toggleUnselected;
            choiceToggle.sprite = LobbyManager.main.toggleSelected;

            endingListContents.SetActive(false);
            choiceHistoryContents.SetActive(true);
        }


        public void OnDragScreen(InputAction.CallbackContext context)
        {
            cursor = context.ReadValue<Vector2>();

            if(context.started)
            {
                startX = cursor.x;
            }
            else if(context.performed)
            {
                dragX = cursor.x;
            }
            else if(context.canceled)
            {
                if (startX - dragX > 4.5f)
                    scrollSnap.GoToNextPanel();
                else if (startX - dragX < -4.5f)
                    scrollSnap.GoToPreviousPanel();
            }
        }

    }
}
