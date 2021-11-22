using System;

namespace PIERStory
{

    public class RowActionScreenEffectRemove : IRowAction
    {
        ScriptRow row;

        public RowActionScreenEffectRemove(ScriptRow __row)
        {
            row = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            __actionCallback(); // 바로 콜백 실행 
            GameManager.main.isWaitingScreenTouch = false;

            if (string.IsNullOrEmpty(row.script_data))
                ScreenEffectManager.main.RemoveAllScreenEffect();
            else
            {
                if (RowActionScreenEffect.ListCameraEffect.Contains(row.script_data))
                    ScreenEffectManager.main.RemoveCameraEffect(row.script_data);

                if (RowActionScreenEffect.ListGeneralEffect.Contains(row.script_data))

                    ScreenEffectManager.main.RemoveGeneralEffect(row.script_data);
            }
        }

        public void EndAction() { }
    }
}