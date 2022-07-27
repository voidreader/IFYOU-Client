using System;
using UnityEngine;

namespace PIERStory
{
    public class RowActionExit : IRowAction
    {
        ScriptRow scriptRow = null;
        string targetSceneID = string.Empty;

        public RowActionExit(ScriptRow __row)
        {
            scriptRow = __row;
            targetSceneID = __row.target_scene_id;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            
            // target_scene_id 있는지 체크해서 추가처리 
            if(!string.IsNullOrEmpty(targetSceneID)) {
                Debug.Log(string.Format("RowActionExit Call with [{0}]", targetSceneID));
                
                // 앞에 구분자 #, @가 있어야하는데 없으면 empty로 만든다. 
                if(!targetSceneID.Contains(GameConst.TAG_TARGET_EPISODE) && !targetSceneID.Contains(GameConst.TAG_TARGET_EPISODE2)) {
                    Debug.Log("Invalid Exit targetEpisodeID");
                    targetSceneID = string.Empty;
                }
                
            }
            else {
                Debug.Log("RowActionExit Call");
                targetSceneID = string.Empty;
            }
            
            

            if (UserManager.main.useRecord)
                GameManager.main.ShowGameEnd(targetSceneID);
            else
            {
                NetworkLoader.main.RequestCompleteEpisode(null);
                SystemManager.ShowSystemPopupLocalize("6203", GameManager.main.RetryPlay, GameManager.main.EndGame);
            }
        }

        public void EndAction() { }
    }
}