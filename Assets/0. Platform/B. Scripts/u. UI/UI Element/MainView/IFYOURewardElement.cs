using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
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
        string currency = string.Empty;
        public TextMeshProUGUI rewardAmount;
        int attendanceId = 7;
        int daySeq = 1;

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
        public void InitContinuousAttendanceReward(JsonData __j, bool getable)
        {
            InitCommonReward();

            rewardDayText.gameObject.SetActive(true);

            rewardDayText.text = SystemManager.GetJsonNodeString(__j, "day_seq");
            currency = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            attendanceId = SystemManager.GetJsonNodeInt(__j, "attendance_id");
            daySeq = SystemManager.GetJsonNodeInt(__j, "day_seq");

            switch (currency)
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
                    rewardCurrency.OnDownloadImage = CurrencyLoadComplete;
                    rewardCurrency.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY_KEY), true);
                    break;
            }

            rewardAmount.gameObject.SetActive(currency == LobbyConst.COIN || currency == LobbyConst.GEM);

            rewardHalo.gameObject.SetActive(!SystemManager.GetJsonNodeBool(__j, "reward_check") && getable);

            if (rewardHalo.gameObject.activeSelf)
            {
                rewardHalo.DOKill();
                rewardHalo.color = new Color(1, 1, 1, 0);
                rewardHalo.DOFade(1, 1).SetLoops(-1, LoopType.Yoyo);
            }

            rewardMask.sprite = rewardHalo.gameObject.activeSelf ? LobbyManager.main.spriteCircleOpen : LobbyManager.main.spriteCircleBase;

            getRewardCheck.SetActive(SystemManager.GetJsonNodeBool(__j, "reward_check"));
            disableBox.SetActive(!getable);
        }


        /// <summary>
        /// 기본적인 보상 세팅
        /// </summary>
        public void InitIfyouPlayReward(JsonData __j)
        {
            InitCommonReward();

            currency = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            daySeq = SystemManager.GetJsonNodeInt(__j, "day_seq");

            switch (currency)
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
                    rewardCurrency.SetDownloadURL(SystemManager.GetJsonNodeString(__j, "icon_image_url"), SystemManager.GetJsonNodeString(__j, "icon_image_key"));
                    break;
            }

            rewardHalo.gameObject.SetActive(!SystemManager.GetJsonNodeBool(__j, "is_receive") && SystemManager.GetJsonNodeBool(__j, "click_check") && SystemManager.GetJsonNodeBool(__j, "current"));

            if(rewardHalo.gameObject.activeSelf)
            {
                rewardHalo.DOKill();
                rewardHalo.color = new Color(1, 1, 1, 0);
                rewardHalo.DOFade(1, 1).SetLoops(-1, LoopType.Yoyo);
            }

            rewardMask.sprite = rewardHalo.gameObject.activeSelf ? LobbyManager.main.spriteSquareOpen : LobbyManager.main.spriteSquareBase;
            getRewardCheck.SetActive(SystemManager.GetJsonNodeBool(__j, "is_receive"));
        }

        void InitCommonReward()
        {
            disableBox.SetActive(false);
            rewardDayText.gameObject.SetActive(false);

            rewardHalo.SetNativeSize();
            rewardMask.SetNativeSize();
        }    



        /// <summary>
        /// 연속 출석 보상 받기 버튼 클릭
        /// </summary>
        public void OnClickContinuousAttendanceReward()
        {
            getRewardButton.interactable = false;

            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "receiveAttendanceMissionReward";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending["request_day"] = daySeq;

            NetworkLoader.main.SendPost(CallbackReceiveContinuousAttendanceReward, sending, true);
        }

        void CallbackReceiveContinuousAttendanceReward(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackReceiveContinuousAttendanceReward");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            // 연속출석 미션 보상 받고 난 후, 이프유플레이 JsonData를 갱신해준다
            UserManager.main.RefreshIfyouplayJsonData(result);
            SystemManager.ShowSimpleAlertLocalize("6177", false);

            // 이프유플레이 화면 갱신이 필요함
            MainIfyouplay.OnRefreshIfyouplay?.Invoke();
        }

        /// <summary>
        /// Daily attendance 보상 받기버튼 클릭
        /// </summary>
        public void OnClickDailyAttendanceReward()
        {
            // 터치 연타 막기
            getRewardButton.interactable = false;

            NetworkLoader.main.SendAttendanceReward(attendanceId, daySeq, CallbackAttendanceReward);
        }

        void CallbackAttendanceReward(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackAttendanceReward");
                getRewardButton.interactable = true;
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            // 보상을 받았으니 리스트를 갱신해주자
            UserManager.main.SetNotificationInfo(result);
            UserManager.main.RefreshIfyouplayJsonData(result);

            // 이프유 업적
            NetworkLoader.main.RequestIFYOUAchievement(2);

            NetworkLoader.main.RequestIFYOUAchievement(7);

            SystemManager.ShowSimpleAlertLocalize("6177", false);

            MainIfyouplay.OnRefreshIfyouplay?.Invoke();
            getRewardButton.interactable = true;
        }


        /// <summary>
        /// 보상 받기
        /// </summary>
        public void OnClickGetReward()
        {
            
        }

       
        void CurrencyLoadComplete()
        {
            rewardIcon.rectTransform.sizeDelta = Vector2.one * 100f;
        }
    }
}