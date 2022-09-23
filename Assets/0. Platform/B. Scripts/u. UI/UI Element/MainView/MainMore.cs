using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

using TMPro;
using Toast.Gamebase;
using Doozy.Runtime.Reactor.Animators;

using Firebase.Analytics;

namespace PIERStory {

    public class MainMore : MonoBehaviour
    {
        public static System.Action OnRefreshMore = null;
        public static System.Action<string> OnUpdateNickname = null;

        [SerializeField] GameObject accountBonus;
        [SerializeField] GameObject couponButton; // 쿠폰 버튼 (iOS에서 비활성)
        [SerializeField] RectTransform usermenuRect; // 유저 메뉴 Rect
        public TextMeshProUGUI useNicknameText;     // 유저 닉네임
        [SerializeField] TextMeshProUGUI textUID;

        public Image pushAlert;                 // 푸쉬 알림
        

        Vector2 toggleOnPosition = new Vector2(11, -3);
        Vector2 toggleOffPosition = new Vector2(-11, -3);
        

        public Sprite spriteToggleOn;
        public Sprite spriteToggleOff;
        public RectTransform toggleIconPos;

        
        [SerializeField] TextMeshProUGUI textVersion;
        
        [SerializeField] int clickLevelCount = 0; 
        [SerializeField] int clickVersionCount = 0;

        void Start()
        {
            OnRefreshMore = RefreshScreen;
            OnUpdateNickname = UpdateUserNickname;

            // iOS에서는 쿠폰 제거 
#if UNITY_IOS
            couponButton.SetActive(false);
            usermenuRect.sizeDelta = new Vector2(720, 300);
#endif
        }

        private void OnEnable()
        {
            RefreshScreen();
        }


        void RefreshScreen()
        {
            clickLevelCount = 0;
            clickVersionCount = 0;
            
            accountBonus.SetActive(false);

            if (UserManager.main == null || string.IsNullOrEmpty(UserManager.main.userKey))
                return;
                
            Debug.Log("#### RefreshScreen");

            accountBonus.SetActive(!UserManager.main.CheckAccountLink());
            

            textVersion.text = SystemManager.GetLocalizedText("5053") + " " + Application.version;      // 버전
            textUID.text = string.Format("UID : {0}", UserManager.main.GetUserPinCode());               // UID
            SystemManager.SetText(useNicknameText, UserManager.main.nickname);


            #region 게임베이스 push

            if (Application.isEditor)          
                return;

            // 푸시 토글 세팅 
            if (SystemManager.main.pushTokenInfo == null)
            {
                // SystemManager.main.QueryPushTokenInfo(RefreshScreen);
            }
            else
            {
                if(!SystemManager.main.pushTokenInfo.agreement.pushEnabled || SystemManager.main.pushTokenInfo.agreement.adAgreement) {
                    pushAlert.sprite = spriteToggleOff;
                    toggleIconPos.anchoredPosition = toggleOffPosition;
                }
                else {
                    pushAlert.sprite = spriteToggleOn;
                    toggleIconPos.anchoredPosition = toggleOnPosition;
                }

            }
            
            #endregion

        }

        void UpdateUserNickname(string nickname)
        {
            UserManager.main.nickname = nickname;
            SystemManager.SetText(useNicknameText, nickname);
        }
        

        
        /// <summary>
        /// 푸쉬 알림 토글 클릭
        /// </summary>
        public void OnClickPushAlert()
        {
            Debug.Log("OnClickPushAlert");
            
            if(Application.isEditor) {
                Debug.Log("It's Editor");
                return;
            }
            
            if(SystemManager.main.pushTokenInfo == null) {
                Debug.Log("Push token is null");
                SystemManager.main.QueryPushTokenInfo(OnClickPushAlert);
                return;
            }
                
                
            // 푸시 동의를 하지 않으면 진행되지 않음 .
            if(!SystemManager.main.pushTokenInfo.agreement.pushEnabled) {
                Debug.Log("Push is disable");
                SystemManager.ShowMessageAlert(SystemManager.GetLocalizedText("6485"));
                return;
            }                

            // 푸쉬 토글이 On이면 Off로 변경 
            if(pushAlert.sprite == spriteToggleOn)
            {
                SystemManager.main.PushRegister(false, false);
                pushAlert.sprite = spriteToggleOff;
                toggleIconPos.anchoredPosition = toggleOffPosition;
            }
            else
            {
                SystemManager.main.PushRegister(true, true);
                pushAlert.sprite = spriteToggleOn;
                toggleIconPos.anchoredPosition = toggleOnPosition;
                
            }
        }



