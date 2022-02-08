
namespace PIERStory
{
    public class PopupEndingAlert : PopupBase
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