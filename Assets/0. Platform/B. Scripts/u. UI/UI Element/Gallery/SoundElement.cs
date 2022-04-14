using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class SoundElement : MonoBehaviour
    {
        AudioClip audioClip;
        JsonData soundData;
        public ScriptSoundMount soundMount;

        [Header("BGM require")]
        public GameObject playIcon;
        public TextMeshProUGUI soundNumText;
        public TextMeshProUGUI bgmTitle;
        public TextMeshProUGUI soundPlaytime;

        [Header("Voice require")]
        public Image voiceButtonBoarderImage;
        public TextMeshProUGUI voiceScriptData;
        public GameObject lockIcon;
        public GameObject playingIcon;
        public bool isOpen = false;

        int soundNumber = -1;
        bool isBgm = false;

        Color titleColor = new Color32(51, 51, 51, 255);
        Color timeColor = new Color32(153, 153, 153, 255);
        Color playColor = new Color32(255, 0, 128, 255);

        public void SetBGMElement(int index, JsonData __j)
        {
            soundData = __j;
            soundNumber = index;
            soundNumText.text = (index + 1).ToString();
            bgmTitle.text = SystemManager.GetJsonNodeString(__j, CommonConst.SOUND_NAME);
            isBgm = true;

            BGMStopMode();
            ClipSetting();
        }


        public void SetVoiceElement(int index, JsonData __j)
        {
            soundData = __j;
            soundNumber = index;
            voiceScriptData.text = SystemManager.GetJsonNodeString(__j, GameConst.COL_SCRIPT_DATA);
            voiceScriptData.text = voiceScriptData.text.Replace('\\', ' ');
            isOpen = SystemManager.GetJsonNodeBool(__j, CommonConst.IS_OPEN);
            isBgm = false;

            VoiceStopMode();
            ClipSetting();
        }

        void ClipSetting()
        {
            soundMount = null;

            if (isBgm)
                soundMount = new ScriptSoundMount(GameConst.TEMPLATE_BGM, soundData, SuccessLoad);
            else
                soundMount = new ScriptSoundMount(GameConst.COL_VOICE, soundData, SuccessLoad);

            gameObject.SetActive(true);
        }

        void SuccessLoad()
        {
            Debug.Log("Sound load complete. sound name = " + SystemManager.GetJsonNodeString(soundData, CommonConst.SOUND_NAME));
            ViewSoundDetail.OnSoundLoadCheck?.Invoke();
        }

        public void SetAudioClip()
        {
            if (!soundMount.isMounted)
            {
                Debug.LogError("Sound mount failed. sound name = " + SystemManager.GetJsonNodeString(soundData, CommonConst.SOUND_NAME));
                return;
            }

            audioClip = soundMount.audioClip;

            if (voiceScriptData == null)
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
                if (isOpen)
                {
                    voiceButtonBoarderImage.sprite = LobbyManager.main.spriteOpenVoice;
                    voiceScriptData.gameObject.SetActive(true);
                    lockIcon.SetActive(false);
                }
                else
                {
                    voiceButtonBoarderImage.sprite = LobbyManager.main.spriteLockVoice;
                    voiceScriptData.gameObject.SetActive(false);
                    lockIcon.SetActive(true);
                }
            }
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
