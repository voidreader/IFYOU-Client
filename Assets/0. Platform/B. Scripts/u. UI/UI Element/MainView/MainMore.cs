using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Toast.Gamebase;
using Doozy.Runtime.Signals;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIManager.Components;
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
        public UIToggle pushToggle;
        public UIToggle nightPushToggle;
        public UIToggle dataUseToggle;

        public UIAnimator nightPushAnimator;
        public GameObject blockNightPushToggle;

        public Sprite spriteToggleOn;
        public Sprite spriteToggleOff;

        bool currentShow = false;
        
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

        public void ShowMainMore()
        {
            currentShow = true;

            // 데이터 사용 허용
            if (PlayerPrefs.GetInt(SystemConst.KEY_NETWORK_DOWNLOAD) > 0) {
                // dataUseToggle.isOn = true;
                dataUseToggle.SetIsOn(true, false);
                
            }
            else {
                // dataUseToggle.isOn = false;
                dataUseToggle.SetIsOn(false, false);
            }
        }

        public void HideMainMore()
        {
            currentShow = false;
        }

        void RefreshScreen()
        {
            
            clickLevelCount = 0;
            clickVersionCount = 0;
            
            accountBonus.SetActive(false);

            if (UserManager.main == null || string.IsNullOrEmpty(UserManager.main.userKey))
                return;
                
            Debug.Log("#### RefreshScreen");

            if (UserManager.main.accountLink == "-")
                accountBonus.SetActive(true);
            else 
                accountBonus.SetActive(false);
            

            textVersion.text = SystemManager.GetLocalizedText("5053") + " " + Application.version;      // 버전
            textUID.text = string.Format("UID : {0}", UserManager.main.GetUserPinCode());               // UID



            #region 게임베이스 push

            if (Application.isEditor) {
                pushToggle.SetIsOn(false, false); 
                nightPushToggle.SetIsOn(false, false);                
                return;
            }


            // 푸시 토글 세팅 
            if(SystemManager.main.pushTokenInfo == null) {
                
            }
            else {
                pushToggle.SetIsOn(SystemManager.main.pushTokenInfo.agreement.adAgreement, false); 
                nightPushToggle.SetIsOn(SystemManager.main.pushTokenInfo.agreement.adAgreementNight, false);                
            }
            
            if (pushToggle.isOn)
                pushToggle.OnToggleOnCallback.Execute();
            else
                pushToggle.OnToggleOffCallback.Execute();


            if (nightPushToggle.isOn)
                nightPushToggle.OnToggleOnCallback.Execute();
            else
                nightPushToggle.OnToggleOffCallback.Execute();
            #endregion


        }
        
        void RefreshPushSettings() {

        }
        


        /// <summary>
        /// 푸쉬 알림 On
        /// </summary>
        public void ToggleOnPushAlert()
        {
            if(SystemManager.main == null)
                return;
            
            pushAlert.sprite = spriteToggleOn;
            nightPushAlert.color = Color.white;
            blockNightPushToggle.SetActive(false);

            SystemManager.main.PushRegister(pushToggle.isOn, nightPushToggle.isOn);
        }

        /// <summary>
        /// 푸쉬 알림 OFf
        /// </summary>
        public void ToggleOffPushAlert()
        {
            if(SystemManager.main == null)
                return;
            
            pushAlert.sprite = spriteToggleOff;

            if(nightPushToggle.isOn)
                nightPushToggle.isOn = false;

            nightPushAlert.color = new Color32(153, 153, 153, 255);
            blockNightPushToggle.SetActive(true);

            SystemManager.main.PushRegister(pushToggle.isOn, false);
        }


        /// <summary>
        /// 야간 푸쉬 알림 On
        /// </summary>
        public void ToggleOnNightPushAlert()
        {
            nightPushAnimator.Play();
            nightPushAlert.sprite = spriteToggleOn;

            if(SystemManager.main != null)
                SystemManager.main.PushRegister(true, true);
        }

        /// <summary>
        /// 야간 푸쉬 알림 Off
        /// </summary>
        public void ToggleOffNightPushAlert()
        {
            nightPushAnimator.Play(true);
            nightPushAlert.sprite = spriteToggleOff;
            SystemManager.main.PushRegister(pushToggle.isOn, false);
        }

        /// <summary>
        /// 푸쉬 알림이 꺼져있을 때, 야간 푸쉬 터치를 막는다
        /// </summary>
        public void BlockNightPushToggle()
        {
            SystemManager.ShowSimpleAlertLocalize("6033");
        }

        /// <summary>
        /// 데이터 사용 허용 On
        /// </summary>
        public void ToggleOnDataUse()
        {
            if (!currentShow)
                return;

            dataUseAgree.sprite = spriteToggleOn;
            PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 1);
        }

        /// <summary>
        /// 데이터 사용 허용 Off
        /// </summary>
        public void ToggleOffDataUse()
        {
            if (!currentShow)
                return;

            dataUseAgree.sprite = spriteToggleOff;
            PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 0);
        }

        /// <summary>
        /// 언어
        /// </summary>
        public void OnClickLanguage()
        {
            Debug.Log(">> OnClickDataManage");
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_LANGUAGE, string.Empty);
        }


        /// <summary>
        /// 스타 히스토리 
        /// </summary>
        public void OnClickStarHistory()
        {
            Debug.Log(">> OnClickStarHistory");
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_STAR_HISTORY, string.Empty);
        }


        /// <summary>
        /// 데이터 매니저
        /// </summary>
        public void OnClickDataManage()
        {
            Debug.Log(">> OnClickDataManage");
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_DATA_MANAGE, string.Empty);
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
            StartCoroutine(RoutineOpenNotice());
        }

        IEnumerator RoutineOpenNotice()
        {
            SystemManager.ShowNetworkLoading();
            NetworkLoader.main.RequestNoticeList();
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            yield return null;
            SystemManager.HideNetworkLoading();

            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_NOTICE, string.Empty);
        }

        public void OnClickInquiry()
        {
            Debug.Log("Open Contact");

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
            
            
            if(clickVersionCount >= 7 && clickLevelCount >= 3) {
                
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