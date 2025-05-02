using UnityEngine;

public class ParallelSelectorNode : CompositeNode
{
    protected override void EnterNode()
    {
    }

    protected override void ExitNode()
    {
    }

    protected override State DoUpdateState()
    {
        bool anyRunning = false;
        bool anySuccess = false;

        foreach (var child in children)
        {
            var childState = child.CallUpdate();
            if (childState == State.Running)
            {
                anyRunning = true;
            }
            
            if (childState == State.Success)
            {
                anySuccess = true;
                break;
            }
        }

        return anySuccess ? State.Success : (anyRunning ? State.Running : State.Failure);
    }

    public override string FriendlyToolTipDescription()
    {
        return "Parallel Selector Node: Executes all child nodes simultaneously and succeeds if any succeed.";

    }
    
}
