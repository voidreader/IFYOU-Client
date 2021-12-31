using System.Collections.Generic;
using UnityEngine;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using DanielLochner.Assets.SimpleScrollSnap;

namespace PIERStory
{
    public class ViewIfyouSelection : CommonView
    {
        public SimpleScrollSnap scrollSnap;
        
        public TextMeshProUGUI currentRound;
        public TextMeshProUGUI prevRound;
        public TextMeshProUGUI formerTimesRound;

        public TextMeshProUGUI nonePlayText;
        public GameObject prevRoundScroll;
        public GameObject formerTimesRoundScroll;

        public Transform currentContent;
        public Transform prevContent;
        public Transform formerTimesContent;

        public GameObject episodeTitlePrefab;
        public GameObject prevScriptPrefab;
        public GameObject selectionScriptPrefab;
        public GameObject emptyPrefab;
        public GameObject endingTitlePrefab;

        List<GameObject> createObject = new List<GameObject>();

        public Sprite selectBoxSprite;
        public Sprite unselectBoxSprite;

        Color selectTextColor = new Color32(79, 79, 79, 255);
        Color unSelectTextColor = new Color32(196, 196, 196, 255);

        // Input System을 사용하려고 했으나, 촉박한 일정으로 인해 일단 InputManager를 사용하여 구현하고
        // 이후 슬금슬금 익혀서 변경하기로 한다
        // Input System이 2019년도에 나왔기 때문에, InputManager가 언제 소리소문 없이 사라질지 모르기 때문에 익혀둘 필요가 있다
        Vector3 touchInPos = Vector3.forward;
        Vector3 touchOutPos = Vector3.back;

