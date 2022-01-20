
namespace PIERStory
{
    public class PopupTutorialStarPlay : PopupBase
    {

        public void OnClickNextStep()
        {
            Hide();
        }

        public void OpenNextTutorial()
        {
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_TUTORIAL_PREMIUM_PASS);
            PopupManager.main.ShowPopup(p, false);
        }

        public void OnClickCloseTutorial()
        {
            SystemManager.ShowLobbyPopup(SystemManager.GetLocalizedText("80108"), CancelTutorial, null);
        }

        void CancelTutorial()
        {
            UserManager.main.UpdateTutorialStep(3);
            Hide();
        }
    }
}