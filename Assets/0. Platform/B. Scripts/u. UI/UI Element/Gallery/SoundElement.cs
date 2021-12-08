using UnityEngine;
using UnityEngine.UI;

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
        public Image voiceButtonImage;
        public TextMeshProUGUI voiceScriptData;
        public GameObject lockIcon;
        public bool isOpen = false;


        public bool isPlaying = false;          // 재생 중인가?

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
            voiceScriptData.text = voiceScriptData.text.Replace('\\', ' ');
            soundUrl = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_URL);
            soundKey = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_KEY);
            isOpen = SystemManager.GetJsonNodeBool(__j, CommonConst.IS_OPEN);

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

            if(voiceScriptData == null)
            {
                int clipLength = (int)audioClip.length;
                string second = string.Empty;

                if (clipLength % 60 < 10)
                    second = string.Format("0{0}", clipLength % 60);
                else
                    second = string.Format("{0}", clipLength % 60);

                sonudPlaytime.text = string.Format("{0}:{1}", clipLength / 60, second);
            }
            else
            {
                if(isOpen)
                {
                    voiceButtonImage.sprite = LobbyManager.main.spriteOpenVoice;
                    voiceButtonImage.color = LobbyManager.main.colorFreeBox;
                    voiceScriptData.gameObject.SetActive(true);
                    lockIcon.SetActive(false);
                }
                else
                {
                    voiceButtonImage.sprite = LobbyManager.main.spriteLockVoice;
                    voiceButtonImage.color = Color.white;
                    voiceScriptData.gameObject.SetActive(false);
                    lockIcon.SetActive(true);
                }
            }
            
            gameObject.SetActive(true);
        }

        public void PlaySound()
        {
            // 보이스 세팅인데 열려있지 않으면 재생하지 않음
            if (voiceScriptData != null && !isOpen)
                return;

            ViewSoundDetail.PlaySound(audioClip);
            isPlaying = true;
            ViewSoundDetail.OnFindPlayIndex?.Invoke();

            if (voiceScriptData == null)
            {
                playIcon.SetActive(true);
                soundNumText.gameObject.SetActive(false);
            }
        }
    }
}
