using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewMission : CommonView
    {
        public static Action OnCompleteReward = null;
        public static Action OnRefreshProgressor = null;
        public static bool ScreenSetComplete = false;

        public Image allClearRewardBox;

        public TextMeshProUGUI missionProgressText;
        public TextMeshProUGUI missionPercent;
        public Image missionProgressBar;

        public ScrollRect missionScroll;
        public MissionElement[] missionElements;

        public Image getAllButton;
        public TextMeshProUGUI getAllText;

        public Color32 getAllOpenColor = new Color32(51, 51, 51, 255);
        public Color32 getAllLockColor = new Color32(153, 153, 153, 255);

        public Sprite spriteAllClearOff;
        public Sprite spriteAllClearOn;

        void OnEnable() {
            // 상태 저장 
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
        }

        private void Start()
        {
            OnCompleteReward = OnStartView;
            OnRefreshProgressor = SetMissionProgressor;
        }

        public override void OnStartView()
        {
            //파이어베이스
            Firebase.Analytics.FirebaseAnalytics.LogEvent("lobby_mission", "project_id", StoryManager.main.CurrentProjectID); 
            
            base.OnStartView();
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5026"), string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);
            
            // ! 경험치 관련 문제로 잠시 모든 보상 받기 비활성화.
            // SetGetAllButtonState();
            
            SetMissionProgressor();

            missionScroll.verticalNormalizedPosition = 1f;
            
            // 미션 올클리어 이미지 세팅
            if(UserManager.main.GetProjectMissionAllClear())
            {
                allClearRewardBox.sprite = spriteAllClearOn;
                allClearRewardBox.material.EnableKeyword("GREYSCALE_ON");
                allClearRewardBox.SetNativeSize();
                allClearRewardBox.rectTransform.localScale = Vector2.one * 0.8f;
            }
            else
            {
                allClearRewardBox.material.DisableKeyword("GREYSCALE_ON");

                if (missionProgressBar.fillAmount < 1f)
                {
                    allClearRewardBox.sprite = spriteAllClearOff;
                    allClearRewardBox.SetNativeSize();
                    allClearRewardBox.rectTransform.localScale = Vector2.one;
                }
                else
                {
                    allClearRewardBox.sprite = spriteAllClearOn;
                    allClearRewardBox.SetNativeSize();
                    allClearRewardBox.rectTransform.localScale = Vector2.one * 0.8f;
                }
            }
        }

        void SetMissionProgressor()
        {
            #region mission setting

            int lockHiddenMissionCount = 0;     // 공개되지 않은 히든미션 count
            int sortIndex = 0;                  // unlock_state 등에 의하여 정렬을 도와줄 index
            int completeValue = 0;

            // * 달성 후 보상 미수령 => 잠금 => 보상 받음 순서로 한다. 
            // 달성 후 보상 미수령 상태             
            foreach (MissionData missionData in UserManager.main.DictStoryMission.Values)
            {
                if (missionData.missionState == MissionState.unlocked)
                {
                    missionElements[sortIndex].InitMission(missionData);
                    sortIndex++;
                    completeValue++;
                }
            }


            // 잠금 상태
            foreach (MissionData missionData in UserManager.main.DictStoryMission.Values)
            {
                if (missionData.missionState == MissionState.locked)
                {
                    missionElements[sortIndex].InitMission(missionData);

                    if(missionData.isHidden && missionElements[sortIndex].missionGauge.fillAmount == 0f)
                    {
                        lockHiddenMissionCount++;
                        continue;
                    }

                    sortIndex++;
                }
            }


            // 히든엔딩 갯수 표기
            if (lockHiddenMissionCount > 0)
            {
                missionElements[sortIndex].HighlightHidden(lockHiddenMissionCount);
                sortIndex++;
            }

            // 달성 후 보상 수령 완료 
            foreach (MissionData missionData in UserManager.main.DictStoryMission.Values)
            {
                if (missionData.missionState == MissionState.finish)
                {
                    missionElements[sortIndex].InitMission(missionData);
                    sortIndex++;
                    completeValue++;
                }
            }

            #endregion

            /*
            int completeValue = 0;

            // * 달성 후 보상 미수령 => 잠금 => 보상 받음 순서로 한다. 
            // 달성 후 보상 미수령 상태             
            foreach (MissionData missionData in UserManager.main.DictStoryMission.Values)
            {
                if (missionData.missionState == MissionState.unlocked)
                    completeValue++;
                else if (missionData.missionState == MissionState.finish)
                    completeValue++;
            }
            */
            
            Debug.Log(string.Format("### SetMissionProgressor [{0}]/[{1}]", completeValue, UserManager.main.DictStoryMission.Count));

            SystemManager.SetText(missionProgressText, string.Format(SystemManager.GetLocalizedText("5032"), completeValue, UserManager.main.DictStoryMission.Count));

            float percentage = (float)completeValue / (float)UserManager.main.DictStoryMission.Count;
            missionPercent.text = string.Format("{0}%", Mathf.Round(percentage * 100));
            missionProgressBar.fillAmount = percentage;

            ScreenSetComplete = true;
        }


        public override void OnHideView()
        {
            base.OnHideView();

            foreach (MissionElement me in missionElements)
                me.gameObject.SetActive(false);

            StoryLobbyMain.OnInitializeContentGroup?.Invoke();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        public void SetGetAllButtonState()
        {
            // 1개 이상인 경우 활성화해줌
            if (UnlockRewardCount() > 0)
                ChangeGetAllButton(getAllOpenColor, Color.white);
            else
                ChangeGetAllButton(getAllLockColor, Color.grey);
        }

        int UnlockRewardCount()
        {
            int countOpen = 0;

            foreach (MissionData missionData in UserManager.main.DictStoryMission.Values)
            {
                if (missionData.missionState == MissionState.unlocked)
                    countOpen++;
            }

            return countOpen;
        }

        /// <summary>
        /// 모두받기 버튼 찐 세팅
        /// </summary>
        /// <param name="textColor">버튼에 들어가는 글자 색상</param>
        /// <param name="overlaySprite">버튼 이미지 sprite</param>
        void ChangeGetAllButton(Color textColor, Color buttonColor)
        {
            getAllText.color = textColor;
            getAllButton.color = buttonColor;
        }

        public void OnClickGetAllReward()
        {
            if (UnlockRewardCount() == 0)
                return;

            foreach (MissionElement missionElement in missionElements)
            {
                if (missionElement.state == MissionState.unlocked)
                    missionElement.OnClickGetReward();
            }

            SystemManager.ShowSimpleAlertLocalize("6123");
            //SetGetAllButtonState();
            OnCompleteReward?.Invoke();
        }

        /// <summary>
        /// 미션 올 클리어 보상 받기
        /// </summary>
        public void OnClickGetMissionAllClearReward()
        {
            if(!ScreenSetComplete)
            {
                Debug.LogWarning("화면 갱신이 아직 이루어지지 않음!");
                return;
            }

            ScreenSetComplete = false;

            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "requestMissionAllReward";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            sending[LobbyConst.COL_LANG] = SystemManager.main.currentAppLanguageCode;

            NetworkLoader.main.SendPost(CallbackRequestMissionAllReward, sending, true);
        }

        void CallbackRequestMissionAllReward(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackRequestMissionAllReward");
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            if(result["reward"] == null || result["reward"].Count == 0)
            {
                SystemManager.ShowSystemPopup("미션 올클리어 보상이 존재하지 않습니다.", null, null, false, false);
                return;
            }

            PopupBase p = null;

            if (!result["bank"].ContainsKey(LobbyConst.COIN))
            {
                p = PopupManager.main.GetPopup(LobbyConst.POPUP_ALL_CLEAR_REWARD_LIST);

                string coinAmount = string.Empty, starAmount = string.Empty;

                if (!result.ContainsKey("reward"))
                    return;

                for (int i = 0; i < result["reward"].Count; i++)
                {
                    if (SystemManager.GetJsonNodeString(result["reward"][i], LobbyConst.NODE_CURRENCY) == LobbyConst.COIN)
                        coinAmount = SystemManager.GetJsonNodeString(result["reward"][i], "quantity");
                    else if (SystemManager.GetJsonNodeString(result["reward"][i], LobbyConst.NODE_CURRENCY) == LobbyConst.GEM)
                        starAmount = SystemManager.GetJsonNodeString(result["reward"][i], "quantity");
                }
                
                p.Data.SetLabelsTexts(coinAmount, starAmount);
            }
            else
            {
                p = PopupManager.main.GetPopup(LobbyConst.POPUP_MISSION_CLEAR_REWARD);
            }

            if (p == null)
            {
                Debug.LogError(string.Format("{0} 팝업이 없음!", p.popupName));
                return;
            }

            p.Data.contentJson = result;
            PopupManager.main.ShowPopup(p, false);
        }
    }
}
