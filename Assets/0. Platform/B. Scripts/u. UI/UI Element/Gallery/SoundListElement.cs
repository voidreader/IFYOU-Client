﻿using UnityEngine;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class SoundListElement : MonoBehaviour
    {
        public ImageRequireDownload soundThumbnail;

        public TextMeshProUGUI voiceInfo;

        string image_url = string.Empty;
        string image_key = string.Empty;

        const string BGM_BANNER = "bgmBanner";
        const string SHOW_SOUND_DETAIL = "showSoundDetail";

        public void SetBGMListElement()
        {
            image_url = SystemManager.GetJsonNodeString(SystemManager.GetJsonNode(UserManager.main.currentStoryJson, BGM_BANNER), CommonConst.COL_IMAGE_URL);
            image_key = SystemManager.GetJsonNodeString(SystemManager.GetJsonNode(UserManager.main.currentStoryJson, BGM_BANNER), CommonConst.COL_IMAGE_KEY);

            soundThumbnail.SetDownloadURL(image_url, image_key);
        }

        public void SetVoiceElement(JsonData __nameTag, JsonData __voiceData)
        {
            image_url = SystemManager.GetJsonNodeString(__nameTag, LobbyConst.BANNER_URL);
            image_key = SystemManager.GetJsonNodeString(__nameTag, LobbyConst.BANNER_KEY);
            
            soundThumbnail.SetDownloadURL(image_url, image_key);

            int unlockCount = 0, totalCount = 0;

            for(int i=0;i<__voiceData.Count;i++)
            {
                for(int j=0;j<__voiceData[i].Count;j++)
                {
                    if (SystemManager.GetJsonNodeBool(__voiceData[i][j], CommonConst.IS_OPEN))
                        unlockCount++;
                }
                totalCount += __voiceData[i].Count;
            }

            voiceInfo.text = string.Format("{0} 모아듣기\n<color=#A0A0A0FF>{1}개 / {2}개</color>", SystemManager.GetJsonNodeString(__nameTag, GameConst.COL_SPEAKER), unlockCount, totalCount);
            gameObject.SetActive(true);
        }

        public void ShowDetailSoundList()
        {
            Signal.Send(LobbyConst.STREAM_IFYOU, SHOW_SOUND_DETAIL, string.Empty);
        }
    }
}
