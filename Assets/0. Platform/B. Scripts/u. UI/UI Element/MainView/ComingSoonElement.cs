using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class ComingSoonElement : MonoBehaviour
    {
        public ImageRequireDownload bannerImage;
        public TextMeshProUGUI titleText;


        /// <summary>
        /// 커밍순 데이터 삽입
        /// </summary>
        public void InitComingStoryData(string __url, string __key, string __title)
        {
            bannerImage.SetDownloadURL(__url, __key);
            titleText.text = __title;

            gameObject.SetActive(true);
        }
    }
}