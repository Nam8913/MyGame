using UnityEngine;

public class SequenceNode : CompositeNode
{
    protected override void EnterNode()
    {
    }

    protected override void ExitNode()
    {
    }

    protected override State DoUpdateState()
    {
        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            child.CallUpdate();
            if (child.state == State.Inactive)
            {
                child.state = State.Running;
            }
            if (child.state == State.Running)
            {
                return State.Running;
            }
            else if (child.state == State.Failure)
            {
                return State.Failure;
            }
        }
        return State.Success;
    }

    public override string FriendlyToolTipDescription()
    {
        return "Sequence Node: Executes child nodes in order until one fails or all succeed.";
    }
}
