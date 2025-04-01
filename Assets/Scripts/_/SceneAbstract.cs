using UnityEngine;

public abstract class SceneAbstract : MonoBehaviour
{
    private bool flag_check = true;
    void Init()
    {

    }
    public virtual void Start()
    {
        
    }
 
    public virtual void Update()
    {
        if(flag_check)
        {
            // Do something inside ..
            EventCall.UpdateEvent();
        }
        
    }

    public virtual void FixedUpdate()
    {
        if(flag_check)
        {
            // Do something inside ..
        }
    }

    public virtual void LateUpdate()
    {
        if(flag_check)
        {
            // Do something inside ..
        }
    }

    public virtual void OnGUI()
    {
        if(flag_check)
        {
            // Do something inside ..
        }
    }

    private void OnDestroy()
    {
        flag_check = false;
    }
}
