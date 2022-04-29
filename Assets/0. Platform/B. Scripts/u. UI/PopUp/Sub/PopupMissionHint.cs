using UnityEngine;

namespace PIERStory
{
    public class PopupMissionHint : PopupBase
    {
        [Space(15)]
        public RectTransform popupBox;

        public GameObject missionHintPrefab;
        public Transform scrollContent;

        const float simpleSize = 225f;
        const float detailSize = 472f;

        public override void Show()
        {
            if (isShow)
                return;

            base.Show();

            if (Data.isPositive)
                popupBox.sizeDelta = new Vector2(popupBox.sizeDelta.x, detailSize);
            else
            {
                popupBox.sizeDelta = new Vector2(popupBox.sizeDelta.x, simpleSize);
                return;
            }

            MissionData missionData = UserManager.main.DictStoryMission[Data.contentValue];
            string title = string.Empty, amount = string.Empty;
            MissionHintElement hintElement = null;
            EpisodeData episodeData = null;

            // 미션타입이 에피소드인 경우
            if (missionData.missionType == MissionType.episode)
            {
                for (int i = 0; i < missionData.episodeDetailHint.Count; i++)
                {
                    hintElement = Instantiate(missionHintPrefab, scrollContent).GetComponent<MissionHintElement>();

                    foreach (EpisodeData epiData in StoryManager.main.ListCurrentProjectEpisodes)
                    {
                        if (epiData.episodeID == missionData.episodeDetailHint[i])
                        {
                            episodeData = epiData;
                            break;
                        }
                    }

                    if (episodeData.episodeType == EpisodeType.Chapter)
                        title = string.Format("{0} {1:D2}", SystemManager.GetLocalizedText("5027"), episodeData.episodeNumber);
                    else
                        title = episodeData.episodeTitle;

                    amount = UserManager.main.IsCompleteEpisode(episodeData.episodeID) ? "1/1" : "0/1";
                    hintElement.InitMissionHint(UserManager.main.IsCompleteEpisode(missionData.episodeDetailHint[i]), title, amount);

                }
            }

            // 미션 타입이 사건인 경우
            else if(missionData.missionType == MissionType.scene)
            {
                for(int i=0;i<missionData.eventDetailHint.Count;i++)
                {
                    hintElement = Instantiate(missionHintPrefab, scrollContent).GetComponent<MissionHintElement>();

                    foreach(EpisodeData epiData in StoryManager.main.ListCurrentProjectEpisodes)
                    {
                        if(epiData.episodeID == missionData.eventDetailHint[i].episodeId)
                        {
                            episodeData = epiData;
                            break;
                        }
                    }

                    if (episodeData.episodeType == EpisodeType.Chapter)
                        title = string.Format("{0} {1:D2}", SystemManager.GetLocalizedText("5027"), episodeData.episodeNumber);
                    else
                        title = episodeData.episodeTitle;

                    amount = string.Format("{0}/{1}", missionData.eventDetailHint[i].played, missionData.eventDetailHint[i].total);
                    hintElement.InitMissionHint(missionData.eventDetailHint[i].played >= missionData.eventDetailHint[i].total, title, amount);

                }
            }

        }
    }
}