using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {

    public class PopupResource : PopupBase
    {
        [SerializeField] ImageRequireDownload icon;
        [SerializeField] TextMeshProUGUI textQuantity;
        
        [SerializeField] GameObject groupConfirm;
        [SerializeField] GameObject groupOK;
        
        
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            
            
            // 아이콘을 다운로드 해야하는 경우 
            if(!string.IsNullOrEmpty(Data.imageURL)) {
                
                icon.GetComponent<RectTransform>().sizeDelta = new Vector2(100,100);
                // 아이콘 처리
                icon.SetDownloadURL(Data.imageURL, Data.imageKey);
            }
            
            // 수량과 메세지는 앞단에서 처리됨 
            
            
            // 버튼 제어 
            groupConfirm.SetActive(Data.isConfirm);
            groupOK.SetActive(!Data.isConfirm);
            
            
        }
        
        /// <summary>
        /// 뱅킹정보 리프레시 
        /// </summary>
        public void OnClickOK() {
            // UserManager.main.SetBankInfo(Data.contentJson); // 뱅킹정보 리프레시 
        }
        
        /*
        public void OnClickPositive() {
            Data.positiveButtonCallback?.Invoke();
        }
        */
    }
}