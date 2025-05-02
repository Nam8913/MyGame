using UnityEngine;

public class RandomFailureNode : ActionNode
{
    [Range(0,1)] private float failureChance = 0.5f; // 50% chance to fail

    protected override void EnterNode()
    {
        // No specific action on entering this node
    }

    protected override void ExitNode()
    {
        // No specific action on exiting this node
    }

    protected override State DoUpdateState()
    {
        // Randomly determine if the node fails
        if (Random.value < failureChance)
        {
            return State.Failure;
        }
        return State.Success;
    }

    public override string FriendlyToolTipDescription()
    {
        return "Random Failure Node: Randomly fails with a specified chance.";
    }
}