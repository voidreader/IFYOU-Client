using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using TMPro;
using LitJson;
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

            StartCoroutine(LayoutRebuild());
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

            try {
                fastplayButton.SetActive(true);
                latestPlayStory = StoryManager.main.FindProject(StoryManager.main.latestPlayProjectID.ToString());
                
                if(!latestPlayStory.isValidData) {
                    fastplayButton.SetActive(false);
                    return;
                }
                
                latestStoryBanner.SetDownloadURL(latestPlayStory.thumbnailURL, latestPlayStory.thumbnailKey);

                // 정규 에피소드의 경우 에피소드 + 현재 챕터
                /*
                if (SystemManager.GetJsonNodeString(StoryManager.main.latestPlayStoryJSON[0], "episode_type") == "chapter")
                    SystemManager.SetText(episodeText, string.Format("{0}. {1:D2}", SystemManager.GetLocalizedText("5027"), SystemManager.GetJsonNodeString(StoryManager.main.latestPlayStoryJSON[0], "chapter_number")));
                else
                    SystemManager.SetText(episodeText, SystemManager.GetLocalizedText("5025"));
                */
            }
            catch {
                fastplayButton.SetActive(false);
                return;
            }
        }


        /// <summary>
        /// 어드민에서 전달받은 메인 카테고리 리스트대로 세팅
        /// </summary>
        void InitMainCategoryList()
        {
            // 중복 생성 방지
            if (mainCategoryList.Count > 0)
                return;

            JsonData mainCategoryData = StoryManager.main.mainCategoryListJson;

            if(mainCategoryData == null || mainCategoryData.Count < 1)
            {
                Debug.LogError("메인 카테고리 리스트가 없음! Critical error! Emergency!!!");
                return;
            }

            mainCategoryList.Clear();

            for (int i = 0; i < mainCategoryData.Count; i++)
            {
                // 수동 설정의 경우
                if (SystemManager.GetJsonNodeString(mainCategoryData[i], "project_kind") == "manual")
                {
                    MainManualGroup manualGroup = Instantiate(manualGroupPrefab, recommendWorkList).GetComponent<MainManualGroup>();
                    manualGroup.InitCategoryData(mainCategoryData[i]);
                    mainCategoryList.Add(manualGroup.gameObject);
                }
                // 조회수 대로
                else if (SystemManager.GetJsonNodeString(mainCategoryData[i], "project_kind") == "view")
                {
                    MainHitsGroup hitsGroup = Instantiate(hitsGroupPrefab, recommendWorkList).GetComponent<MainHitsGroup>();
                    hitsGroup.InitCategoryData(mainCategoryData[i]);
                    mainCategoryList.Add(hitsGroup.gameObject);
                }
                // 장르 대로
                else
                {
                    MainManualGroup genreGroup = Instantiate(genreGroupPrefab, recommendWorkList).GetComponent<MainManualGroup>();
                    genreGroup.InitCategoryData(mainCategoryData[i]);
                    mainCategoryList.Add(genreGroup.gameObject);
                }
            }
        }
        
        IEnumerator LayoutRebuild()
        {
            yield return new WaitUntil(() => mainCategoryList.Count > 0);

            yield return null;

            yield return new WaitUntil(() => mainCategoryList[0].activeSelf);

            foreach(GameObject g in mainCategoryList)
            {
                if (g == null || g.GetComponent<MainManualGroup>() == null)
                    continue;

                g.GetComponent<MainManualGroup>().ResizeArea();
            }
            
            recommendWorkList.gameObject.SetActive(false);
            yield return null;
            recommendWorkList.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 가장 최근에 플레이 작품 Ready 버튼 클릭 
        /// </summary>
        public void OnClickReady() {
            StoryManager.main.RequestStoryInfo(latestPlayStory);
            
            
            Firebase.Analytics.FirebaseAnalytics.LogEvent("main_quickstart");
        }
    }
}