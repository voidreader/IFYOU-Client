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
        
        public int currentResult = 0;
        public int limitCount = 0;
        public int received = 0; 

        /// <summary>
        /// 베이스 값 설정 
        /// </summary>
        /// <param name="__j"></param>
        void InitBaseParams(JsonData __j) {
            currentResult = SystemManager.GetJsonNodeInt(__j, "current_result");
            limitCount = SystemManager.GetJsonNodeInt(__j, "limit_count");
            received = SystemManager.GetJsonNodeInt(__j, "received");
        }



        /// <summary>
        /// 기본적인 보상 세팅
        /// </summary>
        public void InitDailyAttendanceReward(JsonData __j)
        {
            InitCommonReward();

            currency = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);
            daySeq = SystemManager.GetJsonNodeInt(__j, LobbyConst.NODE_DAY_SEQ);

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
                    rewardCurrency.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_IMAGE_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_IMAGE_KEY));
                    break;
            }

            rewardHalo.gameObject.SetActive(!SystemManager.GetJsonNodeBool(__j, "is_receive") && SystemManager.GetJsonNodeBool(__j, "click_check") && SystemManager.GetJsonNodeBool(__j, "current"));

            HaloEffect();

            rewardMask.sprite = rewardHalo.gameObject.activeSelf ? LobbyManager.main.spriteSquareOpen : LobbyManager.main.spriteSquareBase;
            getRewardCheck.SetActive(SystemManager.GetJsonNodeBool(__j, "is_receive"));
        }

        /// <summary>
        /// 전체미션보상 세팅
        /// </summary>
        /// <param name="__j"></param>
        public void InitDailyTotalReward(JsonData __j)
        {
            
            InitCommonReward();
            InitBaseParams(__j);

            currency = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

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
            }
            
            // 보상을 받을 수 있는 상태
            rewardHalo.gameObject.SetActive(currentResult >= limitCount);
            rewardMask.sprite = rewardHalo.gameObject.activeSelf ? LobbyManager.main.spriteCircleOpen : LobbyManager.main.spriteCircleBase;
            
            // 보상을 받았는지? 
            getRewardCheck.SetActive(received > 0);

            HaloEffect();
        }

        /// <summary>
        /// 데일리 미션 개별 세팅
        /// </summary>
        /// <param name="__j"></param>
        public void InitDailyMissionReward(JsonData __j)
        {
            InitCommonReward();
            InitBaseParams(__j);

            currency = SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_CURRENCY);

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
                    rewardCurrency.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_IMAGE_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.NODE_ICON_IMAGE_KEY));
                    break;
            }

            rewardHalo.gameObject.SetActive(currentResult >= limitCount);
            rewardMask.sprite = rewardHalo.gameObject.activeSelf ? LobbyManager.main.spriteSquareOpen : LobbyManager.main.spriteSquareBase;
            
            // 보상을 받았는지? 
            getRewardCheck.SetActive(received > 0);

            HaloEffect();
        }


        void InitCommonReward()
        {
            disableBox.SetActive(false);
            rewardDayText.gameObject.SetActive(false);

            rewardHalo.SetNativeSize();
            rewardMask.SetNativeSize();

            rewardCurrency = rewardIcon.GetComponent<ImageRequireDownload>();
        }    



        /// <summary>
        /// 연속 출석 보상 받기 버튼 클릭
        /// </summary>
        public void OnClickContinuousAttendanceReward()
        {
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
            NetworkLoader.main.SendAttendanceReward(attendanceId, daySeq, CallbackAttendanceReward);
        }

        void CallbackAttendanceReward(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackAttendanceReward");
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
        }


        /// <summary>
        /// 데일리 전체미션 달성 보상 받기
        /// </summary>
        public void OnClickGetTotalMissionReward()
        {
            UserManager.main.RequestDailyMissionReward(1);
        }




        void CurrencyLoadComplete()
        {
            rewardIcon.rectTransform.sizeDelta = Vector2.one * 100f;
        }


        void HaloEffect()
        {
            if (!rewardHalo.gameObject.activeSelf)
                return;

            rewardHalo.DOKill();
            rewardHalo.color = new Color(1, 1, 1, 0);
            rewardHalo.DOFade(1, 1).SetLoops(-1, LoopType.Yoyo);
        }    
    }
}