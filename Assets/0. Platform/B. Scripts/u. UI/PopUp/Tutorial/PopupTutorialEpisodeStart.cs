using UnityEngine;
using UnityEngine.SceneManagement;

using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class PopupTutorialEpisodeStart : PopupBase
    {
        EpisodeData episodeData;

        /// <summary>
        /// 게임 시작!
        /// </summary>
        public void OnClicGameStart()
        {
            episodeData = StoryManager.main.RegularEpisodeList[0];

            SystemManager.main.givenEpisodeData = episodeData;
            episodeData.SetPurchaseState();

            // 에디터 환경에서 인터넷 체크 안함
            if (Application.isEditor)
                RealStart();
            else
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    SystemManager.ShowMessageWithLocalize("80074", false);
                    return;
                }

                if (PlayerPrefs.GetInt(SystemConst.KEY_NETWORK_DOWNLOAD) == 1)
                    RealStart();
                else
                {
                    if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                        RealStart();
                    else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                        SystemManager.ShowLobbyPopup(SystemManager.GetLocalizedText("80075"), ChangeNetworkSetting, null);
                }
            }
            
        }

        void RealStart()
        {
            Signal.Send(LobbyConst.STREAM_GAME, "deadEnd", string.Empty);
            IntermissionManager.isMovingLobby = false; // 게임으로 진입하도록 요청
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single).allowSceneActivation = true;

            GameManager.SetNewGame();

            NetworkLoader.main.UpdateUserProjectCurrent(episodeData.episodeID, null, 0);
            UserManager.main.UpdateFirstTutorial(int.Parse(StoryManager.main.CurrentProjectID));
            UserManager.main.UpdateTutorialStep(2);
        }

        void ChangeNetworkSetting()
        {
            PlayerPrefs.SetInt(SystemConst.KEY_NETWORK_DOWNLOAD, 1);
            RealStart();
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