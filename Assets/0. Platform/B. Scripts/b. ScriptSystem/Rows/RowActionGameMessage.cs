using System;
using UnityEngine;

namespace PIERStory
{
    public class RowActionGameMessage : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };

        public RowActionGameMessage(ScriptRow __row)
        {
            scriptRow = __row;
        }


        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            GameManager.main.isWaitingScreenTouch = false;

            // 스킵을 사용한 경우 화면에 띄우지 않는다
            if(__isInstant || string.IsNullOrEmpty(scriptRow.script_data))
            {
                __actionCallback();
                return;
            }

            PopupBase p = PopupManager.main.GetPopup(GameConst.TEMPLATE_GAME_MESSAGE);
            p.Data.SetLabelsTexts(scriptRow.script_data);

            Color c = Color.white;

            if(!string.IsNullOrEmpty(scriptRow.control))
            {
                if (scriptRow.controlAlternativeName == "긍정")
                    c = HexCodeChanger.HexToColor("EAB0B0B3");
                else if (scriptRow.controlAlternativeName == "부정")
                    c = HexCodeChanger.HexToColor("A3A4D1B3");
            }

            p.Data.Images[0].color = c;
            p.autoDestroyTime = 2f;
            PopupManager.main.ShowPopup(p, false, false);
            __actionCallback();
        }

        public void EndAction()
        {
            
        }
    }
}