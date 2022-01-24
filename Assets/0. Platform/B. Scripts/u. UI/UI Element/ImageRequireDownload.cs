
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using DG.Tweening;

namespace PIERStory {
    
        
    /// <summary>
    /// 다운로드를 받아서 설정해야하는 Image, Raw 이미지 제어 스크립트
    /// </summary>
    public class ImageRequireDownload : MonoBehaviour
    {
        public System.Action OnDownloadImage = null;
        

        [Header("RawImage Check")]
        [SerializeField] bool isRawImage = false; // RawImage인 경우에 체크
        [SerializeField] bool useFade = true; // 이미지를 불러온 경우 FadeIn 사용 
        bool useNativeSize = false;
        
        [Space]
        public bool isLoadComplete = false; // 이미지 정상 로드 완료되었는지 여부 
        [SerializeField] string imageURL = string.Empty;
        [SerializeField] string imageKey = string.Empty;
        
        [SerializeField] Image targetImage = null;
        [SerializeField] RawImage targetRawImage = null;
        public Texture2D downloadedTexture = null;
        public Sprite downloadedSprite = null;
        
        
        
        /// <summary>
        /// 텍스쳐 직접 할당 
        /// </summary>
        /// <param name="__texture"></param>
        public void SetTexture2D(Texture2D __texture) {
            
            InitImage();
            downloadedTexture = __texture;
            
            if(downloadedTexture == null) {
                SetNoImage();
                return;
            }
                
            
            SetTextureToTargetImage();
        }
        
        /// <summary>
        /// 다운로드 URL 및 키 설정 여기가 시작이야.
        /// </summary>
        /// <param name="__url"></param>
        /// <param name="__key"></param>
        public void SetDownloadURL(string __url, string __key, bool useNative = false) {
            
            
            // * 조건 추가 (같은 이미지 여러번 불러오지 않기 위해)
            // 이미 로드 완료한 상태이고, url key 같으면 진행하지 않음.
            if(isLoadComplete && __url == imageURL && __key == imageKey) {
                OnDownloadImage?.Invoke();
                return;
            }
            
            
            imageURL = __url;
            imageKey = __key;
            useNativeSize = useNative;

            // 초기화 
            InitImage();
            
            #region 유효성 체크 
            if(targetRawImage == null && targetImage == null) {
                Debug.LogError("No target images here : " + this.gameObject.name);
                return;
            }
            
            // RawImage인데 RawImage가 없으면 X
            if(isRawImage && targetRawImage == null) {
                Debug.LogError("No target  RawImage here : " + this.gameObject.name);
                return;
            }
            
            if(!isRawImage && targetImage == null) {
                Debug.LogError("No target  Image here : " + this.gameObject.name);
                return;
            }
            
            #endregion
            
            // URL 이나 KEY 정보가 올바르지 않음
            if(string.IsNullOrEmpty(imageURL) || string.IsNullOrEmpty(imageKey)) {
                SetNoImage();
                return;
            } 
            
            // 파일이 있음, 디바이스에서 불러와서 세팅 
            if(ES3.FileExists(imageKey)) {
                LoadLocalSavedImage();
                return;
            }
            else {
                RequestImageDownload(); // 없으면 다운로드
            }
        }
        
        /// <summary>
        /// 이미지 다운로드 요청 
        /// </summary>
        void RequestImageDownload() {
            SystemManager.RequestDownloadImage(imageURL, imageKey, OnCompleteDownload);
        }
        
        void OnCompleteDownload(HTTPRequest request, HTTPResponse response) {
            if(request.State == HTTPRequestStates.Finished && response.IsSuccess) {
                downloadedTexture = response.DataAsTexture2D; // 다운로드 받은 데이터를 texture로 받기. 
                // 불러온 텍스쳐 할당하기
                SetTextureToTargetImage();       
            }
            else {
                Debug.Log("Fail in downloading, " + this.gameObject.name);
                SetNoImage();
            }
        }
        
        
        /// <summary>
        /// 디바이스에 저장된 이미지 불러오기
        /// </summary>
        void LoadLocalSavedImage() {
            
            try {
                downloadedTexture = ES3.LoadImage(imageKey);
            }
            catch(System.Exception e) {
                Debug.Log(e.StackTrace);
                SetNoImage();
                return;
            }
            
            if(!downloadedTexture)  {
                Debug.Log("Fail in loading Downloaded Texture, " + this.gameObject.name);
                SetNoImage();
                return;
            }
            
            // 불러온 텍스쳐 할당하기
            SetTextureToTargetImage();
            
        } // ? 끝!
        
        /// <summary>
        /// 불러오기에 성공한 텍스쳐를 이미지에 세팅
        /// </summary>
        void SetTextureToTargetImage() {
            if(isRawImage) {
                targetRawImage.texture = downloadedTexture;

                if (useNativeSize)
                    targetRawImage.SetNativeSize();

                // 페이드인 쓸래 말래 처리 
                if(useFade)
                    targetRawImage.DOFade(1, 0.4f);
                else 
                    targetRawImage.color = Color.white;
                
            }
            else {
                downloadedSprite = Sprite.Create(downloadedTexture, new Rect(0, 0, downloadedTexture.width, downloadedTexture.height), new Vector2(0.5f, 0.5f));
                targetImage.sprite = downloadedSprite;

                if (useNativeSize)
                    targetImage.SetNativeSize();
                
                if(useFade)
                    targetImage.DOFade(1, 0.4f);
                else 
                    targetImage.color = Color.white;
            }

            
            // 본인 콜백
            OnCompleteLoadImage(); 
            
            // 연결된 다른 콜백
            OnDownloadImage?.Invoke();
        }
        
        
        /// <summary>
        /// 이미지 정상적으로 불러왔을때. 
        /// </summary>
        void OnCompleteLoadImage() {
            isLoadComplete = true; 
        }
        
        /// <summary>
        /// 시작할때 초기화하기
        /// </summary>
        public void InitImage() {
            isLoadComplete = false;
            
            if(targetRawImage) {
                targetRawImage.color = CommonConst.COLOR_IMAGE_TRANSPARENT;
                targetRawImage.texture = null;
            }
            
            if(targetImage) {
                targetImage.color = CommonConst.COLOR_IMAGE_TRANSPARENT;
                targetImage.sprite = null;
                downloadedSprite = null;
             }
        }
        
        
        /// <summary>
        /// 다운로드 받을 수 없음 처리 
        /// </summary>
        void SetNoImage() {
            if(targetRawImage != null) {
                targetRawImage.texture = null;
                targetRawImage.color = Color.black;
            }
            
            if(targetImage != null) {
                targetImage.sprite = null;
                targetImage.color = Color.black;
                downloadedSprite = null;
            }

            OnDownloadImage?.Invoke();
        }
        
    }
}