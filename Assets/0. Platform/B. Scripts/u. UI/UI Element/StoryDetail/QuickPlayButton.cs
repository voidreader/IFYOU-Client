using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using DG.Tweening;


namespace PIERStory {
    public class QuickPlayButton : MonoBehaviour
    {
        
        [SerializeField] GameObject bubble;
        
        
        [SerializeField] TextMeshProUGUI textNo;
        [SerializeField] TextMeshProUGUI textTitle;
        [SerializeField] EpisodeData quickEpisode; // 빠른플레이 대상 에피소드
        [SerializeField] string targetEpisodeID; // 대상 에피소드의 ID 
        [SerializeField] bool isRegular = true; // 정규야 사이드야?
        
        [SerializeField] Transform iconParent; // 아이콘 트랜스폼 
        
        void InitControls() {
            bubble.SetActive(true);
            
            iconParent.DOKill();
            iconParent.localPosition = Vector3.zero;
        }
        
        
        /// <summary>
        /// 사이드 퀵플레이 설정 
        /// </summary>
        /// <param name="quickData"></param>
        public void SetSideQuickPlay(JsonData quickData) {
            InitControls();
            
            targetEpisodeID = SystemManager.GetJsonNodeString(quickData, LobbyConst.STORY_EPISODE_ID);
            quickEpisode = StoryManager.GetNextFollowingEpisodeData(targetEpisodeID);
            
            if(quickEpisode == null || !quickEpisode.isValidData) {
                this.gameObject.SetActive(false);
                return;
            }
            
            textNo.text = SystemManager.GetLocalizedText("5028");
            textTitle.text = quickEpisode.episodeTitle;
            
            this.gameObject.SetActive(true);
            isRegular = false;
        }
        
        /// <summary>
        /// 정규 에피소드의 퀵 플레이 설정 
        /// </summary>
        /// <param name="targetEpisodeID"></param>
        /// <param name="quickData"></param>
        public void SetRegularQuickPlay(JsonData quickData) {
            
            InitControls();
            
            targetEpisodeID = SystemManager.GetJsonNodeString(quickData, LobbyConst.STORY_EPISODE_ID);
            quickEpisode = StoryManager.GetNextFollowingEpisodeData(targetEpisodeID);
            
            if(quickEpisode == null || !quickEpisode.isValidData) {
                this.gameObject.SetActive(false);
                return;
            }
            
            if(quickEpisode.episodeNO  == "1" && string.IsNullOrEmpty(quickData[GameConst.COL_SCENE_ID].ToString())) {
                this.gameObject.SetActive(false);
                return;
            }
            
            this.gameObject.SetActive(true);
            
            // 타이틀 
            textTitle.text = quickEpisode.episodeTitle;
            
            // N화 표기 
            if(quickEpisode.episodeType == EpisodeType.Chapter) {
                textNo.text = SystemManager.GetLocalizedText("5027") + " " + quickEpisode.episodeNO;
            }
            else {
                
                if(quickEpisode.endingType == "hidden") 
                    textNo.text = SystemManager.GetLocalizedText("5087");
                else
                    textNo.text = SystemManager.GetLocalizedText("5088");
            }
            
            
            isRegular = true;
        }
        
        /// <summary>
        /// 퀵 버튼 클릭
        /// </summary>        
        public void OnClickQuickButton() {
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, quickEpisode, string.Empty);
            
            iconParent.DOLocalMoveY(-10, 0.2f).SetLoops(1, LoopType.Yoyo);
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void OnClickBubble() {
            
        }
        
    } // ? end of class
}