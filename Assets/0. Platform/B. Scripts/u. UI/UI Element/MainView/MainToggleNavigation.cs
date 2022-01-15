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
        public static System.Action OnToggleAccountBonus = null;
        
        [SerializeField] TextMeshProUGUI textName;
        [SerializeField] Image icon;
        
        [SerializeField] MainNavigationType mainNavigationType;
        [SerializeField] GameObject accountBonus;
        
        void Start() {
            if(mainNavigationType != MainNavigationType.More)
                return;
                
            OnToggleAccountBonus = RefreshAccountBonus;
        }
        
        void OnEnable() {
            RefreshAccountBonus();
        }
        
        void RefreshAccountBonus() {
            if(UserManager.main == null || string.IsNullOrEmpty(UserManager.main.userKey))
                return;
            
            // 더보기에서만 작동
            // 계정연동 되지 않은 경우에는 보상 표시
            if(mainNavigationType == MainNavigationType.More && UserManager.main.accountLink == "-")
                accountBonus.SetActive(true);
            else 
                accountBonus.SetActive(false);
            
        }
        
        
        
        /// <summary>
        /// 비활성 상태 
        /// </summary>
        void InitOff() {
            
            if(LobbyManager.main == null)
                return;
            
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

            if (LobbyManager.main == null)
                return;

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