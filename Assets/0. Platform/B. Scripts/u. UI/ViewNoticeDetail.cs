using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class ViewNoticeDetail : CommonView
    {
        static JsonData detailData;
        static string startDate;

        public TextMeshProUGUI noticeTitle;
        public TextMeshProUGUI noticeDate;
        public TextMeshProUGUI noticeContentText;
        public ImageRequireDownload noticeContentImage;

        public ScrollRect noticeDetailScroll;
        public RectTransform textContent;
        public RectTransform imageContent;

        const string COL_CONTENTS = "contents";

        const string DETAIL_BANNER_URL = "detail_banner_url";
        const string DETAIL_BANNER_KEY = "detail_banner_key";

        public static void SetNoticeDetail(JsonData __j, string __date)
        {
            detailData = __j;
            startDate = __date;
        }

        public override void OnStartView()
        {
            base.OnStartView();
            
            noticeTitle.text = SystemManager.GetJsonNodeString(detailData, LobbyConst.STORY_TITLE);
            noticeDate.text = startDate;
            noticeContentText.text = SystemManager.GetJsonNodeString(detailData, COL_CONTENTS);

            noticeDetailScroll.content = textContent;
            textContent.gameObject.SetActive(true);
            imageContent.gameObject.SetActive(false);


            if (string.IsNullOrEmpty(noticeContentText.text))
            {
                noticeDetailScroll.content = imageContent;
                textContent.gameObject.SetActive(false);
                imageContent.gameObject.SetActive(true);
                noticeContentImage.SetDownloadURL(SystemManager.GetJsonNodeString(detailData, DETAIL_BANNER_URL), SystemManager.GetJsonNodeString(detailData, DETAIL_BANNER_KEY));
            }
        }
    }
}
