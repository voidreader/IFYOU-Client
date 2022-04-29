using System;
using System.Collections.Generic;

using LitJson;

namespace PIERStory
{

    /// <summary>
    /// 미션 상태 
    /// </summary>
    public enum MissionState
    {
        locked, // 잠김
        unlocked, // 해금 (보상받기전)
        finish // 보상 받음
    }

    /// <summary>
    /// 미션 타입
    /// </summary>
    public enum MissionType
    {
        drop, // drop 미션 
        scene, // 사건 클리어 
        illust, // 일러스트 수집
        episode // 에피소드 클리어 
    }

    [Serializable]
    public class MissionData
    {
        JsonData missionJSON; // 미션 원본 

        public int missionID = 0; // 미션 ID
        public string missionName = string.Empty; // 미션 이름
        public string originName = string.Empty; // 미션 스크립트용 이름 

        public string missionHint = string.Empty; // 미션 힌트

        public MissionType missionType; // 미션 타입 

        public bool isHidden = false; // 히든 

        public string rewardCurrency = string.Empty; // 보상재화
        public string currency_icon_url = string.Empty;
        public string currency_icon_key = string.Empty;
        public int rewardQuantity = 0; // 수량 
        public int rewardExp = 0; // 경험지


        public string imageURL = string.Empty;
        public string imageKey = string.Empty;
        public MissionState missionState; // 미션 상태

        public bool detailHint = false;
        public List<string> episodeDetailHint;      // 미션이 에피소드 타입일 때, 들어올 미션 힌트

        public struct EventDetailHintData
        {
            public string episodeId;
            public int played;
            public int total;
        }

        public List<EventDetailHintData> eventDetailHint;   // 미션이 사건 타입일 때, 들어올 미션 힌트

        public MissionData(JsonData __j)
        {
            SetEpisodeData(__j);
        }

        public void SetEpisodeData(JsonData __j)
        {
            missionJSON = __j;
            InitData();
        }

        void InitData()
        {
            if (missionJSON == null)
                return;

            missionID = SystemManager.GetJsonNodeInt(missionJSON, "mission_id");
            missionName = SystemManager.GetJsonNodeString(missionJSON, "mission_name");
            originName = SystemManager.GetJsonNodeString(missionJSON, "origin_name");
            missionHint = SystemManager.GetJsonNodeString(missionJSON, "mission_hint");

            isHidden = SystemManager.GetJsonNodeBool(missionJSON, "is_hidden");

            rewardCurrency = SystemManager.GetJsonNodeString(missionJSON, "reward_currency");
            rewardExp = SystemManager.GetJsonNodeInt(missionJSON, "reward_exp");
            rewardQuantity = SystemManager.GetJsonNodeInt(missionJSON, "reward_quantity");

            imageURL = SystemManager.GetJsonNodeString(missionJSON, "image_url");
            imageKey = SystemManager.GetJsonNodeString(missionJSON, "image_key");

            currency_icon_url = SystemManager.GetJsonNodeString(missionJSON, "icon_image_url");
            currency_icon_key = SystemManager.GetJsonNodeString(missionJSON, "icon_image_key");

            string originMissionType = SystemManager.GetJsonNodeString(missionJSON, "mission_type");
            string originUnlockState = SystemManager.GetJsonNodeString(missionJSON, "unlock_state");

            // 미션 타입 
            switch (originMissionType)
            {
                case "drop":
                    missionType = MissionType.drop;
                    break;
                case "illust":
                    missionType = MissionType.illust;
                    break;
                case "event":
                    missionType = MissionType.scene;
                    break;
                case "episode":
                    missionType = MissionType.episode;
                    break;
            }

            // 미션 상태
            switch (originUnlockState)
            {
                case "0":
                    missionState = MissionState.unlocked;
                    break;
                case "1":
                    missionState = MissionState.finish;
                    break;
                default:
                    missionState = MissionState.locked;
                    break;
            }

            detailHint = SystemManager.GetJsonNodeBool(missionJSON, "detail_hint");

            SetDetailHint();
        }

        void SetDetailHint()
        {
            if (missionType != MissionType.episode && missionType != MissionType.scene)
                return;

            JsonData detailHint = SystemManager.GetJsonNode(missionJSON, "hint");

            if (detailHint == null || detailHint.Count < 1)
                return;

            if(missionType == MissionType.episode)
            {
                episodeDetailHint = new List<string>();

                for (int i = 0; i < detailHint.Count; i++)
                    episodeDetailHint.Add(detailHint[i].ToString());
            }


            if (missionType == MissionType.scene)
            {
                eventDetailHint = new List<EventDetailHintData>();

                for (int i = 0; i < detailHint.Count; i++)
                {
                    EventDetailHintData eventHintData = new EventDetailHintData();
                    eventHintData.episodeId = SystemManager.GetJsonNodeString(detailHint[i], CommonConst.COL_EPISODE_ID);
                    eventHintData.played = SystemManager.GetJsonNodeInt(detailHint[i], "played");
                    eventHintData.total = SystemManager.GetJsonNodeInt(detailHint[i], "total");
                    eventDetailHint.Add(eventHintData);
                }
            }
        }
    }
}