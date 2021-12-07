using System;

using DG.Tweening;

namespace PIERStory
{
    public class RowActionLiveObjectRemove : IRowAction
    {
        string out_effect = string.Empty;
        Action callback = delegate { };

        public RowActionLiveObjectRemove(ScriptRow __row)
        {
            out_effect = __row.out_effect;

            if (!string.IsNullOrEmpty(out_effect) && out_effect.Equals(CommonConst.NONE))
                out_effect = string.Empty;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;

            if (__isInstant)
                FinDotween();

            if (!string.IsNullOrEmpty(out_effect) && GameManager.main.currentLiveObj != null && GameManager.main.currentLiveObj.liveImageController != null)
            {
                GameLiveImageCtrl liveObjCtrl = GameManager.main.currentLiveObj.liveImageController;

                if (out_effect.Equals(GameConst.INOUT_EFFECT_SCALEDOWN))
                    liveObjCtrl.transform.DOScale(0, 0.2f).SetEase(Ease.OutBack).OnComplete(FinDotween);
                else if (out_effect.Equals(GameConst.INOUT_EFFECT_FADEOUT))
                    liveObjCtrl.textureImage.DOFade(0f, 0.4f).OnComplete(FinDotween);
            }
            else
                FinDotween();

        }

        void FinDotween()
        {
            GameManager.main.HideLiveObj();

            if (GameManager.main.isThreadHold && !GameManager.main.useSkip)
            {
                callback();
                GameManager.main.isWaitingScreenTouch = false;
            }
        }

        public void EndAction() { }
    }
}