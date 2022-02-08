
namespace PIERStory
{
    public class PopupTutorialComplete : PopupBase
    {
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            
            AdManager.main.AnalyticsEnter("tutorial_clear");
        }
    }
}