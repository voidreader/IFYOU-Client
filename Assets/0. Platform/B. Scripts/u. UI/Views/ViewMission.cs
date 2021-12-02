using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class ViewMission : CommonView
    {
        JsonData missionJson = null;

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

        Color32 getAllOpenColor = new Color32(51, 51, 51, 255);
        Color32 getAllLockColor = new Color32(153, 153, 153, 255);
        Color32 missionGreen = new Color32(69, 198, 80, 255);

        public override void OnStartView()
        {
            base.OnStartView();

            foreach (MissionElement me in missionElements)
                me.gameObject.SetActive(false);

            SetGetAllButton();

            #region mission setting
            int lockHIddenMissionCount = 0;     // 공개되지 않은 히든미션 count
            int sortIndex = 0;                  // unlock_state 등에 의하여 정렬을 도와줄 index
            int completeValue = 0;

            // 달성 후 보상 미수령
            for(int i=0;i<missionJson.Count;i++)
            {
                if (SystemManager.GetJsonNodeString(missionJson[i], UNLOCK_STATE).Equals("0"))
                {
                    missionElements[sortIndex].InitMission(missionJson[i]);
                    sortIndex++;
                    completeValue++;
                }
            }

            // 미해금
            for (int i = 0; i < missionJson.Count; i++)
            {
                if (string.IsNullOrEmpty(SystemManager.GetJsonNodeString(missionJson[i], UNLOCK_STATE)))
                {
                    // 진행중인 히든 미션 갯수 카운팅
                    if (SystemManager.GetJsonNodeBool(missionJson, IS_HIDDEN))
                        lockHIddenMissionCount++;
                    else
                    {
                        // 진행 중이나, 히든 미션이 아닌경우
                        missionElements[sortIndex].InitMission(missionJson[i]);
                        sortIndex++;
                    }
                }
            }

            // 히든엔딩 갯수 표기
            if (lockHIddenMissionCount > 0)
            {
                //rewardBoxes[sortIndex].HighlightHidden(lockHIddenMissionCount);
                sortIndex++;
            }

            // 달성 후 보상 수령 완료
            for (int i = 0; i < missionJson.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(missionJson[i], UNLOCK_STATE).Equals("1"))
                {
                    missionElements[sortIndex].InitMission(missionJson[i]);
                    sortIndex++;
                    completeValue++;
                }
            }

            #endregion

            projectTitle.text = StoryManager.main.CurrentProjectTitle;
            missionProgressText.text = string.Format("미션 달성 진행률 ({0}/{1})", completeValue, missionJson.Count);

            float percentage = (float)completeValue / missionJson.Count;
            missionPercent.text = string.Format("{0}%", Mathf.Round(percentage * 100));
            missionProgressBar.fillAmount = percentage;

            missionScroll.verticalNormalizedPosition = 1f;
        }

        public void SetGetAllButton()
        {
            missionJson = UserManager.main.GetNodeProjectChallenges();
            int countOpen = 0;

            for(int i=0;i<missionJson.Count;i++)
            {
                if (SystemManager.GetJsonNodeString(missionJson[i], UNLOCK_STATE).Equals("0"))
                    countOpen++;
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
