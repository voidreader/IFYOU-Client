using System.Collections;
using UnityEngine;

using DanielLochner.Assets.SimpleScrollSnap;

namespace PIERStory
{
    public class PopupTutorialSlide : PopupBase
    {
        public GameObject previousButton;
        public GameObject nextButton;
        public SimpleScrollSnap scrollSnap;

        public TutorialSelection selection;

        private IEnumerator Start()
        {
            UserManager.main.UpdateTutorialStep(1);
            yield return new WaitUntil(() => selection.selectionBar.fillAmount == 1f);
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());
            Hide();
        }

        // Update is called once per frame
        void Update()
        {
            if (scrollSnap.CurrentPanel == 0)
                previousButton.SetActive(false);
            else
                previousButton.SetActive(true);


            if (scrollSnap.CurrentPanel == scrollSnap.Panels.Length - 1)
                nextButton.SetActive(false);
            else
                nextButton.SetActive(true);
        }


        public void CloseTutorial()
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