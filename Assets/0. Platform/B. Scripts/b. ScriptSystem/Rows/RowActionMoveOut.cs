using System;
using UnityEngine;

using DG.Tweening;

namespace PIERStory
{
    public class RowActionMoveOut : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };

        string scriptData = string.Empty;

        // 배경 트윈용 변수 
        float moveDistance = 0f;
        float tweenTime = 0f;
        GameSpriteCtrl currentBG = null; // 현재 배경개체 

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="__row"></param>
        public RowActionMoveOut(ScriptRow __row)
        {
            scriptRow = __row;
            scriptData = scriptRow.script_data;

            // 기본값은 오른쪽 이탈. 
            if (string.IsNullOrEmpty(scriptData))
                scriptData = GameConst.POS_RIGHT;
            else
            {
                scriptData = scriptData.ToUpper().Trim();
                // 입력된 값 체크해서 유효하지 않은 경우 R로 자동설정
                if (!scriptData.Equals(GameConst.POS_LEFT) && !scriptData.Equals(GameConst.POS_RIGHT))
                    scriptData = GameConst.POS_RIGHT;
            }

        }

        public void DoAction(Action __actionCallback, bool __isInstant)
        {

            callback = __actionCallback;
            GameManager.main.isWaitingScreenTouch = false;

            if (GameManager.main.IsDefaultBG() || __isInstant)
            {

                // 유효한 배경이 없는 상태라면 무시한다.
                if (GameManager.main.IsDefaultBG() && !__isInstant)
                    GameManager.ShowMissingComponent("장소이탈", "유효한 배경 없음");

                callback?.Invoke();
                return; // 로직 종료 
            }

            // 실제 로직 동작을 시작합니다.
            currentBG = GameManager.main.currentBG;

            // 거리에 따라 트윈 시간 설정 
            tweenTime = GameManager.main.CalcMoveBGAnimTime(ref moveDistance);

            // 화면 연출을 제외한 이미지 리소스, 캐릭터, 말풍선을 지워준다
            GameManager.main.HideImageResources();
            GameManager.main.HideCharacters();
            ViewGame.main.HideBubbles();

            // 배경 이동처리 
            MoveBackground();

        }

        /// <summary>
        /// 배경 이동 처리 
        /// </summary>
        void MoveBackground()
        {
            if (scriptData.Equals(GameConst.POS_RIGHT))
                moveDistance *= -1;

            currentBG.transform.DOMoveX(moveDistance, tweenTime).OnComplete(OnMoveBackground);
        }

        void OnMoveBackground()
        {
            // 현재 배경을 암전시킨다.
            currentBG.spriteRenderer.DOColor(Color.black, 1.5f).OnComplete(() =>
            {
                // Position 돌려준다. 
                currentBG.transform.position = Vector3.zero;
                GameManager.main.currentBackgroundMount.EndImage();
                // 페이드 아웃이 완료되면 화면 연출도 지워준다.
                ScreenEffectManager.main.RemoveAllScreenEffect();
                // 완료되면 callback 처리 
                callback?.Invoke();

            });
        }


        public void EndAction()
        {
            // 색상은 원래 최초에 암전부터 시작하기때문에 복원시켜줄 필요가 없다.
        }
    }
}


