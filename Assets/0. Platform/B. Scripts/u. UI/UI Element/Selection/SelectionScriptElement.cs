using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory
{
    public class SelectionScriptElement :MonoBehaviour
    {
        public Image selectionBox;
        public TextMeshProUGUI selectionScript;

        public void SetSelectionScript(Sprite s, string script, Color textColor)
        {
            selectionBox.sprite = s;
            selectionScript.text = script;
            selectionScript.color = textColor;
        }
    }
}
