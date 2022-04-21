using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory
{
    public class PopupEndingHint : PopupBase
    {
        [Space(15)]
        EndingHintData endingHint;
        public VerticalLayoutGroup hintListContent;

        public TextMeshProUGUI dependsEpisodeText;
        public GameObject dependEpisodeRadioButton;

        public GameObject abilityBoxPrefab;

        public override void Show()
        {
            if (isShow)
                return;

            base.Show();

            endingHint = GetEndingHintData();
            string episodeNum = string.Empty;

            // 귀속 에피소드 세팅
            if (endingHint == null)
            {
                EpisodeData dependsEpisode = StoryManager.GetRegularEpisodeByID(Data.contentEpisode.dependEpisode);
                episodeNum = dependsEpisode.episodeNumber < 10 ? "0" + dependsEpisode.episodeNO : dependsEpisode.episodeNO;
                dependEpisodeRadioButton.SetActive(UserManager.main.IsCompleteEpisode(dependsEpisode.episodeID));
            }
            else
            {
                episodeNum = endingHint.dependEpisodeData.episodeNumber < 10 ? "0" + endingHint.dependEpisodeData.episodeNO : endingHint.dependEpisodeData.episodeNO;

                // 해당 귀속 에피소드 클리어 체크
                dependEpisodeRadioButton.SetActive(UserManager.main.IsCompleteEpisode(endingHint.dependEpisodeData.episodeID));
            }
            
            dependsEpisodeText.text = string.Format("{0} {1}", SystemManager.GetLocalizedText("5027"), episodeNum);


            // 엔딩에 필요한 능력치 계수 세팅
            if (endingHint == null || endingHint.abilityConditions.Count < 1)
                hintListContent.padding.top = 50;
            else
            {
                hintListContent.padding.top = 0;

                EndingHintAbility hintAbility = null;

                for (int i = 0; i < endingHint.abilityConditions.Count; i++)
                {
                    hintAbility = Instantiate(abilityBoxPrefab, hintListContent.transform).GetComponent<EndingHintAbility>();
                    hintAbility.InitHintAbility(endingHint.abilityConditions[i].speaker, endingHint.abilityConditions[i].abilityName, endingHint.abilityConditions[i].oper, endingHint.abilityConditions[i].value);
                }
            }
        }

        EndingHintData GetEndingHintData()
        {
            foreach(EndingHintData hintData in StoryManager.main.listEndingHint)
            {
                if (hintData.endingId == Data.contentEpisode.episodeID)
                    return hintData;
            }

            return null;
        }
    }
}