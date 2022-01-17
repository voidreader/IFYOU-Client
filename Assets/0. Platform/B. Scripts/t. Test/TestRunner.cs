using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LitJson;

namespace PIERStory {
    public class TestRunner : MonoBehaviour
    {
        static readonly  Vector2 originSizeDelta = new Vector2(600, 80);
        static readonly  Vector2 selectedSizeDelta = new Vector2(600, 80);
        
        public List<IFYouGameSelectionCtrl> ListSelection;
        
        
        
        RectTransform baseTransform; // 
        
        // Start is called before the first frame update
        void Start()
        {
            PopupManager.main.InitPopupManager();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q)) {
                ShowSelectionTEST();
            }
            
            if(Input.GetKeyDown(KeyCode.P)) { 
                /*
                JsonData data = new JsonData();
                data["current"] = JsonMapper.ToObject("{}");
                data["before"] = JsonMapper.ToObject("{}");
                
                data["current"]["level"] = 3;
                data["current"]["experience"] = 7;
                
                data["before"]["level"] = 2;
                data["before"]["experience"] = 7;
                data["before"]["get_experience"] = 10;
                
                PopupBase p =  PopupManager.main.GetPopup("EXP");
                p.Data.SetContentJson(data);
                
                PopupManager.main.ShowPopup(p, false, false);
                */
                PopupBase popup = PopupManager.main.GetPopup(SystemConst.POPUP_ILLUST_ACHIEVEMENT);
                PopupManager.main.ShowPopup(popup, true, false);                
                
            }
        }
        
        public void FillSelection() {
            baseTransform.DOSizeDelta(selectedSizeDelta, 0.4f);
        }
        
        void ShowSelectionTEST() {
            for(int i=0; i<ListSelection.Count; i++) {
                ListSelection[i].SetSelection(i);
            }
        }
    }
}

