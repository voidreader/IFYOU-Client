using System;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace PIERStory
{
    public class RowActionImage : IRowAction
    {
        ScriptRow scriptRow;
        Image gameSprite;       // minicut 들어오는 sprite
        Action callback = delegate { };

        string in_effect = string.Empty;

        public RowActionImage(ScriptRow __row)
        {
            scriptRow = __row;
            in_effect = __row.in_effect;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            if (NetworkLoader.main.UpdateUserImage(scriptRow.script_data, "minicut"))
                GameManager.main.ShowAchieveIllust(SystemManager.GetJsonNodeString(StoryManager.main.GetPublicMinicutJsonByName(scriptRow.script_data), "public_name"));

            // 스킵을 사용하면 바로 return
            if (__isInstant && GameManager.main.RenderingPass())
            {
                GameManager.main.isWaitingScreenTouch = false;
                __actionCallback();
                return;
            }

            callback = __actionCallback;

            // 게임매니저에게 미니컷 오픈을 요청 
            // ! 미니컷은 UI Image를 사용해요!
            // ! 기본 미니컷을 띄워주는 작업은 GameManager에서 하고 
            // ! 연출 처리는 여기서. 
            GameManager.main.SetMinicutImage(scriptRow.script_data);
            gameSprite = GameManager.main.currentMinicut;

            // 일치하는 미니컷 없는 경우, 뒤에 등장연출 신경쓰지 않는다.
            if (!gameSprite.gameObject.activeSelf)
                return;

            // 22. 01.19 페어 시스템으로 인한 오류 제거
            // 라이브 오브제에서는 이미지가 화면 위에 있으면 제거를 하고 있지만 이미지는 라이브오브제를 제거하고 있지 않음
            // 따라서 라이브 오브제가 존재하면 제거하도록 한다
            if (GameManager.main.currentLiveObj != null && GameManager.main.currentLiveObj.liveImageController != null)
                GameManager.main.currentLiveObj.liveImageController.HideModel();


            // 효과는 GameBubbleCtrl.cs 참조
            if (!string.IsNullOrEmpty(in_effect))
            {
                if (in_effect.Equals(GameConst.INOUT_EFFECT_FADEIN))
                {
                    gameSprite.color = new Color(gameSprite.color.r, gameSprite.color.g, gameSprite.color.b, 0f);
                    gameSprite.DOFade(1f, 0.4f).OnComplete(FinDotween);
                }
                else if (in_effect.Equals(GameConst.INOUT_EFFECT_SHAKE))
                {
                    gameSprite.rectTransform.DOPunchRotation(new Vector3(0, 0, 20), 0.2f, 40).OnComplete(FinDotween);
                }
                else if (in_effect.Equals(GameConst.INOUT_EFFECT_SCALEUP))
                {
                    Vector3 originScale = gameSprite.rectTransform.localScale;

                    gameSprite.rectTransform.localScale = Vector3.zero;
                    gameSprite.rectTransform.DOScale(originScale, 0.2f).SetEase(Ease.OutBack).OnComplete(FinDotween);
                }
            }
            else
                FinDotween();
        }

        void FinDotween()
        {
            // hold가 안풀려서 true면 false로 만들어준다
            if (GameManager.main.isThreadHold)
                callback();
        }

        public void EndAction() { }
    }
}