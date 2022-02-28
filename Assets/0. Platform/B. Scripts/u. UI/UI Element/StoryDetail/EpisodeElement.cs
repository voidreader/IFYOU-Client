using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class EpisodeElement : MonoBehaviour
    {
        
        
        JsonData purchaseData; // 에피소드 구매 정보
        
        
        
        [SerializeField] ImageRequireDownload thumbnailImage; 
        public EpisodeData episodeData = null; 
        public bool hasDependentEnding = false; // 귀속된 엔딩이 있는지 체크 
        public bool hasCurrentDependentEnding = false; // 귀속된 엔딩중 현재 상태의 엔딩이 있는지.
        public List<EpisodeData> ListDependentEnding = new List<EpisodeData>();
        
        
        // 플래이 상태(현재, 과거, 미래)
        [Header("== 플레이 상태(과거,현재,미래) ==")]
        [SerializeField] Image playStateCover; // 커버
        [SerializeField] Image playStateIcon; // 아이콘 
        [SerializeField] GameObject currentPlayFlag;  // 책갈피 표시
        
        [Space]
        [Header("== 구매 상태 (프리미엄, 1회플레이, 프리) ==")]
        [SerializeField] Image purchaseStateBox;
        [SerializeField] TextMeshProUGUI textPurchaseState;
        
        [Space]
        [SerializeField] TextMeshProUGUI textEpisodeTitle; // 타이틀 
        [SerializeField] TextMeshProUGUI textEpisodeNumbering; // 에피소드 번호 
        
        [SerializeField] GameObject btnSpreadEnding; // 엔딩 펼침 버튼 
        ThreeEpisodeRow parentThreeRow; // 부모 ThreeRow
        public int columnIndex = 0;
        
        /// <summary>
        /// 에피소드 리셋 
        /// </summary>
        void ResetData() {
            
            
            thumbnailImage.InitImage();
            playStateCover.gameObject.SetActive(false);
            currentPlayFlag.gameObject.SetActive(false);
            
            textEpisodeNumbering.text = string.Empty;
            textEpisodeTitle.text = string.Empty;
            
            purchaseData = null;
            
            btnSpreadEnding.SetActive(false);
        }
        
        
        /// <summary>
        /// 에피소드 초기화
        /// </summary>
        /// <param name="__data"></param>
        public void InitElement(ThreeEpisodeRow __threeRow, EpisodeData __data, int __columnIndex) {
            
            this.gameObject.SetActive(true);
            
            ResetData();
            
            episodeData = __data;
            columnIndex = __columnIndex;
            parentThreeRow = __threeRow;
          
            
            // 목록 썸네일 처리 
            thumbnailImage.SetDownloadURL(episodeData.squareImageURL, episodeData.squareImageKey);
            
            // 타이틀 
            textEpisodeTitle.text = episodeData.episodeTitle;
            textEpisodeNumbering.text = string.Empty;
            
            // 에피소드 타입에 따라. 
            switch(episodeData.episodeType) {
                case EpisodeType.Chapter:
                textEpisodeNumbering.text = string.Format(SystemManager.GetLocalizedText("6090"), episodeData.episodeNO);
                break;
                
                case EpisodeType.Side:
                textEpisodeNumbering.text = SystemManager.GetLocalizedText("5028");
                break;
            }
            
            
            SetPurchaseState(); // 구매 상태 처리 
            
            SetPlayStateCover(); // 플레이 상태에 대한 처리 
            
            SetDependentEnding(); // 귀속 엔딩 유무 체크 , 버튼 활성화
            
            CheckDependEndingCurrent(); // 귀속 엔딩 중 현재 상태의 엔딩 체크 
        }
        
        /// <summary>
        /// 귀속된 엔딩 설정 
        /// </summary>
        void SetDependentEnding() {
            
            ListDependentEnding.Clear();
            hasDependentEnding = false;
            hasCurrentDependentEnding = false;
            
            if(episodeData.episodeType != EpisodeType.Chapter)
                return;
            
            
            for(int i=0; i<StoryManager.main.ListCurrentProjectEpisodes.Count;i++) {
                
                // 엔딩이 아니거나, 종속 에피소드가 지정되지 않은 경우는 처리하지 않음.
                if(StoryManager.main.ListCurrentProjectEpisodes[i].episodeType != EpisodeType.Ending ||
                    StoryManager.main.ListCurrentProjectEpisodes[i].dependEpisode == "-1" )
                    continue;
                
                // ID가 동일하고, 해금된 경우에만 한다. 
                // * 어드민 유저의 경우는 모두 해금된 것으로 간주한다. 
                if(StoryManager.main.ListCurrentProjectEpisodes[i].dependEpisode == episodeData.episodeID 
                    && (StoryManager.main.ListCurrentProjectEpisodes[i].endingOpen || UserManager.main.CheckAdminUser())) {
                        
                    ListDependentEnding.Add(StoryManager.main.ListCurrentProjectEpisodes[i]); // 다 모은다. 여러개도 가능하다.
                    hasDependentEnding = true; // 귀속된 엔딩이 있다!
                }
            }
            
            btnSpreadEnding.SetActive(hasDependentEnding);
            
        } // ? SetDependentEnding END
        
        
        /// <summary>
        /// 구매 상태 설정 
        /// </summary>
        public void SetPurchaseState() {
            string episodePurchaseStateText = string.Empty;

            purchaseStateBox.gameObject.SetActive(true);
            
            // 구매 상태에 따라서 purchaseStateBox 처리 
            switch(episodeData.purchaseState) {
                 case PurchaseState.None: // * 구매이력 없음 
                    purchaseStateBox.gameObject.SetActive(false);
                    break;

                case PurchaseState.Rent: // * 대여 현재 안씁니다. (2021.09.16)

                    // 대여 만료 시간을 체크
                    int minDiff = int.Parse(purchaseData["min_diff"].ToString());
                    string remainTime = string.Empty;
                    if (minDiff > 0)
                    {
                        if (minDiff >= 60)
                            remainTime = minDiff / 60 + "시간 " + minDiff % 60 + "분 남음";
                        else
                            remainTime = "\t" + minDiff % 60 + "분 남음";
                    }
                    else
                        remainTime = "대여 기간 만료";

                    episodePurchaseStateText = string.Format("대여 중 | <size=18>{0}</size>", remainTime);

                    break;

                case PurchaseState.Free: // * 무료
                    episodePurchaseStateText = SystemManager.GetLocalizedText("6098");
                    
                    purchaseStateBox.sprite = LobbyManager.main.spritePlateFree;
                    break;

                case PurchaseState.Permanent: // * 스타 플레이 
                    
                    // 프리미엄 패스 보유여부에 따라 달라짐 . 
                    if(UserManager.main.HasProjectFreepass()) {
                        episodePurchaseStateText = SystemManager.GetLocalizedText("6006");
                        purchaseStateBox.sprite = LobbyManager.main.spritePlatePremium;    
                    }
                    else {
                        episodePurchaseStateText = SystemManager.GetLocalizedText("5002");
                        purchaseStateBox.sprite = LobbyManager.main.spritePlateStarPlay;    
                    }
                    
                    break;

                case PurchaseState.OneTime: // * 1회 플레이
                    episodePurchaseStateText = SystemManager.GetLocalizedText("6005");
                    // purchaseStateBox.color = LobbyManager.main.colorOneTimeBox;
                    break;
            } // ? end of switch
            
            
            // 프리미엄 패스 유저 예외처리 (구매하지 않았어도 붙여준다.)
            if(UserManager.main.HasProjectFreepass()) {
                episodePurchaseStateText = SystemManager.GetLocalizedText("6006");
                purchaseStateBox.sprite = LobbyManager.main.spritePlatePremium;    
                purchaseStateBox.gameObject.SetActive(true);
                
            }
            
            // 구매 상태 텍스트 처리, 크기 처리 
            textPurchaseState.text = episodePurchaseStateText;
            Vector2 preferredSize = textPurchaseState.GetPreferredValues(episodePurchaseStateText);
            purchaseStateBox.rectTransform.sizeDelta = preferredSize;
            
                        
        } // ? end of SetPurchaseState
        
        
        
        /// <summary>
        /// 플레이 상태에 따른 커버 및 아이콘 처리 
        /// </summary>
        void SetPlayStateCover() {
            
            playStateCover.gameObject.SetActive(true);
            playStateIcon.gameObject.SetActive(true);
            
            switch(episodeData.episodeState) {
                case EpisodeState.Prev:
                playStateCover.sprite = LobbyManager.main.spriteEpisodePrevCover;
                playStateIcon.sprite = LobbyManager.main.spriteEpisodePrevIcon;
                break;
                
                case EpisodeState.Current:
                playStateCover.sprite = LobbyManager.main.spriteEpisodeCurrentCover;
                currentPlayFlag.gameObject.SetActive(true);
                playStateIcon.gameObject.SetActive(false);
                break;
                
                case EpisodeState.Future:
                playStateCover.sprite = LobbyManager.main.spriteEpisodeNextCover;
                playStateIcon.sprite = LobbyManager.main.spriteEpisodeNextIcon;
                break;
                
            }
            
            if(playStateIcon.gameObject.activeSelf)
                playStateIcon.SetNativeSize();
            
        } // ? end of SetPlayStateCover
        
        /// <summary>
        /// 엔딩 펼침 버튼 감추기
        /// </summary>
        public void HideSpreadButton() {
            btnSpreadEnding.SetActive(false);
        }
        
        /// <summary>
        /// 엔딩 버튼 보여주기 (EndingEpisodeElement에서 호출)
        /// </summary>
        public void ShowSpreadButton() {
            
            // 보여주기가 호출되었어도 갖고있는 엔딩이 없으면 활성화하지 않는다.
            btnSpreadEnding.SetActive(hasDependentEnding);
        }
        
        
        /// <summary>
        /// 목록 클릭, 에피소드 시작 화면 오픈 
        /// </summary>
        public void OnClickElement() {
            
            Debug.Log(">> OnClickElement");
            
            // * 이전이나 블록 상태. 
            if(episodeData.episodeState == EpisodeState.Prev || episodeData.episodeState == EpisodeState.Block) {
                
                // * 리셋 조건 개정(2022.02.28) 
                // * 자유롭게 리셋 할 수 있도록 변경한다. 
                //if(UserManager.main.HasProjectFreepass() || UserManager.main.CheckAdminUser() || UserManager.main.CheckUserProjectRegularEpisodeFinal()) {
                // Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_EPISODE_RESET, episodeData, string.Empty);
                // SystemManager.ShowLobbySubmitPopup(SystemManager.GetLocalizedText("6199"));                    
                
                SystemManager.ShowFlowResetPopup(episodeData);
            }
            
            UserManager.main.useRecord = true;
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, episodeData, string.Empty);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void DelayOnClickEndingSpread() {
            Invoke("OnCickEndingSpread",  0.1f);
        }
        
        /// <summary>
        /// 엔딩 펼침버튼 누르기 
        /// </summary>
        public void OnCickEndingSpread() {
            // 소속되어있는 ThreeRow한테 전달을 해줘야해요. 
            parentThreeRow.SpreadEnding(ListDependentEnding, columnIndex);
        }
        
        
        /// <summary>
        /// 귀속 엔딩 중에서 상태가 current인 엔딩이 있는지 체크 
        /// </summary>
        void CheckDependEndingCurrent() {
            
            if(!hasDependentEnding)
                return;
            
            
            for(int i=0;i<ListDependentEnding.Count;i++) {
                if(ListDependentEnding[i].episodeState == EpisodeState.Current) {
                    hasCurrentDependentEnding = true;
                    return;
                }
            }
        }
        
    } // ? end of class

}