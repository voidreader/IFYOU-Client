using System;
using UnityEngine;

namespace PIERStory
{

    public class RowActionSelection : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };

        public RowActionSelection(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;

            // 이어하기 처리
            if (GameManager.isResumePlay)
            {
                // 이어하기 중에서는 선택지를 띄우지 않아야해. 
                // target_scene_id를 user_selection_progress 에서 찾아야해!
                if (UserManager.main.CheckProjectSelectionProgressExists(StoryManager.main.CurrentEpisodeID, scriptRow.target_scene_id))
                {
                    // 있으면 마치 선택지를 처리한것처럼 해야한다. 
                    // 이렇게 호출해야지 선택지 기록 업데이트 통신을 하지 않는다. (하면 안됨!!)
                    GameManager.main.currentPage.SetCurrentRowBySceneID(scriptRow.target_scene_id);
                    Debug.Log("Resume Play => " + scriptRow.target_scene_id);
                }

                EndSelectionPrepare();
                return;

            } // ? 이어하기 처리 종료 

            // 선택지를 만나면 skip을 중단한다.  
            // isResumePlay의 경우는 기록을 조회해서 바로 이동하도록 한다. 
            GameManager.main.useSkip = false;

            ViewGame.main.CollectSelections(scriptRow.script_data);
            // 다음 행도 선택지 인지 체크한다.
            // 다음 행이 뭐죠!?
            ScriptRow nextRow = GameManager.main.currentPage.GetNextRowWithoutIncrement();

            // 다음행이 없거나, 선택지 템플릿이 아닌 경우 UI 호출을 지시해야 한다 
            if (nextRow == null || nextRow.template != GameConst.TEMPLATE_SELECTION)
                ViewGame.main.StackSelection(scriptRow, true);
            else
                ViewGame.main.StackSelection(scriptRow, false); // 다음 행도 선택지면  UI 띄우지 않음.


            EndSelectionPrepare();
        }

        void EndSelectionPrepare()
        {
            // 바로 콜백 실행하고, 진행되도록 한다. (StackSelection에서 입력을 기다리게 한다)
            callback?.Invoke();
            GameManager.main.isWaitingScreenTouch = false;
        }


        public void EndAction() { }
    }
}