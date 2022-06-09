using UnityEngine;
using UnityEngine.UI;

using Doozy.Runtime.Reactor.Animators;

namespace PIERStory
{
    public class PopupGameOption : PopupBase
    {
        [Space(15)][Header("Alert Toggles")]
        public UIAnimator missionAnimator;
        public UIAnimator illustAnimator;

        public Image missionToggle;
        public Image illustToggle;

        RectTransform missionToggleHandle;
        RectTransform illustToggleHandle;

        public Sprite spriteToggleOn;
        public Sprite spriteToggleOff;

        [Header("Sound sliders")]
        public Slider voiceSlider;
        public Slider bgmSlider;
        public Slider seSlider;

        public Image voiceSoundIcon;
        public Image bgmSoundIcon;
        public Image seSoundIcon;

        public Sprite spriteSoundOn;
        public Sprite spriteSoundOff;

        [Header("AutoPlay Switches")]
        public GameObject slowToggle;
        public GameObject normalToggle;
        public GameObject fastToggle;

        public override void Show()
        {
            base.Show();

            missionToggleHandle = missionAnimator.GetComponent<RectTransform>();
            illustToggleHandle = illustAnimator.GetComponent<RectTransform>();

            AlertSetting();

            voiceSlider.value = PlayerPrefs.GetFloat(GameConst.VOICE_VOLUME);
            bgmSlider.value = PlayerPrefs.GetFloat(GameConst.BGM_VOLUME);
            seSlider.value = PlayerPrefs.GetFloat(GameConst.SOUNDEFFECT_VOLUME);

            AutoPlayerToggleInit();
        }


        public override void Hide()
        {
            base.Hide();

            PlayerPrefs.SetFloat(GameConst.VOICE_VOLUME, voiceSlider.value);
            PlayerPrefs.SetFloat(GameConst.BGM_VOLUME, bgmSlider.value);
            PlayerPrefs.SetFloat(GameConst.SOUNDEFFECT_VOLUME, seSlider.value);
        }


        #region OnButtonEvent

        public void OnClickMissionPopup()
        {
            if (PlayerPrefs.GetInt(GameConst.MISSION_POPUP) == 1)
            {
                PlayerPrefs.SetInt(GameConst.MISSION_POPUP, 0);
                missionToggle.sprite = spriteToggleOff;
                missionAnimator.Play(true);
            }
            else
            {
                PlayerPrefs.SetInt(GameConst.MISSION_POPUP, 1);
                missionToggle.sprite = spriteToggleOn;
                missionAnimator.Play();
            }
        }


        public void OnClickIllustPopup()
        {
            if (PlayerPrefs.GetInt(GameConst.ILLUST_POPUP) == 1)
            {
                PlayerPrefs.SetInt(GameConst.ILLUST_POPUP, 0);
                illustToggle.sprite = spriteToggleOff;
                illustAnimator.Play(true);
            }
            else
            {
                PlayerPrefs.SetInt(GameConst.ILLUST_POPUP, 1);
                illustToggle.sprite = spriteToggleOn;
                illustAnimator.Play();
            }
        }


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


        void AlertSetting()
        {
            // 미션 팝업 세팅
            if (!PlayerPrefs.HasKey(GameConst.MISSION_POPUP))
                PlayerPrefs.SetInt(GameConst.MISSION_POPUP, 1);

            if (PlayerPrefs.GetInt(GameConst.MISSION_POPUP) == 1)
            {
                missionToggle.sprite = spriteToggleOn;
                missionToggleHandle.anchoredPosition = new Vector2(11, -3);
            }
            else
            {
                missionToggle.sprite = spriteToggleOff;
                missionToggleHandle.anchoredPosition = new Vector2(-11, -3);
            }


            // 일러스트 팝업 세팅
            if (!PlayerPrefs.HasKey(GameConst.ILLUST_POPUP))
                PlayerPrefs.SetInt(GameConst.ILLUST_POPUP, 1);


            if (PlayerPrefs.GetInt(GameConst.ILLUST_POPUP) == 1)
            {
                illustToggle.sprite = spriteToggleOn;
                illustToggleHandle.anchoredPosition = new Vector2(11, -3);
            }
            else
            {
                illustToggle.sprite = spriteToggleOff;
                illustToggleHandle.anchoredPosition = new Vector2(-11, -3);
            }
        }


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