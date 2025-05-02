using UnityEngine;

public class SelectorNode : CompositeNode
{
    protected override void EnterNode()
    {
        // Initialize the selector node, if needed
    }

    protected override void ExitNode()
    {
        // Clean up the selector node, if needed
    }

    protected override State DoUpdateState()
    {
        foreach (var child in children)
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
        return "Selector Node: Executes child nodes in order until one succeeds or all fail.";
    }
}
