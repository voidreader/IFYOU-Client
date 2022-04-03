using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class ViewSpecialEpisode : CommonView
    {
        public List<SpecialEpisodeElement> ListSpecialEpisodes;
        public TextMeshProUGUI textProgress; // 수집율
        
        
        public Doozy.Runtime.Reactor.Progressor progressor;
        public float currentProgressValues = 0;
        
        public override void OnStartView() {
            base.OnStartView();
            
            // 상단 처리
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("6233"), string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
            
            
            InitSpecialEpisode();
            
            
            // 진행도 처리 
            int totalSpecialEpisodeCount = StoryManager.main.SideEpisodeList.Count;
            int openSpecialEpisodeCount = StoryManager.main.SideEpisodeList.Count( ep => ep.isUnlock);
            
            if(totalSpecialEpisodeCount == 0 || openSpecialEpisodeCount == 0)
                currentProgressValues = 0;
            else {
                currentProgressValues = (float)openSpecialEpisodeCount / (float)totalSpecialEpisodeCount;
            }    
            
            progressor.SetProgressAt(currentProgressValues);
            
            // 수집율 
            textProgress.text = string.Format(SystemManager.GetLocalizedText("6138"), openSpecialEpisodeCount, totalSpecialEpisodeCount);
        }
        
        public override void OnHideView() {
            base.OnHideView();
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);            
        }
        
        
        void InitSpecialEpisode() {
            for(int i=0; i<ListSpecialEpisodes.Count;i++ ) {
                ListSpecialEpisodes[i].gameObject.SetActive(false);
            }
            
            if(StoryManager.main.sideEpisodeCount > ListSpecialEpisodes.Count) {
                Debug.LogError("Too many special episode");
                return;
            }
            
            
            for(int i=0; i<StoryManager.main.SideEpisodeList.Count;i++) {
                
                ListSpecialEpisodes[i].InitSpecialEpisode(StoryManager.main.SideEpisodeList[i]);
            }
        }
    }
}