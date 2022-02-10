using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace PIERStory
{
    public class TutorialSelection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image imageSelection;
        public Image selectionBar;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (selectionBar.fillAmount == 1f)
                return;

            selectionBar.DOKill();
            selectionBar.DOFillAmount(1, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (selectionBar.fillAmount == 1f)
                    UserManager.main.RequestTutorialReward();
            });
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            selectionBar.DOKill();
            selectionBar.DOFillAmount(0, 0.1f);
        }
    }
}