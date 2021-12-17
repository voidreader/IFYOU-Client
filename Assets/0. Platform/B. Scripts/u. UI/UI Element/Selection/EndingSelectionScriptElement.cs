using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class EndingSelectionScriptElement : MonoBehaviour
    {
        public TextMeshProUGUI prevScript;
        public TextMeshProUGUI selectionScript;

        public void SetSelectionScript(string prevData, string selectionScriptData)
        {
            prevScript.text = prevData;
            selectionScript.text = selectionScriptData;
        }
    }
}