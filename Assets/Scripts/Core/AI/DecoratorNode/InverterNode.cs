using UnityEngine;

public class InverterNode : DecoratorNode
{
    // InverterNode inverts the result of its child node.
    // If the child node succeeds, the inverter fails, and vice versa.
    // If the child node is running, the inverter also returns running.

    public override string FriendlyToolTipDescription()
    {
        return "Inverter Node: Inverts the result of its child node.";
    }

    protected override void EnterNode()
    {
    }

    protected override void ExitNode()
    {
    }

    protected override State DoUpdateState()
    {
        
        var childState = children.CallUpdate();
        if (childState == State.Running)
        {
            return State.Running;
        }
        else if (childState == State.Success)
        {
            return State.Failure;
        }
        else // childState == State.Failure
        {
            return State.Success;
        }
    }
}