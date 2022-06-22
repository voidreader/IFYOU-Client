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
            string benefitText = string.Empty;
            
            switch (__grade)
            {
                case 1:
                    // IFYOU 등급
                    break;
                case 2:
                    // 플래티넘 등급
                    benefitText = string.Format(SystemManager.GetLocalizedText("6299"), 10, 5, 30) + "\n" + string.Format(SystemManager.GetLocalizedText("6270"));
                    
                    break;
                case 3:
                    // 골드 등급
                    benefitText = string.Format(SystemManager.GetLocalizedText("6299"), 7, 5, 20);
                    break;
                case 4:
                    // 실버 등급
                    benefitText = string.Format(SystemManager.GetLocalizedText("6299"), 5, 5, 10);
                    break;
                case 5:
                    // 브론즈 등급
                    benefitText = string.Format(SystemManager.GetLocalizedText("6296"));
                    break;
            }
            
            SystemManager.SetText(benefitDetail, benefitText);
        }
    }
}