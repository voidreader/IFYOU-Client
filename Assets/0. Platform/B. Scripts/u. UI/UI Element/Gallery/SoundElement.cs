using UnityEngine;

using TMPro;
using LitJson;
using BestHTTP;

namespace PIERStory
{
    public class SoundElement : MonoBehaviour
    {
        AudioClip audioClip;
        string soundUrl = string.Empty;
        string soundKey = string.Empty;

        [Header("BGM require")]
        public GameObject playIcon;
        public TextMeshProUGUI soundNumText;
        public TextMeshProUGUI bgmTitle;
        public TextMeshProUGUI sonudPlaytime;

        [Header("Voice require")]
        public TextMeshProUGUI voiceScriptData;


        public void SetBGMElement(int index, JsonData __j)
        {
            soundNumText.text = index.ToString();
            bgmTitle.text = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_NAME);
            soundUrl = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_URL);
            soundKey = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_KEY);

            ClipSetting();
        }


        public void SetVoiceElement(JsonData __j)
        {
            voiceScriptData.text = SystemManager.GetJsonNodeString(__j, GameConst.COL_SCRIPT_DATA);
            soundUrl = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_URL);
            soundKey = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_KEY);

            ClipSetting();
        }

        void ClipSetting()
        {
            if (ES3.FileExists(soundKey))
                SuccessLoad();
            else
            {
                var req = new HTTPRequest(new System.Uri(soundUrl), OnSoundDownloaded);
                req.Send();
            }
        }

        void OnSoundDownloaded(HTTPRequest req, HTTPResponse res)
        {
            if (req.State != HTTPRequestStates.Finished)
            {
                Debug.LogError("Download failed : " + soundUrl);
                return;
            }

            ES3.SaveRaw(res.Data, soundKey);
            SuccessLoad();
        }

        void SuccessLoad()
        {
            if (soundKey.Contains("mp3"))
                audioClip = ES3.LoadAudio(soundKey, AudioType.MPEG);
            else if (soundKey.Contains("wav"))
                audioClip = ES3.LoadAudio(soundKey, AudioType.WAV);

            int clipLength = (int)audioClip.length;
            string second = string.Empty;

            if (clipLength % 60 < 10)
                second = string.Format("0{0}", clipLength % 60);
            else
                second = string.Format("{0}", clipLength % 60);

            sonudPlaytime.text = string.Format("{0}:{1}", clipLength / 60, second);
        }
    }
}
