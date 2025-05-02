using UnityEngine;

public class ContextTree : MonoBehaviour
{
    public BehaviorTree behaviorTree;
    public Rigidbody2D rb2d;
    public Collider2D collider2d;

    public static ContextTree SetupContextForTree(BehaviorTree behaviorTree,GameObject owner)
    {
        ContextTree contextTree = owner.GetComponent<ContextTree>();
        if (contextTree == null)
        {
            contextTree = owner.AddComponent<ContextTree>();
        }
        contextTree.behaviorTree = behaviorTree;
        return contextTree;
    }

    void Update()
    {
        if (behaviorTree != null)
        {
            behaviorTree.UpdateTreeBehavior();
        }
        Debug.Log(behaviorTree.stateTree);
    }
    void FixedUpdate()
    {
        if (behaviorTree != null)
        {
            behaviorTree.FixedUpdateTreeBehavior();
        }   
    }
    void LateUpdate()
    {
        if (behaviorTree != null)
        {
            behaviorTree.LateUpdateTreeBehavior();
        }   
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (behaviorTree != null)
        {
            behaviorTree.OnTriggerExit2DForTree(collision);
        }   
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (behaviorTree != null)
        {
            behaviorTree.OnTriggerEnter2DForTree(collision);
        }   
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        if (behaviorTree != null)
        {
            behaviorTree.OnTriggerStay2DForTree(collision);
        }   
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (behaviorTree != null)
        {
            behaviorTree.OnCollisionEnter2DForTree(collision);
        }   
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (behaviorTree != null)
        {
            behaviorTree.OnCollisionExit2DForTree(collision);
        }   
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (behaviorTree != null)
        {
            behaviorTree.OnCollisionStay2DForTree(collision);
        }   
    }

    void OnDrawGizmos()
    {
        if (behaviorTree != null)
        {
            behaviorTree.OnDrawGizmosForTree();
        }   
    }

    void OnDrawGizmosSelected()
    {
        if (behaviorTree != null)
        {
            behaviorTree.OnDrawGizmosSelectedForTree();
        }
    }

    public T OrderOrSetComp<T>(ref T comp) where T : Component
    {
        if (comp == null)
        {
            comp = GetComponent<T>();
            if (comp == null)
            {
                comp = gameObject.AddComponent<T>();
            }
        }
        return comp;
    }

    public T OrderCollider2D<T>(ref T comp) where T : Collider2D
    {
        if (comp == null)
        {
            comp = GetComponent<T>();
            if (comp == null)
            {
                comp = gameObject.AddComponent<T>();
            }
        }
        return comp;
    }
}
