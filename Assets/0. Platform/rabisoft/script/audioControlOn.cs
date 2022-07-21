using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioControlOn : MonoBehaviour
{
    public AudioSource mainBgmControl;
    public GameObject check;

    void Update()
    {
        if(check.activeSelf == false)
            mainBgmControl.volume += Time.deltaTime * 1.0f;
    }
}