using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressableManager : MonoBehaviour
{
    public Image image;
    public Sprite sprite;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClickUpdateCatalog() {
        Addressables.LoadContentCatalogAsync("https://d2dvrqwa14jiay.cloudfront.net/bundle/Android/catalog_1.json").Completed += (op) => {
            Debug.Log(op.Status.ToString());
        };
    }
    
    public void OnClickBundleCheck() {
        StartCoroutine(CheckingBundle());
    }
    
    IEnumerator CheckingBundle() {
        AsyncOperationHandle<IList<IResourceLocation>> bundleCheckHandle = Addressables.LoadResourceLocationsAsync("57");
        yield return bundleCheckHandle;
        
        Debug.Log("### " + bundleCheckHandle.Status.ToString());
        
        if( bundleCheckHandle.Status == AsyncOperationStatus.Succeeded) { // 실패
            Debug.Log("count bundle :: " + bundleCheckHandle.Result.Count);
        }
    
    }
    
    public void OnClickLoadBundle() {
        Addressables.LoadAssetAsync<SpriteAtlas>("BG00_노을").Completed += (op) => {
            
            sprite = op.Result.GetSprite("BG00_노을");
            image.sprite = sprite;
          // image.SetNativeSize();
        };
    }
    
    public void OnClickDownloadBundle() {
        Addressables.GetDownloadSizeAsync("57").Completed += (op) => {
            Debug.Log("### GetDownloadSizeAsync : " + op.Result);
            
            if(op.Result > 0) {
                            // 다운로드 해야한다. 
                AsyncOperationHandle downloadHandle =  Addressables.DownloadDependenciesAsync("57");
                downloadHandle.Completed += (op) => {
                    Debug.Log("#### done");
                };
            }
        };
    }
    
    
    
}
