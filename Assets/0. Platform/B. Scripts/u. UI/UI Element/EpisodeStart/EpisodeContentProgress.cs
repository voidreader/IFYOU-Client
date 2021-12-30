using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PIERStory {

    public class EpisodeContentProgress : MonoBehaviour
    {
        [SerializeField] Image progressor;
        [SerializeField] TextMeshProUGUI textPercent;
        
        [SerializeField] float currentValue = 0;
        [SerializeField] int roundedValue = 0;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__v"></param>
        public void SetProgress(float __v) {
            
            // 임시 로직.. 언제 나오니 Progressor
            currentValue = __v;
            
            progressor.fillAmount = currentValue;
            
            roundedValue = Mathf.RoundToInt(currentValue * 100);
            if(roundedValue > 100)
                roundedValue = 100;
                
            textPercent.text =  roundedValue.ToString() + "%";
        }
    }
}