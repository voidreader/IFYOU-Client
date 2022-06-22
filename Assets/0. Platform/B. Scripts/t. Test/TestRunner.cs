using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LitJson;

using TMPro;
using RTLTMPro;

namespace PIERStory {
    public class TestRunner : MonoBehaviour
    {
        static readonly  Vector2 originSizeDelta = new Vector2(600, 80);
        static readonly  Vector2 selectedSizeDelta = new Vector2(600, 80);
        
        public List<IFYouGameSelectionCtrl> ListSelection;
        
        public TextMeshProUGUI textArabic;
        
        
        RectTransform baseTransform; // 
        protected readonly FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);
        
        // Start is called before the first frame update
        void Start()
        {
            PopupManager.main.InitPopupManager();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q)) {
                // ShowSelectionTEST();
                // textArabic.isRightToLeftText = true;
                // textArabic.alignment = TextAlignmentOptions.TopRight;
                // textArabic.text = ArabicFixer.Fix(textArabic.text, false, false);
                string originalText = textArabic.text;
                
                /*
                if(!TextUtils.IsRTLInput(originalText)) {
                    Debug.Log("No Arabic TEXT");
                    return; // 아랍 텍스트 없으면 진행하지 않음. 
                }
                */
                
                textArabic.isRightToLeftText = true;
                
                finalText.Clear();
                RTLSupport.FixRTL(originalText, finalText, false, true, true);
                finalText.Reverse();
                textArabic.text = finalText.ToString();
                
                
                
                
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
                PopupBase popup = PopupManager.main.GetPopup(GameConst.POPUP_ACHIEVEMENT_ILLUST);
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

