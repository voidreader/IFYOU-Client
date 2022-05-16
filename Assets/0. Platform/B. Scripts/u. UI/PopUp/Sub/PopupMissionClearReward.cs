using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class PopupMissionClearReward : PopupBase
    {
        [Space(15)]
        public ImageRequireDownload decoCurrency;

        string iconUrl = string.Empty, iconKey = string.Empty;
        
        [Space(15)]
        public TextMeshProUGUI textCoinQuantity;
        public TextMeshProUGUI textGemQuantity;

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
                
                // 수량 추가 
                if (SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_CURRENCY) == LobbyConst.COIN) {
                    textCoinQuantity.text = SystemManager.GetJsonNodeString(rewardData[i], CommonConst.NODE_QUANTITY);
                    continue;
                }
                
                if (SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_CURRENCY) == LobbyConst.GEM) {
                    textGemQuantity.text = SystemManager.GetJsonNodeString(rewardData[i], CommonConst.NODE_QUANTITY);
                    continue;
                }
                

                iconUrl = SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_ICON_IMAGE_URL);
                iconKey = SystemManager.GetJsonNodeString(rewardData[i], LobbyConst.NODE_ICON_IMAGE_KEY);
                break;
            }

            decoCurrency.SetDownloadURL(iconUrl, iconKey);

            UserManager.main.SetBankInfo(Data.contentJson);

            UserManager.main.SetProjectMissionAllClear(1);
            ViewMission.OnCompleteReward?.Invoke();
        }
    }
}