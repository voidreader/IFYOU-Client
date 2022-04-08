using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class PopupGradeBenefit : PopupBase
    {
        [Space(15)]
        public TextMeshProUGUI benefitDetail;

        public override void Show()
        {
            base.Show();
        }


        public void OnClickGradeToggles(int __grade)
        {
            switch (__grade)
            {
                case 1:
                    // IFYOU 등급
                    break;
                case 2:
                    // 플래티넘 등급
                    benefitDetail.text = string.Format(SystemManager.GetLocalizedText("6269") + "\n" + string.Format("6270"), 10, 5, 5, 30);
                    break;
                case 3:
                    // 골드 등급
                    benefitDetail.text = string.Format(SystemManager.GetLocalizedText("6269") + "\n" + string.Format("6270"), 7, 5, 5, 20);
                    break;
                case 4:
                    // 실버 등급
                    benefitDetail.text = string.Format(SystemManager.GetLocalizedText("6269") + "\n" + string.Format("6270"), 5, 5, 5, 10);
                    break;
                case 5:
                    // 브론즈 등급
                    benefitDetail.text = string.Format(SystemManager.GetLocalizedText("6296"));
                    break;
            }
        }
    }
}