using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {
    public class StoryLobbyDeco : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // 안드로이드 백버튼 이슈 때문에 추가
            if(Input.GetKeyDown(KeyCode.Escape) && SystemManager.main.isWebViewOpened) {            
                Debug.Log("StoryLobbyDeco Backbuton Check....");
                SystemManager.main.HideWebviewForce();
            }
        }
    }
    }