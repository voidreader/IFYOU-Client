using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {

    public class AllPassTimer : MonoBehaviour
    {
        public TextMeshProUGUI textTimer;
        public bool isCountable = false;
        
        
        /// <summary>
        /// 올패스 타이머 초기화 
        /// </summary>
        public void InitAllPassTimer() {
            
            this.gameObject.SetActive(true);
            
            if(!string.IsNullOrEmpty(UserManager.main.GetAllPassTimeDiff())) {
                isCountable = true;
            }
        }
        
        void Update() {
            
            if(!isCountable)
                return;
            
            
            // 시간 체크해서 만료 시간까지는 살아있게 한다. 
            // 10 프레임마다 체크 
            if(Time.frameCount % 10 == 0) {
                textTimer.text = UserManager.main.GetAllPassTimeDiff();
                
                if(string.IsNullOrEmpty(textTimer.text)) {
                    isCountable = false;
                    this.gameObject.SetActive(false);
                    
                    // 화면 갱신 
                    StoryLobbyTop.OnInitializeStoryLobbyTop?.Invoke();
                }
            }
        }
        
        
    }
}