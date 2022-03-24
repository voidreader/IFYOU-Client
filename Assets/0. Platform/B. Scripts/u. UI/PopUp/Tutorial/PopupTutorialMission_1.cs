using UnityEngine;

using TMPro;
using DG.Tweening;

namespace PIERStory
{
    public class PopupTutorialMission_1 : PopupBase
    {
        [Space(15)]
        public TextMeshProUGUI tutorialMissionText;
        public TextMeshProUGUI rewardText;

        public RectTransform pointerIcon;

        public override void Show()
        {
            base.Show();

            tutorialMissionText.text = string.Format(SystemManager.GetLocalizedText("5167"), 1);
            rewardText.text = string.Format(SystemManager.GetLocalizedText("5168"), 100);

            pointerIcon.DOAnchorPosY(-440, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }


        public void OnClickPlayButton()
        {
            UserManager.main.UpdateTutorialStep(1, CallbackUpdateTutorial);
        }

        void CallbackUpdateTutorial(BestHTTP.HTTPRequest req, BestHTTP.HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackTutorialUpdate, Tutorial Mission1");
                return;
            }

            // 튜토리얼 단계 업데이트 후 플레이
            pointerIcon.DOKill();
            StoryLobbyMain.OnEpisodePlay?.Invoke();
            SystemManager.ShowNetworkLoading();
        }
    }
}