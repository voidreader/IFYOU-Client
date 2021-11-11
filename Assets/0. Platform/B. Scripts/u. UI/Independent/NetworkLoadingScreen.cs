using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace PIERStory {

    public class NetworkLoadingScreen : MonoBehaviour {



        [SerializeField] Image _overlay;
        [SerializeField] GameObject _animatedIcon;
        // [SerializeField] Transform _icon;

       
        /// <summary>
        /// 네트워크 기다림!
        /// </summary>
        public void ShowNetworkLoading()
        {
            // 네트워크 로딩 화면은 바로 등장시키지 않고, 약간의 텀을 두고 나오도록 한다. 
            // 빠른 통신의 경우 굳이 이 화면을 노출할 필요는 없다. 
            Debug.Log("[[NetworkLoading]]");
            _animatedIcon.gameObject.SetActive(false);
            _overlay.DOKill();
            /*
            _icon.DOKill();
            _icon.gameObject.SetActive(false);
            _icon.localEulerAngles = new Vector3(0, 0, -10);
            */

            _overlay.color = new Color(0, 0, 0, 0);
            
            this.gameObject.SetActive(true);
            _overlay.DOFade(0.4f, 1f).SetDelay(0.5f).OnComplete(OnStartShow); // 딜레이 1초 
        }

        /// <summary>
        /// 등장 후, 시계 처리 
        /// </summary>
        void OnStartShow()
        {
            /*
            _icon.gameObject.SetActive(true);
            _icon.DOLocalRotate(new Vector3(0, 0, 10), 1, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
            */
            
            _animatedIcon.gameObject.SetActive(true);
        }

        /// <summary>
        /// 끄기!
        /// </summary>
        public void OffNetworkLoading()
        {
            _animatedIcon.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}