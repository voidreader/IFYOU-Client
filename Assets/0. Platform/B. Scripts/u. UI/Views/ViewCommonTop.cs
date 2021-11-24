using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PIERStory {
    public class ViewCommonTop : CommonView
    {
        [SerializeField] GameObject backButton;
        
        public override void OnView()
        {
            base.OnView();
        }
        
        public void OnSignalControlBackButton(bool __flag) {
            backButton.SetActive(__flag);
        }
        
        
    }
}