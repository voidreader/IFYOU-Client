using UnityEngine;

using LitJson;

namespace PIERStory
{
    public class PromotionProject : MonoBehaviour
    {
        public ImageRequireDownload promotionBanner;
        
        [SerializeField] string bannerURL = string.Empty;
        [SerializeField] string bannerKey = string.Empty;
        
        StoryData storyData = null;

        /// <summary>
        /// 프로모션 프로젝트 세팅
        /// </summary>
        public void SetPromotionProject(string __projectId, JsonData detail)
        {

            // 프로젝트 Id값을 받아서 해당 id와 동일한 storyData를 찾아서 넣어준다
            storyData = StoryManager.main.FindProject(__projectId);
            
            Debug.Log("SetPromotionProject detail count : " + detail.Count);
            
            for (int i = 0; i < detail.Count; i++)
            {
                // 해당 국가 코드에 맞는 promotion banner 이미지를 세팅한다
                if (SystemManager.GetJsonNodeString(detail[i], LobbyConst.COL_LANG) == SystemManager.main.currentAppLanguageCode.ToUpper())
                {
                    bannerURL = SystemManager.GetJsonNodeString(detail[i], LobbyConst.NODE_PROMOTION_BANNER_URL);
                    bannerKey = SystemManager.GetJsonNodeString(detail[i], LobbyConst.NODE_PROMOTION_BANNER_KEY);
                    
                    promotionBanner.SetDownloadURL(bannerURL, bannerKey, true);
                    break;
                }
                else {
                    // Debug.Log(string.Format("{0}/{1}", SystemManager.GetJsonNodeString(detail[i], LobbyConst.COL_LANG),SystemManager.main.currentAppLanguageCode));
                }
            }
        }


        public void OnClickPromotionBanner()
        {
            // StoryManager.main.RequestStoryInfo(storyData);
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE, storyData);
        }

    }
}