        /// <summary>
        /// 언어
        /// </summary>
        public void OnClickLanguage()
        {
            PopupBase p = PopupManager.main.GetPopup("Language");
            

            PopupManager.main.ShowPopup(p, true);
        }


        /// <summary>
        /// 코인스타 히스토리 
        /// </summary>
        public void OnClickStarHistory()
        {
            PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_COIN_STAR_HISTORY);
            Debug.Log(">> OnClickStarHistory");

            PopupManager.main.ShowPopup(p, true);
        }


        /// <summary>
        /// 데이터 매니저
        /// </summary>
        public void OnClickDataManage()
        {
            // PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_DATA_MANAGER);
            // PopupManager.main.ShowPopup(p, true);
            Debug.Log(">> OnClickDataManage");
            
            SystemManager.ShowSystemPopupLocalize("6439", DeleteAsset, null, true);
        }
        
        void DeleteAsset() {
            
            Debug.Log("Delete All Asset");
            
            for(int i=0;i<StoryManager.main.listTotalStory.Count;i++) {
                Addressables.ClearDependencyCacheAsync(StoryManager.main.listTotalStory[i].projectID);
            }
            
            
            SystemManager.ShowMessageWithLocalize("6022");
            
        }

        /// <summary>
        /// 쿠폰
        /// </summary>
        public void OnClickCoupon()
        {
            Debug.Log(">> OnClickCoupon");

            PopupBase coupon = PopupManager.main.GetPopup("Coupon");
            PopupManager.main.ShowPopup(coupon, true, false);
        }

        /// <summary>
        /// 공지사항
        /// </summary>
        public void OnClickNotice()
        {
            FirebaseAnalytics.LogEvent("menu_notice");
            
            
            PopupBase p = PopupManager.main.GetPopup("Notice");
            PopupManager.main.ShowPopup(p, true);
        }

        public void OnClickInquiry()
        {
            Debug.Log("Open Contact :: " + Gamebase.GetDeviceLanguageCode() +"/" + Gamebase.GetDisplayLanguageCode());

            Gamebase.Contact.OpenContact((error) =>
            {
                if (Gamebase.IsSuccess(error))
                {

                }
                else
                {
                    Debug.Log("GameBase Contact Error : " + error.code);
                }
            });
        }

        public void OnClickPrivacy()
        {
            SystemManager.main.OpenPrivacyURL();
        }

        public void OnClickTermsOfUse()
        {
            SystemManager.main.OpenTermsURL();
        }

        public void OnClickLeave()
        {

        }
        
        public void OnClickCopyUID() {
            UniClipboard.SetText(textUID.text);
            SystemManager.ShowSimpleAlertLocalize("6017");
        }
        

        /// <summary>
        /// 닉네임 체인지 
        /// </summary>
        public void OnClickNicknameChange()
        {
            PopupBase p = PopupManager.main.GetPopup("Nickname");
            if (p == null)
            {
                Debug.LogError("No Nickname popup");
                return;
            }

            PopupManager.main.ShowPopup(p, true);
        }

        /// <summary>
        /// 계정 연결 
        /// </summary>
        public void OnClickAccount()
        {
            PopupBase p = PopupManager.main.GetPopup("Account");
            if (p == null)
            {
                Debug.LogError("No account popup");
                return;
            }

            PopupManager.main.ShowPopup(p, true);
            
            
            // 7번 누르면 어드민 설정 가능 
            if(clickVersionCount >= 7) {
                
                UserManager.main.SetAdminUser();
            }
        }
        
        
        
        public void OnClickVersion() {
            clickVersionCount++;
        }
        
        public void OnClickLevel() {
            clickLevelCount++;
        }
        
        public void OnClickCopyright() {
            SystemManager.main.OpenCopyrightURL();
        }
        
        public void OnClickWithdraw() {
            SystemManager.ShowSystemPopupLocalize("6050", Withdraw, null, true);
        }
        
        void Withdraw() {
            SystemManager.main.WithdrawGamebase();
        }

    }
}