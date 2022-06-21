using UnityEngine;

using LitJson;

namespace PIERStory
{
    public class NoticeElement : MonoBehaviour
    {
        public ImageRequireDownload noticeBanner;

        JsonData detailNotice = null;
        string startDate = string.Empty;
        string urlLink = string.Empty;

        const string START_DATE = "start_date";

        /// <summary>
        /// 프로모션에서 독립적으로 띄워질때는 fromView가 true로 온다. 
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="__fromView"></param>
        public void InitNoticeBanner(JsonData __j, bool __fromView = false)
        {
            detailNotice = null;

            JsonData detailData = SystemManager.GetJsonNode(__j, LobbyConst.NODE_DETAIL);
            startDate = SystemManager.GetJsonNodeString(__j, START_DATE);

            for (int i=0;i< detailData.Count;i++)
            {
                // 앱 언어설정과 동일한 notice Detail 데이터를 넣어준다
                if (SystemManager.GetJsonNodeString(detailData[i], LobbyConst.COL_LANG) == SystemManager.main.currentAppLanguageCode)
                {
                    detailNotice = detailData[i];
                    break;
                }
            }

            if(detailNotice == null)
            {
                Debug.LogWarning("No Detail");
                return;
            }

            urlLink = SystemManager.GetJsonNodeString(detailNotice, "url_link");
            
            noticeBanner.SetDownloadURL(SystemManager.GetJsonNodeString(detailNotice, LobbyConst.BANNER_URL), SystemManager.GetJsonNodeString(detailNotice, LobbyConst.BANNER_KEY));
            gameObject.SetActive(true);
        }

        public void OnClickNoticeBanner()
        {
            if (string.IsNullOrEmpty(urlLink))
            {
                if (detailNotice != null)
                    PopupNotice.ShowNoticeDetail?.Invoke(detailNotice, startDate);
                else
                    SystemManager.ShowMessageAlert("Data error");
            }
            else
                SystemManager.main.ShowDefaultWebview(urlLink, "NoticeDetail");
        }
    }
}
