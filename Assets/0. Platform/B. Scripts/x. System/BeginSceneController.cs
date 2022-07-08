using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;


namespace PIERStory {
    
    
    public class BeginSceneController : MonoBehaviour
    {
        public AsyncOperation sceneOperation;
        public Image loadingBar;
        public RectTransform circle;
        
        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return null;
            yield return null;
            yield return null;
            sceneOperation = SceneManager.LoadSceneAsync(CommonConst.SCENE_MAIN_LOBBY, LoadSceneMode.Single);
            sceneOperation.allowSceneActivation = true;
            
            circle.DORotate(new Vector3(0,0, -360), 2, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            
            while(sceneOperation.progress < 1) {
                loadingBar.fillAmount = sceneOperation.progress;
                yield return null;
            }
            
            
        }
        
        void OnApplicationPause(bool pauseStatus) {
            
            if(Application.isEditor)
                return;
            
            // 이상태에서 백그라운드로 돌릴때. 
            if(pauseStatus) {
                Debug.Log("OnApplicationPause in BeginScene");
                Application.Quit();
            }
        }
    }
}