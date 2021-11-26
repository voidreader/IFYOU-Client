using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class MissionElement : MonoBehaviour
    {
        public ImageRequireDownload missionThumbnail;
        public TextMeshProUGUI missionTitle;
        public TextMeshProUGUI missionHint;

        public GameObject rewardInfo;
        public GameObject completeMark;

        public Image currencyIcon;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI currencyAmount;
        public TextMeshProUGUI missionState;
        public Image stateButtonImage;

        enum MissionState { LOCK, OPEN, GOT }
        MissionState state = MissionState.LOCK;

        const string MISSION_HINT = "misson_hint";
        const string REWARD_EXP = "reward_exp";
        const string REWARD_CURRENCY = "reward_currency";
        const string REWARD_QUANTITY = "reward_quantity";
        const string UNLOCK_STATE = "unlock_state";

        Color32 missionGreen = new Color32(69, 198, 80, 255);

        public void InitMission(JsonData __j)
        {
            missionThumbnail.SetDownloadURL(SystemManager.GetJsonNodeString(__j, CommonConst.COL_IMAGE_URL), SystemManager.GetJsonNodeString(__j, CommonConst.COL_IMAGE_KEY));
            missionTitle.text = SystemManager.GetJsonNodeString(__j, LobbyConst.MISSION_NAME);
            missionHint.text = SystemManager.GetJsonNodeString(__j, MISSION_HINT);

            expText.text = string.Format("EXP {0}", SystemManager.GetJsonNodeString(__j, REWARD_EXP));

            SetCurrencyIcon(SystemManager.GetJsonNodeString(__j, REWARD_CURRENCY), SystemManager.GetJsonNodeString(__j, REWARD_QUANTITY));
            SetMissionState(SystemManager.GetJsonNodeString(__j, UNLOCK_STATE));
        }

        void SetCurrencyIcon(string __type, string __amount)
        {
            switch (__type)
            {
                case CommonConst.NONE:
                    currencyIcon.gameObject.SetActive(false);
                    return;
                        
                case LobbyConst.COIN:
                    break;

                case LobbyConst.GEM:
                    break;

                default:
                    break;
            }

            currencyAmount.text = string.Format("<b>{0}</b><size=17>개</size>", __amount);
        }

        void SetMissionState(string __state)
        {
            rewardInfo.SetActive(true);
            completeMark.SetActive(false);

            switch (__state)
            {
                case "0":
                    state = MissionState.OPEN;
                    missionState.color = Color.white;
                    stateButtonImage.color = missionGreen;
                    break;
                case "1":
                    state = MissionState.GOT;
                    rewardInfo.SetActive(false);
                    completeMark.SetActive(true);
                    break;

                default:
                    state = MissionState.LOCK;
                    missionState.color = missionGreen;
                    stateButtonImage.color = Color.white;
                    break;
            }
        }

        public void OnClickGetReward()
        {
            // 열려 있는 상태가 아니라면 동작하지 않는다
            if (state != MissionState.OPEN)
                return;
        }
    }
}
