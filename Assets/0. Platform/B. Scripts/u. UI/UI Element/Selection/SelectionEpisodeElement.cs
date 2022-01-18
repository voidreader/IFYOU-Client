using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class SelectionEpisodeElement : MonoBehaviour
    {
        public TextMeshProUGUI prevScript;
        public TextMeshProUGUI selectionScript;

        /// <summary>
        /// 에피소드 종료 페이지에서 현재 화에서 선택한 선택지 정리해서 보여주기
        /// </summary>
        /// <param name="prevData">선택지 직전 대사</param>
        /// <param name="selectionData">선택한 선택지</param>
        public void SetCurrentEpisodeSelection(string prevData, string selectionData)
        {
            prevScript.text = prevData.Replace('\\', ' ');
            selectionScript.text = selectionData.Replace("\\", " ");
        }
    }
}
