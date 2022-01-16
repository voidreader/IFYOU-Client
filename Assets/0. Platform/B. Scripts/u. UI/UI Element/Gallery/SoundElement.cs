using System;
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
        public TextMeshProUGUI soundPlaytime;

        [Header("Voice require")]
        public Image voiceButtonImage;
        public TextMeshProUGUI voiceScriptData;
        public GameObject lockIcon;
        public GameObject playingIcon;
        public bool isOpen = false;

        int soundNumber = -1;

        Color titleColor = new Color32(51, 51, 51, 255);
        Color timeColor = new Color32(153, 153, 153, 255);
        Color playColor = new Color32(243, 140, 161, 255);

        public void SetBGMElement(int index, JsonData __j)
        {
            soundNumber = index;
            soundNumText.text = (index + 1).ToString();
            bgmTitle.text = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_NAME);
            soundUrl = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_URL);
            soundKey = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_KEY);

            BGMStopMode();
            ClipSetting();
        }


        public void SetVoiceElement(int index, JsonData __j)
        {
            soundNumber = index;
            voiceScriptData.text = SystemManager.GetJsonNodeString(__j, GameConst.COL_SCRIPT_DATA);
            voiceScriptData.text = voiceScriptData.text.Replace('\\', ' ');
            soundUrl = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_URL);
            soundKey = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_KEY);
            isOpen = SystemManager.GetJsonNodeBool(__j, CommonConst.IS_OPEN);

            
            VoiceStopMode();
            ClipSetting();
        }

        void ClipSetting()
        {
            if (ES3.FileExists(soundKey))
                SuccessLoad();
            else
            {
                var req = new HTTPRequest(new Uri(soundUrl), OnSoundDownloaded);
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

            ES3.SaveRaw(res.Data, soundKey, SystemManager.noEncryptionSetting);
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

                soundPlaytime.text = string.Format("{0}:{1}", clipLength / 60, second);
            }
            else
            {
                if(isOpen)
                {
                    voiceButtonImage.sprite = LobbyManager.main.spriteOpenVoice;
                    voiceScriptData.gameObject.SetActive(true);
                    lockIcon.SetActive(false);
                }
                else
                {
                    voiceButtonImage.sprite = LobbyManager.main.spriteLockVoice;
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


            if (voiceScriptData == null)
                ViewSoundDetail.OnPlayBGM?.Invoke(audioClip, soundNumber);
            else
                ViewSoundDetail.OnPlayVoice?.Invoke(audioClip, soundNumber);
        }

        public void BGMPlayMode()
        {
            playIcon.SetActive(true);
            soundNumText.gameObject.SetActive(false);
            bgmTitle.color = playColor;
            soundPlaytime.color = playColor;
        }

        public void BGMStopMode()
        {
            playIcon.SetActive(false);
            soundNumText.gameObject.SetActive(true);
            bgmTitle.color = titleColor;
            soundPlaytime.color = timeColor;
        }

        public void VoicePlayMode()
        {
            playingIcon.SetActive(true);
        }

        public void VoiceStopMode()
        {
            playingIcon.SetActive(false);
        }
    }
}
