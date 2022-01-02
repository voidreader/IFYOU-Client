using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


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
            
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F)) {
                
            }
        }
        
        public void FillSelection() {
            baseTransform.DOSizeDelta(selectedSizeDelta, 0.4f);
        }
    }
}

