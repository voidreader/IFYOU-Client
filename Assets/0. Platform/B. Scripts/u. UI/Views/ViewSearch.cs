using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using System.Collections;

namespace PIERStory
{
    public class ViewSearch : CommonView
    {
        public static Action<string> FindStoryForKeyword = null;
        public static Action<SearchRecordElement> DeleteRecordElement = null;

        TouchScreenKeyboard keypad;
        public TMP_InputField searchInputField;

        public GameObject searchRecordPrefab;
        public Transform searchRecordContent;
        JsonData recordData = null;
        List<string> searchKeyword = new List<string>();
        List<SearchRecordElement> recordElements = new List<SearchRecordElement>();         // 검색 기록 element들을 저장해둘 List

        List<StoryData> totalProjectList = null;
        public GameObject storyElementPrefab;
        public RectTransform searchResultContent;
        public Transform searchResultElements;
        List<NewStoryElement> createResults = new List<NewStoryElement>();                  // 검색 결과의 생성 object들을 담아둘 List

        [Space(20)]
        public GameObject noneRecordAlert;
        public GameObject searchRecord;
        public GameObject searchResult;

        public Transform bottomStorage;
        public GameObject deleteAllRecord;

        
        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);

            // 에디터 환경이 아닌 경우
            if (!Application.isEditor)
                keypad = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default);

            totalProjectList = StoryManager.main.listTotalStory;
            FindStoryForKeyword = FindKeywordProject;
            DeleteRecordElement = DeleteSearchRecord;

            recordData = new JsonData();

            if (!PlayerPrefs.HasKey(LobbyConst.KEY_SEARCH_RECORD))
                NoneDataSetting();
            else
            {
                recordData = JsonMapper.ToObject(PlayerPrefs.GetString(LobbyConst.KEY_SEARCH_RECORD));
                Debug.Log(JsonMapper.ToStringUnicode(recordData));

                foreach (SearchRecordElement sre in recordElements)
                    Destroy(sre.gameObject);

                recordElements.Clear();

                for (int i = recordData.Count - 1; i >= 0; i--)
                {
                    SearchRecordElement recordElement = Instantiate(searchRecordPrefab, searchRecordContent).GetComponent<SearchRecordElement>();
                    recordElement.InitData(recordData[i].ToString());
                    recordElements.Add(recordElement);
                }

                // 과거 기록도 추가해준다
                for (int i = 0; i < recordData.Count; i++)
                    searchKeyword.Add(recordData[i].ToString());

                noneRecordAlert.SetActive(false);
                searchRecord.SetActive(true);
                searchResult.SetActive(false);

                deleteAllRecord.transform.SetParent(bottomStorage);
                deleteAllRecord.transform.SetParent(searchRecordContent);
            }

            searchInputField.text = string.Empty;
        }

        public override void OnHideView()
        {
            base.OnHideView();

            FindStoryForKeyword = null;

            deleteAllRecord.transform.SetParent(bottomStorage);
        }

        private void Update()
        {
            // FindSotryForKeyword Action이 null값이면 이 페이지가 활성화된 상태가 아니므로 아래 동작들이 실행되지 않도록 한다
            if (FindStoryForKeyword == null)
                return;

            // 키패드가 활성화되어 있고, 에디터 환경이 아닌경우
            if (!Application.isEditor && keypad != null && keypad.active)
                searchInputField.text = keypad.text;

            // 엔터를 누르면 검색 하기
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (!Application.isEditor)
                    keypad.active = false;

                FindKeywordProject(searchInputField.text);
            }
        }

        /// <summary>
        /// 검색 기록 전체 삭제
        /// </summary>
        public void OnClickDeleteAllSearchRecord()
        {
            foreach (SearchRecordElement sre in recordElements)
                Destroy(sre.gameObject);

            recordElements.Clear();
            recordData.Clear();
            PlayerPrefs.DeleteKey(LobbyConst.KEY_SEARCH_RECORD);
            NoneDataSetting();
        }

        /// <summary>
        /// 단일 element 삭제(파괴)
        /// </summary>
        /// <param name="recordElement">선택한 검색 기록</param>
        void DeleteSearchRecord(SearchRecordElement recordElement)
        {
            searchKeyword.Remove(recordElement.recordText.text);
            Destroy(recordElement.gameObject);
            recordElements.Remove(recordElement);

            // 단일 삭제 했는데 List가 비었다면?
            if(recordElements.Count < 1)
                OnClickDeleteAllSearchRecord();
            else
            {
                recordData.Clear();

                for (int i = 0; i < recordElements.Count; i++)
                    recordData.Add(recordElements[i].recordText.text);

                PlayerPrefs.SetString(LobbyConst.KEY_SEARCH_RECORD, JsonMapper.ToJson(recordData));
            }
        }

        void FindKeywordProject(string keyword)
        {
            searchInputField.text = keyword;

            // 이전 검색 결과 삭제(파괴)
            foreach (NewStoryElement nse in createResults)
                Destroy(nse.gameObject);

            // List 청소
            createResults.Clear();

            for (int i = 0; i < totalProjectList.Count; i++)
            {
                // 검색어 키워드를 제목에 포함하고 있다면
                if (totalProjectList[i].title.Contains(keyword))
                {
                    NewStoryElement newStory = Instantiate(storyElementPrefab, searchResultElements).GetComponent<NewStoryElement>();
                    newStory.InitStoryElement(totalProjectList[i]);
                    createResults.Add(newStory);
                }
            }

            noneRecordAlert.SetActive(false);
            searchRecord.SetActive(false);
            searchResult.SetActive(true);

            StartCoroutine(UpdateLayout());
            
            // 중복 체크
            for (int i=0;i < searchKeyword.Count;i++)
            {
                if(searchKeyword[i].Equals(keyword))
                {
                    searchKeyword.Remove(keyword);
                    break;
                }
            }

            searchKeyword.Add(keyword);

            recordData.Clear();
            foreach (string key in searchKeyword)
                recordData.Add(key);

            PlayerPrefs.SetString(LobbyConst.KEY_SEARCH_RECORD, JsonMapper.ToJson(recordData));
            Debug.Log(JsonMapper.ToStringUnicode(recordData));
        }

        IEnumerator UpdateLayout()
        {
            searchResultElements.GetComponent<GridLayoutGroup>().enabled = false;
            searchResultContent.GetComponent<VerticalLayoutGroup>().enabled = false;
            
            yield return null;

            searchResultElements.GetComponent<GridLayoutGroup>().enabled = true;
            searchResultContent.GetComponent<VerticalLayoutGroup>().enabled = true;
        }

        /// <summary>
        /// 검색 기록 존재 안함
        /// </summary>
        void NoneDataSetting()
        {
            noneRecordAlert.SetActive(true);
            searchRecord.SetActive(false);
            searchResult.SetActive(false);
        }

        public void SearchHistorySetting()
        {
            if (!PlayerPrefs.HasKey(LobbyConst.KEY_SEARCH_RECORD))
                NoneDataSetting();
            else
            {
                foreach (SearchRecordElement sre in recordElements)
                    Destroy(sre.gameObject);

                recordElements.Clear();

                recordData = JsonMapper.ToObject(PlayerPrefs.GetString(LobbyConst.KEY_SEARCH_RECORD));
                for (int i = recordData.Count - 1; i >= 0; i--)
                {
                    SearchRecordElement recordElement = Instantiate(searchRecordPrefab, searchRecordContent).GetComponent<SearchRecordElement>();
                    recordElement.InitData(recordData[i].ToString());
                    recordElements.Add(recordElement);
                }

                noneRecordAlert.SetActive(false);
                searchRecord.SetActive(true);
                searchResult.SetActive(false);

                deleteAllRecord.transform.SetParent(bottomStorage);
                deleteAllRecord.transform.SetParent(searchRecordContent);
            }
        }
    }
}
