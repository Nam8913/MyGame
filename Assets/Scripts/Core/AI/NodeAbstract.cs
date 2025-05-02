using System;
using UnityEngine;

public abstract class NodeAbstract
{
    public enum State
    {
        Inactive,
        Running,
        Failure,
        Success
    }
    public State state = State.Inactive;
    public virtual float Priority { get { return 0f; } }
    

    public State CallUpdate()
    {
        if (!isStarted)
        {
            if(state == State.Inactive)
            {
                state = State.Running;
            }
            EnterNode();
            isStarted = true;
        }
        state = DoUpdateState();
        switch (state)
        {
            case State.Success:
                OnSuccess();
                break;
            case State.Failure:
                OnFailure();
                break;
            case State.Running:
                OnRunning();
                break;
            case State.Inactive:
                OnInactive();
                break;
        }

        if (state == State.Failure || state == State.Success)
        {
            ExitNode();
            isStarted = false;
        }
        return state;
    }


    protected abstract void EnterNode();
    protected abstract void ExitNode();
    public virtual void OnLateUpdate(){}
    public virtual void OnFixedUpdate(){}
    protected abstract State DoUpdateState();
    public virtual void OnTreeEnter(){}
    public virtual void OnTreeExit(){}

    protected virtual void OnSuccess(){}
    protected virtual void OnFailure(){}
    protected virtual void OnRunning(){}
    protected virtual void OnInactive(){}

    public virtual void OnTriggerEnter2D(Collider2D other){}
    public virtual void OnTriggerExit2D(Collider2D other){}
    public virtual void OnTriggerStay2D(Collider2D other){}
    public virtual void OnCollisionEnter2D(Collision2D other){}
    public virtual void OnCollisionExit2D(Collision2D other){}
    public virtual void OnCollisionStay2D(Collision2D other){}

    public virtual void OnDrawGizmos(){}
    public virtual void OnDrawGizmosSelected(){}

    

    public void SetTree(BehaviorTree bhTree)
    {
        owner = bhTree;
    }
    public BehaviorTree GetTree()
    {
        return owner;
    }

    public Blackboard GetBlackboard()
    {
        return owner.blackboard;
    }

    public ContextTree GetContextTree()
    {
        return GetBlackboard().GetContextTree;
    }

    public NodeAbstract CreateChildNode(string nodeName, NodeAbstract node)
    {
        if (node == null)
        {
            Debug.LogError("Node is null");
            return null;
        }
        node.SetName(nodeName);
        node.SetTree(owner);
        if(this is RootNode)
        {
            ((RootNode)this).children = (node);
        }
        else if (this is CompositeNode)
        {
            ((CompositeNode)this).children.Add(node);
        }
        else if (this is DecoratorNode)
        {
            ((DecoratorNode)this).children = (node);
        }

        return node;
    }

    public virtual string FriendlyToolTipDescription()
    {
        return string.Format("{0} - {1}", name, description);
    }

    public void SetName(string newName)
    {
        name = newName;
    }
    public string GetName()
    {
        return name;
    }
    public void SetDescription(string newDescription)
    {
        description = newDescription;
    }
    public string GetDescription()
    {
        return description;
    }
    protected string name;
    protected string description;
    private bool isStarted = false;
    private BehaviorTree owner;

}
