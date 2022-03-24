using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

namespace PIERStory {
    
public enum StoryElementType {
    general,
    category,
    future
}

/// <summary>
/// * 2022.03.23 추가 
/// </summary>
public class LobbyStoryElement : MonoBehaviour
{
    public ImageRequireDownload bannerImage; // 배너 이미지 
    public GameObject groupLikeStat; // 좋아요 카운팅 통계 
    public TextMeshProUGUI textLikeCount; // 좋아요 카운트 텍스트 
    
    public StoryData storyData = null; // 작품 정보
    
    
  
    public StoryElementType elementType = StoryElementType.general;
    
    /// <summary>
    /// 기본 초기화 
    /// </summary>
    /// <param name="__story"></param>
    public void Init(StoryData __story, StoryElementType __type) {
        storyData = __story;
        elementType = __type;
        
        bannerImage.SetDownloadURL(storyData.categoryImageURL, storyData.categoryImageKey);
        
        this.gameObject.SetActive(true);
        
        if(__type == StoryElementType.general && groupLikeStat != null) {
            groupLikeStat.gameObject.SetActive(false);
        }
    }
    
    public void OnClickElement() {
        if(elementType == StoryElementType.general || elementType == StoryElementType.general) {
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE, storyData);
        }
        
    }
    
}

}