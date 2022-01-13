using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {

    public class NicknameIndicator : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textNickname = null;
        
        void Start() {
            
            if(textNickname == null)
                return;
                
            UserManager.main.AddNicknameIndicator(this);

        }
        
        void OnEnable() {
            if(textNickname != null && UserManager.main != null && !string.IsNullOrEmpty(UserManager.main.nickname)) {
                textNickname.text = UserManager.main.nickname;
            }
        }
        
        
        /// <summary>
        /// 갱신
        /// </summary>
        /// <param name="__nick"></param>
        public void RefreshNickname(string __nick) {
            textNickname.text =__nick;
        }
    }
    
}