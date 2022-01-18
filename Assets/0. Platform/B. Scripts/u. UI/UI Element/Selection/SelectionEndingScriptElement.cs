using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class SelectionEndingScriptElement : MonoBehaviour
    {
        public TextMeshProUGUI prevScript;
        public TextMeshProUGUI selectionScript;

        /// <summary>
        /// 엔딩 페이지의 EndingDetail의 선택지 보는 것에서 선택지 전 대사와 사용자가 고른 선택지를 세팅해준다
        /// </summary>
        /// <param name="prevData">선택지 전 대사</param>
        /// <param name="selectionScriptData">사용자가 선택한 선택지</param>
        public void SetSelectionScript(string prevData, string selectionScriptData)
        {
            prevScript.text = prevData.Replace('\\', ' ');
            selectionScript.text = selectionScriptData;
        }
    }
}