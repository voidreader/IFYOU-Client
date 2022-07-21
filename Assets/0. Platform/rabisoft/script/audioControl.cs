using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioControl : MonoBehaviour
{
    public AudioSource mainBgmControl;
    public GameObject check;

    void Update()
    {
        if(check.activeSelf == true)
            mainBgmControl.volume -= Time.deltaTime * 1.0f;
    }
}