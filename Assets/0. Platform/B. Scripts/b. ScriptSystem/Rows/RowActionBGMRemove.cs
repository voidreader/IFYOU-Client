using System;

namespace PIERStory
{
    public class RowActionBGMRemove : IRowAction
    {
        GameSoundCtrl soundCtrl;

        public RowActionBGMRemove(ScriptRow __row) { }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            __actionCallback();
            GameManager.main.isWaitingScreenTouch = false;

            soundCtrl = GameManager.main.SoundGroup[0];
            soundCtrl.PauseBGM();
        }

        public void EndAction() { }
    }
}


