using DG.Tweening;
using System;

namespace PIERStory
{
    public class RowActionFlowTime : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };

        public RowActionFlowTime(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;
            GameManager.main.isWaitingScreenTouch = false; // 화면터치는 풀어줍니다. 어차피 threadHold가 true니까요
            ViewGame.main.CreateNarrationLog(scriptRow.script_data);

            if (__isInstant)
            {
                callback();
                return;
            }

            ScriptRow nextRow = GameManager.main.currentPage.GetNextRowWithoutIncrement();
            bool isAboutBG = false;
            bool reversal = !string.IsNullOrEmpty(scriptRow.controlAlternativeName) ? true : false;     // 제어명이 존재한다면 true, 없다면 false

            // 다음행이 존재하고, 배경 or 장소진입인 경우 true로 변경
            if (nextRow != null && (nextRow.template.Equals(GameConst.TEMPLATE_BACKGROUND) || nextRow.template.Equals(GameConst.TEMPLATE_MOVEIN) || nextRow.template.Equals(GameConst.TEMPLATE_ILLUST)))
                isAboutBG = true;

            // 연출 진행합니다. 
            ViewGame.main.FlowTimeAnim(scriptRow.script_data, scriptRow.voice, reversal, isAboutBG);
        }

        public void EndAction() 
        { 
            // 시간 연출 중에 스킵 누르면 연출 멈추고 비활성화
            if(GameManager.main.useSkip && ViewGame.main.fadeImage.color.a > 0f)
            {
                ViewGame.main.fadeImage.DOKill();
                ViewGame.main.fadeImage.gameObject.SetActive(false);
            }
        
        }
    }
}