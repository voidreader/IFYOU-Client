using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PIERStory {
    public class PopupManager : SerializedMonoBehaviour
    {
        public static PopupManager main = null;
        
        public Canvas popupCanvas = null;
        
        public Dictionary<string, GameObject> DictPopup;
        
        private void Awake() {
            if(main != null) {
                Destroy(this.gameObject);
                return;
            }
            
            main = this;
            DontDestroyOnLoad(this);
                
        }
        
        /// <summary>
        /// 팝업 생성 및 받아오기 
        /// </summary>
        /// <param name="popupName"></param>
        /// <returns></returns>
        public PopupBase GetPopup(string popupName) {
            if(!DictPopup.ContainsKey(popupName))
                return null;
                
            // 생성될 캔버스 처리 
            if(!popupCanvas) {
                popupCanvas = GameObject.FindGameObjectWithTag("PopupCanvas").GetComponent<Canvas>();
            }
            
            GameObject clone = Instantiate(DictPopup[popupName], popupCanvas.transform);
            return clone.GetComponent<PopupBase>();
        }
        
        
    }
}