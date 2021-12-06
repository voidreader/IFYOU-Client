using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PIERStory {
    
    /// <summary>
    /// 엔딩 에피소드 element.
    /// </summary>
    public class EndingEpisodeElement : MonoBehaviour
    {
        [SerializeField] ImageRequireDownload endingThumbnail;
        [SerializeField] TextMeshProUGUI textEndingType; // 엔딩 타입 
        [SerializeField] TextMeshProUGUI textEndingTitle; // 엔딩 타이틀 
        
        ThreeEpisodeRow parentThreeRow; // 부모 ThreeRow
        
        [SerializeField] EpisodeData endingData;
        
        // 귀속된 엄마 엔딩을 보여주기 위한 화살표 친구들 
        [SerializeField] GameObject leftArrow;
        [SerializeField] GameObject centerArrow;
        [SerializeField] GameObject rightArrow;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__data"></param>
        /// <param name="__rowIndex"></param>
        /// <param name="__colIndex"></param>
        public void InitEndingElement(ThreeEpisodeRow __parentThreeRow, EpisodeData __data, int __rowIndex, int __colIndex) {
           
           this.gameObject.SetActive(true);
           parentThreeRow = __parentThreeRow;
           
            
            // 기본 데이터 세팅 
            endingData = __data;
            endingThumbnail.SetDownloadURL(__data.squareImageURL, __data.squareImageKey);
           
            if(__data.endingType == "hidden") {
                textEndingType.text = SystemManager.GetLocalizedText("5087");
            }
            else {
                textEndingType.text = SystemManager.GetLocalizedText("5088");
            }
            
            textEndingTitle.text = endingData.episodeTitle;
            
            
            // 맨 위의 (rowIndex = 0) 엔딩은 colIndex에 따라서 화살표를 표기해줘야한다. 
            leftArrow.SetActive(false);
            centerArrow.SetActive(false);
            rightArrow.SetActive(false);
            if(__rowIndex > 0) {
                return;
            }
            
            // 3개가 합쳐서 하나를 이루기 때문에 0~2까지 체크해서 화살표를 알맞은 위치에 띄워준다.
            switch(__colIndex) {
                case 0:
                leftArrow.SetActive(true);
                break;
                case 1:
                centerArrow.SetActive(true);
                break;
                case 2:
                rightArrow.SetActive(true);
                break;
            }
        }
        
        /// <summary>
        /// 엔딩 클릭시, 에피소드 시작팝업 호출
        /// </summary>
        public void OnClickEnding() {
            Debug.Log(">> OnClick EndingElement");
            
            Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, endingData, string.Empty);
        }
        
        /// <summary>
        /// 우측 하단의 접기 누르기 
        /// </summary>
        public void OnClickFoldEnding() {
            // ThreeRow에서 접어주고, 각 에피소드의 펼침 버튼 다시 표기
            parentThreeRow.FoldEnding();
        }
    }
}