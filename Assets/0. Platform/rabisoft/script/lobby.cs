using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobby : MonoBehaviour
{
    public GameObject live2d;
    
    public void show()
    {
        live2d.SetActive(true);
    }
    public void hide()
    {
        live2d.SetActive(false);
    }
}
