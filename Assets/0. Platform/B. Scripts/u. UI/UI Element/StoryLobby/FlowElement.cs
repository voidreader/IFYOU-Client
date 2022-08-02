using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;



namespace PIERStory {

    public class FlowElement : MonoBehaviour
    {
        public EpisodeData currentEpisode; // Flow 대상 에피소드 정보 
        
        public RectTransform rectOrigin; 
        public RectTransform rectBody; 
        public LayoutElement layoutElement;
        
        public Image imageIcon; // 왼쪽 아이콘 
        public Image imageOutline; //아웃라인 
        
        public TextMeshProUGUI textEpisodeNumber;
        public TextMeshProUGUI textEpisodeTitle;
        public TextMeshProUGUI textCommingSoon; // 커밍순 안내 
        public TextMeshProUGUI textPublishDate; // 연재일 안내 
        
        public GameObject groupOpenLock; // 기다리면 무료 그룹 
        public TextMeshProUGUI textsOpenTime;
        
        
        public RectTransform rectCover; // 미해금 엔딩 잠금 커버
        
        public RectTransform detailFrame; // 상세 프레임 
        public RectTransform detailArrow; // 상세 프레임 펼치고 닫는 화살표 
        
        
        public DateTime openDate; // 다음 오픈시간 
        public long openDateTick; // 다음 오픈시간 tick 
        public TimeSpan timeDiff; // 오픈시간까지 남은 차이 
        [SerializeField] bool isOpenTimeCountable = false; // 타이머 카운팅이 가능한지
        
        
        // * 디테일
        public TextMeshProUGUI textSummary;
        public GameObject groupIllustProgressor; // 일러스트 진행도
        public GameObject groupSceneProgressor; // 사건진행도 
        public Doozy.Runtime.Reactor.Progressor illustProgressor; // 일러스드 획득 진행도 
        public Doozy.Runtime.Reactor.Progressor sceneProgressor; // 사건 획득 진행도 
        
        
        bool inTrasitionDetailFrame = false;
        
        
        [Space]
        [Header("Colors")] 
        public Sprite spriteReset; // 리셋 아이콘 
        public Sprite spriteCurrent; // 현재 에피소드 아이콘 
        public Sprite spriteCommingSoon; // 커밍순 아이콘 
        
        public Sprite spriteCurrentOutline; // 현재 활성화 아웃라인 
        public Sprite spriteNormalOutline; // 일반 아웃라인 
        
        
        
        [Space]
        [Header("Colors")]
        public Color colorEndingTitle; // 엔딩 타이틀 컬러
        public Color colorPastTitle; // 과거 에피소드 타이틀 컬러
        public Color colorCurrentTitle; // 현재 에피소드 타이클 컬러 
        public Color colorGeneralTextColor; // 일반 텍스트 컬러
        public Color colorInactiveTextColor; // 비활성 텍스트 컬러 
        
        
        public Color colorPastOutline; // 과거 아웃라인 컬러 
        
        [Space]
        [Header("Vectors")] 
        public Vector2 normalEpisodeNumberPos; // 일반 에피소드 넘버링 포지션 
        public Vector2 normalEpisodeTitlePos; // 일반 에피소드 타이틀 포지션
        public Vector2 endingEpisodeNumberPos; // 엔딩 에피소드 넘버링 포지션 
        public Vector2 endingEpisodeTitlePos; // 엔딩 에피소드 타이틀 포지션
        
        public Vector2 sizeNormalText;
        public Vector2 sizeEndingText;
        
        public Vector2 sizeNormalBody; // 사이즈 노멀 바디 사이즈
        public Vector2 sizeEndingBody; // 사이즈 엔딩 바디 사이트
        
        public Vector2 sizeDetailOrigin; // 디테일 정보 짧은 크기 
        public Vector2 sizeDetailLong; //  디테일 정보 긴 크기 
        public Vector2 sizeOriginLong; // 디테일 펼쳤을때 본체의 크기 
        
        public Vector2 groupLockNoTimePos; // 타이머 없는 잠금 위치... 
        public Vector2 groupLockTimePos; // 타이머 있는 잠금 위치..
        
        
        private void Update() {
            if(!isOpenTimeCountable) {
                return;
            }
            
            if(Time.frameCount % 5 == 0) {
                textsOpenTime.text = GetOpenRemainTime();
            }
        }
        
