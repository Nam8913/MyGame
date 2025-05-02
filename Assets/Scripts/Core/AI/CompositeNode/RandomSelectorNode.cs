using UnityEngine;

public class RandomSelectorNode : CompositeNode
{   
    private int currentIndex = -1;

    protected override void EnterNode()
    {
        // Initialize the random selector node, if needed
    }

    protected override void ExitNode()
    {
        // Clean up the random selector node, if needed
    }

    protected override State DoUpdateState()
    {
        if (children.Count == 0)
        {
            return State.Failure; // No children to select from
        }

        currentIndex = Random.Range(0, children.Count);
        var child = children[currentIndex];
        var childState = child.CallUpdate();

        if (childState == State.Running || childState == State.Success)
        {
            return childState;
        }
        else
        {
            return State.Failure;
        }
    }

    public override string FriendlyToolTipDescription()
    {
        return "Random Selector Node: Executes a random child node each update.";
    }
}
