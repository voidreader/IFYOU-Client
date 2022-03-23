using System;

namespace PIERStory
{
    public class RowActionMessenger : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };
        string template = string.Empty;

        public RowActionMessenger(ScriptRow __row)
        {
            scriptRow = __row;
            template = scriptRow.template;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;

            switch (template)
            {
                case GameConst.TEMPLATE_MESSAGE_RECEIVE:
                    GameManager.main.HideCharacters();

                    ViewGame.main.ReceiveMessage(callback, GameManager.main.GetNotationName(scriptRow));
                    break;

                case GameConst.TEMPLATE_MESSAGE_END:
                    ViewGame.main.DestoryAllContents();
                    callback();
                    GameManager.main.isWaitingScreenTouch = false;
                    break;

                default:
                    GameManager.main.HideCharacters();

                    // 비활성화 상태이면 활성화 시켜준다.
                    if (!ViewGame.main.messenger.activeSelf)
                    {
                        ViewGame.main.messenger.SetActive(true);
                        ViewGame.main.SetMessengerScrollBar();
                    }

                    if (template.Equals(GameConst.TEMPLATE_MESSAGE_CALL))
                        scriptRow.speaker = string.Empty;

                    ViewGame.main.CreateForMeesenger(template, scriptRow);
                    callback();
                    break;
            }
        }

        public void EndAction()
        {
            // 다음 템플릿이 선택지 템플릿이 아니면 일단 무조건 숨기기
            if (!GameManager.main.IsSameTemplate(GameManager.main.nextRow, GameConst.TEMPLATE_SELECTION))
                ViewGame.main.messenger.SetActive(false);

            // 다음 템플릿이 종료이면 내용 삭제 및 비활성화
            if (GameManager.main.IsSameTemplate(GameManager.main.nextRow, GameConst.TEMPLATE_EXIT))
                ViewGame.main.DestoryAllContents();

            if (!HoldMessenger())
                ViewGame.main.messenger.SetActive(false);

            // 메시지 도착 템플릿의 종료Action으로는 폰이미지를 숨겨준다
            if (template.Equals(GameConst.TEMPLATE_MESSAGE_RECEIVE))
                ViewGame.main.HIdePhoneImage();

        }

        /// <summary>
        /// 메신저 창(UI)을 유지할지를 체크한다
        /// </summary>
        /// <returns></returns>
        bool HoldMessenger()
        {
            // 다음 행이 메시지 관련 템플릿이라면 유지한다
            if (GameManager.main.IsSameTemplate(GameManager.main.nextRow, "message"))
                return true;

            // 다음 행이 선택지 안내 템플릿이라면 유지한다
            if (GameManager.main.IsSameTemplate(GameManager.main.nextRow, GameConst.TEMPLATE_SELECTION_INFO))
                return true;

            // 다음 행이 선택지라면 유지한다
            if (GameManager.main.IsSameTemplate(GameManager.main.nextRow, GameConst.TEMPLATE_SELECTION))
                return true;

            // 다음 행이 게임메시지라면 유지한다
            if (GameManager.main.IsSameTemplate(GameManager.main.nextRow, GameConst.TEMPLATE_GAME_MESSAGE))
                return true;

            // 다음 행이 능력지 템플릿이라면 유지한다
            if (GameManager.main.IsSameTemplate(GameManager.main.nextRow, GameConst.TEMPLATE_ABILITY))
                return true;


            // 위의 경우들이 아니라면 메신저 창을 유지하지 않는다
            return false;
        }
    }
}

