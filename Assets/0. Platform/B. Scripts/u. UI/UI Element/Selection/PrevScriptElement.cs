using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class PrevScriptElement : MonoBehaviour
    {
        public TextMeshProUGUI prevScript;

        public void SetPrevScript(string script)
        {
            prevScript.text = script;
        }
    }
}
