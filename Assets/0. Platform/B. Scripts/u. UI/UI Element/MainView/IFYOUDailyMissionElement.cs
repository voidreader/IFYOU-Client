using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;

namespace PIERStory
{
    public class IFYOUDailyMissionElement : MonoBehaviour
    {
        public IFYOURewardElement rewardCurrency;
        public TextMeshProUGUI missionText;

        public Image buttonImage;
        public TextMeshProUGUI buttonLabel;

        int missionNo = 2;
        bool interactable = false;
        
        public int currentResult = 0;
        public int limitCount = 0;
        public int received = 0; 

        /// <summary>
        /// 이프유 데일리 미션 초기화
        /// </summary>
        /// <param name="__j"></param>
        public void InitDailyMission(JsonData __j)
        {
            missionNo = SystemManager.GetJsonNodeInt(__j, "mission_no");
            SystemManager.SetText(missionText, string.Format("{0} ({1}/{2})", SystemManager.GetJsonNodeString(__j, "content"), SystemManager.GetJsonNodeInt(__j, "current_result"), SystemManager.GetJsonNodeInt(__j, "limit_count")));
            rewardCurrency.InitDailyMissionReward(__j);
            
            
            currentResult = SystemManager.GetJsonNodeInt(__j, "current_result");
            limitCount = SystemManager.GetJsonNodeInt(__j, "limit_count");
            received = SystemManager.GetJsonNodeInt(__j, "received");

            switch (SystemManager.GetJsonNodeInt(__j, "state"))
            {
                case 0:
                    buttonImage.sprite = LobbyManager.main.spriteDailyMissionOngoing;
                    SystemManager.SetLocalizedText(buttonLabel, "5209");
                    buttonLabel.color = HexCodeChanger.HexToColor("333333");
                    break;
                case 1:
                    buttonImage.sprite = LobbyManager.main.spriteDailyMissionClaim;
                    SystemManager.SetLocalizedText(buttonLabel, "5212");
                    buttonLabel.color = Color.white;
                    break;
                case 2:
                    buttonImage.sprite = LobbyManager.main.spriteDailyMissionFinish;
                    SystemManager.SetLocalizedText(buttonLabel, "5210");
                    buttonLabel.color = HexCodeChanger.HexToColor("999999");
                    break;
            }

            // interactable = SystemManager.GetJsonNodeInt(__j, "state") == 1 ? true : false;
            
            if(currentResult >= limitCount && received == 0)
                interactable = true;
            else    
                interactable = false;
        }


        /// <summary>
        /// 데일리 미션 보상 받기
        /// </summary>
        public void OnClickGetMissionReward()
        {
            if (!interactable)
                return;

            UserManager.main.RequestDailyMissionReward(missionNo, CallbackGetMissionReward);
        }

        void CallbackGetMissionReward(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackGetMissionReward");

                JsonData errordata = JsonMapper.ToObject(res.DataAsText);

                // 이미 받았다고 하면 화면 갱신을 해주자
                if (SystemManager.GetJsonNodeInt(errordata, "code") == 6123)
                {
                    NetworkLoader.main.RequestIfyouplayList();
                    StartCoroutine(RefreshIfyouplayScreen());
                }

                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            UserManager.main.SetBankInfo(result);
            UserManager.main.userIfyouPlayJson[LobbyConst.NODE_ATTENDANCE_MISSION] = result[LobbyConst.NODE_ATTENDANCE_MISSION];
            UserManager.main.userIfyouPlayJson[LobbyConst.NODE_DAILY_MISSION] = result[LobbyConst.NODE_DAILY_MISSION];

            MainIfyouplay.OnRefreshIfyouplay?.Invoke();
        }


        IEnumerator RefreshIfyouplayScreen()
        {
            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());

            MainIfyouplay.OnRefreshIfyouplay?.Invoke();
        }
    }
}