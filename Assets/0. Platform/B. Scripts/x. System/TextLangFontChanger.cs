using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {
    public class TextLangFontChanger : MonoBehaviour
    {
        
        [SerializeField] TextMeshProUGUI _text = null;
        public bool isException = true;  // 한글 영어에서 기본 UI 폰트 유지할지 처리 (true일때 유지)
        public bool isFontSet = false; // 텍스트 설정되었는지 처리 
        
        // Start is called before the first frame update
        void Awake() {
            
            isFontSet = false;
            
            // 없으면 GetComponent해주지만, Inspector에서 설정해주는게 제일 좋다. 
            if(_text == null) 
                _text = this.GetComponent<TextMeshProUGUI>();
        }
        
        void SetFont() {
            if(_text == null)
                return;
                
            if(SystemManager.main == null)
                return;
            
            // 한번 설정했으면 두번 호출할 필요없다. 
            if(isFontSet)
                return; 
                
            _text.font = SystemManager.main.getCurrentLangFont(isException); // 폰트 가져오기 
    
        }
        
        void OnEnable() {
            SetFont();
        }

    }
}