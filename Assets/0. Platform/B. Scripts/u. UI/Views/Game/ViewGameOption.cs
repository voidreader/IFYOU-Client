using UnityEngine;
using UnityEngine.UI;


namespace PIERStory
{
    public class ViewGameOption : CommonView
    {
        [Header("Sound sliders")]
        public Slider voiceSlider;
        public Slider bgmSlider;
        public Slider seSlider;

        public Image voiceSoundIcon;
        public Image bgmSoundIcon;
        public Image seSoundIcon;

        public Sprite spriteSoundOn;
        public Sprite spriteSoundOff;

        [Header("AutoPlay Toggles")]
        public GameObject slowToggle;
        public GameObject normalToggle;
        public GameObject fastToggle;

        bool startHide = true;

        public override void OnStartView()
        {
            base.OnStartView();

            voiceSlider.value = PlayerPrefs.GetFloat(GameConst.VOICE_VOLUME);
            bgmSlider.value = PlayerPrefs.GetFloat(GameConst.BGM_VOLUME);
            seSlider.value = PlayerPrefs.GetFloat(GameConst.SOUNDEFFECT_VOLUME);

            AutoPlayerToggleInit();
        }

        public override void OnHideView()
        {
            base.OnHideView();

            if(startHide)
            {
                startHide = false;
                return;
            }

            PlayerPrefs.SetFloat(GameConst.VOICE_VOLUME, voiceSlider.value);
            PlayerPrefs.SetFloat(GameConst.BGM_VOLUME, bgmSlider.value);
            PlayerPrefs.SetFloat(GameConst.SOUNDEFFECT_VOLUME, seSlider.value);
        }


        #region OnButtonEvent


        public void OnChangedVoiceVolume()
        {
            if (voiceSlider.value == 0f)
                voiceSoundIcon.sprite = spriteSoundOff;
            else
                voiceSoundIcon.sprite = spriteSoundOn;

            GameManager.main.SoundGroup[1].ChangeSoundVolume(voiceSlider.value);
        }

        public void OnChangedBGMVolume()
        {
            if (bgmSlider.value == 0f)
                bgmSoundIcon.sprite = spriteSoundOff;
            else
                bgmSoundIcon.sprite = spriteSoundOn;

            GameManager.main.SoundGroup[0].ChangeSoundVolume(bgmSlider.value);
        }

        public void OnChangedSEVolume()
        {
            if (seSlider.value == 0f)
                seSoundIcon.sprite = spriteSoundOff;
            else
                seSoundIcon.sprite = spriteSoundOn;

            GameManager.main.SoundGroup[2].ChangeSoundVolume(seSlider.value);
        }


        // AutoPlayToggle 설정
        public void SetAutoPlaySlow()
        {
            if (GameManager.main != null)
            {
                slowToggle.SetActive(true);
                normalToggle.SetActive(false);
                fastToggle.SetActive(false);

                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.slowDelay);
            }
        }

        public void SetAutoPlayNormal()
        {
            if (GameManager.main != null)
            {
                slowToggle.SetActive(false);
                normalToggle.SetActive(true);
                fastToggle.SetActive(false);

                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.normalDelay);
            }
        }

        public void SetAutoPlayFast()
        {
            if (GameManager.main != null)
            {
                slowToggle.SetActive(false);
                normalToggle.SetActive(false);
                fastToggle.SetActive(true);

                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.fastDelay);
            }
        }

        #endregion


        void AutoPlayerToggleInit()
        {
            Debug.Log("AutoPlayerToggleInit");

            if(!PlayerPrefs.HasKey(GameConst.AUTO_PLAY))
                PlayerPrefs.SetFloat(GameConst.AUTO_PLAY, GameConst.normalDelay);

            switch (PlayerPrefs.GetFloat(GameConst.AUTO_PLAY))
            {
                case GameConst.slowDelay:
                    SetAutoPlaySlow();
                    break;
                case GameConst.normalDelay:
                    SetAutoPlayNormal();
                    break;
                case GameConst.fastDelay:
                    SetAutoPlayFast();
                    break;
            }
        }
    }
}