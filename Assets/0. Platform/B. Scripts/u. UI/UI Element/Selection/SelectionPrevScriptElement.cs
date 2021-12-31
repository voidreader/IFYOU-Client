using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class SelectionPrevScriptElement : MonoBehaviour
    {
        public TextMeshProUGUI prevScript;

        /// <summary>
        /// Ifyou 선택지에서 선택지 전 대사에 대한 element(요소) 셋팅
        /// </summary>
        /// <param name="script">선택지 전 대사</param>
        public void SetPrevScript(string script)
        {
            prevScript.text = script;
            prevScript.text = prevScript.text.Replace("\\", " ");
        }
    }
}
