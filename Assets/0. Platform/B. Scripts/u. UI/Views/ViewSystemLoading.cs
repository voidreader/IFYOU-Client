using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {
    public class ViewSystemLoading : CommonView
    {
        public static bool needRefreshStory = false;
        
        public override void OnView()
        {
            base.OnView();
            
            if(needRefreshStory && SystemManager.main != null && StoryManager.main != null) {
                
                Debug.Log("Refresh Story from Game ####");
                StoryManager.main.RequestStoryInfo(SystemManager.main.givenStoryData);
                
                needRefreshStory = false;
            }
            
        }
    }
}