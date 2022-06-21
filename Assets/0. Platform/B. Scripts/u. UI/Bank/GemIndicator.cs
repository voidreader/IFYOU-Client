using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


namespace PIERStory {

    public class GemIndicator : MonoBehaviour {

        // CoinIndicator랑 똑같네... 그냥 합칠까?

        [SerializeField] TextMeshProUGUI _textCount = null;
        [SerializeField] RectTransform _icon = null;
        
        [Header("넘버링 이펙트 연출 사용 여부")]
        [SerializeField] bool _numberingEffectUse = false;
        
        
        bool _isInit = false; // 초기화 완료 체크 
        int _currentValue = 0;
        int _nextValue = 0;

        private void Start()
        {
            if (_textCount == null)
            {
                _textCount = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            }

            UserManager.main.AddGemIndicator(this);
        }

        /// <summary>
        /// 코인 refresh
        /// </summary>
        /// <param name="__newCount"></param>
        public void RefreshGem(int __newCount)
        {
            _nextValue = __newCount;
            
            
            // effect를 사용한다고 체크를 했어도 첫 할당에서는 연출이 일어나지 않음 
            if(_numberingEffectUse 
                && this.gameObject.activeSelf 
                && _currentValue != _nextValue
                && _isInit) {
                _textCount.DOCounter(_currentValue, _nextValue, 0.5f, true, null);
                
                if(_icon != null) {
                    _icon.localScale= Vector3.one; // 크기는 조정해주고 한다. 
                    _icon.DOScale(1.2f, 0.2f).SetLoops(6, LoopType.Yoyo);
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
    }
}