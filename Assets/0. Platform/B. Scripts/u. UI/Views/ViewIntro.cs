using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory {

    public class ViewIntro : CommonView
    {
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
            step1.Show();
        }
        
        public override void OnHideView() {
            base.OnHideView();
        }
    }
}