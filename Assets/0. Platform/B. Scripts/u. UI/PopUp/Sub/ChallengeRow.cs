using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PIERStory {
    public class ChallengeRow : MonoBehaviour
    {
        public ChallengeData challengeData; 
        public EpisodeData targetEpisode; // 연결되는 에피소드 
        
        
        [Header("중앙 에피소드 정보")]
        public Image clearEpisodeBG;
        public TextMeshProUGUI textEpisode;
        public TextMeshProUGUI textClearEpisode;
        
        [Header("컬럼 정보")]
        public ChallengeCol basicCol;
        public ChallengeCol premiumCol;
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__data"></param>
        public void SetChallengeRow(ChallengeData __data) {
            // 연결되는 에피소드 찾기
            targetEpisode = StoryManager.GetRegularEpisodeByNumber(__data.chapterNumber);
            
            if(targetEpisode == null || !targetEpisode.isValidData) {
                Debug.LogError("Wrong challenge chapter");
                return;
            }
            //  "EP" + string.Format("{0:D2}", episodeNumber);
            textEpisode.text = "EP\n" + string.Format("{0:D2}", targetEpisode.episodeNumber);
            textClearEpisode.text = textEpisode.text;
            
            basicCol.SetChallenge(__data, targetEpisode);
            premiumCol.SetChallenge(__data, targetEpisode, true);
            
            this.gameObject.SetActive(true);
            clearEpisodeBG.gameObject.SetActive(targetEpisode.isClear);
            
        }
        
        
        
    }
}