﻿using System;
using UnityEngine;

namespace PIERStory
{
    public class RowActionExit : IRowAction
    {
        ScriptRow scriptRow = null;

        public RowActionExit(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            Debug.Log("RowActionExit Call");
            
            if (UserManager.main.useRecord)
                GameManager.main.ShowGameEnd(null);
            else
                SystemManager.ShowGamePopup("다시 보시겠습니까?", GameManager.main.RetryPlay, GameManager.main.EndGame);
        }

        public void EndAction() { }
    }
}