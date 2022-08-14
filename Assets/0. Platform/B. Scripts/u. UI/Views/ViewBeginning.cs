using UnityEngine;

using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;

namespace PIERStory {
    public class ViewBeginning : CommonView
    {
        public override void OnView()
        {
            base.OnView();
            
            //Debug.Log("Signal Send");
            
            
            // 첫 시작시, 타이틀로 이동을 요청하기 전 메인 로딩 이미지에 대한 url, key값에 대한 정보를 받습니다.
            // 게임 씬에서 넘어온 경우(givenStoryData) 바로 목록 화면으로 보낸다. 
            if (SystemManager.main.givenStoryData == null || string.IsNullOrEmpty(SystemManager.main.givenStoryData.projectID))
            {
                Debug.Log("Go Title From Begining");
                Signal.Send("IFYOU", "moveTitle", "open!");
            }
            else {
                Debug.Log("##### Go Detail From Begining");
                
                StoryManager.main.RequestStoryInfo(SystemManager.main.givenStoryData);
                
                ViewCommonTop.OnRefreshSuperUser?.Invoke(); // 게임플레이 후 다시 돌아왔을때 슈퍼 유저 마크 계속 유지되도록 처리
                
                // 다녀오면 리프레시 되도록 처리한다.
                StoryManager.main.RequestStoryList(OnRequestStoryList);
            }
        }
        
        
                /// <summary>
        /// callback 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void OnRequestStoryList(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response)) {
                return;
            }
            
            Debug.Log(">> OnRequestStoryList : " + response.DataAsText);
            
            // 작품 리스트 받아와서 스토리 매니저에게 전달. 
            StoryManager.main.SetStoryList(JsonMapper.ToObject(response.DataAsText));
        }
        
    }
}