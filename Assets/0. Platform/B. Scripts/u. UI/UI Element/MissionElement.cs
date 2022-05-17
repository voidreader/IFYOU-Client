using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;

namespace PIERStory
{
    public class MissionElement : MonoBehaviour
    {
        [Header("미션 이미지 관련")]
        public ImageRequireDownload missionThumbnail;
        public GameObject hiddenHighlight;
        public GameObject missionHintButton;

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
            missionHintButton.SetActive(false);
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
            missionHintButton.SetActive(false);
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
                    SetMissionHint();

                    missionProgress.SetActive(true);
                    missionHintButton.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// 미션 힌트 세팅
        /// </summary>
        void SetMissionHint()
        {
            int total = 1;
            int current = 0;
            float percent = 1f;

            switch (missionData.missionType)
            {
                case MissionType.drop:
                    missionProgressText.text = string.Format(SystemManager.GetLocalizedText("5042") + "(0/1)");
                    missionPercent.text = "0%";
                    missionGauge.fillAmount = 0f;
                    break;
                case MissionType.scene:

                    if (missionData.eventDetailHint == null)
                        break;

                    total = missionData.eventDetailHint.Count;
                    for(int i=0;i<missionData.eventDetailHint.Count;i++)
                    {
                        if (missionData.eventDetailHint[i].played >= missionData.eventDetailHint[i].total)
                            current++;
                    }
                    percent = (float)current / total;

                    missionProgressText.text = string.Format(SystemManager.GetLocalizedText("5042") + "({0}/{1})", current, total);
                    missionPercent.text = string.Format("{0}%", Mathf.RoundToInt(percent * 100f));
                    missionGauge.fillAmount = percent;
                    break;
                case MissionType.episode:

                    if (missionData.episodeDetailHint == null)
                        break;

                    total = missionData.episodeDetailHint.Count;

                    for(int i=0;i<missionData.episodeDetailHint.Count;i++)
                    {
                        if (UserManager.main.IsCompleteEpisode(missionData.episodeDetailHint[i]))
                            current++;
                    }
                    percent = (float)current / (float)total;
                    missionProgressText.text = string.Format(SystemManager.GetLocalizedText("5042") + "({0}/{1})", current, total);
                    missionPercent.text = string.Format("{0}%", Math.Round(percent * 100f, 0));
                    missionGauge.fillAmount = percent;
                    break;
            }
        }


        public void OnClickGetReward()
        {
            // 열려 있는 상태가 아니라면 동작하지 않는다
            if (state != MissionState.unlocked)
                return;

            if(!ViewMission.ScreenSetComplete)
            {
                Debug.LogWarning("화면 갱신이 아직 이루어지지 않음!");
                return;
            }

            ViewMission.ScreenSetComplete = false;

            UserManager.main.GetMissionRewared(missionData, CallbackGetMissionReward);
        }
        
        void CallbackGetMissionReward(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackGetMissionReward");
                ViewMission.ScreenSetComplete = true;
                return;
            }

            JsonData resposeData = JsonMapper.ToObject(res.DataAsText);
            
            // 재화 획득 팝업 
            SystemManager.ShowResourcePopup(SystemManager.GetLocalizedText("6123"), missionData.rewardQuantity, missionData.currency_icon_url, missionData.currency_icon_key);
            UserManager.main.SetBankInfo(resposeData);
            
            // 미션 상태 변경 (보상 받고, 완료로 변경)
            missionData.missionState = MissionState.finish; 
            UserManager.main.SetMissionData(missionData.missionID, missionData);

            // 경험치 획득 팝업 출현
            PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_GRADE_EXP);

            if (p == null)
            {
                Debug.LogError("등급 경험치 획득 팝업 없음!");
                return;
            }

            Sprite s = null;

            switch (UserManager.main.nextGrade + 1)
            {
                case 1:
                    s = LobbyManager.main.spriteBronzeBadge;
                    break;
                case 2:
                    s = LobbyManager.main.spriteSilverBadge;
                    break;
                case 3:
                    s = LobbyManager.main.spriteGoldBadge;
                    break;
                case 4:
                    s = LobbyManager.main.spritePlatinumBadge;
                    break;
                case 5:
                    s = LobbyManager.main.spriteIFYOUBadge;
                    break;
            }

            p.Data.SetImagesSprites(s);
            p.isOverlayUse = false;
            p.Data.SetLabelsTexts(string.Format("+{0}", missionData.rewardExp), string.Format("/{0}", UserManager.main.upgradeGoalPoint));
            p.Data.contentJson = resposeData;
            p.Data.contentValue = missionData.rewardExp;
            PopupManager.main.ShowPopup(p, false);

            // * 성공 했다. => 미션이 해금도 되었고, 보상도 받은 상태가 되는거다. 
            ViewMission.OnRefreshProgressor?.Invoke();
            /*
            if(!ViewMission.clickGetAll)
                SystemManager.ShowSimpleAlertLocalize("6123");
            */
        }

        public void OnClickOpenMissionHint()
        {
            PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_MISSION_HINT);

            if(p == null)
            {
                Debug.LogError("미션 힌트 팝업 없음!");
                return;
            }

            p.Data.SetLabelsTexts(missionData.missionName, missionData.missionHint);
            p.Data.isPositive = missionData.detailHint;
            p.Data.contentValue = missionData.missionID;
            PopupManager.main.ShowPopup(p, false);
        }
    }
}
