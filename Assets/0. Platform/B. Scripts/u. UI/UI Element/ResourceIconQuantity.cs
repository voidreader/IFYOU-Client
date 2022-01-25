using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {
    public class ResourceIconQuantity : MonoBehaviour
    {
        
        [SerializeField] TextMeshProUGUI textQuantity;
        [SerializeField] ImageRequireDownload icon;
        
        
        /// <summary>
        /// 리소스 아이콘 + 개수 페어 
        /// </summary>
        /// <param name="__url"></param>
        /// <param name="__key"></param>
        /// <param name="__quantity"></param>
        public void SetResourceInfo(string __url, string __key, int __quantity) {
            
            this.gameObject.SetActive(true);
            
            icon.SetDownloadURL(__url, __key);
            
            textQuantity.text = __quantity.ToString();
        }
    }
}