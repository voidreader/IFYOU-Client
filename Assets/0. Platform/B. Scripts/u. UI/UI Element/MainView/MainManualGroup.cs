using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class MainManualGroup : MonoBehaviour
    {
        public RectTransform listTitle;
        public TextMeshProUGUI groupNameText;

        [Space(15)][Header("VerticalType")]
        public RectTransform verticalStyle;
        public List<LobbyStoryElement> verticalTypeStoryElements;

        [Header("HorizontalType")]
        public RectTransform horizontalStyle;
        public List<LobbyStoryElement> horizontalTypeStoryElements;


        public void InitCategoryData(JsonData __j)
        {
            SystemManager.SetText(groupNameText, SystemManager.GetJsonNodeString(__j, "name_text"));
            string list = SystemManager.GetJsonNodeString(__j, "project_list");

            if (string.IsNullOrEmpty(list))
            {
                Destroy(gameObject);
                return;
            }

            string[] projectList = list.Split(',');

            // 최대 3개만 들어가는 유형의 배열을 사용하는 경우
            if (SystemManager.GetJsonNodeString(__j, "array_kind") == "1*N")
            {
                foreach (LobbyStoryElement se in verticalTypeStoryElements)
                    se.gameObject.SetActive(false);

                for (int i = 0; i < projectList.Length; i++)
                {
                    // 3개 초과로 들어가면 멈춰
                    if (i >= verticalTypeStoryElements.Count)
                        break;

                    if (string.IsNullOrEmpty(projectList[i]))
                        continue;

                    StoryData storyData = StoryManager.main.FindProject(projectList[i]);
                    verticalTypeStoryElements[i].Init(storyData, true, SystemManager.GetJsonNodeBool(__j, "is_favorite"), SystemManager.GetJsonNodeBool(__j, "is_view"));
                }
            }
            // 최대 10개가 들어가는 유형의 한 라인에 2개씩 들어가는 배열을 사용하는 경우
            else
            {
                foreach (LobbyStoryElement se in horizontalTypeStoryElements)
                    se.gameObject.SetActive(false);

                for (int i = 0; i < projectList.Length; i++)
                {
                    // 10개 초과로 들어가면 멈춰!
                    if (i >= horizontalTypeStoryElements.Count)
                        break;

                    if (string.IsNullOrEmpty(projectList[i]))
                        continue;

                    StoryData storyData = StoryManager.main.FindProject(projectList[i]);
                    horizontalTypeStoryElements[i].Init(storyData, true, SystemManager.GetJsonNodeBool(__j, "is_favorite"), SystemManager.GetJsonNodeBool(__j, "is_view"));
                }
            }

            verticalStyle.gameObject.SetActive(SystemManager.GetJsonNodeString(__j, "array_kind") == "1*N");
            horizontalStyle.gameObject.SetActive(SystemManager.GetJsonNodeString(__j, "array_kind") != "1*N");

            StartCoroutine(ResizeArea());
        }

        /// <summary>
        /// 화면 재배율
        /// </summary>
        /// <returns></returns>
        IEnumerator ResizeArea()
        {
            yield return null;
            RectTransform groupRect = GetComponent<RectTransform>();
            groupRect.sizeDelta = verticalStyle.gameObject.activeSelf ? new Vector2(720f, 130 + verticalStyle.sizeDelta.y) : new Vector2(720f, 130 + horizontalStyle.sizeDelta.y);
        }


        /// <summary>
        /// 장르일 경우에만 사용되는 탭 이동시켜주는 버튼 이벤트
        /// </summary>
        public void OnClickMoveLibraryTab()
        {

        }
    }
}