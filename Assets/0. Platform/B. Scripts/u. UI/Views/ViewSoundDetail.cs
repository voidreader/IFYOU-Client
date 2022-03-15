using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewSoundDetail : CommonView
    {
        public static Action<int> OnSoundSetting = null;
        public static Action OnSoundLoadCheck = null;
        public static Action<AudioClip, int> OnPlayBGM = null;
        public static Action<AudioClip, int> OnPlayVoice = null;
        public static Action<float> OnMovePlayTime = null;

        public static bool setComplete = false;

        static AudioSource playSound;

        static bool playBGM = true;
        
        static JsonData soundData = null;
        static Sprite titleSprite = null;
        static string soundTitle = string.Empty;

        public ScrollRect bgmScroll;
        public ScrollRect voiceScroll;

        public TextMeshProUGUI title;
        public Image titleImage;
        public Image gaugeBack;
        public Image playtimeBar;
        public Image playtimeBarHandle;

        public UIToggle shuffleToggle;
        public Image playButton;
        public Sprite spritePlay;
        public Sprite spritePause;

        [Header("BGM 재생목록 관련")]
        public GameObject bgmList;
        public SoundElement[] BGMElements;

        [Header("음성 재생목록 관련")]
        public GameObject voiceList;
        public Transform voiceContents;
        public Transform storage;
        public TextMeshProUGUI[] episodeInfo;
        public SoundElement[] voiceElements;
        List<SoundElement> currentVoiceList = new List<SoundElement>();

        [Space(30)]
        public Image repeatIcon;
        public Sprite repeatOn;
        public Sprite repeatOff;
        public GameObject onceText;
        enum RepeatState { NONE, ALL, ONCE};
        RepeatState repeatState = RepeatState.NONE;

        static int voiceTotalCount = 0;
        int checkSoundSetting = 0;
        int currentSoundIndex = 0;

        const float radius = 180f;          // 반지름
        const float circleAgree = 360f;     // 360도

        float agree = 0f;                   // 각도

        private void Awake()
        {
            OnSoundSetting = SoundDetailPageSetting;
            OnSoundLoadCheck = SoundSetCompleteCheck;
        }


        public static void SetSoundDetail(bool isBGM, JsonData __j, Sprite s, string __title)
        {
            playBGM = isBGM;
            soundData = __j;
            titleSprite = s;
            soundTitle = __title;
        }


        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);

            if (playSound == null)
                playSound = GetComponent<AudioSource>();

            title.text = soundTitle;
            titleImage.sprite = null;
            titleImage.sprite = titleSprite;

            OnPlayBGM = PlayBGM;
            OnPlayVoice = PlayVoice;
            OnMovePlayTime = MovePlayTime;

            playSound.Stop();
            playSound.clip = null;

            // 반복 설정
            repeatState = RepeatState.NONE;
            onceText.gameObject.SetActive(false);
            repeatIcon.sprite = repeatOff;

            shuffleToggle.isOn = false;
            playtimeBar.fillAmount = 0f;
            playButton.sprite = spritePlay;
        }

        public override void OnView()
        {
            base.OnView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
        }


        public override void OnHideView()
        {
            base.OnHideView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);

            if (playSound != null)
                playSound.Stop();


            if(playBGM)
            {
                for (int i = 0; i < BGMElements.Length; i++)
                    BGMElements[i].gameObject.SetActive(false);
            }
            else
            {
                foreach (TextMeshProUGUI info in episodeInfo)
                    info.transform.SetParent(storage);

                foreach (SoundElement voice in currentVoiceList)
                    voice.transform.SetParent(storage);

                currentVoiceList.Clear();
            }

            setComplete = false;
        }

        #region Action Function


        void SoundDetailPageSetting(int __voiceTotal = -1)
        {
            bgmList.SetActive(false);
            voiceList.SetActive(false);

            checkSoundSetting = 0;
            voiceTotalCount = __voiceTotal;

            // 배경음을 재생하는지, 보이스를 재생하는지에 따라 이후 세팅이 다름
            if (playBGM)
            {
                for (int i = 0; i < soundData.Count; i++)
                    BGMElements[i].SetBGMElement(i, soundData[i]);

                bgmList.SetActive(true);
                bgmScroll.verticalNormalizedPosition = 1f;
            }
            else
            {
                int textIndex = 0, voiceIndex = 0;

                // 보이스 세팅
                foreach (string key in soundData.Keys)
                {
                    // 에피소드 넘버링, 제목 세팅
                    for (int i = 0; i < StoryManager.main.ListCurrentProjectEpisodes.Count; i++)
                    {
                        // 일치하는 에피소드 아이디를 찾았다!
                        if (StoryManager.main.ListCurrentProjectEpisodes[i].episodeID.Equals(SystemManager.GetJsonNodeString(soundData[key][0], LobbyConst.STORY_EPISODE_ID)))
                        {
                            episodeInfo[textIndex].text = StoryManager.main.ListCurrentProjectEpisodes[i].combinedEpisodeTitle;
                            episodeInfo[textIndex].transform.SetParent(voiceContents);
                            textIndex++;
                            break;
                        }
                    }

                    // 보이스 세팅
                    for (int i = 0; i < soundData[key].Count; i++)
                    {
                        voiceElements[voiceIndex].SetVoiceElement(voiceIndex, soundData[key][i]);
                        voiceElements[voiceIndex].transform.SetParent(voiceContents);
                        currentVoiceList.Add(voiceElements[voiceIndex]);
                        voiceIndex++;
                    }
                }

                voiceList.SetActive(true);
                voiceScroll.verticalNormalizedPosition = 1f;
            }
        }

        void SoundSetCompleteCheck()
        {
            checkSoundSetting++;

            if(playBGM)
            {
                if (soundData.Count == checkSoundSetting)
                    setComplete = true;
            }
            else
            {
                if(voiceTotalCount > 0 && voiceTotalCount == checkSoundSetting)
                    setComplete = true;
            }
        }


        void PlayBGM(AudioClip clip, int playIndex)
        {
            // 선택한 BGM만 선택된 표시를 해준다
            for (int i = 0; i < soundData.Count; i++)
            {
                if (i == playIndex)
                    BGMElements[i].BGMPlayMode();
                else
                    BGMElements[i].BGMStopMode();
            }

            PlaySound(clip, playIndex);
        }


        void PlayVoice(AudioClip clip, int playIndex)
        {
            // 선택된 Voice만 선택된 표시를 해준다
            for (int i = 0; i < currentVoiceList.Count; i++)
            {
                if (i == playIndex)
                    currentVoiceList[i].VoicePlayMode();
                else
                    currentVoiceList[i].VoiceStopMode();
            }

            PlaySound(clip, playIndex);
        }


        void PlaySound(AudioClip clip, int playIndex)
        {
            currentSoundIndex = playIndex;
            playSound.clip = clip;
            playSound.time = 0f;

            playSound.Play();
            playButton.sprite = spritePause;
            StartCoroutine(RoutinePlaySound());
        }


        IEnumerator RoutinePlaySound()
        {
            if (playSound.clip == null)
                yield break;


            while (playSound.isPlaying || playSound.time <= playSound.clip.length)
            {
                playtimeBar.fillAmount = playSound.time / playSound.clip.length;
                agree = 270f - playtimeBar.fillAmount * circleAgree;
                playtimeBarHandle.rectTransform.anchoredPosition = new Vector2(Mathf.Cos(agree * Mathf.Deg2Rad) * radius, Mathf.Sin(agree * Mathf.Deg2Rad) * radius);
                yield return null;

                if (!playSound.isPlaying && playSound.time == 0f)
                    break;
            }

            //Debug.Log(string.Format("playSound.time == playSound.clip.length? {0}\nplaySound.time = {1}\nplaySound.clip.length = {2}", (playSound.time == playSound.clip.length), playSound.time, playSound.clip.length));

            playtimeBar.fillAmount = 0f;
            playtimeBarHandle.rectTransform.anchoredPosition = new Vector2(0, -radius);
            playButton.sprite = spritePlay;

            // 랜덤 재생이지만 한곡 반복은 아닌지 먼저 체크하고
            if (shuffleToggle.isOn && repeatState != RepeatState.ONCE)
            {
                RandomPlay();

                // 랜덤 재생이면 코루틴을 빠져나온다
                yield break;
            }
            

            // 스트레이트 한번인지, 전체 뺑뺑이인지, 한곡만 주구장창인지 체크
            switch (repeatState)
            {
                case RepeatState.NONE:
                    
                    if(playBGM)
                    {
                        if(currentSoundIndex + 1 < BGMElements.Length)
                        {
                            BGMElements[currentSoundIndex + 1].PlaySound();
                            yield break;
                        }
                        else
                        {
                            for (int i = 0; i < BGMElements.Length; i++)
                                BGMElements[i].BGMStopMode();
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            // 일단 냅다 증가하고
                            currentSoundIndex++;

                            // currentSoundIndex가 총량 이상인 경우 break
                            if (currentSoundIndex >= currentVoiceList.Count)
                            {
                                for (int i = 0; i < currentVoiceList.Count; i++)
                                    currentVoiceList[i].VoiceStopMode();

                                break;
                            }

                            // 다음 index가 해금되어 있는 것이라면 재생!
                            if (currentVoiceList[currentSoundIndex].isOpen)
                            {
                                currentVoiceList[currentSoundIndex].PlaySound();
                                yield break;
                            }
                        }
                    }

                    playSound.clip = null;


                    break;

                case RepeatState.ALL:
                    OnClickPlayNext();

                    break;

                case RepeatState.ONCE:

                    if(playBGM)
                        BGMElements[currentSoundIndex].PlaySound();
                    else
                        currentVoiceList[currentSoundIndex].PlaySound();

                    break;
            }
        }

        int RandomSoundIndex()
        {
            int index = -1;

            // BGM 리스트인지?
            if(playBGM)
            {
                while(true)
                {
                    index = UnityEngine.Random.Range(0, soundData.Count);

                    // 랜덤 값이 현재 재생값과 달라야 빠져나올 수 있다
                    if (index != currentSoundIndex)
                        return index;
                }
            }
            else
            {
                while(true)
                {
                    index = UnityEngine.Random.Range(0, currentVoiceList.Count);

                    // 랜덤 값이 현재 재생값과 다르고, 해금한 voice여야 빠져나온다
                    if (index != currentSoundIndex && currentVoiceList[index].isOpen)
                        return index;
                }
            }
        }

        void MovePlayTime(float angle)
        {
            if (playSound.clip == null)
                return;

            playtimeBar.fillAmount = angle / circleAgree;
            playtimeBarHandle.rectTransform.anchoredPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
            playSound.time = playtimeBar.fillAmount * playSound.clip.length;
        }

        #endregion

        /// <summary>
        /// 무작위 재생
        /// </summary>
        void RandomPlay()
        {
            if (playBGM)
                BGMElements[RandomSoundIndex()].PlaySound();
            else
                currentVoiceList[RandomSoundIndex()].PlaySound();
        }

        /// <summary>
        /// 이전곡 재생
        /// </summary>
        public void OnClickPlayPrev()
        {
            if (playSound.clip == null)
            {
                SystemManager.ShowSimpleAlertLocalize("6169");
                return;
            }

            // 랜덤 재생을 사용중이라면?
            if (shuffleToggle.isOn)
                RandomPlay();
            else
            {

                if(playBGM)
                {
                    if (currentSoundIndex - 1 < 0)
                        currentSoundIndex = soundData.Count - 1;
                    else
                        currentSoundIndex--;

                    BGMElements[currentSoundIndex].PlaySound();
                }
                else
                {
                    while(true)
                    {
                        currentSoundIndex--;

                        if (currentSoundIndex - 1 < 0)
                            currentSoundIndex = currentVoiceList.Count - 1;

                        if(currentVoiceList[currentSoundIndex].isOpen)
                        {
                            currentVoiceList[currentSoundIndex].PlaySound();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 다음 곡 재생
        /// </summary>
        public void OnClickPlayNext()
        {
            if (playSound.clip == null)
            {
                SystemManager.ShowSimpleAlertLocalize("6169");
                return;
            }

            if (shuffleToggle.isOn)
                RandomPlay();
            else
            {
                if (playBGM)
                {
                    if (currentSoundIndex + 1 < soundData.Count)
                        BGMElements[currentSoundIndex + 1].PlaySound();
                    else
                        BGMElements[0].PlaySound();
                }
                else
                {
                    while (true)
                    {
                        currentSoundIndex++;

                        // currentSoundIndex가 총량 이상인 경우 0으로 만든다
                        if (currentSoundIndex >= currentVoiceList.Count)
                            currentSoundIndex = 0;

                        if (currentVoiceList[currentSoundIndex].isOpen)
                        {
                            currentVoiceList[currentSoundIndex].PlaySound();
                            break;
                        }
                    }
                }
            }
        }


        public void OnClickPlay()
        {
            // audioSource에 clip이 넣어진게 없으면 아무것도 하지 않는다
            if (playSound.clip == null)
            {
                SystemManager.ShowSimpleAlertLocalize("6169");
                return;
            }


            if (playSound.isPlaying)
            {
                playSound.Pause();
                playButton.sprite = spritePlay;
            }
            else
            {
                playSound.Play();
                playButton.sprite = spritePause;
                StartCoroutine(RoutinePlaySound());
            }
        }


        public void OnClickRepeatSetting()
        {
            switch (repeatState)
            {
                case RepeatState.NONE:
                    repeatState = RepeatState.ALL;
                    repeatIcon.sprite = repeatOn;
                    break;
                case RepeatState.ALL:
                    repeatState = RepeatState.ONCE;
                    onceText.gameObject.SetActive(true);
                    break;
                case RepeatState.ONCE:
                    repeatState = RepeatState.NONE;
                    onceText.gameObject.SetActive(false);
                    repeatIcon.sprite = repeatOff;
                    break;
            }
        }
    }
}
