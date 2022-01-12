using UnityEngine;
using UnityEngine.UI;

namespace PIERStory
{
    public class PopupMessageAlert : PopupBase
    {
        public Image popupBox;

        public Sprite spritePositiveBox;
        public Sprite spriteNegativeBox;

        public override void Show()
        {
            base.Show();

            if (Data.isPositive)
                popupBox.sprite = spritePositiveBox;
            else
                popupBox.sprite = spriteNegativeBox;
        }
    }
}