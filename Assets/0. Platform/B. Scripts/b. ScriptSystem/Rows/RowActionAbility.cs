using System;

namespace PIERStory
{
    public class RowActionAbility : IRowAction
    {
        ScriptRow scriptRow;
        string speaker = string.Empty;
        string fullText = string.Empty; // 입력된 모든 텍스트
        string[] splitText;
        string abilityName = string.Empty; // 능력 이름 
        int addValue = 0; // 증감수치


        public RowActionAbility(ScriptRow  __row)
        {
            scriptRow = __row;
            fullText = scriptRow.script_data;
            speaker = scriptRow.speaker;
            
            // 화자, 데이터 컬럼 필수 
            if(string.IsNullOrEmpty(speaker) || string.IsNullOrEmpty(fullText) || !fullText.Contains(":")) {
                SetFailAbility();
                return;
            }
            
            fullText = fullText.Replace(" ", ""); // 공백제거
            splitText = fullText.Split(':'); // 콜론으로 분리 
            
            // 분리한 텍스트가 모자라면 
            if(splitText.Length < 2) {
                SetFailAbility();
                return; 
            }
            
            
            abilityName = splitText[0]; // 능력 이름 
            int.TryParse(splitText[1], out addValue); // 증감 값 
            
        }


        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            // TOOD 통신처리 
            if(string.IsNullOrEmpty(speaker) || string.IsNullOrEmpty(abilityName)) {
                // MissingComponent 띄워주기 
            }
        }

        public void EndAction()
        {
        }
        
        
        /// <summary>
        /// 데이터에 문제가 있어서 유효하지 않다고 설정하기. 
        /// </summary>
        void SetFailAbility() {
            speaker = string.Empty;
            abilityName = string.Empty;
            addValue = 0;
        }
    }
}