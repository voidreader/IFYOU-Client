using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Framework.Motion;
using UnityEngine.SceneManagement;

public class ModelManager : MonoBehaviour
{
    public List<GameObject> ListModels;
    
    public GameObject currentModel;
    public List<AnimationClip> ListClips = null;
    public CubismMotionController motionController;
    
    public int modelIndex = 0;
    
    void Update() {
        if(Input.GetKeyDown(KeyCode.A)) {
            PlayAnimation();
        }
        else if(Input.GetKeyDown(KeyCode.S)) {
            NextModel();
        }
        else if(Input.GetKeyDown(KeyCode.Z)) {
            SceneManager.LoadSceneAsync("Sample", LoadSceneMode.Single).allowSceneActivation = true;
        }
    }
    
    public void NextModel() {
        
        if(ListModels == null)
            return;
            
        for(int i=0; i<ListModels.Count;i++) {
            ListModels[i].gameObject.SetActive(false);
        }
        
        currentModel = ListModels[modelIndex];
        currentModel.SetActive(true);
        currentModel.transform.localScale = new Vector3(15,15,15);
        currentModel.transform.localPosition = new Vector3(0, -7, 0);
        
        modelIndex ++;
        
        if(modelIndex >= ListModels.Count)
            modelIndex = 0;
    }
    
    
    public void PlayAnimation() {
        
        ListClips = currentModel.GetComponent<ModelClips>().ListClips;
        
        motionController = currentModel.GetComponent<CubismMotionController>();
        
        AnimationClip clip = ListClips[UnityEngine.Random.Range(0, ListClips.Count)];
        
        motionController.PlayAnimation(clip, 0, CubismMotionPriority.PriorityForce);
        Debug.Log(clip.name);
        
        
    }
}
