
namespace PIERStory
{
    public class PopupTutorialStoryDetail : PopupBase
    {

        /// <summary>
        /// 빠른 플레이버튼 눌러서 에피소드 시작 페이지 열기
        /// </summary>
        public void OnClickOpenEpisodeStart()
        {
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, StoryManager.main.RegularEpisodeList[0], string.Empty);
            PopupManager.main.CurrentQueuePopup.Hide();
        }


        public void OnClickCloseTutorial()
        {
            SystemManager.ShowLobbyPopup(SystemManager.GetLocalizedText("80108"), CancelTutorial, null);
        }

        void CancelTutorial()
        {
            UserManager.main.UpdateTutorialStep(3);
            PopupManager.main.CurrentQueuePopup.Hide();
        }
    }
}