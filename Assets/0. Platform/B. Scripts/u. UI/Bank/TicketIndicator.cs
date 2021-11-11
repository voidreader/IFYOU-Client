using UnityEngine;

using TMPro;
using DG.Tweening;

namespace PIERStory
{
    public class TicketIndicator : MonoBehaviour
    {
        public TextMeshProUGUI ticketAmountText;
        public RectTransform icon;

        int currentValue = 0;
        int nextValue = 0;

        private void OnEnable()
        {
            if (UserManager.main == null || StoryManager.main == null || string.IsNullOrEmpty(StoryManager.main.CurrentProjectID))
                return;

            UserManager.main.ticketIndicators = this;

            if (StoryManager.main.CurrentProjectID.Equals("57"))
                currentValue = UserManager.main.GetOneTimeProjectTicket("57");
            else if(StoryManager.main.CurrentProjectID.Equals("60"))
                currentValue = UserManager.main.GetOneTimeProjectTicket("60");

            ticketAmountText.text = string.Format("{0}", currentValue);
        }

        /// <summary>
        /// 1회권 갱신
        /// </summary>
        public void RefreshTicket()
        {
            if (StoryManager.main.CurrentProjectID.Equals("57"))
                nextValue = UserManager.main.GetOneTimeProjectTicket("57");
            else if (StoryManager.main.CurrentProjectID.Equals("60"))
                nextValue = UserManager.main.GetOneTimeProjectTicket("60");

            ticketAmountText.DOCounter(currentValue, nextValue, 0.2f, true, null);
            icon.localScale = Vector3.one;
            icon.DOScale(1.2f, 0.2f).SetLoops(4, LoopType.Yoyo);

            currentValue = nextValue;
        }
    }

}




