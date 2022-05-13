using UnityEngine;

using LitJson;

namespace PIERStory
{
    public class PopupAllClearReward : PopupBase
    {
        [Space(15)]
        public ImageRequireDownload decoCurrency;

        string iconUrl = string.Empty, iconKey = string.Empty;

        public override void Show()
        {
            base.Show();

            JsonData rewardData = SystemManager.GetJsonNode(Data.contentJson, "reward");

            if (rewardData == null)
                return;

            for (int i = 0; i < rewardData.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_CURRENCY) == LobbyConst.COIN || SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_CURRENCY) == LobbyConst.GEM)
                    continue;

                iconUrl = SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_ICON_IMAGE_URL);
                iconKey = SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_ICON_IMAGE_KEY);
                break;
            }

            decoCurrency.SetDownloadURL(iconUrl, iconKey);
        }
    }
}