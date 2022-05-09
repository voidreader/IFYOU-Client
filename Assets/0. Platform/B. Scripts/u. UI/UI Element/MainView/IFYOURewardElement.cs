using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using DG.Tweening;

namespace PIERStory
{
    public class IFYOURewardElement : MonoBehaviour
    {
        public Image rewardHalo;
        Button getRewardButton;

        public Image rewardMask;
        public Image rewardIcon;
        ImageRequireDownload rewardCurrency;
        public TextMeshProUGUI rewardAmount;

        public GameObject getRewardCheck;

        // 연속 출석체크에서만 사용될 Objects
        public GameObject disableBox;           // 비활성화 상태 표시
        public TextMeshProUGUI rewardDayText;   // 연속 출석 보상일

        private void Start()
        {
            rewardCurrency = rewardIcon.GetComponent<ImageRequireDownload>();
            getRewardButton = rewardHalo.GetComponent<Button>();
        }

        /// <summary>
        /// 연속 출석 보상 세팅
        /// </summary>
        public void InitContinuousAttendanceReward(JsonData __j)
        {
            InitIfyouPlayReward(__j);

            rewardDayText.gameObject.SetActive(true);

            rewardDayText.text = SystemManager.GetJsonNodeString(__j, "day_seq");

            switch (SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY))
            {
                case LobbyConst.COIN:
                case LobbyConst.GEM:

                    if (SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY) == LobbyConst.COIN)
                        rewardIcon.sprite = SystemManager.main.spriteCoin;
                    else
                        rewardIcon.sprite = SystemManager.main.spriteStar;

                    rewardAmount.text = SystemManager.GetJsonNodeString(__j, CommonConst.NODE_QUANTITY);
                    break;

                default:
                    rewardCurrency.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY), true);
                    break;
            }

            rewardAmount.gameObject.SetActive(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY) == LobbyConst.COIN || SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY) == LobbyConst.GEM);

        }


        /// <summary>
        /// 기본적인 보상 세팅
        /// </summary>
        public void InitIfyouPlayReward(JsonData __j)
        {
            disableBox.SetActive(false);
            rewardDayText.gameObject.SetActive(false);

            rewardHalo.SetNativeSize();
            rewardMask.SetNativeSize();


        }


        /// <summary>
        /// 보상 받기
        /// </summary>
        public void OnClickGetReward()
        {
            // 연속 터치 막기
            getRewardButton.interactable = false;
        }
    }
}