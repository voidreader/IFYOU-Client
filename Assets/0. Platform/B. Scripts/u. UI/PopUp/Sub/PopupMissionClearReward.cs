using UnityEngine;

using LitJson;

namespace PIERStory
{
    public class PopupMissionClearReward : PopupBase
    {
        [Space(15)]
        public ImageRequireDownload decoCurrency;

        string iconUrl = string.Empty, iconKey = string.Empty;

        public override void Show()
        {
            if (isShow)
                return;

            base.Show();

            JsonData rewardData = SystemManager.GetJsonNode(Data.contentJson, "reward");

            if (rewardData == null)
                return;

            for (int i = 0; i < rewardData.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_CURRENCY) == LobbyConst.COIN || SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_CURRENCY) == LobbyConst.GEM)
                    continue;

                iconUrl = SystemManager.GetJsonNodeString(rewardData[i], "icon_image_url");
                iconKey = SystemManager.GetJsonNodeString(rewardData[i], "icon_image_key");
                break;
            }

            decoCurrency.SetDownloadURL(iconUrl, iconKey);

            UserManager.main.SetBankInfo(Data.contentJson);

            UserManager.main.SetProjectMissionAllClear(1);
            ViewMission.OnCompleteReward?.Invoke();
        }
    }
}