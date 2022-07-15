using UnityEngine;
using TMPro;

namespace PIERStory
{

    public class jpLocalFont : MonoBehaviour
    {

        [SerializeField] string _textID = string.Empty; // ����� �ؽ�ƮID 
        [SerializeField] TextMeshProUGUI _text = null;
        [SerializeField] string _localizedText = string.Empty;

        public bool isException = false;  // �ѱ� ����� �⺻ UI ��Ʈ �������� ó�� (true�϶� ����)
        public bool isTextSet = false; // �ؽ�Ʈ �����Ǿ����� ó�� 

        void Awake()
        {

            isTextSet = false;

            // ������ GetComponent��������, Inspector���� �������ִ°� ���� ����. 
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

            // �ѹ� ���������� �ι� ȣ���� �ʿ����. 
            // if(isTextSet)
            //     return; 

        }

        void OnEnable()
        {
            SetText();
        }
    }
}