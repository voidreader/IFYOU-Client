using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class MissionHintElement : MonoBehaviour
    {
        public GameObject radioButton;

        public TextMeshProUGUI episodeTitle;
        public TextMeshProUGUI totalCount;

        public void InitMissionHint(bool isComplete, string __title, string __count)
        {
            radioButton.SetActive(isComplete);

            SystemManager.SetText(episodeTitle, __title);
            SystemManager.SetText(totalCount, __count);
        }
    }
}