using System;

namespace PIERStory
{
    public class RowActionClearScreen : IRowAction
    {
        public RowActionClearScreen(ScriptRow __row) { }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            // 화면 정리 행은 알아서 지나갑니다
            __actionCallback();
            GameManager.main.isWaitingScreenTouch = false;

            GameManager.main.CleanScreenWithoutBackground();
        }

        public void EndAction() { }
    }
}