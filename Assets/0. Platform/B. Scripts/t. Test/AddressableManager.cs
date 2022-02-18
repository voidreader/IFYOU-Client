using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Framework.Motion;

public class AddressableManager : MonoBehaviour
{
    public Image image;
    public Sprite sprite;
    
    public string assetKey = "";
    public Transform modelParent;
    public GameObject loadedObject = null;
    public Animation modelAnim = null;
    public string modelName = string.Empty;
    
    public List<AnimationClip> ListClips = null;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public void OnClickPlayAnimation() {
        
        
        // motionController.PlayAnimation(ListClips[UnityEngine.Random.Range(0, ListClips.Count)], 0, CubismMotionPriority.PriorityForce);
        loadedObject.GetComponent<CubismMotionController>().PlayAnimation(ListClips[UnityEngine.Random.Range(0, ListClips.Count)], 0, CubismMotionPriority.PriorityForce);
        
        /*
        string motionName = ListClips[UnityEngine.Random.Range(0, ListClips.Count)].name;
        Debug.Log("OnClickPlayAnimation : " + motionName);
        
        modelAnim.CrossFade(motionName, 0.3f);
        */
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
        AsyncOperationHandle<IList<IResourceLocation>> bundleCheckHandle = Addressables.LoadResourceLocationsAsync(assetKey);
        yield return bundleCheckHandle;
        
        Debug.Log("### " + bundleCheckHandle.Status.ToString());
        
        if( bundleCheckHandle.Status == AsyncOperationStatus.Succeeded) { // 실패
            Debug.Log("count bundle :: " + bundleCheckHandle.Result.Count);
        }
    
    }
    
    public void OnClickLoadBundle() {
        
        
        
        Addressables.InstantiateAsync(modelName, Vector3.zero, Quaternion.identity).Completed += (op) => {
            if(op.Status != AsyncOperationStatus.Succeeded) {
                Debug.Log("Failed to InstantiateAsync");
                Addressables.Release(op);
                return;
            }
            
            loadedObject = op.Result;
            
            Live2D.Cubism.Core.CubismModel model = loadedObject.GetComponent<CubismModel>();
            
            Shader cubismShader = Shader.Find("Live2D Cubism/Unlit");
            
            
            /*            
            Material[] materials;
            materials = loadedObject.GetComponentsInChildren<Material>();
            for(int j=0; j<materials.Length;j++) {
                materials[j].shader = cubismShader;
            }
            */
            
            
            for(int i=0;i <model.Drawables.Length;i++) {
                 CubismRenderer render = model.Drawables[i].gameObject.GetComponent<CubismRenderer>();
                 
                 render.Material.shader = cubismShader;
                 
            }
        
            loadedObject.transform.localEulerAngles = Vector3.zero;
            loadedObject.transform.localScale = Vector3.one * 5;
            
            
            // CubismMotionController motionController = loadedObject.GetComponent<CubismMotionController>();
            ModelClips clips = loadedObject.GetComponent<ModelClips>();
            ListClips = clips.ListClips;
            
            Debug.Log("Clip Count : " +  clips.ListClips.Count);
            
            /*
            modelAnim = loadedObject.AddComponent<Animation>();
            
            for(int i =0; i<ListClips.Count;i++) {
                
                // Debug.Log(ListClips[i].name);
                ListClips[i].legacy = true;
                
                
                modelAnim.AddClip(ListClips[i], ListClips[i].name);
            }
            */
            

            Debug.Log("Done InstantiateAsync");
        };
        
        /*
        Addressables.LoadAssetAsync<GameObject>("narim").Completed += (op) => {
            
            
            
            // sprite = op.Result.GetSprite("BG00_노을");
            // image.sprite = sprite;
            // image.SetNativeSize();
        };
        */
    }
    
    public void OnClickDownloadBundle() {
        Addressables.GetDownloadSizeAsync(assetKey).Completed += (op) => {
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
