using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

namespace PIERStory {

    public class IntroMessage : MonoBehaviour
    {
        public ImageRequireDownload imagePortrait; // 초상화 이미지
        public Image imageTag; // 네임태그 
        public Image imageTagTriangle; 
        public Image imageBackground; // 백그라운드
        
        public TextMeshProUGUI textCharacterMessage; // 캐릭터 메세지
        public TextMeshProUGUI textPublicMessage; // 퍼블릭 메세지 
        
        public Color mainColor;
        public int connectedProjectID = -1; // 연결 프로젝트 
        
        PopupIntro basePopup = null;

        
        /// <summary>
        /// 초기화 설정하기 
        /// </summary>
        /// <param name="__j"></param>
        public void Init(JsonData __j, PopupIntro __base) {
            
            basePopup = __base;
            
            // 메인 색상 설정 
            ColorUtility.TryParseHtmlString("#" + SystemManager.GetJsonNodeString(__j, "color_rgb"), out mainColor);
            imageTag.color = mainColor;
            imageTagTriangle.color = mainColor;
            imageBackground.color = mainColor;
            
            // 초상화 이미지 
            imagePortrait.SetDownloadURL(SystemManager.GetJsonNodeString(__j, "image_url"), SystemManager.GetJsonNodeString(__j, "image_key"));
            
            textCharacterMessage.text = SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, "character_msg"));
            textPublicMessage.text = SystemManager.GetLocalizedText(SystemManager.GetJsonNodeString(__j, "public_msg"));
            
            connectedProjectID = SystemManager.GetJsonNodeInt(__j, "connected_project_id");
            
            this.gameObject.GetComponent<CanvasGroup>().alpha = 0; // 투명하게 해놓는다.
            
        }
        
        /// <summary>
        /// 메세지 클릭하면? 
        /// </summary>
        public void OnClickMessage() {
            Debug.Log("OnClick IntroMessage : " + connectedProjectID);
            
            StoryData story = StoryManager.main.FindProject(connectedProjectID.ToString());
            // Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE, story, "recommend");
            
            

            
            // PopupManager.main.HideActivePopup();
            
            // 선택 완료
            basePopup.SelectIntroMessage(story);
            
        }
        
        
        
    }
}