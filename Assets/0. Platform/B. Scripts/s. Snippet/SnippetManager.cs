using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace PIERStory {
    public class SnippetManager : MonoBehaviour
    {
        public static SnippetManager main = null;
        JsonData playSnippetData = null; // 스니핏 마스터 데이터 
        JsonData snippetScriptData = null; // 스니핏 스크립트 데이터 
        
        public int playRow = 0; // 플레이 Row. 
        public List<ScriptRow> ListRows = new List<ScriptRow>();
        
        
        const string NODE_PLAY_SNIPPET = "playSnippet";
        const string NODE_SNIPPET_SCRIPT = "snippetScript";
        
        
        void Awake() {
            main = this;
        }
        
        
        /// <summary>
        /// 스니핏 초기화
        /// </summary>
        /// <param name="__snippetData"></param>
        public void InitSnippet(JsonData __snippetData ) {
            
            playRow = 0;
            playSnippetData = __snippetData[NODE_PLAY_SNIPPET]; // playSnippet
            snippetScriptData = __snippetData[NODE_SNIPPET_SCRIPT]; // snippetScript 
            
            ListRows.Clear(); // 리스트를 새로 생성한다. 
            
            for(int i=0; i<snippetScriptData.Count;i++) {
                SnippetRow row = new SnippetRow(snippetScriptData[i], null);
                ListRows.Add(row);
            }
        }
    }
}