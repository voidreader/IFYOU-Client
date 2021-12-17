using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class EndingTitleElement : MonoBehaviour
    {
        public TextMeshProUGUI endingTitle;

        public void SetEndingTitle(string title)
        {
            endingTitle.text = title;
        }
    }
}
