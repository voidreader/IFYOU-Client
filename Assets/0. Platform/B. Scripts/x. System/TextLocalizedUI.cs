using UnityEngine;
using TMPro;

namespace PIERStory {

    public class TextLocalizedUI : MonoBehaviour
    {
        
        [SerializeField] string _textID = string.Empty; // 사용할 텍스트ID 
        [SerializeField] TextMeshProUGUI _text = null;
        [SerializeField] string _localizedText = string.Empty;
        
        void Awake() {
            // 없으면 GetComponent해주지만, Inspector에서 설정해주는게 제일 좋다. 
            if(_text) 
                _text = this.GetComponent<TextMeshProUGUI>();
                
            SetText();
        }
        
        // Start is called before the first frame update
        void Start()
        {
            // Debug.Log("TextLocalizedUI Start Called");
            

        }
        
        void SetText() {
            if(_text == null)
                return;
                
            // 언어별 텍스트 불러와서 할당해주기
            _localizedText = SystemManager.GetLocalizedText(_textID);
            if(!string.IsNullOrEmpty(_localizedText)) {
                _text.text = _localizedText;
            }
            
        }
        
        void OnEnable() {
            SetText();
        }
    }
}