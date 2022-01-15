using UnityEngine;
using UnityEngine.UI;

using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewGameOption : CommonView
    {
        public Sprite soundOn;
        public Sprite soundOff;

        [Header("Sound buttons")]
        public Image voiceButton;
        public Image bgmButton;
        public Image seButton;

        public UIToggle voiceToggle;
        public UIToggle bgmToggle;
        public UIToggle seToggle;

        [Header("AutoPlay Toggles")]
        public UIToggle slowToggle;
        public UIToggle normalToggle;
        public UIToggle fastToggle;

        bool currentShow = false;

        public override void OnStartView()
        {
            base.OnStartView();

            currentShow = true;

            voiceToggle.isOn = PlayerPrefs.GetInt(GameConst.VOICE_MUTE) > 0 ? false : true;
            bgmToggle.isOn = PlayerPrefs.GetInt(GameConst.BGM_MUTE) > 0 ? false : true;
            seToggle.isOn = PlayerPrefs.GetInt(GameConst.SOUNDEFFECT_MUTE) > 0 ? false : true;

            AutoPlayerToggleInit();
        }


        #region OnToggleEvent


        public void VoiceToggleOn()
        {
            if (!currentShow)
                return;

            ToggleEvent(voiceButton, GameConst.VOICE_MUTE, 1, true);
        }

        public void VoiceToggleOff()
        {
            if (!currentShow)
                return;

            ToggleEvent(voiceButton, GameConst.VOICE_MUTE, 1, false);
        }

        public void BGMToggleOn()
        {
            if (!currentShow)
                return;

            ToggleEvent(bgmButton, GameConst.BGM_MUTE, 0, true);
        }

        public void BGMToggleOff()
        {
            if (!currentShow)
                return;

            ToggleEvent(bgmButton, GameConst.BGM_MUTE, 0, false);
        }

        public void SoundEffectToggleOn()
        {
            if (!currentShow)
                return;

            ToggleEvent(seButton, GameConst.SOUNDEFFECT_MUTE, 2, true);
        }

        public void SoundEffectToggleOff()
        {
            if (!currentShow)
                return;

            ToggleEvent(seButton, GameConst.SOUNDEFFECT_MUTE, 2, false);
        }


        /// <summary>
        /// 사운드 관련 토글에 대한 이벤트 function
        /// </summary>
        void ToggleEvent(Image button, string key, int i, bool isOn)
        {
            if(isOn)
            {
                button.sprite = soundOn;
                GameManager.main.SoundGroup[i].UnmuteAudioClip();
                PlayerPrefs.SetInt(key, 0);
            }
            else
            {
                button.sprite = soundOff;
                GameManager.main.SoundGroup[i].MuteAudioClip();
                PlayerPrefs.SetInt(key, 1);
            }
        }


        // AutoPlayToggle 설정
        public void SetAutoPlaySlow()
        {
            if (!currentShow)
                return;

            if (slowToggle.isOn && GameManager.main != null)
                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.slowDelay);
        }

        public void SetAutoPlayNormal()
        {
            if (!currentShow)
                return;

            if (normalToggle.isOn && GameManager.main != null)
                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.normalDelay);
        }

        public void SetAutoPlayFast()
        {
            if (!currentShow)
                return;

            if (fastToggle.isOn && GameManager.main != null)
                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.fastDelay);
        }

        #endregion


        void AutoPlayerToggleInit()
        {
            Debug.Log("AutoPlayerToggleInit");

            if(!PlayerPrefs.HasKey(GameConst.AUTO_PLAY))
            {
                SetAutoPlayNormal();
                normalToggle.isOn = true;
            }
            else
            {
                switch (PlayerPrefs.GetFloat(GameConst.AUTO_PLAY))
                {
                    case GameConst.slowDelay:
                        slowToggle.isOn = true;
                        break;
                    case GameConst.normalDelay:
                        normalToggle.isOn = true;
                        break;
                    case GameConst.fastDelay:
                        fastToggle.isOn = true;
                        break;
                }
            }
        }
    }
}