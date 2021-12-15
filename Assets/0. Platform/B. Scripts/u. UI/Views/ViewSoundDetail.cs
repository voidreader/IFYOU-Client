using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewSoundDetail : CommonView
    {
        static Action OnPlaySound = null;
        public static Action OnFindPlayIndex = null;

        static AudioSource playSound;
        static AudioClip audioClip;

        static bool playBGM = true;
        static JsonData soundData = null;
        static Sprite titleSprite = null;
        static string soundTitle = string.Empty;

        public TextMeshProUGUI title;
        public Image titleImage;
        public Image gaugeBack;
        public Image playtimeBar;
        public Image playtimeBarHandle;

        public UIToggle shuffleToggle;
        public UIToggle playToggle;

        [Header("BGM 재생목록 관련")]
        public GameObject bgmList;
        public SoundElement[] BGMElements;

        [Header("음성 재생목록 관련")]
        public GameObject voiceList;
        public Transform voiceContents;
        public Transform storage;
        public TextMeshProUGUI[] episodeInfo;
        public SoundElement[] voiceElements;


        [Space(30)]
        public Image repeatIcon;
        public Sprite repeatOn;
        public Sprite repeatOff;
        public GameObject onceText;
        enum RepeatState { NONE, ALL, ONCE};
        RepeatState repeatState = RepeatState.NONE;

        [Space(20)]
        public Transform circleCenter;
        enum DragState { DRAGGING, NOT_DRAGGING};
        DragState drag = DragState.NOT_DRAGGING;

        const float radius = 180f;          // 반지름
        const float circleAgree = 360f;     // 360도

        float agree = 0f;                   // 각도
        int currentValue = 0;
        int activeTotalCount = 0;           // 활성화 총량
        int playIndex = 0;                  // 현재 재생중인 element의 index값

        public static void SetSoundDetail(bool isBGM, JsonData __j, Sprite s, string __title)
        {
            playBGM = isBGM;
            soundData = __j;
            titleSprite = s;
            soundTitle = __title;
        }

        public static void PlaySound(AudioClip clip)
        {
            OnPlaySound?.Invoke();
            playSound.clip = clip;
            audioClip = clip;
            playSound.time = 0f;
            playSound.Play();
        }


        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_VIEW_NAME, false, string.Empty);

            if (playSound == null)
                playSound = GetComponent<AudioSource>();

            bgmList.SetActive(false);
            voiceList.SetActive(false);
            
            activeTotalCount = 0;
            title.text = soundTitle;
            titleImage.sprite = titleSprite;

            // 배경음을 재생하는지, 보이스를 재생하는지에 따라 이후 세팅이 다름
            if(playBGM)
            {
                for (int i = 0; i < BGMElements.Length; i++)
                    BGMElements[i].gameObject.SetActive(false);

                for (int i = 0; i < soundData.Count; i++)
                {
                    BGMElements[i].SetBGMElement(i + 1, soundData[i]);
                    activeTotalCount++;
                }

                bgmList.SetActive(true);
            }
            else
            {
                // 초기화
                foreach (TextMeshProUGUI info in episodeInfo)
                    info.transform.SetParent(storage);

                foreach (SoundElement voice in voiceElements)
                    voice.transform.SetParent(storage);

                int textIndex = 0, voiceIndex = 0;

                // 보이스 세팅
                foreach(string key in soundData.Keys)
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
                        voiceElements[voiceIndex].SetVoiceElement(soundData[key][i]);
                        voiceElements[voiceIndex].transform.SetParent(voiceContents);

                        // 해금했는지 확인되어야 활성화 총량 카운트를 증가시킨다
                        if(SystemManager.GetJsonNodeBool(soundData[key][i], CommonConst.IS_OPEN))
                            activeTotalCount++;

                        voiceIndex++;
                    }
                }

                voiceList.SetActive(true);
            }

            OnPlaySound = SetBGMPlayIcon;
            OnFindPlayIndex = FindPlayIndex;

            // 반복 설정
            repeatState = RepeatState.NONE;
            onceText.gameObject.SetActive(false);
            repeatIcon.sprite = repeatOff;

            shuffleToggle.isOn = false;
            playtimeBar.fillAmount = 0f;
        }

        public override void OnHideView()
        {
            base.OnHideView();

            if (playSound !=null && playSound.isPlaying)
            {
                playSound.Stop();
                playSound.clip = null;
            }

        }

        #region Action Function

        void SetBGMPlayIcon()
        {
            if(playBGM)
            {
                foreach (SoundElement se in BGMElements)
                {
                    se.playIcon.SetActive(false);
                    se.soundNumText.gameObject.SetActive(true);
                    se.isPlaying = false;
                }
            }
            
            playToggle.isOn = true;
        }

        void FindPlayIndex()
        {
            // BGM 리스트 보는중
            if (playBGM)
            {
                for (int i = 0; i < activeTotalCount; i++)
                {
                    // 플레이 중인 index 찾기
                    if (BGMElements[i].isPlaying)
                    {
                        playIndex = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < activeTotalCount; i++)
                {
                    if (voiceElements[i].isPlaying)
                    {
                        playIndex = i;
                        break;
                    }
                }
            }
        }

        #endregion

        private void Update()
        {
            if (playSound == null || !playSound.isPlaying)
                return;

            // 재생 중~
            if (playSound.isPlaying)
            {
                playtimeBar.fillAmount = playSound.time / audioClip.length;
                agree = 270f - playtimeBar.fillAmount * circleAgree;
                playtimeBarHandle.rectTransform.anchoredPosition = new Vector2(Mathf.Cos(agree * Mathf.Deg2Rad) * radius, Mathf.Sin(agree * Mathf.Deg2Rad) * radius);
            }

            // 재생을 끝까지 다 한 경우
            if (playSound.time == audioClip.length)
            {
                playSound.time = 0;

                // 한곡 반복인 경우
                if (repeatState == RepeatState.ONCE)
                {
                    playSound.time = 0f;
                    playSound.Play();
                    return;
                }

                // 랜덤 재생을 사용하는 경우
                if (shuffleToggle.isOn)
                {
                    RandomPlay();
                }
                else
                {
                    switch (repeatState)
                    {
                        case RepeatState.NONE:
                            if (playIndex + 1 < activeTotalCount)
                            {
                                playIndex++;

                                if (playBGM)
                                    BGMElements[playIndex].PlaySound();
                                else
                                    voiceElements[playIndex].PlaySound();
                            }
                            else
                            {
                                if (playBGM)
                                {
                                    BGMElements[playIndex].playIcon.SetActive(false);
                                    BGMElements[playIndex].soundNumText.gameObject.SetActive(true);
                                    BGMElements[playIndex].isPlaying = false;
                                }

                                playToggle.isOn = false;
                                return;
                            }

                            break;
                        case RepeatState.ALL:

                            if (playIndex + 1 < activeTotalCount)
                                playIndex++;
                            else
                                playIndex = 0;

                            if (playBGM)
                                BGMElements[playIndex].PlaySound();
                            else
                                voiceElements[playIndex].PlaySound();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 이전곡 재생
        /// </summary>
        public void OnClickPlayPrev()
        {
            // 랜덤 재생을 사용중이라면?
            if (shuffleToggle.isOn)
                RandomPlay();
            else
            {
                if (playIndex - 1 < 0)
                    playIndex = activeTotalCount - 1;
                else
                    playIndex -= 1;

                if (playBGM)
                    BGMElements[playIndex].PlaySound();
                else
                    voiceElements[playIndex].PlaySound();
            }
        }

        /// <summary>
        /// 다음 곡 재생
        /// </summary>
        public void OnClickPlayNext()
        {
            if (shuffleToggle.isOn)
                RandomPlay();
            else
            {
                if (playIndex + 1 == activeTotalCount)
                    playIndex = 0;
                else
                    playIndex += 1;

                if (playBGM)
                    BGMElements[playIndex].PlaySound();
                else
                    voiceElements[playIndex].PlaySound();
            }
        }

        void RandomPlay()
        {
            int index = UnityEngine.Random.Range(0, activeTotalCount);

            // 랜덤값 중복 체크
            while (true)
            {
                // 같은 값이 아니어야 이곳을 빠져나감
                if (index != playIndex)
                    break;
                else
                    index = UnityEngine.Random.Range(0, activeTotalCount);
            }

            playIndex = index;

            if (playBGM)
                BGMElements[index].PlaySound();
            else
                voiceElements[index].PlaySound();
        }


        public void OnClickPlay()
        {
            if (playSound.isPlaying)
            {
                playSound.Pause();
                playToggle.isOn = false;
            }
            else
            {
                playSound.Play();
                playToggle.isOn = true;
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

        public void DragCircleArea(bool isClick)
        {
            if(isClick && drag == DragState.DRAGGING)
            {
                drag = DragState.NOT_DRAGGING;
                return;
            }

            if(isClick)
                drag = DragState.NOT_DRAGGING;
            else
                drag = DragState.DRAGGING;

            // 기준 벡터
            Vector3 standard = new Vector3(circleCenter.position.x, circleCenter.position.y -180, circleCenter.position.z) - circleCenter.position;
            Vector3 changed = Input.mousePosition - circleCenter.position;      // 기준점으로부터 위치 벡터

            float f = Application.isEditor? Vector3.Angle(
                changed, standard) : Vector3.Angle(Vector3.down, new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0) - circleCenter.position);
            bool onTheRight = Application.isEditor ? Input.mousePosition.x > circleCenter.position.x : Input.GetTouch(0).position.x > circleCenter.position.x;

            Debug.Log(string.Format("Angle = {0}, standard_Vector = {1}, changed_Vector = {2}", f, standard, changed));

            int detectedValue = onTheRight ? (int)f : 180 + (180 - (int)f);

            if (detectedValue > 350)
                detectedValue = 360;
            else if (currentValue == 360 && detectedValue < 10)
                detectedValue = 360;
            else if (currentValue == 0 && detectedValue > 350)
                detectedValue = 0;
            else if (detectedValue < 10)
                detectedValue = 0;

            currentValue = detectedValue;
            playtimeBar.fillAmount = currentValue / circleAgree;
            agree = 270f - playtimeBar.fillAmount * circleAgree;
            playtimeBarHandle.rectTransform.anchoredPosition = new Vector2(Mathf.Cos(agree * Mathf.Deg2Rad) * radius, Mathf.Sin(agree * Mathf.Deg2Rad) * radius);

            if(playSound.clip != null)
                playSound.time = playtimeBar.fillAmount * playSound.clip.length;
        }
    }
}
