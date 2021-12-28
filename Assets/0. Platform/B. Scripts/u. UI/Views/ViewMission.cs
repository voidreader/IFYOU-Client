using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewMission : CommonView
    {
        public static Action OnCompleteReward = null;

        public TextMeshProUGUI projectTitle;
        public TextMeshProUGUI missionProgressText;
        public TextMeshProUGUI missionPercent;
        public Image missionProgressBar;

        public ScrollRect missionScroll;
        public MissionElement[] missionElements;

        public Image getAllButton;
        public TextMeshProUGUI getAllText;

        public Sprite openStateSprite;
        public Sprite lockStateSprite;

        const string IS_HIDDEN = "is_hidden";
        const string UNLOCK_STATE = "unlock_state";

        public Color32 getAllOpenColor = new Color32(51, 51, 51, 255);
        public Color32 getAllLockColor = new Color32(153, 153, 153, 255);
        public Color32 missionGreen = new Color32(69, 198, 80, 255);

        void OnEnable() {
            // 상태 저장 
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
        }
        
        private void Start() {
            OnCompleteReward = OnStartView;
        }

        public override void OnStartView()
        {
            base.OnStartView();
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5026"), string.Empty);
            

            foreach (MissionElement me in missionElements)
                me.gameObject.SetActive(false);

            SetGetAllButtonState();

            #region mission setting
            int lockHiddenMissionCount = 0;     // 공개되지 않은 히든미션 count
            int sortIndex = 0;                  // unlock_state 등에 의하여 정렬을 도와줄 index
            int completeValue = 0;
            
            // * 달성 후 보상 미수령 => 잠금 => 보상 받음 순서로 한다. 
            // 달성 후 보상 미수령 상태             
            foreach(MissionData missionData in UserManager.main.DictStoryMission.Values) {
                if(missionData.missionState == MissionState.unlocked) {
                    missionElements[sortIndex].InitMission(missionData);
                    sortIndex++;
                    completeValue++;
                }
            }
            
            
            // 잠금 상태
            foreach(MissionData missionData in UserManager.main.DictStoryMission.Values) {
                if(missionData.missionState == MissionState.locked) {
                    
                    if(missionData.isHidden)
                        lockHiddenMissionCount++;
                    else {
                        missionElements[sortIndex].InitMission(missionData);
                        sortIndex++;    
                    }
                }
            }


  
            // 히든엔딩 갯수 표기
            if (lockHiddenMissionCount > 0)
            {
                //rewardBoxes[sortIndex].HighlightHidden(lockHIddenMissionCount);
                // sortIndex++;
            }

            // 달성 후 보상 수령 완료 
            foreach(MissionData missionData in UserManager.main.DictStoryMission.Values) {
                if(missionData.missionState == MissionState.finish) {
                    
                    missionElements[sortIndex].InitMission(missionData);
                    sortIndex++;
                    completeValue++;
                }
            }

            #endregion

            projectTitle.text = StoryManager.main.CurrentProjectTitle;
            missionProgressText.text = string.Format("미션 달성 진행률 ({0}/{1})", completeValue, UserManager.main.DictStoryMission.Count);

            float percentage = (float)completeValue / (float)UserManager.main.DictStoryMission.Count;
            missionPercent.text = string.Format("{0}%", Mathf.Round(percentage * 100));
            missionProgressBar.fillAmount = percentage;

            missionScroll.verticalNormalizedPosition = 1f;
        }
        
        
        public override void OnHideView() {
            base.OnHideView();
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);
        }        
        
        

        /// <summary>
        /// 
        /// </summary>
        public void SetGetAllButtonState()
        {
            
            int countOpen = 0;
            
            foreach(MissionData missionData in UserManager.main.DictStoryMission.Values) {
                if(missionData.missionState == MissionState.unlocked) {
                    countOpen++;
                }
            }

            if (countOpen > 0)
                // 1개 이상인 경우 활성화해줌
                if (countOpen > 0)
                    ChangeGetAllButton(getAllOpenColor, missionGreen, openStateSprite);
                else
                    ChangeGetAllButton(getAllLockColor, Color.white, lockStateSprite);
        }

        /// <summary>
        /// 모두받기 버튼 찐 세팅
        /// </summary>
        /// <param name="textColor">버튼에 들어가는 글자 색상</param>
        /// <param name="overlaySprite">버튼 이미지 sprite</param>
        void ChangeGetAllButton(Color textColor, Color buttonColor, Sprite overlaySprite)
        {
            getAllText.color = textColor;
            getAllButton.sprite = overlaySprite;
            getAllButton.color = buttonColor;
        }
    }
}
