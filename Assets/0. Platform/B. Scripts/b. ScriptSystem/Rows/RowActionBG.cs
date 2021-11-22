using System;
using UnityEngine;

using DG.Tweening;

namespace PIERStory
{
    public class RowActionBG : IRowAction
    {
        ScriptRow scriptRow;
        GameSpriteCtrl gameSprite;
        Action callback = delegate { };


        public RowActionBG(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            // 배경은 자동진행이다. 
            GameManager.main.isWaitingScreenTouch = false;

            callback = __actionCallback;

            // 게임 매니저에게 백그라운드 설정을 요청합니다. 
            gameSprite = GameManager.main.SetGameBackground(scriptRow.script_data);

            // 2021.10.12 효과음들이 길어서 배경이 전환 될 때, 효과음을 멈춘다
            if (GameManager.main.SoundGroup[2].GetIsPlaying)
                GameManager.main.SoundGroup[2].PauseAudioClip();

            if (gameSprite == null)
            {
                GameManager.ShowMissingComponent("배경", scriptRow.script_data);

                // null 이면 default로 게임매니저에서 자동 세팅된다. 
                // 바로 완료 콜백 호출
                callback();
                return;
            }

            if (__isInstant)
            {
                gameSprite.gameObject.SetActive(true);
                gameSprite.spriteRenderer.color = Color.white;
                callback?.Invoke();
                return;
            }

            // 암전상태에서 시작하도록 합니다.
            // 위치 체크
            gameSprite.spriteRenderer.color = Color.black;

            // 제어값이 존재한다면 배경 반전
            if (!string.IsNullOrEmpty(scriptRow.controlAlternativeName))
                gameSprite.transform.localScale = new Vector3(-gameSprite.gameScale, gameSprite.gameScale, 1f);

            gameSprite.gameObject.SetActive(true);
            ViewGame.main.FadeOutTimeFlow();
            gameSprite.spriteRenderer.DOColor(Color.white, 1.2f);
            OnTween();

        }
        void OnTween()
        {
            callback();
        }

        public void EndAction() { }
    }
}


