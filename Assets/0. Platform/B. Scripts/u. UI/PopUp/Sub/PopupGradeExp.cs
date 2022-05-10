using UnityEngine;

using DG.Tweening;
using Doozy.Runtime.Reactor;

namespace PIERStory
{
    public class PopupGradeExp : PopupBase
    {
        [Space(15)]
        public RectTransform expBackAura;

        public TMPro.TextMeshProUGUI currentExp;
        public UnityEngine.UI.Image expGauge;
        public Progressor expProgressor;
        
        public bool isShowUpgradePopup = false;

        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();

            Vector2 imageSize = Data.Images[0].rectTransform.sizeDelta;
            Data.Images[0].rectTransform.sizeDelta = new Vector2(imageSize.x, imageSize.y) * 0.15f;
            expBackAura.DORotate(new Vector3(0, 0, 360f), 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            
            Debug.Log(string.Format("PopupGradeExp [{0}]/[{1}]", UserManager.main.gradeExperience, UserManager.main.upgradeGoalPoint));

            expProgressor.fromValue = (float)UserManager.main.gradeExperience / UserManager.main.upgradeGoalPoint;
            expProgressor.toValue = (float)(Data.contentValue + UserManager.main.gradeExperience) / UserManager.main.upgradeGoalPoint;


            Debug.Log(string.Format("expProgressor [{0}]/[{1}]", expProgressor.fromValue, expProgressor.toValue));
        }

        public void ShowComplete()
        {
            Debug.Log(string.Format("### ShowComplete [{0}] / [{1}]", UserManager.main.gradeExperience, Data.contentValue + UserManager.main.gradeExperience));

            currentExp.DOCounter(UserManager.main.gradeExperience, Data.contentValue + UserManager.main.gradeExperience, 0.8f);
            expProgressor.Play();
            
            
            // 업적에서만 사용
            if(Data.contentJson != null && isOverlayUse) {
                Debug.Log("### ShowComplete ### 2");
            
                UserManager.main.SetUserGradeInfo(Data.contentJson);
                UserManager.main.SetAchievementList(Data.contentJson);
            }
        }

        public void OpenGradeUpPopup()
        {
            if(expProgressor.currentValue >= 1f && !isShowUpgradePopup)  {
                
                isShowUpgradePopup = true;
                
                PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_GRADE_UP);
                if (p == null)
                {
                    Debug.LogError("등급 업 팝업 없음");
                    return;
                }

                string nextGrade = string.Empty;
                Sprite nextBadge = null;

                switch (UserManager.main.nextGrade)
                {
                    case 2:
                        nextGrade = SystemManager.GetLocalizedText("5192");
                        nextBadge = LobbyManager.main.spriteSilverBadge;
                        break;
                    case 3:
                        nextGrade = SystemManager.GetLocalizedText("5193");
                        nextBadge = LobbyManager.main.spriteGoldBadge;
                        break;
                    case 4:
                        nextGrade = SystemManager.GetLocalizedText("5194");
                        nextBadge = LobbyManager.main.spritePlatinumBadge;
                        break;
                    case 5:
                        nextGrade = SystemManager.GetLocalizedText("5195");
                        nextBadge = LobbyManager.main.spriteIFYOUBadge;
                        break;
                }

                p.Data.SetImagesSprites(nextBadge);
                p.Data.SetLabelsTexts(nextGrade);

                PopupManager.main.ShowPopup(p, false);
                Hide();
            }
        }


        public override void Hide()
        {
            base.Hide();

            // 미션 보상에서 호출되는 경우
            if(!isOverlayUse)
            {
                // 현재 등급 경험치, 등급업 경험치를 갱신해준다
                UserManager.main.upgradeGoalPoint = SystemManager.GetJsonNodeInt(Data.contentJson["grade_info"], "upgrade_point");
                UserManager.main.gradeExperience = SystemManager.GetJsonNodeInt(Data.contentJson["experience_info"], "total_exp");

                // 혹시 등급업 할 수도 있으니 다음 등급을 갱신
                UserManager.main.nextGrade = SystemManager.GetJsonNodeInt(Data.contentJson["grade_info"], "next_grade");
            }
        }
    }
}