using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace PIERStory {

    public class PopupAchievement : PopupBase
    {
        public Image icon;
        public Image halo;

        // Start is called before the first frame update
        public override void Show() {
            //base 호출하지 않음 
            base.Show();

            icon.rectTransform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f);
            halo.rectTransform.DORotate(new Vector3(0, 0, 360f), 3f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            
        }
    }
}