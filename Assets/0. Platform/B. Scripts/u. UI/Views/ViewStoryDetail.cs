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
        float mainScrollRectY = 0;
        
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
        
        [SerializeField] StoryContentsButton buttonContentsMission;
        [SerializeField] StoryContentsButton buttonContentsGallery;
        [SerializeField] StoryContentsButton buttonContentsEnding;
        [SerializeField] StoryContentsButton buttonContentsSelection;
        
        
        
        [Space]
        [Header("== Lower Controls ==")]
        
        [SerializeField] UIToggle toggleRegular; // 토글 정규 에피소드
        [SerializeField] UIToggle toggleSpecial; // 토글 스페셜 에피소드 
        
        [SerializeField] TextMeshProUGUI textSorting; // 정렬 tmp text
        [SerializeField] TextMeshProUGUI textTotalEpisodeCount; // 총 몇개의 에피소드가 있다. 
        [SerializeField] TextMeshProUGUI textDetailEpisodeCount; // 상세 에피소드 카운팅 
        [SerializeField] TextMeshProUGUI textSpecialEpisodeExplain; // 스페셜 에피소드 부연설명!
        
        [SerializeField] GameObject endingNotification; // 엔딩 알림
        
        JsonData continueData = null;
        
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
            
            UserManager.OnRequestEpisodeReset = this.OnStartView;
            UserManager.OnFreepassPurchase = this.SetFreepassInfo;
            RefreshStoryDetail = this.OnStartView;
        }
        
        public override void OnStartView() {
            base.OnStartView();
            
            
            
            SetProjectBaseInfo(); // 기본 프로젝트 정보
            
            // * 게임씬에 있다가 돌아온 경우에 대한 처리 
            if(StoryManager.enterGameScene) {
                StoryManager.enterGameScene = false;
            }
            else {
                ShowEpisodeList(true);
            }
            
            
            // 상단 처리
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_VIEW_NAME, false, string.Empty);
            
        }
        
        
        public override void OnHideView() {
            // 백버튼 비활성화
            // * StoryDetail이 비활성화 되는 경우는 메인으로 돌아갈때만이다. 
            // * 되돌아갈때가 어렵네...
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
        }        
        
        
        
        void Update() {
            
            /*
            if(ViewCommonTop.staticCurrentTopOwner != this.gameObject.name)
                return;
            
            
            if(mainScrollRect.content.transform.localPosition.y > 150f && !ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
                return;
            }
            
            
            if(mainScrollRect.content.transform.localPosition.y <= 150f && ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
                return;
            }
            */
            
             
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
            
            // 컨텐츠 버튼 초기화
            buttonContentsSelection.InitContentsButton();
            buttonContentsGallery.InitContentsButton();
            buttonContentsMission.InitContentsButton();
            buttonContentsEnding.InitContentsButton();
            
            // 빠른플레이, 엔딩 알람을 위한 project current 
            continueData = UserManager.main.GetUserProjectRegularEpisodeCurrent();
            SetBottomNotification(); // continueData 가져가서 처리 
        }
        
        
        /// <summary>
        /// 메인 스크롤렉트 제일 상단으로 보내기 
        /// </summary>
        public void SetScrollTop() {
            mainScrollRect.verticalNormalizedPosition = 0;
        }
        
        
        #region 에피소드 리스트 처리
        
        /// <summary>
        /// 정규 에피소드 리스트 리프레시 
        /// </summary>
        public void RefreshRegularEpisodeList() {
            // 리셋하고 호출하도록 한다. (Signal listener에서 호출)
            
            StoryManager.main.UpdateRegularEpisodeData();
            ShowEpisodeList(true);
            
            
        }
        
        /// <summary>
        /// 에피소드 리스트를 보여주세요! 제발!
        /// </summary>
        /// <param name="__isRegular"></param>
        public void ShowEpisodeList(bool __isRegular) {
            
            Debug.Log("ShowEpisodeList : " + __isRegular);
            
            if (!StoryManager.main || string.IsNullOrEmpty(StoryManager.main.CurrentProjectID))
                return;
                
            if(__isRegular) { // * 정규 에피소드 
                SetEpisodeList(StoryManager.main.RegularEpisodeList);
                SetRegularEpisodeCountText(StoryManager.main.regularEpisodeCount,  StoryManager.main.unlockEndingCount);
            }
            else  { // * 스페셜(사이드) 에피소드
                SetEpisodeList(StoryManager.main.SideEpisodeList);
                SetSideEpisodeCountText(StoryManager.main.sideEpisodeCount, StoryManager.main.unlockSideEpisodeCount);
            }
            

        }
        
        /// <summary>
        /// 정규 에피소드 카운팅 텍스트 설정 
        /// </summary>
        /// <param name="episodeCount"></param>
        /// <param name="unlockEndingCount"></param>
        void SetRegularEpisodeCountText(int episodeCount, int unlockEndingCount)
        {
            textTotalEpisodeCount.text = string.Format(SystemManager.GetLocalizedText("10000"), episodeCount + unlockEndingCount);
            textDetailEpisodeCount.text = string.Format(SystemManager.GetLocalizedText("6054"), episodeCount, unlockEndingCount);
        }
        
        /// <summary>
        /// 사이드 에피소드 카운팅 텍스트 설정 
        /// </summary>
        /// <param name="sideCount"></param>
        /// <param name="unlockSideCount"></param>
        void SetSideEpisodeCountText(int sideCount, int unlockSideCount) {
            textTotalEpisodeCount.text = string.Format(SystemManager.GetLocalizedText("10000"), unlockSideCount);
            textDetailEpisodeCount.text = string.Empty;
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
        void SetEpisodeList(List<EpisodeData> __listEpisode) {
            ResetEpisodeList();
            
            if(__listEpisode == null || __listEpisode.Count == 0)
                return;
            
            // * 작품개수를 3으로 나눈다. 
            int dividedThree = Mathf.FloorToInt((float)__listEpisode.Count / 3f );
            
            // 
            for(int i=0; i<dividedThree; i++) {
                ListThreeEpisodeRows[i].InitRow(__listEpisode, i);
            }
                           
        }
        
        
        
        #endregion
        
        
        #region 프리패스 관련 처리 
        
        void SetFreepassInfo() {
            
        }
        
        #endregion
        
        
        #region 빠른플레이 & 엔딩 알림
        
        /// <summary>
        /// 엔딩 알림, 빠른 플레이 표기 처리 
        /// </summary>
        void SetBottomNotification() {
            
            // * 엔딩이고 엔딩 플레이를 완료한 경우에는 엔딩 알람을 하단에 띄워준다. 
            if( SystemManager.GetJsonNodeBool(continueData, "is_final") 
            && SystemManager.GetJsonNodeBool(continueData, "is_ending")) {
                endingNotification.SetActive(true); 
                mainScrollVertical.padding.bottom = 100; // 간격 줘야함!
                return;
            }
        }
        
        /// <summary>
        /// 빠른 플레이 처리 
        /// </summary>
        void SetFastPlay() {
            
        }
        
        #endregion
        
        
        /// <summary>
        /// 메인ScrollRect 상하 변경시.. 상단 제어 
        /// </summary>
        /// <param name="vec"></param>
        public void OnValueChangedMainScroll(Vector2 vec) {
            
            if(mainScrollRectY == vec.y)
                return;
                
            mainScrollRectY = vec.y;
            
            if(mainScrollRectY < 0.95f && !ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
                return;
            }
           
            
            if(mainScrollRectY >= 0.95f && ViewCommonTop.isBackgroundShow) {
                Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
                return;
            }
        }
        
    }
}