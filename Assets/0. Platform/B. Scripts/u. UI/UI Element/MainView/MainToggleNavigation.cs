using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PIERStory {
    
    public enum MainNavigationType {
        Lobby,
        Category,
        Shop,
        IFYou,
        Profile,
        More
    }
    
    public class MainToggleNavigation : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textName;
        [SerializeField] Image icon;
        
        [SerializeField] MainNavigationType mainNavigationType;
        
        /// <summary>
        /// 비활성 상태 
        /// </summary>
        void InitOff() {
            
            textName.color = LobbyManager.main.colorNavOff;
            
            switch(mainNavigationType) {
                case MainNavigationType.Lobby:
                icon.sprite = LobbyManager.main.spriteNavLobbyOff;
                break;
                
                case MainNavigationType.Category:
                icon.sprite = LobbyManager.main.spriteNavCategoryOff;
                break;
                
                case MainNavigationType.Shop:
                icon.sprite = LobbyManager.main.spriteNavShopOff;
                break;
                
                case MainNavigationType.IFYou:
                icon.sprite = LobbyManager.main.spriteNavIfYouOff;
                break;
                
                case MainNavigationType.Profile:
                icon.sprite = LobbyManager.main.spriteNavProfileOff;
                break;
                
                case MainNavigationType.More:
                icon.sprite = LobbyManager.main.spriteNavMoreOff;
                break;
            }
            
            icon.SetNativeSize();
        }
        
        /// <summary>
        /// 활성 상태
        /// </summary>
        void InitOn() {
            textName.color = LobbyManager.main.colorNavOn;
            
            switch(mainNavigationType) {
                case MainNavigationType.Lobby:
                icon.sprite = LobbyManager.main.spriteNavLobbyOn;
                break;
                
                case MainNavigationType.Category:
                icon.sprite = LobbyManager.main.spriteNavCategoryOn;
                break;
                
                case MainNavigationType.Shop:
                icon.sprite = LobbyManager.main.spriteNavShopOn;
                break;
                
                case MainNavigationType.IFYou:
                icon.sprite = LobbyManager.main.spriteNavIfYouOn;
                break;
                
                case MainNavigationType.Profile:
                icon.sprite = LobbyManager.main.spriteNavProfileOn;
                break;
                
                case MainNavigationType.More:
                icon.sprite = LobbyManager.main.spriteNavMoreOn;
                break;
            }
            
            icon.SetNativeSize();
        }
        
        
        
        public void OnToggle() {
            InitOn();
        }
        
        public void OffToggle() {
            InitOff();
        }
    }
}