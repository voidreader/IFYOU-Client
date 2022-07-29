using System;

namespace PIERStory
{
    public class RowActionMission : IRowAction
    {
        ScriptRow scriptRow;

        public RowActionMission(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            GameManager.main.isWaitingScreenTouch = false;
            GameManager.main.isThreadHold = false;

            if(!string.IsNullOrEmpty(scriptRow.script_data))
                UserManager.main.CompleteMissionByDrop(scriptRow.script_data);
            
            
                // NetworkLoader.main.UpdateScriptMission(scriptRow.script_data);
        }

        public void EndAction() { }
    }
}