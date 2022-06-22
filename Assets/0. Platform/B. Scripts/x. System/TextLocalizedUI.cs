using UnityEngine;
using TMPro;
using RTLTMPro;

namespace PIERStory {

    public class TextLocalizedUI : MonoBehaviour
    {
        
        [SerializeField] string _textID = string.Empty; // 사용할 텍스트ID 
        [SerializeField] TextMeshProUGUI _text = null;
        [SerializeField] string _localizedText = string.Empty;
        
        public bool isException = true;  // 한글 영어에서 기본 UI 폰트 유지할지 처리 (true일때 유지)
        
        public bool isAlignmentFix = true; // 기본값은 true
        
        public string originText = string.Empty;
        
        HorizontalAlignmentOptions originAlign = HorizontalAlignmentOptions.Center;
        
        void Awake() {
            
            
            // 없으면 GetComponent해주지만, Inspector에서 설정해주는게 제일 좋다. 
            if(_text) 
                _text = this.GetComponent<TextMeshProUGUI>();
                
            SetText();
        }
        
        void Start() {
            
            originAlign = _text.horizontalAlignment;
            
            SetText();    
        }
        
        void Update() {
            
            // 아랍어일때만. 
            /*
            if(SystemManager.main.currentAppLanguageCode != CommonConst.COL_AR) 
                return;
                
            // 텍스트에 변경이 일어났으면? 
            if(_text.text != originText) {
                SystemManager.SetArabicTextUI(_text);
                Debug.Log("Update L-Arabic TEXT");
                originText = _text.text;
            }
            */
        }        
        

        void SetText() {
            if(_text == null)
                return;
                
            if(SystemManager.main == null)
                return;
            
                
            // 언어별 텍스트 불러와서 할당해주기
            _localizedText = SystemManager.GetLocalizedText(_textID);
            if(!string.IsNullOrEmpty(_localizedText)) {
                
                _text.font = SystemManager.main.getCurrentLangFont(isException); // 폰트 가져오기 
                _text.text = _localizedText;
            }
            
            
            // 아랍어 처리             
            if(SystemManager.main.currentAppLanguageCode == "AR") {

                SystemManager.SetArabicTextUI(_text);
                // _text.horizontalAlignment = originAlign;
                
                // 좌측정렬이면서 정렬픽스 아닌 경우는 아랍에서 오른쪽 정렬로 변경해준다. 
                if(!isAlignmentFix && _text.horizontalAlignment == HorizontalAlignmentOptions.Left) {
                    _text.horizontalAlignment = HorizontalAlignmentOptions.Right;
                }
            }
            else {
                _text.isRightToLeftText = false;
            } 
            
            // originText에 입력해놓는다. 
            originText = _text.text;  

           
        }
        
        void OnEnable() {
            SetText();
        }
    }
}