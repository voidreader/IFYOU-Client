using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class EpisodeTitleElement : MonoBehaviour
    {
        public TextMeshProUGUI episodeNumber;
        public TextMeshProUGUI episodeTitle;

        public void SetEpisodeTitle(int num, string title)
        {
            episodeNumber.text = string.Format("{0}", num);
            episodeTitle.text = title;
        }
    }
}
