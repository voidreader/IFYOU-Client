using UnityEngine;
using UnityEngine.UI;

namespace PIERStory
{
    public class PopupGameType : PopupBase
    {
        public Image popupBox;
        public GameObject okButton;
        public GameObject confirmButtons;

        public Sprite spritePositiveBackground;
        public Sprite spriteNegativeBackground;

        public override void Show()
        {
            base.Show();

            if (Data.isPositive)
                popupBox.sprite = spritePositiveBackground;
            else
                popupBox.sprite = spriteNegativeBackground;


            if (Data.isConfirm)
            {
                okButton.SetActive(false);
                confirmButtons.SetActive(true);
            }
            else
            {
                okButton.SetActive(true);
                confirmButtons.SetActive(false);
            }
        }
    }
}