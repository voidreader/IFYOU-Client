using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using DanielLochner.Assets.SimpleScrollSnap;


namespace PIERStory {

    public class PopupRecommend : PopupBase
    {
        public CanvasGroup imageTitle;
        public SimpleScrollSnap scrollSnap;
        
        public GameObject storyPrefab;
        
        JsonData recommededData = null;
        
        public List<GameObject> panels = new List<GameObject>();
        
        public GameObject ButtonRight; 
        public GameObject ButtonLeft; 
        
        public GameObject ButtonClose;

        public override void Show()
        {
            
            if(isShow)
                return;
            
            base.Show();
            recommededData = Data.contentJson;
            
            InitButton();
            
            AddStory();
            
            StartCoroutine(SetRecommedStoryData());
            
            
            ButtonClose.SetActive(false); // 닫기 버튼은 처음에 안보이도록 변경 
        }
        
        void InitButton() {
            ButtonRight.SetActive(false);
            ButtonLeft.SetActive(false);
        }

        
        
        /// <summary>
        /// 추가하기
        /// </summary>
        void AddStory() {
            
            if(recommededData == null || recommededData.Count == 0) {
                Debug.Log("추천 데이터 없음");
                return;
            }
                
                
            for(int i=0; i<recommededData.Count;i++) {
                string projectID = recommededData[i].ToString();
                StoryData story = StoryManager.main.FindProject(projectID);

                /*
                GameObject panel = Instantiate(storyPrefab, Vector3.zero, Quaternion.identity);
                panel.transform.localScale = Vector3.one;
                panel.GetComponent<RecommendStory>().Init(story);
                scrollSnap.Add(panel, scrollSnap.NumberOfPanels, false);
                */

                // 예외처리
                if (story == null || string.IsNullOrEmpty(story.projectID))
                    continue;
                
                storyPrefab.GetComponent<RecommendStory>().Init(story);
                panels.Add(scrollSnap.Add(storyPrefab, scrollSnap.NumberOfPanels));
            }
        }
        
        IEnumerator SetRecommedStoryData() {
            while(content.inTransition)
                yield return null;
            
            for(int i=0; i<panels.Count;i++) {
                panels[i].GetComponent<RecommendStory>().SetData();
            }
            
            // 패널이 여러개면 오른쪽 버튼 등장 
            if(panels.Count > 1)  {
                ButtonRight.SetActive(true);
            }
            
            yield return new WaitForSeconds(2);
            
            ButtonClose.SetActive(true); // 버튼 등장 
        }
        
        
        
        /// <summary>
        /// 
        /// </summary>
        public void OnClickPlay() {
            if(LobbyManager.main != null) { // 로비에서 떴을떄 
            
                // RequestStory 호출하기
                StoryManager.main.RequestStoryInfo(panels[scrollSnap.CurrentPanel].GetComponent<RecommendStory>().story);
            }
            else { // 게임에서 떴을때 
                // givenStory를 바꾼다. 
                SystemManager.main.givenStoryData = panels[scrollSnap.CurrentPanel].GetComponent<RecommendStory>().story;
                UserManager.main.gameComplete = true;
                GameManager.main.EndGame();
                ViewCommonTop.OnBackAction = null; // 액션 초기화 
            }
            
            Hide();
            
        }
        
        
        
        public void OnClickRight() {
            scrollSnap.GoToNextPanel();
        }
        
        public void OnClickLeft() {
            scrollSnap.GoToPreviousPanel();
        }
        
        
        public void OnPanelChanged() {
            Debug.Log("OnPanelChanged : " + scrollSnap.CurrentPanel);
            
            if(scrollSnap.CurrentPanel == 0) {
                
                ButtonLeft.SetActive(false);
                
                if(scrollSnap.NumberOfPanels > 1) {
                    ButtonRight.SetActive(true);
                }
                else {
                    ButtonRight.SetActive(false);
                }
                
                return;
            }
            
            
            
            // 마지막 패널인 경우 
            if(scrollSnap.CurrentPanel == scrollSnap.NumberOfPanels - 1) {
                ButtonRight.SetActive(false);
                
                if(scrollSnap.NumberOfPanels > 1) {
                    ButtonLeft.SetActive(true);
                }
                else {
                    ButtonLeft.SetActive(false);
                }
                
                return;
            }
            
            if(scrollSnap.CurrentPanel > 0 && scrollSnap.CurrentPanel < scrollSnap.NumberOfPanels-1) {
                ButtonLeft.SetActive(true);
                ButtonRight.SetActive(true);
            }
            
        }


    }
}