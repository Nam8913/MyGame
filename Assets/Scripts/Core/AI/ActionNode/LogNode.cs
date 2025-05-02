using UnityEngine;

public class LogNode : ActionNode
{
    public string msg = "";

    protected override void EnterNode()
    {
        Debug.Log(msg);
    }

    protected override void ExitNode()
    {
        // Clean up if needed
    }

    protected override State DoUpdateState()
    {
        // Perform any actions needed during the update
        return State.Success;
    }

    public override string FriendlyToolTipDescription()
    {
        return "Log Node: Logs a message when entered.";
    }
}