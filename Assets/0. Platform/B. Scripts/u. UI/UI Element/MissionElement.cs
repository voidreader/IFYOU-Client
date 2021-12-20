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
        public TextMeshProUGUI missionText;
        

        public GameObject rewardInfo;
        public GameObject completeMark;

        public Image currencyIcon;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI currencyAmount;
        // public TextMeshProUGUI missionState;
        public Image btnClaim; // 보상받기 버튼
        public TextMeshProUGUI textClaimText;
        
        public Image RewardMask;
        
        

        MissionState state;


        Color32 missionGreen = new Color32(69, 198, 80, 255);
        
        [SerializeField] MissionData missionData;

        public void InitMission(MissionData __missionData)
        {
            this.gameObject.SetActive(true);
            
            missionData = __missionData;
            
            
            missionThumbnail.SetDownloadURL(missionData.imageURL, missionData.imageKey);
            
            missionText.text = string.Format("<size=24><b>{0}</b></size>\n{1}", missionData.missionName, missionData.missionHint);
            
            // missionTitle.text = missionData.missionName;
            // missionHint.text = missionData.missionHint;

            expText.text = string.Format("EXP {0}", missionData.rewardExp);
            RewardMask.color = new Color(1,1,1,0);
            RewardMask.raycastTarget = false;

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
                    currencyIcon.sprite = LobbyManager.main.spriteCoinIcon;
                    break;

                case LobbyConst.GEM:
                    currencyIcon.sprite = LobbyManager.main.spriteCoinIcon;
                    break;

                default:
                    // 그 외에는 다운로드 
                    break;
            }
            
            currencyIcon.SetNativeSize();

            //currencyAmount.text = string.Format("<b>{0}</b><size=17>개</size>", __amount);
            currencyAmount.text = string.Format("{0}", __amount);
        }

        void SetMissionState(MissionState __state)
        {
            rewardInfo.SetActive(true);
            completeMark.SetActive(false);
            btnClaim.gameObject.SetActive(false);
            
            state = __state;

            switch (__state)
            {
                case MissionState.unlocked:
                    btnClaim.gameObject.SetActive(true);
                
                    //textClaimText.color = Color.white;
                    //btnClaim.image.sprite = null;
                    break;
                case MissionState.finish:
                    rewardInfo.SetActive(false);
                    completeMark.SetActive(true);
                    RewardMask.color = new Color(1,1,1,1);
                    RewardMask.raycastTarget = true;
                    break;

                default: 
                    //textClaimText.color = missionGreen;
                    //btnClaim.image.sprite = null;
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
                SystemManager.ShowAlert("통신 실패!");
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
