using UnityEngine;

using DG.Tweening;

namespace PIERStory
{
    public class PopupTutorialMission_1 : PopupBase
    {
        [Space]
        public RectTransform pointerIcon;

        public override void Show()
        {
            base.Show();

            pointerIcon.DOAnchorPosY(-440, 2f).SetLoops(-1, LoopType.Yoyo);
        }


        public void OnClickPlayButton()
        {
            //  튜토리얼 단계 업데이트 후 플레이
            pointerIcon.DOKill();

            StoryLobbyMain.OnEpisodePlay?.Invoke();

        }
    }
}