using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;
using LitJson;


namespace PIERStory
{
    public class ViewSoundDetail : CommonView, IPointerClickHandler, IDragHandler
    {
        AudioSource playSound;
        AudioClip audioClip;

        static bool playBGM = true;
        static JsonData soundData = null;

        public Image playtimeBar;
        public Image playtimeBarHandle;

        [Header("BGM 재생목록 관련")]
        public GameObject bgmList;
        public SoundElement[] BGMElements;

        [Header("음성 재생목록 관련")]
        public GameObject voiceList;
        public Transform voiceContents;
        public Transform storage;
        public TextMeshProUGUI[] episodeInfo;
        public SoundElement[] voiceElements;


        public static void SetSoundDetail(bool isBGM, JsonData __j)
        {
            playBGM = isBGM;
            soundData = __j;
        }

        public override void OnStartView()
        {
            base.OnStartView();

            if (playSound == null)
                playSound = GetComponent<AudioSource>();

            bgmList.SetActive(false);
            voiceList.SetActive(false);

            // 배경음을 재생하는지, 보이스를 재생하는지에 따라 이후 세팅이 다름
            if(playBGM)
            {
                for (int i = 0; i < soundData.Count; i++)
                    BGMElements[i].SetBGMElement(i +1, soundData[i]);

                bgmList.SetActive(true);
            }
            else
            {
                for (int i = 0; i < soundData.Count; i++)
                    voiceElements[i].SetVoiceElement(soundData[i]);

                voiceList.SetActive(true);
            }
        }


        private void Update()
        {
            if (playSound == null)
                return;



            if(playSound.isPlaying)
                playtimeBar.fillAmount = playSound.time / audioClip.length;
        }

        public void OnDrag(PointerEventData eventData)
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}
