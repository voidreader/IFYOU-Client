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
        public ImageRequireDownload missionThumbnail;
        public GameObject hiddenHighlight;

        public TextMeshProUGUI missionText;
        
        
        public GameObject rewardInfo;
        public GameObject getRewardButton;          // 미션 보상받기
        public GameObject completeMark;             // 미션 완료 도장
        
        public CanvasGroup rewardCanvasGroup; // 리워드 미션 캔버스 그룹 

        public ImageRequireDownload currencyIcon;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI currencyAmount;
        public Image border;
        public Image button;
        public TextMeshProUGUI getRewardText;

        Color pinkColor = new Color32(255, 163, 212, 255);
        Color orangeColor = new Color32(254, 200, 150, 255);
        Color violetColor = new Color32(197, 198, 254, 255);

        Color disableGreyColor = new Color32(248, 248, 248, 255);

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
            MissionGradeColor();

            // 버튼 비활성화 표기
            if (missionData.missionState == MissionState.locked)
            {
                getRewardText.color = HexCodeChanger.HexToColor("C4C4C4");
                button.color = disableGreyColor;
            }
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
            completeMark.SetActive(false);
            
            state = __state;

            switch (__state)
            {
                case MissionState.unlocked:
                    getRewardButton.GetComponent<UIButton>().interactable = true;
                    break;
                case MissionState.finish:
                    rewardInfo.SetActive(false);
                    completeMark.SetActive(true);
                    break;

                default:
                    getRewardButton.GetComponent<UIButton>().interactable = false;
                    break;
            }
        }


        /// <summary>
        /// 미션 등급화
        /// </summary>
        void MissionGradeColor()
        {
            getRewardText.color = HexCodeChanger.HexToColor("404040");

            // 미션 등급화
            if (missionData.rewardExp >= 100)
            {
                border.color = Color.white;
                button.color = Color.white;

                border.sprite = LobbyManager.main.spriteGradientBorder;
                button.sprite = LobbyManager.main.spriteGradientButton;
                return;
            }


            border.sprite = LobbyManager.main.spriteWhiteBorder;
            button.sprite = LobbyManager.main.spriteWhiteButton;

            if (missionData.rewardExp >= 20)
            {
                border.color = violetColor;
                button.color = new Color(violetColor.r, violetColor.g, violetColor.b, violetColor.a * 0.7f);
                return;
            }

            if (missionData.rewardExp >= 7)
            {
                border.color = orangeColor;
                button.color = new Color(orangeColor.r, orangeColor.g, orangeColor.b, orangeColor.a * 0.7f);
                return;
            }


            if (missionData.rewardExp >= 1)
            {
                border.color = pinkColor;
                button.color = new Color(pinkColor.r, pinkColor.g, pinkColor.b, pinkColor.a * 0.7f);
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
