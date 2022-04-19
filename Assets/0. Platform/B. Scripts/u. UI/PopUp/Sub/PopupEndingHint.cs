using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class PopupEndingHint : PopupBase
    {
        [Space(15)]
        public VerticalLayoutGroup hintListContent;

        public TextMeshProUGUI dependsEpisodeText;
        public GameObject dependEpisodeRadioButton;

        public GameObject abilityBoxPrefab;

        public override void Show()
        {
            base.Show();

            JsonData endingHint = UserManager.main.EndingHintData(Data.contentEpisode.episodeID);
            EpisodeData dependsEpisode = StoryManager.GetRegularEpisodeByID(Data.contentEpisode.episodeID);

            // 귀속 에피소드 세팅
            string episodeNum = dependsEpisode.episodeNumber < 10 ? "0" + dependsEpisode.episodeNO : dependsEpisode.episodeNO;
            dependsEpisodeText.text = string.Format("{0} {1}", SystemManager.GetLocalizedText("5027"), episodeNum);

            // 해당 귀속 에피소드 클리어 체크
            dependEpisodeRadioButton.SetActive(UserManager.main.IsCompleteEpisode(dependsEpisode.episodeID));

            JsonData abilityHint = SystemManager.GetJsonNode(endingHint, "ability_condition");
            // 엔딩에 필요한 능력치 계수 세팅
            if (abilityHint == null || abilityHint.Count < 1)
                hintListContent.padding.top = 50;
            else
            {
                hintListContent.padding.top = 0;

                EndingHintAbility hintAbility = null;

                for (int i = 0; i < abilityHint.Count; i++)
                {
                    hintAbility = Instantiate(abilityBoxPrefab, hintListContent.transform).GetComponent<EndingHintAbility>();
                    hintAbility.InitHintAbility(abilityHint[i]);
                }
            }
        }
    }
}