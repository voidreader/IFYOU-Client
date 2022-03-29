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

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}