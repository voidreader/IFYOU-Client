using LitJson;

namespace PIERStory
{
    public class ViewNotice : CommonView
    {
        public NoticeElement[] noticeElements;
        
        public override void OnStartView()
        {
            base.OnStartView();

            foreach (NoticeElement ne in noticeElements)
                ne.gameObject.SetActive(false);

            JsonData noticeList = SystemManager.main.noticeData;

            for (int i = 0; i < noticeList.Count; i++)
                noticeElements[i].InitNoticeBanner(noticeList[i]);
        }
    }
}
