using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;
using TMPro;
using DG.Tweening;

namespace PIERStory {
    public class ViewTitle : CommonView
    {
        
        public static Action<int> OnUpdateLoadingText = null;
        
        public RawImage mainImage; // 다운로드받아서 보여주는 플랫폼 로딩 화면 
        
        [SerializeField] TextMeshProUGUI textLoading;
        [SerializeField] int totalDownloadingImageCount = 0;
        [SerializeField] Image progressBar; 
        
        [SerializeField] GameObject baseScreen; // 기본 스크린 
        
        private void Awake() {
            OnUpdateLoadingText = UpdateLoadingText;
            textLoading.text = string.Empty;
            
            progressBar.fillAmount = 0;
            
            mainImage.gameObject.SetActive(false);
            baseScreen.SetActive(true);
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void OnStartView() {
            base.OnStartView();
            
            // 타이틀 이미지 설정             
            SetTitleTexture();
        }
        
        public override void OnView() {
            base.OnView();
            

            // UserManager에서 원래 connectingDone Signal을 기다린다. 
            // 언어변경에서 넘어오는 경우는 바로 진행 
            if(UserManager.main.completeReadUserData) {
                RequestStoryList();
            }
            
            
            ViewTitle.OnUpdateLoadingText?.Invoke(1);
            
            AdManager.main.AnalyticsEnter("titleEnter");
            
        }
        
        /// <summary>
        /// 로딩 텍스트 변환 
        /// </summary>
        /// <param name="step"></param>
        void UpdateLoadingText(int step) {
            textLoading.text = GetPlatformLoadingText(step);
            
            Debug.Log(" >> UpdateLoadingText :: " + step);
            
            progressBar.DOKill();
            
            // 이미지 fillAmount 추가
            switch(step) {
                case 1:
                //progressBar.DOFillAmount(0.3f, 0.1f);
                progressBar.fillAmount = 0.3f;
                break;
                case 2:
                // progressBar.DOFillAmount(0.7f, 0.1f);
                progressBar.fillAmount = 0.7f;
                break;
                case 3:
                // progressBar.DOFillAmount(1f, 0.1f);
                progressBar.fillAmount = 1;
                break;
            
            }
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        public void RequestStoryList() {
            
            Debug.Log("View Title RequestStoryList");
            
            // * 메인화면 작품 리스트 요청 
            StoryManager.main.RequestStoryList(OnRequestStoryList);
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
            
            StartCoroutine(RoutinePrepareMainPage());
        }
        
        IEnumerator RoutinePrepareMainPage() {

            NetworkLoader.main.RequestNoticeList();     // 공지사항 리스트 요청
            NetworkLoader.main.RequestPromotionList();  // 프로모션 리스트 요청
            NetworkLoader.main.RequestComingSoonList(); // 커밍순 리스트 요청
            NetworkLoader.main.RequestAttendanceList(); // 출석 보상 리스트 요청

            yield return new WaitUntil(() => NetworkLoader.CheckServerWork());

            DownloadStoryMainImages(); // 다운로드 요청 
            
            // 다운로드 완료될때까지 기다린다.
            yield return new WaitUntil(()=> totalDownloadingImageCount <= 0);
            
            // 게이지 다 찰때까지 기다린다.
            // yield return new WaitUntil(()=> progressBar.fillAmount >= 1);
         
            // 준비 끝났으면 signal 전송 
            Signal.Send(LobbyConst.STREAM_IFYOU, "moveMain", "open!");
        }
        
        
        /// <summary>
        /// 메인화면에 필요한 이미지 다운받기 
        /// </summary>
        void DownloadStoryMainImages() {
            totalDownloadingImageCount = 0;
            StoryData currentData;
            string imageURL = string.Empty;
            string imageKey = string.Empty;
            
            for(int i=0; i<StoryManager.main.listTotalStory.Count;i++) {
                
                currentData = StoryManager.main.listTotalStory[i];
                   
                imageURL = currentData.bannerURL;
                imageKey = currentData.bannerKey;
                
                if(string.IsNullOrEmpty(imageURL) || string.IsNullOrEmpty(imageKey))
                    continue;

                // 이미 있으면 continue;                
                if(ES3.FileExists(imageKey))
                    continue;
                
                Debug.Log(imageURL +" / " + imageKey);
                
                // 다운로드 요청 
                totalDownloadingImageCount++; // 유효한 URL당 
                SystemManager.RequestDownloadImage(imageURL, imageKey, OnDownloadEachMainImage);
            }
        }
        
        // 다운로드 이미지 콜백, 성공시에 1씩 차감한다.
        void OnDownloadEachMainImage(HTTPRequest request, HTTPResponse response) {
            
            Debug.Log(string.Format("OnDownloadEachMainImage [{0}] / [{1}]", request.State, response.IsSuccess));
            
            // 성공시, 로컬 저장 
            if(request.State == HTTPRequestStates.Finished && response.IsSuccess) {
                totalDownloadingImageCount--;
            }
        }
        
        
        /// <summary>
        /// 타이틀 texture 설정 
        /// </summary>
        void SetTitleTexture() {
            Texture2D downloadedLoadingTexture = LobbyManager.main.GetRandomPlatformLoadingTexture();
            
            // 다운로드 텍스쳐가 올바르면 교체한다. 
            if(downloadedLoadingTexture != null) {
                mainImage.gameObject.SetActive(true);
                baseScreen.SetActive(false);
                mainImage.texture = downloadedLoadingTexture;
            }
            else { // 다운로드 텍스쳐가 없는경우 
                baseScreen.SetActive(true);
                mainImage.gameObject.SetActive(false);
            }
        }
        
        
        
        /// <summary>
        /// 랜덤 로딩 화면 조회하기.
        /// </summary>
        /// <returns></returns>
        Texture2D GetRandomPlatformLoadingTexture() {
            
            JsonData data = null; // 로컬에 저장된 로딩 이미지 목록 
            int imageIndex = 0; // 랜덤 index 
            string imageKey = string.Empty; // 불러올 이미지 key 
            string imageURL = string.Empty; // 불러올 이미지 url
            Texture2D selectedTexture = null;
            
            
            if(!ES3.KeyExists(SystemConst.KEY_PLATFORM_LOADING))
                return null;
                
            
            data = JsonMapper.ToObject(ES3.Load<string>(SystemConst.KEY_PLATFORM_LOADING));
            Debug.Log("GetRandomPlatformLoadingTexture : " + JsonMapper.ToStringUnicode(data));
            
            if(data == null || data.Count == 0)
                return null;
            
            imageIndex = UnityEngine.Random.Range(0, data.Count);
            imageKey = data[imageIndex][SystemConst.IMAGE_KEY].ToString();
            imageURL = data[imageIndex][SystemConst.IMAGE_URL].ToString();
            
            if(string.IsNullOrEmpty(imageKey) || string.IsNullOrEmpty(imageURL)) 
                return null;
                
            selectedTexture = SystemManager.GetLocalTexture2D(imageKey);
            
           
            return selectedTexture;
        }
        
        
        /// <summary>
        /// 플랫폼 로딩의 텍스트 
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformLoadingText(int step) {
            
            string currentAppLang = string.Empty;
            
            // 로컬라이징 정보가 최초 실행시에는 없다.
            if(!ES3.KeyExists(SystemConst.KEY_LANG)) { // 없는 경우 
            
                Debug.Log("GetPlatformLoadingText : " + Application.systemLanguage);
                
                switch(Application.systemLanguage) {
                    case SystemLanguage.Korean:
                    currentAppLang = "KO";
                    break;
                    
                    default:
                    currentAppLang = "EN";
                    break;
                }
                
                
            }
            else { // 있는 경우 
                currentAppLang = ES3.Load<string>(SystemConst.KEY_LANG);
                currentAppLang = currentAppLang.ToUpper();
            }
            
            switch(step) {
                case 1: 
                return currentAppLang == "KO"?"서버에 접속합니다.":"Connecting to server";
                
                case 2:
                return currentAppLang == "KO"?"플랫폼 정보를 불러오고 있습니다.":"Requesting platform information.";
                
                case 3:
                return currentAppLang == "KO"?"계정 정보를 불러오고 있습니다.":"Loading account Information.";
                
            }
            
            return string.Empty;
            
            
        }
    }
}