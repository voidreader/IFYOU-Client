using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class SelectionEndingTitleElement : MonoBehaviour
    {
        public TextMeshProUGUI endingTitle;

        /// <summary>
        /// EndingDetail과 Ifyou 선택지 페이지에서 도달한 엔딩 타입(히든, 최종)과 제목 셋팅
        /// </summary>
        /// <param name="title"></param>
        public void SetEndingTitle(string title)
        {
            SystemManager.SetText(endingTitle, title);
        }
    }
}