        /// <summary>
        /// Flow 초기화 
        /// </summary>
        /// <param name="__episode"></param>
        public void InitFlowElement(EpisodeData __episode) {
            
            currentEpisode = __episode; // 에피소드 정보 
            
            // 컨트롤들 초기화 
            this.gameObject.SetActive(true);
            rectCover.gameObject.SetActive(false);
            textEpisodeTitle.text = string.Empty;
            textCommingSoon.text = string.Empty;
            textPublishDate.text = string.Empty;
            
            
            isOpenTimeCountable = false; 
            
            
            SystemManager.SetText(textEpisodeTitle, currentEpisode.episodeTitle);
            textEpisodeNumber.text = currentEpisode.flowPrefix;
            
            // 디테일 관련 처리 
            detailFrame.sizeDelta = sizeDetailOrigin;
            detailArrow.eulerAngles = Vector3.zero;
            rectOrigin.sizeDelta = sizeNormalBody;
            layoutElement.minWidth = sizeDetailOrigin.x;
            layoutElement.minHeight = sizeDetailOrigin.y;

            // Summary 및 게이지 처리             
            SystemManager.SetText(textSummary, currentEpisode.episodeSummary);
            if(currentEpisode.episodeGalleryImageProgressValue > -1) {
                groupIllustProgressor.SetActive(true);
                illustProgressor.SetProgressAt(currentEpisode.episodeGalleryImageProgressValue);
            }
            else {
                groupIllustProgressor.SetActive(false);
            }
            
            // 사건 게이지 설정 
            sceneProgressor.SetProgressAt(currentEpisode.sceneProgressorValue);
            
            
            // 상태 설정 
            SetState();
            
            // 엔딩의 경우 추가 처리 
            if(__episode.episodeType == EpisodeType.Ending) {
                SetEndingFlow();
            }
            
        }
        
        
        /// <summary>
        /// 엔딩에 대한 추가 처리
        /// </summary>
        void SetEndingFlow() {
            groupOpenLock.SetActive(false);
            imageIcon.gameObject.SetActive(false);
            rectCover.gameObject.SetActive(true);
            
            // 색상도 변경 
            textEpisodeNumber.color = colorEndingTitle;
            textEpisodeTitle.color = colorGeneralTextColor;
            
            
            // 엔딩은 크기가 달라..
            rectBody.sizeDelta = sizeEndingBody;
            rectCover.sizeDelta = sizeEndingBody;
            
            textEpisodeNumber.rectTransform.sizeDelta = sizeEndingText;
            textEpisodeTitle.rectTransform.sizeDelta = sizeEndingText;
            
            textEpisodeNumber.rectTransform.anchoredPosition = endingEpisodeNumberPos;
            textEpisodeTitle.rectTransform.anchoredPosition = endingEpisodeTitlePos;
            
            // 해금된 경우 
            if(currentEpisode.endingOpen) {
                rectCover.gameObject.SetActive(false);
            }
            else {
                SystemManager.SetText(textEpisodeTitle, "????"); // 미해금된 경우 엔딩 제목을 노출하지 않음 
            }
        }
        
        
        /// <summary>
        /// 에피소드 상태에 따른 색상 및 아이콘 처리 . 
        /// </summary>
        void SetState() {
            
            groupOpenLock.SetActive(false);
            imageIcon.gameObject.SetActive(false);
            imageOutline.sprite = spriteNormalOutline;
            imageOutline.color = Color.white;
            
            // 일반과 엔딩은 크기가 다르다. 
            rectBody.sizeDelta = sizeNormalBody;
            rectCover.sizeDelta = sizeNormalBody;
            
            textEpisodeNumber.rectTransform.sizeDelta = sizeNormalText;
            textEpisodeTitle.rectTransform.sizeDelta = sizeNormalText;
            
            textEpisodeNumber.rectTransform.anchoredPosition = normalEpisodeNumberPos;
            textEpisodeTitle.rectTransform.anchoredPosition = normalEpisodeTitlePos;
            
            
            
            switch(currentEpisode.episodeState) {
                case EpisodeState.Prev: // 과거 
                
                // icon
                imageIcon.gameObject.SetActive(true);
                imageIcon.sprite = spriteReset; 
                imageIcon.SetNativeSize();
                
                
                textEpisodeNumber.color = colorPastTitle;
                textEpisodeTitle.color = colorGeneralTextColor;
                
                imageOutline.color = colorPastOutline; // 과거 아웃라인 색상 변경
                
                break; // ? 과거끝 
                    
                case EpisodeState.Current: // 현재 
                
                
                // icon
                imageIcon.gameObject.SetActive(true);
                imageIcon.sprite = spriteCurrent;
                imageIcon.SetNativeSize();
                
                textEpisodeNumber.color = colorCurrentTitle;
                textEpisodeTitle.color = colorGeneralTextColor;
                
                imageOutline.sprite = spriteCurrentOutline;
                
                break;
                
                case EpisodeState.Future: // 미래
                
                // 그룹 잠금.. 시간 
                groupOpenLock.SetActive(true);
                groupOpenLock.GetComponent<RectTransform>().anchoredPosition = groupLockNoTimePos;
                
                textsOpenTime.text = string.Empty;
                
                
                textEpisodeNumber.color = colorInactiveTextColor;
                textEpisodeTitle.color = colorInactiveTextColor;
                
                
                imageOutline.color = Color.white; // 미래 아웃라인 색상 변경
                
                break;
            }
        }
        
        
        /// <summary>
        /// 공개 예정일에 대한 처리
        /// </summary>
        /// <param name="__publishDate"></param>
        public void SetPublishDate(DateTime __publishDate) {
            
            // * 나도 까먹어서 적는 연재일 처리 
            // 엔딩, 스페셜에는 영향을 주지 않는다.
            // 과거 상태, 이미 연재일이 지난 경우 반영하지 않음
            // 패스 유무에 관계없이 적용된다. 
            
            
            
            // 엔딩 제외
            if(currentEpisode.episodeType == EpisodeType.Ending)
                return;
                
            // 과거는 변경하지 않음 
            if(currentEpisode.episodeState == EpisodeState.Prev)
                return; 
   
            

            
            openDate = __publishDate;
            timeDiff = openDate - DateTime.UtcNow.AddHours(9);
            
            // 현재이고 이미 오픈된 상태는 return
            if(currentEpisode.episodeState == EpisodeState.Current && timeDiff.Ticks <= 0 ) {
                return;
            }            
            
            groupOpenLock.SetActive(false);
            imageIcon.gameObject.SetActive(true);
            imageIcon.sprite = spriteCommingSoon;
            
            textEpisodeTitle.text = string.Empty; // 타이틀 없애고 . 
            imageIcon.sprite = spriteCommingSoon;
            
            SystemManager.SetLocalizedText(textCommingSoon, "5187");
            
            textEpisodeNumber.color = colorInactiveTextColor; // 에피소드 번호 색상 회색으로 변경 
            
            
            // 미래 시점에 열린다고 날짜 적힌것만 처리 
            if(timeDiff.Ticks > 0) {
                // 미래에 열린다.  totalDays는 0.n 으로 찍히네. 
                // Debug.Log(textEpisodeNumber.text + " : " + timeDiff.TotalDays +"/" + timeDiff.Days);
                if(timeDiff.Days <=0 ) {
                    textPublishDate.text = SystemManager.GetLocalizedText("5189");
                }
                else {
                    textPublishDate.text = string.Format(SystemManager.GetLocalizedText("5188"), (int)timeDiff.Days);
                }
                
            }
            
        }
        
