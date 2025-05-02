using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
    public RootNode rootNode;
    public NodeAbstract.State stateTree = NodeAbstract.State.Inactive;

    public Blackboard blackboard = new Blackboard();
    
    
    protected bool isStarted = false;
    protected bool isTreeCompleted = false;

    #region All func update for tree
    public NodeAbstract.State UpdateTreeBehavior()
    {
        if (!isStarted)
        {
            isStarted = true;
            Traverse(rootNode, (node) =>
            {
                node.OnTreeEnter();
            });
            if (rootNode.state == NodeAbstract.State.Inactive)
            {
                rootNode.state = NodeAbstract.State.Running;
            }
        }
        if (rootNode.state == NodeAbstract.State.Running)
        {
            stateTree = rootNode.CallUpdate();

        }
        else if (rootNode.state == NodeAbstract.State.Success || rootNode.state == NodeAbstract.State.Failure)
        {
            if (!isTreeCompleted)
            {
                isTreeCompleted = true;
                Traverse(rootNode, (node) =>
                {
                    node.OnTreeExit();
                });
            }
        }
        return stateTree;
    }

    public void LateUpdateTreeBehavior()
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                if(node.state == NodeAbstract.State.Running)
                {
                    node.OnLateUpdate();
                }
            });
        }
    }

    public void FixedUpdateTreeBehavior()
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                if(node.state == NodeAbstract.State.Running)
                node.OnFixedUpdate();
            });
        }
    }

    public void OnTriggerExit2DForTree(Collider2D other)
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                if(node.state == NodeAbstract.State.Running)
                node.OnTriggerExit2D(other);
            });
        }
    }

    public void OnTriggerEnter2DForTree(Collider2D other)
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                if(node.state == NodeAbstract.State.Running)
                node.OnTriggerEnter2D(other);
            });
        }
    }

    public void OnTriggerStay2DForTree(Collider2D other)
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                if(node.state == NodeAbstract.State.Running)
                node.OnTriggerStay2D(other);
            });
        }
    }

    public void OnCollisionExit2DForTree(Collision2D other)
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                if(node.state == NodeAbstract.State.Running)
                node.OnCollisionExit2D(other);
            });
        }
    }

    public void OnCollisionEnter2DForTree(Collision2D other)
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                if(node.state == NodeAbstract.State.Running)
                node.OnCollisionEnter2D(other);
            });
        }
    }

    public void OnCollisionStay2DForTree(Collision2D other)
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                if(node.state == NodeAbstract.State.Running)
                node.OnCollisionStay2D(other);
            });
        }
    }

    public void OnDrawGizmosForTree()
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                node.OnDrawGizmos();
            });
        }
    }

    public void OnDrawGizmosSelectedForTree()
    {
        if (rootNode != null)
        {
            Traverse(rootNode, (node) =>
            {
                node.OnDrawGizmosSelected();
            });
        }
    }

    #endregion
   

    public static void Traverse(NodeAbstract node, System.Action<NodeAbstract> visiter) {
        if (node != null) {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }
    public static List<NodeAbstract> GetChildren(NodeAbstract parent) {
        List<NodeAbstract> children = new List<NodeAbstract>();

        if (parent is DecoratorNode decorator && decorator.children != null) {
            children.Add(decorator.children);
        }

        if (parent is RootNode rootNode && rootNode.children != null) {
            children.Add(rootNode.children);
        }

        if (parent is CompositeNode composite) {
            return composite.children;
        }

        return children;
    }

    public RootNode CreateRootNode()
    {
        rootNode = new RootNode();
        rootNode.SetTree(this);
        rootNode.SetName("RootNode");
        return rootNode;
    }
    

    public BehaviorTree(GameObject owner)
    {
        blackboard.SetContextTree = ContextTree.SetupContextForTree(this, owner);
    }
}
