using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class SearchRecordElement : MonoBehaviour
    {
        public TextMeshProUGUI recordText;

        public void InitData(string data)
        {
            recordText.text = data;
        }

        public void OnClickSearch()
        {
            ViewSearch.FindStoryForKeyword?.Invoke(recordText.text);
        }

        public void OnClickDeleteElement()
        {
            ViewSearch.DeleteRecordElement(this);
        }
    }
}
