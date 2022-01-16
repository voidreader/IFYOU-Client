using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace PIERStory
{
    public class GameSoundCtrl : MonoBehaviour
    {
        Dictionary<string, ScriptSoundMount> DictSound = new Dictionary<string, ScriptSoundMount>();    // key값으로 audioclip 저장
                                                                                                        // SoundMount를 아예 째로 저장
        string soundKey = string.Empty;
        AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// 씬을 전환하지 않고 다음 에피소드로 진행시에는 필히 호출. 
        /// </summary>
        public void ClearDictionary()
        {
            DictSound.Clear();
        }


        public void InitSound(ScriptSoundMount soundMount, string __name)
        {
            if (DictSound.ContainsKey(__name))
                return;

            DictSound.Add(__name, soundMount);
        }

        public void PlayBGM(string keyName)
        {
            if (string.IsNullOrEmpty(keyName) || !DictSound.ContainsKey(keyName))
                return;

            audioSource.loop = true;
            soundKey = keyName;

            if (audioSource.isPlaying)
            {
                audioSource.DOFade(0f, 1.5f).OnComplete(() =>
                {
                    audioSource.Pause();
                    audioSource.clip = DictSound[keyName].audioClip;
                    audioSource.Play();
                    audioSource.DOFade(DictSound[keyName].volume, 1.5f).SetDelay(1f);
                });
            }
            else
            {
                audioSource.volume = 0f;
                audioSource.clip = DictSound[keyName].audioClip;
                audioSource.Play();
                audioSource.DOFade(DictSound[keyName].volume, 1.5f);
            }
        }

        public void PlayVoice(string keyName)
        {
            if (!DictSound.ContainsKey(keyName))
                return;

            ViewGame.main.ActiveMicrophoneIcon();

            // * 2021.09.28 sound_id 추가
            NetworkLoader.main.UpdateUserVoice(keyName, DictSound[keyName].sound_id);
            soundKey = keyName;

            audioSource.clip = DictSound[keyName].audioClip;
            audioSource.loop = false;
            audioSource.volume = DictSound[keyName].volume;
            audioSource.Play();
        }


        public void PlaySoundEffect(string keyName)
        {
            if (!DictSound.ContainsKey(keyName))
                return;

            soundKey = keyName;

            audioSource.clip = DictSound[keyName].audioClip;
            audioSource.loop = false;
            audioSource.volume = DictSound[keyName].volume;
            audioSource.PlayOneShot(audioSource.clip);

        }

        public void PauseAudioClip()
        {
            audioSource.Stop();
        }

        public void PauseBGM()
        {
            audioSource.DOFade(0f, 1.5f).OnComplete(() =>
            {
                audioSource.Stop();
            });
        }

        public void MuteAudioClip()
        {
            audioSource.mute = true;
        }

        public void UnmuteAudioClip()
        {
            audioSource.mute = false;
        }

        public bool GetIsPlaying { get { return audioSource.isPlaying; } }
    }
}