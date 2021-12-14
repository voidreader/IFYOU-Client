﻿using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewEnding : CommonView
    {
        public TextMeshProUGUI collectionText;
        public TextMeshProUGUI collectionPercentage;
        public Image collectionGauge;

        public EndingElement[] endingElements;

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_VIEW_NAME, true, string.Empty);

            collectionText.text = string.Format("수집률 ({0}/{1})", StoryManager.main.unlockEndingCount, StoryManager.main.totalEndingCount);

            float percentage = (float)(StoryManager.main.unlockEndingCount / StoryManager.main.totalEndingCount);
            collectionPercentage.text = string.Format("{0}%", Mathf.Round(percentage * 100f));
            collectionGauge.fillAmount = percentage;

            #region 엔딩 요소 pooling

            int elemmentIndex = 0;

            for(int i=0;i<StoryManager.main.ListCurrentProjectEpisodes.Count;i++)
            {
                if (StoryManager.main.ListCurrentProjectEpisodes[i].endingOpen)
                {
                    endingElements[elemmentIndex].InitEndingInfo(StoryManager.main.ListCurrentProjectEpisodes[i]);
                    elemmentIndex++;
                }
            }

            #endregion
        }

    }
}
