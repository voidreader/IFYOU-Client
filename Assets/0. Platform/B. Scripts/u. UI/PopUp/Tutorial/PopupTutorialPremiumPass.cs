
namespace PIERStory
{
    public class PopupTutorialPremiumPass : PopupBase
    {
        public void OnClickNextStep()
        {
            Hide();
        }

        public void OpenNextTutorial()
        {
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_TUTORIAL_EPISODE_START);
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