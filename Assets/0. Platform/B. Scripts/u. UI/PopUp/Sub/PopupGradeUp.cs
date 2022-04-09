using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;

namespace PIERStory
{
    public class PopupGradeUp : PopupBase
    {
        [Space(15)]
        public RectTransform gradeBadgeContents;
        public Image badgeAura;

        public TextMeshProUGUI seasonEndText;

        public Button overlayClose;
        public GameObject closeButton;

        public override void Show()
        {
            base.Show();

            gradeBadgeContents.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBounce);
            badgeAura.rectTransform.DORotate(new Vector3(0, 0, 360f), 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

            seasonEndText.text = string.Format(SystemManager.GetLocalizedText("6295"), UserManager.main.remainDay);
            StartCoroutine(WaitParticleEffect());
        }

        IEnumerator WaitParticleEffect()
        {
            yield return new WaitForSeconds(2.5f);
            overlayClose.interactable = true;
            closeButton.SetActive(true);
        }
    }
}