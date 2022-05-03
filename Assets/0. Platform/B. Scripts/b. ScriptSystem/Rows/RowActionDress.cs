using System;
using UnityEngine;

using LitJson;

namespace PIERStory
{

    public class RowActionDress : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };

        public RowActionDress(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            GameManager.main.isWaitingScreenTouch = false; // 자동 진행이니까. 

            callback = __actionCallback;

            // 맞는 의상정보의 dress_id를 찾아서 데이터 통신을 진행한다. 
            string speaker = scriptRow.speaker;
            string dress_name = scriptRow.script_data;
            string dress_id = string.Empty;
            JsonData targetDressCodeNode = null; // dress기준정보 Node 찾기. 
            string connected_model_name = string.Empty;

            // 프로젝트 의상정보에 일치하는 의상이 있는지 찾아본다.
            Debug.Log(string.Format("{0} CHANGE DRESS TO {1}", speaker, dress_name));
            targetDressCodeNode = StoryManager.main.GetTargetDressCodeNodeByDressName(speaker, dress_name);


            // 없다면 
            if (targetDressCodeNode == null)
            {
                Debug.Log(string.Format("<color=yellow>Wrong Speaker and Dress Name {0} / {1}</color>", speaker, dress_name));
                // 없으면 경고 띄우고 그냥 넘어간다. 
                GameManager.ShowMissingComponent("의상", string.Format("일치하는 의상 : [{0}]/[{1}] 없음!", speaker, dress_name));
                SendComplete();
                return;
            }

            connected_model_name = targetDressCodeNode[GameConst.COL_MODEL_NAME].ToString();
            dress_id = targetDressCodeNode[GameConst.COL_DRESS_ID].ToString();

            // 선 교체해준다. 
            GameManager.main.UpdateModelByDress(speaker, connected_model_name);

            

            SendComplete();
        }

        public void EndAction() { }

        void SendComplete()
        {
            callback();
        }
    }
}