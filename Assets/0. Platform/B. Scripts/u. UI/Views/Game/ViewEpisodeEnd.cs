using UnityEngine;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;
using DanielLochner.Assets.SimpleScrollSnap;

namespace PIERStory
{
    public class ViewEpisodeEnd : CommonView
    {
        public ImageRequireDownload episodeImage;
        public EpisodeEndControls episodeEndControls;


        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
            
            episodeEndControls.InitStoryLobbyControls();

        }

        public override void OnHideView()
        {
            base.OnHideView();

        }


        #region OnClick event

        public void OnClickNextEpisode()
        {
            
            Debug.Log(">> OnClickNextEpisode << ");
            
            // Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, nextData, string.Empty);
            // Signal.Send(LobbyConst.STREAM_GAME, GameConst.SIGNAL_NEXT_EPISODE, string.Empty);
        }

        public void OnClickRetryEpisode()
        {
            GameManager.main.RetryPlay();
        }

        public void OnClickReturnLobby()
        {
            UserManager.main.gameComplete = true;
            GameManager.main.EndGame();
        }

        #endregion
    }
}
