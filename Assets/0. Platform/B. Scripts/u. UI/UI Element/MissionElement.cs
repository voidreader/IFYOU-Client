using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Components;
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

        public TextMeshProUGUI missionText;
        
        
        public GameObject rewardInfo;
        public GameObject missionProceeding;        // 미션 진행중
        public GameObject getRewardButton;          // 미션 보상받기
        public GameObject completeMark;             // 미션 완료 도장
        
        public CanvasGroup rewardCanvasGroup; // 리워드 미션 캔버스 그룹 

        public ImageRequireDownload currencyIcon;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI currencyAmount;
        // public TextMeshProUGUI missionState;
        
        
        public MissionState state;

        
        [SerializeField] MissionData missionData;

        public void InitMission(MissionData __missionData)
        {
            gameObject.SetActive(true);
            
            rewardCanvasGroup.alpha = 1;
            
            
            missionData = __missionData;
            missionThumbnail.gameObject.SetActive(true);
            hiddenHighlight.SetActive(false);
            

            missionThumbnail.SetDownloadURL(missionData.imageURL, missionData.imageKey);
            
            missionText.text = string.Format("<size=24><b>{0}</b></size>\n{1}", missionData.missionName, missionData.missionHint);
            
            // missionTitle.text = missionData.missionName;
            // missionHint.text = missionData.missionHint;

            expText.text = string.Format("EXP {0}", missionData.rewardExp);

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
                    getRewardButton.GetComponent<UIButton>().interactable = true;
                    
                    missionProceeding.SetActive(false);
                    break;
                case MissionState.finish:
                    rewardInfo.SetActive(false);
                    completeMark.SetActive(true);
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
                // SystemManager.ShowMessageAlert("통신 실패!", false);
                return;
            }

            Debug.Log("> CallbackGetMissionReward : " + res.DataAsText);
            getRewardButton.GetComponent<UIButton>().interactable = false;
            
            JsonData resposeData = JsonMapper.ToObject(res.DataAsText);
            
            // 재화 획득 팝업 
            SystemManager.ShowResourcePopup(SystemManager.GetLocalizedText("6123"), missionData.rewardQuantity, missionData.currency_icon_url, missionData.currency_icon_key);
            
            // 미션 상태 변경 (보상 받고, 완료로 변경)
            missionData.missionState = MissionState.finish; 
            UserManager.main.SetMissionData(missionData.missionID, missionData);
            
            StoryContentsButton.onStoryContentsButtonMission?.Invoke();
            
            
            // * 성공 했다. => 미션이 해금도 되었고, 보상도 받은 상태가 되는거다. 
            SetMissionComplete();

            if(!ViewMission.clickGetAll)
                SystemManager.ShowSimpleAlertLocalize("6123");
        }
        
        /// <summary>
        /// 미션 보상 받는 연출 시작
        /// </summary>
        void SetMissionComplete() {
            
            // 경험치 처리 
            NetworkLoader.main.UpdateUserExp(missionData.rewardExp, "mission", missionData.missionID);
            
            rewardCanvasGroup.DOFade(0, 0.2f).OnComplete(CompleteStep2);
            // RewardMask.DOFade(1, 0.2f).OnComplete(CompleteStep2);
            
            completeMark.transform.localScale = Vector3.one * 1.5f;

        }
        
        void CompleteStep2() {
            completeMark.gameObject.SetActive(true);
            completeMark.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.InQuad).OnComplete(RefreshViewMission);
            
            // ViewMission.OnCompleteReward?.Invoke();
        }
        
        void RefreshViewMission() {
            // ViewMission.OnCompleteReward?.Invoke();
            ViewMission.OnRefreshProgressor?.Invoke();
        }
    }
    
    
}
