using System;
using UnityEngine;


namespace PIERStory {
    public class StoryLobbyTop : MonoBehaviour
    {
        public static Action OnInitializeStoryLobbyTop = null;
        
        public PassButton passButton;
        public ImageRequireDownload passBadge;
        
        // Start is called before the first frame update
        void Start()
        {
            OnInitializeStoryLobbyTop = InitStoryLobbyTop;
        }


        /// <summary>
        /// 초기화
        /// </summary>
        void InitStoryLobbyTop()
        {
            // 만약 튜토리얼 2단계까지 마치지 않으면 프리미엄 패스를 구매할 수 없도록 모두 비활성화
            if(UserManager.main.tutorialStep <= 2)
            {
                if(UserManager.main.tutorialStep == 2 && UserManager.main.tutorialClear)
                {

                }
                else
                {
                    passBadge.gameObject.SetActive(false);
                    passButton.gameObject.SetActive(false);
                    return;
                }
                
            }

            passBadge.SetDownloadURL(StoryManager.main.freepassBadgeURL, StoryManager.main.freepassBadgeKey, true);
            passBadge.gameObject.SetActive(UserManager.main.HasProjectFreepass());
            passButton.gameObject.SetActive(!UserManager.main.HasProjectFreepass());

            // 프리미엄 패스 정보 세팅하기 
            if (passButton.gameObject.activeSelf)
                passButton.SetPremiumPass();
        }
    }
}