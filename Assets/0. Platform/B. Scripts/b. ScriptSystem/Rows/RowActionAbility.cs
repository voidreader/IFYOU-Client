using System;
using LitJson;
using BestHTTP;


namespace PIERStory
{
    public class RowActionAbility : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };
        
        
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
            
            callback = __actionCallback;
            GameManager.main.isWaitingScreenTouch = false; // 터치할 필요 없음.
            
            if(__isInstant || !UserManager.main.useRecord || GameManager.isResumePlay) {
                callback?.Invoke();
                return;
            }
            
            
            
            // TOOD 통신처리 
            if(string.IsNullOrEmpty(speaker) || string.IsNullOrEmpty(abilityName)) {
                // MissingComponent 띄워주기 
                callback?.Invoke();
                return;
            }
            
            // 기록 조회해서 같은 씬 안에서 저장된 능력치 증감이 있으면 안함 
            if(UserManager.main.CheckSceneAbilityHistory(StoryManager.main.CurrentEpisodeID, GameManager.main.currentSceneId, speaker, abilityName, addValue)) {
                callback?.Invoke();
                return;
            }
            
            // 통신용 변수 
            JsonData sendingData = new JsonData();
            sendingData["func"] = "addUserAbility";
            sendingData["speaker"] = speaker;
            sendingData["ability"] = abilityName;
            sendingData["add_value"] = addValue;
            
            sendingData["project_id"] = StoryManager.main.CurrentProjectID;
            sendingData["episode_id"] = StoryManager.main.CurrentEpisodeID;
            sendingData["scene_id"] = GameManager.main.currentSceneId;
            
            NetworkLoader.main.SendPost(CallbackAddAbility, sendingData, true);
            
        }
        
        
        /// <summary>
        /// 통신 콜백 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void CallbackAddAbility(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                // callback?.Invoke();
                return;
            }
            
            // 현재 능력치 갱신. 
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            UserManager.main.UpdateUserAbility(result[UserManager.NODE_USER_ABILITY]);
            UserManager.main.UpdateRawStoryAbility(result[UserManager.NODE_RAW_STORY_ABILITY]);
            
            // 팝업 띄워주기             
            SystemManager.ShowAbilityPopup(speaker, abilityName, addValue);
            
            callback?.Invoke();
           
            
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