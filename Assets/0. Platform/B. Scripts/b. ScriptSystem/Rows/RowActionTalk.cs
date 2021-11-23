﻿using System;

namespace PIERStory
{
    public class RowActionTalk : IRowAction
    {

        ScriptRow scriptRow;
        Action callback = delegate { };

        public RowActionTalk(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;

            // 화자가 없는 경우, 데이터가 없는 경우가 있을때 멈추는것 방지
            if (string.IsNullOrEmpty(scriptRow.speaker))
                scriptRow.speaker = string.Empty;

            ViewGame.main.CreateTalkLog(scriptRow.template, GameManager.main.GetNotationName(scriptRow), scriptRow.script_data);

            if (__isInstant && GameManager.main.RenderingPass())
            {
                callback();
                return;
            }

            GameManager.main.SetTalkProcess(scriptRow, callback);
        }

        public void EndAction()
        {
            if (scriptRow.autoplay_row < 1)
                ViewGame.main.HideBubbles();
        }
    }
}
