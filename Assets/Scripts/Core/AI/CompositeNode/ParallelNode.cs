using UnityEngine;

public class ParallelNode : CompositeNode
{
    protected override void EnterNode()
    {
        // Initialize the parallel node, if needed
    }

    protected override void ExitNode()
    {
        // Clean up the parallel node, if needed
    }

    protected override State DoUpdateState()
    {
        bool allSuccess = true;
        bool anyRunning = false;

        foreach (var child in children)
        {
            var childState = child.CallUpdate();
            if (childState == State.Running)
            {
                anyRunning = true;
            }
            else if (childState == State.Failure)
            {
                allSuccess = false;
            }
        }

        return allSuccess ? State.Success : (anyRunning ? State.Running : State.Failure);
    }

    public override string FriendlyToolTipDescription()
    {
        return "Parallel Node: Executes all child nodes simultaneously and succeeds if all succeed.";
    }
}
