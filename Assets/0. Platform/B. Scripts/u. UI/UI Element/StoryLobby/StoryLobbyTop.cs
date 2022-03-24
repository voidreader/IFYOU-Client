using System;
using UnityEngine;

using Doozy.Runtime.Signals;


namespace PIERStory {
    public class StoryLobbyTop : MonoBehaviour
    {
        
        public static Action OnInitializeStoryLobbyTop = null;
        public static Action OnRefreshSuperUser = null; // 슈퍼유저 표기용도 
        
        public GameObject mailNotify; // 상단 메일 알림 
        public GameObject objectSuperUser;
        
        public PassButton passButton;
        public ImageRequireDownload passBadge;
        
        // Start is called before the first frame update
        void Start()
        {
            OnRefreshSuperUser = SetSuperUser;
            UserManager.OnRefreshUnreadMailCount += RefreshMailNotification;
            
            OnInitializeStoryLobbyTop = InitStoryLobbyTop;
        }

        void RefreshMailNotification(int __cnt) {
            mailNotify.SetActive(__cnt > 0);
        }


        /// <summary>
        /// 초기화
        /// </summary>
        void InitStoryLobbyTop()
        {
            // 만약 튜토리얼 2단계까지 마치지 않으면 프리미엄 패스를 구매할 수 없도록 모두 비활성화
            if(UserManager.main.tutorialStep <= 2)
            {
                passBadge.gameObject.SetActive(false);
                passButton.gameObject.SetActive(false);
                return;
            }

            passBadge.SetDownloadURL(StoryManager.main.freepassBadgeURL, StoryManager.main.freepassBadgeKey, true);
            passBadge.gameObject.SetActive(UserManager.main.HasProjectFreepass());
            passButton.gameObject.SetActive(!UserManager.main.HasProjectFreepass());

            // 프리미엄 패스 정보 세팅하기 
            if (passButton.gameObject.activeSelf)
            {
                passButton.SetPremiumPass();
            }
        }
        
        
        
        /// <summary>
        /// 슈퍼유저 표기 
        /// </summary>
        void SetSuperUser() {
            
            if(UserManager.main == null || string.IsNullOrEmpty(UserManager.main.userKey)) {
                objectSuperUser.SetActive(false);
                return;
            }
            
            Debug.Log("### SetSuperUser ###");
            objectSuperUser.SetActive(UserManager.main.CheckAdminUser());
        }
        
        public void OnClickMail() {
            Signal.Send(LobbyConst.STREAM_COMMON, "Mail", string.Empty);
        }
        
        public void OnClickShop() {
            Signal.Send(LobbyConst.STREAM_COMMON, "Shop", string.Empty);
        }
        
        public void OnClickCoin() {
            SystemManager.main.OpenCoinShopWebview();
        }
    }
}