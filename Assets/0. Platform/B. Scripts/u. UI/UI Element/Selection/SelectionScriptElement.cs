using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory
{
    public class SelectionScriptElement :MonoBehaviour
    {
        public Image selectionBox;
        public TextMeshProUGUI selectionScript;

        /// <summary>
        /// Ifyou 선택지 페이지의 선택지 요소(선택,미선택)값 세팅
        /// </summary>
        /// <param name="s">Image에 적용될 box sprite</param>
        /// <param name="script">선택지 대사</param>
        /// <param name="textColor">선택, 미선택 값에 따라 적용될 텍스트 색상</param>
        public void SetSelectionScript(Sprite s, string script, Color textColor)
        {
            selectionBox.sprite = s;
            SystemManager.SetText(selectionScript, script);
            selectionScript.color = textColor;

            selectionBox.SetNativeSize();
        }
    }
}
