using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace PIERStory {
    
    
    public class BeginSceneController : MonoBehaviour
    {
        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return null;
            yield return null;
            yield return null;
            SceneManager.LoadSceneAsync(CommonConst.SCENE_LOBBY, LoadSceneMode.Single).allowSceneActivation = true;
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