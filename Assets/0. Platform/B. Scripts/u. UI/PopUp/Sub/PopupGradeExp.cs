using UnityEngine;

using DG.Tweening;
using Doozy.Runtime.Reactor;

namespace PIERStory
{
    public class PopupGradeExp : PopupBase
    {
        [Space(15)]
        public RectTransform expBackAura;

        public UnityEngine.UI.Image expGauge;
        public Progressor expProgressor;
        
        public bool isShowUpgradePopup = false;

        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();

            expBackAura.DORotate(new Vector3(0, 0, 360f), 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            
            Debug.Log(string.Format("PopupGradeExp [{0}]/[{1}]", UserManager.main.gradeExperience, UserManager.main.upgradeGoalPoint));

            // expProgressor = expGauge.GetComponent<Progressor>();
            expProgressor.fromValue = (float)UserManager.main.gradeExperience / (float)UserManager.main.upgradeGoalPoint;
            expProgressor.toValue = (float)(Data.contentValue + UserManager.main.gradeExperience) / (float)UserManager.main.upgradeGoalPoint;
            
            Debug.Log(string.Format("expProgressor [{0}]/[{1}]", expProgressor.fromValue, expProgressor.toValue));
        }
        
        public void OnProgressChanged(int __p) {
            
        }

        public void ShowComplete()
        {
            Debug.Log("### ShowComplete");
            
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

                p.Data.SetImagesSprites(null);
                p.Data.SetLabelsTexts(string.Empty);

                PopupManager.main.ShowPopup(p, false);
                Hide();
            }
            

        }
    }
}