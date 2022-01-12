
using TMPro;


namespace PIERStory
{
    public class PopupTutorialStoryDetail : PopupBase
    {
        public TextMeshProUGUI episodeNo;
        public TextMeshProUGUI episodeTitle;

        public override void Show()
        {
            base.Show();

            EpisodeData episodeData = StoryManager.main.RegularEpisodeList[0];

            episodeNo.text = string.Format("{0} {1}", SystemManager.GetLocalizedText("5027"), episodeData.episodeNO);
            episodeTitle.text = episodeData.episodeTitle;
        }


        /// <summary>
        /// 빠른 플레이버튼 눌러서 에피소드 시작 페이지 열기
        /// </summary>
        public void OnClickOpenEpisodeStart()
        {
            Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_COMMON, LobbyConst.SIGNAL_EPISODE_START, StoryManager.main.RegularEpisodeList[0], string.Empty);
            Hide();
        }


        public void OnClickCloseTutorial()
        {
            SystemManager.ShowLobbyPopup(SystemManager.GetLocalizedText("80108"), CancelTutorial, null);
        }

        void CancelTutorial()
        {
            UserManager.main.UpdateTutorialStep(3);
            Hide();
        }
    }
}