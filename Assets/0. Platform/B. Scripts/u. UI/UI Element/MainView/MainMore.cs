using System.Collections;
using UnityEngine;

using Doozy.Runtime.Signals;

namespace PIERStory {

    public class MainMore : MonoBehaviour
    {
        public static System.Action OnRefreshMore = null;
        
        [SerializeField] GameObject accountBonus;
        
        void Start() {
            OnRefreshMore = RefreshScreen;
        }
        
        void OnEnable() {
            RefreshScreen();
        }
        
        void RefreshScreen() {
            
            accountBonus.SetActive(false);
            
            if(UserManager.main == null || string.IsNullOrEmpty(UserManager.main.userKey))
                return;
            
            if(UserManager.main.accountLink == "-")
                accountBonus.SetActive(true);
        }
        
        /// <summary>
        /// 언어
        /// </summary>
        public void OnClickLanguage() {
            Debug.Log(">> OnClickDataManage");
            
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_LANGUAGE, string.Empty);
        }
        
        
        
        /// <summary>
        /// 스타 히스토리 
        /// </summary>
        public void OnClickStarHistory() {
            
            Debug.Log(">> OnClickStarHistory");
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_STAR_HISTORY, string.Empty);
        }
        
        
        /// <summary>
        /// 데이터 매니저
        /// </summary>
        public void OnClickDataManage() {
            
            Debug.Log(">> OnClickDataManage");
            
            
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_DATA_MANAGE, string.Empty);
        }
        
        /// <summary>
        /// 쿠폰
        /// </summary>
        public void OnClickCoupon() {
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
        
        public void OnClickInquiry() {
            
        }
        
        public void OnClickPrivacy() {
            
        }
        
        public void OnClickTermsOfUse() {
            
        }
        
        public void OnClickLeave() {
            
        }
        
        /// <summary>
        /// 닉네임 체인지 
        /// </summary>
        public void OnClickNicknameChange() {
            
        }
        
        /// <summary>
        /// 계정 연결 
        /// </summary>
        public void OnClickAccount() {
            PopupBase p = PopupManager.main.GetPopup("Account");
            if( p == null) {
                Debug.LogError("No account popup");
                return;
            }
            
            PopupManager.main.ShowPopup(p, true);
        }
        
        
    }
}