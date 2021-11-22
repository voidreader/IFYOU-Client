using System;

namespace PIERStory
{
    public class RowActionBGM : IRowAction
    {
        ScriptRow scriptRow;
        GameSoundCtrl soundCtrl;

        public RowActionBGM(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            __actionCallback();
            GameManager.main.isWaitingScreenTouch = false;

            soundCtrl = GameManager.main.SoundGroup[0];
            soundCtrl.PlayBGM(scriptRow.script_data);
        }

        public void EndAction() { }
    }
}