using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace PIERStory
{
    public class PopupEndingHint : PopupBase
    {
        [Space(15)]
        public VerticalLayoutGroup hintListContent;

        public GameObject dependEpisodeRadioButton;

        public override void Show()
        {
            base.Show();


            hintListContent.padding.top = 50;
            hintListContent.padding.top = 0;
        }
    }
}