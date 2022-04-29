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
            Debug.Log("### ShowComplete");

            currentExp.DOCounter(UserManager.main.gradeExperience, Data.contentValue + UserManager.main.gradeExperience, 0.8f);
            expProgressor.Play();
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
    }
}