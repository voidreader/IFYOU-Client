using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using DG.Tweening;
using LitJson;
using System.Linq;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.IO;

using TMPro;
using RTLTMPro;

namespace PIERStory {
    public class TestRunner : MonoBehaviour
    {
        static readonly  Vector2 originSizeDelta = new Vector2(600, 80);
        static readonly  Vector2 selectedSizeDelta = new Vector2(600, 80);
        
        public List<IFYouGameSelectionCtrl> ListSelection;
        
        public TextMeshProUGUI textArabic;
        
        
        RectTransform baseTransform; // 
        protected readonly FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);
        
        public AsyncOperationHandle<SpriteAtlas> firstAtlas;
        
        public AsyncOperationHandle<SpriteAtlas> secondAtlas;
        
        // Start is called before the first frame update
        void Start()
        {
            PopupManager.main.InitPopupManager();
            InitAddressableCatalog();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q)) {
                // ShowSelectionTEST();
                // textArabic.isRightToLeftText = true;
                // textArabic.alignment = TextAlignmentOptions.TopRight;
                // textArabic.text = ArabicFixer.Fix(textArabic.text, false, false);
                string originalText = textArabic.text;
                
                /*
                if(!TextUtils.IsRTLInput(originalText)) {
                    Debug.Log("No Arabic TEXT");
                    return; // 아랍 텍스트 없으면 진행하지 않음. 
                }
                */
                
                textArabic.isRightToLeftText = true;
                
                finalText.Clear();
                RTLSupport.FixRTL(originalText, finalText, false, true, true);
                finalText.Reverse();
                textArabic.text = finalText.ToString();
                
                
                
                
            }
            
            if(Input.GetKeyDown(KeyCode.P)) { 
                /*
                JsonData data = new JsonData();
                data["current"] = JsonMapper.ToObject("{}");
                data["before"] = JsonMapper.ToObject("{}");
                
                data["current"]["level"] = 3;
                data["current"]["experience"] = 7;
                
                data["before"]["level"] = 2;
                data["before"]["experience"] = 7;
                data["before"]["get_experience"] = 10;
                
                PopupBase p =  PopupManager.main.GetPopup("EXP");
                p.Data.SetContentJson(data);
                
                PopupManager.main.ShowPopup(p, false, false);
                */
                PopupBase popup = PopupManager.main.GetPopup(GameConst.POPUP_ACHIEVEMENT_ILLUST);
                PopupManager.main.ShowPopup(popup, true, false);                
            }
        }
        
        public void FillSelection() {
            baseTransform.DOSizeDelta(selectedSizeDelta, 0.4f);
        }
        
        void ShowSelectionTEST() {
            for(int i=0; i<ListSelection.Count; i++) {
                ListSelection[i].SetSelection(i);
            }
        }
        
        void DeleteOldCache() {
            
        }
        
        void InitAddressableCatalog() {
            
            Debug.Log("#### InitAddressableCatalog ###");
            string catalogURL = string.Empty;
            
           
            
            #if UNITY_IOS
            catalogURL = "https://d2dvrqwa14jiay.cloudfront.net/bundle2021/iOS/catalog_1.json";
            
            #else
            catalogURL = "https://d2dvrqwa14jiay.cloudfront.net/bundle2021/Android/catalog_1.json";
            
            #endif
            
            Debug.Log("### InitAddressableCatalog URL ::  " +  catalogURL);
            
            Addressables.LoadContentCatalogAsync(catalogURL).Completed += (op) => {
            
                if(op.Status == AsyncOperationStatus.Succeeded) {
                    Debug.Log("### InitAddressableCatalog " +  op.Status.ToString());
                    // SystemManager.main.isAddressableCatalogUpdated = true;
                    
                    /*
                    List<string> allCachePath = new List<string>();
                    Caching.GetAllCachePaths(allCachePath);
                    for(int i=0; i<allCachePath.Count;i++) {
                        Debug.Log(allCachePath[0]);
                    }
                    
                    
                    Debug.Log("GetCachedVersions");
                    List<Hash128> allHash = new List<Hash128>();
                    Caching.GetCachedVersions("60", allHash);
                    
                    for(int i=0; i<allHash.Count;i++) {
                        Debug.Log(allHash[i].ToString());
                    }
                    */
                    
                    // CleanOldAddressable();
                    // CheckCache("57");
                    LoadAssetIndependent();
                    
                    return;
                }
                else {
                    
                    // 한번더 시도한다. 
                    Addressables.LoadContentCatalogAsync(catalogURL).Completed += (op) => {
            
                        if(op.Status == AsyncOperationStatus.Succeeded) {
                            Debug.Log("### InitAddressableCatalog #2" +  op.Status.ToString());
                            // SystemManager.main.isAddressableCatalogUpdated = true;
                            
                            return;
                        }
                        else {
                            
                            NetworkLoader.main.ReportRequestError(op.OperationException.ToString(), "LoadContentCatalogAsync");
                            SystemManager.main.isAddressableCatalogUpdated = false;
                            
                            
                            //  카탈로그 실패시 접속 할 수 없음. 
                            // SystemManager.ShowSystemPopup(SystemManager.GetDefaultServerErrorMessage(), NetworkLoader.OnFailedServer, NetworkLoader.OnFailedServer, false, false);
                            return;
                            
                        }
                        
                    }; // end of second try
                    
                    
                } // end of else
                
            }; // END!!!
        }
        
        
        void LoadAssetIndependent() {
            
            Debug.Log(" >>>>> LoadAssetIndependent");
            
            
            // key = StoryManager.main.CurrentProjectID + middleKey + imageName;
            // spriteatlas
            string firstKey = "111/bg/BG00_크루즈전경_노을.spriteatlas";
            Addressables.LoadResourceLocationsAsync(firstKey).Completed += (op) => {
                
                // 에셋번들 있음 
               if(op.Status == AsyncOperationStatus.Succeeded && op.Result.Count > 0)  {
                   
                   // 배경은 POT (2의 지수) 이슈로 인해서 SpriteAtals로 불러오도록 처리 
                   Addressables.LoadAssetAsync<SpriteAtlas>(firstKey).Completed += (handle) => {
                       if(handle.Status == AsyncOperationStatus.Succeeded) { // * 성공!
                            
                            firstAtlas = handle; // 메모리 해제를 위한 변수.
                            // sprite = mountedAtalsAddressable.Result.GetSprite(imageName); // 이미지 이름으로 스프라이트 할당 
                            Debug.Log("Succeeded LoaddAssetAsync " + firstKey);
                            
                       }
                       else {
                           
                           Debug.Log(">> Failed LoadAssetAsync " + firstKey + " / " + handle.OperationException.Message);
                           
                       }
                   }; // end of LoadAssetAsync
               }
               else {
                   Debug.Log(">> Failed LoadResourceLocationsAsync " + firstKey + " / " + op.OperationException.Message);
               }
            }; // ? end of LoadResourceLocationsAsync            
        }
        
        
        
        void CheckCache(string key) {
            
            
            
            Addressables.LoadResourceLocationsAsync(key).Completed += (op) => {
                if(op.Status == AsyncOperationStatus.Succeeded) {
                    
                    foreach(IResourceLocation loc in op.Result) {
                        Debug.Log(loc.InternalId + "/"  + loc.ProviderId + "/" + loc.PrimaryKey);
                    }
                    
                    
                }
                else {
                    Debug.Log("Failed CheckCache");
                }
            };
            
            // Addressables.ClearDependencyCacheAsync("font");
            // Addressables.ClearDependencyCacheAsync("101");
            
            
        }
        
        
        void CleanOldAddressable() {
            List<string> cachePaths = new List<string>();
            Caching.GetAllCachePaths(cachePaths);
            var cachedBundles = cachePaths.Where(Directory.Exists).SelectMany(path => Directory.EnumerateFileSystemEntries(path));
 
                foreach (var path in cachedBundles)
                {
                    Debug.Log("path : " + path);
                    var cachedBundleName = Path.GetFileName(path);
 
                    if (!string.IsNullOrEmpty(cachedBundleName))
                    {
                        Debug.Log($"[ADDRESSABLES COLLECTOR] cachedBundleName {cachedBundleName}");
 
                        var cachedBundleVersions = new List<Hash128>();
                        Caching.GetCachedVersions(cachedBundleName, cachedBundleVersions);
                        
                        Debug.Log("Count of cachedBundleVersions : " + cachedBundleVersions.Count);
                     
                        foreach (var ver in cachedBundleVersions)
                        {
                            Debug.Log(ver.ToString());
                            // Caching.ClearCachedVersion(cachedBundleName, ver);
                        }
                    }
                }
        }
    }
}