        private void Update()
        {
            if (Application.isEditor)
            {
                if (Input.GetMouseButtonDown(0))
                    touchInPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Input.GetMouseButtonUp(0))
                {
                    touchOutPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    CheckOutPosition();
                }
            }
            else
            {
                if (Input.touchCount < 1)
                    return;

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                    touchInPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    touchOutPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    CheckOutPosition();
                }
            }
        }


        /// <summary>
        /// 드래그를 좌우로 일정 이상 하는 경우 다음 페이지, 혹은 이전 페이지로 scrollSnap 되도록 한다
        /// </summary>
        void CheckOutPosition()
        {
            if (touchInPos.x - touchOutPos.x > 4.5f)
                scrollSnap.GoToNextPanel();
            else if (touchInPos.x - touchOutPos.x < -4.5f)
                scrollSnap.GoToPreviousPanel();
        }
        
        
        void OnEnable() {
            // 상태 저장 
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
        }

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, "선택지", string.Empty);

            // 선택지 데이터
            JsonData selectionData = SystemManager.GetJsonNode(UserManager.main.currentStorySelectionHistoryJson, GameConst.TEMPLATE_SELECTION);
            // 선택에 의한 엔딩 데이터
            JsonData endingData = SystemManager.GetJsonNode(UserManager.main.currentStorySelectionHistoryJson, CommonConst.COL_ENDING);

            // 순서대로 넣고, 오브젝트를 역순으로 넣어주자
            // 서버에서 최신순으로 보내주는 것이 아니라 1회차부터 순서대로 정렬되어 오고 있으며, key값이 메인으로 오고 있기 때문에 오브젝트를 역순으로 넣어주게 되었다
            int reverse = 0;

            // 3회까지 안했을 수도 있으니까 스크롤들을 비활성화 해준다
            if (selectionData.Count < 3)
            {
                formerTimesRoundScroll.SetActive(false);
                reverse = 1;

                // 이번이 첫 플레이면 이전회차도 없다
                if (selectionData.Count < 2)
                {
                    prevRoundScroll.SetActive(false);
                    reverse = 2;
                }
            }

            scrollSnap.Setup();

            if(selectionData.Count == 0)
            {
                currentRound.text = "1회차";
                nonePlayText.gameObject.SetActive(true);
                return;
            }


            foreach(string roundKey in selectionData.Keys)
            {
                // 몇번째 회차인지 설정
                string roundText = string.Format("{0}회차", roundKey);
                nonePlayText.gameObject.SetActive(false);

                if (reverse == 0)
                    formerTimesRound.text = roundText;
                else if (reverse == 1)
                    prevRound.text = roundText;
                else
                    currentRound.text = roundText;

                int episodeNum = 1;

                foreach(string titleKey in selectionData[roundKey].Keys)
                {
                    // 에피소드 제목 설정
                    SelectionEpisodeTitleElement titleElement = Instantiate(episodeTitlePrefab).GetComponent<SelectionEpisodeTitleElement>();
                    titleElement.SetEpisodeTitle(episodeNum, titleKey);
                    episodeNum++;

                    SetObjectParent(reverse, titleElement.gameObject);
                    titleElement.transform.localScale = Vector3.one;
                    createObject.Add(titleElement.gameObject);

                    foreach(string prevScriptKey in selectionData[roundKey][titleKey].Keys)
                    {
                        // 선택지 전 대사 설정
                        SelectionPrevScriptElement prevScript = Instantiate(prevScriptPrefab).GetComponent<SelectionPrevScriptElement>();
                        prevScript.SetPrevScript(prevScriptKey);
                        SetObjectParent(reverse, prevScript.gameObject);
                        prevScript.transform.localScale = Vector3.one;
                        createObject.Add(prevScript.gameObject);

                        // 선택지 셋팅
                        JsonData selectionGroup = selectionData[roundKey][titleKey][prevScriptKey];

                        for (int i = 0; i < selectionGroup.Count; i++)
                        {
                            SelectionScriptElement selectionScript = Instantiate(selectionScriptPrefab).GetComponent<SelectionScriptElement>();
                            string scriptData = SystemManager.GetJsonNodeString(selectionGroup[i], "selection_content");

                            // 선택한 선택지인지?
                            if (SystemManager.GetJsonNodeBool(selectionGroup[i], "selected"))
                                selectionScript.SetSelectionScript(selectBoxSprite, scriptData, selectTextColor);
                            else
                                selectionScript.SetSelectionScript(unselectBoxSprite, scriptData, unSelectTextColor);

                            SetObjectParent(reverse, selectionScript.gameObject);
                            selectionScript.transform.localScale = Vector3.one;
                            createObject.Add(selectionScript.gameObject);
                        }
                    }

                    GameObject emp = Instantiate(emptyPrefab);
                    SetObjectParent(reverse, emp);
                    emp.transform.localScale = Vector3.one;
                    createObject.Add(emp);
                }

                //이전 회차에 대해 엔딩 셋팅
                string endingType = SystemManager.GetJsonNodeString(endingData[roundKey][0], LobbyConst.ENDING_TYPE);

                // 현재 회차에는 엔딩에 도달하지 못했을 수도 있으니 return 한다
                if (string.IsNullOrEmpty(endingType))
                    return;

                if(endingType == LobbyConst.COL_HIDDEN)
                    endingType = SystemManager.GetLocalizedText("5087");
                else
                    endingType = SystemManager.GetLocalizedText("5088");

                SelectionEndingTitleElement endingTitleElement = Instantiate(endingTitlePrefab).GetComponent<SelectionEndingTitleElement>();
                endingTitleElement.SetEndingTitle(string.Format("{0}. {1}", endingType, SystemManager.GetJsonNodeString(endingData[roundKey][0], "ending_title")));

                SetObjectParent(reverse, endingTitleElement.gameObject);
                endingTitleElement.transform.localScale = Vector3.one;
                createObject.Add(endingTitleElement.gameObject);
                
                // 한 회차 셋팅을 끝냈으니 그 다음 회차 셋팅을 해주기 위해 값을 증가한다
                reverse++;
            }
        }
        


        /// <summary>
        /// 생성한 Object의 부모를 설정한다
        /// </summary>
        /// <param name="index">전전인지, 이전인지, 현재인지</param>
        /// <param name="g">생성한 GameObject</param>
        void SetObjectParent(int index, GameObject g)
        {
            switch (index)
            {
                case 0:
                    g.transform.SetParent(formerTimesContent);
                    break;
                case 1:
                    g.transform.SetParent(prevContent);
                    break;
                default:
                    g.transform.SetParent(currentContent);
                    break;
            }
        }

        public override void OnHideView()
        {
            base.OnHideView();

            foreach (GameObject g in createObject)
                Destroy(g);

            createObject.Clear();

            // 스크롤들이 다시 또 사용될 수 있으므로 활성화!
            prevRoundScroll.SetActive(true);
            formerTimesRoundScroll.SetActive(true);
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);
        }
    }
}
