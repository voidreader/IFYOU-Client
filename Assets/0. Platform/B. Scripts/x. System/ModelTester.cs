using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelTester : MonoBehaviour
{

    public Animator animator;
    public Animation singleAnimation;

    public List<AnimationClip> ListHansClip;
    
    public bool param = false;
    public int clipIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        /*
        Debug.Log(singleAnimation.GetClipCount());
        singleAnimation.AddClip(ListHansClip[0], "angry");
        Debug.Log(singleAnimation.GetClipCount());
        */
        // singleAnimation.Play("angry");

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("ListHansClipCount : " + ListHansClip.Count);

            for(int i=0; i<ListHansClip.Count;i++)
            {
                Debug.Log(ListHansClip[i].name);
                ListHansClip[i].legacy = true;
                singleAnimation.AddClip(ListHansClip[i], ListHansClip[i].name);
                // Debug.Log(singleAnimation.GetClipCount());
            }
        }

        /*
        if(Input.GetKeyDown(KeyCode.F))
        {
            param = !param;
            animator.SetBool("NewAction", param);

            SetAnimator();

            Debug.Log("current : " + animator.GetCurrentAnimatorStateInfo(0).ToString());
            Debug.Log("next : " + animator.GetNextAnimatorStateInfo(0).ToString());
        }
        */

        if(Input.GetKeyDown(KeyCode.G))
        {

            
            singleAnimation.CrossFade(ListHansClip[clipIndex++].name);
            if (clipIndex >= ListHansClip.Count)
                clipIndex = 0;
            

            



            
        }

    }

    void SetAnimator()
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        foreach(var a in aoc.animationClips)
        {
            Debug.Log(a.name);
        }
    }
}
