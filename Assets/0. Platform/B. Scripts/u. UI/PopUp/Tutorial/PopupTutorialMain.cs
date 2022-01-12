
namespace PIERStory
{
    public class PopupTutorialMain : PopupBase
    {

        public void OnClickTutorialProject(string __projectId)
        {
            Hide();
            StoryData storyData = StoryManager.main.FindProject(__projectId);
            StoryManager.main.RequestStoryInfo(storyData);
        }


        public void OnClickCloseTutorial()
        {
            SystemManager.ShowLobbyPopup(SystemManager.GetLocalizedText("80108"), CancelTutorial, null);
        }

        /// <summary>
        /// 튜토리얼 포기
        /// </summary>
        void CancelTutorial()
        {
            UserManager.main.UpdateTutorialStep(3);
            Hide();
        }
    }
}