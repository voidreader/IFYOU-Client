using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PIERStory {

    public class PopupSpecialHint : PopupBase
    {
        
        public GameObject hintPrefab;
        public VerticalLayoutGroup hintListContent;
        
        public EpisodeData currentSpecial; // 스페셜 에피소드 데이터 
    
        
        public override void Show() {
            if (isShow)
                return;

            base.Show();
            
            currentSpecial = Data.contentEpisode;
            if(!currentSpecial.isValidData) {
                Debug.LogError("잘못된 스페셜 에피소드 데이터");
                return;
            }
            

            for (int i=0; i<currentSpecial.sideHintData.Count;i++)  {
                
                HintElement hintElement = Instantiate(hintPrefab, hintListContent.transform).GetComponent<HintElement>();
                hintElement.transform.localScale = Vector3.one;
                
                // 언락 스타일에 따라 다름 
                if(currentSpecial.unlockStyle == "episode")
                    hintElement.InitEpisodeHint(currentSpecial.sideHintData[i].ToString());
                else 
                    hintElement.InitSceneHint(currentSpecial.sideHintData[i]);
                
            }
            
        }
    }
}