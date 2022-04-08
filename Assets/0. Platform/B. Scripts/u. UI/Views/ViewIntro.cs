using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory {

    public class ViewIntro : CommonView
    {
        public UIContainer step0;
        public UIContainer step1;
        public UIContainer step2;
        public UIContainer step3_1;
        public UIContainer step3_2;
        
        
        
        public override void OnStartView()
        {
            base.OnStartView();
        }
        
        public override void OnView() {
            base.OnView();
            step0.Show();
        }
        
        public override void OnHideView() {
            base.OnHideView();
        }
        
        
        /// <summary>
        /// 인트로에서 선택된 스토리 처리
        /// </summary>
        /// <param name="__projectID"></param>
        public void ChooseIntroStory(int __projectID) {
            StoryData story = StoryManager.main.FindProject(__projectID.ToString());
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE, story, "recommend");
            
            
            UserManager.main.UpdateIntroComplete();
        }
    }
}