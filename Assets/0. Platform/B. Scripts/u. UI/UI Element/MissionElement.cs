using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class MissionElement : MonoBehaviour
    {
        [Header("미션 이미지 관련")]
        public ImageRequireDownload missionThumbnail;
        public GameObject hiddenHighlight;

        public TextMeshProUGUI missionText;
        public GameObject hiddenMissionTexts;   // 히든 미션 표기 관련 object
        public TextMeshProUGUI hiddenText;      // 히든 미션 관련 텍스트
        

        [Space][Header("미션 진행도 관련")]
        public GameObject missionProgress;
        public TextMeshProUGUI missionProgressText;
        public TextMeshProUGUI missionPercent;
        public Image missionGauge;

        [Space(20)][Header("미션 보상 관련")]
        public GameObject rewardInfo;
        
        public ImageRequireDownload currencyIcon;
        public TextMeshProUGUI currencyAmount;
        public TextMeshProUGUI expText;

        public Image rewardButton;
        public TextMeshProUGUI getRewardText;

        Color pinkColor = new Color32(255, 163, 212, 255);
        Color orangeColor = new Color32(254, 200, 150, 255);
        Color violetColor = new Color32(197, 198, 254, 255);

        Color disableGreyColor = new Color32(248, 248, 248, 255);

        public MissionState state;

        
        [SerializeField] MissionData missionData;

        public void InitMission(MissionData __missionData)
        {
            missionData = __missionData;
            missionThumbnail.gameObject.SetActive(true);
            hiddenHighlight.SetActive(false);
            hiddenMissionTexts.SetActive(false);
            missionText.gameObject.SetActive(true);

            missionThumbnail.SetDownloadURL(missionData.imageURL, missionData.imageKey);
            
            missionText.text = missionData.missionName;

            expText.text = missionData.rewardExp.ToString();

            SetCurrencyIcon(missionData.rewardQuantity);
            SetMissionState(missionData.missionState);
            //MissionGradeColor();

            // 버튼 비활성화 표기
            /*
            if (missionData.missionState == MissionState.locked)
            {
                getRewardText.color = HexCodeChanger.HexToColor("C4C4C4");
                button.sprite = LobbyManager.main.spriteGetReward;
                button.color = disableGreyColor;
            }
            */

            gameObject.SetActive(true);
        }

        /// <summary>
        /// 히든 미션 표기
        /// </summary>
        /// <param name="lockCount"></param>
        public void HighlightHidden(int lockCount)
        {
            gameObject.SetActive(true);

            missionThumbnail.gameObject.SetActive(false);
            hiddenHighlight.SetActive(true);
            hiddenMissionTexts.SetActive(true);
            missionText.gameObject.SetActive(false);
            rewardInfo.SetActive(false);
            missionProgress.SetActive(false);
            rewardButton.gameObject.SetActive(false);

            hiddenText.text = string.Format(SystemManager.GetLocalizedText("6056"), lockCount);
        }

        /// <summary>
        /// 보상 재화 설정
        /// </summary>
        /// <param name="__amount"></param>
        void SetCurrencyIcon(int __amount)
        {
            currencyIcon.SetDownloadURL(missionData.currency_icon_url, missionData.currency_icon_key);
            currencyAmount.text = string.Format("{0}", __amount);
        }


        void SetMissionState(MissionState __state)
        {
            rewardInfo.SetActive(true);
            missionProgress.SetActive(false);
            rewardButton.gameObject.SetActive(false);
            
            state = __state;

            switch (__state)
            {
                case MissionState.unlocked:
                    rewardButton.sprite = LobbyManager.main.spriteGetReward;
                    getRewardText.color = HexCodeChanger.HexToColor("333333");
                    rewardButton.gameObject.SetActive(true);
                    break;
                case MissionState.finish:
                    rewardButton.sprite = LobbyManager.main.spriteGetReward;
                    getRewardText.color = HexCodeChanger.HexToColor("C4C4C4");
                    rewardButton.gameObject.SetActive(true);
                    break;

                default:
                    missionProgressText.text = string.Format(SystemManager.GetLocalizedText("5042") + "(0/1)");
                    missionPercent.text = "0%";
                    missionGauge.fillAmount = 0f;
                    missionProgress.SetActive(true);
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
            rewardButton.GetComponent<UIButton>().interactable = false;
            
            JsonData resposeData = JsonMapper.ToObject(res.DataAsText);
            
            // 재화 획득 팝업 
            SystemManager.ShowResourcePopup(SystemManager.GetLocalizedText("6123"), missionData.rewardQuantity, missionData.currency_icon_url, missionData.currency_icon_key);
            
            // 미션 상태 변경 (보상 받고, 완료로 변경)
            missionData.missionState = MissionState.finish; 
            UserManager.main.SetMissionData(missionData.missionID, missionData);
            
            StoryContentsButton.onStoryContentsButtonMission?.Invoke();
            
            
            // * 성공 했다. => 미션이 해금도 되었고, 보상도 받은 상태가 되는거다. 
            SetMissionComplete();

            /*
            if(!ViewMission.clickGetAll)
                SystemManager.ShowSimpleAlertLocalize("6123");
            */
        }
        
        /// <summary>
        /// 미션 보상 받는 연출 시작
        /// </summary>
        void SetMissionComplete() {
            
            // 경험치 처리 
            NetworkLoader.main.UpdateUserExp(missionData.rewardExp, "mission", missionData.missionID);
            RefreshViewMission();
            //rewardCanvasGroup.DOFade(0, 0.2f).OnComplete(CompleteStep2);
            // RewardMask.DOFade(1, 0.2f).OnComplete(CompleteStep2);

            //completeMark.transform.localScale = Vector3.one * 1.5f;

        }
        
        void CompleteStep2() {
            //completeMark.gameObject.SetActive(true);
            //completeMark.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.InQuad).OnComplete(RefreshViewMission);
            
            // ViewMission.OnCompleteReward?.Invoke();
        }
        
        void RefreshViewMission() {
            // ViewMission.OnCompleteReward?.Invoke();
            ViewMission.OnRefreshProgressor?.Invoke();
            rewardButton.GetComponent<UIButton>().interactable = true;
        }
    }
    
    
}
