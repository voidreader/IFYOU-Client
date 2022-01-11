using UnityEngine;

using LitJson;

namespace PIERStory
{
    public class PromotionGoods : MonoBehaviour
    {
        public ImageRequireDownload goodsBanner;
        string promotionType = string.Empty;

        /// <summary>
        /// 프로모션 상품 세팅
        /// </summary>
        public void SetPromotionGoods(string type, JsonData detail)
        {
            promotionType = type;

            for (int i = 0; i < detail.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(detail[i], LobbyConst.COL_LANG) == SystemManager.main.currentAppLanguageCode)
                {
                    goodsBanner.SetDownloadURL(SystemManager.GetJsonNodeString(detail[i], LobbyConst.NODE_PROMOTION_BANNER_URL), SystemManager.GetJsonNodeString(detail[i], LobbyConst.NODE_PROMOTION_BANNER_KEY), true);
                    break;
                }
            }
        }

        
        public void OnClickPromotionGoods()
        {
            if(promotionType == LobbyConst.COL_STAR)
            {
                // 상점의 경우에는 상점페이지로 이동
                Debug.Log("Move to starShop");

                ViewMain.OnMoveStarShop?.Invoke();
            }
            else if(promotionType == LobbyConst.COL_COIN)
            {
                // 코인의 경우에는 웹뷰 오픈
                if (string.IsNullOrEmpty(SystemManager.main.coinShopURL))
                {
                    Debug.LogError("No Coinshop url");
                    return;
                }

                string uidParam = string.Format("?uid={0}", UserManager.main.GetUserPinCode());
                string langParam = string.Format("&lang={0}", SystemManager.main.currentAppLanguageCode);

                string finalURL = SystemManager.main.coinShopURL + uidParam + langParam;

                SystemManager.main.ShowDefaultWebview(finalURL);
            }
        }
    }
}