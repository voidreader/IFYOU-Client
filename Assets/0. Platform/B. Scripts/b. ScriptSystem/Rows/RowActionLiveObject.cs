using System;
using UnityEngine;

using DG.Tweening;

namespace PIERStory
{
    public class RowActionLiveObject : IRowAction
    {
        ScriptRow scriptRow;
        ScriptLiveMount liveObject;
        Action callback = delegate { };

        string in_effect = string.Empty;
        int delayTime = 3;

        public RowActionLiveObject(ScriptRow __row)
        {
            scriptRow = __row;
            in_effect = scriptRow.in_effect;

            // string값이 not null이고, 제대로 숫자값이 들어왔을 때에만 값을 넣어준다.
            if (!string.IsNullOrEmpty(__row.controlAlternativeName) && int.TryParse(__row.controlAlternativeName, out delayTime))
                delayTime = delayTime > 0 ? delayTime : 3;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            if (NetworkLoader.main.UpdateUserImage(scriptRow.script_data, "live_object"))
                GameManager.main.ShowAchieveIllust(SystemManager.GetJsonNodeString(StoryManager.main.GetPublicMinicutJsonByName(scriptRow.script_data), CommonConst.COL_PUBLIC_NAME));

            callback = __actionCallback;

            // 스킵을 사용하면 바로 return
            if (__isInstant && GameManager.main.RenderingPass())
            {
                GameManager.main.isWaitingScreenTouch = false;
                __actionCallback();
                return;
            }

            liveObject = GameManager.main.SetGameLiveObj(scriptRow.script_data);

            if (liveObject != null && liveObject.liveImageController != null)
                liveObject.PlayCubismAnimation();
            else
            {
                // null이라면 행 종료하고 넘어가기
                GameManager.ShowMissingComponent(scriptRow.template, scriptRow.script_data);
                GameManager.main.isWaitingScreenTouch = false;
                __actionCallback();
                return;
            }

            GameLiveImageCtrl liveObjCtrl = liveObject.liveImageController;

            if (liveObjCtrl.textureImage == null)
                liveObjCtrl.SetParentRawImage();

            if (liveObjCtrl.textureImage.color.a < 1f)
                liveObjCtrl.textureImage.color = new Color(liveObjCtrl.textureImage.color.r, liveObjCtrl.textureImage.color.g, liveObjCtrl.textureImage.color.b, 1f);

            if (!string.IsNullOrEmpty(in_effect))
            {
                if (in_effect.Equals(GameConst.INOUT_EFFECT_FADEIN))
                {
                    liveObjCtrl.textureImage.color = new Color(liveObjCtrl.textureImage.color.r, liveObjCtrl.textureImage.color.g, liveObjCtrl.textureImage.color.b, 0f);
                    liveObjCtrl.textureImage.DOFade(1f, 0.4f).OnComplete(FinDotween);
                }
                else if (in_effect.Equals(GameConst.INOUT_EFFECT_SHAKE))
                    liveObjCtrl.transform.DOPunchRotation(new Vector3(0, 0, 20), 0.3f, 40).OnComplete(FinDotween);
                else if (in_effect.Equals(GameConst.INOUT_EFFECT_SCALEUP))
                {
                    liveObjCtrl.transform.localScale = Vector3.zero;
                    liveObjCtrl.transform.DOScale(liveObject.gameScale, 0.3f).SetEase(Ease.OutBack).OnComplete(FinDotween);
                }
            }
            else
                FinDotween();
        }

        void FinDotween()
        {
            if (scriptRow.autoplay_row < 1)
                GameManager.main.ExecuteDelayRow(delayTime);
        }

        public void EndAction() { }
    }
}