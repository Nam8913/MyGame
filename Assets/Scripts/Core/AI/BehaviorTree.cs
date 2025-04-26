using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
    public NodeAbstract rootNode;
    public NodeAbstract.State stateTree = NodeAbstract.State.Inactive;


    protected bool isStarted = false;
    protected bool isTreeCompleted = false;

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
            stateTree = rootNode.GetUpdate();
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

    public BehaviorTree CreateBehaviorTree()
    {
        BehaviorTree behaviorTree = new BehaviorTree();
        behaviorTree.rootNode = new RootNode();
        return behaviorTree;
    }
}
