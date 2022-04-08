using UnityEngine;

using DG.Tweening;
using Doozy.Runtime.Reactor;

namespace PIERStory
{
    public class PopupGradeExp : PopupBase
    {
        [Space(15)]
        public RectTransform expBackAura;

        public UnityEngine.UI.Image expGauge;
        Progressor expProgressor;

        public override void Show()
        {
            base.Show();

            expBackAura.DORotate(new Vector3(0, 0, 360f), 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

            expProgressor = expGauge.GetComponent<Progressor>();
            expProgressor.fromValue = UserManager.main.gradeExperience / UserManager.main.upgradeGoalPoint;
            expProgressor.toValue = (Data.contentValue + UserManager.main.gradeExperience) / UserManager.main.upgradeGoalPoint;
        }

        public void ShowComplete()
        {
            expProgressor.Play();
        }

        public void OpenGradeUpPopup()
        {
            if (expProgressor.toValue >= 1f && expProgressor.currentValue == expProgressor.toValue)
            {
                PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_GRADE_UP);
                if (p == null)
                {
                    Debug.LogError("등급 업 팝업 없음");
                    return;
                }

                p.Data.SetImagesSprites(null);
                p.Data.SetLabelsTexts(string.Empty);

                PopupManager.main.ShowPopup(p, false);
                Hide();
            }
        }
    }
}