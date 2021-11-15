using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {
    
    public class ViewShop : CommonView
    {
        // Start is called before the first frame update
        public override void OnView()
        {
            base.OnView();
        }
        
        public override void OnStartView() {
            Signal.Send(LobbyConst.STREAM_IFYOU, "activateShop", string.Empty);
        }
    }
}