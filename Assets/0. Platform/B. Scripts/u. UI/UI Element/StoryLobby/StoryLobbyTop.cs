using System;
using UnityEngine;


namespace PIERStory {
    public class StoryLobbyTop : MonoBehaviour
    {
        public static Action OnInitializeStoryLobbyTop = null;
        
        public PassButton passButton; // 프리미엄 패스 구매 버튼
        public ImageRequireDownload passBadge; // 프리미엄 패스 뱃지 
        
        public AllPassTimer allpassTimer; // 올패스 타이머 
        
        
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
            /*
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
            */
            
            // * 프리미엄패스 vs 올패스는 프리미엄패스가 우선한다. 
            if(UserManager.main.HasProjectPremiumPassOnly(StoryManager.main.CurrentProjectID)) {
                
                // 올패스랑 패스버튼 비활성화
                allpassTimer.gameObject.SetActive(false);
                passButton.gameObject.SetActive(false);
                
                // 뱃지 세팅 
                passBadge.SetDownloadURL(StoryManager.main.freepassBadgeURL, StoryManager.main.freepassBadgeKey, true);
                passBadge.gameObject.SetActive(true);    
                
                return;
            }
            
            
            // 유효시간이 남은 올패스 존재 
            if(!string.IsNullOrEmpty(UserManager.main.GetAllPassTimeDiff())) {
                
                passButton.gameObject.SetActive(false);
                passBadge.gameObject.SetActive(false);
                
                allpassTimer.InitAllPassTimer();
                return;
            }
            
            // 프리미엄 패스도 없고 올패스도 없다. 
            // 프리미엄 패스 버튼 세팅하기 
            passButton.gameObject.SetActive(true);
            passButton.SetPremiumPass();
            
            allpassTimer.gameObject.SetActive(false);
            passBadge.gameObject.SetActive(false);
            
                
        }
    }
}