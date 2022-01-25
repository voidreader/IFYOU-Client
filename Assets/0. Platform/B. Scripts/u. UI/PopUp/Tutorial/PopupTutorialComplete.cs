
namespace PIERStory
{
    public class PopupTutorialComplete : PopupBase
    {
        public override void Show()
        {
            base.Show();
            
            AdManager.main.AnalyticsEnter("tutorialClear");
        }
    }
}