using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using TMPro;
using LitJson;
using DG.Tweening;

namespace PIERStory
{
    public class MissionElement : MonoBehaviour
    {
        public ImageRequireDownload missionThumbnail;
        public GameObject hiddenHighlight;
        public Image RewardMask;
        public TextMeshProUGUI missionText;
        
        public GameObject rewardInfo;
        public GameObject missionProceeding;        // 미션 진행중
        public GameObject getRewardButton;          // 미션 보상받기
        public GameObject completeMark;             // 미션 완료 도장

        public ImageRequireDownload currencyIcon;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI currencyAmount;
        // public TextMeshProUGUI missionState;
        
        
        public MissionState state;

        
        [SerializeField] MissionData missionData;

        public void InitMission(MissionData __missionData)
        {
            gameObject.SetActive(true);
            
            missionData = __missionData;
            missionThumbnail.gameObject.SetActive(true);
            hiddenHighlight.SetActive(false);
            

            missionThumbnail.SetDownloadURL(missionData.imageURL, missionData.imageKey);
            
            missionText.text = string.Format("<size=24><b>{0}</b></size>\n{1}", missionData.missionName, missionData.missionHint);
            
            // missionTitle.text = missionData.missionName;
            // missionHint.text = missionData.missionHint;

            expText.text = string.Format("EXP {0}", missionData.rewardExp);
            RewardMask.color = new Color(1,1,1,0);
            RewardMask.raycastTarget = false;

            SetCurrencyIcon(missionData.rewardQuantity);
            SetMissionState(missionData.missionState);
        }

        public void HighlightHidden(int lockCount)
        {
            gameObject.SetActive(true);

            missionThumbnail.gameObject.SetActive(false);
            hiddenHighlight.SetActive(true);
            rewardInfo.SetActive(false);
            completeMark.SetActive(false);

            missionText.text = string.Format("<size=24><b>"+SystemManager.GetLocalizedText("6056")+"</b></size>\n"+SystemManager.GetLocalizedText("6057"), lockCount);
        }

        void SetCurrencyIcon(int __amount)
        {
            currencyIcon.SetDownloadURL(missionData.currency_icon_url, missionData.currency_icon_key);
            currencyAmount.text = string.Format("{0}", __amount);
        }

        void SetMissionState(MissionState __state)
        {
            rewardInfo.SetActive(true);
            getRewardButton.SetActive(false);
            missionProceeding.SetActive(false);
            completeMark.SetActive(false);
            
            state = __state;

            switch (__state)
            {
                case MissionState.unlocked:
                    getRewardButton.SetActive(true);
                    missionProceeding.SetActive(false);
                    break;
                case MissionState.finish:
                    rewardInfo.SetActive(false);
                    completeMark.SetActive(true);
                    RewardMask.color = new Color(1,1,1,1);
                    RewardMask.raycastTarget = true;
                    break;

                default:
                    getRewardButton.SetActive(false);
                    missionProceeding.SetActive(true);
                    break;
            }
        }

        public void OnClickGetReward()
        {
            // 열려 있는 상태가 아니라면 동작하지 않는다
            if (state != MissionState.unlocked)
                return;
                
            UserManager.main.GetMissionRewared(missionData, CallbackGetMissionReward);
        }
        
        void CallbackGetMissionReward(HTTPRequest req, HTTPResponse res)
        {
            
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                SystemManager.ShowMessageAlert("통신 실패!", false);
                return;
            }
            
            JsonData resposeData = JsonMapper.ToObject(res.DataAsText);
            
            
            // 재화 갱신
            UserManager.main.SetBankInfo(resposeData);
            
            // 미션 상태 변경 (보상 받고, 완료로 변경)
            missionData.missionState = MissionState.finish; 
            

            
            // 미션 리스트 갱신 
            // * 2021.09.15 연출을 위해 view에서 갱신을 하지 않음. 
            // storyMission.SetMissionList(resposeData["userMissionList"]);
            
            // * 성공 했다. => 미션이 해금도 되었고, 보상도 받은 상태가 되는거다. 
            SetMissionComplete();

            if(!ViewMission.clickGetAll)
                SystemManager.ShowSimpleAlertLocalize("6123");
        }
        
        /// <summary>
        /// 미션 보상 받는 연출 시작
        /// </summary>
        void SetMissionComplete() {
            RewardMask.DOFade(1, 0.2f).OnComplete(CompleteStep2);
            RewardMask.raycastTarget = true;
            completeMark.transform.localScale = Vector3.one * 1.5f;

        }
        
        void CompleteStep2() {
            completeMark.gameObject.SetActive(true);
            completeMark.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.InQuad).OnComplete(RefreshViewMission);
            
            // ViewMission.OnCompleteReward?.Invoke();
        }
        
        void RefreshViewMission() {
            ViewMission.OnCompleteReward?.Invoke();
        }
    }
    
    
}
