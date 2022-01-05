using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewCoinStarHistory : CommonView
    {
        [Header("재화")]
        public Image starButton;
        public Image coinButton;
        public TextMeshProUGUI starText;
        public TextMeshProUGUI coinText;

        public UIToggle starToggle;
        public UIToggle coinToggle;

        [Space(20)][Header("날짜")]
        public Image _7dayButton;
        public Image _30dayButton;
        public Image _90dayButton;
        public TextMeshProUGUI _7dayText;
        public TextMeshProUGUI _30dayText;
        public TextMeshProUGUI _90dayText;

        public UIToggle _7dayToggle;
        public UIToggle _30dayToggle;
        public UIToggle _90dayToggle;

        public GameObject historyElementPrefab;
        public Transform historyContent;

        public GameObject noneAlertText;            // 내역이 존재하지 않을 때 띄워줄 object
        CoinStarHistoryElement historyElement;
        List<CoinStarHistoryElement> historyElements = new List<CoinStarHistoryElement>();

        [Space(20)][Header("Sprite resource")]
        public Sprite toggleOnSprite;
        public Sprite toggleOffSprite;
        public Sprite dayToggleOnSprite;
        public Sprite dayToggleOffSprite;

        Color toggleOnColor = new Color32(64, 64, 64, 255);
        Color toggleOffColor = new Color32(153, 153, 153, 255);

        const string FUNC_GET_USER_PROPERTY_HISTORY = "getUserPropertyHistory";
        const string PROPERTY_GEM = "gem";
        const string PROPERTY_COIN = "coin";
        const string KEY_PROPERTY = "property";
        const string KEY_RANGE = "range";

        bool viewShow = false;

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5047"), string.Empty);

            viewShow = true;

            _90dayToggle.isOn = false;
            _30dayToggle.isOn = false;
            coinToggle.isOn = false;

            starToggle.isOn = true;
            _7dayToggle.isOn = true;
        }

        public override void OnHideView()
        {
            base.OnHideView();

            viewShow = false;
        }


        #region Toggle Event

        /// <summary>
        /// 스타 충전 및 사용 내역 활성화
        /// </summary>
        public void EnableStarHistory()
        {
            starButton.sprite = toggleOnSprite;
            coinButton.sprite = toggleOffSprite;
            _7dayToggle.isOn = true;
            _30dayToggle.isOn = false;
            _90dayToggle.isOn = false;

            Enable7DayHistory();
        }

        /// <summary>
        /// 코인 및 사용 내역 활성화
        /// </summary>
        public void EnableCoinHistory()
        {
            starButton.sprite = toggleOffSprite;
            coinButton.sprite = toggleOnSprite;
            _7dayToggle.isOn = true;
            _30dayToggle.isOn = false;
            _90dayToggle.isOn = false;

            Enable7DayHistory();
        }

        /// <summary>
        /// 7일치 내역 활성화
        /// </summary>
        public void Enable7DayHistory()
        {
            if (!viewShow)
                return;

            _7dayButton.sprite = dayToggleOnSprite;
            _7dayText.color = toggleOnColor;
            _30dayButton.sprite = dayToggleOffSprite;
            _30dayText.color = toggleOffColor;
            _90dayButton.sprite = dayToggleOffSprite;
            _90dayText.color = toggleOffColor;

            if (starToggle.isOn)
                InquireUserHistory(PROPERTY_GEM, 7, CallbackGemHistory);

            if(coinToggle.isOn)
                InquireUserHistory(PROPERTY_COIN, 7, CallbackCoinHistory);
        }


        /// <summary>
        /// 30일치 내역 활성화
        /// </summary>
        public void Enable30DayHistory()
        {
            _7dayButton.sprite = dayToggleOffSprite;
            _7dayText.color = toggleOffColor;
            _30dayButton.sprite = dayToggleOnSprite;
            _30dayText.color = toggleOnColor;
            _90dayButton.sprite = dayToggleOffSprite;
            _90dayText.color = toggleOffColor;

            if (starToggle.isOn)
                InquireUserHistory(PROPERTY_GEM, 30, CallbackGemHistory);

            if (coinToggle.isOn)
                InquireUserHistory(PROPERTY_COIN, 30, CallbackCoinHistory);
        }

        /// <summary>
        /// 90일치 내역 활성화
        /// </summary>
        public void Enable90DayHistory()
        {
            _7dayButton.sprite = dayToggleOffSprite;
            _7dayText.color = toggleOffColor;
            _30dayButton.sprite = dayToggleOffSprite;
            _30dayText.color = toggleOffColor;
            _90dayButton.sprite = dayToggleOnSprite;
            _90dayText.color = toggleOnColor;

            if (starToggle.isOn)
                InquireUserHistory(PROPERTY_GEM, 90, CallbackGemHistory);

            if (coinToggle.isOn)
                InquireUserHistory(PROPERTY_COIN, 90, CallbackCoinHistory);
        }

        #endregion

        #region 내역 통신

        void InquireUserHistory(string property, int dateRange, OnRequestFinishedDelegate __cb)
        {
            if (NetworkLoader.main == null)
                return;

            JsonData sendingData = new JsonData();
            sendingData[CommonConst.FUNC] = FUNC_GET_USER_PROPERTY_HISTORY;
            sendingData[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sendingData[KEY_PROPERTY] = property;
            sendingData[KEY_RANGE] = dateRange;

            NetworkLoader.main.SendPost(__cb, sendingData, true);
        }

        void CallbackGemHistory(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackGemHistory");
                return;
            }

            CreateHistoryElement(JsonMapper.ToObject(res.DataAsText));
        }

        void CallbackCoinHistory(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackCoinHistory");
                return;
            }

            CreateHistoryElement(JsonMapper.ToObject(res.DataAsText));
        }

        #endregion

        void CreateHistoryElement(JsonData __j)
        {
            foreach (CoinStarHistoryElement csElement in historyElements)
                Destroy(csElement.gameObject);

            historyElements.Clear();

            if (__j.Count < 1)
            {
                noneAlertText.SetActive(true);
                return;
            }

            noneAlertText.SetActive(false);

            for (int i=0;i<__j.Count;i++)
            {
                historyElement = Instantiate(historyElementPrefab, historyContent).GetComponent<CoinStarHistoryElement>();
                historyElement.InitHistoryInfo(__j[i]);
                historyElements.Add(historyElement);
            }
        }

    }
}
