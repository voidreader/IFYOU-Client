using System;
using UnityEngine.UI;

using DG.Tweening;

namespace PIERStory
{
    /// <summary>
    /// 이미지 제거 액션 
    /// </summary>
    public class RowActionImageRemove : IRowAction
    {
        string out_effect = string.Empty;
        Action callback = delegate { };

        public RowActionImageRemove(ScriptRow __row)
        {
            out_effect = __row.out_effect;

            // 없으면 null이나 마찬가지
            if (!string.IsNullOrEmpty(out_effect) && out_effect.Equals(CommonConst.NONE))
                out_effect = string.Empty;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;

            if (__isInstant)
                FinDotween();

            Image minicut = GameManager.main.currentMinicut;

            if (!string.IsNullOrEmpty(out_effect) && minicut.sprite != null)
            {
                if (out_effect.Equals(GameConst.INOUT_EFFECT_SCALEDOWN))
                    minicut.rectTransform.DOScale(0, 0.2f).SetEase(Ease.OutBack).OnComplete(FinDotween);
                else if (out_effect.Equals(GameConst.INOUT_EFFECT_FADEOUT))
                    minicut.DOFade(0f, 0.4f).OnComplete(FinDotween);
            }
            else
                FinDotween();
        }

        void FinDotween()
        {
            GameManager.main.HideImageMinicut();

            if (GameManager.main.isThreadHold && !GameManager.main.useSkip)
            {
                callback();
                GameManager.main.isWaitingScreenTouch = false;
            }
        }

        public void EndAction() { }
    }
}