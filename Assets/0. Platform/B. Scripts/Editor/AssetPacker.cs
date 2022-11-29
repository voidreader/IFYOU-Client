using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityEditor;
using System.IO;

using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.MotionFade;

public class AssetPacker : AssetPostprocessor
{
    
    
    [MenuItem("Assets/[LiveIllust] Collect Model Clips")]    
    public static void CollectLiveIllustClips() {
        
        // * 라이브 오브젝트 후처리 
        
        if (Selection.objects.Length == 0) {
            Debug.LogError("No folder's selected");
            return;
        }
        
        string projectID = string.Empty;
        
        foreach(var obj in Selection.objects) {
            
            
            if(!IsFolder(obj))
                continue;
            
            
            // 선택한 폴더의 경로를 구한다. 
            string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            Debug.Log(string.Format("### origin path [{0}] / simple path [{1}]",path, GetAssetPath(path)));
            
            string extractedPath = path.Replace("Assets/0. NotPOT/", "");
            projectID = extractedPath.Substring(0, extractedPath.IndexOf("/"));
            Debug.Log("Current Project ID : " + projectID);
            
                
            // prefab, AnimationClip만 선택하도록 처리 
            string[] guids = AssetDatabase.FindAssets("t:Prefab t:AnimationClip", new string[] {path});
            
            // * 라이브 개체들은 여기만 보통 들리다. 
            string[] fadeMotion_guids = AssetDatabase.FindAssets("live_illust.fadeMotionList", new string[] {path});
            
            GameObject modelPrefab = null; // 캐릭터 모델 Prefab
            List<AnimationClip> listClips = new List<AnimationClip>(); // 애니메이션 클립들 
            string prefabAssetPath = string.Empty;
            
            CubismFadeMotionList fadeMotionList = null;
            
            if(fadeMotion_guids.Length > 0) {
                Debug.Log("### Found fadeMotionList");
                fadeMotionList = AssetDatabase.LoadAssetAtPath<CubismFadeMotionList>(AssetDatabase.GUIDToAssetPath(fadeMotion_guids[0]));
            }
            else {
                // fadeMotion 모션 없으면 진행하지 않음
                Debug.LogError("No fadeMotionList.");
                continue;
            }
            
            
            
            // for 돌면서 AnimationClip은 수집하고, Prefab에 적용 
            for(int i=0; i<guids.Length;i++) {
                Debug.Log(AssetDatabase.GUIDToAssetPath(guids[i]));
                
                
                if(AssetDatabase.GUIDToAssetPath(guids[i]).Contains(".prefab")) {
                    
                    prefabAssetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    modelPrefab = PrefabUtility.LoadPrefabContents(AssetDatabase.GUIDToAssetPath(guids[i]));
                    // prefab.AddComponent<ModelClips>();
                    // PrefabUtility.ApplyAddedComponent()
                    

                }
                
                if(AssetDatabase.GUIDToAssetPath(guids[i]).Contains(".anim")) {
                    listClips.Add(AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guids[i]))); // clip 추가한다.
                }
                
            } // end of for
            
            
            
            if(modelPrefab == null) {
                Debug.LogError("No model prefab exists!!!");
                return;
            }
            
            ModelClips modelClips;
            
            modelPrefab.AddComponent<ModelClips>();
            modelClips = modelPrefab.GetComponent<ModelClips>();
            
            
            // 클립을 모아준다.
            // modelClips.ListClips = new List<AnimationClip>();
            for(int i=0; i<listClips.Count;i++) {
                modelClips.ListClips.Add(listClips[i]);
            }
            
            if(modelPrefab.GetComponent<CubismMotionController>() == null) {
                modelPrefab.AddComponent<CubismMotionController>();
            }
            
            if(modelPrefab.GetComponent<CubismFadeController>() != null) {
                modelPrefab.GetComponent<CubismFadeController>().CubismFadeMotionList = fadeMotionList;
            }
            
            
            // * 프리팹 저장 
            bool isSuccess = false;
            PrefabUtility.SaveAsPrefabAsset(modelPrefab, prefabAssetPath, out isSuccess);
            
