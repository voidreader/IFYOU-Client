using System.Collections;
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

        public UnityEngine.UI.Button playButton;
        public RectTransform pointerIcon;
        public ParticleSystem coinFirecracker;

        public override void Show()
        {
            base.Show();
            
            SystemManager.SetText(tutorialMissionText, string.Format(SystemManager.GetLocalizedText("5167"), 1));
            SystemManager.SetText(rewardText, string.Format(SystemManager.GetLocalizedText("5168"), 100));

            pointerIcon.DOAnchorPosY(-440, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }


        public void OnClickPlayButton()
        {
            UserManager.main.UpdateTutorialStep(1, 1, CallbackUpdateTutorial);
            playButton.interactable = false;
        }

        void CallbackUpdateTutorial(BestHTTP.HTTPRequest req, BestHTTP.HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackTutorialUpdate, Tutorial Mission1");
                playButton.interactable = true;
                return;
            }

            coinFirecracker.Play(true);

            pointerIcon.DOKill();
            pointerIcon.gameObject.SetActive(false);

            StartCoroutine(WaitParticleEnd());
        }

        /// <summary>
        /// 파티클 연출 종료 후 게임 시작
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitParticleEnd()
        {
            yield return new WaitUntil(() => coinFirecracker.isStopped);
            
            // 튜토리얼 단계 업데이트 후 플레이
            StoryLobbyMain.OnEpisodePlay?.Invoke();
        }
    }
}