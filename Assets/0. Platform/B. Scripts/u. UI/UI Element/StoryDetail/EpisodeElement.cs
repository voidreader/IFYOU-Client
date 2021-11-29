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
        
        JsonData episodeJSON;  // 단일 에피소드 정보
        JsonData purchaseData; // 에피소드 구매 정보
        
        
        
        [SerializeField] ImageRequireDownload thumbnailImage; 
        [SerializeField] EpisodeData episodeData = null; 
        
        
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
        }
        
        
        /// <summary>
        /// 에피소드 초기화
        /// </summary>
        /// <param name="__j"></param>
        public void InitElement(JsonData __j) {
            
            this.gameObject.SetActive(true);
            
            ResetData();
            
            episodeJSON = __j;
            
            // 에피소드 정보 클래스 데이터 세팅 
            if(episodeData == null) 
                episodeData = new EpisodeData(episodeJSON);
            else 
                episodeData.SetEpisodeData(episodeJSON);
            
            
            // 목록 썸네일 처리 
            thumbnailImage.SetDownloadURL(episodeData.squareImageURL, episodeData.squareImageKey);
            
            // 타이틀 
            textEpisodeTitle.text = episodeData.combinedEpisodeTitle;
            
            // 에피소드 타입에 따라. 
            switch(episodeData.episodeType) {
                case "chapter":
                textEpisodeNumbering.text = SystemManager.GetLocalizedText("5027");
                break;
                
                case "side":
                textEpisodeNumbering.text = SystemManager.GetLocalizedText("5028");
                break;
            }
            
            
            SetPurchaseState(); // 구매 상태 처리 
            
            SetPlayStateCover(); // 플레이 상태에 대한 처리 
        }
        
        
        
        /// <summary>
        /// 구매 상태 설정 
        /// </summary>
        void SetPurchaseState() {
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
                    episodePurchaseStateText = "FREE";
                    purchaseStateBox.color = LobbyManager.main.colorFreeBox;
                    break;

                case PurchaseState.Permanent: // * 프리미엄 
                    episodePurchaseStateText = SystemManager.GetLocalizedText("6006");
                    purchaseStateBox.color = LobbyManager.main.colorPremiumBox;
                    break;

                case PurchaseState.OneTime: // * 1회 플레이
                    episodePurchaseStateText = SystemManager.GetLocalizedText("6005");
                    purchaseStateBox.color = LobbyManager.main.colorOneTimeBox;
                    break;
                
            } // ? end of switch
            
            
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
        /// 목록 클릭
        /// </summary>
        public void OnClickElement() {
            
            Debug.Log(">> OnClickElement");
            
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, episodeData, string.Empty);
        }
        
        
    } // ? end of class

}