            if(isSuccess) {
                
                // fadeMotion 일치할때만.!
                if(fadeMotionList != null && fadeMotionList.CubismFadeMotionObjects.Length == modelClips.ListClips.Count) {
                
                    string newPath = "Assets/AddressableContens/" + projectID +"_live/live_illust/" + obj.name + ".prefab";
                    Debug.Log("Prefab update is done :: " + newPath);
                    
                    // 성공하면 프리팹을 옮겨주기. 
                    AssetDatabase.MoveAsset(prefabAssetPath, newPath);
                    
                    
                    
                    // 현재 위치에서의 json 파일 모두 제거 
                    string[] textAssets = AssetDatabase.FindAssets("t:TextAsset", new string[] {path});
                    Debug.Log("## textAssets : " + textAssets.Length);
                    for(int i=0; i<textAssets.Length;i++) {
                        AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(textAssets[i]));
                    }
                    
                    AssetDatabase.SaveAssets();
                }
            }
            else {
                Debug.Log("Prefab update failed");
            }            
            
        } // end of foreach
    } // ? CollectLiveIllustClips
    
   
    [MenuItem("Assets/[LiveObject] Collect Model Clips")]    
    public static void CollectLiveObjectClips() {
        
        // * 라이브 오브젝트 후처리 
        
        if (Selection.objects.Length == 0) {
            Debug.LogError("No folder's selected");
            return;
        }
        
        string projectID = string.Empty;
        
        foreach(var obj in Selection.objects) {
            
            
            if(!IsFolder(obj))
                continue;
            
            
            // 선택한 폴더의 경로를 구한다. 
            string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            Debug.Log(string.Format("### origin path [{0}] / simple path [{1}]",path, GetAssetPath(path)));
            
            string extractedPath = path.Replace("Assets/0. NotPOT/", "");
            projectID = extractedPath.Substring(0, extractedPath.IndexOf("/"));
            Debug.Log("Current Project ID : " + projectID);
            
                
            // prefab, AnimationClip만 선택하도록 처리 
            string[] guids = AssetDatabase.FindAssets("t:Prefab t:AnimationClip", new string[] {path});
            
            // * 라이브 개체들은 여기만 보통 들리다. 
            string[] fadeMotion_guids = AssetDatabase.FindAssets("live_object.fadeMotionList", new string[] {path});
            
            GameObject modelPrefab = null; // 캐릭터 모델 Prefab
            List<AnimationClip> listClips = new List<AnimationClip>(); // 애니메이션 클립들 
            string prefabAssetPath = string.Empty;
            
            CubismFadeMotionList fadeMotionList = null;
            
            if(fadeMotion_guids.Length > 0) {
                Debug.Log("### Found fadeMotionList");
                fadeMotionList = AssetDatabase.LoadAssetAtPath<CubismFadeMotionList>(AssetDatabase.GUIDToAssetPath(fadeMotion_guids[0]));
            }
            else {
                // fadeMotion 모션 없으면 진행하지 않음
                Debug.LogError("No fadeMotionList.");
                continue;
            }
            
            
            
            // for 돌면서 AnimationClip은 수집하고, Prefab에 적용 
            for(int i=0; i<guids.Length;i++) {
                Debug.Log(AssetDatabase.GUIDToAssetPath(guids[i]));
                
                
                if(AssetDatabase.GUIDToAssetPath(guids[i]).Contains(".prefab")) {
                    
                    prefabAssetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    modelPrefab = PrefabUtility.LoadPrefabContents(AssetDatabase.GUIDToAssetPath(guids[i]));
                    // prefab.AddComponent<ModelClips>();
                    // PrefabUtility.ApplyAddedComponent()
                    

                }
                
                if(AssetDatabase.GUIDToAssetPath(guids[i]).Contains(".anim")) {
                    listClips.Add(AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guids[i]))); // clip 추가한다.
                }
                
            } // end of for
            
            
            
            if(modelPrefab == null) {
                Debug.LogError("No model prefab exists!!!");
                return;
            }
            
            ModelClips modelClips;
            
            modelPrefab.AddComponent<ModelClips>();
            modelClips = modelPrefab.GetComponent<ModelClips>();
            
            
            // 클립을 모아준다.
            // modelClips.ListClips = new List<AnimationClip>();
            for(int i=0; i<listClips.Count;i++) {
                modelClips.ListClips.Add(listClips[i]);
            }
            
            if(modelPrefab.GetComponent<CubismMotionController>() == null) {
                modelPrefab.AddComponent<CubismMotionController>();
            }
            
            if(modelPrefab.GetComponent<CubismFadeController>() != null) {
                modelPrefab.GetComponent<CubismFadeController>().CubismFadeMotionList = fadeMotionList;
            }
            
            
            // * 프리팹 저장 
            bool isSuccess = false;
            PrefabUtility.SaveAsPrefabAsset(modelPrefab, prefabAssetPath, out isSuccess);
            
            if(isSuccess) {
                
                // fadeMotion 일치할때만.!
                if(fadeMotionList != null && fadeMotionList.CubismFadeMotionObjects.Length == modelClips.ListClips.Count) {
                
                    string newPath = "Assets/AddressableContens/" + projectID +"_live/live_object/" + obj.name + ".prefab";
                    Debug.Log("Prefab update is done :: " + newPath);
                    
                    // 성공하면 프리팹을 옮겨주기. 
                    AssetDatabase.MoveAsset(prefabAssetPath, newPath);
                    
                    // 현재 위치에서의 json 파일 모두 제거
                    string[] textAssets = AssetDatabase.FindAssets("t:TextAsset", new string[] {path});
                    Debug.Log("## textAssets : " + textAssets.Length);
                    for(int i=0; i<textAssets.Length;i++) {
                        AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(textAssets[i]));
                    }                    
                    
                    AssetDatabase.SaveAssets();
                }
            }
            else {
                Debug.Log("Prefab update failed");
            }            
            
        } // end of foreach
    } // ? CollectLiveObjectClips
   
    
    [MenuItem("Assets/[Character] Collect Model Clips")]    
    public static void CollectLiveClips() {
        
        // 폴더에서만 동작하도록 한다. 
        // 폴더내의 AnimationClip과 prefab을 찾아서 prefab에 적용해준다. 
        // 폴더 하나만 선택하도록 해야 할 것 같다.
        /*
        if(Selection.objects.Length > 1) {
            Debug.LogError("Too many folders. Only one folder is available");
            return;
        }
        */
        if (Selection.objects.Length == 0) {
            Debug.LogError("No folder's selected");
            return;
        }
        
        string projectID = string.Empty;
        
        foreach(var obj in Selection.objects) {
            
            
            if(!IsFolder(obj))
                continue;
            
            
            // 선택한 폴더의 경로를 구한다. 
            string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            Debug.Log(string.Format("### origin path [{0}] / simple path [{1}]",path, GetAssetPath(path)));
            
            string extractedPath = path.Replace("Assets/0. NotPOT/", "");
            projectID = extractedPath.Substring(0, extractedPath.IndexOf("/"));
            Debug.Log("Current Project ID : " + projectID);
            
                
            // prefab, AnimationClip만 선택하도록 처리 
            //string[] guids = AssetDatabase.FindAssets("t:Prefab t:AnimationClip", new string[] {path});
            string[] guids = AssetDatabase.FindAssets("t:Prefab t:AnimationClip", new string[] {path});
            string[] fadeMotion_guids = AssetDatabase.FindAssets("model.fadeMotionList", new string[] {path});
            
            GameObject modelPrefab = null; // 캐릭터 모델 Prefab
            List<AnimationClip> listClips = new List<AnimationClip>(); // 애니메이션 클립들 
            string prefabAssetPath = string.Empty;
            
            CubismFadeMotionList fadeMotionList = null;
            
            if(fadeMotion_guids.Length > 0) {
                Debug.Log("### Found fadeMotionList");
                fadeMotionList = AssetDatabase.LoadAssetAtPath<CubismFadeMotionList>(AssetDatabase.GUIDToAssetPath(fadeMotion_guids[0]));
            }
            else {
                // fadeMotion 모션 없으면 진행하지 않음
                Debug.LogError("No model.fadeMotionList.");
                continue;
            }
            
            
            
            // for 돌면서 AnimationClip은 수집하고, Prefab에 적용 
            for(int i=0; i<guids.Length;i++) {
                Debug.Log(AssetDatabase.GUIDToAssetPath(guids[i]));
                
                
                if(AssetDatabase.GUIDToAssetPath(guids[i]).Contains(".prefab")) {
                    
                    prefabAssetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    modelPrefab = PrefabUtility.LoadPrefabContents(AssetDatabase.GUIDToAssetPath(guids[i]));
                    // prefab.AddComponent<ModelClips>();
                    // PrefabUtility.ApplyAddedComponent()
                    

                }
                
                if(AssetDatabase.GUIDToAssetPath(guids[i]).Contains(".anim")) {
                    listClips.Add(AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guids[i]))); // clip 추가한다.
                }
                
            } // end of for
            
            
            
            if(modelPrefab == null) {
                Debug.LogError("No model prefab exists!!!");
                return;
            }
            
            ModelClips modelClips;
            
            modelPrefab.AddComponent<ModelClips>();
            modelClips = modelPrefab.GetComponent<ModelClips>();
            
            
            // 클립을 모아준다.
            // modelClips.ListClips = new List<AnimationClip>();
            for(int i=0; i<listClips.Count;i++) {
                modelClips.ListClips.Add(listClips[i]);
            }
            
            if(modelPrefab.GetComponent<CubismMotionController>() == null) {
                modelPrefab.AddComponent<CubismMotionController>();
            }
            
            if(modelPrefab.GetComponent<CubismFadeController>() != null) {
                modelPrefab.GetComponent<CubismFadeController>().CubismFadeMotionList = fadeMotionList;
            }
            
            
            // * 프리팹 저장 
            bool isSuccess = false;
            PrefabUtility.SaveAsPrefabAsset(modelPrefab, prefabAssetPath, out isSuccess);
            
            if(isSuccess) {
                
                // fadeMotion 일치할때만.!
                if(fadeMotionList != null && fadeMotionList.CubismFadeMotionObjects.Length == modelClips.ListClips.Count) {
                
                    //string newPath = "Assets/AddressableContens/" + projectID +"/model/" + GetAssetName(prefabAssetPath);
                    string newPath = "Assets/AddressableContens/" + projectID +"_model/model/" + obj.name + ".prefab";
                    Debug.Log("Prefab update is done :: " + newPath);
                    
                    // 성공하면 프리팹을 옮겨주기. 
                    AssetDatabase.MoveAsset(prefabAssetPath, newPath);
                   
                    
                    // 현재 위치에서의 json 파일 모두 제거 
                    string[] textAssets = AssetDatabase.FindAssets("t:TextAsset", new string[] {path});
                    Debug.Log("## textAssets : " + textAssets.Length);
                    for(int i=0; i<textAssets.Length;i++) {
                        AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(textAssets[i]));
                    }
                    
                    AssetDatabase.SaveAssets();
                    
                }
            }
            else {
                Debug.Log("Prefab update failed");
            }            
            
        } // end of foreach
    }
    
    
    
    [MenuItem("Assets/[BG] Create SpriteAtlas")]    
    public static void CreateAtalsWithSelectedSprite() {
        
        // 스프라이트 선택 후 대상 메뉴를 통해서 아틀라스를 자동으로 만들어보자.!
        string originPath = string.Empty;
        string newPath = string.Empty;
        string projectID = string.Empty;
        
        
        foreach(var obj in Selection.objects) {
            if(IsFolder(obj))
                continue;
             
            /*   
            if(obj.GetType() != typeof(Sprite)) {
                continue;
            }
            */
            
            string atlasName = GetAssetPath(AssetDatabase.GetAssetPath(obj.GetInstanceID())) + GetAssetNameWithoutExt(AssetDatabase.GetAssetPath(obj.GetInstanceID())) + ".spriteatlas";
            originPath = atlasName;
            Debug.Log(atlasName);
            
            string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            string extractedPath = path.Replace("Assets/0. NotPOT/", "");
            projectID = extractedPath.Substring(0, extractedPath.IndexOf("/"));
            Debug.Log("Current Project ID : " + projectID);            
            
            
            
            SpriteAtlas atlas = new SpriteAtlas();
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings();
            packSetting.enableRotation = false;
            packSetting.enableTightPacking = false;
            atlas.SetPackingSettings(packSetting);
            
            AssetDatabase.CreateAsset(atlas, atlasName);
          
            
            Object o = obj as Texture2D;
            
            if(o != null) {
                Debug.Log("ADD Sprite in " + atlasName);
                SpriteAtlasExtensions.Add(atlas, new Object[] {o});
                SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] {atlas}, EditorUserBuildSettings.activeBuildTarget);
                
                newPath = "Assets/AddressableContens/" + projectID +"_image/bg/" + GetAssetName(originPath);
                Debug.Log("Prefab update is done :: " + newPath);                
                
                // 성공하면 프리팹을 옮겨주기. 
                AssetDatabase.MoveAsset(originPath, newPath);
            }
            AssetDatabase.SaveAssets();  
            
        }
    }
    
    [MenuItem("Assets/[illust] Create SpriteAtlas")]    
    public static void CreateIllustAtlas() {
        
        // 스프라이트 선택 후 대상 메뉴를 통해서 아틀라스를 자동으로 만들어보자.!
        string originPath = string.Empty;
        string newPath = string.Empty;
        string projectID = string.Empty;
        
        foreach(var obj in Selection.objects) {
            if(IsFolder(obj))
                continue;
             
            /*   
            if(obj.GetType() != typeof(Sprite)) {
                continue;
            }
            */
            
            string atlasName = GetAssetPath(AssetDatabase.GetAssetPath(obj.GetInstanceID())) + GetAssetNameWithoutExt(AssetDatabase.GetAssetPath(obj.GetInstanceID())) + ".spriteatlas";
            originPath = atlasName;
            Debug.Log(atlasName);
            
            string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            string extractedPath = path.Replace("Assets/0. NotPOT/", "");
            projectID = extractedPath.Substring(0, extractedPath.IndexOf("/"));
            Debug.Log("Current Project ID : " + projectID);            
            
            
            SpriteAtlas atlas = new SpriteAtlas();
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings();
            packSetting.enableRotation = false;
            packSetting.enableTightPacking = false;
            atlas.SetPackingSettings(packSetting);
            
            AssetDatabase.CreateAsset(atlas, atlasName);
          
            
            Object o = obj as Texture2D;
            
            if(o != null) {
                Debug.Log("ADD Sprite in " + atlasName);
                SpriteAtlasExtensions.Add(atlas, new Object[] {o});
                SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] {atlas}, EditorUserBuildSettings.activeBuildTarget);
                
                newPath = "Assets/AddressableContens/" + projectID +"_image/illust/" + GetAssetName(originPath);
                Debug.Log("Prefab update is done :: " + newPath);
                
                // 성공하면 프리팹을 옮겨주기. 
                AssetDatabase.MoveAsset(originPath, newPath);
            }
            

            
            AssetDatabase.SaveAssets();  
            
        }
    }
    
    
    
    [MenuItem("Assets/[Image] Create SpriteAtlas")]    
    public static void CreateImageAtlas() {
        
        // 스프라이트 선택 후 대상 메뉴를 통해서 아틀라스를 자동으로 만들어보자.!
        string originPath = string.Empty;
        string newPath = string.Empty;
        string projectID = string.Empty;
        
        foreach(var obj in Selection.objects) {
            if(IsFolder(obj))
                continue;
             
            /*   
            if(obj.GetType() != typeof(Sprite)) {
                continue;
            }
            */
            
            string atlasName = GetAssetPath(AssetDatabase.GetAssetPath(obj.GetInstanceID())) + GetAssetNameWithoutExt(AssetDatabase.GetAssetPath(obj.GetInstanceID())) + ".spriteatlas";
            originPath = atlasName;
            Debug.Log(atlasName);
            
            string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            string extractedPath = path.Replace("Assets/0. NotPOT/", "");
            projectID = extractedPath.Substring(0, extractedPath.IndexOf("/"));
            Debug.Log("Current Project ID : " + projectID);            
            
            
            SpriteAtlas atlas = new SpriteAtlas();
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings();
            packSetting.enableRotation = false;
            packSetting.enableTightPacking = false;
            atlas.SetPackingSettings(packSetting);
            
            AssetDatabase.CreateAsset(atlas, atlasName);
          
            
            Object o = obj as Texture2D;
            
            if(o != null) {
                Debug.Log("ADD Sprite in " + atlasName);
                SpriteAtlasExtensions.Add(atlas, new Object[] {o});
                SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] {atlas}, EditorUserBuildSettings.activeBuildTarget);
                
                newPath = "Assets/AddressableContens/" + projectID +"_image/image/" + GetAssetName(originPath);
                Debug.Log("Prefab update is done :: " + newPath);
                
                // 성공하면 프리팹을 옮겨주기. 
                AssetDatabase.MoveAsset(originPath, newPath);
            }
            
                
                
            
            AssetDatabase.SaveAssets();  
            
        }
    }
    
    
    /// <summary>
    /// 선택 개체가 폴더인지 체크 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsFolder(UnityEngine.Object obj) {
        if(obj == null) return false;

        string path = "";
        path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
        if (path.Length > 0)
        {
            if (Directory.Exists(path)) return true;
        }
        return false;        
        
    }
    
    
    
    /// <summary>
    /// 에셋 있는지 없는지 체크 
    /// </summary>
    /// <param name="__assetPath"></param>
    /// <returns></returns>
    public static bool IsExists(string __assetPath) {
        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(__assetPath)) || AssetDatabase.GUIDFromAssetPath(__assetPath) == null  ) return false;
        return true;
    }
    
    /// <summary>
    /// 확장자 없는 에셋 파일 이름 
    /// </summary>
    /// <param name="sAssetPath"></param>
    /// <returns></returns>
    public static string GetAssetNameWithoutExt(string sAssetPath) {
        string assetName = sAssetPath.Substring(sAssetPath.LastIndexOf("/") + 1);
        return assetName.Split('.')[0];
    }
    
    
    /// <summary>
    ///  AssetPath를 넣으면 해당 에셋 파일 이름을 반환
    /// </summary>
    /// <param name="sAssetPath"></param>
    /// <returns></returns>
    public static string GetAssetName(string sAssetPath)
    {
        string sAssetName = sAssetPath.Substring(sAssetPath.LastIndexOf("/") + 1);
        return sAssetName;
    }

    /// <summary>
    /// 에셋 패스를 넣으면 경로 반환 (파일 이름은 제외)
    /// </summary>
    /// <param name="sAssetPath"></param>
    /// <returns></returns>
    public static string GetAssetPath(string sAssetPath)
    {
        string sAssetName = sAssetPath.Substring(0, sAssetPath.LastIndexOf("/"));
        return sAssetName + "/";
    }    
    
    
    [MenuItem("Assets/IFYOU/AssetDatabase Refresh")]
    public static void RefreshDatabase() {
        
        Debug.Log("AssetDatabase refresh");
        
        AssetDatabase.Refresh();
    }
    
    /*
    
    void OnPostprocessAudio(AudioClip clip) {
        AudioImporter audioImporter = (AudioImporter)assetImporter;
        
        if(audioImporter != null) {
            if(audioImporter.preloadAudioData) {
                audioImporter.preloadAudioData = false;
            }
            
            if(!audioImporter.forceToMono) {
                audioImporter.forceToMono = true;
            }
            
            AudioImporterSampleSettings sampleSettings = audioImporter.defaultSampleSettings;
            sampleSettings.loadType = AudioClipLoadType.DecompressOnLoad;
            sampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;
            sampleSettings.quality = 1f;
            
            #if UNITY_ANDROID
            audioImporter.SetOverrideSampleSettings("Android", sampleSettings);
            
            #elif UNITY_IOS
            audioImporter.SetOverrideSampleSettings("iOS", sampleSettings);
            #endif
            
            
        }
        
    }
    */
    
}
