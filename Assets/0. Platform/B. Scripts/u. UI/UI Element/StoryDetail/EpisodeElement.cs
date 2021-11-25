using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

namespace PIERStory {
    public class EpisodeElement : MonoBehaviour
    {
        
        JsonData episodeData;  // 단일 에피소드 정보
        JsonData purchaseData; // 에피소드 구매 정보
        
        
        
        [SerializeField] ImageRequireDownload thumbnailImage; 
        
        
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
        
        [Space]
        [Space]
        [Header("데이터")]

        public string episodeId = string.Empty;
        public string episodeType = string.Empty;
        [SerializeField] string title = string.Empty;
        
        [SerializeField] string endingType = string.Empty;  // 엔딩 타입 
        [SerializeField] string dependEpisode = string.Empty;  // 의존 에피소드 
        [SerializeField] bool endingOpen = false; // 엔딩 오픈 여부 
        [SerializeField] float totalSceneCount = 0f;         // 진행률(분모)
        [SerializeField] float playedSceneCount = 0f;        // 플레이어 진행률(분자)
        
        [SerializeField] string salePrice = string.Empty;
        [SerializeField] int intPrice = 0; // 가격 int
        
        string squareImageURL = string.Empty; // 목록 사각 썸네일 
        string squareImageKey = string.Empty; 
        
        // ! 2021.08.26 episodeState 변경한다. prev, current, future 3종으로. 
        [SerializeField] EpisodeState episodeState = EpisodeState.Future; //
        [SerializeField] PurchaseState purchaseState = PurchaseState.None; // 구매 상태 
        
        
        
        /// <summary>
        /// 에피소드 리셋 
        /// </summary>
        void ResetData() {
            
            squareImageURL = string.Empty;
            squareImageKey = string.Empty;
            
            
            thumbnailImage.InitImage();
            playStateCover.gameObject.SetActive(false);
            currentPlayFlag.gameObject.SetActive(false);
            
            textEpisodeNumbering.text = string.Empty;
            textEpisodeTitle.text = string.Empty;
            
            purchaseData = null;
            purchaseState = PurchaseState.None; 
            
            
        }
        
        
        /// <summary>
        /// 에피소드 초기화
        /// </summary>
        /// <param name="__j"></param>
        public void InitElement(JsonData __j) {
            
            this.gameObject.SetActive(true);
            
            ResetData();
            
            episodeData = __j;
            
            // 자주 참조하는 데이터 가져다놓기
            episodeId = SystemManager.GetJsonNodeString(episodeData, "episode_id");
            episodeType = SystemManager.GetJsonNodeString(episodeData, "episode_type");
            title = SystemManager.GetJsonNodeString(episodeData, "title");
            squareImageURL = SystemManager.GetJsonNodeString(episodeData, LobbyConst.TITLE_IMAGE_URL);
            squareImageKey = SystemManager.GetJsonNodeString(episodeData, LobbyConst.TITLE_IMAGE_KEY);
            salePrice = SystemManager.GetJsonNodeString(episodeData, LobbyConst.EPISODE_SALE_PRICE);
            
            // 목록 썸네일 처리 
            thumbnailImage.SetDownloadURL(squareImageURL, squareImageKey);
            
            // 타이틀 
            textEpisodeTitle.text = title;
            
            // 에피소드 타입에 따라. 
            switch(episodeType) {
                case "chapter":
                textEpisodeNumbering.text = SystemManager.GetLocalizedText("5027");
                break;
                
                case "side":
                textEpisodeNumbering.text = SystemManager.GetLocalizedText("5028");
                break;
            }
            
            SetPlayState(); // 플레이 상태 처리 
            
            SetPurchaseState(); // 구매 상태 처리 
            
            SetPlayStateCover(); // 플레이 상태에 대한 처리 
        }
        
