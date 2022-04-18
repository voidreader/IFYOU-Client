using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PIERStory
{
    public class IntermissionManager : MonoBehaviour
    {
        // * 게임씬에서는 어느 씬으로 이동을 하던, 무조건 intermissino을 거치도록 한다. 
        
        public static bool isMovingLobby = false; // 

        
        
        public AudioClip[] audioClips = null;
        public RenderTexture[] renderTextures = null;
        public Texture2D[] texture2Ds = null;
        
        void Awake() {
            
        }

        
        IEnumerator Start()
        {
            
            
            // AdManager.main.HideBanner();
            SystemManager.ShowNetworkLoading(true);
            
            
            // * 메모리 누수를 알아보기 위해 FindObject 검사 실행. 
            audioClips = FindObjectsOfType<AudioClip>(true); // 정리되지 않음
            texture2Ds = FindObjectsOfType<Texture2D>(true); // 정리되지 않음

            for (int i = 0; i < audioClips.Length; i++)
                Destroy(audioClips[i]);

            Debug.Log(string.Format("[{0}] audioClips are destroyed", audioClips.Length));


            for (int i = 0; i < texture2Ds.Length; i++)
                Destroy(texture2Ds[i]);

            Debug.Log(string.Format("[{0}] texture2Ds are destroyed", texture2Ds.Length));

            // Destory는 Immediate이 아니기 때문에 일정 frame 혹은 시간 대기. 
            yield return new WaitForSeconds(0.2f);

            // 수동 호출. 
            System.GC.Collect();
            
            

            Debug.Log("Intermission Start #2");
            yield return new WaitForSeconds(0.2f);
            
            // 씬 전환하면서 정리하기 2022.03.30
            ViewCommonTop.OnBackAction = null;
            ViewNoticeDetail.isDependent = true;
            PopupManager.main.ClearShowingPopup(); // 인터미션에서 정리하고 넘어가도록 추가 
            CommonView.ClearActiveViews();


            // * 인터미션 씬은 오직 게임씬에서만 진입 가능하고, 에피소드 데이터가 있다.             
            if (SystemManager.main.givenEpisodeData != null)
            {
                Debug.Log("Intermission Start #3");

                if (isMovingLobby)
                {
                    // 로비씬으로 이동
                    Debug.Log("From Intermission to Lobby");
                    SceneManager.LoadSceneAsync(CommonConst.SCENE_LOBBY, LoadSceneMode.Single).allowSceneActivation = true;
                }
                else
                {
                    // 게임 씬으로 이동처리 
                    Debug.Log("From Intermission to Game");
                    SceneManager.LoadSceneAsync(CommonConst.SCENE_GAME, LoadSceneMode.Single).allowSceneActivation = true;
                }
            }
            else
            {
                Debug.LogError("!!!No episode data in intermission!!!");
            }
        }
    }
}