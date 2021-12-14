using UnityEngine;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class IllustElement : MonoBehaviour
    {
        public UIButton illustButton;

        public ImageRequireDownload illustThumbnail;
        public GameObject liveTag;
        public GameObject lockOverlay;
        public TextMeshProUGUI episodeHintText;
        public TextMeshProUGUI illustPublicName;

        JsonData elementData = null;

        public bool illustOpen = false;
        bool isLive = false;
        bool isMinicut = false;
        string illustName = string.Empty;
        string summary = string.Empty;
        string appearEpisodeId = string.Empty;
        string appearEpisodeType = string.Empty;

        
        const string APPEAR_EPISODE_ID = "appear_episode";
        const string APPEAR_EPISODE_TYPE = "appear_episode_type";
        

        public void InitElementInfo(JsonData __j)
        {
            elementData = null;
            illustName = SystemManager.GetJsonNodeString(__j, LobbyConst.ILLUST_NAME);
            summary = SystemManager.GetJsonNodeString(__j, LobbyConst.SUMMARY);

            illustThumbnail.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.THUMBNAIL_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.THUMBNAIL_KEY));
            isLive = SystemManager.GetJsonNodeString(__j, CommonConst.ILLUST_TYPE).Equals(CommonConst.MODEL_TYPE_LIVE2D);
            isMinicut = SystemManager.GetJsonNodeBool(__j, CommonConst.COL_IS_MINICUT);
            liveTag.SetActive(isLive);

            #region 일러스트 획득 관련 세팅

            illustOpen = SystemManager.GetJsonNodeBool(__j, CommonConst.ILLUST_OPEN);
            appearEpisodeId = SystemManager.GetJsonNodeString(__j, APPEAR_EPISODE_ID);
            appearEpisodeType = SystemManager.GetJsonNodeString(__j, APPEAR_EPISODE_TYPE);
            lockOverlay.SetActive(!illustOpen);
            illustButton.interactable = illustOpen;

            // 일러스트를 획득하지 못한 경우에만 실행한다
            if(!illustOpen)
            {
                // 사이드 에피소드인 경우
                if (appearEpisodeType.Equals(CommonConst.COL_SIDE))
                    episodeHintText.text = "Side Episode";
                else
                {
                    JsonData appearEpisodeData = UserManager.main.FindEpisodeData(appearEpisodeType, appearEpisodeId);

                    // 엔딩인 경우
                    if (appearEpisodeType.Equals(CommonConst.COL_ENDING))
                    {
                        // 히든 엔딩 표기
                        if(SystemManager.GetJsonNodeString(appearEpisodeData, LobbyConst.ENDING_TYPE).Equals(LobbyConst.COL_HIDDEN))
                            episodeHintText.text = "Hidden Ending";
                        else
                            episodeHintText.text = "Final Ending";
                    }
                    else
                    {
                        // 정규 에피소드인 경우
                        int episodeNum = int.Parse(SystemManager.GetJsonNodeString(appearEpisodeData, CommonConst.COL_EPISODE_NO));

                        if (episodeNum < 10)
                            episodeHintText.text = string.Format("Episode 0{0}", episodeNum);
                        else
                            episodeHintText.text = string.Format("Episode {0}", episodeNum);

                    }
                }
            }

            #endregion

            illustPublicName.text = SystemManager.GetJsonNodeString(__j, CommonConst.COL_PUBLIC_NAME);

            #region Init elementData

            if (isLive)
            {
                if (!isMinicut)
                    elementData = StoryManager.main.GetLiveIllustJsonByName(illustName);
                else
                    elementData = StoryManager.main.GetLiveObjectJsonByName(illustName);
            }
            else
            {
                if (!isMinicut)
                    elementData = StoryManager.main.GetIllustJsonByIllustName(illustName);
                else
                    elementData = UserManager.main.GetPublicMinicutJsonByName(illustName);
            }

            #endregion

            gameObject.SetActive(true);
        }

        public void OnClickIllustDetail()
        {
            ViewIllustDetail.SetData(elementData, isLive, isMinicut, illustPublicName.text, summary);
            ViewGallery.OnDelayIllustOpen?.Invoke(false);

            if (isLive)
            {
                int scaleOffset = 0;

                if (elementData[0].ContainsKey("scale_offset"))
                    int.TryParse(SystemManager.GetJsonNodeString(elementData[0], "scale_offset"), out scaleOffset);

                LobbyManager.main.SetGalleryLiveIllust(illustName, scaleOffset, isMinicut);
            }
            else
            {
                Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SHOW_ILLUSTDETAIL, string.Empty);
            }
        }
    }
}
