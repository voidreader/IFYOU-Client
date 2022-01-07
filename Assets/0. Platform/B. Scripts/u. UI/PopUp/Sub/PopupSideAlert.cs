
namespace PIERStory
{
    public class PopupSideAlert : PopupBase
    {
        public ImageRequireDownload thumbnail;

        public override void Show()
        {
            base.Show();

            thumbnail.SetDownloadURL(Data.imageURL, Data.imageKey);
        }
    }
}