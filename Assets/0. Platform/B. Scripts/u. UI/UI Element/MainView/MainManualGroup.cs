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


            if (SystemManager.GetJsonNodeString(__j, "array_kind") == "1*N")
            {
                foreach (LobbyStoryElement se in verticalTypeStoryElements)
                    se.gameObject.SetActive(false);

                for (int i = 0; i < projectList.Length; i++)
                {
                    StoryData storyData = StoryManager.main.FindProject(projectList[i]);
                    verticalTypeStoryElements[i].Init(storyData, true, SystemManager.GetJsonNodeBool(__j, "is_favorite"), SystemManager.GetJsonNodeBool(__j, "is_view"));
                }
            }
            else
            {
                foreach (LobbyStoryElement se in horizontalTypeStoryElements)
                    se.gameObject.SetActive(false);

                for (int i = 0; i < projectList.Length; i++)
                {
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