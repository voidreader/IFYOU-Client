using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {

    public class PopupMultiResource : PopupBase
    {
        [SerializeField] ImageRequireDownload[] icon;
        [SerializeField] TextMeshProUGUI[] textQuantity;
        
        
        
        
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            
            for(int i=0; i< icon.Length; i++) {
                icon[i].gameObject.SetActive(false);
            }
            
            
            // 배열값 안맞는 경우에 대한 처리 
            if(Data.arrayContentString == null || Data.arrayContentValue == null) {
                Debug.LogError("Array Value is null");
                return;
            }
            
            if(Data.arrayContentString.Length != Data.arrayContentValue.Length) {
                Debug.LogError("Array length no match");
                return;
            }
                
            
            // * 현재 스타와 코인만 처리한다.
            for(int i=0; i<Data.arrayContentString.Length;i++) {
                icon[i].gameObject.SetActive(true);
                
                if(Data.arrayContentString[i] == LobbyConst.GEM) {
                    
                    icon[i].GetComponent<Image>().sprite = SystemManager.main.spriteStar;
                    textQuantity[i].text = Data.arrayContentValue[i].ToString();
                }
                else if(Data.arrayContentString[i] == LobbyConst.COIN) {
                    icon[i].GetComponent<Image>().sprite = SystemManager.main.spriteCoin;
                    textQuantity[i].text = Data.arrayContentValue[i].ToString();
                }
                
            }
        }
        

        
    }
}