        /// <summary>
        /// 에피소드 플레이 상태 설정 
        /// </summary>
        void SetPlayState() {
            string currentRegularEpisodeID = string.Empty;
            
            if(episodeType == "side") {
                episodeState = EpisodeState.Current;
                return;
            }
            
            
            // 아직 작업중인 작품은 값이 없을 수 있다.
            if(UserManager.main.GetUserProjectRegularEpisodeCurrent() == null) {
                currentRegularEpisodeID = string.Empty;
            }
            else {
                currentRegularEpisodeID = UserManager.main.GetUserProjectRegularEpisodeCurrent()["episode_id"].ToString();
            }
            
            
            // current의 ID와 현재 에피소드 ID가 같음. 
            if(currentRegularEpisodeID == episodeId) {
                episodeState = EpisodeState.Current;
            }
            else { // 다른 경우에 과거, 미래 체크 
            
                // episode Progress 테이블에 있음 
                if (UserManager.main.CheckEpisodeProgress(episodeId)) {
                    episodeState = EpisodeState.Prev; // 과거 
                }    
                else { // 없으면 미래.
                    episodeState = EpisodeState.Future; // 미래 
                }
            }
        }
        
        
        
        /// <summary>
        /// 구매 상태 설정 
        /// </summary>
        void SetPurchaseState() {
            string episodePurchaseState = string.Empty;
            int.TryParse(salePrice, out intPrice);
            
            // 구매내역을 purchaseDate로 out 
            // * purchaseState 정한다. 
            if (UserManager.main.CheckPurchaseEpisode(episodeId, ref purchaseData))
            {
                Debug.Log(JsonMapper.ToStringUnicode(purchaseData));
                
                if(purchaseData["purchase_type"].ToString() == "Permanent")
                {
                    purchaseState = PurchaseState.Permanent; // 영구적인 구매 상태 

                    // 구매 했어도 가격이 원래 무료면 FREE 처리
                    if(intPrice < 1)
                        purchaseState = PurchaseState.Free;
                }
                else if (purchaseData["purchase_type"].ToString() == "OneTime"){ // OneTime
                    purchaseState = PurchaseState.OneTime;
                }
                else if (purchaseData["purchase_type"].ToString() == "Rent"){ // 
                    purchaseState = PurchaseState.Rent;
                }
            }
            else
            {
                // 구매 내역이 없는 경우에 대한 처리 
                if(intPrice < 1)
                    purchaseState = PurchaseState.Free; // 무료!
                
            }
            
            purchaseStateBox.gameObject.SetActive(true);
            
            // 구매 상태에 따라서 purchaseStateBox 처리 
            switch(purchaseState) {
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

                    episodePurchaseState = string.Format("대여 중 | <size=18>{0}</size>", remainTime);

                    break;

                case PurchaseState.Free: // * 무료
                    episodePurchaseState = "FREE";
                    purchaseStateBox.color = LobbyManager.main.colorFreeBox;
                    break;

                case PurchaseState.Permanent: // * 프리미엄 
                    episodePurchaseState = SystemManager.GetLocalizedText("6006");
                    purchaseStateBox.color = LobbyManager.main.colorPremiumBox;
                    break;

                case PurchaseState.OneTime: // * 1회 플레이
                    episodePurchaseState = SystemManager.GetLocalizedText("6005");
                    purchaseStateBox.color = LobbyManager.main.colorOneTimeBox;
                    break;
                
            } // ? end of switch
            
            
            // 구매 상태 텍스트 처리, 크기 처리 
            textPurchaseState.text = episodePurchaseState;
            Vector2 preferredSize = textPurchaseState.GetPreferredValues(episodePurchaseState);
            purchaseStateBox.rectTransform.sizeDelta = preferredSize;
            
                        
        } // ? end of SetPurchaseState
        
        
        
        /// <summary>
        /// 플레이 상태에 따른 커버 및 아이콘 처리 
        /// </summary>
        void SetPlayStateCover() {
            
            
            // 스페셜, 사이드 에피소드는 커버처리 없다.
            if(episodeType == "side") {
                episodeState = EpisodeState.Current;
                return;
            }
            
            
            playStateCover.gameObject.SetActive(true);
            playStateIcon.gameObject.SetActive(true);
            
            switch(episodeState) {
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
        /// 목록 클릭
        /// </summary>
        public void OnClickElement() {
            
        }
        
        
    } // ? end of class

}