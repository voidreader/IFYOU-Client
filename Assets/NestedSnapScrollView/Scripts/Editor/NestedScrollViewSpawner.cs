using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NestedScroll.Editor
{
    public class NestedScrollViewSpawner
    {
        [MenuItem("GameObject/UI/NestedScroll/ScrollRect/DefaultScrollView", false, 0)]
        public static void SpawnOnSceneExternPrefab()
        {
            List<NestedScrollViewContainer> container = FindAssetsByType<NestedScrollViewContainer>();

            if (container.Count > 0)
            {
                if (Selection.gameObjects.Length > 0)
                    MonoBehaviour.Instantiate(container[0].ExternScrollView, Selection.gameObjects[0].transform);
                else
                    MonoBehaviour.Instantiate(container[0].ExternScrollView);
            }
            else
            {
                Debug.LogWarning("Missing some objects on project folders ... reimport plugin please ");
            }
        }

        [MenuItem("GameObject/UI//NestedScroll/ScrollContentElement/DefaultElement", false, 0)]
        public static void SpawnOnSceneNestedPrefab()
        {
            List<NestedScrollViewContainer> container = FindAssetsByType<NestedScrollViewContainer>();
            if (container.Count > 0)
            {
                if (Selection.gameObjects.Length > 0)
                    MonoBehaviour.Instantiate(container[0].NestedScrollView, Selection.gameObjects[0].transform);
                else
                    MonoBehaviour.Instantiate(container[0].NestedScrollView);
            }
            else
            {
                Debug.LogWarning("Missing some objects on project folders ... reimport plugin please ");
            }
        }

        [MenuItem("GameObject/UI//NestedScroll/ScrollRect/SnappingScrollView", false, 0)]
        public static void SpawnOnSceneExternPrefabWithSnap()
        {
            List<NestedScrollViewContainer> container = FindAssetsByType<NestedScrollViewContainer>();
            if (container.Count > 0)
            {
                if (Selection.gameObjects.Length > 0)
                    MonoBehaviour.Instantiate(container[0].ExternWithSnapScrollView,
                        Selection.gameObjects[0].transform);
                else
                    MonoBehaviour.Instantiate(container[0].ExternWithSnapScrollView);
            }
            else
            {

                Debug.LogWarning("Missing some objects on project folders ... reimport plugin please ");
            }
        }

        [MenuItem("GameObject/UI/NestedScroll/ScrollContentElement/SnappingElement", false, 0)]
        public static void SpawnOnSceneNestedPrefabWithSnap()
        {
            List<NestedScrollViewContainer> container = FindAssetsByType<NestedScrollViewContainer>();
            if (container.Count > 0)
            {
                if (Selection.gameObjects.Length > 0)
                    MonoBehaviour.Instantiate(container[0].NestedWithSnapScrollView,
                        Selection.gameObjects[0].transform);
                else
                    MonoBehaviour.Instantiate(container[0].NestedWithSnapScrollView);
            }
            else
            {

                Debug.LogWarning("Missing some objects on project folders ... reimport plugin please ");
            }
        }
        
        [MenuItem("GameObject/UI/NestedScroll/ScrollContentElement/SideMenuElement", false, 0)]
        public static void SpawnOnSceneNestedPrefabScrollMenu()
        {
            List<NestedScrollViewContainer> container = FindAssetsByType<NestedScrollViewContainer>();
            if (container.Count > 0)
            {
                if (Selection.gameObjects.Length > 0)
                    MonoBehaviour.Instantiate(container[0].NestedSideMenu,
                        Selection.gameObjects[0].transform);
                else
                    MonoBehaviour.Instantiate(container[0].NestedSideMenu);
            }
            else
            {
                Debug.LogWarning("Missing some objects on project folders ... reimport plugin please ");
            }
        }

        public static List<T> FindAssetsByType<T>() where T : NestedScrollViewContainer
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            
            Debug.Log(assets.Count);
            return assets;
        }
    }
}
