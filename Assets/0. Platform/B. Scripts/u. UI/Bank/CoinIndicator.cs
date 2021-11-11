using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;
using Toast.Gamebase;



namespace PIERStory {

    public class CoinIndicator : MonoBehaviour {


        [SerializeField] TextMeshProUGUI _textCount = null;
        [SerializeField] RectTransform _icon = null;
        Button coinButton;

        [Header("넘버링 이펙트 연출 사용 여부")]
        [SerializeField] bool _numberingEffectUse = false;
        
        bool _isInit = false; // 초기화 완료 체크 
        
        int _currentValue = 0;
        int _nextValue = 0;

        private void Start()
        {
            if(_textCount == null)
            {
                _textCount = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            }

            UserManager.main.AddCoinIndicator(this);

            coinButton = GetComponent<Button>();

            if(coinButton != null)
                coinButton.interactable = SystemManager.main.isCoinPrizeUse;

        }

        /// <summary>
        /// 코인 refresh
        /// </summary>
        /// <param name="__newCount"></param>
        public void RefreshCoin(int __newCount)
        {
            _nextValue = __newCount;
            
            
            // effect를 사용한다고 체크를 했어도 첫 할당에서는 연출이 일어나지 않음 
            if(_numberingEffectUse 
                && this.gameObject.activeSelf 
                && _currentValue != _nextValue
                && _isInit) {
                _textCount.DOCounter(_currentValue, _nextValue, 0.2f, true, null);
                
                if(_icon != null) {
                    _icon.localScale= Vector3.one; // 크기는 조정해주고 한다. 
                    _icon.DOScale(1.2f, 0.2f).SetLoops(4, LoopType.Yoyo);
                }
                
            }
            else {
                // 천단위로 콤마 찍어주기.
                _textCount.text = string.Format("{0:#,0}", __newCount);    
                if(_icon != null) {
                    _icon.localScale= Vector3.one;
                }
            }
            
            _currentValue = _nextValue;
            
            _isInit = true;
        }
        
        public void OnClickCoin() {
            if(!SystemManager.main.isCoinPrizeUse)
                return;
                
            if(string.IsNullOrEmpty(SystemManager.main.coinPrizeURL))
                return;
                
            string param = string.Format("?uid={0}", UserManager.main.GetUserPinCode());
            string finalURL = SystemManager.main.coinPrizeURL + param;
            
            Debug.Log(finalURL);
                
            //
            GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration = new GamebaseRequest.Webview.GamebaseWebViewConfiguration();
            configuration.title = "";
            configuration.orientation = GamebaseScreenOrientation.PORTRAIT;
            configuration.colorR = 98;
            configuration.colorG = 132;
            configuration.colorB = 207;
            configuration.colorA = 255;
            configuration.barHeight = 30;
            configuration.isBackButtonVisible = false;
            // configuration.contentMode = GamebaseWebViewContentMode.MOBILE;

            
            Gamebase.Webview.ShowWebView(finalURL, configuration, (error) =>{ 
                Debug.Log("Webview Closed");
                NetworkLoader.main.RequestUserBaseProperty();
                // AppsFlyerSDK.AppsFlyer.sendEvent("USER_TICKETEVENT_SITE", null);
            }, null, null);
        }
    }
}