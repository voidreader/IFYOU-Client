﻿using System;

namespace PIERStory
{
    public class RowActionPhoneCall : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };

        string template = string.Empty;
        string scriptData = string.Empty;


        public RowActionPhoneCall(ScriptRow __row)
        {
            scriptRow = __row;
            template = scriptRow.template;
            scriptData = scriptRow.script_data;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;

            // 스탠딩 캐릭터가 있다면 숨겨준다
            GameManager.main.HideCharacters();

            switch (template)
            {
                case GameConst.TEMPLATE_PHONECALL:
                    ViewGame.main.SetPhoneCallInfo(scriptRow);
                    break;
                case GameConst.TEMPLATE_PHONE_SELF:
                    PhoneTalkerProcess(__isInstant, true);
                    break;
                case GameConst.TEMPLATE_PHONE_PARTNER:
                    PhoneTalkerProcess(__isInstant, false);
                    break;
            }


            if (__isInstant && GameManager.main.RenderingPass())
            {
                callback();
                return;
            }

            // 전화관련 액션이 들어오면 무조건 화면에 전화기를 띄워준다.
            ViewGame.main.ActivePhoneImage(template);
        }


        /// <summary>
        /// 전화 통화중 로그 기록 및 말풍선 활성화
        /// </summary>
        /// <param name="__isInstant">즉시 실행(스킵)</param>
        /// <param name="callReceiver">전화 받는 사람인가?(본인인가?)</param>
        void PhoneTalkerProcess(bool __isInstant, bool callReceiver)
        {
            ViewGame.main.CreateTalkLog(template, GameManager.main.GetNotationName(scriptRow), scriptData);

            // 스킵이 아닌 경우에만 말풍선을 활성화 한다
            if (!__isInstant)
                ViewGame.main.SetPhoneProcess(scriptRow, callback, callReceiver);
        }

        public void EndAction()
        {
            ViewGame.main.HIdePhoneImage();

            // 자동진행 아니고, 템플릿이 전화본인 혹은 전화상대인 경우
            if (scriptRow.autoplay_row < 1 && (template.Equals(GameConst.TEMPLATE_PHONE_SELF) || template.Equals(GameConst.TEMPLATE_PHONE_PARTNER)))
            {
                // 전화 받은 사람이면 true 전화상대면 false
                bool callReciever = true;

                if (template.Equals(GameConst.TEMPLATE_PHONE_PARTNER))
                    callReciever = false;

                ViewGame.main.HidePhoneBubble(callReciever);
            }

            // 전화 관련 템플릿 또는 선택지 템플릿이 아닐 때 말풍선 숨기기
            if (!GameManager.main.IsSameTemplate(GameManager.main.currentRow, "phone") ||
                !GameManager.main.IsSameTemplate(GameManager.main.currentRow, GameConst.TEMPLATE_SELECTION))
                ViewGame.main.HideBubbles();
        }
    }
}

