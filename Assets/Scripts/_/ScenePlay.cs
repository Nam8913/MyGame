using System.Collections.Generic;
using UnityEngine;

public class ScenePlay : SceneAbstract
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
         
        ModEngineConfig.Process();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
