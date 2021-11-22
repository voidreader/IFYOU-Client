using System;

namespace PIERStory
{
    public class RowActionNarration : IRowAction
    {
        ScriptRow scriptRow;

        public RowActionNarration(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            ViewGame.main.CreateNarrationLog(scriptRow.script_data);

            if (__isInstant && GameManager.main.RenderingPass())
            {
                __actionCallback();
                return;
            }

            ViewGame.main.ShowNarration(scriptRow.script_data, !string.IsNullOrEmpty(scriptRow.controlAlternativeName), __actionCallback);
        }

        public void EndAction()
        {
            ViewGame.main.HideNarration();
        }
    }

}