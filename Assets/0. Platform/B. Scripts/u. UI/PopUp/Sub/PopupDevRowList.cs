using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PIERStory {
public class PopupDevRowList : PopupBase
{
    
    public GameObject prefabRow;
    
    public override void Show() {
        if(isShow)
            return;
            
        base.Show();
    }
    
    
    
    public override void Hide() {
        
    }
    
}
}