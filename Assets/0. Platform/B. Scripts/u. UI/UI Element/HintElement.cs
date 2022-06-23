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
            
            EpisodeData episodeData = StoryManager.main.FindEpisode(__data["episode_id"].ToString());
            
            if(episodeData == null || !episodeData.isValidData)
                return;

            SystemManager.SetText(textHint, episodeData.episodeType == EpisodeType.Chapter ? string.Format("{0} {1:D2}", SystemManager.GetLocalizedText("5027"), episodeData.episodeNumber) : episodeData.episodeType == EpisodeType.Side ? string.Format("[{0}] {1}", SystemManager.GetLocalizedText("5028"), episodeData.episodeTitle) : string.Format("[{0}] {1}", SystemManager.GetLocalizedText("5025"), episodeData.episodeTitle));
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
            EpisodeData episodeData = StoryManager.main.FindEpisode(__episodeID);
            if(UserManager.main.IsCompleteEpisode(__episodeID)) {
                imageOn.SetActive(true);    
            }

            SystemManager.SetText(textHint, episodeData.episodeType == EpisodeType.Chapter ? string.Format("{0} {1:D2}", SystemManager.GetLocalizedText("5027"), episodeData.episodeNumber) : episodeData.episodeType == EpisodeType.Side ? string.Format("[{0}] {1}", SystemManager.GetLocalizedText("5028"), episodeData.episodeTitle) : string.Format("[{0}] {1}", SystemManager.GetLocalizedText("5025"), episodeData.episodeTitle));
        }
    }
}