using UnityEngine;

public class RepeaterNode : DecoratorNode
{
    // RepeaterNode repeats the execution of its child node until it succeeds.

    public override string FriendlyToolTipDescription()
    {
        return "Repeater Node: Repeats the execution of its child node until it succeeds.";
    }

    protected override void EnterNode()
    {
    }

    protected override void ExitNode()
    {
    }

    protected override State DoUpdateState()
    {
        while(repeatCount < maxRepeat || endless)
        {
            var childState = children.CallUpdate();
            repeatCount++;
            return State.Running;
        }
        return State.Success; // All attempts failed
    }

    private int repeatCount = 0;
    public int maxRepeat = 1;
    public bool endless = false; // Whether to repeat indefinitely
}