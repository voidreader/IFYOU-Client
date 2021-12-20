using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class SelectionEpisodeTitleElement : MonoBehaviour
    {
        public TextMeshProUGUI episodeNumber;
        public TextMeshProUGUI episodeTitle;

        /// <summary>
        /// EndingDetail과 Ifyou선택지 페이지에서 몇번째 에피소드와 제목에 대해 셋팅
        /// </summary>
        /// <param name="num">에피소드 num</param>
        /// <param name="title">에피소드 제목</param>
        public void SetEpisodeTitle(int num, string title)
        {
            episodeNumber.text = string.Format("{0}", num);
            episodeTitle.text = title;
        }
    }
}
