using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {

    public class MainMore : MonoBehaviour
    {
        
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
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_COUPON, string.Empty);
        }
        
        /// <summary>
        /// 공지사항
        /// </summary>
        public void OnClickNotice() {
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
        
    }
}