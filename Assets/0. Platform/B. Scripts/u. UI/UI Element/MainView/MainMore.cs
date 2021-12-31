using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

namespace PIERStory {

    public class MainMore : MonoBehaviour
    {
        public void OnClickLanguage() {
            Signal.Send(LobbyConst.STREAM_IFYOU, "language", string.Empty);
        }
    }
}