        /// <summary>
        /// 오픈 시간에 대한 설정 
        /// </summary>
        /// <param name="__nextOpenTick"></param>
        public void SetOpenTime(long __nextOpenTick) {
            openDateTick = __nextOpenTick;
            openDate = new DateTime(openDateTick);
            
            timeDiff = openDate - DateTime.UtcNow;
            
            if(timeDiff.Ticks > 0) {
                isOpenTimeCountable = true; // 카운팅 가능한상태 
            }
            
            // 패스 보유 중인지 체크한다.
            if(UserManager.main.ifyouPassDay > 0 || StoryManager.main.CurrentProject.hasPremiumPass || StoryManager.main.CurrentProject.IsValidOnedayPass())
                isOpenTimeCountable = false; // 대기시간 없다.
            
            // isOpenTimeCountable의 반대 값을 전달 
            // 시간 카운팅 = 열리지 않았음
            RefreshOpenTimeState(!isOpenTimeCountable);
           
        }
        
        string GetOpenRemainTime() {
            timeDiff = openDate - DateTime.UtcNow;
            
            if(timeDiff.Ticks < 0) {

                RefreshOpenTimeState(true); // 오픈 
                return string.Empty;
            }
            
            return string.Format ("{0:D2}:{1:D2}:{2:D2}",timeDiff.Hours ,timeDiff.Minutes, timeDiff.Seconds);
        }
        
