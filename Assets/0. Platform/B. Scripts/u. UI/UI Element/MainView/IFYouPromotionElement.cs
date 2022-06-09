using UnityEngine;

using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory {

    /// <summary>
    /// 신규 이프유 프로모션 
    /// </summary>
    public class IFYouPromotionElement : MonoBehaviour
    {
        
        public ImageRequireDownload promotionBanner;
        
        [SerializeField] string bannerURL = string.Empty;
        [SerializeField] string bannerKey = string.Empty;
        
        JsonData allDetail; // 언어별 모든 정보 
        JsonData master; // 마스터 프로모션
        JsonData currentDetail; // 현재 언어 설정에 맞는 상세정보 
        
        public string promotionType = string.Empty;
        public string targetID = string.Empty;
        
        // * 작품 영역 
        public StoryData storyData = null; // 타겟 작품
        
        // * 공지사항 영역
        JsonData noticeData = null; // 타겟 공지사항
        JsonData noticeDetailData = null; // 타겟 공지사항 상세정보 
        
        
        public void SetPromotion(JsonData __master, JsonData __detail) {
            
            master = __master;
            allDetail = __detail;
            
            promotionType = SystemManager.GetJsonNodeString(master, "promotion_type");
            targetID = SystemManager.GetJsonNodeString(master, "location");
            
            // 이미지 세팅 
            for (int i = 0; i < allDetail.Count; i++)
            {
                // 해당 국가 코드에 맞는 promotion banner 이미지를 세팅한다
                if (SystemManager.GetJsonNodeString(allDetail[i], LobbyConst.COL_LANG) == SystemManager.main.currentAppLanguageCode.ToUpper())
                {
                    currentDetail = allDetail[i];
                    
                    bannerURL = SystemManager.GetJsonNodeString(currentDetail, LobbyConst.NODE_PROMOTION_BANNER_URL);
                    bannerKey = SystemManager.GetJsonNodeString(currentDetail, LobbyConst.NODE_PROMOTION_BANNER_KEY);
                    
                    promotionBanner.SetDownloadURL(bannerURL, bannerKey, true);
                    break;
                }
            }
            
            if(promotionType == "notice") { // 대상 공지사항 찾기 
                if(SystemManager.main.noticeData == null)
                    return;
                
                for(int i=0; i<SystemManager.main.noticeData.Count;i++) {
                    if(SystemManager.GetJsonNodeString(SystemManager.main.noticeData[i], "notice_no") == targetID) {
                        noticeData = SystemManager.main.noticeData[i];
                        break;
                    }
                }
            }
            else if(promotionType == "project") { // 작품일때는 대상 프로젝트 찾기. 
                storyData = StoryManager.main.FindProject(targetID);
            }

        }
        
        public void OnClickPromotion() {
            if(promotionType == "notice") { // 공지사항 오픈 
                if(noticeData == null) {
                    Debug.LogError(string.Format("Can't find notice [{0}]", targetID));
                    return;
                }
                
                // 상세정보 찾기 (언어..)
                noticeDetailData = null;
                for(int i=0; i<noticeData[LobbyConst.NODE_DETAIL].Count;i++) {
                    if (SystemManager.GetJsonNodeString(noticeData[LobbyConst.NODE_DETAIL][i], LobbyConst.COL_LANG) == SystemManager.main.currentAppLanguageCode)
                    {
                        noticeDetailData = noticeData[LobbyConst.NODE_DETAIL][i];
                        break;
                    }                    
                }

                 string startDate = SystemManager.GetJsonNodeString(noticeData, "start_date"); // 시작일자
                 
                 if(noticeDetailData == null) {
                     Debug.LogError(string.Format("Can't find notice detail [{0}]", targetID));
                     return;
                 }

                string urlLink = SystemManager.GetJsonNodeString(noticeDetailData, "url_link");

                if(!string.IsNullOrEmpty(urlLink))
                {
                    SystemManager.main.ShowDefaultWebview(urlLink, "NoticeDetail");
                    return;
                }

                PopupBase p = PopupManager.main.GetPopup("Notice");
                PopupManager.main.ShowPopup(p, false);

                PopupNotice.ShowNoticeDetail(noticeDetailData, startDate);
            }
            else if(promotionType == "project") { // 프로젝트 소개 페이지 오픈 
                if(storyData == null) {
                    Debug.LogError(string.Format("Can't find story [{0}]", targetID));
                }
                else {
                    Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE, storyData);
                    
                }
            }
            else if(promotionType == "page") {
                
                if(string.IsNullOrEmpty(targetID))
                    return;
                    
                Debug.Log("page target ID : " + targetID);
                
                if(targetID == "star") {
                    Signal.Send(LobbyConst.STREAM_COMMON, "Shop", string.Empty);
                }
                else if (targetID == "coin") {
                    // 웹 코인샵 오픈 
                    SystemManager.main.OpenCoinShopWebview();
                }
            }
        }
    }
}