using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RTLTMPro;

namespace PIERStory {
    
    /// <summary>
    /// 다국어를 처리해야하는 TextMeshPro에서 
    /// LocalizedText를 직접 설정받는 곳에서만 사용.
    /// </summary>
    public class TextLangFontChanger : MonoBehaviour
    {
        
        [SerializeField] TextMeshProUGUI _text = null;
        public bool isException = true;  // 한글 영어에서 기본 UI 폰트 유지할지 처리 (true일때 유지)
        
        public bool isAlignmentFix = true; // 기본값은 true
        
        public string originText = string.Empty;
        
        HorizontalAlignmentOptions originAlign = HorizontalAlignmentOptions.Center;
        
        // Start is called before the first frame update
        void Awake() {
            
            // 없으면 GetComponent해주지만, Inspector에서 설정해주는게 제일 좋다. 
            if(_text == null) 
                _text = this.GetComponent<TextMeshProUGUI>();
                
            if(_text != null) {
                originAlign = _text.horizontalAlignment;
            }
        }
        
        void Start() {
            SetFont();
        }
        
        
        /// <summary>
        /// 폰트 설정
        /// </summary>
        void SetFont() {
            if(_text == null) 
                return;
                
            if(SystemManager.main == null)
                return;
            
            // 한번 설정했으면 두번 호출할 필요없다. 
            // if(isFontSet)
            //     return; 
               
            _text.font = SystemManager.main.getCurrentLangFont(isException); // 폰트 가져오기 
        }
        
        
        /// <summary>
        /// 텍스트 변경시 이벤트 처리 
        /// </summary>
        /// <param name="obj"></param>
        void OnTextChanged(Object obj) {
            if(obj != _text) {
                return;
            }
            
            if(SystemManager.main.currentAppLanguageCode == CommonConst.COL_AR) {

                if(!TextUtils.IsRTLInput(_text.text)) {
                    SetNonArabic();
                    return;
                }
                Debug.Log(">> Arabic OnTextChanged : " + obj.name);
                
                
                // 이벤트 뺐다가 다시 넣어준다. 
                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
                SystemManager.SetArabicTextUI(_text);
                SetArabicAlignment();
                TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
            }
            else {
                SetNonArabic();
            }
            
        }
        
        public void SetNonArabic() {
            _text.isRightToLeftText = false;
            _text.horizontalAlignment = originAlign;
        }
        
        
        /// <summary>
        /// 아랍어 정렬처리 
        /// </summary>
        public void SetArabicAlignment() {
            
            // 정렬 fix가 아니고, 좌측 정렬이었으면 아랍어에서는 우측정렬로 변경한다.
            if(!isAlignmentFix && _text.horizontalAlignment == HorizontalAlignmentOptions.Left) {
                Debug.Log(">> Arabic SetArabicAlignment : " + this.gameObject.name);
                
                _text.horizontalAlignment = HorizontalAlignmentOptions.Right;
                // _text.ForceMeshUpdate();
            }
        }
        
        void OnEnable() {
            SetFont();
            // TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }
        
        void OnDisable() {
            // TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }
        
        void OnDestroy() {
            // TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

    }
}