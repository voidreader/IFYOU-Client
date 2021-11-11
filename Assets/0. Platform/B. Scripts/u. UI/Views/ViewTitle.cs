using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using BestHTTP;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory {
    public class ViewTitle : CommonView
    {
        
        public RawImage mainImage;
        [SerializeField] UISlider progressor;
        float loadingGauge = 0f;
        
        public override void OnView() {
            base.OnView();
            
            progressor.value = 0;
            
            SetTitleTexture();
            
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
            
            // 작품 리스트 받아와서 스토리 매니저에게 전달. 
            StoryManager.main.totalStoryListJson = JsonMapper.ToObject(response.DataAsText);
            
            progressor.value = 1f;
        }
        
        
        
        
        
        /// <summary>
        /// 타이틀 texture 설정 
        /// </summary>
        void SetTitleTexture() {
            Texture2D downloadedLoadingTexture = LobbyManager.main.GetRandomPlatformLoadingTexture();
            
            // 다운로드 텍스쳐가 올바르면 교체한다. 
            if(downloadedLoadingTexture != null) {
                mainImage.texture = downloadedLoadingTexture;
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
            
            imageIndex = Random.Range(0, data.Count);
            imageKey = data[imageIndex][SystemConst.IMAGE_KEY].ToString();
            imageURL = data[imageIndex][SystemConst.IMAGE_URL].ToString();
            
            if(string.IsNullOrEmpty(imageKey) || string.IsNullOrEmpty(imageURL)) 
                return null;
                
            selectedTexture = SystemManager.GetLocalTexture2D(imageKey);
            
           
            return selectedTexture;
            
               
        }
    }
}