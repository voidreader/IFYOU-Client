using System.Collections.Generic;
using UnityEngine;

namespace PIERStory
{
    public class PopupSelectionHint : PopupBase
    {
        [Space(15)]
        string targetSceneId = string.Empty;
        List<EndingHintData> endingList = new List<EndingHintData>();

        public override void Show()
        {
            base.Show();

            targetSceneId = Data.contentValue.ToString();
            FillEndingList();
        }

        void FillEndingList()
        {
            endingList.Clear();

            foreach (EndingHintData hintdata in StoryManager.main.listEndingHint)
            {
                for (int i = 0; i < hintdata.unlockScenes.Length; i++)
                {
                    // 타겟 scene Id가 힌트 scene id에 포함되어 있으면 list에 포함시킨다
                    if (hintdata.unlockScenes[i] == targetSceneId)
                    {
                        endingList.Add(hintdata);
                        break;
                    }
                }
            }
        }
    }
}