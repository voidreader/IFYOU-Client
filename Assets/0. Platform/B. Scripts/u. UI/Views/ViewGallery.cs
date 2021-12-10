using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewGallery : CommonView
    {
        public static Action<bool> OnDelayIllustOpen = null;

        [Header("Illust part")]
        public GameObject IllustScroll;
        public TextMeshProUGUI totalCollection;
        public TextMeshProUGUI totalCollectionPercentage;
        public Image illustProgress;
        JsonData illustData;

        public IllustElement[] illustElements;
        int totalIllust = -1;
        int openIllust = -1;

        [Header("Sound part")]
        public GameObject soundScroll;
        public SoundListElement[] soundListElements;

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_VIEW_NAME, true, string.Empty);

            #region 일러스트

            illustData = UserManager.main.GetNodeUserIllust();

            // 비활성화 해주면서 초기화
            foreach (IllustElement ie in illustElements)
                ie.gameObject.SetActive(false);

            openIllust = 0;
            totalIllust = illustData.Count;

            for(int i=0;i<totalIllust;i++)
            {
                if (SystemManager.GetJsonNodeBool(illustData[i], CommonConst.ILLUST_OPEN))
                    openIllust++;

                illustElements[i].InitElementInfo(illustData[i]);
            }

            totalCollection.text = string.Format("전체 수집률({0}/{1}", openIllust, totalIllust);
            float illustPrgressPercent = (float)openIllust / (float)totalIllust;
            totalCollectionPercentage.text = string.Format("{0}%", Mathf.Round(illustPrgressPercent * 100));
            illustProgress.fillAmount = illustPrgressPercent;

            OnDelayIllustOpen = DelayIllustOpen;

            #endregion

            #region 사운드

            soundListElements[0].SetBGMListElement();

            for (int i = 1; i < soundListElements.Length; i++)
                soundListElements[i].gameObject.SetActive(false);

            JsonData voiceData = UserManager.main.GetNodeUserVoiceHistory();
            JsonData voiceImageData = StoryManager.main.storyNametagJSON;
            int voiceIndex = 1;
            int voiceMasterIndex = 0;

            foreach(string key in voiceData.Keys)
            {
                for(int i=0;i<voiceImageData.Count;i++)
                {
                    if(SystemManager.GetJsonNodeString(voiceImageData[i], GameConst.COL_SPEAKER).Equals(key))
                    {
                        voiceMasterIndex = i;
                        break;
                    }
                }

                soundListElements[voiceIndex].SetVoiceElement(voiceImageData[voiceMasterIndex], voiceData[key]);
                voiceIndex++;
            }
            
            #endregion
        }


        public void EnableIllustList()
        {
            IllustScroll.SetActive(true);
            soundScroll.SetActive(false);
        }

        public void EnableSoundList()
        {
            IllustScroll.SetActive(false);
            soundScroll.SetActive(true);
        }

        void DelayIllustOpen(bool __interactable)
        {
            for(int i=0;i<illustData.Count;i++)
            {
                if (!illustElements[i].illustOpen)
                    continue;

                illustElements[i].illustButton.interactable = __interactable;
            }
                
        }
    }
}
