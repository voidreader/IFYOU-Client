using System.Collections;
using UnityEngine;

namespace PIERStory
{
    public class PopupHelpBox : PopupBase
    {
        [Space(15)]
        public RectTransform backgroundBox;

        public override void Show()
        {
            base.Show();

            backgroundBox.sizeDelta = new Vector2(backgroundBox.sizeDelta.x, Data.contentValue);
        }
    }
}