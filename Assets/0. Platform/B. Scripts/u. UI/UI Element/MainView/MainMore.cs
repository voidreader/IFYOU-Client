using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Toast.Gamebase;
using Doozy.Runtime.Signals;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory {

    public class MainMore : MonoBehaviour
    {
        public static System.Action OnRefreshMore = null;

        [SerializeField] GameObject accountBonus;

        public Image pushAlert;                 // 푸쉬 알림
        public Image nightPushAlert;            // 야간 푸쉬 알림
        public Image dataUseAgree;              // 데이터 사용 허용
        public UIToggle pushToggle;
        public UIToggle nightPushToggle;
        public UIToggle dataUseToggle;

        public UIAnimator nightPushAnimator;

        public Sprite spriteToggleOn;
        public Sprite spriteToggleOff;

        bool currentShow = false;

        void Start()
        {
            OnRefreshMore = RefreshScreen;
        }

        private void OnEnable()
        {
            RefreshScreen();
        }

        public void ShowMainMore()
        {
            currentShow = true;

            // 데이터 사용 허용
            if (PlayerPrefs.GetInt(SystemConst.KEY_NETWORK_DOWNLOAD) > 0)
                dataUseToggle.isOn = true;
            else
                dataUseToggle.isOn = false;
        }

        public void HideMainMore()
        {
            currentShow = false;
        }

        void RefreshScreen()
        {
            accountBonus.SetActive(false);

            if (UserManager.main == null || string.IsNullOrEmpty(UserManager.main.userKey))
                return;

            if (UserManager.main.accountLink == "-")
                accountBonus.SetActive(true);


            #region 게임베이스 push

            if (Application.isEditor)
                return;


            Gamebase.Push.QueryTokenInfo((data, error) =>
            {
                if (Gamebase.IsSuccess(error))
                {
                    pushToggle.isOn = data.agreement.adAgreement;
                    nightPushToggle.isOn = data.agreement.adAgreementNight;

                    Debug.Log(string.Format("TokenInfo = pushAlert : {0}, nightPush : {1}", data.agreement.adAgreement, data.agreement.adAgreementNight));
                }
                else
                    Debug.LogError(string.Format("QueryToken response failed. Error : {0}", error));
            });

            

            #endregion

            if (pushToggle.isOn)
                pushToggle.OnToggleOnCallback.Execute();
            else
                pushToggle.OnToggleOffCallback.Execute();


            if (nightPushToggle.isOn)
                nightPushToggle.OnToggleOnCallback.Execute();
            else
                nightPushToggle.OnToggleOffCallback.Execute();

            /*
            if (dataUseToggle.isOn)
                dataUseToggle.OnToggleOnCallback.Execute();
            else
                dataUseToggle.OnToggleOffCallback.Execute();
            */
        }


        /// <summary>
        /// 푸쉬 알림 On
        /// </summary>
        public void ToggleOnPushAlert()
        {
            pushAlert.sprite = spriteToggleOn;
            nightPushAlert.color = Color.white;
            SystemManager.main.PushRegister(pushToggle.isOn, nightPushToggle.isOn);
        }

        /// <summary>
        /// 푸쉬 알림 OFf
        /// </summary>
        public void ToggleOffPushAlert()
        {
            pushAlert.sprite = spriteToggleOff;

            if(nightPushToggle.isOn)
                nightPushToggle.isOn = false;

            nightPushAlert.color = new Color32(64, 64, 64, 255);

            SystemManager.main.PushRegister(pushToggle.isOn, false);
        }


        /// <summary>
        /// 야간 푸쉬 알림 On
        /// </summary>
        public void ToggleOnNightPushAlert()
        {
            // 푸쉬 알림이 켜져있지 않으면 잠군다
            if (!pushToggle.isOn)
            {
                SystemManager.ShowSimpleAlertLocalize("6033");
                return;
            }

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

        }

        public void OnClickPrivacy()
        {

        }

        public void OnClickTermsOfUse()
        {

        }

        public void OnClickLeave()
        {

        }

        /// <summary>
        /// 닉네임 체인지 
        /// </summary>
        public void OnClickNicknameChange()
        {

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
        }
    }
}