using UnityEngine;

using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class NoticeElement : MonoBehaviour
    {
        public ImageRequireDownload noticeBanner;

        JsonData detailNotice = null;
        string startDate = string.Empty;

        const string START_DATE = "start_date";
        const string SHOW_NOTICE_DETAIL = "showNoticeDetail";

        public void InitNoticeBanner(JsonData __j)
        {
            detailNotice = null;
            gameObject.SetActive(true);

            JsonData detailData = SystemManager.GetJsonNode(__j, LobbyConst.NODE_DETAIL);
            startDate = SystemManager.GetJsonNodeString(__j, START_DATE);

            for (int i=0;i< detailData.Count;i++)
            {
                if (SystemManager.GetJsonNodeString(detailData[i], LobbyConst.COL_LANG).Equals("KO"))
                {
                    detailNotice = detailData[i];
                    break;
                }
            }

            if(detailNotice == null)
            {
                Debug.Log("No Korean Detail");
                return;
            }

            noticeBanner.SetDownloadURL(SystemManager.GetJsonNodeString(detailNotice, LobbyConst.BANNER_URL), SystemManager.GetJsonNodeString(detailNotice, LobbyConst.BANNER_KEY));
        }

        public void OnClickNoticeBanner()
        {
            // ViewNoticeDetail.SetNoticeDetail(detailNotice, startDate);
            Signal.Send(LobbyConst.STREAM_IFYOU, SHOW_NOTICE_DETAIL, string.Empty);
        }
    }
}