        /// <summary>
        /// 오픈 타임에 따른 추가 상태 설정 
        /// </summary>
        /// <param name="__isOpen"></param>
        void RefreshOpenTimeState(bool __isOpen) {
            
            // 엔딩 무시하기.
            if(currentEpisode.episodeType == EpisodeType.Ending)
                return;
            
            // 미래, 현재 상태에 따라 처리가 달라져야 한다.
            if(currentEpisode.episodeState == EpisodeState.Current) {
                
                if(__isOpen) { // 열림 상태 
                    textEpisodeNumber.color = colorCurrentTitle;
                    textEpisodeTitle.color = colorGeneralTextColor;
                    
                    groupOpenLock.SetActive(false);
                    imageIcon.gameObject.SetActive(true);
                    
                    //groupOpenLock.GetComponent<RectTransform>().anchoredPosition = groupLockTimePos;
                    
                }
                else { // 대기 상태 
                    textEpisodeNumber.color = colorInactiveTextColor;
                    textEpisodeTitle.color = colorInactiveTextColor;
                    
                    groupOpenLock.SetActive(true);
                    imageIcon.gameObject.SetActive(false);
                    
                    groupOpenLock.GetComponent<RectTransform>().anchoredPosition = groupLockTimePos;
                    
                }
                
                return;
                
            } // ? 현재 상태
            
            
            
            // * 미래 상태일때 처리 
            if(currentEpisode.episodeState == EpisodeState.Future) {
                groupOpenLock.GetComponent<RectTransform>().anchoredPosition = groupLockTimePos;
                
                groupOpenLock.SetActive(true);
                    imageIcon.gameObject.SetActive(false);
                
            }
            
            
        }
        
    
    
        /// <summary>
        /// 디테일 펼치기
        /// </summary>        
        public void OnClickArrow() {
            if(inTrasitionDetailFrame)
                return;

            // 펼친다.             
            if(detailFrame.sizeDelta.y < 100) {
                detailFrame.DOSizeDelta(sizeDetailLong, 0.4f).OnStart(()=> {inTrasitionDetailFrame = true;}).OnComplete(()=> {inTrasitionDetailFrame=false;});
                layoutElement.DOMinSize(sizeDetailLong, 0.4f); 
                rectOrigin.DOSizeDelta(sizeOriginLong, 0.4f);
                
                  
            }
            else { // 접는다.
                detailFrame.DOSizeDelta(sizeDetailOrigin, 0.4f).OnStart(()=> {inTrasitionDetailFrame = true;}).OnComplete(()=> {inTrasitionDetailFrame=false;});
                layoutElement.DOMinSize(sizeDetailOrigin, 0.4f);
                rectOrigin.DOSizeDelta(sizeNormalBody, 0.4f);
            }
            
        }
        
        public void OnClickIcon() {
            
            // 리셋에 대한 처리다. 
            if(currentEpisode.episodeState == EpisodeState.Prev || currentEpisode.episodeState == EpisodeState.Block) {
                SystemManager.ShowFlowResetPopup(currentEpisode); // 리셋 호출.
            }
        }
        
        /// <summary>
        /// 플로우 자체를 버튼처럼 사용할 수 있게 변경.
        /// </summary>
        public void OnClickFlow() {
            if(UserManager.main.CheckAdminUser()) {
                SystemManager.ShowSystemPopup("슈퍼 유저입니다. 선택한 에피소드를 플레이 하겠습니까?", SuperUserEpisodeStart, null);
            }
        }
        
        void SuperUserEpisodeStart() {
            // 바로 시작 
                StoryLobbyMain.SuperUserFlowEpisodeStart?.Invoke(currentEpisode);
        }

    }
}