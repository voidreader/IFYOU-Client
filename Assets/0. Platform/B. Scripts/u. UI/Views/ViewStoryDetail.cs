using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

namespace PIERStory {
    public class ViewStoryDetail : CommonView
    {
        
        public static Action RefreshStoryDetail = null; // Refresh 액션
        
        [Header("== ScrollRect ==")]        
        [SerializeField] ScrollRect mainScrollRect;
        [SerializeField] GameObject topButton;  // 맨 위로 가기 버튼
        [SerializeField] VerticalLayoutGroup mainScrollVertical;
        
        [Space]
        [Header("== Upper Controls ==")]
        [SerializeField] ImageRequireDownload mainThumbnail; // 썸네일 
        [SerializeField] GameObject btnCredit; 
        [SerializeField] TextMeshProUGUI textTitle; // 타이틀
        [SerializeField] TextMeshProUGUI textAuthor; // 원작자
        [SerializeField] TextMeshProUGUI textProducer; // 제작사
        [SerializeField] TextMeshProUGUI textGenre; // 장르 
        [SerializeField] TextMeshProUGUI textSummary; // 요약
        
        [Space]
        [Header("== Lower Controls ==")]
        [SerializeField] TextMeshProUGUI textSorting; // 정렬 tmp text
        
        
        
        JsonData episodeListJson; // 정규 에피소드 순서대로 
        JsonData reverseEpisodeListJson ; // 정규 에피소드 역순
        JsonData specialListJson; // 스페셜 에피소드 순서대로 
        JsonData endingListJson; // 엔딩 에피소드 
        
        int episodeCount = 0;           // 에피소드 갯수, 미해금된 사이드 갯수
        int openEndingCount = 0;        // 열린 엔딩 갯수
        [SerializeField] List<EpisodeElement> listEpisodeElements; // 에피소드 리스트 (로비매니저에서 옮겨왔음 )
        
        string  totalEpisodeCount = string.Empty; // 정규 에피소드 카운트
        bool isReverse = false; // 역순 리스트 
        bool isCreditUse = false; // 크레딧 버튼 사용 
        
        public override void OnView()
        {
            base.OnView();
            
            UserManager.OnRequestEpisodeReset = this.OnView;
            UserManager.OnFreepassPurchase = this.SetFreepassInfo;
            RefreshStoryDetail = this.OnView;
            
            
        }
        
        public override void OnStartView() {
            base.OnStartView();
            
            
            SetProjectBaseInfo(); // 기본 프로젝트 정보
        }
        
        /// <summary>
        /// 리프레시가 필요없는 기본 프로젝트 정보
        /// </summary>
        void SetProjectBaseInfo() {
            
            string thumbnailURL = StoryManager.main.GetStoryDetailInfo("ifyou_thumbnail_url");
            string thumbnailKey = StoryManager.main.GetStoryDetailInfo("ifyou_thumbnail_key");
            
            mainThumbnail.SetDownloadURL(thumbnailURL, thumbnailKey); // 썸네일 
            
            textTitle.text = StoryManager.main.GetStoryDetailInfo("title"); // 타이틀 
            textAuthor.text = StoryManager.main.GetStoryDetailInfo("original"); // 원작사
            textProducer.text = StoryManager.main.GetStoryDetailInfo("writer"); // 제작사 
            textSummary.text = StoryManager.main.GetStoryDetailInfo("summary"); // 요약 
            textGenre.text = "#장르"; // 해시태그 장르 
            
            
            totalEpisodeCount = StoryManager.main.GetStoryDetailInfo("episode_count"); // 메인 에피소드의 개수 (엔딩제외)
            isCreditUse = StoryManager.main.GetStoryDetailInfo("is_credit").Equals("1") ? true : false; // 크레딧 사용 여부 
        }
        
        
        #region 에피소드 리스트 처리
        
        /// <summary>
        /// 에피소드 카운트 레이블 설정
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="episodeCount"></param>
        /// <param name="endingCount"></param>
        void SetEpisodeCountText(int totalCount, int episodeCount, int endingCount = 0)
        {
            /*   
            textEpisodeCount.text = string.Format(SystemManager.GetLocalizedText("6053"), totalCount);

            if (toggleEpisode.isOn)
                episodeCountDetail.text = string.Format(SystemManager.GetLocalizedText("6054"), episodeCount, endingCount);
                //episodeCountDetail.text = string.Format("에피소드 {0}개 / 엔딩 {1}개", episodeCount, endingCount);

            if (toggleSpecial.isOn)
                episodeCountDetail.text = string.Format("");
            */
        }
        
        
        /// <summary>
        /// 에피소드 리스트 리셋 
        /// </summary>
        void ResetEpisodeList() {
            for(int i=0; i<listEpisodeElements.Count;i++) {
                listEpisodeElements[i].gameObject.SetActive(false);
            }
            
            episodeCount = 0;
            openEndingCount = 0;
        }
        
        /// <summary>
        /// JSON 받아서 에피소드 element 설정
        /// 정규 에피소드 LIST 정보를 가지고 호출한다.
        /// </summary>
        /// <param name="__listJSON">에피소드 리스트</param>
        /// <param name="__isMain">정규 에피소드 여부</param>
        void SetRegularEpisodeList(JsonData __listJSON) {
            ResetEpisodeList();
            
            if(__listJSON == null)
                return;
                
            int listIndex = 0; 
                
            for (int i=0; i< __listJSON.Count;i++) {
                
                // 미리 만들어놓은 개수보다 많아지면 종료해놓자 
                if(listEpisodeElements.Count -1 < i) {
                    return;
                }
                
                // * 엔딩은 따로 빼놓는다. 
                
                // 정규에피소드의 경우는 'chapter' 타입만 처리한다. 
                if(!SystemManager.GetJsonNodeString(__listJSON[i], "episode_type").Equals("chapter"))
                    continue;
                
                // 리스트 설정하기.
                listEpisodeElements[listIndex++].InitElement(__listJSON[i]);
            } // ? end for
                            
        }
        
        #endregion
        
        
        #region 프리패스 관련 처리 
        
        void SetFreepassInfo() {
            
        }
        
        #endregion
    }
}