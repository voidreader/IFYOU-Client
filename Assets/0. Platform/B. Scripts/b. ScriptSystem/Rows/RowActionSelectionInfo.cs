using System;

namespace PIERStory
{
    public class RowActionSelectionInfo : IRowAction
    {
        ScriptRow scriptRow;

        public RowActionSelectionInfo(ScriptRow __row)
        {
            scriptRow = __row;
        }


        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            __actionCallback?.Invoke();
            GameManager.main.isWaitingScreenTouch = false;

            // 이어하기 중이었다면 아무것도 안하고 다음행으로 넘기자
            if (GameManager.isResumePlay)
                return;

            // 다음행이 선택지가 아니라면 아무것도 하지 말자
            if (GameManager.main.nextRow.template != GameConst.TEMPLATE_SELECTION)
                return;

            // 스킵중일 수도 있으니 풀어주고
            GameManager.main.useSkip = false;

            ViewGame.main.SetSelectionInfoText(scriptRow.script_data);
        }

        public void EndAction() { }
    }
}