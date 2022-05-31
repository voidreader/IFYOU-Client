using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Toast.Gamebase;
using Doozy.Runtime.Signals;
using Doozy.Runtime.Reactor.Animators;
using TMPro;

namespace PIERStory {

    public class MainMore : MonoBehaviour
    {
        public static System.Action OnRefreshMore = null;

        [SerializeField] GameObject accountBonus;
        [SerializeField] GameObject couponButton; // 쿠폰 버튼 (iOS에서 비활성)
        [SerializeField] RectTransform usermenuRect; // 유저 메뉴 Rect
        [SerializeField] TextMeshProUGUI textUID;

        public Image pushAlert;                 // 푸쉬 알림
        public Image nightPushAlert;            // 야간 푸쉬 알림
        public Image dataUseAgree;              // 데이터 사용 허용
        public UIAnimator pushAlertAnimator;
        public UIAnimator nightAlertAnimator;
        public UIAnimator networkAgreeAnimator;
        

        public Sprite spriteToggleOn;
        public Sprite spriteToggleOff;

        
        [SerializeField] TextMeshProUGUI textVersion;
        
        [SerializeField] int clickLevelCount = 0; 
        [SerializeField] int clickVersionCount = 0;

        void Start()
        {
            OnRefreshMore = RefreshScreen;
            
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



            #region 게임베이스 push

            if (Application.isEditor)          
                return;

            // 푸시 토글 세팅 
            if (SystemManager.main.pushTokenInfo == null)
            {
                SystemManager.main.QueryPushTokenInfo();
            }
            else
            {
                if (SystemManager.main.pushTokenInfo.agreement.adAgreement)
                {
                    pushAlert.sprite = spriteToggleOn;
                    pushAlertAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(11, -3);
                    nightPushAlert.color = Color.white;
                }
                else
                {
                    pushAlert.sprite = spriteToggleOff;
                    pushAlertAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(-11, -3);
                    nightPushAlert.color = new Color32(153, 153, 153, 255);
                }

                if(SystemManager.main.pushTokenInfo.agreement.adAgreementNight)
                {
                    nightPushAlert.sprite = spriteToggleOn;
                    nightAlertAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(11, -3);
                }
                else
                {
                    nightPushAlert.sprite = spriteToggleOff;
                    nightAlertAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(-11, -3);
                }

                if(PlayerPrefs.GetInt(SystemConst.KEY_NETWORK_DOWNLOAD) == 1)
                {
                    dataUseAgree.sprite = spriteToggleOn;
                    networkAgreeAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(11, -3);
                }
                else
                {
                    dataUseAgree.sprite = spriteToggleOff;
                    networkAgreeAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(-11, -3);
                }
            }
            
            #endregion

        }
        

        
        /// <summary>
        /// 푸쉬 알림 토글 클릭
        /// </summary>
        public void OnClickPushAlert()
        {
            if (pushAlertAnimator.animation.isPlaying || nightAlertAnimator.animation.isPlaying)
                return;

            // 푸쉬 토글이 On이면
            if(SystemManager.main.pushTokenInfo.agreement.adAgreement)
            {
                // 야간 푸쉬알림도 On이었다면
                if(SystemManager.main.pushTokenInfo.agreement.adAgreementNight)
                {
                    nightPushAlert.sprite = spriteToggleOff;
                    nightAlertAnimator.Play(true);
                }

                SystemManager.main.PushRegister(false, false);
                pushAlert.sprite = spriteToggleOff;
                pushAlertAnimator.Play(true);
                nightPushAlert.color = new Color32(153, 153, 153, 255);
            }
            else
            {
                SystemManager.main.PushRegister(true, false);
                pushAlert.sprite = spriteToggleOn;
                nightPushAlert.color = Color.white;
                pushAlertAnimator.Play();
            }
        }

        /// <summary>
        /// 야간 푸쉬 알림 토글 클릭
        /// </summary>
        public void OnClickNightAlert()
        {
            // 애니메이션 중이어도 막는다
            if (pushAlertAnimator.animation.isPlaying || nightAlertAnimator.animation.isPlaying)
                return;

            // 푸쉬 토글이 On이 아니면 팝업만 띄우고 막는다
            if (!SystemManager.main.pushTokenInfo.agreement.adAgreement)
            {
                SystemManager.ShowSimpleAlertLocalize("6033");
                return;
            }

            // 야간 알림이 On이면
            if (SystemManager.main.pushTokenInfo.agreement.adAgreementNight)
            {
                SystemManager.main.PushRegister(true, false);
                nightPushAlert.sprite = spriteToggleOff;
                nightAlertAnimator.Play(true);
            }
            else
            {
                SystemManager.main.PushRegister(true, true);
                nightPushAlert.sprite = spriteToggleOn;
                nightAlertAnimator.Play();
            }
        }

        /// <summary>
        /// 데이터 사용
        /// </summary>
        public void OnClickDataUse()
        {
            // 데이터 사용 허용중이면
            if(PlayerPrefs.GetInt(SystemConst.KEY_NETWORK_DOWNLOAD) == 1)
            {
                PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 0);
                dataUseAgree.sprite = spriteToggleOff;
                networkAgreeAnimator.Play(true);
            }
            else
            {
                PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 1);
                dataUseAgree.sprite = spriteToggleOn;
                networkAgreeAnimator.Play();
            }
        }


        /// <summary>
        /// 언어
        /// </summary>
        public void OnClickLanguage()
        {
            PopupBase p = PopupManager.main.GetPopup("Language");
            Debug.Log(">> OnClickDataManage");

            PopupManager.main.ShowPopup(p, true);
            //Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_LANGUAGE, string.Empty);
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
            PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_DATA_MANAGER);
            Debug.Log(">> OnClickDataManage");

            PopupManager.main.ShowPopup(p, true);
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
        
        

    }
}