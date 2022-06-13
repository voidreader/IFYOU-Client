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

            try
            {
                // 미션타입이 에피소드인 경우
                if (missionData.missionType == MissionType.episode)
                {
                    for (int i = 0; i < missionData.episodeDetailHint.Count; i++)
                    {
                        hintElement = Instantiate(missionHintPrefab, scrollContent).GetComponent<MissionHintElement>();
                        episodeData = null;

                        foreach (EpisodeData epiData in StoryManager.main.ListCurrentProjectEpisodes)
                        {
                            try
                            {
                                if (epiData.episodeID == missionData.episodeDetailHint[i])
                                {
                                    episodeData = epiData;
                                    break;
                                }
                            }
                            catch
                            {
                                if (string.IsNullOrEmpty(epiData.episodeID))
                                    NetworkLoader.main.ReportRequestError("Error in missionHint #1, type Episode", string.Format("Error EpisodeData Episode ID is null, Mission ID = {0}", missionData.missionID));
                                else if (string.IsNullOrEmpty(missionData.episodeDetailHint[i]))
                                    NetworkLoader.main.ReportRequestError("Error in missionHint #1, type Episode", string.Format("Error MissionData HintEpisode ID is null, Mission ID = {0}", missionData.missionID));
                            }
                        }

                        try
                        {
                            title = episodeData.episodeType == EpisodeType.Chapter ? string.Format("{0} {1:D2}", SystemManager.GetLocalizedText("5027"), episodeData.episodeNumber) : episodeData.episodeTitle;
                            amount = UserManager.main.IsCompleteEpisode(episodeData.episodeID) ? "1/1" : "0/1";
                            hintElement.InitMissionHint(UserManager.main.IsCompleteEpisode(missionData.episodeDetailHint[i]), title, amount);
                        }
                        catch
                        {
                            NetworkLoader.main.ReportRequestError("Error in missionHint #2, type Episode", string.Format("Error HintEpisode ID = {0}, Mission ID = {1}", missionData.episodeDetailHint[i], missionData.missionID));
                        }
                    }
                }

                // 미션 타입이 사건인 경우
                else if (missionData.missionType == MissionType.scene)
                {
                    for (int i = 0; i < missionData.eventDetailHint.Count; i++)
                    {
                        hintElement = Instantiate(missionHintPrefab, scrollContent).GetComponent<MissionHintElement>();
                        episodeData = null;

                        foreach (EpisodeData epiData in StoryManager.main.ListCurrentProjectEpisodes)
                        {
                            try
                            {
                                if (epiData.episodeID == missionData.eventDetailHint[i].episodeId)
                                {
                                    episodeData = epiData;
                                    break;
                                }
                            }
                            catch
                            {
                                if (string.IsNullOrEmpty(epiData.episodeID))
                                    NetworkLoader.main.ReportRequestError("Error in missionHint #1, type Scene", string.Format("Error EpisodeData Episode ID is null, Mission ID = {0}", missionData.missionID));
                                else if (string.IsNullOrEmpty(missionData.eventDetailHint[i].episodeId))
                                    NetworkLoader.main.ReportRequestError("Error in missionHint #1, type Scene", string.Format("Error MissionData HintEpisode ID is null, Mission ID = {0}", missionData.missionID));
                            }
                        }


                        try
                        {
                            title = episodeData.episodeType == EpisodeType.Chapter ? string.Format("{0} {1:D2}", SystemManager.GetLocalizedText("5027"), episodeData.episodeNumber) : episodeData.episodeTitle;
                            amount = string.Format("{0}/{1}", missionData.eventDetailHint[i].played, missionData.eventDetailHint[i].total);
                            hintElement.InitMissionHint(missionData.eventDetailHint[i].played >= missionData.eventDetailHint[i].total, title, amount);
                        }
                        catch
                        {
                            NetworkLoader.main.ReportRequestError("Error in missionHint #2, type Scene", string.Format("Error HintEpisode ID = {0}, Mission ID = {1}", missionData.eventDetailHint[i].episodeId, missionData.missionID));
                        }

                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.StackTrace);
                NetworkLoader.main.ReportRequestError("Error in missionHint", string.Format("Error Mission ID = {0},\n{1}", missionData.missionID, e.StackTrace));
            }
        }
    }
}