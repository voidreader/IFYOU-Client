using UnityEngine;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using System.Collections;

namespace PIERStory
{
    public class SoundListElement : MonoBehaviour
    {
        public ImageRequireDownload soundThumbnail;

        public TextMeshProUGUI voiceInfo;

        string image_url = string.Empty;
        string image_key = string.Empty;

        JsonData voiceData = null;
        string voiceMaster = string.Empty;
        int voiceTotalCount = 0;

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
            voiceData = __voiceData;
            image_url = SystemManager.GetJsonNodeString(__nameTag, LobbyConst.BANNER_URL);
            image_key = SystemManager.GetJsonNodeString(__nameTag, LobbyConst.BANNER_KEY);
            voiceMaster = SystemManager.GetJsonNodeString(__nameTag, GameConst.COL_SPEAKER);

            soundThumbnail.SetDownloadURL(image_url, image_key);

            int unlockCount = 0;
            voiceTotalCount = 0;

            for (int i = 0; i < __voiceData.Count; i++)
            {
                for (int j = 0; j < __voiceData[i].Count; j++)
                {
                    if (SystemManager.GetJsonNodeBool(__voiceData[i][j], CommonConst.IS_OPEN))
                        unlockCount++;
                }
                voiceTotalCount += __voiceData[i].Count;
            }

            voiceInfo.text = string.Format(SystemManager.GetLocalizedText("6058") + "\n<size=60>{1} / {2}</size>", StoryManager.main.GetNametagName(voiceMaster), unlockCount, voiceTotalCount);
            gameObject.SetActive(true);
        }

        public void ShowDetailSoundList()
        {
            SystemManager.ShowNetworkLoading();

            if (voiceData == null)
            {
                // 공개중인 BGM list가 없으면 무한로딩에 빠지므로 팝업 띄워주고 빠져나가게 하자
                if(UserManager.main.currentStoryJson["bgms"] == null || UserManager.main.currentStoryJson["bgms"].Count == 0)
                {
                    SystemManager.ShowMessageWithLocalize("80111");
                    SystemManager.HideNetworkLoading();
                    return;
                }

                ViewSoundDetail.SetSoundDetail(true, SystemManager.GetJsonNode(UserManager.main.currentStoryJson, "bgms"), soundThumbnail.downloadedSprite, SystemManager.GetLocalizedText("6139"));
            }
            else
                ViewSoundDetail.SetSoundDetail(false, voiceData, soundThumbnail.downloadedSprite, string.Format(SystemManager.GetLocalizedText("6058"), StoryManager.main.GetNametagName(voiceMaster)));

            StartCoroutine(MoveToSoundDetail());
        }

        IEnumerator MoveToSoundDetail()
        {
            ViewSoundDetail.OnSoundSetting?.Invoke(voiceTotalCount);
            yield return new WaitUntil(() => ViewSoundDetail.setComplete);

            Signal.Send(LobbyConst.STREAM_IFYOU, SHOW_SOUND_DETAIL, string.Empty);
            SystemManager.HideNetworkLoading();
        }
    }
}
