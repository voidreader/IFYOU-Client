using System.Collections;
using UnityEngine;

using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class StoryLobbyManager : MonoBehaviour
    {
        public static StoryLobbyManager main = null;
        

        [Header("작품 로비")]
        public SpriteRenderer lobbyBackground;

        [Space(15)][Header("갤러리 - 일러스트")]
        public ScriptLiveMount currentLiveIllust = null; // Live Illust for Gallery 
        public ScriptLiveMount currentLiveObject = null; // Live Object for Gallery
        public TouchEffect touchEffect;                  // 일러스트 상세에서 터치 이펙트 제어용

        int scaleOffset = 0;
        string illustName = string.Empty;

        [Header("갤러리 - 사운드 BGMSprite")]
        public Sprite spriteOpenVoice;
        public Sprite spriteLockVoice;
        public Sprite toggleSelected;
        public Sprite toggleUnselected;

        [Header("미션View Sprite")]
        public Sprite spriteGetReward;          // 얻을 수 있는 보상 버튼
        public Sprite spriteGotReward;          // 얻은 보상 버튼

        [Header("에피소드 관련 Sprite")]
        public Sprite spriteEpisodeOpen;    // 열린 스페셜, 엔딩 에피소드
        public Sprite spriteEpisodeLock;    // 잠긴 스페셜, 엔딩 에피소드


        [Space]
        public NetworkLoadingScreen storylobbyNetworkLoadingScreen;

        private void Awake()
        {
            main = this;
        }


        // Use this for initialization
        IEnumerator Start()
        {
            Debug.Log(">> StoryManager Start ");
            
            yield return null;
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            yield return new WaitUntil(() => ViewStoryLoading.assetLoadComplete);

            StoryManager.main.SetLobbyBubbleMaster();

            yield return new WaitForSeconds(0.5f);
            
            Debug.Log(">> StoryManager assetLoadComplete Done");
            
            // if (BubbleManager.main != null)
            //     BubbleManager.main.ShowFakeBubbles(false);

            ViewStoryLobby.OnDecorateSet?.Invoke();
            yield return new WaitUntil(() => ViewStoryLobby.loadComplete);
            yield return new WaitForSeconds(0.1f);
            SystemManager.SetBlockBackButton(false);

            Signal.Send(LobbyConst.STREAM_IFYOU, "storyLobbyLoadComplete", string.Empty);
            
            UserManager.main.CheckUnlockedMission(); // 미션 해금 체크 

        }


        public void SetLiveParent(Transform __model)
        {
            __model.SetParent(transform);
        }

        /// <summary>
        /// 로비 씬에서 사용하는 네트워크 로딩 스크린 받기
        /// </summary>
        /// <returns></returns>
        public NetworkLoadingScreen GetLobbyNetworkLoadingScreen()
        {
            return storylobbyNetworkLoadingScreen;
        }


        /// <summary>
        /// 갤러리의 라이브 일러스트 처리!
        /// </summary>
        /// <param name="__name"></param>
        /// <param name="__scale"></param>
        public void SetGalleryLiveIllust(string __name, int __scale, bool liveObj)
        {
            scaleOffset = __scale;
            illustName = __name;


            if (!liveObj)
            {
                currentLiveIllust = new ScriptLiveMount(illustName, OnGalleryLiveIllustMount, this, false);
                currentLiveIllust.SetModelDataFromStoryManager(true);

            }
            else
            { // 라이브 오브제 추가 
                currentLiveObject = new ScriptLiveMount(illustName, OnGalleryLiveObjectMount, this, true);
                currentLiveObject.SetModelDataFromStoryManager(true);
            }
        }

        /// <summary>
        /// 갤러리 Live Object 마운트 완료 
        /// </summary>
        void OnGalleryLiveObjectMount()
        {

            if (currentLiveObject == null || currentLiveObject.liveImage == null)
            {
                Debug.LogError("Something wrong in OnGalleryLiveObjectMount");
                return;
            }

            currentLiveObject.liveImage.transform.localScale = new Vector3(currentLiveObject.gameScale, currentLiveObject.gameScale, 1);

            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SHOW_ILLUSTDETAIL, string.Empty);
        }

        void OnGalleryLiveIllustMount()
        {
            Debug.Log(string.Format("OnGalleryLiveIllustMount gameScale({0})/scaleOffset({1})", currentLiveIllust.gameScale, scaleOffset));
            float scale = currentLiveIllust.gameScale + scaleOffset;

            currentLiveIllust.liveImage.transform.localScale = new Vector3(scale, scale, 1);

            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SHOW_ILLUSTDETAIL, string.Empty);
        }

    }
}