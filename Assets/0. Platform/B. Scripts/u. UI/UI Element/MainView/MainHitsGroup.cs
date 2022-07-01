using System.Collections.Generic;
using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class MainHitsGroup : MonoBehaviour
    {
        public TextMeshProUGUI groupNameText;

        public List<LobbyStoryElement> storyElements;

        public void InitCategoryData(JsonData __j)
        {
            SystemManager.SetText(groupNameText, SystemManager.GetJsonNodeString(__j, "name_text"));
            string list = SystemManager.GetJsonNodeString(__j, "project_list");
            string[] projectList = list.Split(',');

            foreach (LobbyStoryElement se in storyElements)
                se.gameObject.SetActive(false);

            for (int i = 0; i < projectList.Length; i++)
            {
                StoryData storyData = StoryManager.main.FindProject(projectList[i]);
                storyElements[i].Init(storyData, true, SystemManager.GetJsonNodeBool(__j, "is_favorite"), SystemManager.GetJsonNodeBool(__j, "is_view"));
            }

        }
    }
}