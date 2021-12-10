using System.Collections.Generic;
using UnityEngine;

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewOnetimeHistory : CommonView
    {
        public UIToggle getHistoryToggle;
        public UIToggle useHistoryToggle;

        public GameObject elementPrefab;
        public Transform content;

        // 생성된 1회플레이 내역 elements
        List<OnetimePlayHistoryElement> createHistoryElements = new List<OnetimePlayHistoryElement>();

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_VIEW_NAME, true, string.Empty);

            useHistoryToggle.isOn = false;
            getHistoryToggle.isOn = true;
        }

        public override void OnHideView()
        {
            base.OnHideView();
        }

        public void EnableGetHistory()
        {
            DestoryAllContents();

            OnetimePlayHistoryElement historyElement = Instantiate(elementPrefab, content).GetComponent<OnetimePlayHistoryElement>();
            historyElement.InitHistoryData(false);

        }

        public void EnableUseHistory()
        {
            DestoryAllContents();
        }

        /// <summary>
        /// 내역 목록 초기화
        /// </summary>
        void DestoryAllContents()
        {
            foreach (OnetimePlayHistoryElement historyElement in createHistoryElements)
                Destroy(historyElement.gameObject);

            createHistoryElements.Clear();
        }
    }
}
