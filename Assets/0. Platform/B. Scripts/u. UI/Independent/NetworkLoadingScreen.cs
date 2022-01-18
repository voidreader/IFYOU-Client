using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace PIERStory {

    public class NetworkLoadingScreen : MonoBehaviour {



        [SerializeField] Image _overlay;
        [SerializeField] Image _icon;
        [SerializeField] TextMeshProUGUI _textLoading;

       
        /// <summary>
        /// 네트워크 기다림!
        /// </summary>
        public void ShowNetworkLoading()
        {
            // 이미 보여지고 있으면 또 하지 않음. 
            if(this.gameObject.activeSelf)
                return; 
            
            // 네트워크 로딩 화면은 바로 등장시키지 않고, 약간의 텀을 두고 나오도록 한다. 
            // 빠른 통신의 경우 굳이 이 화면을 노출할 필요는 없다. 
            Debug.Log("[[NetworkLoading]]");
            
            _icon.transform.localScale = Vector3.one;
            _icon.color = CommonConst.COLOR_IMAGE_TRANSPARENT;
            
            _icon.DOKill();
            _icon.transform.DOKill();
            _overlay.DOKill();
            _overlay.color = new Color(0, 0, 0, 0);

            
            // _textLoading.DOKill();
            // 텍스트 로딩 변경 
            _textLoading.color = CommonConst.COLOR_IMAGE_TRANSPARENT;
            _textLoading.text = string.Empty;
            
            
            this.gameObject.SetActive(true);
            _overlay.DOFade(0.7f, 1f).SetDelay(0.5f).OnComplete(OnStartShow); // 딜레이 1초 
        }

        /// <summary>
        /// 등장 후, 시계 처리 
        /// </summary>
        void OnStartShow()
        {
            _icon.gameObject.SetActive(true);
            
            
            _textLoading.text = "Loading...";
            _textLoading.DOFade(1, 0.4f);
            
            
            // 아이콘 둥둥 
            _icon.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            _icon.DOFade(1, 0.4f);
            
        }
        


        /// <summary>
        /// 끄기!
        /// </summary>
        public void OffNetworkLoading()
        {
            _icon.transform.localScale = Vector3.one;
            _icon.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
            _textLoading.text = string.Empty;
        }
    }
}