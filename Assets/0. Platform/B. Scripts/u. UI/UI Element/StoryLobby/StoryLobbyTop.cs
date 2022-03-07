using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PIERStory {
    public class StoryLobbyTop : MonoBehaviour
    {
        
        public static Action OnInitializeStoryLobbyTop = null;
        public static Action OnRefreshSuperUser = null; // 슈퍼유저 표기용도 
        
        public GameObject mailNotify; // 상단 메일 알림 
        public GameObject objectSuperUser;
        
        public PassButton passButton;
        
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
        void InitStoryLobbyTop() {
            passButton.gameObject.SetActive(!UserManager.main.HasProjectFreepass());
            
            // 프리미엄 패스 정보 세팅하기 
            if(passButton.gameObject.activeSelf) {
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
    }
}