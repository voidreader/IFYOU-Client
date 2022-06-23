using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PIERStory {
    
    
    /// <summary>
    /// 능력치 획득 알림. 
    /// </summary>
    public class PopupGameAbility : PopupBase
    {
        public ImageRequireDownload emoticon;
        public ImageRequireDownload abilityIcon;
        public TextMeshProUGUI textAbility;
        public string abilityLocalID = string.Empty;
        public int addValue = 0;
        public string textValue = string.Empty;
        public Image body;
        
        public Sprite spPositiveBody;
        public Sprite spNegativeBody;
        
        public override void Show() {
            if(isShow)
                return;
                
            base.Show();
            
           
            // 이모티콘, 어빌리티 처리 
            emoticon.SetDownloadURL(SystemManager.GetJsonNodeString(Data.contentJson, "emoticon_design_url"), SystemManager.GetJsonNodeString(Data.contentJson, "emoticon_design_key"));
            abilityIcon.SetDownloadURL(SystemManager.GetJsonNodeString(Data.contentJson, "icon_design_url"), SystemManager.GetJsonNodeString(Data.contentJson, "icon_design_key"));
            
            abilityLocalID = SystemManager.GetJsonNodeString(Data.contentJson, "local_id");
            
            
            if(Data.contentValue > 0) {
                textValue = "+" + Data.contentValue.ToString();
                body.sprite = spPositiveBody;
            }
            else {
                textValue = Data.contentValue.ToString();
                body.sprite = spNegativeBody;
            }
            
            // 텍스트 처리
            SystemManager.SetText (textAbility, SystemManager.GetLocalizedText(abilityLocalID) + " " + textValue);
            // textAbility.text = SystemManager.GetLocalizedText(abilityLocalID) + " " + textValue;
        }
    }
}