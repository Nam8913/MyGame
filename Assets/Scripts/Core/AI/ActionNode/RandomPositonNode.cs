using UnityEngine;

public class RandomPositionNode : ActionNode
{
    public Vector2 minPosition;
    public Vector2 maxPosition;

    protected override void EnterNode()
    {
        // Set a random position within the specified range
        Vector2 randomPosition = new Vector2(
            Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y)
        );
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
        return "Random Position Node: Sets a random position within the specified range.";
    }
}