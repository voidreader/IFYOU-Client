using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                Debug.Log("Go Detail From Begining");
                StoryManager.main.RequestStoryInfo(SystemManager.main.givenStoryData);
            }
        }
    }
}