using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory {
    
    public enum MainNavigationType {
        Lobby,
        Library,
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
        
        public Color colorActive;
        public Color colorInactive;
        
        
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
            
            // textName.color = LobbyManager.main.colorNavOff;
            
            icon.color = colorInactive;
        }
        
        /// <summary>
        /// 활성 상태
        /// </summary>
        void InitOn() {

            if (LobbyManager.main == null)
                return;

            // textName.color = LobbyManager.main.colorNavOn;
            
            icon.color = colorActive;
        }
        
        
        
        public void OnToggle() {
            InitOn();
        }
        
        public void OffToggle() {
            InitOff();
        }
    }
}