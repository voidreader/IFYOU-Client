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
        
        public GameObject notify; // 신규 표시

        JsonData userGalleryData = null; // galleryImages 노드의 데이터 
        JsonData elementData = null;

        public bool illustOpen = false;
        public bool isLobbyOpen = false; // 로비의 갤러리에서 한번이라도 열어본적이 있는지 
        bool isLive = false;
        bool isMinicut = false;
        string illustName = string.Empty;
        string illustPublicName = string.Empty;
        string summary = string.Empty;
        string appearEpisodeId = string.Empty;
        string appearEpisodeType = string.Empty;
        string illustType = string.Empty;

        
        const string APPEAR_EPISODE_ID = "appear_episode";
        const string APPEAR_EPISODE_TYPE = "appear_episode_type";
        
        
        
        /// <summary>
        /// 데이터만 갱신한다.
        /// </summary>
        /// <param name="__newData"></param>
        public void SetIllustData(JsonData __newData) {
            userGalleryData = __newData;
            CheckNotify();
        }

        public void InitElementInfo(JsonData __j)
        {
            this.gameObject.SetActive(true);
            
            userGalleryData = __j;
            
            elementData = null;
            illustName = SystemManager.GetJsonNodeString(__j, LobbyConst.ILLUST_NAME);
            summary = SystemManager.GetJsonNodeString(__j, LobbyConst.SUMMARY);
            illustType = SystemManager.GetJsonNodeString(__j, "illust_type");
            
            // set native 되지 않게 변경함. 2022.01.17 
            illustThumbnail.SetDownloadURL(SystemManager.GetJsonNodeString(__j, LobbyConst.THUMBNAIL_URL), SystemManager.GetJsonNodeString(__j, LobbyConst.THUMBNAIL_KEY), true);
            isLive = SystemManager.GetJsonNodeString(__j, CommonConst.ILLUST_TYPE).Contains("live");
            isMinicut = illustType == "live_object" || illustType == "minicut" ? true : false;
            liveTag.SetActive(isLive);

            #region 일러스트 획득 관련 세팅

            illustOpen = SystemManager.GetJsonNodeBool(__j, CommonConst.ILLUST_OPEN);
            isLobbyOpen = SystemManager.GetJsonNodeBool(__j, "gallery_open");
            appearEpisodeId = SystemManager.GetJsonNodeString(__j, APPEAR_EPISODE_ID);
            appearEpisodeType = SystemManager.GetJsonNodeString(__j, APPEAR_EPISODE_TYPE);
            lockOverlay.SetActive(!illustOpen);
            illustButton.interactable = illustOpen;

            // 일러스트를 획득하지 못한 경우에만 실행한다
            if(!illustOpen)
            {
                // 사이드 에피소드인 경우
                if (appearEpisodeType.Equals(CommonConst.COL_SIDE))
                    episodeHintText.text = SystemManager.GetLocalizedText("5028");
                else
                {
                    EpisodeData appearEpisodeData = StoryManager.GetNextFollowingEpisodeData(appearEpisodeId);
                    if(appearEpisodeData == null || !appearEpisodeData.isValidData) {
                        Debug.LogError("Wrong appear episode ID : "+ appearEpisodeId);
                    }

                    // 엔딩인 경우
                    if (appearEpisodeType.Equals(CommonConst.COL_ENDING))
                    {
                        // 히든 엔딩 표기
                        if(appearEpisodeData.endingType == LobbyConst.COL_HIDDEN) 
                            episodeHintText.text = SystemManager.GetLocalizedText("5087");
                        else 
                            episodeHintText.text = SystemManager.GetLocalizedText("8004");
                    }
                    else
                    {
                        // 정규 에피소드인 경우
                        //episodeHintText.text = string.Format(SystemManager.GetLocalizedText("5027") + " {0}", appearEpisodeData.episodeNO);
                        episodeHintText.text = string.Format(SystemManager.GetLocalizedText("5027"));
                    }
                }
            }

            #endregion

            illustPublicName = SystemManager.GetJsonNodeString(__j, CommonConst.COL_PUBLIC_NAME);

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
                    elementData = StoryManager.main.GetPublicMinicutJsonByName(illustName);
            }

            #endregion
            
            CheckNotify();


            gameObject.SetActive(true);
        }
        
        void CheckNotify() {
            
            isLobbyOpen = SystemManager.GetJsonNodeBool(userGalleryData, "gallery_open");
            
            if(illustOpen && !isLobbyOpen) {
                notify.gameObject.SetActive(true);
            }
            else {
                notify.gameObject.SetActive(false);
            }            
        }

        public void OnClickIllustDetail()
        {
            ViewGallery.OnDelayIllustOpen?.Invoke(false);
            SystemManager.ShowNetworkLoading();
            ViewIllustDetail.SetData(elementData, isLive, isMinicut, illustPublicName, summary, userGalleryData);

            if (isLive)
            {
                int scaleOffset = 0;

                if (elementData[0].ContainsKey("scale_offset"))
                    scaleOffset = SystemManager.GetJsonNodeInt(elementData[0], "scale_offset");

                LobbyManager.main.SetGalleryLiveIllust(illustName, scaleOffset, isMinicut);
            }
            else
            {
                Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SHOW_ILLUSTDETAIL, string.Empty);
            }
        }
    }
}
