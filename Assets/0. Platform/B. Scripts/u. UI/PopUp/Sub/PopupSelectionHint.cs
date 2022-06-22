using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory
{
    public class PopupSelectionHint : PopupBase
    {
        [Space(15)]
        public GameObject selectionHintElementPrefab;
        public Transform prefabParent;


        public override void Show()
        {
            if (isShow)
                return;

            base.Show();

            GameObject g = null;
            TextMeshProUGUI[] texts = null;

            foreach(EndingHintData hintData in StoryManager.main.selectedEndingHintList)
            {
                g = Instantiate(selectionHintElementPrefab, prefabParent);
                texts = g.GetComponentsInChildren<TextMeshProUGUI>();

                if (hintData.isHidden)
                    g.GetComponent<Image>().sprite = GameManager.main.spriteSelectionUnlockedBase;
                else
                    g.GetComponent<Image>().sprite = GameManager.main.spriteSelectionNormalBase;

                SystemManager.SetText(texts[0], hintData.endingType);
                SystemManager.SetText(texts[1], hintData.endingTitle);
            }


        }
    }
}