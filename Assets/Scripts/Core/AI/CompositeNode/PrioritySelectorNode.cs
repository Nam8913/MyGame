using System.Collections.Generic;
using UnityEngine;

public class PrioritySelectorNode : CompositeNode
{
    protected override void EnterNode()
    {
        // Initialize the priority selector node, if needed
    }

    protected override void ExitNode()
    {
        // Clean up the priority selector node, if needed
    }

    protected override State DoUpdateState()
    {
        List<NodeAbstract> sortedChildren = new List<NodeAbstract>(children);
        sortedChildren.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        foreach (var child in sortedChildren)
        {
            var childState = child.CallUpdate();
            if (childState == State.Running || childState == State.Success)
            {
                return childState;
            }
        }
        return State.Failure;
    }

    public override string FriendlyToolTipDescription()
    {
        return "Priority Selector Node: Executes child nodes in order of priority until one succeeds or all fail.";
    }
}
