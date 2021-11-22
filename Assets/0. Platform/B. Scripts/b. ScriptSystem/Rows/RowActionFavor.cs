using System;
using UnityEngine;

using BestHTTP;
using LitJson;

namespace PIERStory
{
    public class RowActionFavor : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };

        string template = string.Empty;
        string favorTarget = string.Empty;
        string script_data = string.Empty;

        public RowActionFavor(ScriptRow __row)
        {
            scriptRow = __row;

            template = scriptRow.template;
            favorTarget = scriptRow.speaker;
            script_data = scriptRow.script_data;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;
            GameManager.main.isWaitingScreenTouch = false;
            int favorValue = 0;

            if (!string.IsNullOrEmpty(scriptRow.scene_id))
            {
                GameManager.ShowMissingComponent(template, "상황 ID를 입력해야합니다");
                callback();
                return;
            }

            if (!int.TryParse(script_data, out favorValue))
            {
                GameManager.ShowMissingComponent(template, "데이터가 정수가 아닙니다");
                callback();
                return;
            }

            NetworkLoader.main.UpdateUserFavor(favorTarget, favorValue, OnUpdateFavor);

        }

        public void EndAction()
        {
            callback();
        }

        void OnUpdateFavor(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Network Error in OnUpdateFavor");
                callback();
                return;
            }

            Debug.Log(res.DataAsText);

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            // UserManager 갱신하기
            UserManager.main.SetNodeUserFavor(result);
            callback();
        }
    }
}

