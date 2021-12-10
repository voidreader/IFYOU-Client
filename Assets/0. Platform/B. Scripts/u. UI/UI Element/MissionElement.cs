using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using TMPro;
using LitJson;

namespace PIERStory
{
    public class MissionElement : MonoBehaviour
    {
        public ImageRequireDownload missionThumbnail;
        public TextMeshProUGUI missionTitle;
        public TextMeshProUGUI missionHint;

        public GameObject rewardInfo;
        public GameObject completeMark;

        public Image currencyIcon;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI currencyAmount;
        public TextMeshProUGUI missionState;
        public Image stateButtonImage;

        MissionState state;


        Color32 missionGreen = new Color32(69, 198, 80, 255);
        
        [SerializeField] MissionData missionData;

        public void InitMission(MissionData __missionData)
        {
            missionData = __missionData;
            
            
            missionThumbnail.SetDownloadURL(missionData.imageURL, missionData.imageKey);
            missionTitle.text = missionData.missionName;
            missionHint.text = missionData.missionHint;

            expText.text = string.Format("EXP {0}", missionData.rewardExp);

            SetCurrencyIcon(missionData.rewardCurrency, missionData.rewardQuantity.ToString());
            SetMissionState(missionData.missionState);
        }

        void SetCurrencyIcon(string __type, string __amount)
        {
            switch (__type)
            {
                case CommonConst.NONE:
                    currencyIcon.gameObject.SetActive(false);
                    return;
                        
                case LobbyConst.COIN:
                    break;

                case LobbyConst.GEM:
                    break;

                default:
                    break;
            }

            currencyAmount.text = string.Format("<b>{0}</b><size=17>개</size>", __amount);
        }

        void SetMissionState(MissionState __state)
        {
            rewardInfo.SetActive(true);
            completeMark.SetActive(false);
            
            state = __state;

            switch (__state)
            {
                case MissionState.unlocked:
                    missionState.color = Color.white;
                    stateButtonImage.color = missionGreen;
                    break;
                case MissionState.finish:
                    rewardInfo.SetActive(false);
                    completeMark.SetActive(true);
                    break;

                default:
                    missionState.color = missionGreen;
                    stateButtonImage.color = Color.white;
                    break;
            }
        }

        public void OnClickGetReward()
        {
            // 열려 있는 상태가 아니라면 동작하지 않는다
            if (state != MissionState.unlocked)
                return;
                
            UserManager.main.GetMissionRewared(missionData.missionID, missionData.rewardCurrency, missionData.rewardQuantity.ToString(), CallbackGetMissionReward);
        }
        
        void CallbackGetMissionReward(HTTPRequest req, HTTPResponse res)
        {
            /*
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                SystemManager.ShowAlert("통신 실패!");
                return;
            }
            
            JsonData resposeData = JsonMapper.ToObject(res.DataAsText);
            
            // node 갱신 
            UserManager.main.currentStoryJson["missions"] = resposeData["userMissionList"];
            // 재화 갱신
            UserManager.main.SetBankInfo(resposeData);

            storyMission.SetGetAllButton();
            
            // 미션 리스트 갱신 
            // * 2021.09.15 연출을 위해 view에서 갱신을 하지 않음. 
            // storyMission.SetMissionList(resposeData["userMissionList"]);
            
            // * 성공 했다. => 미션이 해금도 되었고, 보상도 받은 상태가 되는거다. 
            SetMissionComplete();
            */
        }
    }
    
    
}
