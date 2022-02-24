using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {

    public class FlowElement : MonoBehaviour
    {
        public EpisodeData currentEpisode;
        
        public RectTransform rectBody; 
        
        public Image imageIcon; // 왼쪽 아이콘 
        public Image imageOutline; //아웃라인 
        
        public TextMeshProUGUI textEpisodeNumber;
        public TextMeshProUGUI textEpisodeTitle;
        
        public GameObject groupOpenLock; // 기다리면 무료 그룹 
        public TextMeshProUGUI textsOpenTime;
        
        
        public RectTransform rectCover; // 미해금 엔딩 잠금 커버
        
        
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
        
        public Vector2 sizeNormal;
        public Vector2 sizeEnding;
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__episode"></param>
        public void InitFlowElement(EpisodeData __episode) {
            
            this.gameObject.SetActive(true);
            
            currentEpisode = __episode;
            
            textEpisodeTitle.text = currentEpisode.episodeTitle;
            textEpisodeNumber.text = currentEpisode.flowPrefix;
            

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
            rectBody.sizeDelta = sizeEnding;
            rectCover.sizeDelta = sizeEnding;
            textEpisodeNumber.rectTransform.anchoredPosition = endingEpisodeNumberPos;
            textEpisodeTitle.rectTransform.anchoredPosition = endingEpisodeTitlePos;
            
            // 해금된 경우 
            if(currentEpisode.endingOpen) {
                rectCover.gameObject.SetActive(false);
            }
            else {
                textEpisodeTitle.text = "????"; // 미해금된 경우 엔딩 제목을 노출하지 않음 
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
            rectBody.sizeDelta = sizeNormal;
            rectCover.sizeDelta = sizeNormal;
            textEpisodeNumber.rectTransform.anchoredPosition = normalEpisodeNumberPos;
            textEpisodeTitle.rectTransform.anchoredPosition = normalEpisodeTitlePos;
            
            
            
            switch(currentEpisode.episodeState) {
                case EpisodeState.Prev:
                
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
                textsOpenTime.text = string.Empty;
                
                
                textEpisodeNumber.color = colorInactiveTextColor;
                textEpisodeTitle.color = colorInactiveTextColor;
                
                
                imageOutline.color = Color.white; // 미래 아웃라인 색상 변경
                
                break;
            }
        }
        
        
        public void OnClickArrow() {
            
        }
        
        public void OnClickIcon() {
            
        }

    }
}