using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class litTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CreateData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void CreateData () {
        JsonData parent = new JsonData();
        parent["list"] = JsonMapper.ToObject("[]");
        
        
        for(int i=0; i<10;i++) {
        
            JsonData son = new JsonData();
            son["id"] = i;
            son["name"] = i + "_name";
            son["height"] = i * Random.Range(1,100);
            
            parent["list"].Add(son);
        }
        
        Debug.Log(JsonMapper.ToStringUnicode(parent));
    }
}
