using UnityEngine;

public abstract class SceneAbstract : MonoBehaviour
{
    private bool isSceneDestroyed = false;
    void InitTestDemo()
    {
        ModEngineConfig.Init();
        ModEngineLoader.LoadModProcess();
    }
    public void Awake()
    {
        CurrentGame.GlobalNotifyChangedScene();
        InitTestDemo();
    }
    public virtual void Start()
    {
        
    }
 
    public virtual void Update()
    {
        bool flag;
        EventCall.UpdateEvent(out flag);
        if(flag)
        {
            isSceneDestroyed = true;
        }
        
    }

    public virtual void FixedUpdate()
    {
        if(!isSceneDestroyed)
        {
            // Do something inside ..
        }
    }

    public virtual void LateUpdate()
    {
        if(!isSceneDestroyed)
        {
            // Do something inside ..
        }
    }

    public virtual void OnGUI()
    {
        if(!isSceneDestroyed)
        {
            // Do something inside ..
        }
    }

    private void OnDestroy()
    {
        isSceneDestroyed = true;
    }
}
