using UnityEngine;
using TMPro;

namespace PIERStory
{

    public class jpLocalFont : MonoBehaviour
    {

        [SerializeField] string _textID = string.Empty; // 사용할 텍스트ID 
        [SerializeField] TextMeshProUGUI _text = null;
        [SerializeField] string _localizedText = string.Empty;

        public bool isException = false;  // 한글 영어에서 기본 UI 폰트 유지할지 처리 (true일때 유지)
        public bool isTextSet = false; // 텍스트 설정되었는지 처리 

        void Awake()
        {

            isTextSet = false;

            // 없으면 GetComponent해주지만, Inspector에서 설정해주는게 제일 좋다. 
            if (_text)
                _text = this.GetComponent<TextMeshProUGUI>();

            SetText();
        }

        void Start()
        {
            SetText();
        }


        void SetText()
        {
            if (_text == null)
                return;

            if (SystemManager.main == null)
                return;

            // 한번 설정했으면 두번 호출할 필요없다. 
            // if(isTextSet)
            //     return; 

        }

        void OnEnable()
        {
            SetText();
        }
    }
}