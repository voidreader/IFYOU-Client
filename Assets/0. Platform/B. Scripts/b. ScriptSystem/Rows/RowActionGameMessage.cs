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

            scriptRow.script_data = scriptRow.script_data.Replace("\\", "\n");

            PopupBase p = PopupManager.main.GetPopup(GameConst.TEMPLATE_GAME_MESSAGE);
            p.Data.SetLabelsTexts(scriptRow.script_data);
            p.Data.SetImagesSprites(GameManager.main.spriteGameMessageNormal);

            if(!string.IsNullOrEmpty(scriptRow.control))
            {
                if (scriptRow.controlAlternativeName == "긍정")
                    p.Data.SetImagesSprites(GameManager.main.spriteGameMessagePositive);
                else if (scriptRow.controlAlternativeName == "부정")
                    p.Data.SetImagesSprites(GameManager.main.spriteGameMessageNegative);
            }

            p.autoDestroyTime = 2f;
            PopupManager.main.ShowPopup(p, false, false);
            __actionCallback();
        }

        public void EndAction()
        {
            
        }
    }
}