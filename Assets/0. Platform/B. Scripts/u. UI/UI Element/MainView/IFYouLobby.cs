using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using Doozy.Runtime.UIManager.Components;
using DanielLochner.Assets.SimpleScrollSnap;


namespace PIERStory {

    public class IFYouLobby : MonoBehaviour
    {
        JsonData promotionList = null;
        [Header("프로모션")]
        public SimpleScrollSnap promotionScroll;
        public Transform promotionContent;
        public GameObject promotionPrefab;
        public Transform promotionPagenation;
        public GameObject pageTogglePrefab;

        [Space(15)][Header("메인 스토리 리스트")]
        public GameObject fastplayButton;
        public TextMeshProUGUI episodeText;
        public StoryData latestPlayStory = null; // 마지막 플레이한 스토리 
        public ImageRequireDownload latestStoryBanner;

        [Space]
        public Transform recommendWorkList;
        public GameObject manualGroupPrefab;
        public GameObject hitsGroupPrefab;
        public GameObject genreGroupPrefab;
        List<GameObject> mainCategoryList = new List<GameObject>();
        

        
        /// <summary>
        /// 로비 탭 컨테이너 초기화..
        /// </summary>
        public void InitLobby() {
            
            InitPromotionList();
            InitFastPlay();
            InitMainCategoryList();
        }
        
        
        /// <summary>
        /// 프로모션 초기화
        /// </summary>
        void InitPromotionList() {
            promotionList = SystemManager.main.promotionData;
            
            if(promotionList == null)
                return;

            // 이미 생성된거 있으면 다시 생성.. 
            if (promotionScroll.NumberOfPanels > 0)
                return;
            
            
            for(int i=0; i<promotionList.Count;i++) {
                
                // 생성 
                IFYouPromotionElement promotion = Instantiate(promotionPrefab, promotionContent).GetComponent<IFYouPromotionElement>();
                promotion.SetPromotion(promotionList[i], promotionList[i]["detail"]); // 초기화 
                
                Instantiate(pageTogglePrefab, promotionPagenation); // 페이지네이션 관련 처리 
            }
            
            promotionScroll.Setup();
        }

        /// <summary>
        /// 빠른 플레이 세팅
        /// </summary>
        void InitFastPlay()
        {
            if (StoryManager.main.latestPlayProjectID < 0)
            {
                fastplayButton.SetActive(false);
                return;
            }

            fastplayButton.SetActive(true);
            latestPlayStory = StoryManager.main.FindProject(StoryManager.main.latestPlayProjectID.ToString());
            latestStoryBanner.SetDownloadURL(latestPlayStory.thumbnailURL, latestPlayStory.thumbnailKey);

            if (SystemManager.GetJsonNodeString(StoryManager.main.latestPlayStoryJSON[0], "episode_type") == "chapter")
                SystemManager.SetText(episodeText, string.Format("{0}. {1:D2}", SystemManager.GetLocalizedText("5027"), SystemManager.GetJsonNodeString(StoryManager.main.latestPlayStoryJSON[0], "chapter_number")));
            else
                SystemManager.SetText(episodeText, SystemManager.GetLocalizedText("5025"));
        }


        void InitMainCategoryList()
        {
            if (mainCategoryList.Count > 0)
                return;

            JsonData mainCategoryData = StoryManager.main.mainCategoryListJson;

            if(mainCategoryData == null || mainCategoryData.Count < 1)
            {
                Debug.LogError("메인 카테고리 리스트가 없음! Critical error! Emergency!!!");
                return;
            }

            for (int i = 0; i < mainCategoryData.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(mainCategoryData[i], "project_kind") == "manual")
                {
                    MainManualGroup manualGroup = Instantiate(manualGroupPrefab, recommendWorkList).GetComponent<MainManualGroup>();
                    manualGroup.InitCategoryData(mainCategoryData[i]);
                    mainCategoryList.Add(manualGroup.gameObject);
                }
                else if (SystemManager.GetJsonNodeString(mainCategoryData[i], "project_kind") == "view")
                {
                    MainHitsGroup hitsGroup = Instantiate(hitsGroupPrefab, recommendWorkList).GetComponent<MainHitsGroup>();
                    hitsGroup.InitCategoryData(mainCategoryData[i]);
                    mainCategoryList.Add(hitsGroup.gameObject);
                }
                else
                {
                    MainManualGroup genreGroup = Instantiate(genreGroupPrefab, recommendWorkList).GetComponent<MainManualGroup>();
                    genreGroup.InitCategoryData(mainCategoryData[i]);
                    mainCategoryList.Add(genreGroup.gameObject);
                }
            }
        }
        
        
        /// <summary>
        /// 가장 최근에 플레이 작품 Ready 버튼 클릭 
        /// </summary>
        public void OnClickReady() {
            StoryManager.main.RequestStoryInfo(latestPlayStory);
            //Doozy.Runtime.Signals.Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_INTRODUCE, latestPlayStory);
        }
    }
}