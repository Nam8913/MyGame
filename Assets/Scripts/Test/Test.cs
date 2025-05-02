using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject TestGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        Instantiate(TestGameObject, new Vector3(0, 0, 0), Quaternion.identity);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDrawGizmos()
    {
        
    }
}
