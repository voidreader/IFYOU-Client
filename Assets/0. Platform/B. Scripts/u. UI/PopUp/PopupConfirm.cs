using UnityEngine;
using UnityEngine.UI;

namespace PIERStory {
    public class PopupConfirm : PopupBase
    {
        public Image popupBox;
        public GameObject okButton;
        public GameObject positiveStyleButtons;
        public GameObject negativeStyleButtons;

        public Sprite spritePositiveBackground;
        public Sprite spriteNegativeBackground;

        public override void Show()
        {
            base.Show();

            okButton.SetActive(false);
            positiveStyleButtons.SetActive(false);
            negativeStyleButtons.SetActive(false);

            // 긍정적 내용인 경우
            if (Data.isPositive)
            {
                popupBox.sprite = spritePositiveBackground;
                positiveStyleButtons.SetActive(true);
            }
            else
            {
                popupBox.sprite = spriteNegativeBackground;
                negativeStyleButtons.SetActive(true);
            }

            // 확인버튼만 필요한 경우
            if(!Data.isConfirm)
            {
                okButton.SetActive(true);
                positiveStyleButtons.SetActive(false);
                negativeStyleButtons.SetActive(false);
            }
        }
    }
}