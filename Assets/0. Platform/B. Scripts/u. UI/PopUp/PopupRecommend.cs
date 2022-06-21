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

        public override void Show()
        {
            
            if(isShow)
                return;
            
            base.Show();
            recommededData = Data.contentJson;
            
            InitButton();
            
            AddStory();
            
            StartCoroutine(SetRecommedStoryData());
            
            
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