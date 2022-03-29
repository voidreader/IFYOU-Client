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
        
        bool isFromView = false;

        /// <summary>
        /// 프로모션에서 독립적으로 띄워질때는 fromView가 true로 온다. 
        /// </summary>
        /// <param name="__j"></param>
        /// <param name="__fromView"></param>
        public void InitNoticeBanner(JsonData __j, bool __fromView = false)
        {
            
            isFromView = __fromView;
            
            detailNotice = null;
            gameObject.SetActive(true);

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

            noticeBanner.SetDownloadURL(SystemManager.GetJsonNodeString(detailNotice, LobbyConst.BANNER_URL), SystemManager.GetJsonNodeString(detailNotice, LobbyConst.BANNER_KEY));
        }

        public void OnClickNoticeBanner()
        {
            ViewNoticeDetail.isDependent = isFromView;
            ViewNoticeDetail.SetNoticeDetail(detailNotice, startDate);
            Signal.Send(LobbyConst.STREAM_IFYOU, SHOW_NOTICE_DETAIL, string.Empty);
        }
    }
}
