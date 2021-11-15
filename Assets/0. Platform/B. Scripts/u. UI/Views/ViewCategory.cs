using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;
namespace PIERStory {
    public class ViewCategory : CommonView
    {
        

        public override void OnView()
        {
            base.OnView();
        }
        
        public override void OnStartView() {
            Signal.Send(LobbyConst.STREAM_IFYOU, "activateCategory", string.Empty);
        }
    }

}