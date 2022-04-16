using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Toast.Gamebase;


namespace PIERStory {

    
    public class PopupAccount : PopupBase
    {
        public static System.Action OnRefresh = null;
        
        [SerializeField] Image icon;
        [SerializeField] RectTransform frame;
        
        [SerializeField] GameObject groupBonus; // 그룹 보너스 
        [SerializeField] GameObject buttons; // 하단 버튼
        
        [SerializeField] TextMeshProUGUI textAuthComplete; // 연동 완료 
        [SerializeField] GameObject groupExplain;
        
        [SerializeField] bool isGoogleOn = false;
        [SerializeField] bool isAppleOn = false;        
        [SerializeField] List<string> listAuthMapping; // 게임베이스 연결된 인증 리스트 
        
        
        [Space]
        [Header("Icons")]
        [SerializeField] Sprite iconGuest;
        [SerializeField] Sprite iconGoogle;
        [SerializeField] Sprite iconApple;
        
        
        const string AUTH_APPLE = "appleid"; // 애플 인증
        const string AUTH_GOOGLE = "google"; // 구글 인증
        
        Vector2 bigPosition = new Vector2(0, -300);
        Vector2 smallPosition = new Vector2(0,-55);

        
        Vector2 bigSize = new Vector2(540,620);
        Vector2 smallSize = new Vector2(540,380);
        
        private void Start() {
            OnRefresh = RefreshPopup;
        }
        
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            
            
            
            RefreshPopup();
        }
        
        /// <summary>
        /// 팝업 컨트롤 개체
        /// </summary>
        void RefreshPopup() {
            ResetVariables();
            InitAuthMappingList();
        }
        
        
        /// <summary>
        /// 변수들 초기화!
        /// </summary>
        void ResetVariables() {
            isGoogleOn = false;
            isAppleOn = false;
            
            // 프레임 관련 처리 
            // 연동되지 않은 상태를 기준.
            frame.sizeDelta = bigSize;
            frame.localPosition = bigPosition;
            icon.sprite = iconGuest;
            icon.SetNativeSize();
            
            groupBonus.SetActive(false);
            groupExplain.SetActive(true);
            textAuthComplete.gameObject.SetActive(false);
            
            buttons.SetActive(true);
            
            // 계정연동 안되어있는 경우 groupBonus 활성화 
            groupBonus.SetActive(!UserManager.main.CheckAccountLink());
        }
        
        
        
        /// <summary>
        /// 맵핑 정보 초기화
        /// </summary>
        void InitAuthMappingList() {
            listAuthMapping = Gamebase.GetAuthMappingList();
            
            if(listAuthMapping != null && listAuthMapping.Count > 0) {
            
            
                // * 2021.09 둘 중 하나만 가능하도록 처리
                if(listAuthMapping.Contains(AUTH_APPLE)) {
                    isAppleOn = true;
                    isGoogleOn = false;
                }
                
                if(listAuthMapping.Contains(AUTH_GOOGLE)) {
                    isGoogleOn = true;
                    isAppleOn = false;
                }
            }
            
            if(!isGoogleOn && !isAppleOn) {
                return;
            }
            
            // 하나라도 연동된 정보가 있는 경우
            frame.sizeDelta = smallSize;
            frame.localPosition = smallPosition;
            
            groupBonus.SetActive(false);
            groupExplain.SetActive(false);
            textAuthComplete.gameObject.SetActive(true);
            
            buttons.SetActive(false);
            
            if(isGoogleOn)
                icon.sprite = iconGoogle;
            else
                icon.sprite = iconApple;
                
            icon.SetNativeSize();

        }        
        
        
        
        public void OnClickGoogle() {
            SystemManager.main.LoginGamebaseBySlide(AUTH_GOOGLE);
        }
         
        public void OnClickApple() {
            SystemManager.main.LoginGamebaseBySlide(AUTH_APPLE);
        }     
        
    }
}