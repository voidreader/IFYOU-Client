
namespace PIERStory
{
    public class PopupSideAlert : PopupBase
    {
        public ImageRequireDownload thumbnail;

        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();

            thumbnail.SetDownloadURL(Data.imageURL, Data.imageKey);
        }
    }
}