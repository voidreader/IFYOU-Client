using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace PIERStory
{
    /// <summary>
    /// * 2022.03.23 추가 
    /// </summary>
    public class LobbyStoryElement : MonoBehaviour
    {
        public ImageRequireDownload bannerImage; // 배너 이미지 
        
        public GameObject TagGroup;             // 태그들이 모여있는 object
        public List<GameObject> publicTags;     // 태그들(조회수, 좋아요, 신규)
        public TextMeshProUGUI viewCount;
        public TextMeshProUGUI likeCount;

        public TextMeshProUGUI projectTitle;     // 작품 제목

        public StoryData storyData = null; // 작품 정보


        /// <summary>
        /// 기본 초기화 
        /// </summary>
        /// <param name="__story">storyData</param>
        /// <param name="isVertical">1*N이면 세로형, 2*N이면 가로형</param>
        public void Init(StoryData __story, bool isVertical, bool useFavorite, bool useView)
        {
            storyData = __story;
            bannerImage.SetDownloadURL(storyData.categoryImageURL, storyData.categoryImageKey);

            int hitCount = storyData.hitCount * 10, favoriteCount = storyData.likeCount * 10;

            TagGroup.SetActive(useFavorite || useView);
            publicTags[0].SetActive(useView && hitCount >= 100);
            publicTags[1].SetActive(useFavorite && favoriteCount >= 100);
            publicTags[2].SetActive(TagGroup.activeSelf && !publicTags[0].activeSelf && !publicTags[1].activeSelf);

            if (publicTags[0].activeSelf)
                FormatNumberSymbolic(hitCount);

            if (publicTags[1].activeSelf)
                FormatNumberSymbolic(favoriteCount);

            SystemManager.SetText(projectTitle, storyData.title);

            gameObject.SetActive(true);
        }

        public void OnClickElement()
        {
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE, storyData);
        }


        /// <summary>
        /// 숫자 자리수별 심볼 표기
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        string FormatNumberSymbolic(long num)
        {
            // string[] symbols = { "K", "M", "G", "T", "P", "E", "Z" };
            // 1k = 1000, 10k = 10000, 100k = 100000
            if (num >= 1000000)
                return (num / 1000000).ToString("0.#") + "M";

            if (num >= 100)
                return (num / 100).ToString("0.#") + "K";

            return string.Empty;
        }
    }
}