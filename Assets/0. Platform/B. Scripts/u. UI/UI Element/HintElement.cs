using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LitJson;

namespace PIERStory {
    public class HintElement : MonoBehaviour
    {
        public GameObject imageOn;
        public TextMeshProUGUI textHint;
        
        void InitControls() {
            textHint.text = string.Empty;
            imageOn.SetActive(false);
        }
        
        /// <summary>
        /// 사건ID 잠금해제 스타일 
        /// </summary>
        /// <param name="__data"></param>
        /// <param name="isSceneUnlock"></param>
        public void InitSceneHint(JsonData __data) {
            
            InitControls();
            
            EpisodeData episodeData = StoryManager.main.FindRegularEpisode(__data["episode_id"].ToString());
            
            if(episodeData == null || !episodeData.isValidData)
                return;
         
            textHint.text = SystemManager.GetLocalizedText("5027") + " " + string.Format("{0:D2}", episodeData.episodeNumber);
            textHint.text += "      " + SystemManager.GetJsonNodeString(__data, "played") +"/" + SystemManager.GetJsonNodeString(__data, "total");
            
            if(SystemManager.GetJsonNodeString(__data, "played") == SystemManager.GetJsonNodeString(__data, "total")) {
                imageOn.SetActive(true);
            }
               
        }
        
        
        /// <summary>
        /// 에피소드 잠금해제 스타일
        /// </summary>
        /// <param name="__episodeID"></param>
        public void InitEpisodeHint(string __episodeID) {
            
            InitControls();
            
            // * 에피소드 데이터 찾아와서 번호 세팅 
            EpisodeData episodeData = StoryManager.main.FindRegularEpisode(__episodeID);
            if(UserManager.main.IsCompleteEpisode(__episodeID)) {
                imageOn.SetActive(true);    
            }
            
            
            textHint.text = SystemManager.GetLocalizedText("5027") + " " + string.Format("{0:D2}", episodeData.episodeNumber);
  
        }
    }
}