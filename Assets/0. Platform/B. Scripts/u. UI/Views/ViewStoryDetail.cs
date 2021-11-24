using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

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
        
        [SerializeField] UIToggle toggleRegular; // 토글 정규 에피소드
        [SerializeField] UIToggle toggleSpecial; // 토글 스페셜 에피소드 
        
        [SerializeField] TextMeshProUGUI textSorting; // 정렬 tmp text
        [SerializeField] TextMeshProUGUI textTotalEpisodeCount; // 총 몇개의 에피소드가 있다. 
        [SerializeField] TextMeshProUGUI textDetailEpisodeCount; // 상세 에피소드 카운팅 
        [SerializeField] TextMeshProUGUI textSpecialEpisodeExplain; // 스페셜 에피소드 부연설명!
        
        
        
        JsonData episodeListJson; // 정규 에피소드 순서대로 
        JsonData reverseEpisodeListJson ; // 정규 에피소드 역순
        JsonData specialListJson; // 스페셜 에피소드 순서대로 
        JsonData endingListJson; // 엔딩 에피소드 
        
        int episodeCount = 0;           // 에피소드 갯수, 미해금된 사이드 갯수
        int openEndingCount = 0;        // 열린 엔딩 갯수
        //[SerializeField] List<EpisodeElement> listEpisodeElements; // 에피소드 리스트 (로비매니저에서 옮겨왔음 )
        [SerializeField] List<ThreeEpisodeRow> ListThreeEpisodeRows; // 에피소드 3개짜리 행 
        
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
            
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_ON_BACK_BUTTON, string.Empty);
            
            SetProjectBaseInfo(); // 기본 프로젝트 정보
            
            // * 게임씬에 있다가 돌아온 경우에 대한 처리 
            if(StoryManager.enterGameScene) {
                StoryManager.enterGameScene = false;
            }
            else {
                ShowEpisodeList(true);
            }
            
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
        
        
        /// <summary>
        /// 메인 스크롤렉트 제일 상단으로 보내기 
        /// </summary>
        public void SetScrollTop() {
            mainScrollRect.verticalNormalizedPosition = 0;
        }
        
        
        #region 에피소드 리스트 처리
        
        /// <summary>
        /// 에피소드 리스트를 보여주세요! 제발!
        /// </summary>
        /// <param name="__isRegular"></param>
        public void ShowEpisodeList(bool __isRegular) {
            
            Debug.Log("ShowEpisodeList : " + __isRegular);
            
            if (!StoryManager.main || string.IsNullOrEmpty(StoryManager.main.CurrentProjectID))
                return;
                
            if(__isRegular) {
                SetEpisodeList(StoryManager.main.RegularListJSON);
                
            }
            else  {
                SetEpisodeList(StoryManager.main.SideEpisodeListJson);
            }
            

        }
        
        /// <summary>
        /// 에피소드 카운트 레이블 설정
        /// </summary>
        /// <param name="totalCount">전체 에피소드 (볼 수 있는)</param>
        /// <param name="episodeCount">정규 에피소드 카운트</param>
        /// <param name="endingCount">엔딩 카운트</param>
        void SetEpisodeCountText(int totalCount, int episodeCount, int endingCount = 0)
        {
            textTotalEpisodeCount.text = string.Format(SystemManager.GetLocalizedText("10000"), totalCount);
            textDetailEpisodeCount.text = string.Format(SystemManager.GetLocalizedText("6054"), episodeCount, endingCount);
            
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
            for(int i=0; i<ListThreeEpisodeRows.Count;i++) {
                ListThreeEpisodeRows[i].gameObject.SetActive(false);
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
        void SetEpisodeList(JsonData __listJSON) {
            ResetEpisodeList();
            
            if(__listJSON == null)
                return;
            
            // * 작품개수를 3으로 나눈다. 
            int dividedThree = Mathf.FloorToInt((float)__listJSON.Count / 3f );
            
            // 
            for(int i=0; i<dividedThree; i++) {
                ListThreeEpisodeRows[i].InitRow(__listJSON, i);
            }
                           
        }
        
        
        
        #endregion
        
        
        #region 프리패스 관련 처리 
        
        void SetFreepassInfo() {
            
        }
        
        #endregion
    }
}