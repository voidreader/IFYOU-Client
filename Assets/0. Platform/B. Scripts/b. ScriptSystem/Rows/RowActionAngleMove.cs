using System;
using System.Collections;
using UnityEngine;

using DG.Tweening;

namespace PIERStory
{
    public class RowActionAngleMove : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };

        float locX = 0f;    // 이동 좌표(percent) -1f ~ 1f
        float movableDistance = 0f;
        float tweenTime = 0f;

        GameSpriteCtrl currentBG = null; // 현재 배경개체

        public RowActionAngleMove(ScriptRow __row)
        {
            scriptRow = __row;

            // 일부러 -1 ~ 1을 벗어나는 값을 준다. DoAction에서 return 시키기 위해
            if (!float.TryParse(__row.script_data, out locX))
                locX = 1.1f;
            else
                locX = Mathf.Clamp(locX, -1f, 1f);  // 혹시 모를 범위 벗어나는 값을 사전 차단
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            // 터치 없이 진행됩니다.
            GameManager.main.isWaitingScreenTouch = false;

            callback = __actionCallback;
            currentBG = GameManager.main.currentBG;

            tweenTime = GameManager.main.CalcMoveBGAnimTime(ref movableDistance) * 0.5f;

            // 앵글 이동하기 전에 캐릭터, 말풍선, 이미지 모두 제거
            // 여기도 바꾸자. 앵글 이동은 보통 배경이 쓰여진 상태에서 쓰기 때문에 화면정리가 일어난 후에 이동한다고 생각하자
            // 화면 연출만 제외하고 모두 지우자
            GameManager.main.HideImageResources();
            GameManager.main.HideCharacters();
            ViewGame.main.HideBubbles();

            // 스킵 중이거나 자동 진행을 사용하면 연출 없이 바로 그 자리로
            if (__isInstant || scriptRow.autoplay_row > 0)
            {
                currentBG.transform.position = new Vector3(-movableDistance * locX, 0f, 0f);
                callback();
                return;
            }

            currentBG.transform.DOMoveX(-movableDistance * locX, tweenTime).OnComplete(MoveComplete);
        }
        void MoveComplete()
        {
            callback?.Invoke();
        }


        public void EndAction()
        {
        }
    }

}

