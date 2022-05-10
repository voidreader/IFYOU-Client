using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PIERStory {
public class PopupDevRowList : PopupBase
{
    
    public GameObject prefabRow;
    
    public int devRowIndex = 0;
    public List<DevRowCtrl> listDevRows;
    
    
    public override void Show() {
        if(isShow)
            return;
            
        base.Show();
        
        ScriptPage page = GameManager.main.currentPage;
        
        for(int i=0; i<page.ListRows.Count;i++) {
            
            // 선택지로 돌아갈 수 없음 
            if(page.ListRows[i].template == "selection")
                continue;
            
            
            // 신규 사건 ID 등장 
            if(!string.IsNullOrEmpty(page.ListRows[i].scene_id)) {
                MakeDevRow(page.ListRows[i]);
                continue;
            }
            
            // 배경 템플릿마다 
            if(page.ListRows[i].template == "background" || page.ListRows[i].template == "move_in") {
                MakeDevRow(page.ListRows[i]);
                continue;
            }
            
            
        }
        
        
    }
    
    void MakeDevRow(ScriptRow __row) {
        listDevRows[devRowIndex].InitDevRow(__row);
        
    }    
    
    
    public override void Hide() {
        
    }
    
}
}