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

        [Header("AutoPlay Toggles")]
        public UIToggle slowToggle;
        public UIToggle normalToggle;
        public UIToggle fastToggle;

        public override void OnView()
        {
            base.OnView();
        }

        public override void OnStartView()
        {
            base.OnStartView();

            SetButtonSprite(voiceButton, GameConst.VOICE_MUTE);
            SetButtonSprite(bgmButton, GameConst.BGM_MUTE);
            SetButtonSprite(seButton, GameConst.SOUNDEFFECT_MUTE);


            AutoPlayerToggleInit();
        }

        /// <summary>
        /// 버튼 이미지 playerprefs에 저장된 값으로 sprite 변경
        /// </summary>
        /// <param name="button"></param>
        /// <param name="key"></param>
        void SetButtonSprite(Image button, string key)
        {
            // 0이면 mute가 false이므로 On
            if (PlayerPrefs.GetInt(key) < 1)
                button.sprite = soundOn;
            else
                button.sprite = soundOff;
        }


        #region OnClickEvent

        public void OnClickVoiceButton()
        {
            ChangeSoundSetting(GameConst.VOICE_MUTE, 1);
            SetButtonSprite(voiceButton, GameConst.VOICE_MUTE);
        }

        public void OnClickBGMButton()
        {
            ChangeSoundSetting(GameConst.BGM_MUTE, 0);
            SetButtonSprite(bgmButton, GameConst.BGM_MUTE);
        }

        public void OnClickSoundEffectButton()
        {
            ChangeSoundSetting(GameConst.SOUNDEFFECT_MUTE, 2);
            SetButtonSprite(seButton, GameConst.SOUNDEFFECT_MUTE);
        }

        // AutoPlayToggle 설정
        public void SetAutoPlaySlow()
        {
            if (slowToggle.isOn && GameManager.main != null)
                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.slowDelay);
        }

        public void SetAutoPlayNormal()
        {
            if (slowToggle.isOn && GameManager.main != null)
                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.normalDelay);
        }

        public void SetAutoPlayFast()
        {
            if (slowToggle.isOn && GameManager.main != null)
                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.fastDelay);
        }

        #endregion

        /// <summary>
        /// 사운드 관련된 셋팅을 바꿔준다
        /// </summary>
        /// <param name="key"></param>
        /// <param name="i">GameManager에 있는 soundgroup 배열의 index값</param>
        void ChangeSoundSetting(string key, int i)
        {
            // 0이면 false였으므로 true로 바꿔준다
            if (PlayerPrefs.GetInt(key) < 1)
            {
                GameManager.main.SoundGroup[i].MuteAudioClip();
                PlayerPrefs.SetInt(key, 1);
            }
            else
            {
                GameManager.main.SoundGroup[i].UnmuteAudioClip();
                PlayerPrefs.SetInt(key, 0);
            }
        }

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


