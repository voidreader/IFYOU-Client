using UnityEngine;

namespace PIERStory
{
    public class PopupLobbyType : PopupBase
    {
        public GameObject okButton;
        public GameObject confirmButtons;


        public override void Show()
        {
            base.Show();

            if(Data.isConfirm)
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