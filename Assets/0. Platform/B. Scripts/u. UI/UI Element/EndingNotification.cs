using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PIERStory {

    public class EndingNotification : MonoBehaviour
    {
        public EpisodeData targetEpisode;
        
        public ImageRequireDownload episodeBanner;
        public TextMeshProUGUI textTitle;
        
        Action OnClick = null;
    
        
        
                
        public void SetEndingNotification(EpisodeData __target, Action __click) {
            
            this.gameObject.SetActive(true);
            
            targetEpisode = __target;
            episodeBanner.SetDownloadURL(targetEpisode.popupImageURL, targetEpisode.popupImageKey);
            textTitle.text = targetEpisode.episodeTitle;
            
            OnClick = __click;
        }
        
        public void OnClickEnding() {
            
            Debug.Log("OnClickEnding");
            
            OnClick?.Invoke();
        }
        
    }
}