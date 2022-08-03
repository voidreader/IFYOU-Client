using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
                    audioSource.DOFade(PlayerPrefs.GetFloat(GameConst.BGM_VOLUME), 1.5f).SetDelay(1f);
                });
            }
            else
            {
                audioSource.volume = 0f;
                audioSource.clip = DictSound[keyName].audioClip;
                audioSource.Play();
                audioSource.DOFade(PlayerPrefs.GetFloat(GameConst.BGM_VOLUME), 1.5f);
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
            audioSource.Play();
        }


        public void PlaySoundEffect(string keyName)
        {
            if (!DictSound.ContainsKey(keyName))
                return;

            soundKey = keyName;

            audioSource.clip = DictSound[keyName].audioClip;
            audioSource.loop = false;
            audioSource.PlayOneShot(audioSource.clip);
        }


        /// <summary>
        /// 클립 일시 정지
        /// </summary>
        public void PlayAudioClip(bool __play)
        {
            if (__play)
            {
                if (audioSource.clip != null)
                    audioSource.Play();
            }
            else
                audioSource.Pause();
        }


        /// <summary>
        /// 클립 정지
        /// </summary>
        public void StopAudioClip()
        {
            if (audioSource.clip == null)
                return;

            audioSource.Stop();
            audioSource.clip = null;
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

        public void ChangeSoundVolume(float __value)
        {
            audioSource.volume = __value;
        }

        public bool GetIsPlaying { get { return audioSource.isPlaying; } }

        /// <summary>
        /// 오디오 메모리 정리
        /// </summary>
        public void DestroyAudioClip()
        {
            // 없으면 사용 안한거니까 넘기기(보이스의 경우 백망되, 허블만 사용하고 있기 떄문에)
            if (DictSound.Count == 0)
                return;

            foreach (string key in DictSound.Keys) {
                
                if(DictSound[key].isAddressable)
                    DictSound[key].ReleaseClip();
                else    
                    DestroyImmediate(DictSound[key].audioClip, true);
            }

            DictSound.Clear();
        }
    }
}