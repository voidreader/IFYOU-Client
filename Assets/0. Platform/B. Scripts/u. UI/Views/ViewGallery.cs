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
        [Header("Top tab")]
        public Image illustToggleBG;
        public Image soundToggleBG;
        public TextMeshProUGUI illustToggleText;
        public TextMeshProUGUI soundToggleText;

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
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5024"), string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);

            #region 일러스트

            illustData = UserManager.main.GetUserGalleryImage();

            // 비활성화 해주면서 초기화
            foreach (IllustElement ie in illustElements)
                ie.gameObject.SetActive(false);

            openIllust = 0;
            totalIllust = 0;
            int elementIndex = 0;

            for(int i=0;i<illustData.Count;i++)
            {
                if(!SystemManager.GetJsonNodeBool(illustData[i], "valid"))
                    continue;
                
                if (SystemManager.GetJsonNodeBool(illustData[i], CommonConst.ILLUST_OPEN))
                    openIllust++;
                    
                totalIllust++;

                illustElements[elementIndex++].InitElementInfo(illustData[i]);
            }

            totalCollection.text = string.Format(SystemManager.GetLocalizedText("6138"), openIllust, totalIllust);
            float illustPrgressPercent = 0;
            
            if(openIllust == 0 || totalIllust == 0) 
                illustPrgressPercent = 0;
            else 
                illustPrgressPercent = (float)openIllust / (float)totalIllust;
            
            
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


        public override void OnHideView() {
            base.OnHideView();
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, true, string.Empty);
        }


        public void EnableIllustList()
        {
            if (LobbyManager.main == null)
                return;

            illustToggleBG.sprite = LobbyManager.main.toggleSelected;
            soundToggleBG.sprite = LobbyManager.main.toggleUnselected;

            IllustScroll.SetActive(true);
            soundScroll.SetActive(false);
        }

        public void EnableSoundList()
        {
            illustToggleBG.sprite = LobbyManager.main.toggleUnselected;
            soundToggleBG.sprite = LobbyManager.main.toggleSelected;

            IllustScroll.SetActive(false);
            soundScroll.SetActive(true);
        }

        void DelayIllustOpen(bool __interactable)
        {
            
            for(int i=0; i<illustElements.Length; i++) {
                if(!illustElements[i].gameObject.activeSelf)
                    continue;
                    
                if (!illustElements[i].illustOpen)
                    continue;
                    
                illustElements[i].illustButton.interactable = __interactable;
            }
            
            /*
            for(int i=0;i<illustData.Count;i++)
            {
                
                if(!SystemManager.GetJsonNodeBool(illustData[i], "valid"))
                    continue;
                
                if (!illustElements[elementIndex].illustOpen)
                    continue;

                illustElements[i].illustButton.interactable = __interactable;
            }
            */
                
        }
    }
}
