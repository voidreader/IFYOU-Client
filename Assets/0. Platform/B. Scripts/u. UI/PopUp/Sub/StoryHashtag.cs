using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {
    public class StoryHashtag : MonoBehaviour
    {
        public TextMeshProUGUI textTag;
        
        public void Init(string __tag) {
            
            this.gameObject.SetActive(true);
            textTag.text = "#" + __tag;
        }
    }
}