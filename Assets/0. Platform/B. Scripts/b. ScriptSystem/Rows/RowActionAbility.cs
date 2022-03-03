using System;

namespace PIERStory
{
    public class RowActionAbility : IRowAction
    {
        ScriptRow scriptRow;


        public RowActionAbility(ScriptRow  __row)
        {
            scriptRow = __row;
        }


        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
        }

        public void EndAction()
        {
        }
    }
}